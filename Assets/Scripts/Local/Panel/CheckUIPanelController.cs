using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckUIPanelController : UIBase
{
    public Button signInButton;
    public Button CloseCheckBtn;
    // 引用五个签到天数的 UI 元素（第1天至第5天及以上）
    public GameObject[] dayUIs; // 在 Inspector 中按顺序赋值：Day1, Day2, ..., Day5+

    // 高亮预制体（例如一个边框）
    public GameObject highlightPrefab; // 在 Inspector 中指定

    void Start()
    {
        GetAllChild(transform);
        signInButton = childDic["ReceiveBtn_F"].GetComponent<Button>();
        CloseCheckBtn = childDic["Return_F"].GetComponent<Button>();
        signInButton.onClick.AddListener(OnSignInClicked);
        CloseCheckBtn.onClick.AddListener(OnCloseClicked);
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
            signInButton.interactable = false;
        }
        else
        {
            signInButton.interactable = true;
        }
        // 高亮当前签到天数
        HighlightCurrentDay();
    }

    /// <summary>
    /// 根据连续签到天数高亮当前天数
    /// </summary>
    private void HighlightCurrentDay()
    {
        // 首先，移除所有现有的高亮
        foreach (var dayUI in dayUIs)
        {
            // 假设高亮是一个子对象，名称为 "Highlight"
            Transform highlight = dayUI.transform.Find("Highlight");
            if (highlight != null)
            {
                Destroy(highlight.gameObject);
            }
        }
        // 确定要高亮的天数
        int dayToHighlight = PlayInforManager.Instance.playInfor.consecutiveDays;
        if (dayToHighlight > dayUIs.Length)
        {
            dayToHighlight = dayUIs.Length; // 超过最后一天时，高亮最后一天
        }

        if (dayToHighlight > 0 && dayToHighlight <= dayUIs.Length)
        {
            GameObject dayUI = dayUIs[dayToHighlight - 1];
            Instantiate(highlightPrefab, dayUI.transform);
        }
    }

    /// <summary>
    /// 动画金币从当前签到天数 UI 飞向 totalCoinsText
    /// </summary>
    private void AnimateCoinOnSignIn()
    {
        // 获取 ReadyPanelController 的 totalCoinsText 的位置
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel == null) return;
        Vector3 targetPosition = readyPanel.totalCoinsText.transform.position;
        // 获取当前签到天数 UI 的位置
        int currentDay = PlayInforManager.Instance.playInfor.consecutiveDays;
        if (currentDay > dayUIs.Length) currentDay = dayUIs.Length;
        if (currentDay <= 0) return;
        GameObject currentDayUI = dayUIs[currentDay - 1];
        Vector3 startPosition = currentDayUI.transform.position;
        // 动画金币
        readyPanel.AnimateCoin(startPosition, targetPosition);
    }

    /// <summary>
    /// 关闭签到面板
    /// </summary>
    private void OnCloseClicked()
    {
        Destroy(transform.gameObject);
    }
}
