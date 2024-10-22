using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckUIPanelController : UIBase
{
    public Button signInButton;
    public Button CloseCheckBtn;
    // �������ǩ�������� UI Ԫ�أ���1������5�켰���ϣ�
    public GameObject[] dayUIs; // �� Inspector �а�˳��ֵ��Day1, Day2, ..., Day5+

    // ����Ԥ���壨����һ���߿�
    public GameObject highlightPrefab; // �� Inspector ��ָ��

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
    /// ����ǩ����ť����¼�
    /// </summary>
    private void OnSignInClicked()
    {
        int reward;
        bool success = AccountManager.Instance.SignIn(out reward);
        if (success)
        {
            // 1. ���� ReadyPanelController �� totalCoinsText
            ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
            if (readyPanel != null)
            {
                readyPanel.UpdateTotalCoinsUI(reward);
            }
            // 2. ������ǰǩ������
            HighlightCurrentDay();
            // 3. ������ҷ��� totalCoinsText
            AnimateCoinOnSignIn();

            Debug.Log("�����˽�ң�");
        }
        else
        {
            Debug.Log("������Ѿ�ǩ�����ˡ�");
        }
        UpdateUI();
    }

    /// <summary>
    /// ���� UI Ԫ��
    /// </summary>
    private void UpdateUI()
    {
        // ���������ǩ��������ǩ����ť
        DateTime today = DateTime.Today;
        if (PlayInforManager.Instance.playInfor.lastSignInDate.Date == today)
        {
            signInButton.interactable = false;
        }
        else
        {
            signInButton.interactable = true;
        }
        // ������ǰǩ������
        HighlightCurrentDay();
    }

    /// <summary>
    /// ��������ǩ������������ǰ����
    /// </summary>
    private void HighlightCurrentDay()
    {
        // ���ȣ��Ƴ��������еĸ���
        foreach (var dayUI in dayUIs)
        {
            // ���������һ���Ӷ�������Ϊ "Highlight"
            Transform highlight = dayUI.transform.Find("Highlight");
            if (highlight != null)
            {
                Destroy(highlight.gameObject);
            }
        }
        // ȷ��Ҫ����������
        int dayToHighlight = PlayInforManager.Instance.playInfor.consecutiveDays;
        if (dayToHighlight > dayUIs.Length)
        {
            dayToHighlight = dayUIs.Length; // �������һ��ʱ���������һ��
        }

        if (dayToHighlight > 0 && dayToHighlight <= dayUIs.Length)
        {
            GameObject dayUI = dayUIs[dayToHighlight - 1];
            Instantiate(highlightPrefab, dayUI.transform);
        }
    }

    /// <summary>
    /// ������Ҵӵ�ǰǩ������ UI ���� totalCoinsText
    /// </summary>
    private void AnimateCoinOnSignIn()
    {
        // ��ȡ ReadyPanelController �� totalCoinsText ��λ��
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel == null) return;
        Vector3 targetPosition = readyPanel.totalCoinsText.transform.position;
        // ��ȡ��ǰǩ������ UI ��λ��
        int currentDay = PlayInforManager.Instance.playInfor.consecutiveDays;
        if (currentDay > dayUIs.Length) currentDay = dayUIs.Length;
        if (currentDay <= 0) return;
        GameObject currentDayUI = dayUIs[currentDay - 1];
        Vector3 startPosition = currentDayUI.transform.position;
        // �������
        readyPanel.AnimateCoin(startPosition, targetPosition);
    }

    /// <summary>
    /// �ر�ǩ�����
    /// </summary>
    private void OnCloseClicked()
    {
        Destroy(transform.gameObject);
    }
}
