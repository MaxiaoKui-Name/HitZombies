using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Button = UnityEngine.UI.Button;
using DragonBones;
using Transform = UnityEngine.Transform;

public class FailPanelController : UIBase
{
    // UI元素
    [Header("UI元素")]
    public Button ContibueBtn_F;     // 继续本关游戏按钮
    public Button ReturnBtn_F;       // 返回主页按钮
    public Button InitialFailReturnBtn_F;

    // 主菜单场景名称（请确保在Build Settings中已添加该场景）
    [Header("场景设置")]
    public string mainMenuSceneName = "MainMenu"; // 主菜单场景名称
    public Transform Box_F;                      // 包含动画的 Box_F 对象

    public UnityArmatureComponent boxArmature;   // 龙骨动画组件

    void Start()
    {
        // 获取子物体
        GetAllChild(transform);
        ContibueBtn_F = childDic["FailContibueBtn_F"].GetComponent<Button>();
        ReturnBtn_F = childDic["FailReturnBtn_F"].GetComponent<Button>();
        InitialFailReturnBtn_F = childDic["InitialFailReturnBtn_F"].GetComponent<Button>();
        Box_F = childDic["Box_F"];

        // 初始化按钮隐藏
        ContibueBtn_F.gameObject.SetActive(false);
        ReturnBtn_F.gameObject.SetActive(false);
        InitialFailReturnBtn_F.gameObject.SetActive(false);
        // 初始化玩家
        GameManage.Instance.InitialPalyer();

        // 初始化面板缩放为0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));

        // 获取 Box_F 的龙骨动画组件
        boxArmature = Box_F.GetChild(0).GetComponent<UnityArmatureComponent>();

        // 添加按钮点击事件监听器
        ContibueBtn_F.onClick.AddListener(() => StartCoroutine(OnContibueBtn_FClicked()));
        ReturnBtn_F.onClick.AddListener(() => StartCoroutine(OnReturnBtn_FClicked()));
        InitialFailReturnBtn_F.onClick.AddListener(() => StartCoroutine(OnInitialFailReturnBtn_FClicked()));
        // 开始动画逻辑
        StartCoroutine(PlayFailPanelAnimation());
    }

    /// <summary>
    /// 播放失败页面动画的协程
    /// </summary>
    private IEnumerator PlayFailPanelAnimation()
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        if (boxArmature == null)
        {
            Debug.LogError("Box_F 的龙骨动画组件未找到！");
            yield break;
        }
        // 播放 "start" 动画一次
        boxArmature.animation.Play("start", 1);
        Debug.Log("播放 start 动画");

        var animationState = boxArmature.animation.Play("start", 1);
        while (!animationState.isCompleted)
        {
            yield return null;
        }
        // 播放 "stay" 动画循环
        boxArmature.animation.Play("stay", 0);
        Debug.Log("播放 stay 动画");
        if(!PlayInforManager.Instance.playInfor.FirstZeroToOne)
        {
            // 显示按钮
            ContibueBtn_F.gameObject.SetActive(true);
            ReturnBtn_F.gameObject.SetActive(true);
        }
        else
        {
            InitialFailReturnBtn_F.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// 处理继续按钮点击事件的协程
    /// </summary>
    private IEnumerator OnContibueBtn_FClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(ContibueBtn_F.GetComponent<RectTransform>(), OnContinueClicked));
    }

    /// <summary>
    /// 处理返回主页按钮点击事件的协程
    /// </summary>
    private IEnumerator OnReturnBtn_FClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(ReturnBtn_F.GetComponent<RectTransform>(), OnReturnClicked));
    }
    private IEnumerator OnInitialFailReturnBtn_FClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(InitialFailReturnBtn_F.GetComponent<RectTransform>(), OnReturnClicked));
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
        GameManage.Instance.InitialPalyer();
        AccountManager.Instance.SaveAccountData();
        UIManager.Instance.ChangeState(GameState.Ready);
        boxArmature.animation.Play("<None>", 0);
        Destroy(gameObject);
    }
}
