using Cysharp.Threading.Tasks;
using dnlib.DotNet.Pdb;
using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;
using Random = UnityEngine.Random;

public enum EnemyType
{
    CuipiMonster1,
    CuipiMonster2,
    ShortMonster,
    DisMonster,
    ElitesMonster,
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
    public float horizontalRange = 1.3f; // X轴的随机偏移范围
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

    public int activeEnemyCount = 0;
    public int currentSortingOrder = 1000; // 初始化一个较高的排序顺序
    public GameMainPanelController gameMainPanelController;
    public bool isFistNoteOne = false; // 添加冰冻状态变量
    public bool isFistNoteTwo = false; // 添加冰冻状态变量
    public bool isFistNoteThree = false; // 添加冰冻状态变量
    public async UniTask Init(List<GameObject> enemyPrefabs, List<GameObject> bulletPrefabs, List<GameObject> CoinPrefabs)
    {
        gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        isAddIE = false;
        isCreatePool = false;
        GenerationIntervalBullet = (float)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Cd / 1000f);
        EnemyPoint = LevelManager.Instance.levelData.enemySpawnPoints;
        FirePoint = GameObject.Find("Player/FirePoint").transform;
        mainCamera = Camera.main;
        BulletPos = GameObject.Find("AllPre/BulletPre").transform;
        EnemyPos = GameObject.Find("AllPre/EnemyPre").transform;
        CoinPar = GameObject.Find("CoinAnimationCanvas").transform;
        await CreatePools(enemyPrefabs, bulletPrefabs,CoinPrefabs);
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

    public void StartGame()
    {
        if(!isAddIE)
        {
            isAddIE = true;
            IEList.Add(IEnumeratorTool.StartCoroutine(IE_PlayEnemies()));
            IEList.Add(IEnumeratorTool.StartCoroutine(IE_PlayBullet()));
        }
    }

