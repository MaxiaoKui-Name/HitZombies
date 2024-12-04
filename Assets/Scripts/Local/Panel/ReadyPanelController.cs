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
    private GameObject CheckUIPanel; // ǩ�����
    private GameObject TurnTablePanel; // ת�����
    private GameObject MailPanel; // ת�����
                                  // ���ڽ�Ҷ���������
    public UnityEngine.Transform coinStartTransform; // ͨ�� Inspector ָ����̬��ȡ
    private Coroutine coinAnimationCoroutine;
    public Image RedNoteImg;
    public Image TurntableRedNoteImg;
    public Image redIndicator; // ��ɫ��ʾͼƬ
    public TextMeshProUGUI DataBarLevelText;

    //�����Ϣ��ʾ
    public TextMeshProUGUI DataBarIDText_F;

    //����
    //��λ
    public Image DanImg_F;
    private Dictionary<string, Sprite> danSpriteDict;
    public TextMeshProUGUI DanText;
    //�Ǽ�
    private List<GameObject> starBackList = new List<GameObject>();
    private List<GameObject> starLightList = new List<GameObject>();
    private GameObject kingStart_F;

    //����
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
        [JsonIgnore] // �������л�
        public GameObject chestObject;
    }

    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        // ���ر�������
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

        //����
        DanImg_F = childDic["DanImg_F"].GetComponent<Image>();
        DanText = childDic["DanTextImg_F "].GetChild(0).GetComponent<TextMeshProUGUI>();
        InitializeDanSprites();
        UpdateDanImage();
        UpdateDanText();


        //��ȡ�Ǽ�
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

        //����\
        chestuiPrefab = Resources.Load<GameObject>("Prefabs/UIPannel/Chestui1");
        ChestText_F = childDic["ChestText_F"].gameObject;
        ChestText_F.SetActive(false);
        //TTOD1������
        rewardChestuiPanelPrefab = Resources.Load<GameObject>("Prefabs/UIPannel/RewardPanel");
        Point.transform.parent.gameObject.SetActive(false);

        // �ж��Ƿ�ÿ���Ƿ��״ε�¼
        UpdateRedNote();
        OpenURLBtn.gameObject.SetActive(ConfigManager.Instance.Tables.TableJumpConfig.Get(1).IsOpen);
        UpdateRedIndicator();
        // ������Ϣ�����¼�
        MessageManager.Instance.OnMessagesUpdated += UpdateRedIndicator;

        if (ConfigManager.Instance.Tables.TableJumpConfig.Get(1).IsOpen)
        {

            OpenURLBtn.onClick.AddListener(OnOpenURLButtonClicked); // ��Ӵ���
        }
        // ��ʼ�������ʾ
        UpdateTotalCoinsUI(AccountManager.Instance.GetTotalCoins());
        if (GameFlowManager.Instance.currentLevelIndex > 1)
        {
            previousDan = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex - 1).Dan.Substring(0, 3);
            CheckDanLevelUp();
            InitializeChestUI();
        }
        //����
      
        // ��ȡ ClickAMature �����䶯�����
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
/// ����ǩ����ť����¼���Э��
/// </summary>
private IEnumerator OnStartGameBtnClicked()
{
    // ���ŵ������
    yield return StartCoroutine(HandleButtonClickAnimation(transform));

    // ִ�а�ť�������������ú����߼�
    yield return StartCoroutine(ButtonBounceAnimation(StartGameBtn.GetComponent<RectTransform>(), OnStartGameButtonClicked));
}

/// <summary>
/// ����رհ�ť����¼���Э��
/// </summary>
private IEnumerator OnCheckBtnClicked()
{
    // ���ŵ������
    yield return StartCoroutine(HandleButtonClickAnimation(transform));

    // ִ�а�ť�������������ú����߼�
    yield return StartCoroutine(ButtonBounceAnimation(CheckBtn.GetComponent<RectTransform>(), OnCheckonClicked));
}
private IEnumerator OnTurntableBtntnClicked()
{
    // ���ŵ������
    yield return StartCoroutine(HandleButtonClickAnimation(transform));

    // ִ�а�ť�������������ú����߼�
    yield return StartCoroutine(ButtonBounceAnimation(TurntableBtn.GetComponent<RectTransform>(), OnWheelonClicked));
}
private IEnumerator OnMailBtnClicked()
{
    // ���ŵ������
    yield return StartCoroutine(HandleButtonClickAnimation(transform));

    // ִ�а�ť�������������ú����߼�
    yield return StartCoroutine(ButtonBounceAnimation(MailBtn.GetComponent<RectTransform>(), OnMailClicked));
}

