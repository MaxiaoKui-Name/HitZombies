using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;  // �����ƶ��ٶ�
    public float attackRange = 3f;  // ���˹�������
    public int damage = 10;  // ���˹���ʱ�������ɵ��˺�
    public float attackCooldown = 1f;  // ������ȴʱ��

    private Transform player;  // ��Ҷ��������
    private float lastAttackTime = 0f;  // �ϴι�����ʱ��
    private bool isAttacking = false;  // �жϵ����Ƿ��ڹ�����
    public EnemyType enemyType;
    void OnEnable()
    {
        // �ҵ���Ҷ��󣨼�����ҵ�Tag��"Player"��
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
