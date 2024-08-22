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
    public Transform EnemyPoint;     // ���˷����
    public float horizontalRange = 1.3f; // X������ƫ�Ʒ�Χ
    public GameObject bulletPrefab; // �ӵ�Ԥ�Ƽ�
    public GameObject EnemyPrefab; // ����Ԥ����
    private Camera mainCamera;
    public float screenBoundaryOffset = 1f;  // �����ӵ���΢������Ļ�ı߽�������

   
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

        // Ԥ֧����
        readyCount = 500;
        for (int j = 0; j < readyCount; j++)
            tempObjList.Add(EnemyPool.Get());
        for (int j = 0; j < tempObjList.Count; j++)
            EnemyPool.Release(tempObjList[j]);
        tempObjList.Clear();

        // Ԥ֧�ӵ�
        readyCount = 100;
        for (int j = 0; j < readyCount; j++)
            tempObjList.Add(BulletPool.Get());
        for (int j = 0; j < tempObjList.Count; j++)
            BulletPool.Release(tempObjList[j]);
        tempObjList.Clear();
    }

    #region ����TNT����ش���
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
        // ��EnemyPointλ�õ����ҷ�Χ������һ�������X��ƫ��
        float randomX = Random.Range(-horizontalRange, horizontalRange);

        // ����������ɵ�λ��
        return new Vector3(Essentialpos.position.x + randomX, Essentialpos.position.y, 0);
    }

    //�жϹ����Ƿ񳬳��߽�
   public void DignoExtre(ObjectPool<GameObject> objPool,GameObject objPre)
    {
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(objPre.transform.position);
        if (screenPosition.y < -screenBoundaryOffset || screenPosition.y > 1 + screenBoundaryOffset ||
            screenPosition.x < -screenBoundaryOffset || screenPosition.x > 1 + screenBoundaryOffset)
        {
            // �ӵ�������Ļ�߽磬���ز����ض����
            HideAndReturnToPool(objPool, objPre);
        }
    }
    public void HideAndReturnToPool(ObjectPool<GameObject>objPool,GameObject objPre)
    {
        // �����ӵ������ض����
        objPre.SetActive(false);
        objPool.Release(objPre);
    }
}
