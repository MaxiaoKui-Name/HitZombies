using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 10f;  // 子弹的移动速度
    public float firepower; // 子弹伤害
    public BulletType bulletType;
    public float bulletcost;
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
        bulletSpeed = 15f;// ConfigManager.Instance.Tables.TableTransmit.Get(20200).StrategyParams[0];
        GetTypeValue(bulletType);
    }

    public void GetTypeValue(BulletType bulletType)
    {
        switch (bulletType)
        {
            case BulletType.bullet_01:
                bulletcost = ConfigManager.Instance.Tables.TablePlayerLevelRes.Get(0).Total;
                firepower = ConfigManager.Instance.Tables.TableTransmit.Get(20200).AtkRate * bulletcost;
                break;
            case BulletType.bullet_04:
                bulletcost = ConfigManager.Instance.Tables.TablePlayerLevelRes.Get(0).Total;
                firepower = ConfigManager.Instance.Tables.TableTransmit.Get(20200).AtkRate * bulletcost;
                break;
              
        }
        firepower = (float)(firepower * (1 + BuffDoorController.Instance.attackFac));// ConfigManager.Instance.Tables.TableAttributeResattributeConfig.Get(2000).GenusValue;
        Debug.Log(firepower + "子弹伤害值=====================");

    }

    void Update()
    {
        // 子弹向下移动
        transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);

        // Ensure that the bullet is being checked for deactivation
        if (gameObject.activeSelf && gameObject != null)
        {
            PreController.Instance.DignoExtre(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // 检查子弹是否碰到了敌人
        if (other.gameObject.layer == 6)  // 假设敌人处于Layer 6
        {
            // 处理敌人受伤
            if (other.gameObject.activeSelf)
            {
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null && enemyController.health >0)
                {
                    enemyController.TakeDamage(firepower, other.gameObject);
                }
            }
        }
        if (other.gameObject.layer == 13)
        {
            ChestController chest = other.gameObject.GetComponent<ChestController>();
            if (chest != null)
            {
                chest.TakeDamage(firepower,gameObject);  // 扣除宝箱血量
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
