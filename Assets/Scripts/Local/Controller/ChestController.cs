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
using System.Threading; // 引入用于 CancellationToken 的命名空间

namespace Hitzb
{
    public class ChestController : MonoBehaviour
    {
        [Header("Chest Properties")]
        public float chestHealth = 100;  // 宝箱的初始血量
        public Transform coinTarget;   // 金币目标（比如UI上的金币位置）
        public float coinsToSpawn = 5;   // 生成的金币数量
        public UnityArmatureComponent armatureComponent;
        private bool isOpened = false; // 标记宝箱是否已经打开

        [Header("Movement Properties")]
        public float moveSpeed; // 设置物体向下移动的速度
        public float hideYPosition = -10f; // 超出屏幕的Y坐标
        public bool isMove = false;

        [Header("Spawn Properties")]
        public float bombDropInterval;  // 炸弹投掷间隔时间
        public float coinBase = 100f;   // 基础金币数量

        private Collider2D chestCollider; // 宝箱的碰撞体
        private Camera mainCamera;
        public Transform healthBarCanvas; // 血条所在的Canvas
        public TextMeshProUGUI ChestBar; // 宝箱血量
        public Text ChestCoinText; // 宝箱钱
        public Vector3 addVector = Vector3.zero;
        public Vector3 ScaleVector = new Vector3(0.01f, 0.01f, 0.01f);
        public bool isVise;
        public GameMainPanelController gameMainPanelController;
        public bool isFrozen;

        private CancellationTokenSource cts; // 添加 CancellationTokenSource

        void OnEnable()
        {
            //EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
            cts = new CancellationTokenSource(); // 初始化 CancellationTokenSource
            InitializeChest();
            StartCoroutine(StartAnimation());
        }

        private void RecycleEnemy(GameObject gameObject)
        {
            isMove = false;
            if (gameObject != null && gameObject.activeSelf)
            {
                cts?.Cancel(); // 取消任何正在进行的任务
                cts?.Dispose(); // 释放 CancellationTokenSource
                //EventDispatcher.instance.UnRegist(EventNameDef.GAME_OVER, (v) => RecycleEnemy(gameObject));
                Destroy(gameObject);
            }
        }

        IEnumerator StartAnimation()
        {
            yield return new WaitForSeconds(0.5f); // 等待0.5秒，确保动画组件已初始化

            // 播放初始动画
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
                cts?.Cancel(); // 取消任何正在进行的任务
                cts?.Dispose(); // 释放 CancellationTokenSource
                Destroy(gameObject);
                return; // 冻结时不执行任何逻辑
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

        // 初始化宝箱
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
            chestCollider = GetComponent<Collider2D>(); // 获取碰撞体组件
            if (chestCollider != null)
            {
                chestCollider.enabled = true; // 确保碰撞体启用
            }
        }
        private float initialScale = 0.16f; // Initial chest scale
        private float targetScale = 0.25f; // Target chest scale

        // 物体向下移动
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

