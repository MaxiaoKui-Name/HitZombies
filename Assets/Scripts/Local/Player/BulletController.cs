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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        Init();
    }

    private void Init()
    {
        bulletSpeed = 15;
        firepower = 0;
        bulletType = 0;
        GetTypeValue(bulletType);
    }

    public void GetTypeValue(BulletType bulletType)
    {
        switch (bulletType)
        {
            case BulletType.bullet_01:
                firepower = 800;// ConfigManager.Instance.Tables.TableAttributeResattributeConfig.Get(2000).GenusValue;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查子弹是否碰到了敌人
        if (other.gameObject.layer == 6)  // 假设敌人处于Layer 6
        {
            // 处理敌人受伤
            if (other.gameObject.activeSelf)
            {
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(firepower, other.gameObject);
                }
            }

            // 处理子弹的回收
            if (gameObject.activeSelf)
            {
                var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
                bulletPool.Release(gameObject);
                ParticleManager.Instance.ShowEffect(EffectType.BulletEffect, transform.position, Quaternion.identity);
            }
        }
    }
}
