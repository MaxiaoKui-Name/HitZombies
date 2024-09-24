using UnityEngine;
using UnityEngine.UI;
using DragonBones;
using Transform = UnityEngine.Transform;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;   // 玩家左右移动速度
    public Transform firePoint;    // 子弹发射点
    public float currentValue;     // 当前血量
    public float MaxValue;         // 最大血量
    public Slider healthSlider;    // 血量显示的Slider
    public TextMeshProUGUI DeCoinMonText; 
    public Transform healthBarCanvas; // 血条所在的Canvas (World Space Canvas)
    public float leftBoundary = -1.5f;  // 左边界限制
    public float rightBoundary = 1.5f;  // 右边界限制

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
        EventDispatcher.instance.Regist(EventNameDef.ShowBuyBulletText, (v) => ShowDeclineMoney());
    }

    private void Init()
    {
        currentValue = PlayInforManager.Instance.playInfor.health;
        MaxValue = PlayInforManager.Instance.playInfor.health;
        healthSlider.maxValue = MaxValue;
        healthSlider.value = currentValue;
        moveSpeed = 2f;
    }

    void Update()
    {
        if (GameManage.Instance.gameState != GameState.Running)
            return;
        // 使用鼠标控制玩家左右移动
        ControlMovementWithMouse();
        // 更新血条的位置，使其跟随玩家移动
        UpdateHealthBarPosition();
    }

    // 使用鼠标X轴位置控制玩家左右移动
    void ControlMovementWithMouse()
    {
        // 获取鼠标在屏幕上的X轴位置
        Vector3 mousePosition = Input.mousePosition;

        // 将屏幕坐标转换为世界坐标
        // 设置Z值与玩家的Z值相同，以确保转换正确
        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // 仅使用X轴的位置更新玩家位置
        Vector3 newPosition = new Vector3(worldPosition.x, transform.position.y, transform.position.z);

        // 限制玩家移动范围在左右边界之间
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);

        // 设置玩家的位置
        transform.position = newPosition;
    }

    // 播放并循环"DragonAnimation"
    void PlayDragonAnimation()
    {
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play("shoot+walk", 0);  // 无限循环动画
        }
    }

    // 更新血条的位置，确保它跟随玩家
    void UpdateHealthBarPosition()
    {
        if (healthBarCanvas != null)
        {
            // 将血条设置为玩家头顶的位置 (世界坐标)
            healthBarCanvas.position = transform.position + new Vector3(0, 1.6f, 0);  // Y轴的偏移量
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // 调整血条的缩放
        }
    }
    async void ShowDeclineMoney()
    {
        if (DeCoinMonText != null)
        {
            DeCoinMonText.text = $"-{ConfigManager.Instance.Tables.TablePlayer.Get(0).Total}";
            ShowDeCoinMonText();
        }
    }
    private async UniTask ShowDeCoinMonText()
    {
        DeCoinMonText.gameObject.SetActive(true); // 显示文本
        await UniTask.Delay(500);
        DeCoinMonText.gameObject.SetActive(false); // 隐藏文本
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
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                TakeDamage(enemy.damage);
            }
        }
    }

    // 玩家死亡时的处理
    private void PlayerDie()
    {
        Debug.Log("Player has died");
        UIManager.Instance.ChangeState(GameState.GameOver);
    }
}
