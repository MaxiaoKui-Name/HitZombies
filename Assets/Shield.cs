using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 6)
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                PlayerController  playerController = transform.parent.GetComponent<PlayerController>();
                playerController.TakeDamage(enemy.damage);
            }
        }
    }

}
