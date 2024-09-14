using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 10f;  // �ӵ����ƶ��ٶ�
    public float firepower; // �ӵ��˺�
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
        Debug.Log(firepower + "�ӵ��˺�ֵ=====================");

    }

    void Update()
    {
        // �ӵ������ƶ�
        transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);

        // Ensure that the bullet is being checked for deactivation
        if (gameObject.activeSelf && gameObject != null)
        {
            PreController.Instance.DignoExtre(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // ����ӵ��Ƿ������˵���
        if (other.gameObject.layer == 6)  // ������˴���Layer 6
        {
            // �����������
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
                chest.TakeDamage(firepower,gameObject);  // �۳�����Ѫ��
            }
        }
        // �����ӵ��Ļ���
        if (gameObject.activeSelf)
        {
            var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
            bulletPool.Release(gameObject);
            ParticleManager.Instance.ShowEffect(EffectType.BulletEffect, transform.position, Quaternion.identity);
        }
    }
}
