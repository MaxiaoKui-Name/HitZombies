using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonBones;
using Hitzb;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

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

    [Header("新手引导")]
    public GameObject Panel_F;
    public GameObject SkillFinger_F1;
    public GameObject SkillFinger_F2;
    public Image GuidArrowL;
    public Image GuidArrowR;
    private Image GuidCircle;
    //private Image Guidfinger;
    private Image GuidText;
    public GameObject HighLight;
    public GameObject HighLightPlayer;
    public GameObject CoinNoteImg2_F;
    public TextMeshProUGUI ContinueTextOne_F;
    public TextMeshProUGUI ContinueTextTwo_F;
    public GameObject CoinNote_F;
    public GameObject KillNote_F;
    public GameObject BoxNote_F;
    public GameObject SkillNote_F;
    public Image SkillFinger_F;

    [Header("换枪")]
    public Button RedBoxBtn_F;
    public Image ChooseFinger_F;
    public Button ChooseMaxBtn_F;
    public GameObject ChooseGunNote_F;
    public GameObject ChooseGun_F;


    [Header("Spawn Properties")]
    public float bombDropInterval;  // 炸弹投掷间隔时间
    public float planeSpeed = 2f;   // 飞机移动速度

    public GameObject player;
    // 引用Canvas的RectTransform
    public RectTransform canvasRectTransform;

    public Vector3 coinspattern_F;
    // 添加用于检测滑动的变量
    private Vector3 lastMousePosition;
    private float dragThreshold = 0.01f; // 滑动阈值，可以根据需要调整
    private bool isDragging = false;
    void Start()
    {
        GetAllChild(transform);
        // 找到子对象中的按钮和文本框
        //新手引导
        canvasRectTransform = transform.parent.GetComponent<RectTransform>();
        GuidArrowL = childDic["GuidArrowL_F"].GetComponent<Image>();
        GuidArrowR = childDic["GuidArrowR_F"].GetComponent<Image>();
        GuidCircle = childDic["GuidCircle_F"].GetComponent<Image>();
        HighLight = childDic["BalanceHigh_F"].gameObject;
        HighLightPlayer = childDic["Playerbox_F"].gameObject;
        CoinNoteImg2_F = childDic["CoinNoteImg2_F"].gameObject; 
        ContinueTextOne_F = childDic["ContinueTextOne_F"].GetComponent<TextMeshProUGUI>();
        ContinueTextTwo_F = childDic["ContinueTextTwo_F"].GetComponent<TextMeshProUGUI>();
        CoinNote_F = childDic["CoinNote_F"].gameObject;
        CoinNote_F.SetActive(false);
        ContinueTextTwo_F.gameObject.SetActive(false);
        ContinueTextOne_F.gameObject.SetActive(false);
        KillNote_F = childDic["KillNote_F"].gameObject;
        KillNote_F.SetActive(false);
        coinspattern_F = childDic["coinspattern_F"].GetComponent<RectTransform>().anchoredPosition;

        ChooseFinger_F = childDic["Choosefinger_F"].GetComponent<Image>();
        RedBoxBtn_F = childDic["RedBoxBtn_F"].GetComponent<Button>();
        ChooseGunNote_F = childDic["ChooseGunNote_F"].gameObject;
        ChooseMaxBtn_F = childDic["ChooseMaxBtn_F"].GetComponent<Button>();
        ChooseGun_F = childDic["ChooseGun_F"].gameObject;
        BoxNote_F = childDic["BoxNote_F"].gameObject;
        SkillNote_F = childDic["SkillNote_F"].gameObject;
        SkillFinger_F = childDic["SkillFinger_F"].GetComponent<Image>();
        Panel_F = childDic["Panel_F"].gameObject;
        RedBoxBtn_F.gameObject.SetActive(false);
        ChooseGunNote_F.SetActive(false);
        ChooseMaxBtn_F.gameObject.SetActive(false);
        ChooseGun_F.gameObject.SetActive(false);
        BoxNote_F.gameObject.SetActive(false);
        SkillNote_F.gameObject.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);
        SkillFinger_F1 = childDic["SkillFinger_F1"].gameObject;
        SkillFinger_F2 = childDic["SkillFinger_F2"].gameObject;
        SkillFinger_F1.SetActive(false);
        SkillFinger_F2.SetActive(false);

        //Guidfinger = childDic["Guidfinger_F"].GetComponent<Image>();
        GuidText = childDic["GuidText_F"].GetComponent<Image>();
        pauseButton = childDic["pause_Btn_F"].GetComponent<Button>();
        coinText = childDic["valueText_F"].GetComponent<Text>();
        buffFrozenText = childDic["Frozentimes_F"].GetComponent<TextMeshProUGUI>();
        buffFrozenBtn = childDic["FrozenBtn_F"].GetComponent<Button>();
        buffBlastText = childDic["Blastimes_F"].GetComponent<TextMeshProUGUI>();
        buffBlastBtn = childDic["BlastBtn_F"].GetComponent<Button>();
        buffBlastBack = buffBlastBtn.GetComponent<Image>();
        buffForzenBack = buffFrozenBtn.GetComponent<Image>();
        buffBlastBack.sprite = buffBlastImages[0];
        buffForzenBack.sprite = buffForzenImages[0];
        buffBlastText.text = "0";
        buffFrozenText.text = "0";
        EventDispatcher.instance.Regist(EventNameDef.UPDATECOIN, (v) => UpdateCoinText());
        // 添加暂停按钮的点击事件监听器
        pauseButton.onClick.AddListener(TogglePause);
        buffFrozenBtn.onClick.AddListener(ToggleFrozen);
        buffBlastBtn.onClick.AddListener(ToggleBlast);
        //if (GameFlowManager.Instance.currentLevelIndex == 0)
        //{
        //    buffFrozenBtn.interactable = false;
        //    buffBlastBtn.interactable = false;
        //}
        HighLight.SetActive(false);
        HighLightPlayer.SetActive(false);
        CoinNoteImg2_F.SetActive(false);
        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            pauseButton.transform.parent.gameObject.SetActive(false);
            Panel_F.transform.gameObject.SetActive(true);
        }
        if (GameFlowManager.Instance.currentLevelIndex != 0)
        {
            Panel_F.transform.gameObject.SetActive(false);
            GuidArrowL.transform.parent.gameObject.SetActive(false);
        }
        // 添加RedBoxBtn_F的事件监听
        RedBoxBtn_F.gameObject.AddComponent<RedBoxButtonHandler>().Initialize(this);
        if (PlayerPrefs.HasKey("PlayerAccountID"))
        {
            string accountID = PlayerPrefs.GetString("PlayerAccountID");
            PlayInforManager.Instance.playInfor.BalstBuffCount = PlayerPrefs.GetInt($"{accountID}PlayerBalstBuffCount");
            PlayInforManager.Instance.playInfor.FrozenBuffCount = PlayerPrefs.GetInt($"{accountID}PlayerFrozenBuffCount");
            UpdateBuffText(PlayerPrefs.GetInt($"{accountID}PlayerFrozenBuffCount"), PlayerPrefs.GetInt($"{accountID}PlayerBalstBuffCount"));
        }
        UpdateCoinText();
    }
    public bool isGuidAnimationPlaying = false; // 标志是否正在播放引导动画
    private bool hasGuidAnimationPlayed = false; // 标志引导动画是否已播放过
    void Update()
    {
        
        // 新手引导逻辑
        if (GameManage.Instance.gameState == GameState.Guid)
        {
            if (!isGuidAnimationPlaying && !hasGuidAnimationPlayed)
            {
                StartCoroutine(RunGuidCircleAnimation());
            }
            else if (!isGuidAnimationPlaying && hasGuidAnimationPlayed)
            {
                HandleNewbieGuide();
            }
        }
    }
    private void OnDestroy()
    {
        EventDispatcher.instance.UnRegist(EventNameDef.UPDATECOIN, (v) => UpdateCoinText());
        // 确保销毁时不会访问已销毁的引用
        coinText = null;
    }

    void UpdateCoinText()
    {
        if (coinText == null)
        {
            Debug.LogWarning("coinText is null or destroyed");
            return; // 避免继续访问已销毁的组件
        }
        // 实时更新显示的金币数量
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum:N0}";
    }
    /// <summary>
    /// 更新金币文本并添加滚动动画
    /// </summary>
    public void UpdateCoinTextWithDOTween(int AddCoin)
    {
        // 更新UI显示滚动效果
        int currentCoin = (int)(PlayInforManager.Instance.playInfor.coinNum); // 计算增加的金币数
        float duration = 1f; // 动画持续时间
        int targetCoin = (int)PlayInforManager.Instance.playInfor.coinNum + AddCoin;

        DOTween.To(() => currentCoin, x =>
        {
            currentCoin = x;
            PlayInforManager.Instance.playInfor.coinNum = currentCoin;
            coinText.text = $"{currentCoin}";
        }, targetCoin, duration)
        .SetEase(Ease.Linear)
        .SetUpdate(true); // 使用不缩放时间，确保在暂停时动画仍然播放); // Ensure it continues to update even when time is paused
    }


    private IEnumerator RunGuidCircleAnimation()
    {
        isGuidAnimationPlaying = true; // 标记动画正在播放
        // 暂停游戏
        Time.timeScale = 0f;
        Debug.Log("Time.timeScale 设置为: " + Time.timeScale);
        // 获取 RectTransform
        RectTransform guidCircleRect = GuidCircle.GetComponent<RectTransform>();

        // 获取左边界和右边界的位置
        Vector2 leftLimit = GuidArrowL.rectTransform.anchoredPosition;
        Vector2 rightLimit = GuidArrowR.rectTransform.anchoredPosition;
        Vector2 initialPos = guidCircleRect.anchoredPosition;

        // 动画参数
        float moveDuration = 1f; // 每次移动的持续时间

        // 使用 DOTween 创建动画序列
        Sequence guidSequence = DOTween.Sequence();
        guidSequence.Append(guidCircleRect.DOAnchorPos(new Vector2(leftLimit.x, initialPos.y), moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(new Vector2(rightLimit.x, initialPos.y), moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialPos, moveDuration).SetEase(Ease.InOutSine))
                    .SetUpdate(true) // 使用不缩放时间，确保在暂停时动画仍然播放
                    .OnComplete(() =>
                    {
                        Debug.Log("引导动画播放完成");
                        isGuidAnimationPlaying = false; // 动画播放完成，允许检测鼠标输入
                        hasGuidAnimationPlayed = true;   // 标记引导动画已播放
                    });
        guidSequence.Play();
        // 等待动画完成
        yield return new WaitUntil(() => !isGuidAnimationPlaying);
        // 继续后续逻辑，可以在这里恢复游戏时间（如果需要）
        // Time.timeScale = 0f; // 保持暂停状态
        Debug.Log("引导动画结束，开始检测鼠标输入");
    }
    private Vector3 targetPosition;

    private void HandleNewbieGuide()
    {
        // 如果引导动画正在播放，则不处理鼠标输入
        if (isGuidAnimationPlaying)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            // 记录鼠标按下时的位置
            lastMousePosition = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            float distance = Vector3.Distance(currentMousePosition, lastMousePosition);

            if (!isDragging && distance > dragThreshold)
            {
                // 检测到滑动行为，立即切换游戏状态
                GuidCircle.transform.parent.gameObject.SetActive(false);
                GameManage.Instance.SwitchState(GameState.Running);
                isDragging = true; // 确保只切换一次
            }

            if (!isDragging)
            {
                // 按住鼠标左键时，跟随鼠标移动
                Vector3 mousePos = Input.mousePosition;
                Vector2 localPoint;

                // 将屏幕点转换为Canvas的本地点
                bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePos, null, out localPoint);
                if (isInside)
                {
                    // 限制GuidCircle在箭头之间移动
                    Vector3 leftLimit = GuidArrowL.rectTransform.anchoredPosition;
                    Vector3 rightLimit = GuidArrowR.rectTransform.anchoredPosition;

                    float clampedX = Mathf.Clamp(localPoint.x, leftLimit.x, rightLimit.x);
                    // 这里假设y轴位置保持不变
                    GuidCircle.rectTransform.anchoredPosition = new Vector2(clampedX, GuidCircle.rectTransform.anchoredPosition.y);

                    // 移动玩家
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
                    if (player == null)
                        player = GameObject.Find("Player");
                    worldPos.z = 0;
                    worldPos.y = player.transform.position.y;
                    worldPos.x = Mathf.Clamp(worldPos.x, -1.5f, 1.5f);
                    player.transform.position = worldPos;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
            {
                // 如果没有滑动，只在松开时切换状态
                GuidCircle.transform.parent.gameObject.SetActive(false);
                GameManage.Instance.SwitchState(GameState.Running);
            }
            // 重置拖动状态
            isDragging = false;
        }
    }


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

    void UapdateBuffBack(int FrozenBuffCount, int BalstBuffCount)
    {
        if(FrozenBuffCount > 0)
            buffForzenBack.sprite = buffForzenImages[1];
        else
            buffForzenBack.sprite = buffForzenImages[0];
        if (BalstBuffCount > 0)
            buffBlastBack.sprite = buffBlastImages[1];
        else
            buffBlastBack.sprite = buffBlastImages[0];
    }

    // 切换暂停和继续游戏的状态
    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f; // 暂停游戏
        }
        else
        {
            Time.timeScale = 1f; // 继续游戏
        }
    }
    public void UpdateBuffText(int FrozenBuffCount,int BalstBuffCount)
    {
        buffFrozenText.text = $"{FrozenBuffCount}";
        buffBlastText.text = $"{BalstBuffCount}";
        UapdateBuffBack(FrozenBuffCount, BalstBuffCount);
    }
    void ToggleBlast()
    {
        //执行全屏爆炸功能
        if(PlayInforManager.Instance.playInfor.BalstBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.BalstBuffCount--;
            if(GameFlowManager.Instance.currentLevelIndex == 0)
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
            MoveFrozenPlaneAndDropBombs().Forget();
        }
    }
    #region 全屏爆炸逻辑
    public async UniTask SpawnPlane()
    {
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // 生成飞机在屏幕底部
        Debug.Log("Plane spawned!");
        await MovePlaneAndDropBombs(plane);
        if (plane != null)
        {
            Destroy(plane);
        }
    }
    // 飞机移动并投放炸弹的异步方法
    private async UniTask MovePlaneAndDropBombs(GameObject plane)
    {
        float dropTime = 0f;
        bool isThrow = false;
        while (plane != null && plane.activeSelf && plane.transform.position.y < 6f)
        {
            // Move the plane upwards
            plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
            //dropTime += Time.deltaTime;

            //// TTOD1投放炸弹逻辑
            //if (dropTime >= bombDropInterval)
            //{
            //    dropTime = 0f;
            //    Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
            //    DropBomb(bombPosition).Forget();
            //}
            //// Yield control to allow other operations
            //await UniTask.Yield();
            // TTOD1投放炸弹逻辑
            if (plane.transform.position.y > 0 && !isThrow)
            {
                isThrow = true;
                Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
                DropBomb(bombPosition).Forget();
            }
            // Yield control to allow other operations
            await UniTask.Yield();
        }
    }
        // 投放炸弹（异步）
        private async UniTask DropBomb(Vector3 planePosition)
        {
            GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
            await BombExplosion(bomb, ConfigManager.Instance.Tables.TableTransmitConfig.Get(2).DamageScope);
        }

        // 炸弹爆炸动画，并消灭敌人（异步）
        private async UniTask BombExplosion(GameObject bomb,float width)
        {
            UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
            if (bombArmature != null)
            {
                bombArmature.animation.Play("fly", 1); // 播放一次飞行动画
            }
            // 获取炸弹位置
            Vector3 bombPos = bomb.transform.position;

            // 定义矩形范围的左上角和右下角
            Vector3 topLeft = new Vector3(bombPos.x - width / 2, bombPos.y + width / 2, bombPos.z);
            Vector3 bottomRight = new Vector3(bombPos.x + width / 2, bombPos.y - width / 2, bombPos.z);
            float DamageNum = ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TableBoxcontent.Get(7).Fires[0]).AtkRate * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
           // 找到并炸毁矩形范围内的敌人
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Vector3 enemyPos = enemy.transform.position;

                if (IsWithinRectangle(enemyPos, topLeft, bottomRight) && enemy.activeSelf)
                {
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();
                    if (enemyController != null && !enemyController.isDead && enemyController.isVise)
                    {
                        enemyController.Enemycoins2 = 1;
                    //TTOD1攻击系数乘基础攻击系数 代改
                        enemyController.TakeDamage(DamageNum, enemy); // 对敌人造成极高的伤害
                    }
                }
            }
            // 找到在矩形范围内的宝箱爆炸消失
            GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
            foreach (GameObject chest in chests)
            {
                Vector3 chestPos = chest.transform.position;
                if (IsWithinRectangle(chestPos, topLeft, bottomRight))
                {
                    ChestController chestController = chest.GetComponent<ChestController>();
                    if (chestController != null && chestController.isVise)
                    {
                        chestController.TakeDamage(DamageNum, chest); // 冻结宝箱
                    }
                }
            }
            await UniTask.Delay(1000);
            Destroy(bomb);
        }
    #endregion
    #region 全屏冰冻逻辑

    //public async UniTask SpawnFrozenPlane()
    //{
    //    GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // 生成冰冻飞机在屏幕底部
    //    Debug.Log("Frozen Plane spawned!");
    //    await MoveFrozenPlaneAndDropBombs(plane);
    //    if (plane != null)
    //    {
    //        Destroy(plane);
    //    }
    //}

    // 飞机移动并投放冰冻炸弹的异步方法
    private async UniTask MoveFrozenPlaneAndDropBombs()
    {
        Vector3 bombPosition = PreController.Instance.RandomPosition(new Vector3(0f, 3f, 0f));
        DropFrozenBomb(bombPosition).Forget();
        //while (plane != null && plane.activeSelf && plane.transform.position.y < 3f) // 假设 3 是场景中间的 Y 位置
        //{
        //    plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
        //    await UniTask.Yield();
        //}

        //// 在飞机到达场景中间时投放冰冻炸弹
        //if (plane != null)
        //{
        //    Vector3 bombPosition = PreController.Instance.RandomPosition(new Vector3(0f,3f,0f));
        //    DropFrozenBomb(bombPosition).Forget();
        //}
    }

    // 投放冰冻炸弹（异步）
    private async UniTask DropFrozenBomb(Vector3 planePosition)
    {
        GameObject frozenBomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
        await FrozenBombEffect(frozenBomb);
    }

    // 冰冻炸弹效果
    private async UniTask FrozenBombEffect(GameObject bomb)
    {
        // 播放冰冻效果动画
        UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
        if (bombArmature != null)
        {
            bombArmature.animation.Play("fly", 1); // 播放冰冻动画
        }
        // 暂停所有敌人和宝箱
        FreezeAllEnemiesAndChests(bomb.transform.position,ConfigManager.Instance.Tables.TableTransmitConfig.Get(1).DamageScope);
        // 等待 5 秒
        await UniTask.Delay(5000);
        // 解除冰冻效果
        UnfreezeAllEnemiesAndChests(bomb.transform.position, ConfigManager.Instance.Tables.TableTransmitConfig.Get(1).DamageScope);
        Destroy(bomb);
    }

    // 冻结所有敌人和宝箱
    //private void FreezeAllEnemiesAndChests(Vector3 FreezePos)
    //{
    //    GameManage.Instance.isFrozen = true;
    //    PreController.Instance.isFrozen = true;
    //    Debug.Log("开始冰冻=========================！!！");
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //    foreach (GameObject enemy in enemies)
    //    {
    //        EnemyController enemyController = enemy.GetComponent<EnemyController>();
    //        if (enemyController != null && !enemyController.isDead)
    //        {
    //            enemyController.isFrozen = true; // 添加一个 isFrozen 属性来控制敌人状态
    //        }
    //    }

    //    GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
    //    foreach (GameObject chest in chests)
    //    {
    //        // 暂停宝箱逻辑
    //        // 例如可以设置一个 isFrozen 属性
    //        ChestController chestController = chest.GetComponent<ChestController>();
    //        if (chestController != null && chestController.isVise)
    //        {
    //            chestController.isFrozen = true; // 添加一个 isFrozen 属性来控制敌人状态
    //        }
    //    }
    //}
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

    #region 新增方法和逻辑

    // 新增方法：启动 ChooseFinger_F 的动画
    public void StartChooseFingerAnimation()
    {
        Debug.Log("StartChooseFingerAnimation called");
        ChooseFinger_F.gameObject.SetActive(true);
        StartCoroutine(FingerMoveLoop());
    }

    // 新增方法：停止 ChooseFinger_F 的动画
    public void StopChooseFingerAnimation()
    {
        Debug.Log("StopChooseFingerAnimation called");
        StopAllCoroutines();
        ChooseFinger_F.GetComponent<RectTransform>().anchoredPosition = Vector3.zero; // 重置位置
        ChooseFinger_F.gameObject.SetActive(false);
    }

    // 协程：ChooseFinger_F 循环移动（使用非缩放时间）
    private IEnumerator FingerMoveLoop()
    {
        Debug.Log("FingerMoveLoop started");
        RectTransform fingerRect = ChooseFinger_F.GetComponent<RectTransform>();
        Vector2 originalPos = fingerRect.anchoredPosition;
        Vector2 targetPos1 = originalPos + new Vector2(0, 40f);
        Vector2 targetPos2 = originalPos - new Vector2(0, 40f);
        while (true)
        {
            // 向上移动60
            yield return StartCoroutine(MoveFinger(fingerRect, originalPos, targetPos1, 0.5f));
            yield return StartCoroutine(MoveFinger(fingerRect, targetPos1, originalPos, 0.5f));
            // 停止1秒（使用 WaitForSecondsRealtime）
            yield return new WaitForSecondsRealtime(1f);
            // 向下移动回原位
            yield return StartCoroutine(MoveFinger(fingerRect, originalPos, targetPos2, 0.5f));
            yield return StartCoroutine(MoveFinger(fingerRect, targetPos2, originalPos, 0.5f));
            // 停止1秒（使用 WaitForSecondsRealtime）
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    // 协程：移动 ChooseFinger_F（使用非缩放时间）
    private IEnumerator MoveFinger(RectTransform finger, Vector2 from, Vector2 to, float duration)
    {
        Debug.Log($"MoveFinger from {from} to {to} over {duration} seconds");
        float elapsed = 0f;
        while (elapsed < duration)
        {
            finger.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
            elapsed += Time.unscaledDeltaTime; // 使用非缩放时间
            yield return null;
        }
        finger.anchoredPosition = to;
        Debug.Log($"MoveFinger to {to} completed");
    }

    // 新增方法：等待 RedBoxBtn_F 的长按
    public async UniTask WaitForRedBoxLongPress()
    {
        var tcs = new UniTaskCompletionSource();

        // 获取 RedBoxButtonHandler 组件并设置回调
        RedBoxButtonHandler handler = RedBoxBtn_F.GetComponent<RedBoxButtonHandler>();
        if (handler != null)
        {
            void OnLongPressHandler()
            {
                tcs.TrySetResult();
            }

            handler.OnLongPress += OnLongPressHandler;
            await tcs.Task;
            handler.OnLongPress -= OnLongPressHandler;
        }
    }
    #endregion

    #region//宝箱技能
    public async void ShowSkillGuide(int indexChest)
    {
        StartCoroutine(ShowSkillGuideCoroutine(indexChest));
    }
    private Vector2 InitialPos = new Vector2(0, 0);
    private IEnumerator ShowSkillGuideCoroutine(int indexChest)
    {
        Debug.Log("ShowSkillGuide() 方法被调用");

        if (SkillNote_F == null || SkillFinger_F == null || buffBlastBtn == null)
        {
            Debug.LogError("SkillNote_F、SkillFinger_F 或 buffBlastBtn 未正确赋值。");
            yield break;
        }

        // 暂停游戏
        Time.timeScale = 0f;
        Debug.Log("Time.timeScale 设置为: " + Time.timeScale);
        // 记录起始位置
        // 激活 SkillNote_F 和 SkillFinger_F
        SkillNote_F.SetActive(true);
        Vector2 targetPos = new Vector2(22, -104);
        if (indexChest == 1)
        {
            SkillFinger_F1.SetActive(true);
            targetPos = new Vector2(42f, -118f);
            Vector2 startPos = SkillFinger_F.GetComponent<RectTransform>().anchoredPosition;
            InitialPos = startPos;
        }
        if (indexChest == 2)
        {
            SkillFinger_F2.SetActive(true);
            targetPos = new Vector2(42f, -241);
        }
        SkillFinger_F.gameObject.SetActive(true);
        Debug.Log("SkillFinger_F 激活状态: " + SkillFinger_F.gameObject.activeSelf);


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
        Vector3 originalScale = SkillFinger_F.GetComponent<RectTransform>().localScale;
        // 开始重复点击动画
        // 使用 DOTween 创建一个无限循环的点击动画，并设置为使用不缩放时间
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(SkillFinger_F.transform.DOScale(originalScale * 1.2f, 0.2f).SetEase(Ease.InOutSine))
                    .Append(SkillFinger_F.transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutSine))
                    .SetLoops(-1) // 无限循环
                    .SetUpdate(true); // 使用不缩放时间
        clickSequence.Play();
        Debug.Log("开始重复点击动画");

        // 等待 buffBlastBtn 被点击
        bool buttonClicked = false;
        void OnBuffBlastBtnClicked()
        {
            buttonClicked = true;
            buffBlastBtn.onClick.RemoveListener(OnBuffBlastBtnClicked);
        }

        buffBlastBtn.onClick.AddListener(OnBuffBlastBtnClicked);
        Debug.Log("等待 buffBlastBtn 被点击");
        while (!buttonClicked)
        {
            yield return null;
        }

        // 停止重复点击动画
        clickSequence.Kill();
        Debug.Log("停止重复点击动画");

        // 继续后续逻辑，例如隐藏指引元素，恢复游戏等
        SkillNote_F.SetActive(false);
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
}
