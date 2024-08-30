using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 10f;  // 子弹的移动速度
    public float firepower;//子弹伤害
    public BulletType bulletType;
    public int bulletValue;
    void OnEnable()
    {
        //数值初始化
        Init();
    }

    private void Init()
    {
        GetTypeValue(bulletType);
    }
    public void GetTypeValue(BulletType enemyType)
    {
        switch (enemyType)
        {
            case BulletType.TEgaugeBullet:
                firepower = ConfigManager.Instance.Tables.TableAttributeResattributeConfig.Get(2000).GenusValue;
                bulletValue = ConfigManager.Instance.Tables.TableLevelResequipmentConfi.Get(20001).NeedGold;
                bulletSpeed = ConfigManager.Instance.Tables.TableFireResskillConfig.Get(20000).StrategyParams[1];
                break;

        }
    }
    void Update()
    {
        // 子弹向下移动
        transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);
        if (gameObject.activeSelf)
            PreController.Instance.DignoExtre(gameObject);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查子弹是否碰到了敌人
        if (collision.gameObject.layer == 6)  // 假设敌人处于Layer 8
        {
            // 销毁子弹
            if (collision.gameObject.activeSelf)
            {
                EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
                enemyController.TakeDamage(firepower, collision.gameObject);
              
            }
            if (gameObject.activeSelf)
            {
                var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
                bulletPool.Release(gameObject);
            }
        }
    }

   
}
