using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DragonBones;
using Transform = UnityEngine.Transform;
using System.Collections.Generic;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using System;
using Sequence = DG.Tweening.Sequence;
using Random = UnityEngine.Random;

public class SuccessPanelController : UIBase
{
    [Header("UI References")]
    public Text coinsText;
    public Vector3 chestTransform;
    public Transform coinsPrePar;
    public Transform coinsTextTransform;
    public Button drawButton;
    public Button returnButton;
    public Button claimButton;
    public Image arrow; 
    public List<Transform> multipliers = new List<Transform>(); // 5个倍数Text对象
    public float moveDuration = 1f; // Text横向刷动的动画持续时间
    public float rotationDuration = 2f; // 箭头旋转的持续时间

    [Header("Reward Settings")]
    public int rewardAmount;
    public int TypeChestIndex;
    public int coinCount = 20;
    public int resueCoinAll;
    private List<int> probabilities = new List<int>(); // 存储概率值
    void Start()
    {
        //开始宝箱弹跳动画
        //chestAnimation.PlayStayAnimation();
        gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        GetAllChild(transform);
        coinsPrePar = childDic["CoinAnimation_F"].transform;
        chestTransform = childDic["CoinAnimation_F"].GetComponent<RectTransform>().anchoredPosition;
        coinsText = childDic["CoinText_F"].GetComponent<Text>();
        drawButton = childDic["DrawMultiplierBtn_F"].GetComponent<Button>();
        returnButton = childDic["ReturnHomeButton_F"].GetComponent<Button>();
        claimButton = childDic["ClaimButton_F"].GetComponent<Button>();
        arrow = childDic["wheelJiantou_F"].GetComponent<Image>();
        for (int i = 1; i <= 5; i++)
        {
            string nameText = "Multiplier" + i + "_F";
            multipliers.Add(childDic[nameText].transform);
        }
        resueCoinAll = (int)(ConfigManager.Instance.Tables.TableGlobal.Get(15).IntValue * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total);
        claimButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
        arrow.transform.parent.gameObject.SetActive(false);

        ////设置按钮监听
        //drawButton.onClick.AddListener(OnDrawButtonClicked);
        //returnButton.onClick.AddListener(OnClaimButtonClicked);
        //claimButton.onClick.AddListener(OnAdClaimButtonClicked);

        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // 获取 ClickAMature 对象及其动画组件
        GetClickAnim(transform);
        // 修改按钮点击事件监听器
        drawButton.onClick.AddListener(() => StartCoroutine(OndrawButtonClicked()));
        returnButton.onClick.AddListener(() => StartCoroutine(OnreturnButtonClicked()));
        claimButton.onClick.AddListener(() => StartCoroutine(OnclaimButtonClicked()));
    }