        // 处理宝箱受伤
        public async void TakeDamage(float damage, GameObject bulletObj)
        {
            if (isOpened) return; // 如果已经打开，直接返回
            if (cts.IsCancellationRequested) return; // 检查是否请求取消

            chestHealth -= damage;
            ChestBar.text = $"{Mathf.Max(chestHealth, 0)}";
            coinsToSpawn = bulletObj.GetComponent<BulletController>().bulletcost;

            // 播放 hit 动画并等待完成
            if (armatureComponent != null && isFinishHit)
            {
                isFinishHit = false;
                try
                {
                    await PlayAndWaitForAnimation(armatureComponent, "hit", 1).AttachExternalCancellation(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // 处理取消情况（如果需要）
                    return;
                }
            }

            // 如果宝箱血量小于等于0，播放开箱动画并生成金币和飞机
            if (chestHealth <= 0 && !isOpened)
            {
                isOpened = true;
                ChestBar.gameObject.SetActive(false);
                Debug.Log("Chest opened!");
                PreController.Instance.DecrementActiveEnemy();
                Vector3 deathPosition = transform.position;
                // Fire and forget，传递取消令牌
                GetProbability(deathPosition).Forget(); // 可考虑在这里处理取消
                // 播放开箱动画并等待完成
                if (armatureComponent != null)
                {
                    try
                    {
                        await PlayAndWaitForAnimation(armatureComponent, "open", 1).AttachExternalCancellation(cts.Token); // 播放一次开箱动画
                        armatureComponent.animation.Play("open_stay", -1); // 播放一次开箱动画
                    }
                    catch (OperationCanceledException)
                    {
                        // 处理取消情况（如果需要）
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
            if (cts.IsCancellationRequested) return; // 检查是否请求取消

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

            // 保存当前的动画状态（如果需要）
            string currentAnimation = armatureComponent?.animation?.lastAnimationName;
            string newArmatureName = ConfigManager.Instance.Tables.TableBoxcontent.Get(indexChest).Name;
            Debug.Log("宝箱indexChest数字=========" + indexChest);
            Debug.Log("宝箱动画名字=========" + newArmatureName);

            // 释放当前的 armature
            if (armatureComponent != null)
            {
                armatureComponent.armature.Dispose();
            }

            // 使用新的 armatureName 重新构建骨架
            armatureComponent = UnityFactory.factory.BuildArmatureComponent(newArmatureName, "宝箱拆件", transform.GetChild(0).gameObject.name);
            armatureComponent.transform.parent = gameObject.transform;
            armatureComponent.transform.localPosition = Vector3.zero;
            armatureComponent.transform.localScale = Vector3.one * 0.5f;

            // 检查 armatureComponent 是否成功创建
            if (armatureComponent != null)
            {
                // 恢复之前的动画状态，或播放新动画
                if (!string.IsNullOrEmpty(currentAnimation) && armatureComponent.animation.HasAnimation(currentAnimation))
                {
                    armatureComponent.animation.Play(currentAnimation);
                }
                else
                {
                    armatureComponent.animation.Play("breath"); // 如果没有保存的动画，播放默认动画
                }
            }
            else
            {
                Debug.LogError("Failed to create armatureComponent for: " + newArmatureName);
                return; // 如果创建失败，提前返回
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
                    GetBuffIndex(deathPosition).Forget(); // 可考虑在这里处理取消
                }
            }

            await SpawnAndMoveCoins(coinBase, deathPosition).AttachExternalCancellation(cts.Token);
            PlayInforManager.Instance.playInfor.AddCoins((int)(coinsToSpawn - coinBase));
            gameMainPanelController.UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);

            if (GameFlowManager.Instance.currentLevelIndex == 0 && ChestGuid)
            {
                ChestGuid = false;
                gameMainPanelController.ShowSkillGuide(); // 调用显示技能提示的方法
            }
        }

        public int GetCoinIndex()
        {
            int randomNum = Random.Range(1, 100);
            Debug.Log("宝箱抽的数字" + randomNum);
            var coinindexConfig = ConfigManager.Instance.Tables.TableBoxcontent;
            if (randomNum < coinindexConfig.Get(1).Probability)
                return 1;
            else if (randomNum > coinindexConfig.Get(1).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability) // 71.45 + 23
                return 2;
            else if (randomNum > coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability) // 94.45 + 5
                return 3;
            else if (randomNum > coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability && randomNum < coinindexConfig.Get(1).Probability + coinindexConfig.Get(2).Probability + coinindexConfig.Get(3).Probability + coinindexConfig.Get(4).Probability) // 99.45 + 0.5
                return 4;
            else // 如果小于100，返回5
                return 5;
        }

        // 增加的 Buff 逻辑
        public async UniTask GetBuffIndex(Vector3 deathPosition)
        {
            if (cts.IsCancellationRequested) return; // 检查是否请求取消

            var BuffIndexConfig = ConfigManager.Instance.Tables.TableBoxcontent;
            int randomNum = Random.Range(0, (int)(BuffIndexConfig.Get(7).Probability + BuffIndexConfig.Get(6).Probability));
            if (randomNum < BuffIndexConfig.Get(6).Probability)
            {
                // 增加全屏冰冻次数
                PlayInforManager.Instance.playInfor.FrozenBuffCount++;
            }
            else
            {
                // 增加全屏轰炸次数
                PlayInforManager.Instance.playInfor.BalstBuffCount++;
                // SpawnPlane().Forget();
            }
        }

        // 生成金币并移动到UI标识
        public async UniTask SpawnAndMoveCoins(float coinCount, Vector3 deathPosition)
        {
            if (cts.IsCancellationRequested) return; // 检查是否请求取消

            List<UniTask> coinTasks = new List<UniTask>();
            for (int i = 0; i < coinCount; i++)
            {
                if (cts.IsCancellationRequested) break; // 如果取消，提前退出

                string CoinName = "gold";
                Debug.Log(transform.gameObject.name + "产生的金币" + i);
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
                    gold.AwaitMove(selectedCoinPool, CointargetPos).Forget(); // 假设 AwaitMove 内部处理取消
                }
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // 处理取消情况（如果需要）
                    break;
                }
            }
        }

        private async UniTask PlayAndWaitForAnimation(UnityArmatureComponent armature, string animationName, int playTimes = 1)
        {
            var tcs = new UniTaskCompletionSource();

            // 定义事件处理程序
            void OnAnimationComplete(string type, EventObject eventObject)
            {
                if (eventObject.animationState.name == animationName)
                {
                    armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);  // 移除监听器
                    tcs.TrySetResult(); // 完成任务
                }
            }

            // 添加事件监听器
            armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);

            // 播放指定动画，并指定播放次数
            armature.animation.Play(animationName, playTimes);

            // 等待任务完成并附加取消令牌
            try
            {
                await tcs.Task.AttachExternalCancellation(cts.Token);
            }
            catch (OperationCanceledException)
            {
                armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete); // 确保移除监听器
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
                    // 播放 "out" 动画并同步 ChestCoinText
                    await PlayCoinTextAnimation(ChestObj.transform.GetComponentInChildren<UnityArmatureComponent>()).AttachExternalCancellation(cts.Token);
                }
                else
                {
                    Debug.LogError("Failed to create new Armature");
                }
            }
        }

        // 播放 ChestCoinText 动画
        private async UniTask PlayCoinTextAnimation(UnityArmatureComponent newArmature)
        {
            if (cts.IsCancellationRequested) return; // 检查是否请求取消

            ChestCoinText.gameObject.SetActive(true);
            ChestCoinText.text = coinsToSpawn.ToString("N0");
            // 播放 "out" 动画，期间 ChestCoinText 逐渐变大
            newArmature.animation.Play("out", 1);
            ChestCoinText.transform.DOScale(Vector3.one * 0.5f, 0.5f); // 逐渐变大到1.5倍

            try
            {
                await UniTask.Delay(500, cancellationToken: cts.Token); // 等待动画完成
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

            // 保持大小不变，播放 "stay" 动画
            newArmature.animation.Play("stay", 3);
            try
            {
                await UniTask.Delay(3000, cancellationToken: cts.Token); // 停留一段时间
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

            // 播放 "end" 动画，期间 ChestCoinText 逐渐缩小并消失
            newArmature.animation.Play("end", 1);
            ChestCoinText.transform.DOScale(Vector3.zero, 0.5f); // 缩小到0
            try
            {
                await UniTask.Delay(500, cancellationToken: cts.Token); // 等待动画结束
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
