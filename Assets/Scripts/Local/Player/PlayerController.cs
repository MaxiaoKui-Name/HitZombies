using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;   // 玩家左右移动速度
    //public GameObject bulletPrefab; // 子弹预制件
    public Transform firePoint;     // 子弹发射点
    //public GameObject EnemyPrefab; // 敌人预制体
    //public Transform EnemyParents;
    //public Transform BulletParents;
    public float leftBoundary = -1.5f;  // 左边界限制
    public float rightBoundary = 1.5f;  // 右边界限制
    private float horizontalInput;
    private float fireTimer = 0f;    // 用于计时的定时器
    public float fireRate = 0.3f;    // 发射速率（每0.5秒发射一次）
   
    void Update()
    {
        // 获取玩家的输入
        horizontalInput = Input.GetAxis("Horizontal");
        // 计算新的位置
        Vector3 newPosition = transform.position + new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0);
        // 限制玩家位置在边界内
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
        // 更新玩家位置
        transform.position = newPosition;

        // 增加计时器
        fireTimer += Time.deltaTime;

        // 当计时器超过发射间隔时，发射子弹
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;  // 重置计时器
        }
    }

    void Shoot()
    {
        GameObject Bullet = PreController.Instance.BulletPool.Get();
        Bullet.SetActive(true);
        Bullet.transform.position = firePoint.position;
    }
}
