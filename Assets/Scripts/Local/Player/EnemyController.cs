using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;  // �����ƶ��ٶ�
    public float attackRange = 0.5f;  // ���˹�������
    public float StraightRange = 8f;  // ���˹�������
    public int damage = 10;  // ���˹���ʱ�������ɵ��˺�
    public float attackCooldown = 1f;  // ������ȴʱ��
    public float health = 100;  // ���˵ĳ�ʼѪ��
    private Transform HitTarget;  // ��Ҷ��������
    public EnemyType enemyType;

    void OnEnable()
    {
        // �ҵ���Ҷ��󣨼�����ҵ�Tag��"HitTarget"��
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
        //��ֵ��ʼ��
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
            // ʹ���˳�����ƶ�
            transform.position += (HitTarget.position - transform.position).normalized * moveSpeed * Time.deltaTime;
    }

    // ��������������ܵ��˺�
    public void TakeDamage(float damageAmount,GameObject enemyObj)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die(enemyObj);
        }
    }

    // ��������ʱ���õķ���
    void Die(GameObject enemyObj)
    {
        var enemyPool = PreController.Instance.GetEnemyPoolMethod(enemyObj);
        enemyPool.Release(enemyObj);
        PreController.Instance.KillEnemyNun++;
    }
}
