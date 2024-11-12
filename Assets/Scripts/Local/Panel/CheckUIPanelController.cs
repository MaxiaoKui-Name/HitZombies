using Cysharp.Threading.Tasks;
using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;

public class CheckUIPanelController : UIBase
{
    public Button signInButton;
    public Button CloseCheckBtn;
    // 引用五个签到天数的 UI 元素（第1天至第5天及以上）
    public List<GameObject> dayUIs; // 在 Inspector 中按顺序赋值：Day1, Day2, ..., Day5+
    public List<GameObject> dayPoss; // 在 Inspector 中按顺序赋值：Day1, Day2, ..., Day5+
    // 高亮预制体（例如一个边框）
    public GameObject highlightPrefab; // 在 Inspector 中指定

    public TextMeshProUGUI signInText;
    void Start()
    {
        GetAllChild(transform);
        signInButton = childDic["ReceiveBtn_F"].GetComponent<Button>();
        CloseCheckBtn = childDic["Return_F"].GetComponent<Button>();
        signInText = childDic["signInText_F"].GetComponent<TextMeshProUGUI>();
        signInButton.onClick.AddListener(OnSignInClicked);
        CloseCheckBtn.onClick.AddListener(OnCloseClicked);
        dayPoss = new List<GameObject>();
        dayUIs = new List<GameObject>();
        highlightPrefab = Resources.Load<GameObject>("Prefabs/highlightPrefab");
        foreach (Transform child in childDic["CheckDays_F"].transform)
        {
            dayUIs.Add(child.GetChild(1).gameObject);
        }
        foreach (Transform child in GameObject.Find("CheckDayPos").transform)
        {
            dayPoss.Add(child.gameObject);
        }
        UpdateUI();
    }

    /// <summary>
    /// 处理签到按钮点击事件
    /// </summary>
    private void OnSignInClicked()
    {
        int reward;
        bool success = AccountManager.Instance.SignIn(out reward);
        if (success)
        {
            // 1. 更新 ReadyPanelController 的 totalCoinsText
            ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
            if (readyPanel != null)
            {
                readyPanel.UpdateTotalCoinsUI(reward);
            }
            // 2. 高亮当前签到天数
            HighlightCurrentDay();
            // 3. 动画金币飞向 totalCoinsText
            AnimateCoinOnSignIn();
            Debug.Log("你获得了金币！");
        }
        else
        {
            Debug.Log("你今天已经签到过了。");
        }
        UpdateUI();
    }

