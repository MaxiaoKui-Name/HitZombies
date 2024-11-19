using DragonBones;
using System.Collections;
using UnityEngine;
using Transform = UnityEngine.Transform;

public class SoldierController : MonoBehaviour
{
    [Header("�������")]
    public Transform FirePoint;          // ʿ���ķ���㣬��Ҫ��Inspector������

    private GameObject player;
    private Vector3 initialOffset;        // �洢����ҵĳ�ʼƫ����
    public float lifetime;
    private bool isShooting = false;      // ���ʿ���Ƿ��������

    private Coroutine shootCoroutine;     // ���Э������
    private UnityArmatureComponent armatureComponent;   // DragonBones Armature ���
    private void Start()
    {
        FirePoint = transform.Find("FirePoint").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        lifetime = 0;
        if (player != null)
        {
            // �����ʼ����ҵ�ƫ����
            initialOffset = transform.position - player.transform.position;
        }
        armatureComponent.animation.Play("walk+hit",-1);
        // �Զ���ʼ���
        StartShooting();
    }
    private void Update()
    {
        if (player != null)
        {
            FollowPlayer();
        }
        if (GameManage.Instance.gameState != GameState.Running)
        {
            Destroy(gameObject);
            return; // ����ʱ��ִ���κ��߼�
        }
    }

    /// <summary>
    /// ����ʿ����Ŀ�����
    /// </summary>
    /// <param name="player">Ŀ����Ҷ���</param>
    public void SetPlayer(GameObject player)
    {
        this.player = player;
        // �����ʼƫ����
        initialOffset = transform.position - player.transform.position;
    }

    /// <summary>
    /// ����ʿ���Ĵ��ʱ��
    /// </summary>
    /// <param name="time">���ʱ�䣨�룩</param>
    private Coroutine destroyCoroutine;   // ����Э������
    public void SetLifetime(float time)
    {
        lifetime += time; // ���Ӵ��ʱ��
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine); // ֹ֮ͣǰ������Э��
        }
        destroyCoroutine = StartCoroutine(DestroyAfterLifetime(lifetime)); // ������������Э��
    }
    /// <summary>
    /// ��ָ��ʱ�������ʿ��
    /// </summary>
    /// <param name="time">�ȴ�ʱ�䣨�룩</param>
    private IEnumerator DestroyAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        StopShooting();
        Destroy(gameObject); // ���ٶ���
    }

    /// <summary>
    /// ������ҵ�λ��
    /// </summary>
    private void FollowPlayer()
    {
        // ������ҵ�λ�úͳ�ʼƫ�����ƶ�ʿ��
        transform.position = player.transform.position + initialOffset;
    }

    /// <summary>
    /// ��ʼ���
    /// </summary>
    public void StartShooting()
    {
        if (!isShooting)
        {
            isShooting = true;
            shootCoroutine = StartCoroutine(ShootCoroutine());
        }
    }

    /// <summary>
    /// ֹͣ���
    /// </summary>
    public void StopShooting()
    {
        if (isShooting)
        {
            isShooting = false;
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }
        }
    }

    /// <summary>
    /// ���Э��
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootCoroutine()
    {
        while (isShooting)
        {
            if (PreController.Instance.activeEnemyCount > 0)
            {
                ShootBullet();
            }
            yield return new WaitForSeconds(PreController.Instance.GenerationIntervalBullet);
        }
    }

    /// <summary>
    /// �����ӵ�
    /// </summary>
    private void ShootBullet()
    {
        Gun currentgun = PlayInforManager.Instance.playInfor.currentGun;
        //long bulletCost = ConfigManager.Instance.Tables.TablePlayerConfig.Get(PlayInforManager.Instance.playInfor.level).Total;
        string bulletKey = currentgun.bulletType;
        // ���ӵ����л�ȡ�ӵ�
        if (PreController.Instance.bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
        {
            GameObject bullet = selectedBulletPool.Get();
            if (bullet != null)
            {
                bullet.SetActive(true);
                PreController.Instance.FixSortLayer(bullet);
                bullet.transform.position = FirePoint.position;
                EventDispatcher.instance.DispatchEvent(EventNameDef.ShowBuyBulletText);
            }
            //if (PlayInforManager.Instance.playInfor.SpendCoins(bulletCost))
            //{
            //    GameObject bullet = selectedBulletPool.Get();
            //    if (bullet != null)
            //    {
            //        bullet.SetActive(true);
            //        PreController.Instance.FixSortLayer(bullet);
            //        bullet.transform.position = FirePoint.position;
            //        EventDispatcher.instance.DispatchEvent(EventNameDef.ShowBuyBulletText);
            //    }
            //}
        }
        else
        {
            Debug.LogWarning($"Bullet pool not found for: {bulletKey}");
        }
    }
}
