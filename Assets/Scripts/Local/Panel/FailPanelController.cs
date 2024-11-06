using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FailPanelController : UIBase
{
    // UI元素
    [Header("UI元素")]
    public Button ContibueBtn_F;     // 继续本关游戏按钮
    public Button ReturnBtn_F;       // 返回主页按钮

    // 主菜单场景名称（请确保在Build Settings中已添加该场景）
    [Header("场景设置")]
    public string mainMenuSceneName = "MainMenu"; // 主菜单场景名称

    void Start()
    {
        GetAllChild(transform);
        ContibueBtn_F = childDic["FailContibueBtn_F"].GetComponent<Button>();
        ReturnBtn_F = childDic["FailReturnBtn_F"].GetComponent<Button>();

        // 添加按钮点击事件监听
        ContibueBtn_F.onClick.AddListener(OnContinueClicked);
        ReturnBtn_F.onClick.AddListener(OnReturnClicked);
    }

    /// <summary>
    /// 点击“继续”按钮后的处理
    /// </summary>
    private void OnContinueClicked()
    {
        GameManage.Instance.KilledMonsterNun = 0;
        GameFlowManager.Instance.NextLevel();
        UIManager.Instance.ChangeState(GameState.Running);
        Destroy(gameObject);
    }

    /// <summary>
    /// 点击“返回主页”按钮后的处理
    /// </summary>
    private void OnReturnClicked()
    {
        AccountManager.Instance.SaveAccountData();
        UIManager.Instance.ChangeState(GameState.Ready);
        Destroy(gameObject);
    }
}