    /// <summary>
    /// 更新 UI 元素
    /// </summary>
    private void UpdateUI()
    {
        // 如果今天已签到，禁用签到按钮
        DateTime today = DateTime.Today;
        if (PlayInforManager.Instance.playInfor.lastSignInDate.Date == today)
        {
            //TTOD1文字内容表里进行更改
            signInText.text = "今日已经领取过了";
            signInButton.interactable = false;
        }
        else
        {
            signInText.text = "登录领取奖励";
            signInButton.interactable = true;
        }
        // 高亮当前签到天数
        HighlightCurrentDay();
        // 更新 RedNoteImg 的显示状态
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel != null)
        {
            readyPanel.UpdateRedNote();
        }
    }

    /// <summary>
    /// 根据连续签到天数高亮当前天数
    /// </summary>
    private void HighlightCurrentDay()
    {
        // 首先，移除所有现有的高亮
        foreach (var dayUI in dayUIs)
        {
            dayUI.gameObject.SetActive(false);
        }
        // 确定要高亮的天数
        int dayToHighlight = PlayInforManager.Instance.playInfor.consecutiveDays;
        if (dayToHighlight > dayUIs.Count)
        {
            dayToHighlight = dayUIs.Count; // 超过最后一天时，高亮最后一天
        }

        if (dayToHighlight > 0 && dayToHighlight <= dayUIs.Count)
        {
            dayUIs[dayToHighlight - 1].SetActive(true);
        }
        if (dayToHighlight == 0)
        {
            dayUIs[dayToHighlight].SetActive(true);
        }
    }

    /// <summary>
    /// 动画金币从当前签到天数 UI 飞向 totalCoinsText
    /// </summary>
    //private void AnimateCoinOnSignIn()
    //{
    //    // 获取 ReadyPanelController 的 totalCoinsText 的位置
    //    ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
    //    if (readyPanel == null) return;
    //    Vector3 targetPosition = GameObject.Find("CointargetPos").transform.position;
    //    // 获取当前签到天数 UI 的位置
    //    int currentDay = PlayInforManager.Instance.playInfor.consecutiveDays;
    //    if (currentDay > dayUIs.Count) currentDay = dayUIs.Count;
    //    if (currentDay < 0) return;
    //    if (currentDay == 0)
    //    {
    //        GameObject currentDayUI = dayUIs[currentDay];
    //        Vector3 startPosition = currentDayUI.transform.position;
    //        // 动画金币
    //        readyPanel.AnimateCoin(startPosition, targetPosition, 10);
    //    }
    //    else
    //    {
    //        GameObject currentDayUI = dayUIs[currentDay - 1];
    //        Vector3 startPosition = currentDayUI.transform.position;
    //        // 动画金币
    //        readyPanel.AnimateCoin(startPosition, targetPosition, 10);
    //    }
    public int FlyCoinNum = 10;
    private async UniTask AnimateCoinOnSignIn()
    {
        // 获取 ReadyPanelController 的 totalCoinsText 的位置
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel == null) return;

        RectTransform totalCoinsRect = readyPanel.totalCoinsText.GetComponent<RectTransform>();
        if (totalCoinsRect == null)
        {
            Debug.LogError("totalCoinsText 缺少 RectTransform 组件！");
            return;
        }

        Vector3 targetPosition = totalCoinsRect.anchoredPosition;

        // 获取当前签到天数 UI 的位置
        int currentDay = PlayInforManager.Instance.playInfor.consecutiveDays;
        if (currentDay > dayUIs.Count) currentDay = dayUIs.Count;
        if (currentDay < 0) return;

        GameObject currentDayUI = currentDay > 0 ? dayUIs[currentDay - 1] : dayUIs[0];
        RectTransform currentDayRect = currentDayUI.transform.parent.GetComponent<RectTransform>();
        if (currentDayRect == null)
        {
            Debug.LogError("当前签到天数 UI 缺少 RectTransform 组件！");
            return;
        }

        Vector3 startPosition = currentDayRect.anchoredPosition;
        Debug.Log("当前签到天数 UI 的·位置 =================" + startPosition);
        // 获取当前脚本所在的Canvas
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("找不到父Canvas！");
            return;
        }

        // 动画金币
        for (int i = 1; i <= FlyCoinNum; i++)
        {
            // 实例化coinObj，并将其设置为parentCanvas的子物体
            GameObject coinObj = Instantiate(Resources.Load<GameObject>("Prefabs/Coin/newgold"),parentCanvas.transform); 
            // 设置coinObj的RectTransform的锚点位置为startPosition
            RectTransform coinRect = coinObj.GetComponent<RectTransform>();
            if (coinRect == null)
            {
                Debug.LogError("coinObj 缺少 RectTransform 组件！");
                continue;
            }
            coinRect.anchoredPosition = new Vector2(startPosition.x, startPosition.y);

            // 播放动画
            UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }

            // 获取Gold组件并启动移动逻辑
            Gold gold = coinObj.GetComponent<Gold>();
            if (gold != null)
            {
                Debug.Log("当前的目标正确位置·位置 =================" + targetPosition);
                gold.AwaitMovePanel(targetPosition);
            }
            else
            {
                Debug.LogError("coinObj 缺少 Gold 组件！");
            }
            // 等待0.05秒后继续生成下一个金币
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
    }


    /// <summary>
    /// 关闭签到面板
    /// </summary>
    private void OnCloseClicked()
    {
        signInButton.onClick.RemoveListener(OnSignInClicked);
        CloseCheckBtn.onClick.RemoveListener(OnCloseClicked);
        Destroy(transform.gameObject);
    }
}
