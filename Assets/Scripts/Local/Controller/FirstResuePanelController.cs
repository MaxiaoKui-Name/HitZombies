using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FirstResuePanelController : UIBase
{
    // 复活面板的UI元素
    public TextMeshProUGUI countdownNum_F;              // 倒计时显示
    public Button WatchAdBtn_F;              // 观看广告按钮
    public Button CloseBtn;                   // 返回按钮
    public Text CoinNumText;                 // 显示金币数量的文本

    // 闯关失败面板
    public GameObject levelFailedPanel;      // 闯关失败面板
    public GameObject ResuePanel;      // 闯关失败面板
    // 倒计时变量
    private float countdown = 60f;            // 60秒倒计时
    private bool isCounting = false;          // 是否正在计时

    void Start()
    {
        GetAllChild(transform);
        countdownNum_F = childDic["FirstResuecountdownNum_F"].GetComponent<TextMeshProUGUI>();
        WatchAdBtn_F = childDic["FirstResueWatchAdBtn_F"].GetComponent<Button>();
        CloseBtn = childDic["FirstResueCloseBtn_F"].GetComponent<Button>();
        CoinNumText = childDic["FirstResueCoinNumText_F"].GetComponent<Text>(); 
        // TTOD1显示金币数字待读表
        ShowRevivePanel((int)(ConfigManager.Instance.Tables.TableGlobal.Get(15).IntValue * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total));
        //WatchAdBtn_F.onClick.AddListener(OnWatchAdClicked);
        //CloseBtn.onClick.AddListener(OnCloseClicked);
        // 初始化面板缩放为0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // 获取 ClickAMature 对象及其动画组件
        GetClickAnim(transform);
        // 修改按钮点击事件监听器
        WatchAdBtn_F.onClick.AddListener(() => StartCoroutine(OnWatchAdBtn_FClicked()));
        CloseBtn.onClick.AddListener(() => StartCoroutine(OnCloseBtnClicked()));
    
    }

    /// <summary>
    /// 处理签到按钮点击事件的协程
    /// </summary>
    private IEnumerator OnWatchAdBtn_FClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(WatchAdBtn_F.GetComponent<RectTransform>(), OnWatchAdClicked));
    }
    private IEnumerator OnCloseBtnClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(CloseBtn.GetComponent<RectTransform>(), OnCloseClicked));
    }

    void Update()
    {
        if (isCounting)
        {
            countdown -= Time.unscaledDeltaTime; // 使用 unscaledDeltaTime
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

    // 显示复活面板并开始倒计时
    public void ShowRevivePanel(int coinCount)
    {
        AnimateRewardText(coinCount, 0f, 2f, CoinNumText);
        countdown = 60f;
        isCounting = true;
        UpdateCountdownUI();
    }

    // 更新倒计时UI显示
    private void UpdateCountdownUI()
    {
        int displayTime = Mathf.CeilToInt(countdown);
        countdownNum_F.text = displayTime.ToString();
    }

    // 倒计时结束后的处理
    private void OnCountdownFinished()
    {
        ShowLevelFailedPanel();
        Destroy(gameObject);
    }

    // 观看广告按钮点击处理
    private void OnWatchAdClicked()
    {
        // 停止倒计时
        isCounting = false;
        // 模拟观看广告的过程，这里可以集成真实的广告SDK
        StartCoroutine(WatchAdCoroutine());
    }

    // 模拟观看广告的协程
    private IEnumerator WatchAdCoroutine()
    {
        // 显示广告加载提示（可选）
        Debug.Log("广告开始播放...");
        // 假设广告播放需要5秒
        yield return new WaitForSecondsRealtime(1f);
        // 广告播放完成后的处理
        Debug.Log("广告播放完成，玩家复活。");
        ShowResuePanel();
        Destroy(gameObject);
        // 在这里可以添加复活逻辑，例如重置玩家位置、生命等
    }

    // 返回按钮点击处理
    private void OnCloseClicked()
    {
        // 停止倒计时
        isCounting = false;
        ShowLevelFailedPanel();
        Destroy(gameObject);
    }

    // 显示闯关失败面板
    private void ShowLevelFailedPanel()
    {
        UIManager.Instance.ChangeState(GameState.GameOver);
        EventDispatcher.instance.DispatchEvent(EventNameDef.GAME_OVER);
        GameManage.Instance.GameOverReset();
    }

    private void ShowResuePanel()
    {
        ResuePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/ResuePanel"));
        ResuePanel.transform.SetParent(transform.parent, false);
        ResuePanel.transform.localPosition = Vector3.zero;
    }
}
