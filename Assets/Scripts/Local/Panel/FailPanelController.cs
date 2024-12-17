using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Button = UnityEngine.UI.Button;
using DragonBones;
using Transform = UnityEngine.Transform;

public class FailPanelController : UIBase
{
    // UIԪ��
    [Header("UIԪ��")]
    public Button ContibueBtn_F;     // ����������Ϸ��ť
    public Button ReturnBtn_F;       // ������ҳ��ť
    public Button InitialFailReturnBtn_F;

    // ���˵��������ƣ���ȷ����Build Settings������Ӹó�����
    [Header("��������")]
    public string mainMenuSceneName = "MainMenu"; // ���˵���������
    public Transform Box_F;                      // ���������� Box_F ����

    public UnityArmatureComponent boxArmature;   // ���Ƕ������

    void Start()
    {
        // ��ȡ������
        GetAllChild(transform);
        ContibueBtn_F = childDic["FailContibueBtn_F"].GetComponent<Button>();
        ReturnBtn_F = childDic["FailReturnBtn_F"].GetComponent<Button>();
        InitialFailReturnBtn_F = childDic["InitialFailReturnBtn_F"].GetComponent<Button>();
        Box_F = childDic["Box_F"];

        // ��ʼ����ť����
        ContibueBtn_F.gameObject.SetActive(false);
        ReturnBtn_F.gameObject.SetActive(false);
        InitialFailReturnBtn_F.gameObject.SetActive(false);
        // ��ʼ�����
        GameManage.Instance.InitialPalyer();

        // ��ʼ���������Ϊ0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));

        // ��ȡ Box_F �����Ƕ������
        boxArmature = Box_F.GetChild(0).GetComponent<UnityArmatureComponent>();

        // ��Ӱ�ť����¼�������
        ContibueBtn_F.onClick.AddListener(() => StartCoroutine(OnContibueBtn_FClicked()));
        ReturnBtn_F.onClick.AddListener(() => StartCoroutine(OnReturnBtn_FClicked()));
        InitialFailReturnBtn_F.onClick.AddListener(() => StartCoroutine(OnInitialFailReturnBtn_FClicked()));
        // ��ʼ�����߼�
        StartCoroutine(PlayFailPanelAnimation());
    }

    /// <summary>
    /// ����ʧ��ҳ�涯����Э��
    /// </summary>
    private IEnumerator PlayFailPanelAnimation()
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        if (boxArmature == null)
        {
            Debug.LogError("Box_F �����Ƕ������δ�ҵ���");
            yield break;
        }
        // ���� "start" ����һ��
        boxArmature.animation.Play("start", 1);
        Debug.Log("���� start ����");

        var animationState = boxArmature.animation.Play("start", 1);
        while (!animationState.isCompleted)
        {
            yield return null;
        }
        // ���� "stay" ����ѭ��
        boxArmature.animation.Play("stay", 0);
        Debug.Log("���� stay ����");
        if(!PlayInforManager.Instance.playInfor.FirstZeroToOne)
        {
            // ��ʾ��ť
            ContibueBtn_F.gameObject.SetActive(true);
            ReturnBtn_F.gameObject.SetActive(true);
        }
        else
        {
            InitialFailReturnBtn_F.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// ���������ť����¼���Э��
    /// </summary>
    private IEnumerator OnContibueBtn_FClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(ContibueBtn_F.GetComponent<RectTransform>(), OnContinueClicked));
    }

    /// <summary>
    /// ��������ҳ��ť����¼���Э��
    /// </summary>
    private IEnumerator OnReturnBtn_FClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(ReturnBtn_F.GetComponent<RectTransform>(), OnReturnClicked));
    }
    private IEnumerator OnInitialFailReturnBtn_FClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(InitialFailReturnBtn_F.GetComponent<RectTransform>(), OnReturnClicked));
    }

    /// <summary>
    /// �������������ť��Ĵ���
    /// </summary>
    private void OnContinueClicked()
    {
        GameFlowManager.Instance.NextLevel();
        UIManager.Instance.ChangeState(GameState.Running);
        Destroy(gameObject);
    }

    /// <summary>
    /// �����������ҳ����ť��Ĵ���
    /// </summary>
    private void OnReturnClicked()
    {
        GameManage.Instance.InitialPalyer();
        AccountManager.Instance.SaveAccountData();
        UIManager.Instance.ChangeState(GameState.Ready);
        boxArmature.animation.Play("<None>", 0);
        Destroy(gameObject);
    }
}
