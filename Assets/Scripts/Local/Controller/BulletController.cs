using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Hitzb
{
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
            GetTypeValue(bulletType);
        }

        public void GetTypeValue(BulletType bulletType)
        {
            switch (bulletType)
            {
                case BulletType.bullet_01:
                    bulletSpeed = ConfigManager.Instance.Tables.TableTransmitConfig.Get(20000).StrategyParams[0];
                    bulletcost =  ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
                    firepower = 800;//ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).AtkRate * bulletcost;//
                    break;
                case BulletType.bullet_02:
                    bulletSpeed = ConfigManager.Instance.Tables.TableTransmitConfig.Get(20100).StrategyParams[0];
                    bulletcost = ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
                    firepower = 800;//ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).AtkRate * bulletcost;
                    break;
                case BulletType.bullet_04:
                    bulletSpeed = ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).StrategyParams[0];
                    bulletcost =  ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
                    firepower = 800;//ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).AtkRate * bulletcost;
                    break;
            }
            //firepower = (float)(firepower * (1 + PlayInforManager.Instance.playInfor.attackSpFac));
        }

        void Update()
        {
            // �ӵ������ƶ�
            transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);

            // ȷ���ӵ�������Ƿ���Ҫ����
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
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();

                // ���� BaseMethod �е� IsEnemyOnScreen ����
                if (enemyController != null && !enemyController.isDead && enemyController.isVise)
                {
                    enemyController.TakeDamage(firepower, other.gameObject);
                    // �����ӵ��Ļ���
                    if (gameObject.activeSelf)
                    {
                        var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
                        bulletPool.Release(gameObject);
                    }
                }
            }

            if (other.gameObject.layer == 13) // �����
            {
                ChestController chest = other.gameObject.GetComponent<ChestController>();
                if (chest != null && chest.isVise)
                {
                    chest.TakeDamage(firepower, gameObject);  // �۳�����Ѫ��
                                                              // �����ӵ��Ļ���
                    if (gameObject.activeSelf)
                    {
                        var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
                        bulletPool.Release(gameObject);
                    }
                }
            }

          
        }
       
    }
}
