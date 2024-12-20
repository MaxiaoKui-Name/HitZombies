using Cysharp.Threading.Tasks;
using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;
using Random = UnityEngine.Random;
using Hitzb;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using cfg;

public enum EnemyType
{
    NormalMonster,
    BasketMonster,
    SteelMonster,
    HulkMonster,
    Boss
    // 其他敌人类型...
}
public enum BulletType
{
    bullet_01,
    bullet_02,
    bullet_03,
    bullet_04,
    bullet_boss,
    bullet_rocket,
    bullet_zombie,
    // 其他敌人类型...
}


public class PreController : Singleton<PreController>
{
    public Dictionary<string, ObjectPool<GameObject>> enemyPools = new Dictionary<string, ObjectPool<GameObject>>();
    public Dictionary<string, ObjectPool<GameObject>> bulletPools = new Dictionary<string, ObjectPool<GameObject>>();
    public Dictionary<string, ObjectPool<GameObject>> CoinPools = new Dictionary<string, ObjectPool<GameObject>>();
    public List<int> IEList = new List<int>();
    public Vector3 EnemyPoint;     // 敌人发射点
    public Transform FirePoint;     // 子弹发射点
    public float horizontalRange = 1.14f; // X轴的随机偏移范围
    private Camera mainCamera;
    public float screenBoundaryOffset = 1f;  // 允许子弹稍微超出屏幕的边界再隐藏
    public int CurwavEnemyNum = 0;
    public int waveEnemyCount = 0;
    public float GenerationIntervalEnemy;
    public float GenerationIntervalBullet;
    public Transform BulletPos;
    public Transform EnemyPos;
    public Transform CoinPar;
    public bool isAddIE = false;
    public bool isCreatePool = false;
    public bool isFrozen = false; // 添加冰冻状态变量
    public bool isRageSkill = false; // 添加冰冻状态变量
    public int activeEnemyCount = 0;
    public int currentSortingOrder = 1000; // 初始化一个较高的排序顺序
    public GameMainPanelController gameMainPanelController;
    public bool isFistNoteOne = false; // 添加冰冻状态变量
    public bool isFistNoteTwo = false; // 添加冰冻状态变量
    public bool isFistNoteThree = false; // 添加冰冻状态变量

