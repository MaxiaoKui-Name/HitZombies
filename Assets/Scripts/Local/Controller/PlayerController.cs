using UnityEngine;
using UnityEngine.UI;
using DragonBones;
using Transform = UnityEngine.Transform;
using Text = UnityEngine.UI.Text;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using Spine;

public class PlayerController : MonoBehaviour
{
    // 玩家移动相关
    public float moveSpeed = 2f;               // 玩家左右移动速度
    public float leftBoundary = -1.5f;         // 左边界限制
    public float rightBoundary = 1.5f;         // 右边界限制

    // 血量相关
    //public float currentValue;                 // 当前血量
    //public float MaxValue;                     // 最大血量
    //public Slider healthSlider;                // 血量显示的Slider
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
    public UnityArmatureComponent armatureComponent;   // DragonBones Armature 组件

    // 触摸控制相关
    private float touchStartX;      // 触摸开始的X坐标
    private float touchDeltaX;      // 触摸移动的X偏移量
    public bool isTouching = false; // 当前是否有触摸

    public GameMainPanelController gameMainPanelController; // 引用 GameMainPanelController


    private float longPressDuration = 1f; // 长按持续时间
    private float pressTimer = 0f;
    private bool isLongPress = false;
    public GameObject chooseGunPanel;

    // 触摸控制相关
    private Vector2 touchStartPos;      // 触摸开始的位置
    private Vector2 touchCurrentPos;    // 当前触摸的位置
    private float swipeThreshold = 50f; // 判断滑动的阈值
    private void Start()
    {
        // 初始化玩家血量和血条
        Init();
        //TTPD1增加切枪切换龙骨逻辑
        ReplaceGunDragon();

    }

    public void Init()
    {
        // 初始化摄像机
        mainCamera = Camera.main;
        isTouching = false;
        // 初始化血量
        #region[玩家血条]
        //currentValue = 10;// PlayInforManager.Instance.playInfor.health;
        //MaxValue = 10;// PlayInforManager.Instance.playInfor.health;
        //healthSlider.maxValue = MaxValue;
        //healthSlider.value = currentValue;
        #endregion[玩家血条]

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
        //HandleTouchInput();

    }

    public void ReplaceGunDragon()
    {
        // 保存当前的动画状态（如果需要）
        string currentAnimation = armatureComponent?.animation?.lastAnimationName;
        string newArmatureName = PlayInforManager.Instance.playInfor.currentGun.gunName;
        // 释放当前的 armature
        if (armatureComponent != null)
        {
            armatureComponent.armature.Dispose();
        }
        // 使用新的 armatureName 重新构建骨架
        armatureComponent = UnityFactory.factory.BuildArmatureComponent(newArmatureName.Substring(0,7), "player", transform.Find("player1").gameObject.name);
        armatureComponent.transform.gameObject.name = "player1";
        armatureComponent.transform.parent = this.transform;
        armatureComponent.transform.localPosition = new Vector3(-0.037f, -0.226f, 0);
        armatureComponent.transform.localScale = Vector3.one;

        // 检查 armatureComponent 是否成功创建
        if (armatureComponent != null)
        {
            // 恢复之前的动画状态，或播放新动画
            if (!string.IsNullOrEmpty(currentAnimation) && armatureComponent.animation.HasAnimation(currentAnimation))
            {
                armatureComponent.animation.Play(currentAnimation);
            }
            else
            {
                armatureComponent.animation.Play("walk"); // 如果没有保存的动画，播放默认动画
            }
        }
        else
        {
            Debug.LogError("Failed to create armatureComponent for: " + newArmatureName);
            return; // 如果创建失败，提前返回
        }
    }

