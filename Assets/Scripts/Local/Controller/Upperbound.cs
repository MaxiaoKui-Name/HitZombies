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
            // ���� PreController �� IncrementActiveVisibleEnemy ����
            PreController.Instance.IncrementActiveEnemy();
        }
    }
}
