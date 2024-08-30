using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 10f;  // �ӵ����ƶ��ٶ�
    public float firepower;//�ӵ��˺�
    public BulletType bulletType;
    public int bulletValue;
    void OnEnable()
    {
        //��ֵ��ʼ��
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
        // �ӵ������ƶ�
        transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);
        if (gameObject.activeSelf)
            PreController.Instance.DignoExtre(gameObject);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ����ӵ��Ƿ������˵���
        if (collision.gameObject.layer == 6)  // ������˴���Layer 8
        {
            // �����ӵ�
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
