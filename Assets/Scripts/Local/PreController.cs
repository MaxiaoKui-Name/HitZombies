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
    // ������������...
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
    // ������������...
}


public class PreController : Singleton<PreController>
{
    public Dictionary<string, ObjectPool<GameObject>> enemyPools = new Dictionary<string, ObjectPool<GameObject>>();
    public Dictionary<string, ObjectPool<GameObject>> bulletPools = new Dictionary<string, ObjectPool<GameObject>>();
    public Dictionary<string, ObjectPool<GameObject>> CoinPools = new Dictionary<string, ObjectPool<GameObject>>();
    public List<int> IEList = new List<int>();
    public Vector3 EnemyPoint;     // ���˷����
    public Transform FirePoint;     // �ӵ������
    public float horizontalRange = 1.3f; // X������ƫ�Ʒ�Χ
    private Camera mainCamera;
    public float screenBoundaryOffset = 1f;  // �����ӵ���΢������Ļ�ı߽�������
    public int CurwavEnemyNum = 0;
    public int waveEnemyCount = 0;
    public float GenerationIntervalEnemy;
    public float GenerationIntervalBullet;
    public Transform BulletPos;
    public Transform EnemyPos;
    public Transform CoinPar;
    public bool isAddIE = false;
    public bool isCreatePool = false;
    public bool isFrozen = false; // ��ӱ���״̬����

    public int activeEnemyCount = 0;
    public int currentSortingOrder = 1000; // ��ʼ��һ���ϸߵ�����˳��
    public GameMainPanelController gameMainPanelController;
    public bool isFistNoteOne = false; // ��ӱ���״̬����
    public bool isFistNoteTwo = false; // ��ӱ���״̬����
    public bool isFistNoteThree = false; // ��ӱ���״̬����
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


