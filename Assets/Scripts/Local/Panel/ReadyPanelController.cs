using Cysharp.Threading.Tasks;
using DragonBones;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class ReadyPanelController : UIBase
{
    public Button StartGameBtn;
    public Button OpenURLBtn;
    public UIManager uIManager;
    public Button CheckBtn;
    public Button TurntableBtn;
    public Button MailBtn;
    public TextMeshProUGUI totalCoinsText;
    public GameObject TotalCoinImg_F;
    private GameObject CheckUIPanel; // 签到面板
    private GameObject TurnTablePanel; // 转盘面板
    private GameObject MailPanel; // 转盘面板
                                  // 用于金币动画的引用
    public UnityEngine.Transform coinStartTransform; // 通过 Inspector 指定或动态获取
    private Coroutine coinAnimationCoroutine;
    public Image RedNoteImg;
    public Image TurntableRedNoteImg;
    public Image redIndicator; // 红色提示图片
    public TextMeshProUGUI DataBarLevelText;

    //玩家信息显示
    public TextMeshProUGUI DataBarIDText_F;

    //新增
    //段位
    public Image DanImg_F;
    private Dictionary<string, Sprite> danSpriteDict;
    public TextMeshProUGUI DanText;
    //星级
    private List<GameObject> starBackList = new List<GameObject>();
    private List<GameObject> starLightList = new List<GameObject>();
    private GameObject kingStart_F;

    //宝箱
    public ScrollRect chestScrollRect;
    public Transform chestContent;
    public GameObject chestuiPrefab;
    public GameObject rewardChestuiPanelPrefab;
    public GameObject Point;
    public GameObject ChestText_F;

    private List<ChestData> chestDataList = new List<ChestData>();
    private string previousDan;
    public Sprite[]ChestuiImg;

    [Serializable]
    public class ChestData
    {
        public string danLevel;
        public bool isClaimed;
        [JsonIgnore] // 忽略序列化
        public GameObject chestObject;
    }

    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        // 加载宝箱数据
        chestDataList = AccountManager.Instance.LoadChestData();
        GetAllChild(transform);
        RedNoteImg = childDic["RedNote_F"].GetComponent<Image>();
        redIndicator = childDic["MailBtnRedNote_F"].GetComponent<Image>();
        TurntableRedNoteImg = childDic["TurntableRedNote_F"].GetComponent<Image>();
        StartGameBtn = childDic["ReadystartGame_F"].GetComponent<Button>();
        TurntableBtn = childDic["TurntableBtn_F "].GetComponent<Button>();
        OpenURLBtn = childDic["OtherURL_F"].GetComponent<Button>();
        CheckBtn = childDic["CheckBtn_F"].GetComponent<Button>();
        totalCoinsText = childDic["totalCoinsText_F"].GetComponent<TextMeshProUGUI>();
        TotalCoinImg_F = childDic["TotalCoinImg_F"].gameObject;
        MailBtn = childDic["MailBtn_F"].GetComponent<Button>();
        DataBarLevelText = childDic["DataBarLevelImg_F"].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        DataBarLevelText.text = $"Current Stage: ~{GameFlowManager.Instance.currentLevelIndex}";
        totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();

        DataBarIDText_F = childDic["DataBarIDText_F"].GetComponent<TextMeshProUGUI>();
        DataBarIDText_F.text  = PlayInforManager.Instance.playInfor.accountID.ToString();

        //新增
        DanImg_F = childDic["DanImg_F"].GetComponent<Image>();
        DanText = childDic["DanTextImg_F "].GetChild(0).GetComponent<TextMeshProUGUI>();
        InitializeDanSprites();
        UpdateDanImage();
        UpdateDanText();


        //获取星级
        for (int i = 1; i <= 5; i++)
        {
            GameObject star = childDic[$"Start{i}_F"].gameObject;
            GameObject starBack = star.transform.Find($"Start{i}Back_F").gameObject;
            GameObject starLight = star.transform.Find($"Start{i}Light_F").gameObject;
            starBackList.Add(starBack);
            starLightList.Add(starLight);
        }
        kingStart_F = childDic["kingStart_F"].gameObject;
        UpdateStarDisplay();

        //宝箱\
        chestuiPrefab = Resources.Load<GameObject>("Prefabs/UIPannel/Chestui1");
        ChestText_F = childDic["ChestText_F"].gameObject;
        ChestText_F.SetActive(false);
        //TTOD1待更改
        rewardChestuiPanelPrefab = Resources.Load<GameObject>("Prefabs/UIPannel/RewardPanel");
        Point.transform.parent.gameObject.SetActive(false);

        // 判断是否每日是否首次登录
        UpdateRedNote();
        OpenURLBtn.gameObject.SetActive(ConfigManager.Instance.Tables.TableJumpConfig.Get(1).IsOpen);
        UpdateRedIndicator();
        // 订阅消息更新事件
        MessageManager.Instance.OnMessagesUpdated += UpdateRedIndicator;

        if (ConfigManager.Instance.Tables.TableJumpConfig.Get(1).IsOpen)
        {

            OpenURLBtn.onClick.AddListener(OnOpenURLButtonClicked); // 添加此行
        }
        // 初始化金币显示
        UpdateTotalCoinsUI(AccountManager.Instance.GetTotalCoins());
        if (GameFlowManager.Instance.currentLevelIndex > 1)
        {
            previousDan = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex - 1).Dan.Substring(0, 3);
            CheckDanLevelUp();
            InitializeChestUI();
        }
        //弹跳
      
        // 获取 ClickAMature 对象及其动画组件
        GetClickAnim(transform);
        //StartGameBtn.onClick.AddListener(OnStartGameButtonClicked);
        //CheckBtn.onClick.AddListener(OnCheckonClicked);
        //TurntableBtn.onClick.AddListener(OnWheelonClicked);
        //MailBtn.onClick.AddListener(OnMailClicked);
        StartGameBtn.onClick.AddListener(() => StartCoroutine(OnStartGameBtnClicked()));
        CheckBtn.onClick.AddListener(() => StartCoroutine(OnCheckBtnClicked()));
        TurntableBtn.onClick.AddListener(() => StartCoroutine(OnTurntableBtntnClicked()));
        MailBtn.onClick.AddListener(() => StartCoroutine(OnMailBtnClicked()));
    }

