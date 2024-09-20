using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using Transform = UnityEngine.Transform;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;
using Hitzb;
using DG.Tweening;

namespace Hitzb
{
    public class ChestController : MonoBehaviour
    {
        [Header("Chest Properties")]
        public float chestHealth = 100;  // ����ĳ�ʼѪ��
        public Transform coinTarget;   // ���Ŀ�꣨����UI�ϵĽ��λ�ã�
        public float coinsToSpawn = 5;   // ���ɵĽ������
        public UnityArmatureComponent armatureComponent;
        private bool isOpened = false; // ��Ǳ����Ƿ��Ѿ���

        [Header("Movement Properties")]
        public float moveSpeed = 1f; // �������������ƶ����ٶ�
        public float hideYPosition = -10f; // ������Ļ��Y����
        public bool isMove = false;

        [Header("Spawn Properties")]
        public GameObject planePrefab;  // �ɻ���Ԥ����
        public GameObject bombPrefab;   // ը����Ԥ����
        public float planeSpeed = 2f;   // �ɻ��ƶ��ٶ�
        public float bombDropInterval;  // ը��Ͷ�����ʱ��
        public float coinBase = 100f;   // �����������

        private Collider2D chestCollider; // �������ײ��
        private Camera mainCamera;

        void OnEnable()
        {
            InitializeChest();
            StartCoroutine(StartAnimation());
        }

        IEnumerator StartAnimation()
        {
            yield return new WaitForSeconds(0.5f); // �ȴ�0.5�룬ȷ����������ѳ�ʼ��

            // ���ų�ʼ����
            if (armatureComponent != null)
            {
                armatureComponent.animation.Play("breath",-1);
            }
        }

        void Update()
        {
            if (isMove)
            {
                MoveDown();
            }

            if (transform.position.y < hideYPosition)
            {
                isMove = false;
                Destroy(gameObject);
            }
        }

        // ��ʼ������
        private void InitializeChest()
        {
            isMove = true;
            chestHealth = ConfigManager.Instance.Tables.TableBoxgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Boxhp;
            coinTarget = GameObject.Find("CointargetPos").transform;
            mainCamera = Camera.main;
            coinsToSpawn = 0;
            bombDropInterval = 0.25f;
            coinBase = 20f;
            armatureComponent = GetComponent<UnityArmatureComponent>();
            chestCollider = GetComponent<Collider2D>(); // ��ȡ��ײ�����
            if (chestCollider != null)
            {
                chestCollider.enabled = true; // ȷ����ײ������
            }
        }

        // ���������ƶ�
        private void MoveDown()
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }

        // ����������
        public async void TakeDamage(float damage, GameObject bulletObj)
        {
            if (isOpened) return; // ����Ѿ��򿪣�ֱ�ӷ���

            // ����hit�������ȴ����
            if (armatureComponent != null)
            {
                armatureComponent.animation.Play("hit1", 1);  // ����һ��hit����
            }
            chestHealth -= damage;
            coinsToSpawn = bulletObj.GetComponent<BulletController>().bulletcost;
            // �������Ѫ��С�ڵ���0�����ſ��䶯�������ɽ�Һͷɻ�
            if (chestHealth <= 0 && !isOpened)
            {
                isOpened = true;
                Debug.Log("Chest opened!");

                // ���ſ��䶯�����ȴ����
                if (armatureComponent != null)
                {
                    await PlayAndWaitForAnimation(armatureComponent, "open", 1); // ����һ�ο��䶯��
                }

                // ������ײ�壬��ֹ��һ������ײ�¼�
                if (chestCollider != null)
                {
                    chestCollider.enabled = false;
                }

                Vector3 deathPosition = transform.position;

                // ���ɽ�Һͷɻ����߼�
                GetProbability(deathPosition).Forget();
                SpawnPlane().Forget();

                // ���ñ���Ϊ���״̬
                gameObject.SetActive(false); // �������ñ���
            }
        }

        // ������ʲ������Ƿ����ɽ��
        public async UniTask GetProbability(Vector3 deathPosition)
        {
            float probabilityTotal = 0f;
            for (int i = 0; i < ConfigManager.Instance.Tables.TableBoxcontent.DataMap.Count; i++)
            {
                probabilityTotal += ConfigManager.Instance.Tables.TableBoxcontent.Get(i + 1).Weight;
            }
            float probability = ConfigManager.Instance.Tables.TableBoxcontent.Get(8).Weight / probabilityTotal;
            int randomNum = Random.Range(1, 100);
            Debug.Log($"Probability: {probability * 100}%, Random Num: {randomNum}");
            if (randomNum < probability * 100)
            {
                coinsToSpawn *= ConfigManager.Instance.Tables.TableBoxcontent.Get(8).Rewardres;
                await SpawnAndMoveCoins(coinBase, deathPosition);
            }
        }

