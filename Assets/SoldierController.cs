using System.Collections;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [Header("�������")]
    public Transform FirePoint;          // ʿ���ķ���㣬��Ҫ��Inspector������

    private GameObject player;
    private Vector3 initialOffset;        // �洢����ҵĳ�ʼƫ����
    private float lifetime;
    private bool isShooting = false;      // ���ʿ���Ƿ��������

    private Coroutine shootCoroutine;     // ���Э������

    private void Start()
    {
        FirePoint = transform.Find("FirePoint").transform;
        lifetime = 0;
        if (player != null)
        {
            // �����ʼ����ҵ�ƫ����
            initialOffset = transform.position - player.transform.position;
        }
        // �Զ���ʼ���
        StartShooting();
    }

    private void Update()
    {
        if (player != null)
        {
            FollowPlayer();
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
    public void SetLifetime(float time)
    {
        if (lifetime > 0)
        {
            lifetime += time; // ���Ӵ��ʱ��
        }
        else
        {
            lifetime = time; // ��һ������
            StartCoroutine(DestroyAfterLifetime(lifetime)); // ��ʼЭ��
        }
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
        long bulletCost = currentgun.bulletValue;
        string bulletKey = currentgun.bulletType;
        // ���ӵ����л�ȡ�ӵ�
        if (PreController.Instance.bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
        {
            if (PlayInforManager.Instance.playInfor.SpendCoins(bulletCost))
            {
                GameObject bullet = selectedBulletPool.Get();
                if (bullet != null)
                {
                    bullet.SetActive(true);
                    PreController.Instance.FixSortLayer(bullet);
                    bullet.transform.position = FirePoint.position;
                    EventDispatcher.instance.DispatchEvent(EventNameDef.ShowBuyBulletText);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Bullet pool not found for: {bulletKey}");
        }
    }
}
