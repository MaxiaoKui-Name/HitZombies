using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyType
{
    CuipiMonster,
    ShortMonster,
    DisMonster,
    ElitesMonster,
    Boss
    // 其他敌人类型...
}
public enum BulletType
{
    TEgaugeBullet,
    // 其他敌人类型...
}


public class PreController : Singleton<PreController>
{
    public Dictionary<string, ObjectPool<GameObject>> enemyPools = new Dictionary<string, ObjectPool<GameObject>>();
    public Dictionary<string, ObjectPool<GameObject>> bulletPools = new Dictionary<string, ObjectPool<GameObject>>();
    public List<int> IEList = new List<int>();
    public Vector3 EnemyPoint;     // 敌人发射点\
    public Transform FirePoint;     // 子弹发射点
    public float horizontalRange = 1.3f; // X轴的随机偏移范围
    private Camera mainCamera;
    public float screenBoundaryOffset = 1f;  // 允许子弹稍微超出屏幕的边界再隐藏
    public float GenerationIntervalEnemy;
    public float GenerationIntervalBullet;
    public int CurrentEnemyNum = 0;
    public int KillEnemyNun = 0;
    public Transform BulletPos;
    public Transform EnemyPos;
    public bool isAddIE = false;
    public bool isCreatePool = false;

    public void Init(List<GameObject> enemyPrefabs, List<GameObject> bulletPrefabs)
    {
        isCreatePool = false;
        EnemyPoint = LevelManager.Instance.levelData.enemySpawnPoints;
        GenerationIntervalEnemy = LevelManager.Instance.levelData.enemySpawnInterval;
        GenerationIntervalBullet = LevelManager.Instance.levelData.BulletInterval;
        FirePoint = GameObject.Find("Player/Hunter/SkeletonUtility-SkeletonRoot/root/Bone_idle/Bone_idle7/gun_2/IK_sung/FirePoint").transform;
        mainCamera = Camera.main;
        BulletPos = GameObject.Find("AllPre/BulletPre").transform;
        EnemyPos = GameObject.Find("AllPre/EnemyPre").transform;
        CreatePools(enemyPrefabs, bulletPrefabs);
        StartGame();
    }

    private void CreatePools(List<GameObject> enemyPrefabs, List<GameObject> bulletPrefabs)
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
        int readyCount = 500;
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

    private IEnumerator IE_PlayBullet()
    {
        while (true)
        {
            if (isCreatePool)
            {
                //for (int i = 0; i < LevelManager.Instance.levelData.bulletAddresses.Length; i++)
                //{
                //    ObjectPool<GameObject> selectedBulletPool = bulletPools[LevelManager.Instance.levelData.bulletAddresses[i]];
                //    Shoot(selectedBulletPool);
                //    yield return new WaitForSeconds(GenerationIntervalBullet);
                // }
            }
        }
    }
    void Shoot(ObjectPool<GameObject> selectedBulletPool)
    {
        GameObject Bullet = selectedBulletPool.Get();
        Bullet.SetActive(true);
        Bullet.transform.position = FirePoint.position;
    }
    private IEnumerator IE_PlayEnemies()
    {
        while (true)
        {
            if (isCreatePool)
            {
                //for (int i = 0; i < LevelManager.Instance.levelData.enemyAddresses.Length; i++)
                //{
                //    while (CurrentEnemyNum < LevelManager.Instance.levelData.enemyNum[i])
                //    {
                //        // 随机选择一个敌人池
                //        ObjectPool<GameObject> selectedEnemyPool = enemyPools[LevelManager.Instance.levelData.enemyAddresses[i]];
                //        PlayEnemy(selectedEnemyPool);
                //        yield return new WaitForSeconds(GenerationIntervalEnemy);
                //    }
                //    CurrentEnemyNum = 0;
                //}
            }
            

        }
    }

    private void PlayEnemy(ObjectPool<GameObject> enemyPool)
    {
        GameObject enemy = enemyPool.Get();
        enemy.transform.position = RandomPosition(EnemyPoint);
        enemy.SetActive(true);
        CurrentEnemyNum++;
    }

    private Vector3 RandomPosition(Vector3 Essentialpos)
    {
        float randomX = Random.Range(-horizontalRange, horizontalRange);
        return new Vector3(Essentialpos.x + randomX, Essentialpos.y, 0);
    }

    //判断怪物是否超出边界
    public void DignoExtre(GameObject objPre)
    {
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(objPre.transform.position);
        if (screenPosition.y < -screenBoundaryOffset || screenPosition.y > 1 + screenBoundaryOffset ||
            screenPosition.x < -screenBoundaryOffset || screenPosition.x > 1 + screenBoundaryOffset)
        {
            if(objPre.layer == 7)
                HideAndReturnToPool(GetBulletPoolMethod(objPre), objPre);
            //if (objPre.layer == 6)
            //    HideAndReturnToPool(GetEnemyPoolMethod(objPre), objPre);

        }
    }

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
            Debug.LogWarning($"No pool found for enemy: {bulletName}");
            return null;
        }
    }
}

