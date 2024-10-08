using System.Collections;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [Header("发射相关")]
    public Transform FirePoint;          // 士兵的发射点，需要在Inspector中设置

    private GameObject player;
    private Vector3 initialOffset;        // 存储与玩家的初始偏移量
    private float lifetime;
    private bool isShooting = false;      // 标记士兵是否正在射击

    private Coroutine shootCoroutine;     // 射击协程引用

    private void Start()
    {
        FirePoint = transform.Find("FirePoint").transform;
        lifetime = 0;
        if (player != null)
        {
            // 计算初始与玩家的偏移量
            initialOffset = transform.position - player.transform.position;
        }
        // 自动开始射击
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
    /// 设置士兵的目标玩家
    /// </summary>
    /// <param name="player">目标玩家对象</param>
    public void SetPlayer(GameObject player)
    {
        this.player = player;
        // 计算初始偏移量
        initialOffset = transform.position - player.transform.position;
    }

    /// <summary>
    /// 设置士兵的存活时间
    /// </summary>
    /// <param name="time">存活时间（秒）</param>
    public void SetLifetime(float time)
    {
        if (lifetime > 0)
        {
            lifetime += time; // 增加存活时间
        }
        else
        {
            lifetime = time; // 第一次设置
            StartCoroutine(DestroyAfterLifetime(lifetime)); // 开始协程
        }
    }

    /// <summary>
    /// 在指定时间后销毁士兵
    /// </summary>
    /// <param name="time">等待时间（秒）</param>
    private IEnumerator DestroyAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        StopShooting();
        Destroy(gameObject); // 销毁对象
    }

    /// <summary>
    /// 跟随玩家的位置
    /// </summary>
    private void FollowPlayer()
    {
        // 根据玩家的位置和初始偏移量移动士兵
        transform.position = player.transform.position + initialOffset;
    }

    /// <summary>
    /// 开始射击
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
    /// 停止射击
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
    /// 射击协程
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
    /// 发射子弹
    /// </summary>
    private void ShootBullet()
    {
        Gun currentgun = PlayInforManager.Instance.playInfor.currentGun;
        long bulletCost = currentgun.bulletValue;
        string bulletKey = currentgun.bulletType;
        // 从子弹池中获取子弹
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
