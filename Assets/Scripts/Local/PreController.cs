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
    public float horizontalRange = 1.14f; // X������ƫ�Ʒ�Χ
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
    public bool isRageSkill = false; // ��ӱ���״̬����
    public int activeEnemyCount = 0;
    public int currentSortingOrder = 1000; // ��ʼ��һ���ϸߵ�����˳��
    public GameMainPanelController gameMainPanelController;
    public bool isFistNoteOne = false; // ��ӱ���״̬����
    public bool isFistNoteTwo = false; // ��ӱ���״̬����
    public bool isFistNoteThree = false; // ��ӱ���״̬����

    //��Ӵ洢������ӵ�
    public List<BulletController> flyingBullets = new List<BulletController>();
    // ��������־�Ƿ��ѷ����һ���ӵ�
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
    public bool isBulletCostZero = false; // �Ƿ��ӵ������Ľ��
    public bool isFiring = false; // �Ƿ��ӵ������Ľ��
                                  // ����һ���¼�
    public event Action OnPlayerFiring;
    void Shoot(ObjectPool<GameObject> selectedBulletPool, string bulletName)
    {
        long bulletCost = (long)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(PlayInforManager.Instance.playInfor.level).Total);
        if (isBulletCostZero)
        {
            bulletCost = 0;
        }
        // �������Ƿ����㹻�Ľ��
        if (PlayInforManager.Instance.playInfor.SpendCoins(bulletCost))
        {
            GameObject Bullet = selectedBulletPool.Get();
            Bullet.SetActive(true);
            AudioManage.Instance.PlaySFX("ak", null);
            FixSortLayer(Bullet);
            Bullet.transform.position = FirePoint.position;
            EventDispatcher.instance.DispatchEvent(EventNameDef.ShowBuyBulletText);
            // �����ӵ�ʱ�������¶���״̬
            isFiring = true; // ��Ƿ���״̬
            // ���������¼�
            OnPlayerFiring?.Invoke();  // ֪ͨPlayerController���¶���

            // ���������ӵ���������б�
            BulletController bulletController = Bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                flyingBullets.Add(bulletController);
                bulletController.OnBulletDestroyed += HandleBulletDestroyed; // ע���ӵ������¼�
            }
            #region[��ʾ��2]
            //// �������룺�������һ���ӵ�ʱ����ʾ TwoNote_F
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
            Debug.Log("��Ҳ��㣬�޷������ӵ���");
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
            // �����ǰ�ؿ��ǵ�0���� FirstNote_FBool Ϊ false����ȴ� FirstNote_FBool ��Ϊ true
            if (GameFlowManager.Instance.currentLevelIndex == 0 && gameMainPanelController != null && !gameMainPanelController.FirstNote_FBool)
            {
                Debug.Log("��0���� FirstNote_FBool Ϊ false����ͣ�������ɣ��ȴ� FirstNote_F �����ʾ");
                // �ȴ�ֱ�� FirstNote_FBool ��Ϊ true
                while (!gameMainPanelController.FirstNote_FBool)
                {
                    yield return null;
                }
                Debug.Log("FirstNote_F ����ʾ��������������");
            }
            if (GameFlowManager.Instance.currentLevelIndex == 0 && gameMainPanelController != null && gameMainPanelController.TwoNote_FBool)
            {
                Debug.Log("��0���� TwoNote_FBool Ϊ false����ͣ�������ɣ��ȴ� TwoNote_FBool �����ʾ");
                // �ȴ�ֱ�� FirstNote_FBool ��Ϊ true
                while (gameMainPanelController.TwoNote_FBool)
                {
                    yield return null;
                }
                Debug.Log("TwoNote_FBool ����ʾ��������������");
            }
            //if (GameFlowManager.Instance.currentLevelIndex == 0 && gameMainPanelController != null && gameMainPanelController.ThreeNote_FBool)
            //{
            //    Debug.Log("��0���� ThreeNote_FBool Ϊ false����ͣ�������ɣ��ȴ� ThreeNote_FBool �����ʾ");
            //    // �ȴ�ֱ�� FirstNote_FBool ��Ϊ true
            //    while (gameMainPanelController.ThreeNote_FBool)
            //    {
            //        yield return null;
            //    }
            //    Debug.Log("ThreeNote_FBool ����ʾ��������������");
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

        // ���㵱ǰ�����ܵ����������ɼ��
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
            Debug.Log("�ò��ܵ�����: " + totalEnemies);
        }

        foreach (var enemyId in enemyTypestwo)
        {
            if (enemyId != 0)
            {
                int waveEnemyCount = LevelManager.Instance.levelData.WaveEnemyCountDic[waveKey][ListIndex];
                Debug.Log($"����" + waveKey + "����" + waveEnemyCount);

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
                            Debug.Log("��0���� ThreeNote_FBool Ϊ false����ͣ�������ɣ��ȴ� ThreeNote_FBool �����ʾ");
                            // �ȴ�ֱ�� FirstNote_FBool ��Ϊ true
                            while (gameMainPanelController.ThreeNote_FBool)
                            {
                                yield return null;
                            }
                            Debug.Log("ThreeNote_FBool ����ʾ��������������");
                        }
                        if (GameFlowManager.Instance.currentLevelIndex == 0 && waveKey  == 3 && gameMainPanelController != null && !gameMainPanelController.TwoBeThree)
                        {
                            Debug.Log("��0���� ThreeNote_FBool Ϊ false����ͣ�������ɣ��ȴ� ThreeNote_FBool �����ʾ");
                            // �ȴ�ֱ�� FirstNote_FBool ��Ϊ true
                            while (!gameMainPanelController.TwoBeThree)
                            {
                                yield return null;
                            }
                            Debug.Log("ThreeNote_FBool ����ʾ��������������");
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
                            Debug.LogWarning($"δ�ҵ����˳�: {enemyName}");
                        }
                        #region[ԭ����ʾ3]
                        ////TTOD1���ֹ����⴦��
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
                        //    //�����������ɱ��
                        //    if (waveKey < 3)
                        //    {

                        //        Beginnerlevel(waveKey, q, enemyId, waveEnemyCount);
                        //    }
                        //    if (waveKey == 3)
                        //    {
                        //        //TTOD1ִ�������ŵ��߼�
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
                        //        //TTOD1ִ�������ŵ��߼�
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
                        #endregion[ԭ����ʾ3]
                        yield return new WaitForSecondsRealtime(spawnInterval);
                        Debug.Log($"���˼��: {spawnInterval}");
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
    

    public int DoorNumWave;//������buffid
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

                    // ����������ӵ������˺�С�ڵ���������ֵ����������
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
                        // �����е��ӵ���������ǰ�����ˣ����ٿ���
                        isFiring = false;
                        // ���������¼�
                        OnPlayerFiring?.Invoke();  // ֪ͨPlayerController���¶���

                    }
                }
                else
                {
                    // ��ǰ��û�е��ˣ�������
                    isFiring = false;
                    // ���������¼�
                    OnPlayerFiring?.Invoke();  // ֪ͨPlayerController���¶���

                }
            }
            // �ȴ�������
            yield return new WaitForSecondsRealtime(GenerationIntervalBullet);
        }
    }


    public void RestartIEPlayBullet()
    {
        // ֹͣ��ǰ��Э��
        if (playBulletCoroutineId != -1)
        {
            IEnumeratorTool.StopCoroutine(playBulletCoroutineId);
        }
        // ��������Э��
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
        Debug.Log("�����ĵ�������" + InNu);
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
        Debug.Log(activeEnemyCount + "���ӻ�����2=================");
    }

    /// <summary>
    /// ���ٻ�Ծ��������
    /// </summary>
    public void DecrementActiveEnemy()
    {
        initialLevEneNun++;
        Debug.Log(initialLevEneNun + "initialLevEneNun���ӻ�����=================");
        activeEnemyCount --;
        activeEnemyCount = Mathf.Max(activeEnemyCount, 0);
    }

    #region ���� Beginerlevel ����������߼�
    public IEnumerator HandleBeginnerLevelTwo()
    {
        if (gameMainPanelController != null)
        {
            
            yield return StartCoroutine(gameMainPanelController.ShowThreeNote());
        }
        else
        {
            Debug.LogError("GameMainPanelController δ��ֵ��");
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
            Debug.Log("GameMainPanelController δ��ֵ��");
        }
    }


    #endregion


    // ��������ǰ��һ����Χ���Ƿ��е���
    private bool IsEnemyInFront(float HorizdetectionRange,float VertialdetectionRange)
    {
        // ��ȡ���λ��
        Vector3 playerPosition = FirePoint.position;
        //Vector3 playerPosition = GameObject.FindGameObjectWithTag("FirePoint").transform.position;
        // ��������������½Ǻ����Ͻ�
        Vector2 pointA = new Vector2(playerPosition.x - HorizdetectionRange, playerPosition.y);
        Vector2 pointB = new Vector2(playerPosition.x + HorizdetectionRange, playerPosition.y + VertialdetectionRange);
        // ����������ڵ�Layer
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        int ChestLayerMask = LayerMask.GetMask("Chest");
        // ��ȡ��������ڵ�������ײ��
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayerMask | ChestLayerMask);

        foreach (var collider in colliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                return true; // ��Χ���е���
            }
        }
        return false; // ��Χ��û�е���

    }


    //�����������ֵ
    private float GetTotalFlyingBulletDamage(float HoridetectionRange, float VertialdetectionRange)
    {
        float totalDamage = 0f;
        // ��ȡ���λ��
        Vector3 playerPosition = FirePoint.position;
        //Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Debug.Log("��ҵ�λ����Ϣ" + playerPosition);
        // ��������������½Ǻ����Ͻ�
        Vector2 pointA = new Vector2(playerPosition.x - HoridetectionRange, playerPosition.y);
        Vector2 pointB = new Vector2(playerPosition.x + HoridetectionRange, playerPosition.y + VertialdetectionRange);
        // ����������ڵ�Layer
        int bulletLayerMask = LayerMask.GetMask("Bullet");
        // ��ȡ��������ڵ�������ײ��
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

        // ��ȡ���λ��
        Vector3 playerPosition = FirePoint.position;
        //Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Debug.Log("��ҵ�λ����Ϣ" + playerPosition);
        // ��������������½Ǻ����Ͻ�
        Vector2 pointA = new Vector2(playerPosition.x - HoridetectionRange, playerPosition.y);
        Vector2 pointB = new Vector2(playerPosition.x + HoridetectionRange, playerPosition.y + VertialdetectionRange);

        // ����������ڵ�Layer
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        int ChestLayerMask = LayerMask.GetMask("Chest");
        // ��ȡ��������ڵ�������ײ��
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

    // ��������������׷���ӵ�
    // �޸� SpawnHomingBullet ����
    public void SpawnHomingBullet(Vector3 position, Transform target)
    {
        Gun currentGun = PlayInforManager.Instance.playInfor.currentGun;
        if (currentGun != null)
        {
            string bulletKey = currentGun.bulletType;

            if (bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
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

                    // �����ӵ���Ŀ��
                    BulletController bulletController = Bullet.GetComponent<BulletController>();
                    if (bulletController != null)
                    {
                        bulletController.SetTarget(target);
                        // ���������ӵ���������б�ע�������¼�
                        flyingBullets.Add(bulletController);
                        bulletController.OnBulletDestroyed += HandleBulletDestroyed;
                    }
                }
                else
                {
                    Debug.Log("��Ҳ��㣬�޷������ӵ���");
                }
            }
        }
    }
}
