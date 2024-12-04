using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanelController : UIBase
{
    public Text rewardText;//奖励数字文字
    public Button claimX3Button;//观看广告奖励乘3按钮
    public Button claimNowButton;//不乘3选择返回按钮
    public TurnTablePanelContoller turnTablePanelContoller;
    private ReadyPanelController readypanelController;
    void Start()
    {
        GetAllChild(transform);
        turnTablePanelContoller = FindObjectOfType<TurnTablePanelContoller>(); 
        readypanelController = FindObjectOfType<ReadyPanelController>();
        rewardText = childDic["RewardText_F"].GetComponent<Text>();
        claimX3Button = childDic["ClaimAdBtn_F"].GetComponent<Button>();
        claimNowButton = childDic["ClaimRuturnBtn_F"].GetComponent<Button>();
        // 初始时隐藏 claimNowButton
        AnimateRewardText(turnTablePanelContoller.currentReward, 0f, 2f, rewardText);
        //rewardText.text = "×" + turnTablePanelContoller.currentReward.ToString();
        claimNowButton.gameObject.SetActive(false);
        // 启动异步任务，在3秒后显示 claimNowButton
        StartCoroutine(ShowClaimNowButtonWithDelay(3f));

        //claimX3Button.onClick.AddListener(OnClaimX3ButtonClick);
        //claimNowButton.onClick.AddListener(OnClaimNowButtonClick);
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // 获取 ClickAMature 对象及其动画组件
        GetClickAnim(transform);
        // 修改按钮点击事件监听器
        claimX3Button.onClick.AddListener(() => StartCoroutine(OnclaimX3ButtonClicked()));
        claimNowButton.onClick.AddListener(() => StartCoroutine(OnclaimNowButtonClicked()));
    }

    /// <summary>
    /// 处理签到按钮点击事件的协程
    /// </summary>
    private IEnumerator OnclaimX3ButtonClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(claimX3Button.GetComponent<RectTransform>(), OnClaimX3ButtonClick));
    }
    private IEnumerator OnclaimNowButtonClicked()
    {
        // 播放点击动画
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // 执行按钮弹跳动画并调用后续逻辑
        yield return StartCoroutine(ButtonBounceAnimation(claimNowButton.GetComponent<RectTransform>(), OnClaimNowButtonClick));
    }



    /// <summary>
    /// 异步方法，用于延迟显示 claimNowButton
    /// </summary>
    /// <param name="delaySeconds">延迟的秒数</param>
    /// <returns></returns>
    private IEnumerator ShowClaimNowButtonWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        claimNowButton.gameObject.SetActive(true);
    }
    void OnClaimX3ButtonClick()
    {
        PlayInforManager.Instance.playInfor.AddCoins((int)(turnTablePanelContoller.currentReward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total * 3));
        readypanelController.totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        gameObject.SetActive(false);
        turnTablePanelContoller.UpdateButtonState();
        Destroy(gameObject); 
        //TTOD1使用异步播放金币动画
        //AdManager.Instance.ShowRewardedAd(() => {
        //    PlayerData.Instance.AddCoins(currentReward * 3);
        //    rewardPanel.SetActive(false);
        //    // 可以在这里添加奖励领取成功的提示
        //});
    }
    void OnClaimNowButtonClick()
    {
        PlayInforManager.Instance.playInfor.AddCoins((int)(turnTablePanelContoller.currentReward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total));
        readypanelController.totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        gameObject.SetActive(false);
        turnTablePanelContoller.UpdateButtonState();
        Destroy(gameObject);    
        //TTOD1使用异步播放金币动画
        // StartCoroutine(PlayCoinCollectAnimation());
    }

    //IEnumerator PlayCoinCollectAnimation()
    //{
    //    // 实现金币收集动画，例如使用LeanTween
    //    // 假设有一个金币Prefab和目标位置
    //    GameObject coinPrefab = Resources.Load<GameObject>("CoinPrefab"); // 确保在Resources文件夹下有CoinPrefab
    //    Transform target = pointer.transform; // 将金币飞向指针位置

    //    int coinCount = 10;
    //    for (int i = 0; i < coinCount; i++)
    //    {
    //        GameObject coin = Instantiate(coinPrefab, rewardPanel.transform.position, Quaternion.identity);
    //        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50), 0);
    //        Vector3 destination = target.position + randomOffset;

    //        // 使用LeanTween移动
    //        LeanTween.move(coin, destination, 1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
    //            Destroy(coin);
    //        });

    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}
}
