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

    private Transform HitTarget;  // ��Ҷ��������
    private float lastAttackTime = 0f;  // �ϴι�����ʱ��
    private bool isAttacking = false;  // �жϵ����Ƿ��ڹ�����
    public EnemyType enemyType;

    void OnEnable()
    {
        // �ҵ���Ҷ��󣨼�����ҵ�Tag��"Player"��
        HitTarget = GameObject.FindGameObjectWithTag("HitTarget").transform;
    }

    void Update()
    {
         MoveTowardsPlayer();
        //CheckAttackPlayer();

        //// ������˳�����Ļ�߽磬�򷵻ض����
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
            // ʹ���˳�����ƶ�
            transform.position += (HitTarget.position - transform.position).normalized * moveSpeed * Time.deltaTime;
    }


}