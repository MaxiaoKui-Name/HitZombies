using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyType
{
    Cuipi
}
public class PreController : Singleton<PreController>
{

    public ObjectPool<GameObject> EnemyPool;
    public ObjectPool<GameObject> BulletPool;
    public List<int> IEList = new List<int>();
    public Transform EnemyPoint;     // 敌人发射点
    public float horizontalRange = 1.3f; // X轴的随机偏移范围
    public GameObject bulletPrefab; // 子弹预制件
    public GameObject EnemyPrefab; // 敌人预制体
    private Camera mainCamera;
    public float screenBoundaryOffset = 1f;  // 允许子弹稍微超出屏幕的边界再隐藏

   
    private void Start()
    {
        mainCamera = Camera.main;
        CreatePre();
        StartGame();
    }
    public void CreatePre()
    {
        EnemyPoint = GameObject.Find("EnemyPoint").transform;
        EnemyPool = new ObjectPool<GameObject>(() => CreateTNTPrefab(EnemyPrefab, transform.GetChild(0)), Get, Release, MyDestroy, true, 200, 2000);
        BulletPool = new ObjectPool<GameObject>(() => CreateTNTPrefab(bulletPrefab, transform.GetChild(1)), Get, Release, MyDestroy, true, 200, 2000);

        int readyCount = 1000;
        List<GameObject> tempObjList = new List<GameObject>(readyCount);

        // 预支敌人
        readyCount = 500;
        for (int j = 0; j < readyCount; j++)
            tempObjList.Add(EnemyPool.Get());
        for (int j = 0; j < tempObjList.Count; j++)
            EnemyPool.Release(tempObjList[j]);
        tempObjList.Clear();

        // 预支子弹
        readyCount = 100;
        for (int j = 0; j < readyCount; j++)
            tempObjList.Add(BulletPool.Get());
        for (int j = 0; j < tempObjList.Count; j++)
            BulletPool.Release(tempObjList[j]);
        tempObjList.Clear();
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
        IEList.Add(IEnumeratorTool.StartCoroutine(IE_PlayEnemy1()));
    }

    private IEnumerator IE_PlayEnemy1()
    {

        while (true)
        {
            PlayEnemy(EnemyPool);
            // AudioManager.Instance.Play(AudioManager.sound_shoot);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void PlayEnemy(ObjectPool<GameObject> enemyPool)
    {
        GameObject enemy = enemyPool.Get();
        enemy.transform.position = RandomPosition(EnemyPoint);
        enemy.SetActive(true);
    }

    private Vector3 RandomPosition(Transform Essentialpos)
    {
        // 在EnemyPoint位置的左右范围内生成一个随机的X轴偏移
        float randomX = Random.Range(-horizontalRange, horizontalRange);

        // 返回随机生成的位置
        return new Vector3(Essentialpos.position.x + randomX, Essentialpos.position.y, 0);
    }

    //判断怪物是否超出边界
   public void DignoExtre(ObjectPool<GameObject> objPool,GameObject objPre)
    {
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(objPre.transform.position);
        if (screenPosition.y < -screenBoundaryOffset || screenPosition.y > 1 + screenBoundaryOffset ||
            screenPosition.x < -screenBoundaryOffset || screenPosition.x > 1 + screenBoundaryOffset)
        {
            // 子弹超出屏幕边界，隐藏并返回对象池
            HideAndReturnToPool(objPool, objPre);
        }
    }
    public void HideAndReturnToPool(ObjectPool<GameObject>objPool,GameObject objPre)
    {
        // 隐藏子弹并返回对象池
        objPre.SetActive(false);
        objPool.Release(objPre);
    }
}