/// <summary>
/// 处理签到按钮点击事件的协程
/// </summary>
private IEnumerator OnStartGameBtnClicked()
{
    // 播放点击动画
    yield return StartCoroutine(HandleButtonClickAnimation(transform));

    // 执行按钮弹跳动画并调用后续逻辑
    yield return StartCoroutine(ButtonBounceAnimation(StartGameBtn.GetComponent<RectTransform>(), OnStartGameButtonClicked));
}

/// <summary>
/// 处理关闭按钮点击事件的协程
/// </summary>
private IEnumerator OnCheckBtnClicked()
{
    // 播放点击动画
    yield return StartCoroutine(HandleButtonClickAnimation(transform));

    // 执行按钮弹跳动画并调用后续逻辑
    yield return StartCoroutine(ButtonBounceAnimation(CheckBtn.GetComponent<RectTransform>(), OnCheckonClicked));
}
private IEnumerator OnTurntableBtntnClicked()
{
    // 播放点击动画
    yield return StartCoroutine(HandleButtonClickAnimation(transform));

    // 执行按钮弹跳动画并调用后续逻辑
    yield return StartCoroutine(ButtonBounceAnimation(TurntableBtn.GetComponent<RectTransform>(), OnWheelonClicked));
}
private IEnumerator OnMailBtnClicked()
{
    // 播放点击动画
    yield return StartCoroutine(HandleButtonClickAnimation(transform));

    // 执行按钮弹跳动画并调用后续逻辑
    yield return StartCoroutine(ButtonBounceAnimation(MailBtn.GetComponent<RectTransform>(), OnMailClicked));
}