    /// <summary>
    /// 处理签到按钮点击事件的协程
    /// </summary>
    private IEnumerator OndrawButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(drawButton.GetComponent<RectTransform>(), OnDrawButtonClicked));
    }
    private IEnumerator OnreturnButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(returnButton.GetComponent<RectTransform>(), OnClaimButtonClicked));
    }
    private IEnumerator OnclaimButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(claimButton.GetComponent<RectTransform>(), OnAdClaimButtonClicked));
    }




    public void OnChestOpenComplete(int CoinBase, int TypeIndex)
    {
        coinsText.text = CoinBase.ToString();
        TypeChestIndex = TypeIndex;

        //根据TypeIndex设置倍数和概率
        SetMultipliers(TypeIndex);
        SetProbabilities(TypeIndex);
        //显示转盘UI
        ShowTurntable();
    }

    void OnDrawButtonClicked()
    {
        returnButton.onClick.RemoveListener(OnDrawButtonClicked);
        drawButton.onClick.RemoveListener(OnDrawButtonClicked);
        returnButton.gameObject.SetActive(false);
        OnAdWatched(true);
    }

    void OnAdWatched(bool success)
    {
        if (success)
        {
            //显示领取按钮
            Debug.Log("观看广告完成，显示领取按钮");
            drawButton.gameObject.SetActive(false);
            claimButton.gameObject.SetActive(true);
        }
        else
        {
            //广告未完成，显示返回按钮
            returnButton.gameObject.SetActive(true);
        }
    }

    void OnClaimButtonClicked()
    {
        returnButton.onClick.RemoveListener(OnClaimButtonClicked);
        StartCoroutine(ClaimAfterDelay(3f));
    }

    void OnAdClaimButtonClicked()
    {
        claimButton.onClick.RemoveListener(OnAdClaimButtonClicked);
        GenerateAndMoveCoins();
        StartCoroutine(ClaimAfterDelay(3f));
    }

    IEnumerator ClaimAfterDelay(float delay)
    {
        GenerateAndMoveCoinsCoroutine(gameMainPanelController);
        yield return new WaitForSeconds(delay);
        ReturnToHome();
    }

    public void ReturnToHome()
    {
        //实现返回主页逻辑，例如加载主菜单场景
        Destroy(gameMainPanelController.gameObject);
        //TTOD1此处添加本关所解锁的枪的类型\
        // 插入当前关卡解锁的枪的类型到AllGunName的开头
        string gunType = $"{ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation}-{ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Star}";
        if (!PlayInforManager.Instance.AllGunName.Contains(gunType))
        {
            PlayInforManager.Instance.AllGunName.Insert(0, gunType); // 使用Insert(0, gunType)将新武器类型插入到列表的开头

        }
        //每种武器对应子弹资源存储于字典中
        string bulletType = ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource;
        PlayInforManager.Instance.GunToBulletMap[gunType] = bulletType;
        GameFlowManager.Instance.currentLevelIndex++;
        PlayInforManager.Instance.playInfor.level = GameFlowManager.Instance.currentLevelIndex;
        //TTOD1暂时切换最新关卡数据的枪
        PlayInforManager.Instance.playInfor.SetGun(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation, ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.ReplaceGunDragon();

        AccountManager.Instance.SaveAccountData();
        UIManager.Instance.ChangeState(GameState.Ready);
        Destroy(gameObject);
    }

    public void ShowTurntable()
    {
        arrow.transform.parent.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(true);
        //让5个倍数Text对象横向来回刷动
        foreach (Transform multiplier in multipliers)
        {
            multiplier.DOLocalMoveX(multiplier.localPosition.x + 50, moveDuration).SetLoops(-1, LoopType.Yoyo);
        }
    }

    //根据TypeIndex设置每个倍数的值

    void SetMultipliers(int TypeIndex)
    {
        for (int i = 0; i < multipliers.Count; i++)
        {
            float multiplierValue = CalculateMultiplierValue(TypeIndex, i);
            multipliers[i].GetChild(0).GetComponent<Text>().text = multiplierValue.ToString();
        }
    }

    void SetProbabilities(int TypeIndex)
    {
        probabilities = ConfigManager.Instance.Tables.TableSettlementConfig.Get(TypeIndex).Probability;
    }

    float CalculateMultiplierValue(int TypeIndex, int index)
    {
        //根据TypeIndex和index计算倍数值
        return ConfigManager.Instance.Tables.TableSettlementConfig.Get(TypeIndex).Multiplier[index]; // 示例倍数计算公式
    }
    public GameMainPanelController gameMainPanelController;
    private async UniTask GenerateAndMoveCoins()
    {
        await GenerateAndMoveCoinsCoroutine(gameMainPanelController);
        // 等待所有金币移动完成
        await UniTask.Delay(TimeSpan.FromSeconds(2f), ignoreTimeScale: true); // 根据移动时间调整延迟
    }

    private async UniTask GenerateAndMoveCoinsCoroutine(GameMainPanelController gameMainPanelController)
    {
        bool isPlayTextAni = false;
        GameObject coinPrefab = Resources.Load<GameObject>("Prefabs/Coin/newgold");
        if (coinPrefab == null)
        {
            Debug.LogError("找不到金币Prefab！");
        }

        int coinCount = 5;
        for (int i = 0; i < coinCount; i++)
        {
            // 随机偏移量，使金币从不同位置弹出
            Vector3 randomOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            Vector3 spawnPosition = returnButton.GetComponent<RectTransform>().position + randomOffset;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform.parent);
            // 播放动画
            UnityArmatureComponent coinArmature = coin.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }
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
            // 使用 DOTween 移动金币到小偏移位置，然后飞向目标位置
            Vector3 intermediatePosition = spawnPosition + new Vector3(Random.Range(-80f, 80f), Random.Range(-80f, 80f), 0);

            float moveDuration1 = 0.25f;
            float moveDuration2 = 1f;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(coin.transform.DOMove(intermediatePosition, moveDuration1).SetEase(Ease.OutQuad));
            sequence.Append(coin.transform.DOMove(gameMainPanelController.coinspattern_F.position, moveDuration2).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                Destroy(coin);
            });
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), ignoreTimeScale: true);
        }
        // 动画完成后销毁奖励面板
        //yield return new WaitForSeconds(2f);
        //for (int i = 1; i <= coinCount; i++)
        //{
        //    string CoinName = "NewGold";
        //    if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
        //    {
        //        GameObject coinObj = selectedCoinPool.Get();
        //        coinObj.SetActive(true);
        //        RectTransform coinRect = coinObj.GetComponent<RectTransform>();
        //        coinRect.anchoredPosition = claimButton.GetComponent<RectTransform>().anchoredPosition;
        //        // 播放动画
        //        UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        //        if (coinArmature != null)
        //        {
        //            coinArmature.animation.Play("newAnimation", -1);
        //        }
        //        // 获取Gold组件并启动移动逻辑
        //        Gold gold = coinObj.GetComponent<Gold>();
        //        gold.AwaitMovePanel(new Vector3(-210.5f, 745f, 0), 0.5f);
        //    }
        //    if (!isPlayTextAni)
        //    {
        //        isPlayTextAni = true;
        //        // 计算新的金币数
        //        //int newCoinTotal = resueCoinAll - coinCount;
        //        if (gameMainPanelController != null)
        //        {
        //            gameMainPanelController.UpdateCoinTextWithDOTween(resueCoinAll);
        //        }
        //    }
        //    // 等待0.05秒后继续生成下一个金币
        //    await UniTask.Delay(TimeSpan.FromSeconds(0.05f), ignoreTimeScale: true);
        //}
    }
}