#region//������ǰ����
public void UpdateRedIndicator()
    {
        int unreadCount = MessageManager.Instance.GetUnreadCount();
        redIndicator.gameObject.SetActive(unreadCount > 0);
    }
    // ��Ӵ�URL�ķ���
    void OnOpenURLButtonClicked()
    {
        string url = ConfigManager.Instance.Tables.TableJumpConfig.Get(1).URL; // TTOD1Ĭ��URL���������
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
            Debug.Log($"����URL: {url}");
        }
        else
        {
            Debug.LogWarning("URLδ���û�Ϊ�գ�");
        }
    }
    public void UpdateRedNote()
    {
        // �����ҿ���ǩ��������ʾ RedNoteImg����������
        RedNoteImg.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        //CheckBtn.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        TurntableRedNoteImg.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        //TurntableBtn.gameObject.SetActive(AccountManager.Instance.CanSignIn());
    }
    // Update is called once per frame
    void Update()
    {
    }

    // ��ť���ʱ���õķ���
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
            // ����䰴ť���¼�����
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
                OpenURLBtn.onClick.RemoveListener(OnOpenURLButtonClicked); // ��Ӵ���
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
    /// ʹ�ö���Ч�������ܽ�ҵ� UI
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
    /// ���ֹ���������Э��
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

    #region//��������
    //��ʾ��λ
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
            Debug.LogWarning($"δ�ҵ���λ��Ӧ��ͼƬ��{currentDan}");
        }
    }

    private string GetduanName(string duan)
    {
        switch (duan)
        {
            case "��ǿ��ͭ":
                return "Dan1";
            case "�������":
                return "Dan2";
            case "��ҫ�ƽ�":
                return "Dan3";
            case "��󲬽�":
                return "Dan4";
            case "������ʯ":
                return "Dan5";
            case "������ҫ":
                return "Dan6";
            case "��ǿ����":
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
        if (currentDan.Substring(2, 2) != "����")
        {
            fialName = fialName +" "+ currentDan.Substring(4, 1);
        }
        DanText.text = fialName;
    }
    string GetDanName(string ChineseDan)
    {
        switch (ChineseDan)
         {
            case "��ͭ":
               return "Bronze";
            case "����":
                return "Silver";
            case "�ƽ�":
                return "Gold";
            case "����":
                return "Platinum";
            case "��ʯ":
                return "Diamond";
            case "��ҫ":
                return "Master";
            case "����":
                return "Challenger";
            default:
                return "Bronze";
        }

    }

    //��ʾ�Ǽ�
    void UpdateStarDisplay()
    {
        int starRating = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).StarRating;
        string currentDan = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;
        if(currentDan.Substring(0,3) != "��ǿ����")
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

    //���α���
    void InitializeChestUI()
    {
        if (chestDataList.Count > 0)
        {
            Point.transform.parent.gameObject.SetActive(true);
        }
        foreach (var chestData in chestDataList)
        {
            // ʵ��������Ԥ����
            GameObject chest = Instantiate(chestuiPrefab, chestContent);

            // ��ӵ���¼�
            Button chestButton = chest.transform.GetChild(0).GetComponent<Button>();
            chestButton.onClick.AddListener(() => OnChestClicked(chestData.danLevel));
            chestButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;

            // ���±����UI״̬���򿪻�δ�򿪣�
            Image chestImage = chestButton.GetComponent<Image>();
            if (chestData.isClaimed)
            {
                chestImage.sprite = ChestuiImg[1];
            }
            else
            {
                chestImage.sprite = ChestuiImg[0];
            }

            // �����������
            chestData.chestObject = chest;
        }

        // ���±����λ��
        UpdateChestPositions();

        // ���±���������ʾ
        UpdateChestScrollPosition();
    }

    void CheckDanLevelUp()
    {
        string currentDan = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;
        if (currentDan.Substring(0,3) != previousDan)
        {
            Point.transform.parent.gameObject.SetActive(true);
            // ������Σ������µı���
            CreateNewChest(currentDan);
            previousDan = currentDan;
        }
    }
    void CreateNewChest(string danLevel)
    {
        // ʵ��������Ԥ����
        GameObject newChest = Instantiate(chestuiPrefab, chestContent);
        // ���ñ����λ��
        UpdateChestPositions();

        // ��ӵ���¼�
        Button chestButton = newChest.transform.GetChild(0).GetComponent<Button>();
        chestButton.onClick.AddListener(() => OnChestClicked(danLevel));
        chestButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).Dan;


        // �����������ݲ���ӵ��б�
        ChestData chestData = new ChestData
        {
            danLevel = danLevel,
            isClaimed = false,
            chestObject = newChest
        };
        chestDataList.Add(chestData);
        AccountManager.Instance.SaveChestData(chestDataList);
        // ���±���������ʾ
        UpdateChestScrollPosition();
    }

    void OnChestClicked(string danLevel)
    {
        // ���Ҷ�Ӧ�ı�������
        ChestData chestData = chestDataList.Find(c => c.danLevel == danLevel);
        if (chestData != null && !chestData.isClaimed)
        {
            // ��������UI���
            ShowRewardPanel(danLevel);
            // ���Ϊ����ȡ
            chestData.isClaimed = true;
            //chestData.chestObject.GetComponent<Button>().onClick.RemoveListener(() => OnChestClicked(danLevel));
            // ���ı���ͼƬΪ��״̬
            Image chestImage = chestData.chestObject.GetComponent<Image>();
            chestImage.sprite = ChestuiImg[1]; // ����Ҫ��ǰ���ش�״̬�ı���ͼƬ
                                               // ���汦������
            AccountManager.Instance.SaveChestData(chestDataList);
        }
    }
    void ShowRewardPanel(string danLevel)
    {
        // ʵ��������UI��壬��ʾ��Ӧ�Ľ�����Ϣ
        GameObject rewardPanel = Instantiate(rewardChestuiPanelPrefab, transform.parent);
        // �������ڽ�����������ý������ݺ���ȡ�߼�
    }

    void UpdateChestPositions()
    {
        int visibleChestCount = 3; // ���ӱ�������
        float chestSpacing = 149.98f; // ����֮��ļ�࣬���Ը�����Ҫ����

        if (chestDataList.Count <= visibleChestCount)
        {
            // �����������ڻ���ڿ��ӱ�������
            // ������ʼλ�ã�ʹ���������ʾ
            float totalWidth = (chestDataList.Count - 1) * chestSpacing;
            float startX = totalWidth / 2;

            for (int i = 0; i < chestDataList.Count; i++)
            {
                RectTransform chestRect = chestDataList[i].chestObject.GetComponent<RectTransform>();
                // ����ê��Ϊ����
                chestRect.anchorMin = new Vector2(0.5f, 0.5f);
                chestRect.anchorMax = new Vector2(0.5f, 0.5f);
                chestRect.pivot = new Vector2(0.5f, 0.5f);
                chestRect.anchoredPosition = new Vector2(startX + i * chestSpacing, 0);
            }
            // ����Content�Ŀ�ȣ�ȷ�������ֹ���
            RectTransform contentRect = chestContent.GetComponent<RectTransform>();
            contentRect.sizeDelta = chestScrollRect.GetComponent<RectTransform>().sizeDelta;
        }
        else
        {
            // �����������ڿ��ӱ�����������Ҫ�����鿴
            for (int i = 0; i < chestDataList.Count; i++)
            {
                RectTransform chestRect = chestDataList[i].chestObject.GetComponent<RectTransform>();
                // ����ê��Ϊ����
                chestRect.anchorMin = new Vector2(0f, 0.5f);
                chestRect.anchorMax = new Vector2(0f, 0.5f);
                chestRect.pivot = new Vector2(0f, 0.5f);
                chestRect.anchoredPosition = new Vector2(i * chestSpacing, 0);
            }
            // ����Content�Ŀ�ȣ��������б���
            RectTransform contentRect = chestContent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2((chestDataList.Count - 1) * chestSpacing + chestScrollRect.GetComponent<RectTransform>().sizeDelta.x, contentRect.sizeDelta.y);
        }
    }


    void UpdateChestScrollPosition()
    {
        if (chestDataList.Count <= 3)
        {
            // �����������ڻ���ڿ��ӱ�������������Ҫ������������ʾ
            chestScrollRect.horizontalNormalizedPosition = 0.5f;
        }
        else
        {
            // ���ҵ�һ��δ��ȡ�ı���
            int firstUnclaimedIndex = chestDataList.FindIndex(c => !c.isClaimed);
            if (firstUnclaimedIndex == -1)
            {
                firstUnclaimedIndex = chestDataList.Count - 1; // ���ȫ������ȡ����ʾ���һ������
            }
            // ������Ҫ������λ�ã�ʹ��һ��δ��ȡ�ı������
            float totalScrollableWidth = chestContent.GetComponent<RectTransform>().sizeDelta.x - chestScrollRect.GetComponent<RectTransform>().sizeDelta.x;
            float targetPosition = (firstUnclaimedIndex * 200f - chestScrollRect.GetComponent<RectTransform>().sizeDelta.x / 2 + 100f) / totalScrollableWidth;
            targetPosition = Mathf.Clamp01(targetPosition);
            chestScrollRect.horizontalNormalizedPosition = targetPosition;
        }
    }



    #endregion
}
