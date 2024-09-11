using UnityEngine;
using UnityEngine.UI;
using DragonBones;  // Make sure you include the DragonBones namespace if you are using DragonBones
using Transform = UnityEngine.Transform;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;   // 玩家左右移动速度
    public Transform firePoint;    // 子弹发射点
    public float currentValue;     // 当前血量
    public float MaxValue;         // 最大血量
    public Slider healthSlider;    // 血量显示的Slider
    public Transform healthBarCanvas; // 血条所在的Canvas (World Space Canvas)
    public float leftBoundary = -1.5f;  // 左边界限制
    public float rightBoundary = 1.5f;  // 右边界限制
    private float horizontalInput;

    private Camera mainCamera; // 摄像机，用于将世界坐标转为屏幕坐标

    private UnityArmatureComponent armatureComponent; // DragonBones Armature component

    private void Start()
    {
        // 初始化摄像机
        mainCamera = Camera.main;

        // 初始化玩家血量和血条
        Init();

        // 获取 DragonBones Armature 组件
        armatureComponent = GameObject.Find("Player/player_003").GetComponent<UnityArmatureComponent>();

        // 播放并循环指定的动画
        if (armatureComponent != null)
        {
            PlayDragonAnimation();
        }
    }

    private void Init()
    {
        currentValue = 100; // 假设初始血量为100
        MaxValue = 100;
        healthSlider.maxValue = MaxValue;
        healthSlider.value = currentValue;
    }

    void Update()
    {
        if (GameManage.Instance.gameState != GameState.Running)
            return;

        // 玩家左右移动
        horizontalInput = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
        transform.position = newPosition;

        // 更新血条的位置，使其跟随玩家移动
        UpdateHealthBarPosition();
    }

    // 播放并循环"DragonAnimation"
    void PlayDragonAnimation()
    {
        if (armatureComponent != null)
        {
            // 播放名字为"DragonAnimation"的动画，并设置循环播放
            armatureComponent.animation.Play("shoot+walk", 0);  // 第2个参数0表示无限循环
        }
    }

    // 更新血条的位置，确保它跟随玩家
    void UpdateHealthBarPosition()
    {
        if (healthBarCanvas != null)
        {
            // 将血条设置为玩家头顶的位置 (世界坐标)
            healthBarCanvas.position = transform.position + new Vector3(0, 0.8f, 0);  // 1f 为Y轴的偏移量
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // 调整血条的缩放，使其适应场景
        }
    }

    // 处理玩家受到伤害
    public void TakeDamage(float damageAmount)
    {
        currentValue = Mathf.Max(currentValue - damageAmount, 0);
        healthSlider.value = currentValue;

        if (currentValue <= 0)
        {
            PlayerDie();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // 检查玩家是否碰到了敌人
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 假设敌人有一个EnemyController脚本，并包含伤害值
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // 玩家受到敌人的伤害
                TakeDamage(enemy.damage);
            }
        }
    }

    // 玩家死亡时的处理
    private void PlayerDie()
    {
        // 这里可以加入玩家死亡的逻辑，如播放死亡动画、游戏结束等
        Debug.Log("Player has died");
        UIManager.Instance.ChangeState(GameState.GameOver);  // 假设有GameOver的处理逻辑
    }
}
