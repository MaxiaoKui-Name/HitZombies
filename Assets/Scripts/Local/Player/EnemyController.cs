using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;  // 敌人移动速度
    public float attackRange = 0.5f;  // 敌人攻击距离
    public float StraightRange = 8f;  // 敌人攻击距离
    public int damage = 10;  // 敌人攻击时对玩家造成的伤害
    public float attackCooldown = 1f;  // 攻击冷却时间
    public float health = 100;  // 敌人的初始血量
    private Transform HitTarget;  // 玩家对象的引用
    public EnemyType enemyType;

    void OnEnable()
    {
        // 找到玩家对象（假设玩家的Tag是"HitTarget"）
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
        //数值初始化
        Init();
    }

    private void Init()
    {
        GetTypeValue(enemyType);
    }
    public void GetTypeValue(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.CuipiMonster:
                moveSpeed = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                break;
            case EnemyType.ShortMonster:
                moveSpeed = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                break;
            case EnemyType.DisMonster:
                moveSpeed = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                break;
            case EnemyType.ElitesMonster:
                moveSpeed = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                break;
            case EnemyType.Boss:
                moveSpeed = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiSpd;
                health = ConfigManager.Instance.Tables.TablePhysiqueReslevelConfig.Get(1).CuipiHp;
                break;
        }
    }
    void Update()
    {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (transform.position.y > HitTarget.parent.position.y + 3)
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        else
            // 使敌人朝玩家移动
            transform.position += (HitTarget.position - transform.position).normalized * moveSpeed * Time.deltaTime;
    }

    // 方法来处理敌人受到伤害
    public void TakeDamage(float damageAmount,GameObject enemyObj)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die(enemyObj);
        }
    }

    // 敌人死亡时调用的方法
    void Die(GameObject enemyObj)
    {
        var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
        enemyPool.Release(enemyObj);
        PreController.Instance.KillEnemyNun++;
    }
}
