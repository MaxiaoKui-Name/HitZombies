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
using System.ComponentModel;

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
        public float moveSpeed; // �������������ƶ����ٶ�
        public float hideYPosition = -10f; // ������Ļ��Y����
        public bool isMove = false;

        [Header("Spawn Properties")]
        public float bombDropInterval;  // ը��Ͷ�����ʱ��
        public float coinBase = 100f;   // �����������

        private Collider2D chestCollider; // �������ײ��
        private Camera mainCamera;
        public Transform healthBarCanvas; // Ѫ�����ڵ�Canvas
        public TextMeshProUGUI ChestBar; // ����Ѫ��
        public TextMeshProUGUI ChestCoinText; // ����Ǯ
        public Vector3 addVector = Vector3.zero;
        public Vector3 ScaleVector  = new Vector3(0.01f, 0.01f, 0.01f);
        public bool isVise;
        public GameMainPanelController gameMainPanelController;
        public bool isFrozen;

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

            if (isFrozen)
            {
                return;
            }
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
            isFinishHit = true;
            isVise = false;
            moveSpeed = ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue; 
            chestHealth = ConfigManager.Instance.Tables.TableBoxgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Boxhp;
            coinTarget = GameObject.Find("CointargetPos").transform;
            healthBarCanvas = transform.Find("ChestTextCanvas").transform;
            gameMainPanelController = GameObject.Find("UICanvas/GameMainPanel(Clone)").GetComponent<GameMainPanelController>();
            foreach (Transform child in healthBarCanvas)
            {
                child.gameObject.SetActive(true);
            }
            addVector.y = 1f;
            ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
            ChestBar = healthBarCanvas.GetChild(0).GetComponent<TextMeshProUGUI>();
            ChestBar.text = $"{Mathf.Max(Mathf.FloorToInt(chestHealth / 50f), 0)}";
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
        private bool isFinishHit;
        // ����������
        public async void TakeDamage(float damage, GameObject bulletObj)
        {
            if (isOpened) return; // ����Ѿ��򿪣�ֱ�ӷ���
            chestHealth -= damage;
            ChestBar.text = $"{Mathf.Max(Mathf.FloorToInt(chestHealth / 50f), 0)}";
            coinsToSpawn = bulletObj.GetComponent<BulletController>().bulletcost;
            // ����hit�������ȴ����
            if (armatureComponent != null && isFinishHit)
            {
                isFinishHit = false;
                await PlayAndWaitForAnimation(armatureComponent, "hit1", 1); // ����һ��hit����
            }
            // �������Ѫ��С�ڵ���0�����ſ��䶯�������ɽ�Һͷɻ�
            if (chestHealth <= 0 && !isOpened)
            {
                isOpened = true;
                ChestBar.gameObject.SetActive(false);
                Debug.Log("Chest opened!");
                PreController.Instance.DecrementActiveEnemy();
                Vector3 deathPosition = transform.position;
                GetProbability(deathPosition).Forget();
                // ���ſ��䶯�����ȴ����
                if (armatureComponent != null)
                {
                    await PlayAndWaitForAnimation(armatureComponent, "open", 1); // ����һ�ο��䶯��
                }
                if (chestCollider != null)
                {
                    chestCollider.enabled = false;
                }
                // ���ñ���Ϊ���״̬
                gameObject.SetActive(false); // �������ñ���
               
            }
        }

        // ������ʲ������Ƿ����ɽ��
        public async UniTask GetProbability(Vector3 deathPosition)
        {
            int indexCoin = GetCoinIndex();
            coinsToSpawn = coinsToSpawn * ConfigManager.Instance.Tables.TableBoxcontent.Get(indexCoin).Rewardres;
            float propindex = Random.Range(1f, 100f);
            if (propindex > (1 - ConfigManager.Instance.Tables.TableBoxgenerate.Get(LevelManager.Instance.levelData.LevelIndex).WeightProp) * 100)
            {
                GetBuffIndex(deathPosition);
            }
            await SpawnAndMoveCoins(coinBase, deathPosition);
            PlayInforManager.Instance.playInfor.AddCoins((int)(coinsToSpawn - coinBase));
            Destroy(gameObject);
        }
        public int GetCoinIndex()
        {
            float randomNum = Random.Range(0f, 100f);
            var coinindexConfig = ConfigManager.Instance.Tables.TableBoxcontent;
            if (randomNum < coinindexConfig.Get(1).Probability)
                return 1;
            else if (randomNum > coinindexConfig.Get(1).Probability && randomNum < coinindexConfig.Get(2).Probability) // 71.45 + 23
                return 2;
            else if (randomNum > coinindexConfig.Get(2).Probability && randomNum < coinindexConfig.Get(3).Probability) // 94.45 + 5
                return 3;
            else if (randomNum > coinindexConfig.Get(3).Probability && randomNum < coinindexConfig.Get(4).Probability) // 99.45 + 0.5
                return 4;
            else // If it's less than 100, return 5
                return 5;
        }
        //���ӵ�Buff�߼�
        public async UniTask GetBuffIndex(Vector3 deathPosition)
        {
            float randomNum = Random.Range(0f, 100f);
            var BuffIndexConfig = ConfigManager.Instance.Tables.TableBoxcontent;
            if (randomNum < BuffIndexConfig.Get(6).Probability)
            {
                //TTOD1ȫ������������1
                PlayInforManager.Instance.playInfor.FrozenBuffCount++;
            }
            else
            {
                //TTOD1ȫ����ը������1
                PlayInforManager.Instance.playInfor.BalstBuffCount++;
                //SpawnPlane().Forget();
            }
            gameMainPanelController.UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
        }

        // ���ɽ�Ҳ��ƶ���UI��ʶ
        public async UniTask SpawnAndMoveCoins(float coinCount, Vector3 deathPosition)
        {
            List<UniTask> coinTasks = new List<UniTask>();

            for (int i = 0; i < coinCount; i++)
            {
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
            if(animationName == "hit1")
               isFinishHit = true;
            if (animationName == "open")
            {
                ChestCoinText.gameObject.SetActive(true);
                ChestCoinText.text = $"+{coinsToSpawn}";
                await UniTask.Delay(200);
                ChestCoinText.gameObject.SetActive(false);
            }
        }
    }
}
