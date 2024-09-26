using System.Collections;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    private GameObject player;
    private Vector3 initialOffset; // 存储初始偏移量
    private float lifetime;

    private void Start()
    {
        lifetime = 0;
        if (player != null)
        {
            // 计算初始与玩家的偏移量
            initialOffset = transform.position - player.transform.position;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            FollowPlayer();
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        // 计算初始偏移量
        initialOffset = transform.position - player.transform.position;
    }
    public void SetLifetime(float time)
    {
        if (lifetime > 0)
        {
            lifetime += time; // 增加存活时间
        }
        else
        {
            lifetime = time; // 第一次设置
            StartCoroutine(DestroyAfterLifetime(lifetime)); // 开始协程
        }
    }

    private IEnumerator DestroyAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject); // 销毁对象
    }

    private void FollowPlayer()
    {
        // 根据玩家的位置和初始偏移量移动士兵
        transform.position = player.transform.position + initialOffset;
    }
}
