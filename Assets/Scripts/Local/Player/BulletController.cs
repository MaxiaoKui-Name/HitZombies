using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 10f;  // �ӵ����ƶ��ٶ�
    public float firepower; // �ӵ��˺�
    public BulletType bulletType;

    void OnEnable()
    {
        // ��ֵ��ʼ��
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
        // �ӵ������ƶ�
        transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);

        // Ensure that the bullet is being checked for deactivation
        if (gameObject.activeSelf)
        {
            PreController.Instance.DignoExtre(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("��ײ�ɹ�");
        // ����ӵ��Ƿ������˵���
        if (other.gameObject.layer == 6)  // ������˴���Layer 6
        {
            // �����ӵ�
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
