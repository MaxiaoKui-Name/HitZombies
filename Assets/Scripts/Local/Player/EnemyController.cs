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

    private Transform HitTarget;  // 玩家对象的引用
    private float lastAttackTime = 0f;  // 上次攻击的时间
    private bool isAttacking = false;  // 判断敌人是否在攻击中
    public EnemyType enemyType;

    void OnEnable()
    {
        // 找到玩家对象（假设玩家的Tag是"Player"）
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
    }

    void Update()
    {
         MoveTowardsPlayer();
        //CheckAttackPlayer();

        //// 如果敌人超出屏幕边界，则返回对象池
        //if (gameObject.activeSelf)
        //{
        //    PreController.Instance.DignoExtre(gameObject);
        //}
    }

    void MoveTowardsPlayer()
    {
        if (transform.position.y  > HitTarget.parent.position.y + 3)
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        else
            // 使敌人朝玩家移动
            transform.position += (HitTarget.position - transform.position).normalized * moveSpeed * Time.deltaTime;
    }


}