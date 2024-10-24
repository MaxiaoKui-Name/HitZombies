using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanelController : UIBase
{
    public TextMeshProUGUI rewardText;//奖励数字文字
    public Button claimX3Button;//观看广告奖励乘3按钮
    public Button claimNowButton;//不乘3选择返回按钮
    public TurnTablePanelContoller turnTablePanelContoller;
    void Start()
    {
        GetAllChild(transform);
        turnTablePanelContoller = FindObjectOfType<TurnTablePanelContoller>();
        rewardText = childDic["RewardText_F"].GetComponent<TextMeshProUGUI>();
        claimX3Button = childDic["ClaimAdBtn_F"].GetComponent<Button>();
        claimNowButton = childDic["ClaimRuturnBtn_F"].GetComponent<Button>();
        // 初始时隐藏 claimNowButton
        rewardText.text = turnTablePanelContoller.currentReward.ToString();
        claimNowButton.gameObject.SetActive(false);
        // 启动异步任务，在3秒后显示 claimNowButton
        StartCoroutine(ShowClaimNowButtonWithDelay(3f));
        claimX3Button.onClick.AddListener(OnClaimX3ButtonClick);
        claimNowButton.onClick.AddListener(OnClaimNowButtonClick);
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
        PlayInforManager.Instance.playInfor.AddCoins(turnTablePanelContoller.currentReward * 3);
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
        //PlayerData.Instance.AddCoins(currentReward);
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