    void Update()
    {
        if (PlayHightarmatureComponent != null && PlayHightarmatureComponent.animation.isPlaying && !PreController.Instance.PlayisMove)
        {
            // 使用unscaledDeltaTime确保动画在暂停时仍然更新
            float deltaTime = Time.timeScale == 0f ? Time.unscaledDeltaTime : Time.deltaTime;
            PlayHightarmatureComponent.animation.AdvanceTime(deltaTime);
        }
        // 如果游戏状态不是运行中，跳过更新
        if (GameManage.Instance.gameState != GameState.Running)// || Time.timeScale == 0
            return;
        // 使用鼠标控制玩家左右移动
        if (PreController.Instance.PlayisMove)
        {
            HandleTouchInput();
        }
        // 更新血条的位置，使其跟随玩家移动
        UpdateHealthBarPosition();

        // 新增部分：检测敌人并显示/隐藏 DieImg_F
        CheckEnemiesInDetectionArea();
        //HandleInput();

    }
    /// <summary>
    /// 处理触摸输入以控制玩家移动
    /// </summary>
    /// <summary>
    /// 处理触摸输入以控制玩家移动（优化滑动灵敏度）
    /// </summary>
    //private void HandleTouchInput()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);

    //        switch (touch.phase)
    //        {
    //            case TouchPhase.Began:
    //                isTouching = true;
    //                touchStartPos = touch.position;
    //                break;

    //            case TouchPhase.Moved:
    //                if (isTouching)
    //                {
    //                    touchCurrentPos = touch.position;
    //                    float deltaX = (touchCurrentPos.x - touchStartPos.x) / Screen.width * 5;

    //                    // 根据滑动增量计算新位置
    //                    float newX = Mathf.Clamp(transform.position.x + deltaX, leftBoundary, rightBoundary);
    //                    transform.position = new Vector3(newX, transform.position.y, 0);

    //                    // 更新触摸开始位置，避免滑动过快问题
    //                    touchStartPos = touchCurrentPos;
    //                }
    //                break;

    //            case TouchPhase.Ended:
    //            case TouchPhase.Canceled:
    //                isTouching = false;
    //                break;
    //        }
    //    }
    //}

