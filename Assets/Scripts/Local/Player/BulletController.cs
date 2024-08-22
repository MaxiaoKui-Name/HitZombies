using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 10f;  // 子弹的移动速度


    void Update()
    {
        // 子弹向下移动
        transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);
        if (gameObject.activeSelf)
            PreController.Instance.DignoExtre(PreController.Instance.BulletPool, gameObject);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查子弹是否碰到了敌人
        if (collision.gameObject.layer == 8)  // 假设敌人处于Layer 8
        {
            // 销毁子弹
            collision.gameObject.SetActive(false);
            PreController.Instance.EnemyPool.Release(collision.gameObject);
            PreController.Instance.HideAndReturnToPool(PreController.Instance.BulletPool,gameObject);
        }
    }

   
}
