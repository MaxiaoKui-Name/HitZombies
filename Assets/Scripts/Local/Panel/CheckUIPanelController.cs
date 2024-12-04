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

    //控制签到内部
    private bool isHighlightAnimating = true; // 控制高亮动画的开关
    private Vector3 highlightScaleMin = Vector3.one; // 最小缩放
    private Vector3 highlightScaleMax = Vector3.one * 1.2f; // 最大缩放
    public float highlightAnimSpeed = 1.0f; // 缩放速度
    private Coroutine highlightCoroutine; // 保存协程引用
    void Start()
    {
        GetAllChild(transform);
        signInButton = childDic["ReceiveBtn_F"].GetComponent<Button>();
        CloseCheckBtn = childDic["Return_F"].GetComponent<Button>();
        signInText = childDic["signInText_F"].GetComponent<TextMeshProUGUI>();
        dayPoss = new List<GameObject>();
        dayUIs = new List<GameObject>();
        highlightPrefab = Resources.Load<GameObject>("Prefabs/highlightPrefab");
        foreach (Transform child in childDic["CheckDays_F"].transform)
        {
            dayUIs.Add(child.gameObject);
        }
        // 赋值签到奖励金额
        AssignCoinNumText();
        UpdateUI();
        // 初始化面板缩放为0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // 获取 ClickAMature 对象及其动画组件
        GetClickAnim(transform);
        // 修改按钮点击事件监听器
        signInButton.onClick.AddListener(() => StartCoroutine(OnSignInButtonClicked()));
        CloseCheckBtn.onClick.AddListener(() => StartCoroutine(OnCloseButtonClicked()));
    }

    /// <summary>
    /// 处理签到按钮点击事件的协程
    /// </summary>
    private IEnumerator OnSignInButtonClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(signInButton.GetComponent<RectTransform>(), OnSignInClicked));
    }

    /// <summary>
    /// 处理关闭按钮点击事件的协程
    /// </summary>
    private IEnumerator OnCloseButtonClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(CloseCheckBtn.GetComponent<RectTransform>(), OnCloseClicked));
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
            // 停止高亮动画
            isHighlightAnimating = false;
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
                highlightCoroutine = null;
            }
            // 确保高亮图显示并缩放为1
            GameObject currentHighlight = GetCurrentHighlightImage();
            if (currentHighlight != null)
            {
                currentHighlight.SetActive(true);
                RectTransform highlightRect = currentHighlight.GetComponent<RectTransform>();
                if (highlightRect != null)
                {
                    highlightRect.localScale = Vector3.one;
                }
            }

            // 1. 更新 ReadyPanelController 的 totalCoinsText
            ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
            if (readyPanel != null)
            {
                readyPanel.UpdateTotalCoinsUI(reward);
            }
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
        DateTime today = DateTime.Today;
        if (PlayInforManager.Instance.playInfor.lastSignInDate.Date == today)
        {
            signInText.text = "Claimed";
            signInButton.interactable = false;
            isHighlightAnimating = false;
            UpdateHighImages();

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
                highlightCoroutine = null;
            }
        }
        else
        {
            signInText.text = "Claim reward";
            signInButton.interactable = true;
            isHighlightAnimating = true;
            StartHighlightAnimation();
        }
        // 更新打勾图片
        UpdateHookImages();
        // 更新 RedNoteImg 的显示状态
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel != null)
        {
            readyPanel.UpdateRedNote();
        }
    }
    public int FlyCoinNum = 10;
    //金币飞行动画
    private void AnimateCoinOnSignIn()
    {
        // 获取 ReadyPanelController 的 totalCoinsText 的位置
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel == null) return;

        RectTransform totalCoinsRect = readyPanel.TotalCoinImg_F.GetComponent<RectTransform>();
        if (totalCoinsRect == null)
        {
            Debug.LogError("totalCoinsText 缺少 RectTransform 组件！");
            return;
        }
        Transform startPosition = signInButton.GetComponent<RectTransform>();
        Debug.Log("当前签到天数 UI 的·位置 =================" + startPosition);
        // 获取当前脚本所在的Canvas
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("找不到父Canvas！");
            return;
        }
        StartCoroutine(AnimateCoins(startPosition, totalCoinsRect, transform.gameObject));
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

    private void StartHighlightAnimation()
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }
        highlightCoroutine = StartCoroutine(AnimateHighlight());
    }

    private IEnumerator AnimateHighlight()
    {
        GameObject currentHighlight = GetCurrentHighlightImage();
        if (currentHighlight == null)
        {
            yield break;
        }

        RectTransform highlightRect = currentHighlight.GetComponent<RectTransform>();
        if (highlightRect == null)
        {
            yield break;
        }

        bool scalingUp = true;

        while (isHighlightAnimating)
        {
            if (scalingUp)
            {
                highlightRect.localScale = Vector3.MoveTowards(highlightRect.localScale, highlightScaleMax, highlightAnimSpeed * Time.deltaTime);
                if (highlightRect.localScale == highlightScaleMax)
                {
                    scalingUp = false;
                }
            }
            else
            {
                highlightRect.localScale = Vector3.MoveTowards(highlightRect.localScale, highlightScaleMin, highlightAnimSpeed * Time.deltaTime);
                if (highlightRect.localScale == highlightScaleMin)
                {
                    scalingUp = true;
                }
            }
            yield return null;
        }

        // 动画结束后，确保高亮图的缩放为1
        highlightRect.localScale = Vector3.one;
    }
    //获得高亮图
    private GameObject GetCurrentHighlightImage()
    {
        int dayToHighlight = PlayInforManager.Instance.playInfor.consecutiveDays;

        // 如果连续签到天数超过总天数，设置为最大天数
        if (dayToHighlight > dayUIs.Count)
        {
            dayToHighlight = dayUIs.Count;
        }

        // 遍历所有天数的高亮图，先全部隐藏
        foreach (GameObject dayUI in dayUIs)
        {
            Transform dayTransform = dayUI.transform;
            GameObject highlightImage = dayTransform.Find("highlightDay").gameObject;
            if (highlightImage != null)
            {
                highlightImage.SetActive(false); // 隐藏高亮图
            }
        }

        // 获取当前需要显示的高亮图
        if (dayToHighlight > 0 && dayToHighlight <= dayUIs.Count)
        {
            Transform dayTransform = dayUIs[dayToHighlight - 1].transform;
            GameObject highlightImage = dayTransform.Find("highlightDay").gameObject;
            if (highlightImage != null)
            {
                highlightImage.SetActive(true); // 显示当前的高亮图
                return highlightImage;
            }
        }
        else if (dayToHighlight == 0)
        {
            Transform dayTransform = dayUIs[0].transform;
            GameObject highlightImage = dayTransform.Find("highlightDay").gameObject;
            if (highlightImage != null)
            {
                highlightImage.SetActive(true); // 显示第一天的高亮图
                return highlightImage;
            }
        }
        return null;
    }

    //显示签到奖励数
    private void AssignCoinNumText()
    {
        for (int i = 0; i < dayUIs.Count; i++)
        {
            Transform dayTransform = dayUIs[i].transform;
            TextMeshProUGUI coinNumText = dayTransform.Find("CoinNum").GetComponent<TextMeshProUGUI>();
            if (coinNumText != null)
            {
                float rewardAmount = AccountManager.Instance.GetDailyReward(i + 1) * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
                coinNumText.text = $"×{rewardAmount}";
            }
        }
    }
    private void UpdateHighImages()
    {
        for (int i = 0; i < dayUIs.Count; i++)
        {
            Transform dayTransform = dayUIs[i].transform;
            GameObject HighImages = dayTransform.Find("highlightDay").gameObject;
            if (HighImages != null)
            {
                HighImages.SetActive(false);
            }
        }
    }
    //显示打勾图片
    private void UpdateHookImages()
    {
        int signedDays = PlayInforManager.Instance.playInfor.consecutiveDays;
        for (int i = 0; i < dayUIs.Count; i++)
        {
            Transform dayTransform = dayUIs[i].transform;
            GameObject hookImage = dayTransform.Find("DayHook").gameObject;
            if (hookImage != null)
            {
                if (i < signedDays)
                {
                    // 已签到，显示打勾
                    hookImage.SetActive(true);
                }
                else
                {
                    // 未签到，隐藏打勾
                    hookImage.SetActive(false);
                }
            }
        }
    }

}
