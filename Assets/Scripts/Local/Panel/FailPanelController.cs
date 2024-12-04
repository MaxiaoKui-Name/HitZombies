using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
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
        GameManage.Instance.InitialPalyer();
        //// 添加按钮点击事件监听
        //ContibueBtn_F.onClick.AddListener(OnContinueClicked);
        //ReturnBtn_F.onClick.AddListener(OnReturnClicked);
        // 初始化面板缩放为0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // 获取 ClickAMature 对象及其动画组件
        GetClickAnim(transform);
        // 修改按钮点击事件监听器
        ContibueBtn_F.onClick.AddListener(() => StartCoroutine(OnContibueBtn_FClicked()));
        ReturnBtn_F.onClick.AddListener(() => StartCoroutine(OnReturnBtn_FClicked()));
    }

    /// <summary>
    /// 处理签到按钮点击事件的协程
    /// </summary>
    private IEnumerator OnContibueBtn_FClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(ContibueBtn_F.GetComponent<RectTransform>(), OnContinueClicked));
    }
    private IEnumerator OnReturnBtn_FClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(ReturnBtn_F.GetComponent<RectTransform>(), OnReturnClicked));
    }

    /// <summary>
    /// 点击“继续”按钮后的处理
    /// </summary>
    private void OnContinueClicked()
    {
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
