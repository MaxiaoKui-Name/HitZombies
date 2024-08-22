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
            PreController.Instance.DignoExtre(PreController.Instance.BulletPool, gameObject);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ����ӵ��Ƿ������˵���
        if (collision.gameObject.layer == 8)  // ������˴���Layer 8
        {
            // �����ӵ�
            collision.gameObject.SetActive(false);
            PreController.Instance.EnemyPool.Release(collision.gameObject);
            PreController.Instance.HideAndReturnToPool(PreController.Instance.BulletPool,gameObject);
        }
    }

   
}
