using DragonBones;
using Hitzb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transform = UnityEngine.Transform;

public class SoldierController : MonoBehaviour
{
    [Header("发射相关")]
    public Transform FirePoint;          // 士兵的发射点，需要在Inspector中设置

    private GameObject player;
    private Vector3 initialOffset;        // 存储与玩家的初始偏移量
    public float lifetime;
    private bool isShooting = false;      // 标记士兵是否正在射击

    private Coroutine shootCoroutine;     // 射击协程引用
    private UnityArmatureComponent armatureComponent;   // DragonBones Armature 组件

    private List<BulletController> flyingBullets = new List<BulletController>(); // 跟踪士兵发射的子弹

    private float detectionRange = 10f; // 定义检测范围

    private void Start()
    {
        FirePoint = transform.Find("FirePoint").transform;
        armatureComponent = transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        lifetime = 0;

        if (player != null)
        {
            // 计算初始与玩家的偏移量
            initialOffset = transform.position - player.transform.position;
        }

        armatureComponent.animation.Play("walk+hit", -1);


        // 自动开始射击
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
            return; // 冻结时不执行任何逻辑
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
    private Coroutine destroyCoroutine;   // 销毁协程引用

    public void SetLifetime(float time)
    {
        lifetime += time; // 增加存活时间
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine); // 停止之前的销毁协程
        }
        destroyCoroutine = StartCoroutine(DestroyAfterLifetime(lifetime)); // 重新启动销毁协程
    }

    /// <summary>
    /// 在指定时间后销毁士兵
    /// </summary>
    /// <param name="time">等待时间（秒）</param>
    private IEnumerator DestroyAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        StopShooting();

        // 清理飞行中的子弹列表
        foreach (var bullet in flyingBullets)
        {
            if (bullet != null)
            {
                bullet.OnBulletDestroyed -= HandleBulletDestroyed;
                // 这里可以根据需要销毁子弹或其他处理
            }
        }
        flyingBullets.Clear();
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
        float HoridetectionRange = 0.5f;
        float VertialdetectionRange = 8.06f;
        // 只有在检测到前方有敌人并且飞行中的子弹不足以消灭敌人时才射击
        if (IsEnemyInFront(HoridetectionRange, VertialdetectionRange))
        {
            float totalBulletDamage = GetTotalFlyingBulletDamage(HoridetectionRange, VertialdetectionRange);
            float totalEnemyHealth = GetTotalEnemyHealthInRange(HoridetectionRange, VertialdetectionRange);

            if (totalBulletDamage < totalEnemyHealth)
            {
                Gun currentgun = PlayInforManager.Instance.playInfor.currentGun;
                string bulletKey = currentgun.bulletType;

                // 从子弹池中获取子弹
                if (PreController.Instance.bulletPools.TryGetValue(bulletKey, out var selectedBulletPool))
                {
                    GameObject bullet = selectedBulletPool.Get();
                    if (bullet != null)
                    {
                        bullet.SetActive(true);
                        PreController.Instance.FixSortLayer(bullet);
                        bullet.transform.position = FirePoint.position;

                        // 将子弹加入飞行列表
                        BulletController bulletController = bullet.GetComponent<BulletController>();
                        if (bulletController != null)
                        {
                            flyingBullets.Add(bulletController);
                            bulletController.OnBulletDestroyed += HandleBulletDestroyed; // 注册子弹销毁事件
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"Bullet pool not found for: {bulletKey}");
                }
            }
            else
            {
                // 飞行中的子弹足以消灭前方敌人，不再开火
            }
        }
        else
        {
            // 正前方没有敌人，不开火
        }
    }

    // 检测士兵正前方一定范围内是否有敌人
    private bool IsEnemyInFront(float HoridetectionRange, float VertialdetectionRange)
    {
        Vector3 soldierPosition = FirePoint.position;
        //Vector3 soldierPosition = transform.position;

        // 定义检测区域的左下角和右上角
        Vector2 pointA = new Vector2(soldierPosition.x - HoridetectionRange, soldierPosition.y);
        Vector2 pointB = new Vector2(soldierPosition.x + HoridetectionRange, soldierPosition.y + VertialdetectionRange);
        // 定义敌人所在的Layer
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        int ChestLayerMask = LayerMask.GetMask("Chest");
        // 获取检测区域内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayerMask | ChestLayerMask);

        foreach (var collider in colliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                return true; // 范围内有敌人
            }
        }
        return false; // 范围内没有敌人
    }

    // 计算飞行中子弹的总伤害
    private float GetTotalFlyingBulletDamage(float HoridetectionRange, float VertialdetectionRange)
    {
        float totalDamage = 0f;
        // 获取玩家位置
        Vector3 soliderPosition = FirePoint.position;
        //Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Debug.Log("玩家的位置信息" + soliderPosition);
        // 定义检测区域的左下角和右上角
        Vector2 pointA = new Vector2(soliderPosition.x - HoridetectionRange, soliderPosition.y);
        Vector2 pointB = new Vector2(soliderPosition.x + HoridetectionRange, soliderPosition.y + VertialdetectionRange);
        // 定义敌人所在的Layer
        int bulletLayerMask = LayerMask.GetMask("Bullet");
        // 获取检测区域内的所有碰撞体
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

    // 计算正前方敌人的总生命值
    private float GetTotalEnemyHealthInRange(float HoridetectionRange, float VertialdetectionRange)
    {
        float totalHealth = 0f;
        Vector3 soldierPosition = FirePoint.position;
        //Vector3 soldierPosition = transform.position;
        Vector2 pointA = new Vector2(soldierPosition.x - HoridetectionRange, soldierPosition.y);
        Vector2 pointB = new Vector2(soldierPosition.x + HoridetectionRange, soldierPosition.y + VertialdetectionRange);
        // 定义敌人所在的Layer
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        int ChestLayerMask = LayerMask.GetMask("Chest");
        // 获取检测区域内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayerMask | ChestLayerMask);

        foreach (var collider in colliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                totalHealth += enemy.health;
            }
        }

        return totalHealth;
       
    }

    private void HandleBulletDestroyed(BulletController bullet)
    {
        flyingBullets.Remove(bullet);
    }
    private void OnDrawGizmos()
    {
        // 确保在运行时才绘制 Gizmos，或者根据需要去掉此判断
        if (Application.isPlaying)
        {
            DrawDetectionArea();
        }
    }
    private void DrawDetectionArea()
    {
        // 获取玩家位置
        Vector3 playerPosition = GameObject.Find("Player/FirePoint").transform.position;

        // 定义检测区域的左下角和右上角
        Vector2 pointA = new Vector2(playerPosition.x - 0.1f, playerPosition.y);
        Vector2 pointB = new Vector2(playerPosition.x + 0.1f, playerPosition.y + 8.06f);

        // 设置 Gizmos 的颜色
        Gizmos.color = Color.red;

        // 绘制矩形的四个边
        Vector3 bottomLeft = new Vector3(pointA.x, pointA.y, 0);
        Vector3 bottomRight = new Vector3(pointB.x, pointA.y, 0);
        Vector3 topRight = new Vector3(pointB.x, pointB.y, 0);
        Vector3 topLeft = new Vector3(pointA.x, pointB.y, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight); // 下边
        Gizmos.DrawLine(bottomRight, topRight);   // 右边
        Gizmos.DrawLine(topRight, topLeft);       // 上边
        Gizmos.DrawLine(topLeft, bottomLeft);     // 左边
    }
}
