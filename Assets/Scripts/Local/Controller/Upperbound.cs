using Hitzb;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Upperbound : MonoBehaviour
{
    // ��������ײ�����봥����ʱ����
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ������Ķ����Ƿ�Ϊ����
        if (other.gameObject.layer == 6 || other.gameObject.layer == 13)
        {
            switch (other.gameObject.layer)
            {
                case 6:
                    EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                    enemyController.isVise = true;
                    break;
                case 13:
                    ChestController chestController = other.gameObject.GetComponent<ChestController>();
                    chestController.isVise = true;
                break;
            }
            // ���� PreController �� IncrementActiveVisibleEnemy ����
            PreController.Instance.IncrementActiveEnemy();
        }
    }
}