#region//新增以前代码
public void UpdateRedIndicator()
    {
        int unreadCount = MessageManager.Instance.GetUnreadCount();
        redIndicator.gameObject.SetActive(unreadCount > 0);
    }
    // 添加打开URL的方法
    void OnOpenURLButtonClicked()
    {
        string url = ConfigManager.Instance.Tables.TableJumpConfig.Get(1).URL; // TTOD1默认URL，读表更改
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
            Debug.Log($"打开了URL: {url}");
        }
        else
        {
            Debug.LogWarning("URL未设置或为空！");
        }
    }
    public void UpdateRedNote()
    {
        // 如果玩家可以签到，则显示 RedNoteImg，否则隐藏
        RedNoteImg.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        //CheckBtn.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        TurntableRedNoteImg.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        //TurntableBtn.gameObject.SetActive(AccountManager.Instance.CanSignIn());
    }
    // Update is called once per frame
    void Update()
    {
    }

    // 按钮点击时调用的方法
    void OnStartGameButtonClicked()
    {
        if (LevelManager.Instance.levelData != null)
        {
            StartGameBtn.onClick.RemoveListener(OnStartGameButtonClicked);
            CheckBtn.onClick.RemoveListener(OnCheckonClicked);
            TurntableBtn.onClick.RemoveListener(OnWheelonClicked);
            MailBtn.onClick.RemoveListener(OnMailClicked);
            if (MessageManager.Instance != null)
            {
                MessageManager.Instance.OnMessagesUpdated -= UpdateRedIndicator;
            }
            // 解绑宝箱按钮的事件监听
            foreach (var chestData in chestDataList)
            {
                if (chestData.chestObject != null)
                {
                    Button chestButton = chestData.chestObject.transform.GetChild(0).GetComponent<Button>();
                    chestButton.onClick.RemoveAllListeners();
                }
            }
            if (ConfigManager.Instance.Tables.TableJumpConfig.Get(1).IsOpen)
            {
                OpenURLBtn.onClick.RemoveListener(OnOpenURLButtonClicked); // 添加此行
            }
            StartGameBtn.gameObject.SetActive(false);
            PlayInforManager.Instance.playInfor.attackSpFac = 0;
            if (GameFlowManager.Instance.currentLevelIndex != 0 && SceneManager.GetActiveScene().name != "First")
            {
                LevelManager.Instance.LoadScene("First", GameFlowManager.Instance.currentLevelIndex);

            }
            else
            {
                GameFlowManager.Instance.NextLevel();
            }
            InfiniteScroll.Instance.baseScrollSpeed = 0.5f;//ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
            InfiniteScroll.Instance.baseGrowthRate = InfiniteScroll.Instance.baseScrollSpeed / 40;
            uIManager.ChangeState(GameState.Running);
            //LevelManager.Instance.LoadScene("First", 0);
        }
    }
    void OnCheckonClicked()
    {
        if (LevelManager.Instance.levelData != null)
        {
            CheckUIPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/CheckUIPanel"));
            CheckUIPanel.transform.SetParent(transform.parent, false);
            CheckUIPanel.transform.localPosition = Vector3.zero;
        }
    }
    void OnWheelonClicked()
    {
        if (LevelManager.Instance.levelData != null)
        {
            TurnTablePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/TurnTablePanel"));
            TurnTablePanel.transform.SetParent(transform.parent, false);
            TurnTablePanel.transform.localPosition = Vector3.zero;
        }
    }

    void OnMailClicked()
    {
        if (LevelManager.Instance.levelData != null)
        {
            MailPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/MailPanel"));
            MailPanel.transform.SetParent(transform.parent, false);
            MailPanel.transform.localPosition = Vector3.zero;
        }
    }
    /// <summary>
    /// 使用动画效果更新总金币的 UI
    /// </summary>
    public void UpdateTotalCoinsUI(int reward)
    {
        if (coinAnimationCoroutine != null)
        {
            StopCoroutine(coinAnimationCoroutine);
        }
        int start = (int)PlayInforManager.Instance.playInfor.coinNum;
        int end = (int)(PlayInforManager.Instance.playInfor.coinNum + reward);
        coinAnimationCoroutine = StartCoroutine(RollingNumber(totalCoinsText, start, end, 1f));
    }

    /// <summary>
    /// 数字滚动动画的协程
    /// </summary>
    private IEnumerator RollingNumber(TextMeshProUGUI textMesh, int start, int end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int current = Mathf.RoundToInt(Mathf.Lerp(start, end, t));
            textMesh.text = current.ToString();
            PlayInforManager.Instance.playInfor.coinNum = current;
            yield return null;
        }
        textMesh.text = end.ToString();
    }
    #endregion

    #region//新增代码
    //显示段位
    void InitializeDanSprites()
    {
        danSpriteDict = new Dictionary<string, Sprite>();
        for (int i = 1; i <= 7; i++)
        {
            string danKey = $"Dan{i}";
            Sprite danSprite = Resources.Load<Sprite>($"DanSprites/{danKey}");
            if (danSprite != null)
            {
                danSpriteDict.Add(danKey, danSprite);
            }
        }
    }

    void UpdateDanImage()
    {
        string currentDan = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;
        if (danSpriteDict.ContainsKey(GetduanName(currentDan.Substring(0, 3))))
        {
            DanImg_F.sprite = danSpriteDict[GetduanName(currentDan.Substring(0, 3))];
        }
        else
        {
            Debug.LogWarning($"未找到段位对应的图片：{currentDan}");
        }
    }

    private string GetduanName(string duan)
    {
        switch (duan)
        {
            case "倔强青铜":
                return "Dan1";
            case "秩序白银":
                return "Dan2";
            case "荣耀黄金":
                return "Dan3";
            case "尊贵铂金":
                return "Dan4";
            case "永恒钻石":
                return "Dan5";
            case "至尊星耀":
                return "Dan6";
            case "最强王者":
                return "Dan7";
                default:
                return "Dan1";
        }
    }

    void UpdateDanText()
    {
        if (GameFlowManager.Instance.currentLevelIndex == 0)
            return;
        string currentDan = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;
        string fialName = GetDanName(currentDan.Substring(2, 2));
        if (currentDan.Substring(2, 2) != "王者")
        {
            fialName = fialName +" "+ currentDan.Substring(4, 1);
        }
        DanText.text = fialName;
    }
    string GetDanName(string ChineseDan)
    {
        switch (ChineseDan)
         {
            case "青铜":
               return "Bronze";
            case "白银":
                return "Silver";
            case "黄金":
                return "Gold";
            case "铂金":
                return "Platinum";
            case "钻石":
                return "Diamond";
            case "星耀":
                return "Master";
            case "王者":
                return "Challenger";
            default:
                return "Bronze";
        }

    }

    //显示星级
    void UpdateStarDisplay()
    {
        int starRating = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).StarRating;
        string currentDan = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;
        if(currentDan.Substring(0,3) != "最强王者")
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < starRating)
                {
                    starBackList[i].SetActive(true);
                    starLightList[i].SetActive(true);
                }
                else
                {
                    starBackList[i].SetActive(true);
                    starLightList[i].SetActive(false);
                }
            }
            kingStart_F.SetActive(false);
        }
        else
        {
            kingStart_F.SetActive(true);
            childDic[$"kingStart2Text_F"].transform.GetComponent<Text>().text = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).StarRating.ToString();
            childDic[$"Start_F"].gameObject.SetActive(false);
        }
    }

    //升段宝箱
    void InitializeChestUI()
    {
        if (chestDataList.Count > 0)
        {
            Point.transform.parent.gameObject.SetActive(true);
        }
        foreach (var chestData in chestDataList)
        {
            // 实例化宝箱预制体
            GameObject chest = Instantiate(chestuiPrefab, chestContent);

            // 添加点击事件
            Button chestButton = chest.transform.GetChild(0).GetComponent<Button>();
            chestButton.onClick.AddListener(() => OnChestClicked(chestData.danLevel));
            chestButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;

            // 更新宝箱的UI状态（打开或未打开）
            Image chestImage = chestButton.GetComponent<Image>();
            if (chestData.isClaimed)
            {
                chestImage.sprite = ChestuiImg[1];
            }
            else
            {
                chestImage.sprite = ChestuiImg[0];
            }

            // 关联宝箱对象
            chestData.chestObject = chest;
        }

        // 更新宝箱的位置
        UpdateChestPositions();

        // 更新宝箱栏的显示
        UpdateChestScrollPosition();
    }

    void CheckDanLevelUp()
    {
        string currentDan = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;
        if (currentDan.Substring(0,3) != previousDan)
        {
            Point.transform.parent.gameObject.SetActive(true);
            // 玩家升段，创建新的宝箱
            CreateNewChest(currentDan);
            previousDan = currentDan;
        }
    }
    void CreateNewChest(string danLevel)
    {
        // 实例化宝箱预制体
        GameObject newChest = Instantiate(chestuiPrefab, chestContent);
        // 设置宝箱的位置
        UpdateChestPositions();

        // 添加点击事件
        Button chestButton = newChest.transform.GetChild(0).GetComponent<Button>();
        chestButton.onClick.AddListener(() => OnChestClicked(danLevel));
        chestButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;


        // 创建宝箱数据并添加到列表
        ChestData chestData = new ChestData
        {
            danLevel = danLevel,
            isClaimed = false,
            chestObject = newChest
        };
        chestDataList.Add(chestData);
        AccountManager.Instance.SaveChestData(chestDataList);
        // 更新宝箱栏的显示
        UpdateChestScrollPosition();
    }

    void OnChestClicked(string danLevel)
    {
        // 查找对应的宝箱数据
        ChestData chestData = chestDataList.Find(c => c.danLevel == danLevel);
        if (chestData != null && !chestData.isClaimed)
        {
            // 弹出奖励UI面板
            ShowRewardPanel(danLevel);
            // 标记为已领取
            chestData.isClaimed = true;
            //chestData.chestObject.GetComponent<Button>().onClick.RemoveListener(() => OnChestClicked(danLevel));
            // 更改宝箱图片为打开状态
            Image chestImage = chestData.chestObject.GetComponent<Image>();
            chestImage.sprite = ChestuiImg[1]; // 您需要提前加载打开状态的宝箱图片
                                               // 保存宝箱数据
            AccountManager.Instance.SaveChestData(chestDataList);
        }
    }
    void ShowRewardPanel(string danLevel)
    {
        // 实例化奖励UI面板，显示对应的奖励信息
        GameObject rewardPanel = Instantiate(rewardChestuiPanelPrefab, transform.parent);
        // 您可以在奖励面板中设置奖励内容和领取逻辑
    }

    void UpdateChestPositions()
    {
        int visibleChestCount = 3; // 可视宝箱数量
        float chestSpacing = 149.98f; // 宝箱之间的间距，可以根据需要调整

        if (chestDataList.Count <= visibleChestCount)
        {
            // 宝箱数量少于或等于可视宝箱数量
            // 计算起始位置，使宝箱居中显示
            float totalWidth = (chestDataList.Count - 1) * chestSpacing;
            float startX = totalWidth / 2;

            for (int i = 0; i < chestDataList.Count; i++)
            {
                RectTransform chestRect = chestDataList[i].chestObject.GetComponent<RectTransform>();
                // 设置锚点为中心
                chestRect.anchorMin = new Vector2(0.5f, 0.5f);
                chestRect.anchorMax = new Vector2(0.5f, 0.5f);
                chestRect.pivot = new Vector2(0.5f, 0.5f);
                chestRect.anchoredPosition = new Vector2(startX + i * chestSpacing, 0);
            }
            // 更新Content的宽度，确保不出现滚动
            RectTransform contentRect = chestContent.GetComponent<RectTransform>();
            contentRect.sizeDelta = chestScrollRect.GetComponent<RectTransform>().sizeDelta;
        }
        else
        {
            // 宝箱数量多于可视宝箱数量，需要滚动查看
            for (int i = 0; i < chestDataList.Count; i++)
            {
                RectTransform chestRect = chestDataList[i].chestObject.GetComponent<RectTransform>();
                // 设置锚点为左中
                chestRect.anchorMin = new Vector2(0f, 0.5f);
                chestRect.anchorMax = new Vector2(0f, 0.5f);
                chestRect.pivot = new Vector2(0f, 0.5f);
                chestRect.anchoredPosition = new Vector2(i * chestSpacing, 0);
            }
            // 更新Content的宽度，容纳所有宝箱
            RectTransform contentRect = chestContent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2((chestDataList.Count - 1) * chestSpacing + chestScrollRect.GetComponent<RectTransform>().sizeDelta.x, contentRect.sizeDelta.y);
        }
    }


    void UpdateChestScrollPosition()
    {
        if (chestDataList.Count <= 3)
        {
            // 宝箱数量少于或等于可视宝箱数量，不需要滚动，居中显示
            chestScrollRect.horizontalNormalizedPosition = 0.5f;
        }
        else
        {
            // 查找第一个未领取的宝箱
            int firstUnclaimedIndex = chestDataList.FindIndex(c => !c.isClaimed);
            if (firstUnclaimedIndex == -1)
            {
                firstUnclaimedIndex = chestDataList.Count - 1; // 如果全部已领取，显示最后一个宝箱
            }
            // 计算需要滚动的位置，使第一个未领取的宝箱居中
            float totalScrollableWidth = chestContent.GetComponent<RectTransform>().sizeDelta.x - chestScrollRect.GetComponent<RectTransform>().sizeDelta.x;
            float targetPosition = (firstUnclaimedIndex * 200f - chestScrollRect.GetComponent<RectTransform>().sizeDelta.x / 2 + 100f) / totalScrollableWidth;
            targetPosition = Mathf.Clamp01(targetPosition);
            chestScrollRect.horizontalNormalizedPosition = targetPosition;
        }
    }



    #endregion
}
