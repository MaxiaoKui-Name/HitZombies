using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Upperbound : MonoBehaviour
{
    // 当其他碰撞器进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的对象是否为敌人
        if (other.gameObject.layer == 6 || other.gameObject.layer == 13)
        {
            // 调用 PreController 的 IncrementActiveVisibleEnemy 方法
            PreController.Instance.IncrementActiveEnemy();
        }
    }
}
