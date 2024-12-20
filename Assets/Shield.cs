using System.Collections;
using UnityEngine;
using DragonBones;

public class Shield : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 6) // �ж��Ƿ�����˲㼶��ײ
        {
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.enemyType == EnemyType.Boss && GameFlowManager.Instance.currentLevelIndex == 0)
            {
                // �����������ΪBoss��������Э�̴���
                StartCoroutine(HandleBossCollision(enemyController));
            }
            else
            {
                // �������Boss��ֱ�Ӵ�����ͨ���˵���ײ
                HandleNormalEnemyCollision();
            }
        }
    }

    /// <summary>
    /// ������ͨ���˵���ײ
    /// </summary>
    private void HandleNormalEnemyCollision()
    {
        PlayerController playerController = transform.parent.GetComponent<PlayerController>();
        if (playerController != null && !playerController.isDead)
        {
            playerController.TakeDamage();
        }
    }

    /// <summary>
    /// ����Boss���˵���ײ���������Ŷ�����ִ���˺�
    /// </summary>
    /// <param name="enemyController">��ײ����Boss���˿�����</param>
    /// <returns></returns>
    private IEnumerator HandleBossCollision(EnemyController enemyController)
    {
        UnityArmatureComponent armature = enemyController.armatureComponent;
        if (armature != null)
        {
            // ����"hit"����������һ��
            armature.animation.Play("hit", 1);

            bool animationCompleted = false;

            // ���嶯�����ʱ���¼�������
            void OnAnimationComplete(string type, EventObject eventObject)
            {
                if (eventObject.animationState.name == "hit")
                {
                    animationCompleted = true;
                    armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
                }
            }

            // ����¼���������������������¼�
            armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
            // �ȴ������������
            while (!animationCompleted)
            {
                yield return null;
            }
        }
        // ����������ɺ�ִ�ж���ҵ��˺��߼�
        PlayerController playerController = transform.parent.GetComponent<PlayerController>();
        if (playerController != null && !playerController.isDead)
        {
            playerController.TakeDamage();
        }
    }
}
