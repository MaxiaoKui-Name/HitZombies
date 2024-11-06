using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

        // ��Ӱ�ť����¼�����
        ContibueBtn_F.onClick.AddListener(OnContinueClicked);
        ReturnBtn_F.onClick.AddListener(OnReturnClicked);
    }

    /// <summary>
    /// �������������ť��Ĵ���
    /// </summary>
    private void OnContinueClicked()
    {
        GameManage.Instance.KilledMonsterNun = 0;
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
