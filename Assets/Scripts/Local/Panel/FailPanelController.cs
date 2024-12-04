using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class FailPanelController : UIBase
{
    // UIԪ��
    [Header("UIԪ��")]
    public Button ContibueBtn_F;     // ����������Ϸ��ť
    public Button ReturnBtn_F;       // ������ҳ��ť

    // ���˵��������ƣ���ȷ����Build Settings������Ӹó�����
    [Header("��������")]
    public string mainMenuSceneName = "MainMenu"; // ���˵���������

    void Start()
    {
        GetAllChild(transform);
        ContibueBtn_F = childDic["FailContibueBtn_F"].GetComponent<Button>();
        ReturnBtn_F = childDic["FailReturnBtn_F"].GetComponent<Button>();
        GameManage.Instance.InitialPalyer();
        //// ��Ӱ�ť����¼�����
        //ContibueBtn_F.onClick.AddListener(OnContinueClicked);
        //ReturnBtn_F.onClick.AddListener(OnReturnClicked);
        // ��ʼ���������Ϊ0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // ��ȡ ClickAMature �����䶯�����
        GetClickAnim(transform);
        // �޸İ�ť����¼�������
        ContibueBtn_F.onClick.AddListener(() => StartCoroutine(OnContibueBtn_FClicked()));
        ReturnBtn_F.onClick.AddListener(() => StartCoroutine(OnReturnBtn_FClicked()));
    }

    /// <summary>
    /// ����ǩ����ť����¼���Э��
    /// </summary>
    private IEnumerator OnContibueBtn_FClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(ContibueBtn_F.GetComponent<RectTransform>(), OnContinueClicked));
    }
    private IEnumerator OnReturnBtn_FClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(ReturnBtn_F.GetComponent<RectTransform>(), OnReturnClicked));
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
        AccountManager.Instance.SaveAccountData();
        UIManager.Instance.ChangeState(GameState.Ready);
        Destroy(gameObject);
    }
}
