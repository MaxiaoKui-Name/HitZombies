using UnityEngine;
using UnityEngine.UI;
using DragonBones;
using Transform = UnityEngine.Transform;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

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


    // 新增部分：检测敌人并显示 DieImg_F
    [Header("敌人检测相关")]
    public Vector2 detectionAreaSize = new Vector2(10f, 6f); // 检测区域大小（长宽）


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
    public bool isTouching = false; // 当前是否有触摸

    public GameMainPanelController gameMainPanelController; // 引用 GameMainPanelController
    private void Start()
    {
        // 初始化玩家血量和血条
        Init();
    }

    public void Init()
    {
        // 初始化摄像机
        mainCamera = Camera.main;
        isTouching = false;
        // 初始化血量
        currentValue = 10;// PlayInforManager.Instance.playInfor.health;
        MaxValue = 10;// PlayInforManager.Instance.playInfor.health;
        healthSlider.maxValue = MaxValue;
        healthSlider.value = currentValue;

        // 初始化移动速度
        moveSpeed = ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
        // 获取 BuffText 组件并设置为隐藏和缩放为零
        DeCoinMonText = transform.Find("PlaySliderCav/DecoinMonText").GetComponent<Text>();
        DeCoinMonText.gameObject.SetActive(false);
        BuffText = transform.Find("PlaySliderCav/BuffText").GetComponent<Text>();
        BuffText.gameObject.SetActive(false);
        BuffText.transform.localScale = buffStartScale;
        // 获取 DragonBones Armature 组件
        armatureComponent = GameObject.Find("Player/player1").GetComponent<UnityArmatureComponent>();
        transform.Find("cover").GetComponent<Collider2D>().isTrigger = false; // 获取碰撞体组件
        transform.GetComponent<Collider2D>().isTrigger = false;// 获取碰撞体组件
        // 播放并循环指定的动画
        if (armatureComponent != null)
        {
            PlayDragonAnimation();
        }
        PreController.Instance.OnPlayerFiring += UpdatePlayerAnimation; // 绑定事件处理方法
        // 注册事件监听器
        EventDispatcher.instance.Regist(EventNameDef.ShowBuyBulletText, (v) => ShowDeclineMoney());
        ControlMovementWithMouse();
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

        // 新增部分：检测敌人并显示/隐藏 DieImg_F
        CheckEnemiesInDetectionArea();

    }


    // 新增方法：检测指定区域内是否有敌人
    private void CheckEnemiesInDetectionArea()
    {
        // 新增部分：查找 GameMainPanelController 实例
        if (SceneManager.GetActiveScene().name == "First")
        {
            gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        }
        if (gameMainPanelController == null)
            return;

        Vector2 playerPosition = transform.position;
        // 使用 OverlapBoxAll 检测指定区域内的敌人
        int LayerEnemy = LayerMask.NameToLayer("Enemy");
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(playerPosition, detectionAreaSize,LayerEnemy);
        bool hasEnemies = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                hasEnemies = true;
                break;
            }
        }

        if (!hasEnemies)
        {
            // 没有敌人，不显示 DieImg_F
            if (gameMainPanelController.DieImg_F != null && gameMainPanelController.DieImg_F.gameObject.activeSelf)
            {
                gameMainPanelController.DieImg_F.gameObject.SetActive(false);
                Debug.Log("检测到无敌人，隐藏 DieImg_F");
            }
        }
        else
        {
            // 有敌人，显示 DieImg_F
            if (gameMainPanelController.DieImg_F != null && !gameMainPanelController.DieImg_F.gameObject.activeSelf)
            {
                gameMainPanelController.DieImg_F.gameObject.SetActive(true);
                Debug.Log("检测到有敌人，显示 DieImg_F");
            }
        }
    }

    // 可视化检测区域，方便调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionAreaSize);
    }
    // 新增方法：更新玩家动画
    void UpdatePlayerAnimation()
    {
        if (PreController.Instance.isFiring)
        {
            if (armatureComponent.animation.lastAnimationName != "walk+hit")
            {
                armatureComponent.animation.Play("walk+hit", 0);  // 发射子弹时播放攻击动画
            }
        }
        else
        {
            if (armatureComponent.animation.lastAnimationName != "walk")
            {
                armatureComponent.animation.Play("walk", 0);  // 不发射子弹时播放行走动画
            }
        }
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
            armatureComponent.animation.Play("walk", 0);  // 无限循环动画
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
            DeCoinMonText.text = $"-{ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total}";
            await ShowDeCoinMonText();
        }
    }

    // 显示并隐藏金币减少文本
    private async UniTask ShowDeCoinMonText()
    {
        DeCoinMonText.gameObject.SetActive(true); // 显示文本
        await UniTask.Delay(1000);                  // 等待0.5秒
        DeCoinMonText.gameObject.SetActive(false); // 隐藏文本
    }

    // 处理玩家受到伤害
    public void TakeDamage(float damageAmount)
    {
        transform.Find("cover").GetComponent<Collider2D>().isTrigger = true; // 获取碰撞体组件
        transform.GetComponent<Collider2D>().isTrigger = true;// 获取碰撞体组件
        PlayerDie();
        //currentValue = Mathf.Max(currentValue - damageAmount, 0);
        //healthSlider.value = currentValue;
        //if (currentValue <= 0)
        //{
        //    transform.Find("cover").GetComponent<Collider2D>().isTrigger = true; // 获取碰撞体组件
        //    transform.GetComponent<Collider2D>().isTrigger = true;// 获取碰撞体组件
        //    PlayerDie();
        //}
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
            PlayInforManager.Instance.playInfor.level = GameFlowManager.Instance.currentLevelIndex;
            PlayInforManager.Instance.playInfor.SetGun(LevelManager.Instance.levelData.GunBulletList[AccountManager.Instance.GetTransmitID(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0])]);
            AccountManager.Instance.SaveAccountData();
            PlayInforManager.Instance.playInfor.attackSpFac = 0;
            GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
            Destroy(gameMainPanelController.gameObject);
            UIManager.Instance.ChangeState(GameState.Ready);
            GameManage.Instance.InitialPalyer();

        }
        else
        {
            if (PlayInforManager.Instance.playInfor.ResueeCount > 0)
            {
                PlayInforManager.Instance.playInfor.ResueeCount--;
                Time.timeScale = 0;
                UIManager.Instance.ChangeState(GameState.Resue);

            }
            else
            {
                PlayInforManager.Instance.playInfor.attackSpFac = 0;
                AccountManager.Instance.SaveAccountData();
                GameManage.Instance.GameOverReset();
                if(GameManage.Instance.gameState != GameState.NextLevel)
                {
                    UIManager.Instance.ChangeState(GameState.GameOver);
                    EventDispatcher.instance.DispatchEvent(EventNameDef.GAME_OVER);
                }
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
