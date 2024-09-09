using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
    public bool isAddIE = false;
    public bool isCreatePool = false;
    public async UniTask Init(List<GameObject> enemyPrefabs, List<GameObject> bulletPrefabs)
    {
        isCreatePool = false;
        EnemyPoint = LevelManager.Instance.levelData.enemySpawnPoints;
        FirePoint = GameObject.Find("Player/player_001/weapon-01/FirePoint").transform;
        mainCamera = Camera.main;
        BulletPos = GameObject.Find("AllPre/BulletPre").transform;
        EnemyPos = GameObject.Find("AllPre/EnemyPre").transform;
        await CreatePools(enemyPrefabs, bulletPrefabs);
        StartGame();
    }


    #region 创建TNT的相关代码
    private GameObject CreateTNTPrefab(GameObject b, Transform tntParent)
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
        //TTOD1待增加不同子弹耗费金币判断
        long bulletCost = ConfigManager.Instance.Tables.TablePlayerLevelRes.Get(0).Total;
        // 检查玩家是否有足够的金币
        if (PlayInforManager.Instance.playInfor.SpendCoins(bulletCost))
        {
            GameObject Bullet = selectedBulletPool.Get();
            Bullet.SetActive(true);
            Bullet.transform.position = FirePoint.position;
        }
        else
        {
            Debug.LogWarning("Not enough coins to shoot the bullet.");
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
    private async UniTask CreatePools(List<GameObject> enemyPrefabs, List<GameObject> bulletPrefabs)
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

        PreWarmPools(bulletPools.Values);
        PreWarmPools(enemyPools.Values);
    }

    private void PreWarmPools(IEnumerable<ObjectPool<GameObject>> pools)
    {
        int readyCount = 100;
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
        float spawnDelay = 0f;
        for (int waveIndex = 0; waveIndex < LevelManager.Instance.levelData.Monsterwaves.Count; waveIndex++)
        {
            int waveKey = LevelManager.Instance.levelData.Monsterwaves[waveIndex];
            List<List<int>> enemyTypes = LevelManager.Instance.levelData.WavesenEmiesDic[waveKey];

            // 遍历波次
            for (int i = 0; i < enemyTypes.Count; i++)
            {
                List<int> enemyTypestwo = enemyTypes[i];

                // 遍历波次中的敌人列表
                for (int j = 0; j < enemyTypestwo.Count; j++)
                {
                    if (enemyTypestwo[j] != 0)
                    {
                        // 获取该类型敌人的配置信息
                        var enemyConfig = ConfigManager.Instance.Tables.TableEnemyReslevelConfig.Get(enemyTypestwo[j]);
                        waveEnemyCount = enemyConfig.Count;
                        GenerationIntervalEnemy = enemyConfig.Interval / 1000f;
                        spawnDelay = enemyConfig.Delay / 1000f;
                        // 按照生成间隔生成该类型的所有敌人
                        for(int q = 0; q < enemyConfig.MonsterId.Count; q++)
                        {
                            for (int k = 0; k < waveEnemyCount; k++)
                            {
                                string enemyName = GameFlowManager.Instance.GetSpwanPre(enemyConfig.MonsterId[q]);
                                if (enemyPools.TryGetValue(enemyName, out var selectedEnemyPool))
                                {
                                    PlayEnemy(selectedEnemyPool);
                                }
                                else
                                {
                                    Debug.LogWarning($"Enemy pool not found for: {enemyName}");
                                }

                                yield return new WaitForSeconds(GenerationIntervalEnemy); // 等待下一个敌人的生成间隔
                            }
                            CurwavEnemyNum = 0; // 重置当前波次的敌人计数器
                        }
                       
                    }
                }
                yield return new WaitForSeconds(spawnDelay);
            }

            Debug.Log(waveIndex + "波次完成========================");
            // 每波敌人延迟
        }
        Debug.Log("所有波次完成========================");
    }



    private IEnumerator IE_PlayBullet()
    {
        GenerationIntervalBullet = 0.15f;
        while (true)
        {
            if (isCreatePool)
            {
                foreach (var bulletKey in LevelManager.Instance.levelData.GunBulletList)
                {
                    if (bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
                    {
                        Shoot(selectedBulletPool, bulletKey);
                    }
                    else
                    {
                        Debug.LogWarning($"Bullet pool not found for: {bulletKey}");
                    }
                    yield return new WaitForSeconds(GenerationIntervalBullet);
                }
            }
        }
    }

    private void PlayEnemy(ObjectPool<GameObject> enemyPool)
    {
        GameObject enemy = enemyPool.Get();
        enemy.transform.position = RandomPosition(EnemyPoint);
        enemy.SetActive(true);
        CurwavEnemyNum++;
    }
    private Vector3 RandomPosition(Vector3 Essentialpos)
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
}

