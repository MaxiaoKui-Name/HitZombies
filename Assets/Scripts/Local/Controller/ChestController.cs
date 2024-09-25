using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using Transform = UnityEngine.Transform;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;
using Hitzb;
using DG.Tweening;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System;
using Random = UnityEngine.Random;
using TMPro;

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
        public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas
        public TextMeshProUGUI ChestBar; // ����Ѫ��
        public TextMeshProUGUI ChestCoinText; // ����Ǯ
        public Vector3 addVector = Vector3.zero;
        public Vector3 ScaleVector  = new Vector3(0.01f, 0.01f, 0.01f);

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
            UpdateHealthBarPosition();
        }

        // ��ʼ������
        private void InitializeChest()
        {
            isMove = true;
            isOpened = false;
            chestHealth = ConfigManager.Instance.Tables.TableBoxgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Boxhp;
            coinTarget = GameObject.Find("CointargetPos").transform;
            healthBarCanvas = transform.Find("ChestTextCanvas").transform;
            foreach (Transform child in healthBarCanvas)
            {
                child.gameObject.SetActive(true);
            }
            addVector.y = 1f;
            ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
            ChestBar = healthBarCanvas.GetChild(0).GetComponent<TextMeshProUGUI>();
            ChestBar.text = $"{chestHealth}";
            ChestCoinText = healthBarCanvas.GetChild(1).GetComponent<TextMeshProUGUI>();
            ChestCoinText.gameObject.SetActive(false);
            mainCamera = Camera.main;
            coinsToSpawn = 0;
            bombDropInterval = 0.25f;
            coinBase = 100f;
            armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
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
        void UpdateHealthBarPosition()
        {
            if (healthBarCanvas != null)
            {

                healthBarCanvas.position = transform.position + addVector;
                healthBarCanvas.localScale = ScaleVector;
            }
        }

        // ����������
        public async void TakeDamage(float damage, GameObject bulletObj)
        {
            if (isOpened) return; // ����Ѿ��򿪣�ֱ�ӷ���
            // ����hit�������ȴ����
            if (armatureComponent != null)
            {
                await PlayAndWaitForAnimation(armatureComponent, "hit1", 1); // ����һ��hit����
            }
            chestHealth -= damage;
            ChestBar.text = $"{Mathf.Max(chestHealth,0)}";
            coinsToSpawn = bulletObj.GetComponent<BulletController>().bulletcost;
            // �������Ѫ��С�ڵ���0�����ſ��䶯�������ɽ�Һͷɻ�
            if (chestHealth <= 0 && !isOpened)
            {
                isOpened = true;
                ChestBar.gameObject.SetActive(false);
                Debug.Log("Chest opened!");
                PreController.Instance.DecrementActiveEnemy();
                // ���ſ��䶯�����ȴ����
                if (armatureComponent != null)
                {
                    await PlayAndWaitForAnimation(armatureComponent, "open", 1); // ����һ�ο��䶯��
                }
                if (chestCollider != null)
                {
                    chestCollider.enabled = false;
                }

                Vector3 deathPosition = transform.position;
                // ���ñ���Ϊ���״̬
                gameObject.SetActive(false); // �������ñ���
                                             // ���ɽ�Һͷɻ����߼�
                GetProbability(deathPosition).Forget();
                SpawnPlane().Forget();
            }
        }

        // ������ʲ������Ƿ����ɽ��
        public async UniTask GetProbability(Vector3 deathPosition)
        {
            int indexCoin = GetCoinIndex();
            coinsToSpawn *= ConfigManager.Instance.Tables.TableBoxcontent.Get(indexCoin).Rewardres;
            int propindex = Random.Range(1, 100);
            if (propindex > (1 - ConfigManager.Instance.Tables.TableBoxgenerate.Get(LevelManager.Instance.levelData.LevelIndex).WeightProp) * 100)
            {
                GetBuffIndex(deathPosition);
            }
        }
        public int GetCoinIndex()
        {
            float randomNum = Random.Range(0f, 100f);
            if (randomNum < 71.45f)
                return 1;
            else if (randomNum > 71.45f && randomNum < 94.45f) // 71.45 + 23
                return 2;
            else if (randomNum > 94.45f && randomNum < 99.45f) // 94.45 + 5
                return 3;
            else if (randomNum > 99.45f && randomNum < 99.95f) // 99.45 + 0.5
                return 4;
            else // If it's less than 100, return 5
                return 5;
        }
        public async UniTask GetBuffIndex(Vector3 deathPosition)
        {
            float randomNum = Random.Range(0f, 100f);
            if (randomNum < 66.6f)
            {
                //TTOD1ִ��ȫ��������Ч��
            }
            else
            {
                //TTODִ��ȫ����ը��Ч��
                await SpawnAndMoveCoins(coinBase, deathPosition);
            }
               
        }

        // ���ɽ�Ҳ��ƶ���UI��ʶ
        public async UniTask SpawnAndMoveCoins(float coinCount, Vector3 deathPosition)
        {
            List<UniTask> coinTasks = new List<UniTask>();

            for (int i = 0; i < coinCount; i++)
            {
                //string coinName = "gold";
                //if (PreController.Instance.CoinPools.TryGetValue(coinName, out var selectedCoinPool))
                //{
                //    GameObject coinObj = selectedCoinPool.Get();
                //    coinObj.transform.position = deathPosition;
                //    coinObj.SetActive(true);

                //    UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                //    if (coinArmature != null)
                //    {
                //        coinArmature.animation.Play("newAnimation",-1);
                //    }

                //    // ���ÿ����ҵ��ƶ�����
                //    coinTasks.Add(MoveCoinToUI(coinObj, selectedCoinPool));
                //}
                string CoinName = "gold";
                if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
                {
                    GameObject coinObj = selectedCoinPool.Get();
                    coinObj.transform.position = deathPosition;
                    UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                    if (coinArmature != null)
                    {
                        coinArmature.animation.Play("newAnimation", -1);
                    }
                    Gold gold = coinObj.GetComponent<Gold>();
                    gold.AwaitMove(selectedCoinPool);
                }
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            }
        }

        //// �ƶ���ҵ�UI��ʶ
        //public async UniTask MoveCoinToUI(GameObject coinObj, ObjectPool<GameObject> coinPool)
        //{
        //    float duration = 0.5f; // ���ӳ���ʱ���Ա�����Ե��ƶ�Ч��
        //    float elapsedTime = 0f;
        //    Vector3 startPosition = coinObj.transform.position;

        //    while (elapsedTime < duration)
        //    {
        //        elapsedTime += Time.deltaTime;
        //        float t = Mathf.Clamp01(elapsedTime / duration);
        //        Vector3 currentPosition = Vector3.Lerp(startPosition, coinTarget.position, t);
        //        currentPosition.z = -0.1f;
        //        coinObj.transform.position = currentPosition;
        //        await UniTask.Yield();
        //    }

        //    if (coinObj.activeSelf)
        //        coinPool.Release(coinObj);
        //    PlayInforManager.Instance.playInfor.AddCoins(1);
       // }

        // ���ɲ��ƶ��ɻ����첽��
        public async UniTask SpawnPlane()
        {
            GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // ���ɷɻ�����Ļ�ײ�
            Debug.Log("Plane spawned!");
            await MovePlaneAndDropBombs(plane);
            PlayInforManager.Instance.playInfor.AddCoins((int)(coinsToSpawn - coinBase));
            if (plane != null)
            {
                Destroy(plane);
            }
            Destroy(gameObject);

        }

        // �ɻ��ƶ���Ͷ��ը�����첽����
        private async UniTask MovePlaneAndDropBombs(GameObject plane)
        {
            float dropTime = 0f;
            while (plane != null && plane.activeSelf && plane.transform.position.y < 6f)
            {
                // Move the plane upwards
                plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
                dropTime += Time.deltaTime;

                // Drop bombs at specified intervals
                if (dropTime >= bombDropInterval)
                {
                    dropTime = 0f;
                    Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
                    DropBomb(bombPosition).Forget();
                }
                // Yield control to allow other operations
                await UniTask.Yield();
            }
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
                    if (!enemyController.isDead)
                    {
                        enemyController.Enemycoins2 = 10;
                        enemyController.TakeDamage(100000, enemy);
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
            ChestCoinText.gameObject.SetActive(true);
            ChestCoinText.text = $"+{coinsToSpawn}";
            await UniTask.Delay(200);
            ChestCoinText.gameObject.SetActive(false);
        }
    }
}
