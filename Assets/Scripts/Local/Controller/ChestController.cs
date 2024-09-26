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
        public float chestHealth = 100;  // 宝箱的初始血量
        public Transform coinTarget;   // 金币目标（比如UI上的金币位置）
        public float coinsToSpawn = 5;   // 生成的金币数量
        public UnityArmatureComponent armatureComponent;
        private bool isOpened = false; // 标记宝箱是否已经打开

        [Header("Movement Properties")]
        public float moveSpeed = 1f; // 设置物体向下移动的速度
        public float hideYPosition = -10f; // 超出屏幕的Y坐标
        public bool isMove = false;

        [Header("Spawn Properties")]
        public GameObject planePrefab;  // 飞机的预制体
        public GameObject bombPrefab;   // 炸弹的预制体
        public float planeSpeed = 2f;   // 飞机移动速度
        public float bombDropInterval;  // 炸弹投掷间隔时间
        public float coinBase = 100f;   // 基础金币数量

        private Collider2D chestCollider; // 宝箱的碰撞体
        private Camera mainCamera;
        public Transform healthBarCanvas; // 血条所在的Canvas
        public TextMeshProUGUI ChestBar; // 宝箱血量
        public TextMeshProUGUI ChestCoinText; // 宝箱钱
        public Vector3 addVector = Vector3.zero;
        public Vector3 ScaleVector  = new Vector3(0.01f, 0.01f, 0.01f);
        public bool isVise;
        void OnEnable()
        {
           
            InitializeChest();
            StartCoroutine(StartAnimation());
        }

        IEnumerator StartAnimation()
        {
            yield return new WaitForSeconds(0.5f); // 等待0.5秒，确保动画组件已初始化

            // 播放初始动画
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

        // 初始化宝箱
        private void InitializeChest()
        {
            isMove = true;
            isOpened = false;
            isFinishHit = true;
            isVise = false;
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
            chestCollider = GetComponent<Collider2D>(); // 获取碰撞体组件
            if (chestCollider != null)
            {
                chestCollider.enabled = true; // 确保碰撞体启用
            }
        }

        // 物体向下移动
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
        // 处理宝箱受伤
        public async void TakeDamage(float damage, GameObject bulletObj)
        {
            if (isOpened) return; // 如果已经打开，直接返回
            chestHealth -= damage;
            ChestBar.text = $"{Mathf.Max(chestHealth, 0)}";
            coinsToSpawn = bulletObj.GetComponent<BulletController>().bulletcost;
            // 播放hit动画并等待完成
            if (armatureComponent != null && isFinishHit)
            {
                isFinishHit = false;
                await PlayAndWaitForAnimation(armatureComponent, "hit1", 1); // 播放一次hit动画
            }
            // 如果宝箱血量小于等于0，播放开箱动画并生成金币和飞机
            if (chestHealth <= 0 && !isOpened)
            {
                isOpened = true;
                ChestBar.gameObject.SetActive(false);
                Debug.Log("Chest opened!");
                PreController.Instance.DecrementActiveEnemy();
                Vector3 deathPosition = transform.position;
                GetProbability(deathPosition).Forget();
                // 播放开箱动画并等待完成
                if (armatureComponent != null)
                {
                    await PlayAndWaitForAnimation(armatureComponent, "open", 1); // 播放一次开箱动画
                }
                if (chestCollider != null)
                {
                    chestCollider.enabled = false;
                }
                // 设置宝箱为不活动状态
                gameObject.SetActive(false); // 立即禁用宝箱
               
            }
        }

        // 计算概率并决定是否生成金币
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
        public async UniTask GetBuffIndex(Vector3 deathPosition)
        {
            float randomNum = Random.Range(0f, 100f);
            var BuffIndexConfig = ConfigManager.Instance.Tables.TableBoxcontent;
            if (randomNum < BuffIndexConfig.Get(6).Probability)
            {
                //TTOD1执行全屏冰冻的效果
            }
            else
            {
                //TTOD执行全屏轰炸的效果
                SpawnPlane().Forget();
            }
               
        }

        // 生成金币并移动到UI标识
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

                //    // 添加每个金币的移动任务
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

        //// 移动金币到UI标识
        //public async UniTask MoveCoinToUI(GameObject coinObj, ObjectPool<GameObject> coinPool)
        //{
        //    float duration = 0.5f; // 增加持续时间以便更明显的移动效果
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

        // 生成并移动飞机（异步）
        public async UniTask SpawnPlane()
        {
            GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // 生成飞机在屏幕底部
            Debug.Log("Plane spawned!");
            await MovePlaneAndDropBombs(plane);
            PlayInforManager.Instance.playInfor.AddCoins((int)(coinsToSpawn - coinBase));
            if (plane != null)
            {
                Destroy(plane);
            }
            Destroy(gameObject);

        }

        // 飞机移动并投放炸弹的异步方法
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



        // 投放炸弹（异步）
        private async UniTask DropBomb(Vector3 planePosition)
        {
            GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
            await BombExplosion(bomb);
        }

        // 炸弹爆炸动画，并消灭敌人（异步）
        private async UniTask BombExplosion(GameObject bomb)
        {
            UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
            if (bombArmature != null)
            {
                bombArmature.animation.Play("fly", 1); // 播放一次飞行动画
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy.activeSelf)
                {
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();
                    if (!enemyController.isDead && enemyController.isVise)
                    {
                        enemyController.Enemycoins2 = 10;
                        enemyController.TakeDamage(100000, enemy);
                    }
                }
            }
            await UniTask.Delay(1000);
            Destroy(bomb);
        }

        // 等待动画播放完成
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

            // 等待任务完成
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
