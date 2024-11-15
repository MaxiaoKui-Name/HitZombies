using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Cysharp.Threading.Tasks;
using DragonBones;
using Transform = UnityEngine.Transform;
using JetBrains.Annotations;
public class ResuePanelController : UIBase
{
    [Header("UI元素")]
    public Text ResueCoinNumText_F;      // 显示金币数量的文本
    public Button ResueResueBtn_F;       // 复活按钮
    public Transform CoinTarget_F;       // 金币移动的目标位置
    public GameObject CoinPrefab;        // 金币预制体，包含龙骨动画
    public int resueCoinAll;
    // 金币生成数量
    private int coinCount = 20;
    void Start()
    {
        GetAllChild(transform);
        ResueCoinNumText_F = childDic["ResueCoinNumText_F"].GetComponent<Text>();
        ResueResueBtn_F = childDic["ResueResueBtn_F"].GetComponent<Button>();
        CoinTarget_F = childDic["CoinTarget_F"].transform;
        // 添加按钮点击事件监听
        if (ResueResueBtn_F != null)
        {
            ResueResueBtn_F.onClick.AddListener(OnResueBtnClicked);
        }
        else
        {
            Debug.LogError("ResueResueBtn_F 未设置！");
        }
        resueCoinAll = (int)(ConfigManager.Instance.Tables.TableGlobal.Get(15).IntValue * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total);
        //TTOD1复活显示的金币数待读表
        ResueCoinNumText_F.text = $"{resueCoinAll:N0}";
    }

    /// <summary>
    /// 复活按钮点击处理
    /// </summary>
    private void OnResueBtnClicked()
    {
        // 禁用按钮，防止重复点击
        ResueResueBtn_F.interactable = false;
        // 开始生成并移动金币
        GenerateAndMoveCoins();
    }

    private async UniTask GenerateAndMoveCoins()
    {
        GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
      
        await GenerateAndMoveCoinsCoroutine(gameMainPanelController);
        // 等待所有金币移动完成
        await UniTask.Delay(TimeSpan.FromSeconds(3f), ignoreTimeScale: true); // 根据移动时间调整延迟
        // 复活逻辑
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.Init();
        GameManage.Instance.SwitchState(GameState.Running);
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    private async UniTask GenerateAndMoveCoinsCoroutine(GameMainPanelController gameMainPanelController)
    {
        bool isPlayTextAni = false;
        for (int i = 1; i <= coinCount; i++)
        {
            string CoinName = "NewGold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.SetActive(true);
                RectTransform coinRect = coinObj.GetComponent<RectTransform>();
                coinRect.anchoredPosition = ResueResueBtn_F.GetComponent<RectTransform>().anchoredPosition;
                // 播放动画
                UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                if (coinArmature != null)
                {
                    coinArmature.animation.Play("newAnimation", -1);
                }
                // 获取Gold组件并启动移动逻辑
                Gold gold = coinObj.GetComponent<Gold>();
                gold.AwaitMovePanel(new Vector3(-210.5F,745F,0), 0.5f);
                if (!isPlayTextAni)
                {
                    isPlayTextAni = true;
                    // 计算新的金币数
                    //int newCoinTotal = resueCoinAll - coinCount;
                    if (gameMainPanelController != null)
                    {
                        gameMainPanelController.UpdateCoinTextWithDOTween(resueCoinAll);
                    }
                }
                // 等待0.05秒后继续生成下一个金币
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), ignoreTimeScale: true);
            }
        }
    }

}
