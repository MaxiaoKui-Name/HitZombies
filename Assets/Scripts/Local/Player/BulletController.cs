using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 10f;  // 子弹的移动速度
    public float firepower; // 子弹伤害
    public BulletType bulletType;

    void OnEnable()
    {
        // 数值初始化
        Init();
    }

    private void Init()
    {
        bulletSpeed = 10;
        firepower = 0;
        bulletType = 0;
        GetTypeValue(bulletType);
    }

    public void GetTypeValue(BulletType bulletType)
    {
        switch (bulletType)
        {
            case BulletType.bullet_01:
                firepower = ConfigManager.Instance.Tables.TableAttributeResattributeConfig.Get(2000).GenusValue;
                break;
        }
    }

    void Update()
    {
        // 子弹向下移动
        transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);

        // Ensure that the bullet is being checked for deactivation
        if (gameObject.activeSelf)
        {
            PreController.Instance.DignoExtre(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("碰撞成功");
        // 检查子弹是否碰到了敌人
        if (other.gameObject.layer == 6)  // 假设敌人处于Layer 6
        {
            // 销毁子弹
            if (other.gameObject.activeSelf)
            {
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(firepower, other.gameObject);
                }
            }

            if (gameObject.activeSelf)
            {
                var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
                bulletPool.Release(gameObject);
            }
        }
    }
}