    /// <summary>
    /// 使用鼠标输入模拟触摸滑动
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.GetMouseButtonDown(0)) // 鼠标左键按下
        {
            isTouching = true;
            touchStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0)) // 鼠标左键按住
        {
            if (isTouching)
            {
                touchCurrentPos = Input.mousePosition;
                float deltaX = (touchCurrentPos.x - touchStartPos.x) / Screen.width * 5;

                // 计算新位置
                float newX = Mathf.Clamp(transform.position.x + deltaX, leftBoundary, rightBoundary);
                transform.position = new Vector3(newX, transform.position.y, 0);

                // 更新触摸起点
                touchStartPos = touchCurrentPos;
            }
        }

        if (Input.GetMouseButtonUp(0)) // 鼠标左键松开
        {
            isTouching = false;
        }
    }



    //长按切枪功能
    //private void HandleInput()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        pressTimer = 0f;
    //        isLongPress = true;
    //    }
    //    if (Input.GetMouseButton(0))
    //    {
    //        pressTimer += Time.deltaTime;
    //        if (isLongPress && pressTimer >= longPressDuration)
    //        {
    //            isLongPress = false;
    //            // 添加判断，确保当前没有打开的chooseGunPanel
    //            if (chooseGunPanel == null || !chooseGunPanel.activeSelf)
    //            {
    //                OnPlayerLongPressed();
    //            }
    //        }
    //    }
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        isLongPress = false;
    //    }
    //}

    private void OnPlayerLongPressed()
    {
        // 显示ChooseGunPanelUI
        // 实例化奖励面板
        chooseGunPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/ChooseGunPanelNew"));
        chooseGunPanel.transform.SetParent(GameObject.Find("UICanvas").transform, false);
        chooseGunPanel.transform.localPosition = Vector3.zero;
        ChooseGunPanelController chooseGunPanelController = chooseGunPanel.GetComponent<ChooseGunPanelController>();
        chooseGunPanelController.ShowChooseGunPanel(false); // 参数表示不是第一次
    }
    public UnityArmatureComponent dieImgArmature;
    // 新增方法：检测指定区域内是否有敌人
    private void CheckEnemiesInDetectionArea()
    {
        // 查找 GameMainPanelController 实例
        gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        if (gameMainPanelController == null || gameMainPanelController.DieImg_F == null)
            return;

        // 获取玩家当前位置
        Vector2 playerPosition = transform.position;

        // 使用 OverlapBoxAll 检测指定区域内的敌人
        int layerEnemy = LayerMask.GetMask("Enemy");
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(playerPosition, detectionAreaSize, 0, layerEnemy);

        // 判断区域内是否有敌人
        bool hasEnemies = hitColliders.Length > 0;

        // 获取 DieImg_F 的 DragonBones 动画组件
        dieImgArmature = gameMainPanelController.DieImg_F.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        if (dieImgArmature == null)
        {
            Debug.LogError("DieImg_F 未找到 DragonBones 动画组件！");
            return;
        }

        // 根据敌人检测结果控制动画逻辑
        if (hasEnemies)
        {
            // 显示 DieImg_F
            gameMainPanelController.DieImg_F.gameObject.SetActive(true);

            // 如果当前动画不是 "start" 或 "stay"，播放 "start" 动画
            if (dieImgArmature.animation.lastAnimationName != "start" && dieImgArmature.animation.lastAnimationName != "stay")
            {
                dieImgArmature.animation.Play("start", 1); // 播放 "start" 动画一次
                Debug.Log("检测到敌人，播放 start 动画");
            }
            else if (!dieImgArmature.animation.isPlaying && dieImgArmature.animation.lastAnimationName == "start")
            {
                // 如果 "start" 动画播放完成且区域内仍有敌人，则播放 "stay" 动画
                dieImgArmature.animation.Play("stay", 0); // 循环播放 "stay" 动画
                Debug.Log("start 动画完成，播放 stay 动画");
            }
        }
        else
        {
            // 如果没有敌人
            if (dieImgArmature != null)
            {
                // 如果当前动画是 "start" 或 "stay"，则播放 "end" 动画
                if (dieImgArmature.animation.lastAnimationName == "start" || dieImgArmature.animation.lastAnimationName == "stay")
                {
                    dieImgArmature.animation.Play("end", 1); // 播放 "end" 动画一次
                    Debug.Log("检测到无敌人，播放 end 动画");
                }
                else if (!dieImgArmature.animation.isPlaying && dieImgArmature.animation.lastAnimationName == "end")
                {
                    // 如果 "end" 动画播放完成，则切回 <None> 状态
                    dieImgArmature.animation.Play(null); // 切回默认状态
                    Debug.Log("end 动画播放完成，切回默认状态");
                }
            }
            // 隐藏 DieImg_F
            gameMainPanelController.DieImg_F.gameObject.SetActive(false);
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
    //void ControlMovementWithMouse()
    //{
    //    // 获取鼠标在屏幕上的X轴位置
    //    Vector3 mousePosition = Input.mousePosition;

    //    // 将屏幕坐标转换为世界坐标
    //    // 设置Z值与玩家的Z值相同，以确保转换正确
    //    mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
    //    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

    //    // 仅使用X轴的位置更新玩家位置
    //    Vector3 newPosition = new Vector3(worldPosition.x, transform.position.y, transform.position.z);

    //    // 限制玩家移动范围在左右边界之间
    //    newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);

    //    // 设置玩家的位置
    //    transform.position = newPosition;
    //}
    //手控制玩家移动
    // 使用触摸控制玩家左右移动
    //void ControlMovementWithMouse()
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
    #region[玩家血条相关代码]
    //更新血条的位置，确保它跟随玩家
    void UpdateHealthBarPosition()
    {
        if (healthBarCanvas != null)
        {
            // 将血条设置为玩家头顶的位置 (世界坐标)
            healthBarCanvas.position = transform.position + new Vector3(0, 1.6f, 0);  // Y轴的偏移量
            healthBarCanvas.localScale = new Vector3(0.01f, 0.01f, 0.01f);  // 调整血条的缩放
        }
    }



    #endregion[玩家血条相关代码]

    // 显示金币减少文本
    private async void ShowDeclineMoney()
    {
        if (DeCoinMonText != null)
        {
            // 实例化一个新的DeCoinMonText对象，设置其父对象为预制体的父对象
            Text newText = Instantiate(DeCoinMonText, DeCoinMonText.transform.parent);
            newText.GetComponent<RectTransform>().anchoredPosition = new Vector3(32.9f, -84, 0);
            // 设置文本内容，根据是否子弹成本为零来决定显示内容
            if (PreController.Instance.isBulletCostZero)
            {
                newText.text = $"-{0}";
            }
            else
            {
                float total = ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
                newText.text = $"-{total}";
            }

            // 设置初始颜色为完全不透明
            Color initialColor = newText.color;
            initialColor.a = 1f;
            newText.color = initialColor;

            // 重置缩放为1
            newText.transform.localScale = Vector3.one * 1.2f;

            // 确保文本对象可见
            newText.gameObject.SetActive(true);

            // 开始执行显示动画
            await ShowDeCoinMonText(newText);
        }
    }
    // 动画名称
    private const string StartAnimation = "start";
    private const string StayAnimation = "stay";
    private UnityArmatureComponent PlayHightarmatureComponent;
    public void PlayHight()
    {
        PlayHightarmatureComponent = transform.Find("PlaySliderCav/PlayHigh").GetComponent<UnityArmatureComponent>();
        if (PlayHightarmatureComponent != null)
        {
            // 订阅动画完成事件
            PlayHightarmatureComponent.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
            // 播放一次start动画
            PlayHightarmatureComponent.animation.Play(StartAnimation, 1); // 1表示播放一次
        }
    }
    private void OnAnimationComplete(object sender, EventObject eventObject)
    {
        if (eventObject.animationState.name == StartAnimation)
        {
            // 播放循环的stay动画
            PlayHightarmatureComponent.animation.Play(StayAnimation, 0); // 0表示无限循环
        }
    }
    //TTOD1 在玩家点击消失时进行隐藏
    public void DisPlayHight()
    {
        if (PlayHightarmatureComponent != null)
        {
            PlayHightarmatureComponent.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
            PlayHightarmatureComponent.animation.Play("<None>");
            PlayHightarmatureComponent.gameObject.SetActive(false);
        }
    }

    #region[玩家金币减少效果]
    /// <summary>
    /// 显示并隐藏金币减少文本的动画
    /// </summary>
    /// <param name="text">需要动画的文本对象</param>
    private async UniTask ShowDeCoinMonText(Text text)
    {
        if (text == null) return;

        // 获取RectTransform组件以便进行位置和缩放的操作
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        // 存储初始位置和缩放
        Vector2 initialPosition = rectTransform.anchoredPosition;
        Vector3 initialScale = rectTransform.localScale;

        // 设置动画参数
        float duration = 0.3f; // 动画持续时间为1秒
        float elapsed = 0f;   // 已过时间初始化

        // 动画循环
        while (elapsed < duration)
        {
            // 计算动画进度（0到1）
            float t = elapsed / duration;

            // 计算新的位置，向上移动50单位
            Vector2 newPosition = Vector2.Lerp(initialPosition, initialPosition + Vector2.up * 40f, t);
            rectTransform.anchoredPosition = newPosition;
            // 计算新的颜色透明度，从1渐变到0
            Color newColor = text.color;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            text.color = newColor;
            // 计算新的缩放，从1缩小到0.5
            Vector3 newScale = Vector3.Lerp(initialScale, Vector3.one * 0.8f, t);
            rectTransform.localScale = newScale;
            // 增加已过时间
            elapsed += Time.deltaTime;
            // 等待下一帧
            await UniTask.Yield();
        }

        // 确保动画结束时的最终状态
        rectTransform.anchoredPosition = initialPosition + Vector2.up * 50f;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        rectTransform.localScale = Vector3.one * 0.8f;
        // 销毁文本对象，结束动画
        Destroy(text.gameObject);
    }



    #endregion[玩家金币减少效果]
    // 处理玩家受到伤害
    public void TakeDamage()
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
            PlayInforManager.Instance.playInfor.SetGun(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation, ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource);
            AccountManager.Instance.SaveAccountData();
            PlayInforManager.Instance.playInfor.attackSpFac = 0;
            GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
            Destroy(gameMainPanelController.gameObject);
            UIManager.Instance.ChangeState(GameState.GameOver);
            EventDispatcher.instance.DispatchEvent(EventNameDef.GAME_OVER);
            //TTPD1增加切枪切换龙骨逻辑
            ReplaceGunDragon();
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
                AudioManage.Instance.PlaySFX("fail", null);
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
