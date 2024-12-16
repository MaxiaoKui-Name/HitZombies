using Hitzb;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class Upperbound : MonoBehaviour
{
    // 当其他碰撞器进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的对象是否为敌人
        if (other.gameObject.layer == 6 || other.gameObject.layer == 13)
        {
            switch (other.gameObject.layer)
            {
                case 6:
                    EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                    if (!enemyController.isVise)
                    {
                        enemyController.isVise = true;
                        PreController.Instance.IncrementActiveEnemy();
                    }
                    break;
                case 13:
                    ChestController chestController = other.gameObject.GetComponent<ChestController>();
                    if (!chestController.isVise)
                    {
                        chestController.isVise = true;
                        PreController.Instance.IncrementActiveEnemy();
                    }
                    chestController.isVise = true;
                break;
            }
            // 调用 PreController 的 IncrementActiveVisibleEnemy 方法
            
        }
    }
}
