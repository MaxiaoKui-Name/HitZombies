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
        if (other.CompareTag("Enemy"))
        {
            // ���� PreController �� IncrementActiveVisibleEnemy ����
            PreController.Instance.IncrementActiveEnemy();
        }
    }
}