        // ���ɽ�Ҳ��ƶ���UI��ʶ
        public async UniTask SpawnAndMoveCoins(float coinCount, Vector3 deathPosition)
        {
            List<UniTask> coinTasks = new List<UniTask>();

            for (int i = 0; i < coinCount; i++)
            {
                string coinName = "gold";
                if (PreController.Instance.CoinPools.TryGetValue(coinName, out var selectedCoinPool))
                {
                    GameObject coinObj = selectedCoinPool.Get();
                    coinObj.transform.position = deathPosition;
                    coinObj.SetActive(true);

                    UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                    if (coinArmature != null)
                    {
                        coinArmature.animation.Play("newAnimation",-1);
                    }

                    // ���ÿ����ҵ��ƶ�����
                    coinTasks.Add(MoveCoinToUI(coinObj, selectedCoinPool));
                }
            }

            // �ȴ����н���ƶ����
            await UniTask.WhenAll(coinTasks);
        }

        // �ƶ���ҵ�UI��ʶ
        public async UniTask MoveCoinToUI(GameObject coinObj, ObjectPool<GameObject> coinPool)
        {
            float duration = 0.5f; // ���ӳ���ʱ���Ա�����Ե��ƶ�Ч��
            float elapsedTime = 0f;
            Vector3 startPosition = coinObj.transform.position;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, coinTarget.position, t);
                currentPosition.z = -0.1f;
                coinObj.transform.position = currentPosition;
                await UniTask.Yield();
            }

            if (coinObj.activeSelf)
                coinPool.Release(coinObj);
            PlayInforManager.Instance.playInfor.AddCoins(1);
        }

        // ���ɲ��ƶ��ɻ����첽��
        public async UniTask SpawnPlane()
        {
            GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // ���ɷɻ�����Ļ�ײ�
            Debug.Log("Plane spawned!");
            await MovePlaneAndDropBombs(plane);
            PlayInforManager.Instance.playInfor.AddCoins((int)(coinsToSpawn - coinBase));
        }

        // �ɻ��ƶ���Ͷ��ը�����첽����
        private async UniTask MovePlaneAndDropBombs(GameObject plane)
        {
            float dropTime = 0f;
            while (plane.transform.position.y < 6f)
            {
                plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
                dropTime += Time.deltaTime;

                if (dropTime >= bombDropInterval)
                {
                    dropTime = 0f;
                    Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
                    DropBomb(bombPosition).Forget();
                }

                await UniTask.Yield();
            }
            Destroy(plane);
            Destroy(gameObject);
        }

        // Ͷ��ը�����첽��
        private async UniTask DropBomb(Vector3 planePosition)
        {
            GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
            await BombExplosion(bomb);
        }

        // ը����ը��������������ˣ��첽��
        private async UniTask BombExplosion(GameObject bomb)
        {
            await UniTask.Delay(1000);

            UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
            if (bombArmature != null)
            {
                bombArmature.animation.Play("fly", 1); // ����һ�η��ж���
            }

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy.activeSelf)
                {
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();
                    if (enemyController.health > 0 && enemyController.IsEnemyOnScreen(enemy))
                    {
                        enemyController.Enemycoins2 = 2;
                        enemyController.Die(enemy);
                    }
                }
            }
            await UniTask.Delay(1000);
            Destroy(bomb);
        }

        // �ȴ������������
        private async UniTask PlayAndWaitForAnimation(UnityArmatureComponent armature, string animationName, int playTimes = 1)
        {
            var tcs = new UniTaskCompletionSource();

            // �����¼��������
            void OnAnimationComplete(string type, EventObject eventObject)
            {
                if (eventObject.animationState.name == animationName)
                {
                    armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);  // �Ƴ�������
                    tcs.TrySetResult(); // �������
                }
            }

            // ����¼�������
            armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);

            // ����ָ����������ָ�����Ŵ���
            armature.animation.Play(animationName, playTimes);

            // �ȴ��������
            await tcs.Task;
        }
    }
}
