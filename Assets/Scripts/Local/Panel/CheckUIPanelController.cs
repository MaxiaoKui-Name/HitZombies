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
    // �������ǩ�������� UI Ԫ�أ���1������5�켰���ϣ�
    public List<GameObject> dayUIs; // �� Inspector �а�˳��ֵ��Day1, Day2, ..., Day5+
    public List<GameObject> dayPoss; // �� Inspector �а�˳��ֵ��Day1, Day2, ..., Day5+
    // ����Ԥ���壨����һ���߿�
    public GameObject highlightPrefab; // �� Inspector ��ָ��

    public TextMeshProUGUI signInText;

    //����ǩ���ڲ�
    private bool isHighlightAnimating = true; // ���Ƹ��������Ŀ���
    private Vector3 highlightScaleMin = Vector3.one; // ��С����
    private Vector3 highlightScaleMax = Vector3.one * 1.2f; // �������
    public float highlightAnimSpeed = 1.0f; // �����ٶ�
    private Coroutine highlightCoroutine; // ����Э������
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
        // ��ֵǩ���������
        AssignCoinNumText();
        UpdateUI();
        // ��ʼ���������Ϊ0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // ��ȡ ClickAMature �����䶯�����
        GetClickAnim(transform);
        // �޸İ�ť����¼�������
        signInButton.onClick.AddListener(() => StartCoroutine(OnSignInButtonClicked()));
        CloseCheckBtn.onClick.AddListener(() => StartCoroutine(OnCloseButtonClicked()));
    }

    /// <summary>
    /// ����ǩ����ť����¼���Э��
    /// </summary>
    private IEnumerator OnSignInButtonClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(signInButton.GetComponent<RectTransform>(), OnSignInClicked));
    }

    /// <summary>
    /// ����رհ�ť����¼���Э��
    /// </summary>
    private IEnumerator OnCloseButtonClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(CloseCheckBtn.GetComponent<RectTransform>(), OnCloseClicked));
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
            // ֹͣ��������
            isHighlightAnimating = false;
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
                highlightCoroutine = null;
            }
            // ȷ������ͼ��ʾ������Ϊ1
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

            // 1. ���� ReadyPanelController �� totalCoinsText
            ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
            if (readyPanel != null)
            {
                readyPanel.UpdateTotalCoinsUI(reward);
            }
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
        // ���´�ͼƬ
        UpdateHookImages();
        // ���� RedNoteImg ����ʾ״̬
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel != null)
        {
            readyPanel.UpdateRedNote();
        }
    }
    public int FlyCoinNum = 10;
    //��ҷ��ж���
    private void AnimateCoinOnSignIn()
    {
        // ��ȡ ReadyPanelController �� totalCoinsText ��λ��
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel == null) return;

        RectTransform totalCoinsRect = readyPanel.TotalCoinImg_F.GetComponent<RectTransform>();
        if (totalCoinsRect == null)
        {
            Debug.LogError("totalCoinsText ȱ�� RectTransform �����");
            return;
        }
        Transform startPosition = signInButton.GetComponent<RectTransform>();
        Debug.Log("��ǰǩ������ UI �ġ�λ�� =================" + startPosition);
        // ��ȡ��ǰ�ű����ڵ�Canvas
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("�Ҳ�����Canvas��");
            return;
        }
        StartCoroutine(AnimateCoins(startPosition, totalCoinsRect, transform.gameObject));
    }


    /// <summary>
    /// �ر�ǩ�����
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

        // ����������ȷ������ͼ������Ϊ1
        highlightRect.localScale = Vector3.one;
    }
    //��ø���ͼ
    private GameObject GetCurrentHighlightImage()
    {
        int dayToHighlight = PlayInforManager.Instance.playInfor.consecutiveDays;

        // �������ǩ����������������������Ϊ�������
        if (dayToHighlight > dayUIs.Count)
        {
            dayToHighlight = dayUIs.Count;
        }

        // �������������ĸ���ͼ����ȫ������
        foreach (GameObject dayUI in dayUIs)
        {
            Transform dayTransform = dayUI.transform;
            GameObject highlightImage = dayTransform.Find("highlightDay").gameObject;
            if (highlightImage != null)
            {
                highlightImage.SetActive(false); // ���ظ���ͼ
            }
        }

        // ��ȡ��ǰ��Ҫ��ʾ�ĸ���ͼ
        if (dayToHighlight > 0 && dayToHighlight <= dayUIs.Count)
        {
            Transform dayTransform = dayUIs[dayToHighlight - 1].transform;
            GameObject highlightImage = dayTransform.Find("highlightDay").gameObject;
            if (highlightImage != null)
            {
                highlightImage.SetActive(true); // ��ʾ��ǰ�ĸ���ͼ
                return highlightImage;
            }
        }
        else if (dayToHighlight == 0)
        {
            Transform dayTransform = dayUIs[0].transform;
            GameObject highlightImage = dayTransform.Find("highlightDay").gameObject;
            if (highlightImage != null)
            {
                highlightImage.SetActive(true); // ��ʾ��һ��ĸ���ͼ
                return highlightImage;
            }
        }
        return null;
    }

    //��ʾǩ��������
    private void AssignCoinNumText()
    {
        for (int i = 0; i < dayUIs.Count; i++)
        {
            Transform dayTransform = dayUIs[i].transform;
            TextMeshProUGUI coinNumText = dayTransform.Find("CoinNum").GetComponent<TextMeshProUGUI>();
            if (coinNumText != null)
            {
                float rewardAmount = AccountManager.Instance.GetDailyReward(i + 1) * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
                coinNumText.text = $"��{rewardAmount}";
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
    //��ʾ��ͼƬ
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
                    // ��ǩ������ʾ��
                    hookImage.SetActive(true);
                }
                else
                {
                    // δǩ�������ش�
                    hookImage.SetActive(false);
                }
            }
        }
    }

}
