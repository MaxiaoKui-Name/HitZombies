using UnityEngine;
using UnityEngine.UI;
using DragonBones;
using Transform = UnityEngine.Transform;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public class PlayerController : MonoBehaviour
{
    // 玩家移动相关
    public float moveSpeed = 2f;               // 玩家左右移动速度
    public float leftBoundary = -1.5f;         // 左边界限制
    public float rightBoundary = 1.5f;         // 右边界限制

    // 血量相关
    public float currentValue;                 // 当前血量
    public float MaxValue;                     // 最大血量
    public Slider healthSlider;                // 血量显示的Slider
    public Transform healthBarCanvas;          // 血条所在的Canvas (World Space Canvas)

    // UI 文本相关
    public Text DeCoinMonText;      // 金币减少文本
    public Text BuffText;           // Buff文本

    // 子弹发射点
    public Transform firePoint;

    // Buff动画相关
    public float buffAnimationDuration = 0.5f; // 动画持续时间（秒）
    public Vector3 buffStartScale = Vector3.zero; // Buff文本开始缩放
    public Vector3 buffEndScale = Vector3.one;    // Buff文本结束缩放
    public float buffDisplayDuration = 2f;        // Buff文本显示持续时间（秒）

    // 内部变量
    private Camera mainCamera;                         // 主摄像机
    private UnityArmatureComponent armatureComponent;   // DragonBones Armature 组件

    // 触摸控制相关
    private float touchStartX;      // 触摸开始的X坐标
    private float touchDeltaX;      // 触摸移动的X偏移量
    private bool isTouching = false; // 当前是否有触摸
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
        // 注册事件监听器
        EventDispatcher.instance.Regist(EventNameDef.ShowBuyBulletText, (v) => ShowDeclineMoney());
        ControlMovementWithMouse();
    }

    private void Init()
    {
        // 初始化血量
        currentValue = 10;// PlayInforManager.Instance.playInfor.health;
        MaxValue = 10;// PlayInforManager.Instance.playInfor.health;
        healthSlider.maxValue = MaxValue;
        healthSlider.value = currentValue;

        // 初始化移动速度
        moveSpeed = 2f;//ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
        buffEndScale *= 1.5f;
        // 获取 BuffText 组件并设置为隐藏和缩放为零
        BuffText = transform.Find("PlaySliderCav/BuffText").GetComponent<Text>();
        BuffText.gameObject.SetActive(false);
        BuffText.transform.localScale = buffStartScale;
    }

    void Update()
    {
        // 如果游戏状态不是运行中，跳过更新
        if (GameManage.Instance.gameState != GameState.Running || Time.timeScale == 0)
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
    //手控制玩家移动
    // 使用触摸控制玩家左右移动
    //void ControlMovementWithTouch()
    //{
    //    isTouching = false;
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);

    //        switch (touch.phase)
    //        {
    //            case TouchPhase.Began:
    //                isTouching = true;
    //                touchStartX = touch.position.x;
    //                break;

    //            case TouchPhase.Moved:
    //            case TouchPhase.Stationary:
    //                if (isTouching)
    //                {
    //                    touchDeltaX = touch.position.x - touchStartX;
    //                    float screenWidth = Screen.width;
    //                    // 计算触摸偏移量的比例
    //                    float deltaX = (touchDeltaX / screenWidth) * 2f; // 调整移动灵敏度
    //                    // 计算新的X位置
    //                    float newX = Mathf.Clamp(transform.position.x + deltaX * moveSpeed * Time.deltaTime, leftBoundary, rightBoundary);
    //                    transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    //                    touchStartX = touch.position.x; // 更新起始点以实现连续移动
    //                }
    //                break;

    //            case TouchPhase.Ended:
    //            case TouchPhase.Canceled:
    //                isTouching = false;
    //                break;
    //        }
    //    }
    //}


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

    // 显示金币减少文本
    async void ShowDeclineMoney()
    {
        if (DeCoinMonText != null)
        {
            DeCoinMonText.text = $"-{ConfigManager.Instance.Tables.TablePlayerConfig.Get(PlayInforManager.Instance.playInfor.level).Total}";
            await ShowDeCoinMonText();
        }
    }

    // 显示并隐藏金币减少文本
    private async UniTask ShowDeCoinMonText()
    {
        DeCoinMonText.gameObject.SetActive(true); // 显示文本
        await UniTask.Delay(300);                  // 等待0.5秒
        DeCoinMonText.gameObject.SetActive(false); // 隐藏文本
    }

    // 处理玩家受到伤害
    public void TakeDamage(float damageAmount)
    {
        currentValue = Mathf.Max(currentValue - damageAmount, 0);
        healthSlider.value = currentValue;

        if (currentValue <= 0)
        {
            transform.Find("cover").GetComponent<Collider2D>().isTrigger = true; // 获取碰撞体组件
            PlayerDie();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 6)
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
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
        //TTOD1有复活机会
      
        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            GameManage.Instance.GameOverReset();
            GameFlowManager.Instance.currentLevelIndex++;
            UIManager.Instance.ChangeState(GameState.Ready, GameFlowManager.Instance.currentLevelIndex);
            transform.Find("cover").GetComponent<Collider2D>().isTrigger = false; // 获取碰撞体组件
        }
        else
        {
            if (LevelManager.Instance.levelData.resureNum > 0)
            {
                Time.timeScale = 0;
                LevelManager.Instance.levelData.resureNum--;
                UIManager.Instance.ChangeState(GameState.Resue, 0);
            }
            else
            {
                UIManager.Instance.ChangeState(GameState.GameOver, 0);
                GameManage.Instance.GameOverReset();
            }
        }
    }

    // 新增方法：激活动画并显示 BuffText
    private async UniTaskVoid ActivateBuffText()
    {
        if (BuffText != null)
        {
            BuffText.gameObject.SetActive(true);
            BuffText.transform.localScale = buffStartScale;

            float elapsed = 0f;
            while (elapsed < buffAnimationDuration)
            {
                float t = elapsed / buffAnimationDuration;
                BuffText.transform.localScale = Vector3.Lerp(buffStartScale, buffEndScale, t);
                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }
            BuffText.transform.localScale = buffEndScale;
            // 等待一段时间后隐藏
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            BuffText.transform.localScale = buffStartScale;
            BuffText.gameObject.SetActive(false);
        }
    }

    // 新增方法：外部调用以显示 Buff
    public void ShowBuff(string buffDescription,Font font)
    {
        if (BuffText != null)
        {
            BuffText.font = font;
            BuffText.text = buffDescription;
            ActivateBuffText().Forget(); // 使用 Forget() 来忽略返回的 UniTask
        }
    }
  
}
