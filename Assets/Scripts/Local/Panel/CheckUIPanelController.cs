using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckUIPanelController : UIBase
{
    public Button signInButton;
    public Button CloseCheckBtn;
    // �������ǩ�������� UI Ԫ�أ���1������5�켰���ϣ�
    public List<GameObject>dayUIs; // �� Inspector �а�˳��ֵ��Day1, Day2, ..., Day5+
    public List<GameObject> dayPoss; // �� Inspector �а�˳��ֵ��Day1, Day2, ..., Day5+
    // ����Ԥ���壨����һ���߿�
    public GameObject highlightPrefab; // �� Inspector ��ָ��
    
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
            //TTOD1�������ݱ�����и���
            signInText.text = "�����Ѿ���ȡ����";
            signInButton.interactable = false;
        }
        else
        {
            signInText.text = "��¼��ȡ����";
            signInButton.interactable = true;
        }
        // ������ǰǩ������
        HighlightCurrentDay();
        // ���� RedNoteImg ����ʾ״̬
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel != null)
        {
            readyPanel.UpdateRedNote();
        }
    }

    /// <summary>
    /// ��������ǩ������������ǰ����
    /// </summary>
    private void HighlightCurrentDay()
    {
        // ���ȣ��Ƴ��������еĸ���
        foreach (var dayUI in dayUIs)
        {
            dayUI.gameObject.SetActive(false);
        }
        // ȷ��Ҫ����������
        int dayToHighlight = PlayInforManager.Instance.playInfor.consecutiveDays;
        if (dayToHighlight > dayUIs.Count)
        {
            dayToHighlight = dayUIs.Count; // �������һ��ʱ���������һ��
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
    /// ������Ҵӵ�ǰǩ������ UI ���� totalCoinsText
    /// </summary>
    //private void AnimateCoinOnSignIn()
    //{
    //    // ��ȡ ReadyPanelController �� totalCoinsText ��λ��
    //    ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
    //    if (readyPanel == null) return;
    //    Vector3 targetPosition = GameObject.Find("CointargetPos").transform.position;
    //    // ��ȡ��ǰǩ������ UI ��λ��
    //    int currentDay = PlayInforManager.Instance.playInfor.consecutiveDays;
    //    if (currentDay > dayUIs.Count) currentDay = dayUIs.Count;
    //    if (currentDay < 0) return;
    //    if (currentDay == 0)
    //    {
    //        GameObject currentDayUI = dayUIs[currentDay];
    //        Vector3 startPosition = currentDayUI.transform.position;
    //        // �������
    //        readyPanel.AnimateCoin(startPosition, targetPosition, 10);
    //    }
    //    else
    //    {
    //        GameObject currentDayUI = dayUIs[currentDay - 1];
    //        Vector3 startPosition = currentDayUI.transform.position;
    //        // �������
    //        readyPanel.AnimateCoin(startPosition, targetPosition, 10);
    //    }
    private void AnimateCoinOnSignIn()
    {
        // ��ȡ ReadyPanelController �� totalCoinsText ��λ��
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel == null) return;
        RectTransform totalCoinsRect = readyPanel.totalCoinsText.GetComponent<RectTransform>();
        if (totalCoinsRect == null)
        {
            Debug.LogError("totalCoinsText ȱ�� RectTransform �����");
            return;
        }

        Vector3 targetPosition = totalCoinsRect.position;

        // ��ȡ��ǰǩ������ UI ��λ��
        int currentDay = PlayInforManager.Instance.playInfor.consecutiveDays;
        if (currentDay > dayUIs.Count) currentDay = dayUIs.Count;
        if (currentDay < 0) return;
        GameObject currentDayUI = currentDay > 0 ? dayUIs[currentDay - 1] : dayUIs[0];
        RectTransform currentDayRect = currentDayUI.GetComponent<RectTransform>();
        if (currentDayRect == null)
        {
            Debug.LogError("��ǰǩ������ UI ȱ�� RectTransform �����");
            return;
        }

        Vector3 startPosition = currentDayRect.position;

        // �������
        readyPanel.AnimateCoin(startPosition, targetPosition, 10);
    }

    /// <summary>
    /// �ر�ǩ�����
    /// </summary>
    private void OnCloseClicked()
        {
            Destroy(transform.gameObject);
        }
}