    #region ����TNT����ش���
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
        // �������
        
    }
    public void FixSortLayer(GameObject go)
    {
        // ��ȡ���� Renderer ��������������壩
        UnityArmatureComponent[] renderers = go.GetComponentsInChildren<UnityArmatureComponent>();
        foreach (var renderer in renderers)
        {
            // ��������㼶��������Ҫ���� Sorting Layer ���ƣ�
            renderer.sortingLayerName = "Default"; // ȷ�����ж���ʹ����ͬ�� Sorting Layer
            renderer.sortingOrder = currentSortingOrder;
        }
        // �ݼ�����˳����ȷ���ȼ����������и��ߵ�����˳��
        currentSortingOrder --;
        // ��ѡ����ֹ sortingOrder ���ͣ���������˳��
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
        // �������Ƿ����㹻�Ľ��
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

    //�жϹ����Ƿ񳬳��߽�
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

            // �ȴ����е������ɵ�Э�����
            foreach (var coroutine in enemyCoroutines)
            {
                yield return coroutine; // �ȴ�ÿ��Э�����
            }
            //var enemyConfig = ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey);
            //yield return new WaitForSeconds(ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey).Time / 1000f);
            Debug.Log($"{waveIndex}�������========================");
        }
        Debug.Log("���в������========================");
        //TTOD2����ʹ��
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
                        Debug.LogWarning($"δ�ҵ����˳�: {enemyName}");
                    }
                    //TTOD1���ֹ����⴦��
                    if(GameFlowManager.Instance.currentLevelIndex == 0)
                    {
                        //�����������ɱ��
                        if (waveKey < 5)
                        {
                            Beginnerlevel(waveKey, q, enemyId);
                            if (waveKey == 4 && ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey).Door && !isBuffNumThree)
                            {
                                //TTOD1ִ�������ŵ��߼�
                                DoorNumWave = waveKey;
                                StartCoroutine(SpawnBuffDoorAfterDelay(0));
                                isBuffNumThree = true;
                            }
                        }
                        if (waveKey == 5)
                        {
                            //TTOD1ִ�������ŵ��߼�
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
                            //TTOD1ִ�������ŵ��߼�
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
                            //TTOD1ִ�������ŵ��߼�
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
    //        while (isFrozen) // �����ڼ�ͣ������
    //        {
    //            yield return null; // �ȴ�һ֡
    //        }
    //        int waveKey = LevelManager.Instance.levelData.Monsterwaves[waveIndex];
    //        List<List<int>> enemyTypes = LevelManager.Instance.levelData.WavesenEmiesDic[waveKey];
    //        �ڰ˲�����ǿ����
    //        if (waveKey % 10 == 7)
    //        {
    //            GameObject ChestObj = Instantiate(LevelManager.Instance.levelData.PowbuffDoor, new Vector3(-0.08f, 7f, 0f), Quaternion.identity);
    //            FixSortLayer(ChestObj);
    //        }
    //        ��������
    //        for (int i = 0; i < enemyTypes.Count; i++)
    //        {
    //            List<int> enemyTypestwo = enemyTypes[i];//enemyTypestwo�õ����ǹ���idList

    //            ���������еĵ����б�
    //            for (int j = 0; j < enemyTypestwo.Count; j++)
    //            {
    //                if (enemyTypestwo[j] != 0)
    //                {
    //                    ��ȡ�����͵��˵�������Ϣ
    //                   var enemyConfig = ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey);
    //                    waveEnemyCount = LevelManager.Instance.levelData.WaveEnemyCountDic[waveKey][j];
    //                    GenerationIntervalEnemy = enemyConfig.Time / 1000f / LevelManager.Instance.levelData.WaveEnemyAllNumList[waveKey];
    //                    spawnDelay = enemyConfig.Time / 1000f;
    //                    �������ɼ�����ɸ����͵����е���
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

    //                        yield return new WaitForSeconds(GenerationIntervalEnemy); // �ȴ���һ�����˵����ɼ��
    //                        CurwavEnemyNum = 0; // ���õ�ǰ���εĵ��˼�����
    //                    }

    //                }
    //            }
    //        }
    //        yield return new WaitForSeconds(spawnDelay);
    //        Debug.Log(waveIndex + "�������========================");
    //        ÿ�������ӳ�
    //    }
    //    Debug.Log("���в������========================");
    //}



    private IEnumerator IE_PlayBullet()
    {
        float elapsedTime = 0f;
        while (true)
        {
            
            Debug.Log($"������ϵ��=====================: {PlayInforManager.Instance.playInfor.attackSpFac}");
            Debug.Log($"�ӵ�������=====================: {GenerationIntervalBullet}");
            if (isCreatePool && activeEnemyCount > 0 && GameManage.Instance.gameState == GameState.Running)
            {
                elapsedTime += Time.deltaTime; // �ۻ�ʱ��

                // ����ۻ�ʱ���Ƿ�ﵽ�˷�����
                if (elapsedTime >= GenerationIntervalBullet)
                {
                    Gun currentGun = PlayInforManager.Instance.playInfor.currentGun;

                    if (currentGun != null)
                    {
                        string bulletKey = currentGun.bulletType;

                        if (bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
                        {
                            Shoot(selectedBulletPool, bulletKey);
                            Debug.Log($"�ӵ�������elapsedTime=====================: {elapsedTime}");
                        }
                        else
                        {
                            Debug.LogWarning($"Bullet pool not found for: {bulletKey}");
                        }
                    }
                    elapsedTime = 0f; // �����ۻ�ʱ��
                }
            }

            yield return null; // ÿ֡����
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
        Debug.Log(activeEnemyCount + "���ӻ�����=================");
    }

    /// <summary>
    /// ���ٻ�Ծ��������
    /// </summary>
    public void DecrementActiveEnemy()
    {
        activeEnemyCount --;
        activeEnemyCount = Mathf.Max(activeEnemyCount, 0);
    }

    #region ���� Beginerlevel ����������߼�

    // ����������Beginnerlevel
    public async void Beginnerlevel(int waveKey, int waveEnemyCount,int enemyId)
    {
        if (waveKey == 1 && enemyId == 1 && waveEnemyCount == 3 && !isFistNoteOne)//
        {
            isFistNoteOne = true;
            Time.timeScale = 0;
            gameMainPanelController.HighLight.SetActive(true);
            gameMainPanelController.CoinNote_F.SetActive(true);
            // TTOD1 �ڴ˴������߼�
            HandleBeginnerLevelOne();
        }
        if (waveKey == 2 && enemyId == 3 && waveEnemyCount == 2 &&  !isFistNoteTwo)
        {
            isFistNoteTwo = true;
            gameMainPanelController.KillNote_F.SetActive(true);
            // TTOD1 �ڴ˴������߼�
            HandleBeginnerLevelTwo();
        }
        if (waveKey == 3 && enemyId == 4 && waveEnemyCount == 1 && !isFistNoteThree)
        {
            isFistNoteThree = true;
            Time.timeScale = 0;
            gameMainPanelController = FindObjectOfType<GameMainPanelController>();
            // TTOD1 �ڴ˴������߼�
            HandleBeginnerLevelThree().Forget();
        }
    }

    // �����첽������HandleBeginnerLevel
    private async UniTask HandleBeginnerLevelOne()
    {
        // �ȴ�2��
        Debug.Log("���������λ�ü���������ǰ��ʱ��" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        await UniTask.Delay(TimeSpan.FromSeconds(2), ignoreTimeScale: true);
        Debug.Log("���������λ�ü���������������ʱ��" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        gameMainPanelController = FindObjectOfType<GameMainPanelController>();

        // ��ʾ���������λ�ü���������
        gameMainPanelController.ContinueTextOne_F.gameObject.SetActive(true);
        // �ȴ���ҵ������λ��
        await WaitForAnyClick();
        gameMainPanelController.CoinNote_F.SetActive(false);
        gameMainPanelController.HighLight.SetActive(false);
        gameMainPanelController.ContinueTextOne_F.gameObject.SetActive(false);
        // ��ʾHighLightPlayer����ͼƬ
        gameMainPanelController.HighLightPlayer.SetActive(true);
        gameMainPanelController.CoinNoteImg2_F.SetActive(true);

        SetHIghtPlayerPos(gameMainPanelController.HighLightPlayer.GetComponent<RectTransform>());
        // �ȴ�2��
        await UniTask.Delay(TimeSpan.FromSeconds(2), ignoreTimeScale: true);
        // �ٴ���ʾ���������λ�ü���������
        gameMainPanelController.ContinueTextTwo_F.gameObject.SetActive(true);
        // �ȴ�����ٴε������λ��
        await WaitForAnyClick();
        // �������и���ͼƬ�͡�����������
        gameMainPanelController.HighLightPlayer.SetActive(false);
        gameMainPanelController.CoinNoteImg2_F.SetActive(false);
        gameMainPanelController.ContinueTextTwo_F.gameObject.SetActive(false);
        // �ָ���Ϸ
        Time.timeScale = 1f;
    }

    // �����첽�������ȴ�������
    private async UniTask WaitForAnyClick()
    {
        // ����һ���ȴ������� UniTaskCompletionSource
        var task = new UniTaskCompletionSource();
        // ����ص�
        void OnClick()
        {
            task.TrySetResult();
        }
        // ע�����¼�
        // ֧�����ʹ�������
        // ����һ����ʱ GameObject ���������
        GameObject clickListener = new GameObject("ClickListener");
        clickListener.transform.SetParent(this.transform); // ���ø�����
        clickListener.AddComponent<CanvasRenderer>();
        clickListener.AddComponent<GraphicRaycaster>();
        // ���һ��������������
        ClickDetector detector = clickListener.AddComponent<ClickDetector>();
        detector.OnClick += OnClick;
        // �ȴ����
        await task.Task;
        // ����
        detector.OnClick -= OnClick;
        Destroy(clickListener);
    }
    void SetHIghtPlayerPos(RectTransform Rectobj)
    {
        // ��ס������ʱ����������ƶ�
        Vector3 mousePos = GameObject.Find("Player").transform.position;
        // ������λ��ת��Ϊ��Ļλ��
        Vector3 screenPos = mainCamera.WorldToScreenPoint(mousePos);

        // ����Ļλ��ת��Ϊ Canvas �ı���λ��
        Vector2 localPoint;
        RectTransform canvasRect = gameMainPanelController.canvasRectTransform;
        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint); // ���������Ϊ null

        if (isInside)
        {
            // ���� HighLightPlayer ��λ��
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
        // ��ʾ���������λ�ü���������
        gameMainPanelController.KillNote_F.SetActive(false);
    }
    private async UniTask HandleBeginnerLevelThree()
    {
        Debug.Log("HandleBeginnerLevelThree called");

        if (gameMainPanelController != null)
        {
            // ��ʾ RedBoxBtn_F �� ChooseGunNote_F
            gameMainPanelController.RedBoxBtn_F.gameObject.SetActive(true);
            gameMainPanelController.ChooseGunNote_F.SetActive(true);
            SetHIghtPlayerPos(gameMainPanelController.RedBoxBtn_F.GetComponent<RectTransform>());

            // ���� ChooseFinger_F ���ƶ�����
            gameMainPanelController.StartChooseFingerAnimation();
            // �ȴ���ҳ��� RedBoxBtn_F
            await gameMainPanelController.WaitForRedBoxLongPress();

            Debug.Log("RedBoxBtn_F long press detected");

            // ��ʾ ChooseGun_F �� ChooseMaxBtn_F
            gameMainPanelController.ChooseGun_F.SetActive(true);
            gameMainPanelController.ChooseMaxBtn_F.gameObject.SetActive(true);
           // gameMainPanelController.ChooseMaxBtn_F.GetComponent<RectTransform>().anchoredPosition = gameMainPanelController.RedBoxBtn_F.GetComponent<RectTransform>().anchoredPosition;
            // ֹͣ ChooseFinger_F �Ķ���
            gameMainPanelController.StopChooseFingerAnimation();

            // ��� ChooseMaxBtn_F �ĵ��������
            gameMainPanelController.ChooseMaxBtn_F.onClick.AddListener(OnChooseMaxBtnClicked);
        }
    }

    // ��� ChooseMaxBtn_F ʱ�Ļص�����
    private void OnChooseMaxBtnClicked()
    {
       StartCoroutine(HideChooseMaxButton());
    }

    // Э���������� ChooseMaxBtn_F
    private IEnumerator HideChooseMaxButton()
    {
        // �ȴ�Լ1�룬ʹ�÷�����ʱ��
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
