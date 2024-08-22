using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;  // 敌人移动速度
    public float attackRange = 3f;  // 敌人攻击距离
    public int damage = 10;  // 敌人攻击时对玩家造成的伤害
    public float attackCooldown = 1f;  // 攻击冷却时间

    private Transform player;  // 玩家对象的引用
    private float lastAttackTime = 0f;  // 上次攻击的时间
    private bool isAttacking = false;  // 判断敌人是否在攻击中
    public EnemyType enemyType;
    void OnEnable()
    {
        // 找到玩家对象（假设玩家的Tag是"Player"）
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        MoveTowardsPlayer();
        if(gameObject.activeSelf)
           PreController.Instance.DignoExtre(PreController.Instance.EnemyPool,gameObject);
    }

    void MoveTowardsPlayer()
    {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
    }

}
