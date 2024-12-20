using System.Collections;
using UnityEngine;
using DragonBones;

public class Shield : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 6) // 判断是否与敌人层级碰撞
        {
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.enemyType == EnemyType.Boss && GameFlowManager.Instance.currentLevelIndex == 0)
            {
                // 如果敌人类型为Boss，则启动协程处理
                StartCoroutine(HandleBossCollision(enemyController));
            }
            else
            {
                // 如果不是Boss，直接处理普通敌人的碰撞
                HandleNormalEnemyCollision();
            }
        }
    }

    /// <summary>
    /// 处理普通敌人的碰撞
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
    /// 处理Boss敌人的碰撞，包括播放动画和执行伤害
    /// </summary>
    /// <param name="enemyController">碰撞到的Boss敌人控制器</param>
    /// <returns></returns>
    private IEnumerator HandleBossCollision(EnemyController enemyController)
    {
        UnityArmatureComponent armature = enemyController.armatureComponent;
        if (armature != null)
        {
            // 播放"hit"动画，播放一次
            armature.animation.Play("hit", 1);

            bool animationCompleted = false;

            // 定义动画完成时的事件处理器
            void OnAnimationComplete(string type, EventObject eventObject)
            {
                if (eventObject.animationState.name == "hit")
                {
                    animationCompleted = true;
                    armature.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
                }
            }

            // 添加事件监听器，监听动画完成事件
            armature.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
            // 等待动画播放完成
            while (!animationCompleted)
            {
                yield return null;
            }
        }
        // 动画播放完成后，执行对玩家的伤害逻辑
        PlayerController playerController = transform.parent.GetComponent<PlayerController>();
        if (playerController != null && !playerController.isDead)
        {
            playerController.TakeDamage();
        }
    }
}
