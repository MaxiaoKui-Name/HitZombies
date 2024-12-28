using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonBones;
using Hitzb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;
using Transform = UnityEngine.Transform;

public class GameMainPanelController : UIBase
{
    public Button pauseButton;   // 引用暂停按钮
    public Text coinText;        // 引用显示金币的文本框
    private bool isPaused = false;
    public TextMeshProUGUI buffFrozenText;
    public Button buffFrozenBtn;
    public TextMeshProUGUI buffBlastText;
    public Button buffBlastBtn;
    public Image buffBlastBack;
    public Image buffForzenBack;
    public Sprite[] buffBlastImages;
    public Sprite[] buffForzenImages;
    public GameObject Forzen_F;

    [Header("新手引导")]
    public GameObject PanelOne_F;//遮罩图片
    public GameObject Panel_F;//不遮罩
    public GameObject PanelThree_F;
    public GameObject SkillFinger_F1;
    public GameObject SkillFinger_F2;
    public Image GuidArrowL;
    public Image GuidArrowR;
    private Image GuidCircle;
    private Transform Guidfinger_F;

    //private Image Guidfinger;
    private Image GuidText;//新手引导图片
    public GameObject FirstNote_F;
    public GameObject TwoNote_F;
    public GameObject ThreeNote_F;
    public GameObject FourNote_F;
    public GameObject FiveNote_F;
    public GameObject SkillNote_F;
    public Image SkillFinger_F;//手指图片


    public Transform DieImg_F;

    [Header("Spawn Properties")]
    public float bombDropInterval;  // 炸弹投掷间隔时间
    public float planeSpeed = 2f;   // 飞机移动速度

    public GameObject player;
    public GameObject PausePanel;
    // 引用Canvas的RectTransform
    public RectTransform canvasRectTransform;

    public Transform coinspattern_F;
    // 添加用于检测滑动的变量
    private Vector3 lastMousePosition;
    private float dragThreshold = 100f; // 滑动阈值，可以根据需要调整
    private bool isDragging = false;


    //狂暴
    // 新增按钮
    public Button specialButton;          // 狂暴技能按钮
    private Image specialBac_F;
    private Image specialButtonImage;     // 按钮的图片组件
    // 定时器相关变量
    private bool isButtonActive = false; // 按钮是否处于可点击状态
    public bool isSkillActive = false;  // 技能是否激活
    private float cooldownTime = 20f;    // 冷却时间 20 秒
    private float activeTime = 5f;       // 技能持续时间 5 秒
    public Sprite[] Rampages;

    public bool isGuidAnimationPlaying = false; // 标志是否正在播放引导动画
    private bool hasGuidAnimationPlayed = false; // 标志引导动画是否已播放过
    public bool FirstNote_FBool = false;
    public bool TwoNote_FBool = false;
    public bool TwoBeThree = false;
    public bool ThreeNote_FBool = false;
    public bool FourNote_FBool = false;
    public bool FiveNote_FBool = false;
    // 标志文本是否完全显示
    private bool isTextFullyDisplayed = false;


    //来袭动画
    public UnityArmatureComponent EnemiesComeArmature;
    public UnityArmatureComponent MassiveEnemiesComeArmature;
    public UnityArmatureComponent BossComeArmature;


    //顶部金币高亮
    public Transform TopcoinHight_F;
    public UnityArmatureComponent coinHightAmature;
    //特殊怪物的高亮
    public Transform FirstMonsterCoin_F; 

    void Start()
    {
        GetAllChild(transform);
        DOTween.Init();
        // 找到子对象中的按钮和文本框
        //新手引导
        canvasRectTransform = transform.parent.GetComponent<RectTransform>();
        GuidArrowL = childDic["GuidArrowL_F"].GetComponent<Image>();
        GuidArrowR = childDic["GuidArrowR_F"].GetComponent<Image>();
        GuidCircle = childDic["GuidCircle_F"].GetComponent<Image>();
        GuidCircle.transform.parent.gameObject.SetActive(false);
        Guidfinger_F = childDic["Guidfinger_F"];
        FirstNote_F = childDic["FirstNote_F"].gameObject;
        FirstNote_F.SetActive(false);
        TwoNote_F = childDic["TwoNote_F"].gameObject;
        TwoNote_F.SetActive(false);
        ThreeNote_F = childDic["ThreeNote_F"].gameObject;
        ThreeNote_F.SetActive(false);
        FourNote_F = childDic["FourNote_F"].gameObject;
        FourNote_F.SetActive(false);
        FiveNote_F = childDic["FiveNote_F"].gameObject;
        FiveNote_F.SetActive(false);
        coinspattern_F = childDic["coinspattern_F"].GetComponent<RectTransform>();
        DieImg_F = childDic["DieImg_F"];
        DieImg_F.gameObject.SetActive(false);

        SkillNote_F = childDic["SkillNote_F"].gameObject;
        SkillFinger_F = childDic["SkillFinger_F"].GetComponent<Image>();
        PanelOne_F = childDic["PanelOne_F"].gameObject;
        Panel_F = childDic["Panel_F"].gameObject;
        Panel_F.transform.gameObject.SetActive(false);
        PanelThree_F = childDic["PanelThree_F"].gameObject;
        PanelThree_F.transform.gameObject.SetActive(false);
        SkillNote_F.gameObject.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);
        SkillFinger_F1 = childDic["SkillFinger_F1"].gameObject;
        SkillFinger_F2 = childDic["SkillFinger_F2"].gameObject;
        SkillFinger_F1.SetActive(false);
        SkillFinger_F2.SetActive(false);

