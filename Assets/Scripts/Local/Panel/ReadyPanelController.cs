using Cysharp.Threading.Tasks;
using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReadyPanelController : UIBase
{
    public Button StartGameBtn;
    public Button OpenURLBtn;
    public UIManager uIManager;
    public Button CheckBtn;
    public Button TurntableBtn;
    public Button MailBtn;
    public TextMeshProUGUI totalCoinsText;
    private GameObject CheckUIPanel; // ǩ�����
    private GameObject TurnTablePanel; // ת�����
    private GameObject MailPanel; // ת�����
                                  // ���ڽ�Ҷ���������
    public UnityEngine.Transform coinStartTransform; // ͨ�� Inspector ָ����̬��ȡ
    private Coroutine coinAnimationCoroutine;
    public Image RedNoteImg;
    public Image TurntableRedNoteImg;
    public Image redIndicator; // ��ɫ��ʾͼƬ
    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        GetAllChild(transform);
        RedNoteImg = childDic["RedNote_F"].GetComponent<Image>();
        redIndicator = childDic["MailBtnRedNote_F"].GetComponent<Image>();
        TurntableRedNoteImg = childDic["TurntableRedNote_F"].GetComponent<Image>();
        StartGameBtn = childDic["ReadystartGame_F"].GetComponent<Button>();
        TurntableBtn = childDic["TurntableBtn_F "].GetComponent<Button>();
        OpenURLBtn = childDic["OtherURL_F"].GetComponent<Button>();
        CheckBtn = childDic["CheckBtn_F"].GetComponent<Button>();
        totalCoinsText = childDic["totalCoinsText_F"].GetComponent<TextMeshProUGUI>();
        MailBtn = childDic["MailBtn_F"].GetComponent<Button>();
        totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        // �ж��Ƿ�ÿ���Ƿ��״ε�¼
        UpdateRedNote();
        OpenURLBtn.gameObject.SetActive(ConfigManager.Instance.Tables.TableJumpConfig.Get(1).IsOpen);
        StartGameBtn.onClick.AddListener(OnStartGameButtonClicked);
        CheckBtn.onClick.AddListener(OnCheckonClicked);
        TurntableBtn.onClick.AddListener(OnWheelonClicked);
        MailBtn.onClick.AddListener(OnMailClicked);
        UpdateRedIndicator();

        // ������Ϣ�����¼�
        MessageManager.Instance.OnMessagesUpdated += UpdateRedIndicator;

        if (ConfigManager.Instance.Tables.TableJumpConfig.Get(1).IsOpen)
        {

            OpenURLBtn.onClick.AddListener(OnOpenURLButtonClicked); // ��Ӵ���
        }
        // ��ʼ�������ʾ
        UpdateTotalCoinsUI(AccountManager.Instance.GetTotalCoins());
    }
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
        int start = (int)(PlayInforManager.Instance.playInfor.coinNum - reward);
        int end = (int)(PlayInforManager.Instance.playInfor.coinNum);
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
            yield return null;
        }
        textMesh.text = end.ToString();
    }

}
