using Cysharp.Threading.Tasks;
using DragonBones;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Transform = UnityEngine.Transform;

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
    public async UniTask Init(List<GameObject> enemyPrefabs, List<GameObject> bulletPrefabs, List<GameObject> CoinPrefabs)
    {
        isAddIE = false;
        isCreatePool = false;
        EnemyPoint = LevelManager.Instance.levelData.enemySpawnPoints;
        FirePoint = GameObject.Find("Player/FirePoint").transform;
        mainCamera = Camera.main;
        BulletPos = GameObject.Find("AllPre/BulletPre").transform;
        EnemyPos = GameObject.Find("AllPre/EnemyPre").transform;
        CoinPar = GameObject.Find("AllPre/CoinPre").transform;
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
        long bulletCost = ConfigManager.Instance.Tables.TablePlayerConfig.Get(PlayInforManager.Instance.playInfor.level).Total;
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
    }

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
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }
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
        while (true)
        {
            GenerationIntervalBullet = (float)(ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Cd / 1000f * (1 + PlayInforManager.Instance.playInfor.attackSpFac));
            Debug.Log($"�ӵ�������=====================: {GenerationIntervalBullet}");

            if (isCreatePool && activeEnemyCount > 0)
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
            yield return new WaitForSeconds(GenerationIntervalBullet);
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
}