        //顶部以及怪物高亮
        TopcoinHight_F = childDic["TopcoinHight_F"];
        coinHightAmature = TopcoinHight_F.GetChild(0).GetComponent<UnityArmatureComponent>();
        //TopcoinHight_F.gameObject.SetActive(false);
        FirstMonsterCoin_F = childDic["FirstMonsterCoin_F"];
        //Guidfinger = childDic["Guidfinger_F"].GetComponent<Image>();
        GuidText = childDic["GuidText_F"].GetComponent<Image>();
        pauseButton = childDic["pause_Btn_F"].GetComponent<Button>();
        coinText = childDic["valueText_F"].GetComponent<Text>();
        buffFrozenText = childDic["Frozentimes_F"].GetComponent<TextMeshProUGUI>();
        buffFrozenBtn = childDic["FrozenBtn_F"].GetComponent<Button>();
        buffBlastText = childDic["Blastimes_F"].GetComponent<TextMeshProUGUI>();
        buffBlastBtn = childDic["BlastBtn_F"].GetComponent<Button>();
        Forzen_F = childDic["Forzen_F"].gameObject;
        Forzen_F.SetActive(false);
        buffBlastBack = buffBlastBtn.GetComponent<Image>();
        buffForzenBack = buffFrozenBtn.GetComponent<Image>();
        buffBlastBack.sprite = buffBlastImages[0];
        buffForzenBack.sprite = buffForzenImages[0];
        buffBlastText.text = "0";
        buffFrozenText.text = "0";
        EventDispatcher.instance.Regist(EventNameDef.UPDATECOIN, (v) => UpdateCoinText());
        // 添加暂停按钮的点击事件监听器
        pauseButton.onClick.AddListener(() => StartCoroutine(Onpause_Btn_FClicked()));
        buffFrozenBtn.onClick.AddListener(ToggleFrozen);
        buffBlastBtn.onClick.AddListener(ToggleBlast);
        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            childDic["BuffUI_F"].gameObject.SetActive(false);
        }

        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            pauseButton.transform.parent.gameObject.SetActive(false);
            PanelOne_F.transform.gameObject.SetActive(false);
        }
        if (GameFlowManager.Instance.currentLevelIndex != 0)
        {
            PanelOne_F.transform.gameObject.SetActive(false);
            GuidArrowL.transform.parent.gameObject.SetActive(false);
        }
        if (PlayerPrefs.HasKey("PlayerAccountID"))
        {
            string accountID = PlayerPrefs.GetString("PlayerAccountID");
            PlayInforManager.Instance.playInfor.BalstBuffCount = PlayerPrefs.GetInt($"{accountID}PlayerBalstBuffCount");
            PlayInforManager.Instance.playInfor.FrozenBuffCount = PlayerPrefs.GetInt($"{accountID}PlayerFrozenBuffCount");
            UpdateBuffText(PlayerPrefs.GetInt($"{accountID}PlayerFrozenBuffCount"), PlayerPrefs.GetInt($"{accountID}PlayerBalstBuffCount"));
        }
        // 初始化 specialButton
        specialButton = childDic["specialBtn_F"].GetComponent<Button>();
        specialButtonImage = specialButton.GetComponent<Image>();
        specialButtonImage.sprite = Rampages[0];
        specialBac_F = childDic["specialBac_F"].GetComponent<Image>();
        specialButton.onClick.AddListener(OnSpecialButtonClicked);
        specialButton.interactable = false; // 初始不可点击
        // 开始按钮的冷却协程
        StartCoroutine(SpecialButtonCooldownCoroutine());
        UpdateCoinText();

        //来袭动画
        EnemiesComeArmature = childDic["Enemies_F"].GetChild(0).GetComponent<UnityArmatureComponent>();
        MassiveEnemiesComeArmature = childDic["MassiveEnemies_F"].GetChild(0).GetComponent<UnityArmatureComponent>();
        BossComeArmature = childDic["bossincoming_F"].GetChild(0).GetComponent<UnityArmatureComponent>();
        EnemiesComeArmature.transform.parent.gameObject.SetActive(false);
        MassiveEnemiesComeArmature.transform.parent.gameObject.SetActive(false);
        BossComeArmature.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        // 引导状态下的逻辑
        if (GameManage.Instance.gameState == GameState.Guid)
        {
            // 若尚未播放过引导动画，且没有正在播放动画，则开始播放引导动画
            if (!isGuidAnimationPlaying && !hasGuidAnimationPlayed)
            {
                StartCoroutine(RunGuidAnimationWithDelay());
            }
            else if (!isGuidAnimationPlaying && hasGuidAnimationPlayed)
            {
                HandleNewbieGuide();
            }
        }

        if (GameManage.Instance.gameState == GameState.Running)
        {
            // 当技能激活时，检测点击屏幕发射子弹（此处为其他逻辑，略）
            if (isSkillActive && Input.GetMouseButtonDown(0))
            {
                FireHomingBullet();
            }
            if (GameManage.Instance.clickCount)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    clickCount++;
                }
            }
        }
        if (FistMonsterHightarmatureComponent != null && FistMonsterHightarmatureComponent.animation.isPlaying)
        {
            // 使用unscaledDeltaTime确保动画在暂停时仍然更新
            float deltaTime = Time.timeScale == 0f ? Time.unscaledDeltaTime : Time.deltaTime;
            FistMonsterHightarmatureComponent.animation.AdvanceTime(deltaTime);
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.instance.UnRegist(EventNameDef.UPDATECOIN, (v) => UpdateCoinText());
        coinText = null;
    }

    private IEnumerator Onpause_Btn_FClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        pauseButton.interactable = false;
        yield return StartCoroutine(HandleButtonClickAnimation(transform));
        yield return StartCoroutine(ButtonBounceAnimation(pauseButton.GetComponent<RectTransform>(), OnpauseClicked));
    }

    void UpdateCoinText()
    {
        if (coinText == null) return;
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum:N0}";
    }

    public void UpdateCoinTextWithDOTween(long AddCoin, float durations)
    {
        long currentCoin = PlayInforManager.Instance.playInfor.coinNum;
        float duration = durations;
        long targetCoin = PlayInforManager.Instance.playInfor.coinNum + AddCoin;

        DOTween.To(() => currentCoin, x =>
        {
            currentCoin = x;
            PlayInforManager.Instance.playInfor.coinNum = currentCoin;
            coinText.text = $"{currentCoin:N0}";
        }, targetCoin, duration)
        .SetEase(Ease.Linear)
        .SetUpdate(true);
    }
    #region [新手引导逻辑]
    /// <summary>
    /// 播放新手引导动画的协程:
    /// 1. 手指图标从初始位置移动至圆圈位置，模拟点击动作。
    /// 2. 手指与圆圈一起往左、右移动，回到初始位置，完成一次往返。
    /// </summary>
   private DG.Tweening.Sequence guidSequence;               // 引导动画序列引用
   private bool isInitialGuideDone = false;     // 首次引导动画是否完成

    private IEnumerator RunGuidAnimationWithDelay()
    {
        isGuidAnimationPlaying = true;
        PanelOne_F.SetActive(true);
        GuidCircle.transform.parent.gameObject.SetActive(true);
        Guidfinger_F.gameObject.SetActive(true);
        PreController.Instance.PlayisMove = false;
        // 延迟1秒再执行引导动画
        yield return new WaitForSecondsRealtime(0.01f);
        // 执行引导动画
        RunGuidAnimation();
    }
    private void RunGuidAnimation()
    {
        // 暂停游戏逻辑，突出引导动画
        Time.timeScale = 0f;  // 游戏暂停，确保只执行引导动画
        // 获取 RectTransform
        RectTransform guidCircleRect = GuidCircle.GetComponent<RectTransform>();
        RectTransform skillFingerRect = Guidfinger_F.GetComponent<RectTransform>();

        // 初始位置
        Vector2 initialCirclePos = guidCircleRect.anchoredPosition;
        Vector2 initialSkillFingerPos = skillFingerRect.anchoredPosition;

        // 左右箭头的位置
        Vector2 leftPos = GuidArrowL.rectTransform.anchoredPosition;
        leftPos.x += 50f;
        Vector2 rightPos = GuidArrowR.rectTransform.anchoredPosition;
        rightPos.x -= 50f;

        // 动画持续时间
        float moveDuration = 0.25f;

        // 点击动画序列
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(skillFingerRect.DOAnchorPos(new Vector2(initialSkillFingerPos.x, initialSkillFingerPos.y + 5), 0.1f).SetEase(Ease.InOutSine))
                      .Append(skillFingerRect.DOAnchorPos(initialSkillFingerPos, 0.1f).SetEase(Ease.InOutSine));

        // 移动动画序列
        Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(guidCircleRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                     .Join(skillFingerRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                     .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                     .Join(skillFingerRect.DOAnchorPos(initialSkillFingerPos, moveDuration).SetEase(Ease.InOutSine))
                     .Append(guidCircleRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                     .Join(skillFingerRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                     .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                     .Join(skillFingerRect.DOAnchorPos(initialSkillFingerPos, moveDuration).SetEase(Ease.InOutSine));

        // 合并动画序列
        Sequence guidSequence1 = DOTween.Sequence();
        guidSequence1.Append(clickSequence)
                    .Append(moveSequence)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        // 首次引导动画完成后的逻辑
                        isGuidAnimationPlaying = false;
                        hasGuidAnimationPlayed = true;
                        isInitialGuideDone = true;
                        // 将 guidSequence 设置为 null，允许启动循环动画
                        guidSequence = null;
                        //guidSequence1.Kill();
                        //guidSequence1 = null;
                    });
        // 播放动画序列
        guidSequence1.Play();
    }

    /// <summary>
    /// 当首次引导动画结束后，检测玩家输入：
    /// - 若玩家按下屏幕并拖动超过阈值距离，则结束引导并开始游戏。
    /// - 若玩家一直不拖动，则播放无限循环的引导动画，直到检测到拖动为止。
    /// </summary>
    private Vector3 initialMousePosition; // 初始拖动位置
    // 新增变量用于控制是否由PlayerController处理移动
    private bool isControlTransferred = false;
    private void HandleNewbieGuide()
    {
        // 1. 首次动画完成且无动画播放时，检查玩家拖动行为
        if (!isGuidAnimationPlaying && hasGuidAnimationPlayed && !isControlTransferred)
        {
            // 运行时平台检测
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // 触控设备处理
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            initialMousePosition = touch.position;
                            isDragging = false; // 开始触摸时，拖动标志设为假
                            break;

                        case TouchPhase.Moved:
                            Vector3 currentTouchPosition = touch.position;
                            // —— 角色随拖动实时移动 —— 只在拖动时移动玩家
                            if (!isDragging)
                            {
                                Vector3 delta = currentTouchPosition - initialMousePosition;
                                float distanceSquared = delta.sqrMagnitude;
                                if (distanceSquared > dragThreshold)
                                {
                                    isDragging = true; // 达到拖动阈值，开始拖动
                                }
                            }

                            if (isDragging)
                            {
                                MovePlayerToCurrentPosition(currentTouchPosition);
                            }

                            // 检测拖动距离是否超过阈值
                            if (isDragging)
                            {
                                Vector3 delta = currentTouchPosition - initialMousePosition;
                                float distanceSquared = delta.sqrMagnitude;
                                if (distanceSquared > dragThreshold * dragThreshold)
                                {
                                    // 超过阈值，关闭引导
                                    EndGuideAndTransferControl(currentTouchPosition);
                                }
                            }
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if (!isDragging)
                            {
                                // 玩家点击并释放，没有拖动
                                if (guidSequence == null)
                                {
                                    StartLoopGuideAnimation();
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                // 鼠标事件处理
                if (Input.GetMouseButtonDown(0))
                {
                    initialMousePosition = Input.mousePosition;
                    isDragging = false; // 鼠标点击时，标志设为假
                }

                if (Input.GetMouseButton(0))
                {
                    Vector3 currentMousePosition = Input.mousePosition;
                    // —— 角色随拖动实时移动 —— 只在拖动时移动玩家
                    if (!isDragging)
                    {
                        Vector3 delta = currentMousePosition - initialMousePosition;
                        float distanceSquared = delta.sqrMagnitude;
                        if (distanceSquared > dragThreshold)
                        {
                            isDragging = true; // 达到拖动阈值，开始拖动
                        }
                    }

                    if (isDragging)
                    {
                        MovePlayerToCurrentPosition(currentMousePosition);
                    }

                    // 检测拖动距离是否超过阈值
                    if (isDragging)
                    {
                        Vector3 delta = currentMousePosition - initialMousePosition;
                        float distanceSquared = delta.sqrMagnitude;
                        if (distanceSquared > dragThreshold * dragThreshold)
                        {
                            // 超过阈值，关闭引导
                            EndGuideAndTransferControl(currentMousePosition);
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (!isDragging)
                    {
                        // 玩家点击并释放，没有拖动
                        if (guidSequence == null)
                        {
                            StartLoopGuideAnimation();
                        }
                    }
                }
            }
        }

        // 2. 若首次引导完成后长时间无拖动，则开始循环动画重复提示
        if (isInitialGuideDone && !isGuidAnimationPlaying && guidSequence == null && !isControlTransferred)
        {
            Debug.Log("Starting loop guide animation");
            StartLoopGuideAnimation();
        }
    }

    /// <summary>
    /// 角色实时移动到当前触摸/鼠标位置（只移动X轴，其余保持不变）。
    /// </summary>
    private void MovePlayerToCurrentPosition(Vector3 screenPosition)
    {
        // 获取玩家对象
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("Player对象未找到！");
            return;
        }

        // 获取主摄像机
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("主摄像机未找到！");
            return;
        }

        // 将屏幕坐标转换为世界坐标
        Vector3 screenPos = new Vector3(screenPosition.x, screenPosition.y, cam.nearClipPlane);
        Vector3 targetWorldPos = cam.ScreenToWorldPoint(screenPos);

        // 只在 x 轴上进行移动，y、z 位置保持不变（可根据需求改动）
        Vector3 currentPos = playerObj.transform.position;
        Vector3 endPos = new Vector3(targetWorldPos.x, currentPos.y, currentPos.z);

        playerObj.transform.position = endPos;
    }

    /// <summary>
    /// 启动无限循环的左右往返动画，直到玩家发生拖动行为为止。
    /// </summary>
    private void StartLoopGuideAnimation()
    {
        RectTransform guidCircleRect = GuidCircle.GetComponent<RectTransform>();
        RectTransform skillFingerRect = Guidfinger_F.GetComponent<RectTransform>();

        Vector2 initialCirclePos = guidCircleRect.anchoredPosition;
        Vector2 initialskillFingerPos = skillFingerRect.anchoredPosition;
        Vector2 leftPos = GuidArrowL.rectTransform.anchoredPosition;
        leftPos.x += 50f;
        Vector2 rightPos = GuidArrowR.rectTransform.anchoredPosition;
        rightPos.x -= 50f;
        float moveDuration = 0.25f;

        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(skillFingerRect.DOAnchorPos(new Vector2(initialskillFingerPos.x, initialskillFingerPos.y + 5), 0.1f).SetEase(Ease.InOutSine))
                     .Append(skillFingerRect.DOAnchorPos(initialskillFingerPos, 0.1f).SetEase(Ease.InOutSine));


        guidSequence = DOTween.Sequence();
        guidSequence.Append(guidCircleRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(initialskillFingerPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(initialskillFingerPos, moveDuration).SetEase(Ease.InOutSine));

        Sequence guidSequenceAll = DOTween.Sequence();
        guidSequenceAll.Append(clickSequence)
                    .Append(guidSequence)
                    .SetLoops(-1) // 无限循环
                    .SetUpdate(true);
    }

    /// <summary>
    /// 当检测到玩家拖动或点击行为时，结束引导并切换至游戏运行状态。
    /// </summary>
    private void EndGuideAndTransferControl(Vector3 currentPosition)
    {
        PreController.Instance.PlayisMove = true;
        if (guidSequence != null)
        {
            guidSequence.Kill();
            guidSequence = null;
        }
        isGuidAnimationPlaying = false;
        hasGuidAnimationPlayed = true;
        isInitialGuideDone = true;
        isControlTransferred = true; // 标记控制权已转交
        // 恢复游戏
        Time.timeScale = 1f;
        PanelOne_F.SetActive(false);
        GuidCircle.transform.parent.gameObject.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);

        // 切换游戏状态到运行状态
        GameManage.Instance.SwitchState(GameState.Running);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        // 检查当前是否有持续的输入
        bool isTouching = false;
        Vector3 inputPosition = Vector3.zero;
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // 触控设备处理
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    isTouching = true;
                    inputPosition = touch.position;
                }
            }
        }
        else
        {
            // 鼠标事件处理
            if (Input.GetMouseButton(0))
            {
                isTouching = true;
                inputPosition = Input.mousePosition;
            }
        }
        if (isTouching && playerController != null)
        {
            // 将当前输入位置传递给PlayerController，确保无缝接管
            playerController.StartHandlingInput(inputPosition);
        }

        StartCoroutine(ShowFirstNoteAfterDelay());
    }
    #endregion
    #region[新手提示]
    private IEnumerator ShowFirstNoteAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        ShowFirstNote();
    }

    public void ShowFirstNote()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner1").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        StartCoroutine(ShowMultipleNotesCoroutine(FirstNote_F, guidanceTexts));
    }
    public void ShowTwoNote1()
    {
        TwoNote_FBool = true;
        StartCoroutine(ShowTwoNoteAfterDelay());
    }
    public IEnumerator ShowTwoNoteAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0f);
        Time.timeScale = 0f;
        Panel_F.SetActive(true);
        ShowTwoNote2();
    }
    public void ShowTwoNote2()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner2").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.PlayHight();
        StartCoroutine(ShowMultipleNotesCoroutine(TwoNote_F, guidanceTexts));
    }

    private const string StartAnimation = "start";
    private const string StayAnimation = "stay";
    //ShowThreeNotec产生高亮
    //public void SetHight(Vector2 tartPos)
    //{
    //    coinHight_F.gameObject.SetActive(true);
    //    coinHight_F.GetComponent<RectTransform>().anchoredPosition = tartPos;
    //    PlayHight();
    //}
    #region[顶部玩家高亮]
    private UnityArmatureComponent FistMonsterHightarmatureComponent;
    public void SetFirstMonsterCoinPos(Vector2 targetPos)
    {
        FirstMonsterCoin_F.gameObject.SetActive(true);
        FirstMonsterCoin_F.transform.GetComponent<RectTransform>().anchoredPosition = targetPos;
    }
    public void PlayHight(UnityArmatureComponent hightObj)
    {
        if(hightObj == FirstMonsterCoin_F.GetChild(0).GetComponent<UnityArmatureComponent>()) 
        {
            FistMonsterHightarmatureComponent = hightObj;
        }
        if (hightObj != null)
        {
            // 订阅动画完成事件
            hightObj.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
            // 播放一次start动画
            hightObj.animation.Play(StartAnimation, 1); // 1表示播放一次
        }
    }
    private void OnAnimationComplete(object sender, EventObject eventObject)
    {
         if (eventObject.animationState.name == StartAnimation)
         {
            // 3. armature.display 是一个 GameObject，对其 GetComponent
            GameObject armatureGameObject = eventObject.armature.display as GameObject;
            if (armatureGameObject != null)
            {
                // 4. 在该 GameObject 上获取 UnityArmatureComponent
                UnityArmatureComponent hightObj = armatureGameObject.GetComponent<UnityArmatureComponent>();
                if (hightObj != null)
                {
                    // 5. 播放循环动画
                    hightObj.animation.Play(StayAnimation, 0);  // 0表示无限循环
                }
            }
         }
    }
    //TTOD1 在玩家点击消失时进行隐藏
    public void DisPlayHight(UnityArmatureComponent hightObj)
    {
        if (hightObj != null)
        {
            hightObj.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
            hightObj.animation.Play("<None>");
            if (FistMonsterHightarmatureComponent != null)
                FistMonsterHightarmatureComponent = null;
            hightObj.transform.parent.gameObject.SetActive(false);
        }
    }
    #endregion[顶部玩家高亮]

    #region[顶部金币动画]
    /// 开始循环放大/恢复动画
    /// </summary>
    // 记录金币数量，用于判断什么时候停止循环
    [HideInInspector] public int totalCoins;   // 本次生成的金币总数
    [HideInInspector] public int arrivedCoins; // 已到达目标位置的金币数
    private Coroutine blinkCoroutine;  // 用于存储循环协程的引用，以便随时停止
    public void StartCoinEffectBlink()
    {
        // 若已经在循环，则不再重复启动
        if (blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(CoinEffectBlinkLoop());
        }
    }

    /// <summary>
    /// 停止循环放大/恢复动画
    /// </summary>
    public void StopCoinEffectBlink()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        // 恢复为正常大小
        if (coinText != null)
        {
            coinText.rectTransform.localScale = Vector3.one;
        }
        if (coinspattern_F != null)
        {
            RectTransform patternRect = coinspattern_F.GetComponent<RectTransform>();
            patternRect.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 循环让 coinText 和 coinspattern_F 进行缩放动画：1.2 -> 1
    /// </summary>
    private IEnumerator CoinEffectBlinkLoop()
    {
        while (true)
        {
            // 放大到 1.2
            yield return StartCoroutine(ScaleOverTime(coinText, 1.2f, 0.1f));
            yield return StartCoroutine(ScaleOverTime(coinspattern_F.gameObject, 1.2f, 0.1f));

            // 缩放回 1
            yield return StartCoroutine(ScaleOverTime(coinText, 0.7f, 0.1f));
            yield return StartCoroutine(ScaleOverTime(coinspattern_F.gameObject, 0.7f, 0.1f));
        }
    }
    private IEnumerator ScaleOverTime(Text uiText, float targetScale, float duration)
    {
        if (uiText == null) yield break;

        RectTransform rt = uiText.rectTransform;
        Vector3 initialScale = rt.localScale;
        Vector3 finalScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rt.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.localScale = finalScale;
    }

    private IEnumerator ScaleOverTime(GameObject obj, float targetScale, float duration)
    {
        if (obj == null) yield break;
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector3 initialScale = rt.localScale;
        Vector3 finalScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rt.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.localScale = finalScale;
    }

    /// <summary>
    /// 当单枚金币到达目标位置后，调用此方法
    /// </summary>
    public void OnCoinArrived(EnemyController enemyController)
    {
        arrivedCoins++;
        if (arrivedCoins == 1 && enemyController.isSpecialHealth)
        {
            UpdateCoinTextWithDOTween(enemyController.Enemycoins1 - enemyController.Enemycoins2,0.5f);
        }
        // 如果全部金币都已到达目标位置，则停止循环动画
        if (arrivedCoins >= totalCoins)
        {
            //TTOD1滚分然后隐藏高亮
            DisPlayHight(coinHightAmature);
            PanelThree_F.SetActive(false);
            StopCoinEffectBlink();
        }
    }
    #endregion[顶部金币动画]
    public IEnumerator ShowThreeNote(Vector2 backPos)
    {
        Time.timeScale = 0f;
        SetFirstMonsterCoinPos(backPos);
        PlayHight(FirstMonsterCoin_F.GetChild(0).GetComponent<UnityArmatureComponent>());
        // 暂停游戏时间
        yield return StartCoroutine(ShowThreeNoteAfterDelay(backPos));
    }
    private IEnumerator ShowThreeNoteAfterDelay(Vector2 backPos)
    {
        ThreeNote_FBool = true;
        PanelThree_F.SetActive(true);
        PanelThree panelThree = FindObjectOfType<PanelThree>();
        //Vector2 newbackPos = new Vector2(backPos.x - 55, backPos.y - 24);
        panelThree.UpdateHole(backPos, new Vector2(365f, 181f));
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner3").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        yield return StartCoroutine(ShowMultipleNotesCoroutine(ThreeNote_F, guidanceTexts));

    }
    public IEnumerator ShowFourNote()
    {
        FourNote_FBool = true;
        PanelOne_F.SetActive(true);
        Time.timeScale = 0f;
        // 将SpendMoney和EarMoney使用指定颜色标签包裹起来
        long spendMoney = PlayInforManager.Instance.playInfor.SpendMoney;
        long earMoney = PlayInforManager.Instance.playInfor.EarMoney;
        string guidanceText =
            $"A massive wave of monsters! Let me do the math.You just spent <color=#D3692A><size=38>{spendMoney}</size></color> money but earned <color=#D3692A><size=38>{earMoney}</size></color> gold coins. Your profit is:<color=#D3692A><size=38>{earMoney - spendMoney}</size></color> ! Wow, that's impressive!";
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        yield return StartCoroutine(ShowMultipleNotesCoroutine(FourNote_F, guidanceTexts));
    }

    private IEnumerator ShowFiveNoteAfterDelay()
    {
        yield return new WaitForSecondsRealtime(5f);
        ShowFiveNote();
    }

    //提示5
    public void ShowFiveNote()
    {
        FiveNote_FBool = true;
        Time.timeScale = 0f;
        PanelOne_F.SetActive(true);
        ShowFiveNote2();
    }
    public void ShowFiveNote2()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner5").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        StartCoroutine(ShowMultipleNotesCoroutine(FiveNote_F, guidanceTexts));
    }
   
    public RectTransform textBox;    // 确保在编辑器中赋值

    // 点击计数器，用于跟踪用户的点击次数
    private int clickCount = 0;
    public IEnumerator ShowMultipleNotesCoroutine(
        GameObject noteObject,
        List<string> fullTexts
    )
    {
        // 1. 激活面板
        noteObject.SetActive(true);
        GameManage.Instance.clickCount = true;
        if (FourNote_F != null && noteObject.name == FourNote_F.name)
        {
            // 1a. 开始震动效果并等待其完成
            yield return StartCoroutine(ShakeOnceCoroutine(noteObject.transform.GetChild(1).gameObject, duration: 0.3f, magnitude: 40f));
        }
        // 2. 获取提示文本组件（TextMeshProUGUI）
        TextMeshProUGUI noteText = noteObject.GetComponentInChildren<TextMeshProUGUI>();
        if (noteText == null)
        {
            Debug.LogError("未找到 TextMeshProUGUI 组件！");
            yield break;
        }

        // 3. 若未指定 textBox，则自动获取
        if (textBox == null)
        {
            // 假设层级结构中，第3个子节点下有一个RectTransform
            if (SkillNote_F != null && noteObject.name == SkillNote_F.name)
            {
                textBox = noteObject.transform.GetChild(1).GetComponentInChildren<RectTransform>();
            }
            else
            {
                textBox = noteObject.transform.GetChild(2).GetComponentInChildren<RectTransform>();
            }
        }

        // 4. 每个字符显示的时间间隔
        float charInterval = 0.04f;

        // 5. 遍历所有句子
        for (int i = 0; i < fullTexts.Count; i++)
        {
            string fullText = fullTexts[i];
            if (string.IsNullOrEmpty(fullText)) continue;

            // (A) 在显示新句子前，先清空文本
            noteText.text = "";

            // (B) 逐行逐字显示该句（可点击跳过 => 立刻整句补全）
            yield return StartCoroutine(
                TypeSentenceCoroutine(
                    noteText,
                    fullText,
                    textBox.rect.width,
                    charInterval
                )
            );

            // (C) 等待玩家“再点击一下”后再显示下一句
            //     —— 如果玩家不点击，就停留在当前句
            yield return StartCoroutine(WaitForNextClick());

            // (D) 如果已经是最后一句，执行结束逻辑
            if (i == fullTexts.Count - 1)
            {
                // 补全最后一句并执行结束操作
                noteText.text = fullText;

                // 等待玩家再次点击，执行后续逻辑
                //yield return StartCoroutine(WaitForNextClick());
                noteObject.SetActive(false);
                GameManage.Instance.clickCount = false;
                if (SkillNote_F != null && noteObject.name == SkillNote_F.name)
                {
                    StartCoroutine(SkillNoteNoteCompletion(noteObject));
                }
                else
                {
                    HandleNoteCompletion(noteObject); // 你的后续逻辑
                }
            }
        }
    }

    /// <summary>
    /// 逐行、逐字打出一整句（可自动换行）。点击则“立即补全整句”。
    /// </summary>
    private IEnumerator TypeSentenceCoroutine(
        TextMeshProUGUI noteText,
        string sentence,
        float textBoxWidth,
        float charInterval
    )
    {
        // 清空
        noteText.text = "";

        // 拆分为“单词列表”
        List<string> words = SplitIntoWords(sentence);

        // 用于拼接已显示的行
        string displayedSoFar = "";
        // 当前行内容
        string currentLine = "";

        // 是否要跳过剩余的行（整句）
        bool skipRemaining = false;

        // （1）逐个单词拼接，检测是否需要换行
        for (int w = 0; w < words.Count; w++)
        {
            if (skipRemaining) break;

            // 尝试把下一个单词拼到行里
            string testLine = string.IsNullOrEmpty(currentLine)
                ? words[w]
                : currentLine + " " + words[w];

            // 计算这一行的优先宽度
            Vector2 preferredSize = noteText.GetPreferredValues(displayedSoFar + testLine);

            // 若超出 textBox 宽度 => 先把“上一行”逐字显示
            if (preferredSize.x > textBoxWidth)
            {
                if (!string.IsNullOrEmpty(currentLine))
                {
                    // 逐字显示上一行
                    yield return StartCoroutine(
                        DisplayLine(
                            noteText,
                            displayedSoFar,
                            currentLine,
                            charInterval,
                            onClickSkip: () =>
                            {
                                // 如果玩家点击 => 立刻跳过整句剩余
                                skipRemaining = true;
                            }
                        )
                    );

                    if (skipRemaining) break;

                    // 否则上一行显示完毕 => 换行
                    displayedSoFar += currentLine + "\n";
                }
                // 换到下一行，从当前单词开始
                currentLine = words[w];
            }
            else
            {
                // 没超宽，继续拼到 currentLine
                currentLine = testLine;
            }
        }

        // （2）单词循环结束后，若还有最后一行内容没显示
        if (!string.IsNullOrEmpty(currentLine) && !skipRemaining)
        {
            yield return StartCoroutine(
                DisplayLine(
                    noteText,
                    displayedSoFar,
                    currentLine,
                    charInterval,
                    onClickSkip: () =>
                    {
                        skipRemaining = true;
                    }
                )
            );

            if (!skipRemaining)
            {
                // 若没有被跳过 => 把这一行累加
                displayedSoFar += currentLine;
            }
        }

        // （3）如果点击过 => 直接整句补全
        if (skipRemaining)
        {
            noteText.text = sentence;
        }
        else
        {
            // 否则就是正常的逐字输出完毕
            noteText.text = displayedSoFar;
        }
    }

    /// <summary>
    /// “行级”协程——逐字显示本行；如果玩家点击 => 立刻补全本行并标记要跳过整句。
    /// </summary>
    private IEnumerator DisplayLine(
       TextMeshProUGUI noteText,
       string displayedSoFar,
       string lineToDisplay,
       float charInterval,
       System.Action onClickSkip
    )
    {
        // 使用 StringBuilder 优化字符串拼接
        StringBuilder lineBuffer = new StringBuilder();

        for (int i = 0; i < lineToDisplay.Length; i++)
        {
            // 检测是否有跳过输入
            if (clickCount > 0)
            {
                clickCount--; // 消耗一次点击
                onClickSkip?.Invoke();
                // 直接把本行剩余全部显示，并处理数字放大
                string remaining = lineToDisplay.Substring(i);
                string processedRemaining = WrapDigitsWithSize(remaining);
                noteText.text = displayedSoFar + lineBuffer.ToString() + processedRemaining;
                yield break;
            }

            char currentChar = lineToDisplay[i];

            if (currentChar == '<')
            {
                // 处理富文本标签
                int tagEndIndex = lineToDisplay.IndexOf('>', i);
                if (tagEndIndex == -1)
                {
                    // 没找到 '>'，就当普通字符
                    lineBuffer.Append(currentChar);
                }
                else
                {
                    // 一次性把整个 <...> 标签加入
                    string fullTag = lineToDisplay.Substring(i, tagEndIndex - i + 1);
                    lineBuffer.Append(fullTag);
                    i = tagEndIndex; // 跳到标签结尾
                }
            }
            else
            {
                if (char.IsDigit(currentChar))
                {
                    // 数字字符，检测是否是连续数字
                    int start = i;
                    while (i < lineToDisplay.Length && char.IsDigit(lineToDisplay[i]))
                    {
                        i++;
                    }
                    int length = i - start;
                    string number = lineToDisplay.Substring(start, length);
                    // 添加放大标签
                    lineBuffer.Append($"<size=38>{number}</size>");
                    i--; // 调整索引，因为for循环会自动增加
                }
                else
                {
                    // 普通字符
                    lineBuffer.Append(currentChar);
                }

                // 等待一小段时间
                yield return new WaitForSecondsRealtime(charInterval);
            }

            // 每新增一个字符/标签，就更新 Text
            noteText.text = displayedSoFar + lineBuffer.ToString();
        }
    }

    /// <summary>
    /// 将字符串中的数字字符用<size=38>标签包裹起来，并确保不影响其他富文本标签
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>处理后的字符串</returns>
    private string WrapDigitsWithSize(string input)
    {
        StringBuilder result = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            if (input[i] == '<')
            {
                // 处理富文本标签
                int tagEndIndex = input.IndexOf('>', i);
                if (tagEndIndex == -1)
                {
                    // 没找到 '>'，就当普通字符
                    result.Append(input[i]);
                    i++;
                }
                else
                {
                    // 一次性把整个 <...> 标签加入
                    string fullTag = input.Substring(i, tagEndIndex - i + 1);
                    result.Append(fullTag);
                    i = tagEndIndex + 1;
                }
            }
            else if (char.IsDigit(input[i]))
            {
                // 数字字符，检测是否是连续数字
                int start = i;
                while (i < input.Length && char.IsDigit(input[i]))
                {
                    i++;
                }
                int length = i - start;
                string number = input.Substring(start, length);
                // 添加放大标签
                result.Append($"<size=38>{number}</size>");
            }
            else
            {
                // 普通字符
                result.Append(input[i]);
                i++;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 等待玩家“点击一下”后再继续。（强调：这一次点击是“新的点击”）
    /// </summary>
    private IEnumerator WaitForNextClick()
    {
        // 检查是否已经有未处理的点击
        if (clickCount > 0)
        {
            clickCount--; // 消耗一次点击
            yield break;
        }

        // 1) 如果此时玩家还在按着鼠标（比如上一次点了还没松开）
        //    那么先等他松手，避免一次长按被判定为两次点击
        while (Input.GetMouseButton(0))
        {
            yield return null;
        }

        // 2) 等待下一次真正的按下
        while (clickCount == 0)
        {
            yield return null;
        }

        // 3) 消耗一次点击
        clickCount--;
    }
    // 震动协程
    private IEnumerator ShakeOnceCoroutine(GameObject target, float duration, float magnitude)
    {
        Vector3 originalPosition = target.transform.localPosition;
        float halfDuration = duration / 2f;
        float timer = 0f;

        // 生成一个随机方向的偏移量，确保在所有轴上都有变化
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        Vector3 offset = randomDirection * magnitude;

        // 从原位置移动到偏移位置
        while (timer < halfDuration)
        {
            float t = timer / halfDuration;
            target.transform.localPosition = Vector3.Lerp(originalPosition, originalPosition + offset, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        // 确保达到偏移位置
        target.transform.localPosition = originalPosition + offset;

        // 从偏移位置返回到原位置
        timer = 0f;
        while (timer < halfDuration)
        {
            float t = timer / halfDuration;
            target.transform.localPosition = Vector3.Lerp(originalPosition + offset, originalPosition, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        // 确保回到原位置
        target.transform.localPosition = originalPosition;
    }
    private void HandleNoteCompletion(GameObject noteObject)
    {
        if (FirstNote_F != null && noteObject.name == FirstNote_F.name)
        {
            StartCoroutine(PlayEnemyNote(EnemiesComeArmature));
            StartCoroutine(SpawnPowerBuffDoorDelay());
        }

        if (PanelOne_F != null && PanelOne_F.activeSelf)
        {
            PanelOne_F.SetActive(false);
        }

        if (TwoNote_F != null && noteObject.name == TwoNote_F.name)
        {
            TwoNote_FBool = false;
            if (Panel_F != null && Panel_F.activeSelf)
            {
                Panel_F.SetActive(false);
            }
            Time.timeScale = 1f;
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DisPlayHight();
            }
        }

        if (ThreeNote_F != null && noteObject.name == ThreeNote_F.name)
        {
            //if (PanelThree_F != null && PanelThree_F.activeSelf)
            //{
            //    PanelThree_F.SetActive(false);
            //}
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                StartCoroutine(ReSetMovePlayer());
            }
            DisPlayHight(FirstMonsterCoin_F.GetChild(0).GetComponent<UnityArmatureComponent>());
            ThreeNote_FBool = false;
            StartCoroutine(PlayEnemyNote(MassiveEnemiesComeArmature));
        }

        if (FourNote_F != null && noteObject.name == FourNote_F.name)
        {
            Time.timeScale = 1f;
            StartCoroutine(PlayEnemyNote(BossComeArmature));
            GameObject Boss = Instantiate(Resources.Load<GameObject>("Prefabs/Boss"));
            EnemyController enemyController = Boss.GetComponent<EnemyController>();
            enemyController.isInitialBoss = true;
            Boss.transform.position = PreController.Instance.EnemyPoint;
            StartCoroutine(ShowFiveNoteAfterDelay());
        }

        if (FiveNote_F != null && noteObject.name == FiveNote_F.name)
        {
            Time.timeScale = 1f;
        }
      
    }
    private IEnumerator SkillNoteNoteCompletion(GameObject noteObject)
    {
        yield return StartCoroutine(ShowSkillGuideCoroutine(3));  // 显示狂暴技能引导
    }


    //播放怪物来袭动画
    private IEnumerator PlayEnemyNote(UnityArmatureComponent armatureComponent)
    {
        AudioManage.Instance.PlaySFX("bossshow", null);
        armatureComponent.transform.parent.gameObject.SetActive(true);
        armatureComponent.animation.Play("newAnimation");
        yield return new WaitForSecondsRealtime(armatureComponent.animation.GetState("newAnimation")._duration);
        if(armatureComponent == EnemiesComeArmature)
        {
            StartCoroutine(FirstNote_FBoolDelay());

        }
        if (armatureComponent == MassiveEnemiesComeArmature)
        {
            StartCoroutine(TwoBeThreeDelay());
        }
        armatureComponent.animation.Play("<None>");
        armatureComponent.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator FirstNote_FBoolDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        FirstNote_FBool = true;
    }
    private IEnumerator TwoBeThreeDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        TwoBeThree = true;
    }
    private IEnumerator SpawnPowerBuffDoorDelay()
    {
        yield return new WaitForSecondsRealtime(10f);
        SpawnPowerBuffDoor();
    }
    //产生强力门
    public void SpawnPowerBuffDoor()
    {
        Vector3 spawnPowerBuffDoorPoint = new Vector3(0, 5.8f, 0f);//
        GameObject PowerBuffDoor = Instantiate(Resources.Load<GameObject>("Prefabs/Skill/SpecialBuffDoor"), spawnPowerBuffDoorPoint, Quaternion.identity);
        PreController.Instance.FixSortLayer(PowerBuffDoor);
    }
    //恢复玩家移动
    public IEnumerator ReSetMovePlayer()
    {
        yield return new WaitForSecondsRealtime(1f);
        PreController.Instance.PlayisMove = true;
       
    }
    #endregion[新手提示]
    //修改成用手控制
    //private void HandleNewbieGuide()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0); // 获取第一个触摸点
    //        Vector3 touchPos = touch.position;
    //        Vector2 localPoint;

    //        // 将屏幕点转换为Canvas的本地点
    //        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, touchPos, null, out localPoint);

    //        switch (touch.phase)
    //        {
    //            case TouchPhase.Began:
    //            case TouchPhase.Moved:
    //            case TouchPhase.Stationary:
    //                if (isInside)
    //                {
    //                    // 限制GuidCircle在箭头之间移动
    //                    Vector3 leftLimit = GuidArrowL.rectTransform.anchoredPosition;
    //                    Vector3 rightLimit = GuidArrowR.rectTransform.anchoredPosition;

    //                    float clampedX = Mathf.Clamp(localPoint.x, leftLimit.x, rightLimit.x);
    //                    float clampedY = Mathf.Clamp(localPoint.y, leftLimit.y, rightLimit.y);

    //                    // 设置GuidCircle的位置，保持Y轴不变
    //                    GuidCircle.rectTransform.anchoredPosition = new Vector2(clampedX, GuidCircle.rectTransform.anchoredPosition.y);
    //                    Debug.Log($"GuidCircle 位置: {GuidCircle.rectTransform.anchoredPosition}");

    //                    // 移动玩家
    //                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, Camera.main.nearClipPlane));
    //                    worldPos.z = 0;
    //                    worldPos.y = player.transform.position.y;
    //                    worldPos.x = Mathf.Clamp(worldPos.x, -1.5f, 1.5f);
    //                    player.transform.position = worldPos;
    //                    Debug.Log($"Player 位置: {worldPos}");
    //                }
    //                break;

    //            case TouchPhase.Ended:
    //            case TouchPhase.Canceled:
    //                // 松开手指时，隐藏GuidCircle
    //                GuidCircle.transform.parent.gameObject.SetActive(false);
    //                // 如果需要隐藏Guidfinger，可以取消注释以下代码
    //                // Guidfinger.transform.parent.gameObject.SetActive(false);

    //                AccountManager.Instance.isGuid = true;
    //                GameManage.Instance.SwitchState(GameState.Running);
    //                break;
    //        }
    //    }
    //}
    #region[爆炸和冰冻代码]
    void UapdateBuffBack(int FrozenBuffCount, int BalstBuffCount)
    {
        if (FrozenBuffCount > 0)
            buffForzenBack.sprite = buffForzenImages[1];
        else
            buffForzenBack.sprite = buffForzenImages[0];
        if (BalstBuffCount > 0)
            buffBlastBack.sprite = buffBlastImages[1];
        else
            buffBlastBack.sprite = buffBlastImages[0];
    }

    // 切换暂停和继续游戏的状态
    public void OnpauseClicked()
    {
        Time.timeScale = 0f; // 暂停游戏
        PausePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/InPausePanel"));
        PausePanel.transform.SetParent(transform.parent, false);
        PausePanel.transform.localPosition = Vector3.zero;
    }
    public void UpdateBuffText(int FrozenBuffCount, int BalstBuffCount)
    {
        buffFrozenText.text = $"{FrozenBuffCount}";
        buffBlastText.text = $"{BalstBuffCount}";
        UapdateBuffBack(FrozenBuffCount, BalstBuffCount);
    }
    void ToggleBlast()
    {
        //执行全屏爆炸功能
        if (PlayInforManager.Instance.playInfor.BalstBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.BalstBuffCount--;
            if (GameFlowManager.Instance.currentLevelIndex == 0)
            {
                HideSkillGuide();
            }
            UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
            SpawnPlane().Forget();
        }
    }
    void ToggleFrozen()
    {
        //执行全屏冰冻功能
        if (PlayInforManager.Instance.playInfor.FrozenBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.FrozenBuffCount--;
            if (GameFlowManager.Instance.currentLevelIndex == 0)
            {
                HideSkillGuide();
            }
            UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
            FrozenBombEffect(new Vector3(0, 3, 0)).Forget();
        }
    }
   
    public async UniTask SpawnPlane()
    {
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // 生成飞机在屏幕底部
        Debug.Log("Plane spawned!");
    }
    #endregion
    #region [全屏冰冻逻辑]

    // 冰冻炸弹效果
    private async UniTask FrozenBombEffect(Vector3 ForzenPoint)
    {
        // 播放冰冻效果动画
        Forzen_F.SetActive(true);
        UnityArmatureComponent forzenArmature = Forzen_F.transform.GetChild(0).GetComponentInChildren<UnityArmatureComponent>();
        if (forzenArmature != null)
        {
            forzenArmature.animation.Play("start", 1); // 播放冰冻动画
            await UniTask.Delay(TimeSpan.FromSeconds(forzenArmature.animation.GetState("start")._duration));
        }
        forzenArmature.animation.Play("stay", -1); // 播放冰冻动画
        // 暂停所有敌人和宝箱
        FreezeAllEnemiesAndChests(ForzenPoint, ConfigManager.Instance.Tables.TableTransmitConfig.Get(1).DamageScope);
        // 等待 5 秒
        await UniTask.Delay(5000);
        // 解除冰冻效果
        forzenArmature.animation.Play("end", 1); // 播放冰冻动画
        await UniTask.Delay(TimeSpan.FromSeconds(forzenArmature.animation.GetState("end")._duration));
        UnfreezeAllEnemiesAndChests(ForzenPoint, ConfigManager.Instance.Tables.TableTransmitConfig.Get(1).DamageScope);
        forzenArmature.animation.Play("<None>", -1); // 播放冰冻动画
    }


    private void FreezeAllEnemiesAndChests(Vector3 FreezePos, float width)
    {
        GameManage.Instance.isFrozen = true;
        PreController.Instance.isFrozen = true;
        // 根据冰冻点定义矩形范围
        Vector3 topLeft = new Vector3(FreezePos.x - width / 2, FreezePos.y + width / 2, FreezePos.z);
        Vector3 bottomRight = new Vector3(FreezePos.x + width / 2, FreezePos.y - width / 2, FreezePos.z);
        // 找到并冻结在矩形范围内的敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;

            if (IsWithinRectangle(enemyPos, topLeft, bottomRight))
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead)
                {
                    enemyController.isFrozen = true; // 冻结敌人
                }
            }
        }

        // 找到并冻结在矩形范围内的宝箱
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject chest in chests)
        {
            Vector3 chestPos = chest.transform.position;

            if (IsWithinRectangle(chestPos, topLeft, bottomRight))
            {
                ChestController chestController = chest.GetComponent<ChestController>();
                if (chestController != null && chestController.isVise)
                {
                    chestController.isFrozen = true; // 冻结宝箱
                }
            }
        }
    }

    // 辅助函数，用来检查位置是否在矩形范围内
    private bool IsWithinRectangle(Vector3 position, Vector3 topLeft, Vector3 bottomRight)
    {
        return position.x >= topLeft.x && position.x <= bottomRight.x &&
               position.y <= topLeft.y && position.y >= bottomRight.y;
    }

    // 解除冻结效果
    private void UnfreezeAllEnemiesAndChests(Vector3 FreezePos, float width)
    {
        GameManage.Instance.SetFrozenState(false);
        PreController.Instance.isFrozen = false;
        Debug.Log("解除冰冻=========================！!！");
        // 根据冰冻点定义矩形范围
        Vector3 topLeft = new Vector3(FreezePos.x - width / 2, FreezePos.y + width / 2, FreezePos.z);
        Vector3 bottomRight = new Vector3(FreezePos.x + width / 2, FreezePos.y - width / 2, FreezePos.z);
        // 找到并冻结在矩形范围内的敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;

            if (IsWithinRectangle(enemyPos, topLeft, bottomRight))
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead)
                {
                    enemyController.isFrozen = false; // 冻结敌人
                }
            }
        }

        // 找到并冻结在矩形范围内的宝箱
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject chest in chests)
        {
            Vector3 chestPos = chest.transform.position;

            if (IsWithinRectangle(chestPos, topLeft, bottomRight))
            {
                ChestController chestController = chest.GetComponent<ChestController>();
                if (chestController != null && chestController.isVise)
                {
                    chestController.isFrozen = false; // 冻结宝箱
                }
            }
        }
    }
    #endregion
    #region //原始切枪逻辑

    #region//宝箱技能
    public IEnumerator  ShowSkillGuide(int indexChest)
    {
        // 显示引导并等待引导完成
        yield return StartCoroutine(ShowSkillGuideCoroutine(3));  // 显示狂暴技能引导
    }
    private Vector2 InitialPos = new Vector2(0, 0);
    public Vector2[] SkillGuidPos = new Vector2[]{ new Vector2(42f, -118f), new Vector2(42f, -241), new Vector2(12f, -265) };

    public IEnumerator SkillNoteNote()
    {
        Time.timeScale = 0f;
        string guidanceText = "Too many enemies! Quickly tap the button below to trigger Bounty Frenzy, then tap the screen rapidly to clear them out!";//ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner9").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        yield return StartCoroutine(ShowMultipleNotesCoroutine(SkillNote_F, guidanceTexts));
    }
    private IEnumerator ShowSkillGuideCoroutine(int indexChest)
    {
        Debug.Log("Time.timeScale 设置为: " + Time.timeScale);
        Vector2 targetPos = new Vector2(22, -104);
        if (indexChest == 1)
        {
            SkillFinger_F1.SetActive(true);
            targetPos = SkillGuidPos[0];
            Vector2 startPos = SkillFinger_F.GetComponent<RectTransform>().anchoredPosition;
            InitialPos = startPos;
        }
        if (indexChest == 2)
        {
            SkillFinger_F2.SetActive(true);
            targetPos = SkillGuidPos[1];
        }
        if (indexChest == 3)
        {
            SkillFinger_F2.SetActive(true);
            targetPos = SkillGuidPos[2];
        }
        SkillFinger_F.gameObject.SetActive(true);

        // 计算目标位置（buffBlastBtn 的位置）
        Debug.Log($"目标位置: {targetPos}");

        // 动画持续时间
        float moveDuration = 0.5f; // 移动持续时间
        float clickDuration = 0.25f; // 点击动画持续时间

        // 动画移动到目标位置
        Debug.Log("移动动画开始");
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(InitialPos, targetPos, elapsed / moveDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = targetPos;
        Debug.Log("移动动画完成");

        // 开始重复点击动画
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(SkillFinger_F.transform.DOScale(SkillFinger_F.transform.localScale * 1.2f, 0.2f).SetEase(Ease.InOutSine))
                     .Append(SkillFinger_F.transform.DOScale(SkillFinger_F.transform.localScale, 0.2f).SetEase(Ease.InOutSine))
                     .SetLoops(-1) // 无限循环
                     .SetUpdate(true); // 使用不缩放时间
        clickSequence.Play();
        Debug.Log("开始重复点击动画");

        // 等待 buffBlastBtn 被点击
        bool buttonClicked = false;
        void OnBuffBlastBtnClicked()
        {
            buttonClicked = true;
            switch (indexChest)
            {
                case 1:
                    buffBlastBtn.onClick.RemoveListener(OnBuffBlastBtnClicked);
                    break;
                case 3:
                    specialButton.onClick.RemoveListener(OnBuffBlastBtnClicked);
                    break;
            }
        }

        switch (indexChest)
        {
            case 1:
                buffBlastBtn.onClick.AddListener(OnBuffBlastBtnClicked);
                break;
            case 3:
                specialButton.onClick.AddListener(OnBuffBlastBtnClicked);
                break;
        }

        // 等待按钮点击
        Debug.Log("等待 buffBlastBtn 被点击");
        while (!buttonClicked)
        {
            yield return null;
        }

        // 停止重复点击动画
        clickSequence.Kill();
        Debug.Log("停止重复点击动画");

        // 隐藏引导
        SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = InitialPos;
        SkillNote_F.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);
        // 恢复游戏
        Time.timeScale = 1f;
        Debug.Log("Time.timeScale 恢复为: " + Time.timeScale);
    }


    /// <summary>
    /// 隐藏技能提示
    /// </summary>
    public void HideSkillGuide()
    {
        if (SkillNote_F != null)
            SkillNote_F.SetActive(false);
        if (SkillFinger_F != null)
        {
            SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = InitialPos;
            SkillFinger_F.gameObject.SetActive(false);
        }
        SkillFinger_F1.SetActive(false);
        SkillFinger_F2.SetActive(false);

        // 恢复游戏
        Time.timeScale = 1f;
    }
    #endregion
    #endregion
  

    #region[狂暴技能代码]
    // 狂暴技能按钮点击事件
    private async void OnSpecialButtonClicked()
    {
        if (isButtonActive)
        {
            isButtonActive = false;          // 重置按钮状态
            isSkillActive = true;            // 激活技能
            specialButtonImage.sprite = Rampages[0];
            specialButton.interactable = false; // 按钮不可点击

            // 暂停技能计时
            elapsedTimeSkill = 0f;  // 重置技能计时
            elapsedTimeBtn = 0f;    // 重置冷却计时
            // 引导完成后，继续技能冷却计时
            StartCoroutine(SkillActiveCoroutine());
        }
    }

    public float elapsedTimeBtn;         // 冷却计时
    public float elapsedTimeSkill;       // 技能持续计时
    private IEnumerator SkillActiveCoroutine()
    {
        ragepBJ = GameObject.Find("Player/rage");
        elapsedTimeSkill = 0f;
        isStayAnimationPlayed = false;
        isEndAnimationPlayed = false;
        // 播放 start 动画
        PlayDragonBonesAnimation("start", elapsedTimeSkill);
        // 等待直到 start 动画结束
        while (elapsedTimeSkill < activeTime)
        {
            elapsedTimeSkill += Time.unscaledDeltaTime; // 使用不受 Time.timeScale 影响的时间
                                                        // 在 start 动画播放完成后切换到 stay 动画
            if (!isStayAnimationPlayed)
            {
                PlayDragonBonesAnimation("stay", elapsedTimeSkill);
            }
            if (!isEndAnimationPlayed)
            {
                // 在技能持续时间的最后 1 秒切换到 end 动画
                PlayDragonBonesAnimation("end", elapsedTimeSkill);

            }
            yield return null;
        }
        isSkillActive = false;
    }

    private bool isStayAnimationPlayed = false; // 标志 stay 动画是否已经播放
    private bool isEndAnimationPlayed = false;  // 标志 end 动画是否已经播放
    private UnityArmatureComponent armatureComponent;
    private GameObject ragepBJ ;
    private void PlayDragonBonesAnimation(string animationName, float elapsedTimeSkill)
    {
        // 确保玩家对象存在，并且有 DragonBones 组件
        if (ragepBJ == null)
        {
            Debug.LogError("找不到玩家的 rage 对象！");
            return;
        }
        // 查找玩家对象下的 DragonBones 动画组件
        armatureComponent = ragepBJ.GetComponent<UnityArmatureComponent>();

        if (armatureComponent != null)
        {
            // 播放指定的动画
            if (animationName == "stay")
            {
                if (elapsedTimeSkill > armatureComponent.animation.GetState("start")._duration)
                {
                    armatureComponent.animation.Play("stay", -1);
                    isStayAnimationPlayed = true;
                }
            }
            else if (animationName == "end")
            { // 检查 _duration 是否有效
                if (elapsedTimeSkill > activeTime - 1)
                {
                    armatureComponent.animation.Play("end", 1);
                    Debug.Log($"End Animation Duration: {armatureComponent.animation.GetState("end")._duration}");
                    isEndAnimationPlayed = true; // 确保 end 动画只播放一次
                }
            }
            else
            {
                armatureComponent.animation.Play(animationName, 1);
            }
        }
        else
        {
            Debug.LogError("找不到 DragonBones 动画组件！");
        }
    }

    private IEnumerator SpecialButtonCooldownCoroutine()
    {
        while (true)
        {
            // Start of cooldown, reset fillAmount to 0
            specialBac_F.fillAmount = 0f;

            // Wait for cooldown time (20 seconds)
            elapsedTimeBtn = 0f;
            while (elapsedTimeBtn < cooldownTime)
            {
                elapsedTimeBtn += Time.unscaledDeltaTime; // Use unscaled time for accuracy
                specialBac_F.fillAmount = elapsedTimeBtn / cooldownTime; // Update fill amount
                yield return null;
            }

            // Ensure fillAmount is set to 1 at the end of cooldown
            specialBac_F.fillAmount = 1f;

            // Activate button
            specialButtonImage.sprite = Rampages[1];
            specialButton.interactable = true;
            isButtonActive = true;
            // 显示引导并等待引导完成
            // 显示引导并等待引导完成
            if (PlayInforManager.Instance.playInfor.isFirstSpecial && GameFlowManager.Instance.currentLevelIndex == 1)
            {
                PreController.Instance.isRageSkill = true;
                PlayInforManager.Instance.playInfor.isFirstSpecial = false;
                AccountManager.Instance.SaveAccountData();
                yield return StartCoroutine(SkillNoteNote());  // 显示狂暴技能引导
                PreController.Instance.isRageSkill = false;
            }

            // Wait for button to be clicked
            while (isButtonActive)
            {
                yield return null;
            }

            // Skill is active, reset fillAmount if needed
            specialBac_F.fillAmount = 0f;

            // Wait for skill active time
            while (isSkillActive)
            {
                yield return null;
            }
        }
    }


    private void FireHomingBullet()
    {
        // 获取玩家位置
        if (player == null)
            player = GameObject.Find("Player");
        Vector3 playerPosition = player.transform.position;

        // 获取所有敌人和宝箱
        List<GameObject> allTargets = new List<GameObject>();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeInHierarchy)
            {
                allTargets.Add(enemy);
            }
        }
        foreach (GameObject chest in chests)
        {
            if (chest.activeInHierarchy && chest.transform.position.y > 1.3)
            {
                allTargets.Add(chest);
            }
        }

        // 按距离从近到远排序
        allTargets.Sort((a, b) =>
        {
            float distA = Vector3.Distance(playerPosition, a.transform.position);
            float distB = Vector3.Distance(playerPosition, b.transform.position);
            return distA.CompareTo(distB);
        });

        foreach (GameObject target in allTargets)
        {
            float targetHealth = 0f;
            if (target.CompareTag("Enemy"))
            {
                EnemyController enemyController = target.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead)
                {
                    targetHealth = enemyController.health;
                }
                else
                {
                    continue; // 跳过已死亡的敌人
                }
            }
            else if (target.CompareTag("Chest"))
            {
                ChestController chestController = target.GetComponent<ChestController>();
                if (chestController != null)
                {
                    targetHealth = chestController.chestHealth;
                }
                else
                {
                    continue; // 跳过无效的宝箱
                }
            }

            // 计算飞向该目标的子弹的总伤害
            float totalIncomingDamage = 0f;
            foreach (BulletController bullet in PreController.Instance.flyingBullets)
            {
                if (bullet.target == target.transform)
                {
                    totalIncomingDamage += bullet.firepower;
                }
            }

            if (totalIncomingDamage < targetHealth)
            {
                // 目标需要更多伤害，发射子弹
                PreController.Instance.SpawnHomingBullet(playerPosition, target.transform);
                return; // 只发射一颗子弹
            }
            else
            {
                // 该目标的飞行子弹已经足够，尝试下一个目标
                continue;
            }
        }
    }
    #endregion[狂暴技能代码]
}
