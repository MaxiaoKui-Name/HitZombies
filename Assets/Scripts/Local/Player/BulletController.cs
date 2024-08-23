using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 10f;  // �ӵ����ƶ��ٶ�


    void Update()
    {
        // �ӵ������ƶ�
        transform.Translate(Vector2.down * bulletSpeed * Time.deltaTime);
        if (gameObject.activeSelf)
            PreController.Instance.DignoExtre(gameObject);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ����ӵ��Ƿ������˵���
        if (collision.gameObject.layer == 6)  // ������˴���Layer 8
        {
            // �����ӵ�
            collision.gameObject.SetActive(false);
            var enemyPool = PreController.Instance.GetEnemyPoolMethod(collision.gameObject);
            enemyPool.Release(collision.gameObject);
            if (gameObject.activeSelf)
            {
                var bulletPool = PreController.Instance.GetBulletPoolMethod(gameObject);
                bulletPool.Release(gameObject);

            }
        }
    }

   
}