    //添加存储激活的子弹
    public List<BulletController> flyingBullets = new List<BulletController>();
    // 新增：标志是否已发射第一颗子弹
    public bool hasFiredFirstBullet = true;
    public bool PlayisMove = true;
    public int numAll = 0;
    public async UniTask Init(List<GameObject> enemyPrefabs, List<GameObject> bulletPrefabs, List<GameObject> CoinPrefabs)
    {
        gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        isAddIE = false;
        isCreatePool = false;
        TestSuccessful = false;
        GenerationIntervalBullet = (float)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Cd / 1000f);
        BuffManager.Instance.originalGenerationIntervalBullet = GenerationIntervalBullet;
        EnemyPoint = LevelManager.Instance.levelData.enemySpawnPoints;
        FirePoint = GameObject.Find("Player/FirePoint").transform;
        mainCamera = Camera.main;
        BulletPos = GameObject.Find("AllPre/BulletPre").transform;
        EnemyPos = GameObject.Find("AllPre/EnemyPre").transform;
        CoinPar = GameObject.Find("CoinAnimationCanvas").transform;
        await CreatePools(enemyPrefabs, bulletPrefabs, CoinPrefabs);
        StartGame();

    }
    #region 创建TNT的相关代码
    private GameObject CreateTNTPrefab(GameObject b, UnityEngine.Transform tntParent)
    {
        GameObject obj = Instantiate(b, tntParent);
        var mr = obj.GetComponent<MeshRenderer>();
        if (mr != null)
            mr.sortingOrder = 64 + tntParent.GetSiblingIndex();
        return obj;
    }

    public void Get(GameObject go)
    {
        // go.SetActive(true);
        // 激活对象
        
    }
    public void FixSortLayer(GameObject go)
    {
        // 获取所有 Renderer 组件（包括子物体）
        UnityArmatureComponent[] renderers = go.GetComponentsInChildren<UnityArmatureComponent>();
        foreach (var renderer in renderers)
        {
            // 设置排序层级（根据需要调整 Sorting Layer 名称）
            renderer.sortingLayerName = "Default"; // 确保所有对象使用相同的 Sorting Layer
            renderer.sortingOrder = currentSortingOrder;
        }
        // 递减排序顺序，以确保先激活的物体具有更高的排序顺序
        currentSortingOrder --;
        // 可选：防止 sortingOrder 过低，重置排序顺序
        if (currentSortingOrder < 0)
        {
            currentSortingOrder = 10000;
        }
    }
    public void Release(GameObject go)
    {
        go.SetActive(false);
    }

    public void MyDestroy(GameObject go)
    {
        Destroy(go);
    }
    #endregion
    private int playBulletCoroutineId = -1;
    public void StartGame()
    {
        if(!isAddIE)
        {
            isAddIE = true;
            IEList.Add(IEnumeratorTool.StartCoroutine(IE_PlayEnemies()));
            playBulletCoroutineId = IEnumeratorTool.StartCoroutine(IE_PlayBullet());
            IEList.Add(playBulletCoroutineId);
            //IEList.Add(IEnumeratorTool.StartCoroutine(IE_PlayBullet()));
        }
    }
    public bool isBulletCostZero = false; // 是否子弹不消耗金币
    public bool isFiring = false; // 是否子弹不消耗金币
                                  // 定义一个事件
    public event Action OnPlayerFiring;
    void Shoot(ObjectPool<GameObject> selectedBulletPool, string bulletName)
    {
        long bulletCost = (long)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(PlayInforManager.Instance.playInfor.level).Total);
        if (isBulletCostZero)
        {
            bulletCost = 0;
        }
        // 检查玩家是否有足够的金币
        if (PlayInforManager.Instance.playInfor.SpendCoins(bulletCost))
        {
            GameObject Bullet = selectedBulletPool.Get();
            Bullet.SetActive(true);
            AudioManage.Instance.PlaySFX("ak", null);
            FixSortLayer(Bullet);
            Bullet.transform.position = FirePoint.position;
            EventDispatcher.instance.DispatchEvent(EventNameDef.ShowBuyBulletText);
            // 发射子弹时立即更新动画状态
            isFiring = true; // 标记发射状态
            // 触发发射事件
            OnPlayerFiring?.Invoke();  // 通知PlayerController更新动画

            // 新增：将子弹加入飞行列表
            BulletController bulletController = Bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                flyingBullets.Add(bulletController);
                bulletController.OnBulletDestroyed += HandleBulletDestroyed; // 注册子弹销毁事件
            }
            #region[提示·2]
            //// 新增代码：当发射第一颗子弹时，显示 TwoNote_F
            //if (hasFiredFirstBullet && GameFlowManager.Instance.currentLevelIndex == 0)
            //{
            //    hasFiredFirstBullet = false;
            //    if (gameMainPanelController != null)
            //    {
            //        gameMainPanelController.ShowTwoNoteAfterDelay();
            //    }
            //}
            #endregion
        }
        else
        {
            Debug.Log("金币不足，无法发射子弹。");
        }
    }
    public int initialLevEneNun = 0;
    public bool isFour = false;
    private void Update()
    {
        if(GameFlowManager.Instance.currentLevelIndex == 0 && isFour)
        {
            if(initialLevEneNun == numAll )
            {
                StartCoroutine(HandleBeginnerLevelThree());
            }
        } 
    }
    private void HandleBulletDestroyed(BulletController bullet)
    {
        bullet.OnBulletDestroyed -= HandleBulletDestroyed;
        flyingBullets.Remove(bullet);
    }

    //判断怪物是否超出边界
    public void HideAndReturnToPool(ObjectPool<GameObject> objPool, GameObject objPre)
    {
        objPre.SetActive(false);
        objPool.Release(objPre);

    }
    public ObjectPool<GameObject> GetEnemyPoolMethod(GameObject objPre)
    {
        string enemyName = objPre.name.Replace("(Clone)", "").Trim();
        if (enemyPools.ContainsKey(enemyName))
        {
            var PrePool = enemyPools[enemyName];
            return PrePool;
        }
        else
        {
            Debug.LogWarning($"No pool found for enemy: {enemyName}");
            return null;
        
        }
    }
    private async UniTask CreatePools(List<GameObject> enemyPrefabs, List<GameObject> bulletPrefabs, List<GameObject> CoinPrefabs)
    {
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[i];
            string enemyName = enemyPrefab.name;
            ObjectPool<GameObject> enemyPool = new ObjectPool<GameObject>(
                () => CreateTNTPrefab(enemyPrefab, EnemyPos),
                Get,
                Release,
                MyDestroy,
                true, 200, 2000
            );

            enemyPools[enemyName] = enemyPool;
        }
        for (int i = 0; i < bulletPrefabs.Count; i++)
        {
            GameObject bulletPrefab = bulletPrefabs[i];
            string bulletName = bulletPrefab.name;

            ObjectPool<GameObject> bulletPool = new ObjectPool<GameObject>(
                () => CreateTNTPrefab(bulletPrefab, BulletPos),
                Get,
                Release,
                MyDestroy,
                true, 200, 2000
            );

            bulletPools[bulletName] = bulletPool;
        }
        for (int i = 0; i < CoinPrefabs.Count; i++)
        {
            GameObject coinPrefab = CoinPrefabs[i];
            string coinName = coinPrefab.name;

            ObjectPool<GameObject> coinPool = new ObjectPool<GameObject>(
                () => CreateTNTPrefab(coinPrefab, CoinPar),
                Get,
                Release,
                MyDestroy,
                true, 200, 2000
            );

            CoinPools[coinName] = coinPool;
        }

        PreWarmPools(bulletPools.Values);
        PreWarmPools(enemyPools.Values);
        PreWarmPools(CoinPools.Values);
    }

    private void PreWarmPools(IEnumerable<ObjectPool<GameObject>> pools)
    {
        int readyCount = 200;
        List<GameObject> tempObjList = new List<GameObject>(readyCount);

        foreach (var pool in pools)
        {
            for (int j = 0; j < readyCount; j++)
                tempObjList.Add(pool.Get());

            for (int j = 0; j < tempObjList.Count; j++)
                pool.Release(tempObjList[j]);

            tempObjList.Clear();
        }
        isCreatePool = true;
    }
    public bool TestSuccessful = false;
    private IEnumerator IE_PlayEnemies()
    {
        for (int waveIndex = 0; waveIndex < LevelManager.Instance.levelData.Monsterwaves.Count; waveIndex++)
        {
            while (isFrozen)
            {
                yield return null;
            }
            while (isRageSkill)
            {
                yield return null;
            }
            int waveKey = LevelManager.Instance.levelData.Monsterwaves[waveIndex];
            List<List<int>> enemyTypes = LevelManager.Instance.levelData.WavesenEmiesDic[waveKey];

            //if (waveKey % 10 == 7)
            //{
            //    GameObject ChestObj = Instantiate(LevelManager.Instance.levelData.PowbuffDoor, new Vector3(-0.08f, 7f, 0f), Quaternion.identity);
            //    FixSortLayer(ChestObj);
            //}
            // 如果当前关卡是第0关且 FirstNote_FBool 为 false，则等待 FirstNote_FBool 变为 true
            if (GameFlowManager.Instance.currentLevelIndex == 0 && gameMainPanelController != null && !gameMainPanelController.FirstNote_FBool)
            {
                Debug.Log("第0关且 FirstNote_FBool 为 false，暂停敌人生成，等待 FirstNote_F 完成显示");
                // 等待直到 FirstNote_FBool 变为 true
                while (!gameMainPanelController.FirstNote_FBool)
                {
                    yield return null;
                }
                Debug.Log("FirstNote_F 已显示，继续敌人生成");
            }
            if (GameFlowManager.Instance.currentLevelIndex == 0 && gameMainPanelController != null && gameMainPanelController.TwoNote_FBool)
            {
                Debug.Log("第0关且 TwoNote_FBool 为 false，暂停敌人生成，等待 TwoNote_FBool 完成显示");
                // 等待直到 FirstNote_FBool 变为 true
                while (gameMainPanelController.TwoNote_FBool)
                {
                    yield return null;
                }
                Debug.Log("TwoNote_FBool 已显示，继续敌人生成");
            }
            //if (GameFlowManager.Instance.currentLevelIndex == 0 && gameMainPanelController != null && gameMainPanelController.ThreeNote_FBool)
            //{
            //    Debug.Log("第0关且 ThreeNote_FBool 为 false，暂停敌人生成，等待 ThreeNote_FBool 完成显示");
            //    // 等待直到 FirstNote_FBool 变为 true
            //    while (gameMainPanelController.ThreeNote_FBool)
            //    {
            //        yield return null;
            //    }
            //    Debug.Log("ThreeNote_FBool 已显示，继续敌人生成");
            //}
            List<Coroutine> enemyCoroutines = new List<Coroutine>();

            for (int i = 0; i < enemyTypes.Count; i++)
            {
                Coroutine coroutine = StartCoroutine(SpawnEnemies(enemyTypes[i], waveKey, i));
                enemyCoroutines.Add(coroutine);
            }

            // 等待所有敌人生成的协程完成
            foreach (var coroutine in enemyCoroutines)
            {
                yield return coroutine; // 等待每个协程完成
            }
            //var enemyConfig = ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey);
            //yield return new WaitForSeconds(ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey).Time / 1000f);
            Debug.Log($"{waveIndex}波次完成========================");
        }
        Debug.Log("所有波次完成========================");
        //TTOD2测试使用
        if (GameFlowManager.Instance.currentLevelIndex != 0)
        {
            if (!TestSuccessful)
            {
                TestSuccessful = true;
                GameManage.Instance.JudgeVic = true;
            }
        }

    }
    private IEnumerator SpawnEnemies(List<int> enemyTypestwo, int waveKey, int ListIndex)
    {
        var enemyConfig = ConfigManager.Instance.Tables.TableLevelConfig;
        float spawnInterval = 0f;
        int totalEnemies = 0;

        // 计算当前波次总敌人数和生成间隔
        if (LevelManager.Instance.levelData.WaveEnemyCountDic[waveKey].Sum() == 0)
        {
            spawnInterval = (GameFlowManager.Instance.currentLevelIndex == 0 ?
                ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Time :
                enemyConfig.Get(waveKey).Time) / 1000f;
        }
        else
        {
            totalEnemies = LevelManager.Instance.levelData.WaveEnemyCountDic[waveKey].Sum();
            spawnInterval = (GameFlowManager.Instance.currentLevelIndex == 0 ?
                ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Time :
                enemyConfig.Get(waveKey).Time) / 1000f / totalEnemies;
            Debug.Log("该波总敌人数: " + totalEnemies);
        }

        foreach (var enemyId in enemyTypestwo)
        {
            if (enemyId != 0)
            {
                int waveEnemyCount = LevelManager.Instance.levelData.WaveEnemyCountDic[waveKey][ListIndex];
                Debug.Log($"波数" + waveKey + "数量" + waveEnemyCount);

                if (waveEnemyCount == 0)
                {
                    yield return new WaitForSecondsRealtime(spawnInterval);
                }
                else
                {
                    for (int q = 0; q < waveEnemyCount; q++)
                    {
                        if (GameFlowManager.Instance.currentLevelIndex == 0 && gameMainPanelController != null && gameMainPanelController.ThreeNote_FBool)
                        {
                            Debug.Log("第0关且 ThreeNote_FBool 为 false，暂停敌人生成，等待 ThreeNote_FBool 完成显示");
                            // 等待直到 FirstNote_FBool 变为 true
                            while (gameMainPanelController.ThreeNote_FBool)
                            {
                                yield return null;
                            }
                            Debug.Log("ThreeNote_FBool 已显示，继续敌人生成");
                        }
                        if (GameFlowManager.Instance.currentLevelIndex == 0 && waveKey  == 3 && gameMainPanelController != null && !gameMainPanelController.TwoBeThree)
                        {
                            Debug.Log("第0关且 ThreeNote_FBool 为 false，暂停敌人生成，等待 ThreeNote_FBool 完成显示");
                            // 等待直到 FirstNote_FBool 变为 true
                            while (!gameMainPanelController.TwoBeThree)
                            {
                                yield return null;
                            }
                            Debug.Log("ThreeNote_FBool 已显示，继续敌人生成");
                        }
                        string enemyName = GameFlowManager.Instance.GetSpwanPre(enemyId);
                        if (enemyPools.TryGetValue(enemyName, out var selectedEnemyPool))
                        {
                            if (GameFlowManager.Instance.currentLevelIndex == 0 && !isGivePOs)//ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).IsGivePos)
                            {
                                isGivePOs = true;
                                GameObject enemy = selectedEnemyPool.Get();
                                Vector3 playpos = GameObject.Find("Player").transform.position;
                                enemy.transform.position = new Vector3(playpos.x, EnemyPoint.y, 0f);
                                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                                //enemyController.health = 1000;
                                enemyController.isSpecialHealth = true;
                                enemy.SetActive(true);
                                if (enemyId == 4)
                                {
                                    AudioManage.Instance.PlaySFX("monstershow", null);
                                }
                                FixSortLayer(enemy);
                                //if (hasFiredFirstBullet && GameFlowManager.Instance.currentLevelIndex == 0)
                                //{
                                //    hasFiredFirstBullet = false;
                                //    PlayisMove = false;
                                //    if (gameMainPanelController != null)
                                //    {
                                //        gameMainPanelController.ShowTwoNote1();
                                //    }
                                //}
                            }
                            else
                            {
                                PlayEnemy(selectedEnemyPool);
                            }

                        }
                        else
                        {
                            Debug.LogWarning($"未找到敌人池: {enemyName}");
                        }
                        #region[原先提示3]
                        ////TTOD1新手关特殊处理
                        if (GameFlowManager.Instance.currentLevelIndex == 0)
                        {
                            if (waveKey == 3 && q == waveEnemyCount - 1 && numAll == 0)
                            {
                                isFour = true;
                                foreach (char key1 in LevelManager.Instance.levelData.WaveEnemyCountDic.Keys)
                                {
                                    foreach (char key2 in LevelManager.Instance.levelData.WaveEnemyCountDic[key1])
                                    {
                                        numAll += key2;
                                    }
                                }
                            }
                        }
                        //if (GameFlowManager.Instance.currentLevelIndex == 0)
                        //{
                        //    //讲三个怪物击杀后
                        //    if (waveKey < 3)
                        //    {

                        //        Beginnerlevel(waveKey, q, enemyId, waveEnemyCount);
                        //    }
                        //    if (waveKey == 3)
                        //    {
                        //        //TTOD1执行生成门的逻辑
                        //        if (!isBuffNumFour)
                        //        {
                        //            DoorNumWave = waveKey;
                        //        }
                        //        if (ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Door && !isBuffNumFour)
                        //        {
                        //            StartCoroutine(SpawnBuffDoorAfterDelay(0));
                        //            isBuffNumFour = true;
                        //        }
                        //    }
                        //    if (waveKey == 4)
                        //    {
                        //        //TTOD1执行生成门的逻辑
                        //        if (!isBuffNumFive)
                        //        {
                        //            DoorNumWave = waveKey;
                        //        }
                        //        if (ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Door && !isBuffNumFive)
                        //        {
                        //            StartCoroutine(SpawnBuffDoorAfterDelay(0));
                        //            isBuffNumFive = true;
                        //        }
                        //    }
                        //}
                        #endregion[原先提示3]
                        yield return new WaitForSecondsRealtime(spawnInterval);
                        Debug.Log($"敌人间隔: {spawnInterval}");
                    }

                }

            }
        }

        //TTOD1
        //if (GameFlowManager.Instance.currentLevelIndex == 0 && waveKey == LevelManager.Instance.levelData.Monsterwaves.Count && !isCreateBoss)
        //{
        //    isCreateBoss = true;
        //    GameObject Boss = Instantiate(Resources.Load<GameObject>("Prefabs/Boss"));
        //    Boss.transform.position = EnemyPoint;
        //}
    }
    

    public int DoorNumWave;//用来赋buffid
    //public int BoxNumWave;
    public bool isBuffNumFour = false;
    public bool isBuffNumFive = false;
    public bool isCreateBoss = false;
    public bool isGivePOs = false;
    //public bool isBuffNumSix = false;
    //public bool isBoxNumOne = false;
    //public bool isBoxNumTwo = false;


    private IEnumerator IE_PlayBullet()
    {
        while (true)
        {
            if (isCreatePool && activeEnemyCount > 0 && GameManage.Instance.gameState == GameState.Running && Time.timeScale == 1f)
            {
                float HoridetectionRange = 0.1f;
                float VertialdetectionRange = 7f;

                if (IsEnemyInFront(HoridetectionRange, VertialdetectionRange))
                {
                    float totalBulletDamage = GetTotalFlyingBulletDamage(HoridetectionRange, VertialdetectionRange);
                    float totalEnemyHealth = GetTotalEnemyHealthInRange(HoridetectionRange, VertialdetectionRange);

                    // 如果飞行中子弹的总伤害小于敌人总生命值，继续开火
                    if (totalBulletDamage < totalEnemyHealth)
                    {
                        Gun currentGun = PlayInforManager.Instance.playInfor.currentGun;

                        if (currentGun != null)
                        {
                            string bulletKey = currentGun.bulletType;

                            if (bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
                            {
                                Shoot(selectedBulletPool, bulletKey);
                            }
                            else
                            {
                                Debug.LogWarning($"Bullet pool not found for: {bulletKey}");
                            }
                        }
                    }
                    else
                    {
                        // 飞行中的子弹足以消灭前方敌人，不再开火
                        isFiring = false;
                        // 触发发射事件
                        OnPlayerFiring?.Invoke();  // 通知PlayerController更新动画

                    }
                }
                else
                {
                    // 正前方没有敌人，不开火
                    isFiring = false;
                    // 触发发射事件
                    OnPlayerFiring?.Invoke();  // 通知PlayerController更新动画

                }
            }
            // 等待发射间隔
            yield return new WaitForSecondsRealtime(GenerationIntervalBullet);
        }
    }


    public void RestartIEPlayBullet()
    {
        // 停止当前的协程
        if (playBulletCoroutineId != -1)
        {
            IEnumeratorTool.StopCoroutine(playBulletCoroutineId);
        }
        // 重新启动协程
        playBulletCoroutineId = IEnumeratorTool.StartCoroutine(IE_PlayBullet());
    }
    public int InNu = 0;

    private void PlayEnemy(ObjectPool<GameObject> enemyPool)
    {
        GameObject enemy = enemyPool.Get();
        if (enemy.name == "boss_blue(Clone)")
            enemy.transform.position = RandomPosition(EnemyPoint) - new Vector3(0,1f,0f);
        else
            enemy.transform.position = RandomPosition(EnemyPoint);
        enemy.SetActive(true);
        if (enemy.name == "HulkMonster(Clone)")
        {
            AudioManage.Instance.PlaySFX("monstershow", null);
        }
        InNu++;
        FixSortLayer(enemy);
        Debug.Log("真正的敌人人数" + InNu);
    }
    public Vector3 RandomPosition(Vector3 Essentialpos)
    {
        float randomX = Random.Range(-horizontalRange, horizontalRange);
        return new Vector3(Essentialpos.x + randomX, Essentialpos.y, 0);
    }
   
    private IEnumerator SpawnBuffDoorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManage.Instance.SpawnBuffDoor();
    }
   
    public void DignoExtre(GameObject objPre)
    {
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(objPre.transform.position);
        if (screenPosition.y < -screenBoundaryOffset || screenPosition.y > 1 + screenBoundaryOffset ||
            screenPosition.x < -screenBoundaryOffset || screenPosition.x > 1 + screenBoundaryOffset)
        {
            if (objPre.layer == 7)
                HideAndReturnToPool(GetBulletPoolMethod(objPre), objPre);
            //else
            //{
            //    BuffDoorController buffDoorController = objPre.transform.GetComponent<BuffDoorController>();
            //    buffDoorController.HideAllChildren();
            //}
            // if (objPre.layer == 6)
            //     HideAndReturnToPool(GetEnemyPoolMethod(objPre), objPre);
        }
    }

    public ObjectPool<GameObject> GetBulletPoolMethod(GameObject objPre)
    {
        string bulletName = objPre.name.Replace("(Clone)", "").Trim();
        if (bulletPools.ContainsKey(bulletName))
        {
            var PrePool = bulletPools[bulletName];
            return PrePool;
        }
        else
        {
            Debug.LogWarning($"No pool found for bullet: {bulletName}");
            return null;
        }
    }
    public void IncrementActiveEnemy()
    {
        activeEnemyCount++;
        Debug.Log(activeEnemyCount + "可视化数量2=================");
    }

    /// <summary>
    /// 减少活跃敌人数量
    /// </summary>
    public void DecrementActiveEnemy()
    {
        initialLevEneNun++;
        Debug.Log(initialLevEneNun + "initialLevEneNun可视化数量=================");
        activeEnemyCount --;
        activeEnemyCount = Mathf.Max(activeEnemyCount, 0);
    }

    #region 新增 Beginerlevel 方法和相关逻辑
    public IEnumerator HandleBeginnerLevelTwo()
    {
        if (gameMainPanelController != null)
        {
            
            yield return StartCoroutine(gameMainPanelController.ShowThreeNote());
        }
        else
        {
            Debug.LogError("GameMainPanelController 未赋值！");
        }
    }
    public IEnumerator HandleBeginnerLevelThree()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (gameMainPanelController != null && !gameMainPanelController.FourNote_FBool)
        {
            isFour = false;
            yield return StartCoroutine(gameMainPanelController.ShowFourNote());
        }
        else
        {
            Debug.Log("GameMainPanelController 未赋值！");
        }
    }


    #endregion


    // 检测玩家正前方一定范围内是否有敌人
    private bool IsEnemyInFront(float HorizdetectionRange,float VertialdetectionRange)
    {
        // 获取玩家位置
        Vector3 playerPosition = FirePoint.position;
        //Vector3 playerPosition = GameObject.FindGameObjectWithTag("FirePoint").transform.position;
        // 定义检测区域的左下角和右上角
        Vector2 pointA = new Vector2(playerPosition.x - HorizdetectionRange, playerPosition.y);
        Vector2 pointB = new Vector2(playerPosition.x + HorizdetectionRange, playerPosition.y + VertialdetectionRange);
        // 定义敌人所在的Layer
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        int ChestLayerMask = LayerMask.GetMask("Chest");
        // 获取检测区域内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayerMask | ChestLayerMask);

        foreach (var collider in colliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                return true; // 范围内有敌人
            }
        }
        return false; // 范围内没有敌人

    }


    //计算敌人生命值
    private float GetTotalFlyingBulletDamage(float HoridetectionRange, float VertialdetectionRange)
    {
        float totalDamage = 0f;
        // 获取玩家位置
        Vector3 playerPosition = FirePoint.position;
        //Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Debug.Log("玩家的位置信息" + playerPosition);
        // 定义检测区域的左下角和右上角
        Vector2 pointA = new Vector2(playerPosition.x - HoridetectionRange, playerPosition.y);
        Vector2 pointB = new Vector2(playerPosition.x + HoridetectionRange, playerPosition.y + VertialdetectionRange);
        // 定义敌人所在的Layer
        int bulletLayerMask = LayerMask.GetMask("Bullet");
        // 获取检测区域内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, bulletLayerMask);
        foreach (var collider in colliders)
        {
            BulletController bullet = collider.GetComponent<BulletController>();
            if (bullet != null && bullet.gameObject.activeSelf)
            {
                totalDamage += bullet.firepower;
            }
        }
        return totalDamage;
    }
    private float GetTotalEnemyHealthInRange(float HoridetectionRange, float VertialdetectionRange)
    {
        float totalHealth = 0f;

        // 获取玩家位置
        Vector3 playerPosition = FirePoint.position;
        //Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Debug.Log("玩家的位置信息" + playerPosition);
        // 定义检测区域的左下角和右上角
        Vector2 pointA = new Vector2(playerPosition.x - HoridetectionRange, playerPosition.y);
        Vector2 pointB = new Vector2(playerPosition.x + HoridetectionRange, playerPosition.y + VertialdetectionRange);

        // 定义敌人所在的Layer
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        int ChestLayerMask = LayerMask.GetMask("Chest");
        // 获取检测区域内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayerMask | ChestLayerMask);

        foreach (var collider in colliders)
        {
            if(collider.gameObject.layer == 6)
            {
                EnemyController enemy = collider.GetComponent<EnemyController>();
                if (enemy != null && enemy.gameObject.activeSelf)
                {
                    totalHealth += enemy.health;
                }
            }
            if (collider.gameObject.layer == 13)
            {
                ChestController chest = collider.GetComponent<ChestController>();
                if (chest != null && chest.gameObject.activeSelf)
                {
                    totalHealth += chest.chestHealth;
                }
            }
        }
        return totalHealth;
    }

    // 新增方法：生成追踪子弹
    // 修改 SpawnHomingBullet 方法
    public void SpawnHomingBullet(Vector3 position, Transform target)
    {
        Gun currentGun = PlayInforManager.Instance.playInfor.currentGun;
        if (currentGun != null)
        {
            string bulletKey = currentGun.bulletType;

            if (bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
            {
                long bulletCost = (long)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(PlayInforManager.Instance.playInfor.level).Total);
                // 检查玩家是否有足够的金币
                if (PlayInforManager.Instance.playInfor.SpendCoins(bulletCost))
                {
                    GameObject Bullet = selectedBulletPool.Get();
                    Bullet.SetActive(true);
                    FixSortLayer(Bullet);
                    Bullet.transform.position = FirePoint.position;
                    EventDispatcher.instance.DispatchEvent(EventNameDef.ShowBuyBulletText);

                    // 设置子弹的目标
                    BulletController bulletController = Bullet.GetComponent<BulletController>();
                    if (bulletController != null)
                    {
                        bulletController.SetTarget(target);
                        // 新增：将子弹加入飞行列表并注册销毁事件
                        flyingBullets.Add(bulletController);
                        bulletController.OnBulletDestroyed += HandleBulletDestroyed;
                    }
                }
                else
                {
                    Debug.Log("金币不足，无法发射子弹。");
                }
            }
        }
    }
}
