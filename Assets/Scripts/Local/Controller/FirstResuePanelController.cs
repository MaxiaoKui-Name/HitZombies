using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FirstResuePanelController : UIBase
{
    // ��������UIԪ��
    public TextMeshProUGUI countdownNum_F;              // ����ʱ��ʾ
    public Button WatchAdBtn_F;              // �ۿ���水ť
    public Button CloseBtn;                   // ���ذ�ť
    public Text CoinNumText;                 // ��ʾ����������ı�

    // ����ʧ�����
    public GameObject levelFailedPanel;      // ����ʧ�����
    public GameObject ResuePanel;      // ����ʧ�����
    // ����ʱ����
    private float countdown = 60f;            // 60�뵹��ʱ
    private bool isCounting = false;          // �Ƿ����ڼ�ʱ

    void Start()
    {
        GetAllChild(transform);
        countdownNum_F = childDic["FirstResuecountdownNum_F"].GetComponent<TextMeshProUGUI>();
        WatchAdBtn_F = childDic["FirstResueWatchAdBtn_F"].GetComponent<Button>();
        CloseBtn = childDic["FirstResueCloseBtn_F"].GetComponent<Button>();
        CoinNumText = childDic["FirstResueCoinNumText_F"].GetComponent<Text>(); 
        // TTOD1��ʾ������ִ�����
        ShowRevivePanel((int)(ConfigManager.Instance.Tables.TableGlobal.Get(15).IntValue * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total));
        WatchAdBtn_F.onClick.AddListener(OnWatchAdClicked);
        CloseBtn.onClick.AddListener(OnCloseClicked);
    }

    void Update()
    {
        if (isCounting)
        {
            countdown -= Time.unscaledDeltaTime; // ʹ�� unscaledDeltaTime
            if (countdown <= 0)
            {
                countdown = 0;
                UpdateCountdownUI();
                isCounting = false;
                OnCountdownFinished();
            }
            else
            {
                UpdateCountdownUI();
            }
        }
    }

    // ��ʾ������岢��ʼ����ʱ
    public void ShowRevivePanel(int coinCount)
    {
        CoinNumText.text = coinCount.ToString("N0");
        countdown = 60f;
        isCounting = true;
        UpdateCountdownUI();
    }

    // ���µ���ʱUI��ʾ
    private void UpdateCountdownUI()
    {
        int displayTime = Mathf.CeilToInt(countdown);
        countdownNum_F.text = displayTime.ToString();
    }

    // ����ʱ������Ĵ���
    private void OnCountdownFinished()
    {
        ShowLevelFailedPanel();
        Destroy(gameObject);
    }

    // �ۿ���水ť�������
    private void OnWatchAdClicked()
    {
        // ֹͣ����ʱ
        isCounting = false;
        // ģ��ۿ����Ĺ��̣�������Լ�����ʵ�Ĺ��SDK
        StartCoroutine(WatchAdCoroutine());
    }

    // ģ��ۿ�����Э��
    private IEnumerator WatchAdCoroutine()
    {
        // ��ʾ��������ʾ����ѡ��
        Debug.Log("��濪ʼ����...");
        // �����沥����Ҫ5��
        yield return new WaitForSecondsRealtime(1f);
        // ��沥����ɺ�Ĵ���
        Debug.Log("��沥����ɣ���Ҹ��");
        ShowResuePanel();
        Destroy(gameObject);
        // �����������Ӹ����߼��������������λ�á�������
    }

    // ���ذ�ť�������
    private void OnCloseClicked()
    {
        // ֹͣ����ʱ
        isCounting = false;
        ShowLevelFailedPanel();
        Destroy(gameObject);
    }

    // ��ʾ����ʧ�����
    private void ShowLevelFailedPanel()
    {
        UIManager.Instance.ChangeState(GameState.GameOver);
        EventDispatcher.instance.DispatchEvent(EventNameDef.GAME_OVER);
        GameManage.Instance.GameOverReset();
        GameManage.Instance.InitialPalyer();
    }

    private void ShowResuePanel()
    {
        ResuePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/ResuePanel"));
        ResuePanel.transform.SetParent(transform.parent, false);
        ResuePanel.transform.localPosition = Vector3.zero;
    }
}
