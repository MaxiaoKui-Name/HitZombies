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
using UnityEngine.UI;
using System.Diagnostics.Eventing.Reader;
using System.Threading; // �������� CancellationToken �������ռ�

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
        public Text ChestCoinText; // ����Ǯ
        public Vector3 addVector = Vector3.zero;
        public Vector3 ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
        public bool isVise;
        public GameMainPanelController gameMainPanelController;
        public bool isFrozen;

        private CancellationTokenSource cts; // ��� CancellationTokenSource

        void OnEnable()
        {
            //EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
            cts = new CancellationTokenSource(); // ��ʼ�� CancellationTokenSource
            InitializeChest();
            StartCoroutine(StartAnimation());
        }

        private void RecycleEnemy(GameObject gameObject)
        {
            isMove = false;
            if (gameObject != null && gameObject.activeSelf)
            {
                cts?.Cancel(); // ȡ���κ����ڽ��е�����
                cts?.Dispose(); // �ͷ� CancellationTokenSource
                //EventDispatcher.instance.UnRegist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
                Destroy(gameObject);
            }
        }

        IEnumerator StartAnimation()
        {
            yield return new WaitForSeconds(0.5f); // �ȴ�0.5�룬ȷ����������ѳ�ʼ��

            // ���ų�ʼ����
            if (armatureComponent != null)
            {
                armatureComponent.animation.Play("breath", -1);
            }
        }

        void Update()
        {
            if (isFrozen || GameManage.Instance.isFrozen)
            {
                return;
            }
            if (GameManage.Instance.gameState != GameState.Running)
            {
                cts?.Cancel(); // ȡ���κ����ڽ��е�����
                cts?.Dispose(); // �ͷ� CancellationTokenSource
                Destroy(gameObject);
                return; // ����ʱ��ִ���κ��߼�
            }
            if (isMove)
            {
                MoveDown();
            }

            if (transform.position.y < hideYPosition)
            {
                isMove = false;
                RecycleEnemy(gameObject);
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
            transform.localScale = Vector3.one * initialScale;
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
            ChestBar.text = $"{Mathf.Max(chestHealth, 0)}";
            //ChestCoinText = healthBarCanvas.GetChild(1).GetComponent<Text>();
            //ChestCoinText.gameObject.SetActive(false);
            mainCamera = Camera.main;
            coinsToSpawn = 0;
            bombDropInterval = 0.25f;
            coinBase = 10f;
            armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
            chestCollider = GetComponent<Collider2D>(); // ��ȡ��ײ�����
            if (chestCollider != null)
            {
                chestCollider.enabled = true; // ȷ����ײ������
            }
        }
        private float initialScale = 0.16f; // Initial chest scale
        private float targetScale = 0.25f; // Target chest scale

        // ���������ƶ�
        private void MoveDown()
        {
            transform.Translate(Vector3.down * InfiniteScroll.Instance.scrollSpeed * Time.deltaTime);
            if (isVise)
            {
                float currentScale = transform.localScale.x; // Assuming uniform scaling on all axes
                if (currentScale < targetScale)
                {
                    float scaleFactor = InfiniteScroll.Instance.growthRate * Time.deltaTime;
                    float newScale = Mathf.Min(currentScale + scaleFactor, targetScale); // Ensure the scale doesn't exceed the target scale
                                                                                         // Apply the new scale uniformly
                    transform.localScale = new Vector3(newScale, newScale, newScale);
                }
            }
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
            if (cts.IsCancellationRequested) return; // ����Ƿ�����ȡ��

            chestHealth -= damage;
            ChestBar.text = $"{Mathf.Max(chestHealth, 0)}";
            coinsToSpawn = bulletObj.GetComponent<BulletController>().bulletcost;

            // ���� hit �������ȴ����
            if (armatureComponent != null && isFinishHit)
            {
                isFinishHit = false;
                try
                {
                    await PlayAndWaitForAnimation(armatureComponent, "hit", 1).AttachExternalCancellation(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // ����ȡ������������Ҫ��
                    return;
                }
            }

            // �������Ѫ��С�ڵ���0�����ſ��䶯�������ɽ�Һͷɻ�
            if (chestHealth <= 0 && !isOpened)
            {
                isOpened = true;
                ChestBar.gameObject.SetActive(false);
                Debug.Log("Chest opened!");
                PreController.Instance.DecrementActiveEnemy();
                Vector3 deathPosition = transform.position;
                // Fire and forget������ȡ������
                GetProbability(deathPosition).Forget(); // �ɿ��������ﴦ��ȡ��
                // ���ſ��䶯�����ȴ����
                if (armatureComponent != null)
                {
                    try
                    {
                        await PlayAndWaitForAnimation(armatureComponent, "open", 1).AttachExternalCancellation(cts.Token); // ����һ�ο��䶯��
                        armatureComponent.animation.Play("open_stay", -1); // ����һ�ο��䶯��
                    }
                    catch (OperationCanceledException)
                    {
                        // ����ȡ������������Ҫ��
                        return;
                    }
                }
                if (chestCollider != null)
                {
                    chestCollider.enabled = false;
                }
            }
        }

        public int indexChest;
        public bool ChestGuid = false;

        public async UniTask GetProbability(Vector3 deathPosition)
        {
            if (cts.IsCancellationRequested) return; // ����Ƿ�����ȡ��

            if (GameFlowManager.Instance.currentLevelIndex == 0)
            {
                if (PreController.Instance.BoxNumWave == 5)
                {
                    indexChest = 1;
                    ChestGuid = true;
                }
                if (PreController.Instance.BoxNumWave == 6)
                    indexChest = 2;
            }
            else
            {
                indexChest = GetCoinIndex();
            }
            coinsToSpawn *= ConfigManager.Instance.Tables.TableBoxcontent.Get(indexChest).Rewardres;

            // ���浱ǰ�Ķ���״̬�������Ҫ��
            string currentAnimation = armatureComponent?.animation?.lastAnimationName;
            string newArmatureName = ConfigManager.Instance.Tables.TableBoxcontent.Get(indexChest).Name;
            Debug.Log("����indexChest����=========" + indexChest);
            Debug.Log("���䶯������=========" + newArmatureName);

            // �ͷŵ�ǰ�� armature
            if (armatureComponent != null)
            {
                armatureComponent.armature.Dispose();
            }

            // ʹ���µ� armatureName ���¹����Ǽ�
            armatureComponent = UnityFactory.factory.BuildArmatureComponent(newArmatureName, "������", transform.GetChild(0).gameObject.name);
            armatureComponent.transform.parent = gameObject.transform;
            armatureComponent.transform.localPosition = Vector3.zero;
            armatureComponent.transform.localScale = Vector3.one * 0.5f;

            // ��� armatureComponent �Ƿ�ɹ�����
            if (armatureComponent != null)
            {
                // �ָ�֮ǰ�Ķ���״̬���򲥷��¶���
                if (!string.IsNullOrEmpty(currentAnimation) && armatureComponent.animation.HasAnimation(currentAnimation))
                {
                    armatureComponent.animation.Play(currentAnimation);
                }
                else
                {
                    armatureComponent.animation.Play("breath"); // ���û�б���Ķ���������Ĭ�϶���
                }
            }
            else
            {
                Debug.LogError("Failed to create armatureComponent for: " + newArmatureName);
                return; // �������ʧ�ܣ���ǰ����
            }

            if (GameFlowManager.Instance.currentLevelIndex == 0)
            {
                if (PreController.Instance.BoxNumWave == 5)
                {
                    //gameMainPanelController.buffBlastBtn.interactable = true;
                    PlayInforManager.Instance.playInfor.BalstBuffCount++;
                }
                if (PreController.Instance.BoxNumWave == 6)
                {
                   // gameMainPanelController.buffFrozenBtn.interactable = true;
                    PlayInforManager.Instance.playInfor.FrozenBuffCount++;
                }
            }
            else
            {
                int propindex = Random.Range(1, 100);
                if (propindex > 100 - ConfigManager.Instance.Tables.TableBoxgenerate.Get(LevelManager.Instance.levelData.LevelIndex).WeightProp)
                {
                    GetBuffIndex(deathPosition).Forget(); // �ɿ��������ﴦ��ȡ��
                }
            }

            await SpawnAndMoveCoins(coinBase, deathPosition).AttachExternalCancellation(cts.Token);
            PlayInforManager.Instance.playInfor.AddCoins((int)(coinsToSpawn - coinBase));
            gameMainPanelController.UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);

            if (GameFlowManager.Instance.currentLevelIndex == 0 && ChestGuid)
            {
                ChestGuid = false;
                gameMainPanelController.ShowSkillGuide(); // ������ʾ������ʾ�ķ���
            }
        }

        public int GetCoinIndex()
        {
            int randomNum = Random.Range(1, 100);
            Debug.Log("����������" + randomNum);
            var coinindexConfig = ConfigManager.Instance.Tables.TableBoxcontent;
            if (randomNum < coinindexConfig.Get(1).Probability)
                return 1;
            else if (randomNum > coinindexConfig.Get(1).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability) // 71.45 + 23
                return 2;
            else if (randomNum > coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability) // 94.45 + 5
                return 3;
            else if (randomNum > coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability + coinindexConfig.Get(4).Probability) // 99.45 + 0.5
                return 4;
            else // ���С��100������5
                return 5;
        }

        // ���ӵ� Buff �߼�
        public async UniTask GetBuffIndex(Vector3 deathPosition)
        {
            if (cts.IsCancellationRequested) return; // ����Ƿ�����ȡ��

            var BuffIndexConfig = ConfigManager.Instance.Tables.TableBoxcontent;
            int randomNum = Random.Range(0, (int)(BuffIndexConfig.Get(7).Probability + BuffIndexConfig.Get(6).Probability));
            if (randomNum < BuffIndexConfig.Get(6).Probability)
            {
                // ����ȫ����������
                PlayInforManager.Instance.playInfor.FrozenBuffCount++;
            }
            else
            {
                // ����ȫ����ը����
                PlayInforManager.Instance.playInfor.BalstBuffCount++;
                // SpawnPlane().Forget();
            }
        }

        // ���ɽ�Ҳ��ƶ���UI��ʶ
        public async UniTask SpawnAndMoveCoins(float coinCount, Vector3 deathPosition)
        {
            if (cts.IsCancellationRequested) return; // ����Ƿ�����ȡ��

            List<UniTask> coinTasks = new List<UniTask>();
            for (int i = 0; i < coinCount; i++)
            {
                if (cts.IsCancellationRequested) break; // ���ȡ������ǰ�˳�

                string CoinName = "gold";
                Debug.Log(transform.gameObject.name + "�����Ľ��" + i);
                if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
                {
                    GameObject coinObj = selectedCoinPool.Get();
                    coinObj.SetActive(true);
                    coinObj.transform.position = deathPosition;
                    UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                    if (coinArmature != null)
                    {
                        coinArmature.animation.Play("newAnimation", -1);
                    }
                    Gold gold = coinObj.GetComponent<Gold>();
                    Transform CointargetPos = GameObject.Find("CointargetPos").transform;
                    gold.AwaitMove(selectedCoinPool, CointargetPos).Forget(); // ���� AwaitMove �ڲ�����ȡ��
                }
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // ����ȡ������������Ҫ��
                    break;
                }
            }
        }

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

            // �ȴ�������ɲ�����ȡ������
            try
            {
                await tcs.Task.AttachExternalCancellation(cts.Token);
            }
            catch (OperationCanceledException)
            {
                armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete); // ȷ���Ƴ�������
                return;
            }

            if (animationName == "hit")
                isFinishHit = true;

            if (animationName == "open")
            {
                GameObject ChestObj = Instantiate(LevelManager.Instance.levelData.ChestUIList[indexChest - 1]);
                if (ChestObj != null)
                {
                    ChestObj.transform.position = LevelManager.Instance.levelData.ChestPoints;
                    ChestObj.transform.localScale = Vector3.one * 1.2f;
                    ChestObj.transform.localRotation = Quaternion.identity;
                    ChestObj.transform.Find("ChestText").transform.position = ChestObj.transform.position;
                    ChestObj.transform.Find("ChestText").transform.localScale = ScaleVector;
                    ChestCoinText = ChestObj.transform.Find("ChestText/ChestCoinText").GetComponent<Text>();
                    ChestCoinText.gameObject.SetActive(false);
                    // ���� "out" ������ͬ�� ChestCoinText
                    await PlayCoinTextAnimation(ChestObj.transform.GetComponentInChildren<UnityArmatureComponent>()).AttachExternalCancellation(cts.Token);
                }
                else
                {
                    Debug.LogError("Failed to create new Armature");
                }
            }
        }

        // ���� ChestCoinText ����
        private async UniTask PlayCoinTextAnimation(UnityArmatureComponent newArmature)
        {
            if (cts.IsCancellationRequested) return; // ����Ƿ�����ȡ��

            ChestCoinText.gameObject.SetActive(true);
            ChestCoinText.text = coinsToSpawn.ToString("N0");
            // ���� "out" �������ڼ� ChestCoinText �𽥱��
            newArmature.animation.Play("out", 1);
            ChestCoinText.transform.DOScale(Vector3.one * 0.5f, 0.5f); // �𽥱��1.5��

            try
            {
                await UniTask.Delay(500, cancellationToken: cts.Token); // �ȴ��������
            }
            catch (OperationCanceledException)
            {
                ChestCoinText.gameObject.SetActive(false);
                if (newArmature.transform.parent.gameObject != null && newArmature.transform.parent.gameObject.activeSelf)
                {
                    //EventDispatcher.instance.UnRegist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
                    Destroy(newArmature.transform.parent.gameObject);
                }
                //Destroy(newArmature.transform.parent.gameObject);
                return;
            }

            // ���ִ�С���䣬���� "stay" ����
            newArmature.animation.Play("stay", 3);
            try
            {
                await UniTask.Delay(3000, cancellationToken: cts.Token); // ͣ��һ��ʱ��
            }
            catch (OperationCanceledException)
            {
                ChestCoinText.gameObject.SetActive(false);
                if (newArmature.transform.parent.gameObject != null && newArmature.transform.parent.gameObject.activeSelf)
                {
                    //EventDispatcher.instance.UnRegist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
                    Destroy(newArmature.transform.parent.gameObject);
                }
                //Destroy(newArmature.transform.parent.gameObject);
                return;
            }

            // ���� "end" �������ڼ� ChestCoinText ����С����ʧ
            newArmature.animation.Play("end", 1);
            ChestCoinText.transform.DOScale(Vector3.zero, 0.5f); // ��С��0
            try
            {
                await UniTask.Delay(500, cancellationToken: cts.Token); // �ȴ���������
            }
            catch (OperationCanceledException)
            {
                ChestCoinText.gameObject.SetActive(false);
                if (newArmature.transform.parent.gameObject != null && newArmature.transform.parent.gameObject.activeSelf)
                {
                    //EventDispatcher.instance.UnRegist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
                    Destroy(newArmature.transform.parent.gameObject);
                }
                //Destroy(newArmature.transform.parent.gameObject);
                return;
            }

            ChestCoinText.gameObject.SetActive(false);
            if (newArmature.transform.parent.gameObject != null && newArmature.transform.parent.gameObject.activeSelf)
            {
                //EventDispatcher.instance.UnRegist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
                Destroy(newArmature.transform.parent.gameObject);
            }
        }
    }
}