    void Shoot(ObjectPool<GameObject> selectedBulletPool, string bulletName)
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
        }
        else
        {
            Debug.Log("Not enough coins to shoot the bullet.");
        }
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
            int waveKey = LevelManager.Instance.levelData.Monsterwaves[waveIndex];
            List<List<int>> enemyTypes = LevelManager.Instance.levelData.WavesenEmiesDic[waveKey];

            //if (waveKey % 10 == 7)
            //{
            //    GameObject ChestObj = Instantiate(LevelManager.Instance.levelData.PowbuffDoor, new Vector3(-0.08f, 7f, 0f), Quaternion.identity);
            //    FixSortLayer(ChestObj);
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
        if (!TestSuccessful)
        {
            TestSuccessful = true;
            GameManage.Instance.JudgeVic = true;
        }
    }
    public int DoorNumWave;
    public int BoxNumWave;
    public bool isBuffNumThree = false;
    public bool isBuffNumFour = false;
    public bool isBuffNumFive = false;
    public bool isBuffNumSix = false;
    public bool isBoxNumOne = false;
    public bool isBoxNumTwo = false;
    private IEnumerator SpawnEnemies(List<int> enemyTypestwo, int waveKey, int ListIndex)
    {
        var enemyConfig = ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey);
        float spawnInterval = enemyConfig.Time / 1000f / LevelManager.Instance.levelData.WaveEnemyCountDic[waveKey][ListIndex];
        foreach (var enemyId in enemyTypestwo)
        {
            if (enemyId != 0)
            {
                int waveEnemyCount = LevelManager.Instance.levelData.WaveEnemyCountDic[waveKey][ListIndex];
                for (int q = 0; q < waveEnemyCount; q++)
                {
                    string enemyName = GameFlowManager.Instance.GetSpwanPre(enemyId);
                    if (enemyPools.TryGetValue(enemyName, out var selectedEnemyPool))
                    {
                        PlayEnemy(selectedEnemyPool);
                    }
                    else
                    {
                        Debug.LogWarning($"未找到敌人池: {enemyName}");
                    }
                    //TTOD1新手关特殊处理
                    if(GameFlowManager.Instance.currentLevelIndex == 0)
                    {
                        //讲三个怪物击杀后
                        if (waveKey < 5)
                        {
                            Beginnerlevel(waveKey, q, enemyId);
                            if (waveKey == 4 && ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Door && !isBuffNumThree)
                            {
                                //TTOD1执行生成门的逻辑
                                DoorNumWave = waveKey;
                                StartCoroutine(SpawnBuffDoorAfterDelay(0));
                                isBuffNumThree = true;
                            }
                        }
                        if (waveKey == 5)
                        {
                            //TTOD1执行生成门的逻辑
                            if (!isBuffNumFour)
                            {
                                DoorNumWave = waveKey;
                                BoxNumWave = waveKey;
                            }
                            if (ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Door && !isBuffNumFour)
                            {
                                StartCoroutine(SpawnBuffDoorAfterDelay(0));
                                isBuffNumFour = true;
                            }
                            if (ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Box && !isBoxNumOne)
                            {
                                StartCoroutine(SpawnBoxAfterDelay(1));//ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).BoxTime
                                isBoxNumOne = true;
                            }
                        }
                        if (waveKey == 6)
                        {
                            //TTOD1执行生成门的逻辑
                            if (!isBuffNumFive)
                            {
                                BoxNumWave = waveKey;
                            }
                            if (ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Door && !isBuffNumFive)
                            {
                                StartCoroutine(SpawnBuffDoorAfterDelay(0));
                                isBuffNumFive = true;
                            }
                            if (ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Box && !isBoxNumTwo)
                            {
                                StartCoroutine(SpawnBoxAfterDelay(1));//ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).BoxTime
                                isBoxNumTwo = true;
                            }
                        }
                        if (waveKey == 7)
                        {
                            //TTOD1执行生成门的逻辑
                            if (!isBuffNumSix)
                            {
                                DoorNumWave = waveKey;
                            }
                            if (ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Door && !isBuffNumSix)
                            {
                                StartCoroutine(SpawnBuffDoorAfterDelay(0));
                                isBuffNumSix = true;
                            }
                        }
                    }
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }
    }
    private IEnumerator SpawnBuffDoorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManage.Instance.SpawnBuffDoor();
    }
    private IEnumerator SpawnBoxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManage.Instance.SpawnChest();
        if (BoxNumWave == 5)
            StartCoroutine(BoxNotePlay(5));
    }
    private IEnumerator BoxNotePlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameMainPanelController.BoxNote_F.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        gameMainPanelController.BoxNote_F.gameObject.SetActive(false);
    }
    //private IEnumerator IE_PlayEnemies()
    //{
    //    float spawnDelay = 0f;
    //    for (int waveIndex = 0; waveIndex < LevelManager.Instance.levelData.Monsterwaves.Count; waveIndex++)
    //    {
    //        while (isFrozen) // 冰冻期间停在这里
    //        {
    //            yield return null; // 等待一帧
    //        }
    //        int waveKey = LevelManager.Instance.levelData.Monsterwaves[waveIndex];
    //        List<List<int>> enemyTypes = LevelManager.Instance.levelData.WavesenEmiesDic[waveKey];
    //        第八波出现强力门
    //        if (waveKey % 10 == 7)
    //        {
    //            GameObject ChestObj = Instantiate(LevelManager.Instance.levelData.PowbuffDoor, new Vector3(-0.08f, 7f, 0f), Quaternion.identity);
    //            FixSortLayer(ChestObj);
    //        }
    //        遍历波次
    //        for (int i = 0; i < enemyTypes.Count; i++)
    //        {
    //            List<int> enemyTypestwo = enemyTypes[i];//enemyTypestwo拿到的是怪物idList

    //            遍历波次中的敌人列表
    //            for (int j = 0; j < enemyTypestwo.Count; j++)
    //            {
    //                if (enemyTypestwo[j] != 0)
    //                {
    //                    获取该类型敌人的配置信息
    //                   var enemyConfig = ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey);
    //                    waveEnemyCount = LevelManager.Instance.levelData.WaveEnemyCountDic[waveKey][j];
    //                    GenerationIntervalEnemy = enemyConfig.Time / 1000f / LevelManager.Instance.levelData.WaveEnemyAllNumList[waveKey];
    //                    spawnDelay = enemyConfig.Time / 1000f;
    //                    按照生成间隔生成该类型的所有敌人
    //                    for (int q = 0; q < waveEnemyCount; q++)
    //                    {
    //                        string enemyName = GameFlowManager.Instance.GetSpwanPre(enemyTypestwo[j]);
    //                        if (enemyPools.TryGetValue(enemyName, out var selectedEnemyPool))
    //                        {
    //                            PlayEnemy(selectedEnemyPool);
    //                        }
    //                        else
    //                        {
    //                            Debug.LogWarning($"Enemy pool not found for: {enemyName}");
    //                        }

    //                        yield return new WaitForSeconds(GenerationIntervalEnemy); // 等待下一个敌人的生成间隔
    //                        CurwavEnemyNum = 0; // 重置当前波次的敌人计数器
    //                    }

    //                }
    //            }
    //        }
    //        yield return new WaitForSeconds(spawnDelay);
    //        Debug.Log(waveIndex + "波次完成========================");
    //        每波敌人延迟
    //    }
    //    Debug.Log("所有波次完成========================");
    //}



    private IEnumerator IE_PlayBullet()
    {
        float elapsedTime = 0f;
        while (true)
        {
            
            Debug.Log($"发射间隔系数=====================: {PlayInforManager.Instance.playInfor.attackSpFac}");
            Debug.Log($"子弹发射间隔=====================: {GenerationIntervalBullet}");
            if (isCreatePool && activeEnemyCount > 0 && GameManage.Instance.gameState == GameState.Running)
            {
                elapsedTime += Time.deltaTime; // 累积时间

                // 检查累积时间是否达到了发射间隔
                if (elapsedTime >= GenerationIntervalBullet)
                {
                    Gun currentGun = PlayInforManager.Instance.playInfor.currentGun;

                    if (currentGun != null)
                    {
                        string bulletKey = currentGun.bulletType;

                        if (bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
                        {
                            Shoot(selectedBulletPool, bulletKey);
                            Debug.Log($"子弹发射间隔elapsedTime=====================: {elapsedTime}");
                        }
                        else
                        {
                            Debug.LogWarning($"Bullet pool not found for: {bulletKey}");
                        }
                    }
                    elapsedTime = 0f; // 重置累积时间
                }
            }

            yield return null; // 每帧更新
        }
    }


    private void PlayEnemy(ObjectPool<GameObject> enemyPool)
    {
        GameObject enemy = enemyPool.Get();
        if (enemy.name == "boss_blue(Clone)")
            enemy.transform.position = RandomPosition(EnemyPoint) - new Vector3(0,1f,0f);
        else
            enemy.transform.position = RandomPosition(EnemyPoint);
        enemy.SetActive(true);
        FixSortLayer(enemy);
        //IncrementActiveEnemy();
    }
    public Vector3 RandomPosition(Vector3 Essentialpos)
    {
        float randomX = Random.Range(-horizontalRange, horizontalRange);
        return new Vector3(Essentialpos.x + randomX, Essentialpos.y, 0);
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
        Debug.Log(activeEnemyCount + "可视化数量=================");
    }

    /// <summary>
    /// 减少活跃敌人数量
    /// </summary>
    public void DecrementActiveEnemy()
    {
        activeEnemyCount --;
        activeEnemyCount = Mathf.Max(activeEnemyCount, 0);
    }

    #region 新增 Beginerlevel 方法和相关逻辑

    // 新增方法：Beginnerlevel
    public async void Beginnerlevel(int waveKey, int waveEnemyCount,int enemyId)
    {
        if (waveKey == 1 && enemyId == 1 && waveEnemyCount == 3 && !isFistNoteOne)//
        {
            isFistNoteOne = true;
            Time.timeScale = 0;
            gameMainPanelController.HighLight.SetActive(true);
            gameMainPanelController.CoinNote_F.SetActive(true);
            // TTOD1 在此处增加逻辑
            HandleBeginnerLevelOne();
        }
        if (waveKey == 2 && enemyId == 3 && waveEnemyCount == 2 &&  !isFistNoteTwo)
        {
            isFistNoteTwo = true;
            gameMainPanelController.KillNote_F.SetActive(true);
            // TTOD1 在此处增加逻辑
            HandleBeginnerLevelTwo();
        }
        if (waveKey == 3 && enemyId == 4 && waveEnemyCount == 1 && !isFistNoteThree)
        {
            isFistNoteThree = true;
            Time.timeScale = 0;
            gameMainPanelController = FindObjectOfType<GameMainPanelController>();
            // TTOD1 在此处增加逻辑
            HandleBeginnerLevelThree().Forget();
        }
    }

    // 新增异步方法：HandleBeginnerLevel
    private async UniTask HandleBeginnerLevelOne()
    {
        // 等待2秒
        Debug.Log("“点击任意位置继续”文字前的时间" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        await UniTask.Delay(TimeSpan.FromSeconds(2), ignoreTimeScale: true);
        Debug.Log("“点击任意位置继续”文字两秒后的时间" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        gameMainPanelController = FindObjectOfType<GameMainPanelController>();

        // 显示“点击任意位置继续”文字
        gameMainPanelController.ContinueTextOne_F.gameObject.SetActive(true);
        // 等待玩家点击任意位置
        await WaitForAnyClick();
        gameMainPanelController.CoinNote_F.SetActive(false);
        gameMainPanelController.HighLight.SetActive(false);
        gameMainPanelController.ContinueTextOne_F.gameObject.SetActive(false);
        // 显示HighLightPlayer高亮图片
        gameMainPanelController.HighLightPlayer.SetActive(true);
        gameMainPanelController.CoinNoteImg2_F.SetActive(true);

        SetHIghtPlayerPos(gameMainPanelController.HighLightPlayer.GetComponent<RectTransform>());
        // 等待2秒
        await UniTask.Delay(TimeSpan.FromSeconds(2), ignoreTimeScale: true);
        // 再次显示“点击任意位置继续”文字
        gameMainPanelController.ContinueTextTwo_F.gameObject.SetActive(true);
        // 等待玩家再次点击任意位置
        await WaitForAnyClick();
        // 隐藏所有高亮图片和“继续”文字
        gameMainPanelController.HighLightPlayer.SetActive(false);
        gameMainPanelController.CoinNoteImg2_F.SetActive(false);
        gameMainPanelController.ContinueTextTwo_F.gameObject.SetActive(false);
        // 恢复游戏
        Time.timeScale = 1f;
    }

    // 新增异步方法：等待任意点击
    private async UniTask WaitForAnyClick()
    {
        // 创建一个等待条件的 UniTaskCompletionSource
        var task = new UniTaskCompletionSource();
        // 定义回调
        void OnClick()
        {
            task.TrySetResult();
        }
        // 注册点击事件
        // 支持鼠标和触摸输入
        // 创建一个临时 GameObject 来监听点击
        GameObject clickListener = new GameObject("ClickListener");
        clickListener.transform.SetParent(this.transform); // 设置父对象
        clickListener.AddComponent<CanvasRenderer>();
        clickListener.AddComponent<GraphicRaycaster>();
        // 添加一个组件来监听点击
        ClickDetector detector = clickListener.AddComponent<ClickDetector>();
        detector.OnClick += OnClick;
        // 等待点击
        await task.Task;
        // 清理
        detector.OnClick -= OnClick;
        Destroy(clickListener);
    }
    void SetHIghtPlayerPos(RectTransform Rectobj)
    {
        // 按住鼠标左键时，跟随鼠标移动
        Vector3 mousePos = GameObject.Find("Player").transform.position;
        // 将世界位置转换为屏幕位置
        Vector3 screenPos = mainCamera.WorldToScreenPoint(mousePos);

        // 将屏幕位置转换为 Canvas 的本地位置
        Vector2 localPoint;
        RectTransform canvasRect = gameMainPanelController.canvasRectTransform;
        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint); // 摄像机参数为 null

        if (isInside)
        {
            // 设置 HighLightPlayer 的位置
            if (Rectobj != null)
            {
                if (Rectobj.name == "RedBoxBtn_F")
                    localPoint.y += 80;
                Rectobj.anchoredPosition = localPoint;

            }
            else
            {
                Debug.LogError("HighLightPlayer does not have a RectTransform component.");
            }
        }
    }

    private async UniTask HandleBeginnerLevelTwo()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(4), ignoreTimeScale: true);
        // 显示“点击任意位置继续”文字
        gameMainPanelController.KillNote_F.SetActive(false);
    }
    private async UniTask HandleBeginnerLevelThree()
    {
        Debug.Log("HandleBeginnerLevelThree called");

        if (gameMainPanelController != null)
        {
            // 显示 RedBoxBtn_F 和 ChooseGunNote_F
            gameMainPanelController.RedBoxBtn_F.gameObject.SetActive(true);
            gameMainPanelController.ChooseGunNote_F.SetActive(true);
            SetHIghtPlayerPos(gameMainPanelController.RedBoxBtn_F.GetComponent<RectTransform>());

            // 启动 ChooseFinger_F 的移动动画
            gameMainPanelController.StartChooseFingerAnimation();
            // 等待玩家长按 RedBoxBtn_F
            await gameMainPanelController.WaitForRedBoxLongPress();

            Debug.Log("RedBoxBtn_F long press detected");

            // 显示 ChooseGun_F 和 ChooseMaxBtn_F
            gameMainPanelController.ChooseGun_F.SetActive(true);
            gameMainPanelController.ChooseMaxBtn_F.gameObject.SetActive(true);
           // gameMainPanelController.ChooseMaxBtn_F.GetComponent<RectTransform>().anchoredPosition = gameMainPanelController.RedBoxBtn_F.GetComponent<RectTransform>().anchoredPosition;
            // 停止 ChooseFinger_F 的动画
            gameMainPanelController.StopChooseFingerAnimation();

            // 添加 ChooseMaxBtn_F 的点击监听器
            gameMainPanelController.ChooseMaxBtn_F.onClick.AddListener(OnChooseMaxBtnClicked);
        }
    }

    // 点击 ChooseMaxBtn_F 时的回调方法
    private void OnChooseMaxBtnClicked()
    {
       StartCoroutine(HideChooseMaxButton());
    }

    // 协程用于隐藏 ChooseMaxBtn_F
    private IEnumerator HideChooseMaxButton()
    {
        // 等待约1秒，使用非缩放时间
        yield return new WaitForSecondsRealtime(1f);
        if (gameMainPanelController != null)
        {
            gameMainPanelController.ChooseMaxBtn_F.onClick.RemoveListener(OnChooseMaxBtnClicked);
            gameMainPanelController.ChooseMaxBtn_F.gameObject.SetActive(false);
            gameMainPanelController.ChooseGun_F.gameObject.SetActive(false);
            gameMainPanelController.ChooseGunNote_F.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }
    #endregion
}
