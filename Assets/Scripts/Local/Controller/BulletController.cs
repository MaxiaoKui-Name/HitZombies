using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace Hitzb
{
    public class BulletController : MonoBehaviour
    {
        public float bulletSpeed = 10f;  // 子弹的移动速度
        public float firepower; // 子弹伤害
        public BulletType bulletType;
        public float bulletcost;

        public event Action<BulletController> OnBulletDestroyed;

        // 添加一个目标，用于锁定敌人或宝箱
        public Transform target;
        public bool isSoliderBullet = false;
        void OnEnable()
        {
            target = null;
            transform.GetComponent<Collider2D>().enabled = true;
            Init();
        }
        void Start()
        {
            if (isSoliderBullet)
            {
                firepower = firepower * ConfigManager.Instance.Tables.TableGlobal.Get(9).FloatValue;
            }
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
                    firepower = ConfigManager.Instance.Tables.TableTransmitConfig.Get(20000).AtkRate * bulletcost * (1 + (PlayInforManager.Instance.playInfor.attackFac < 0 ? PlayInforManager.Instance.playInfor.attackFac:0));
                    break;
                case BulletType.bullet_02:
                    bulletSpeed = ConfigManager.Instance.Tables.TableTransmitConfig.Get(20100).StrategyParams[0];
                    bulletcost = ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
                    firepower = ConfigManager.Instance.Tables.TableTransmitConfig.Get(20100).AtkRate * bulletcost * (1 + (PlayInforManager.Instance.playInfor.attackFac < 0 ? PlayInforManager.Instance.playInfor.attackFac : 0));
                    break;
                case BulletType.bullet_04:
                    bulletSpeed = ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).StrategyParams[0];
                    bulletcost =  ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
                    firepower = ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).AtkRate * bulletcost * (1 + (PlayInforManager.Instance.playInfor.attackFac < 0 ? PlayInforManager.Instance.playInfor.attackFac : 0));
                    break;
            }
            Debug.Log("火力系数的值"+ PlayInforManager.Instance.playInfor.attackFac + "firepower的值" + firepower);
            //firepower = (float)(firepower * (1 + PlayInforManager.Instance.playInfor.attackSpFac));
        }
        // 新增方法：设置目标
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
        void Update()
        {
            if (target != null && target.gameObject.activeSelf)
            {
                // 朝目标移动
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * bulletSpeed * Time.deltaTime;
            }
            else
            {
                // 子弹向下移动
                transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);
            }

            // 检查子弹是否需要回收
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
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                // 调用 BaseMethod 中的 IsEnemyOnScreen 方法
                if (enemyController != null && !enemyController.isDead && enemyController.isVise)
                {
                    transform.GetComponent<Collider2D>().enabled = false;
                    //if (isSoliderBullet)
                    //{
                    //    Debug.Log("士兵子弹"   +firepower);
                    //}
                    enemyController.TakeDamage((long)firepower, other.gameObject);
                    // 处理子弹的回收
                    if (gameObject.activeSelf)
                    {
                        DestroyBullet();
                        var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
                        isSoliderBullet = false;
                        bulletPool.Release(gameObject);
                    }
                }
            }

            if (other.gameObject.layer == 13) // 宝箱层
            {
                ChestController chest = other.gameObject.GetComponent<ChestController>();
                if (chest != null && chest.isVise)
                {
                    transform.GetComponent<Collider2D>().enabled = false;
                    chest.TakeDamage(firepower, gameObject);  // 扣除宝箱血量
                    if (gameObject.activeSelf)
                    {
                        DestroyBullet();
                        var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
                        bulletPool.Release(gameObject);
                    }
                }
            }

          
        }

        private void DestroyBullet()
        {
            // 触发事件
            OnBulletDestroyed?.Invoke(this);
        }
    }
}
