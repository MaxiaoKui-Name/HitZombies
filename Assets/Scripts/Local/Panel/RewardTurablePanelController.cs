using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RewardTurablePanelController : UIBase
{
    public Text rewardText; // 奖励数字文字
    public Button claimX3Button; // 观看广告奖励乘3按钮
    public Button claimNowButton; // 不乘3选择返回按钮
    public TurnTablePanelContoller turnTablePanelContoller;
    private ReadyPanelController readypanelController;

    // 奖励图片数组
    public GameObject[] rewardImages = new GameObject[6];
    // 当前奖励段
    public int currentRewardSegment;

    void Start()
    {
        GetAllChild(transform);
        turnTablePanelContoller = FindObjectOfType<TurnTablePanelContoller>();
        readypanelController = FindObjectOfType<ReadyPanelController>();
        rewardText = childDic["RewardText_F"].GetComponent<Text>();
        claimX3Button = childDic["ClaimAdBtn_F"].GetComponent<Button>();
        claimNowButton = childDic["ClaimRuturnBtn_F"].GetComponent<Button>();

        InitializeRewardImages();

        // 初始时显示对应奖励图片，隐藏其他图片
        DisplayRewardImage(currentRewardSegment);

        // 使用自定义的动画显示奖励数字
        float totalReward = turnTablePanelContoller.currentReward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        StartCoroutine(AnimateRewardText(totalReward, 0f, 3f, rewardText));
        claimNowButton.gameObject.SetActive(false);
        // 启动异步任务，在3秒后显示 claimNowButton
        StartCoroutine(ShowClaimNowButtonWithDelay(3f));
        claimX3Button.onClick.AddListener(OnClaimX3ButtonClick);
        claimNowButton.onClick.AddListener(OnClaimNowButtonClick);
    }

    // 新增方法，用于在 ShowRewardPanelCoroutine 中调用
    public void InitializeReward()
    {
        // 确保 rewardImages 已经初始化
        InitializeRewardImages();

        // 显示对应奖励图片，隐藏其他图片
        DisplayRewardImage(currentRewardSegment);

        // 使用自定义的动画显示奖励数字
        float totalReward = turnTablePanelContoller.currentReward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        StartCoroutine(AnimateRewardText(totalReward,0f,3f, rewardText));
        claimNowButton.gameObject.SetActive(false);
        // 启动异步任务，在3秒后显示 claimNowButton
        StartCoroutine(ShowClaimNowButtonWithDelay(3f));
    }

    void InitializeRewardImages()
    {
        for (int i = 0; i < 6; i++)
        {
            string imgName = $"RewardImg{i + 1}_F";
            // 假设 RewardImg_F 是 RewardTurablePanel 的子对象，并且 RewardImg1_F 到 RewardImg6_F 是其子对象
            Transform imgTransform = transform.Find($"RewardImg_F/{imgName}");
            if (imgTransform != null)
            {
                rewardImages[i] = imgTransform.gameObject;
            }
            else
            {
                Debug.LogError($"找不到奖励图片：{imgName}");
            }
        }
    }

    // 显示对应的奖励图片，隐藏其他图片
    void DisplayRewardImage(int segment)
    {
        for (int i = 0; i < rewardImages.Length; i++)
        {
            if (i == segment)
            {
                rewardImages[i].SetActive(true);
            }
            else
            {
                rewardImages[i].SetActive(false);
            }
        }
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
        AudioManage.Instance.PlaySFX("button", null);
        // 计算乘3后的奖励
        float totalMultiplier = ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        float rewardToAdd = turnTablePanelContoller.currentReward * totalMultiplier * 3;
        PlayInforManager.Instance.playInfor.AddCoins((int)rewardToAdd);
        readypanelController.totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        gameObject.SetActive(false);
        turnTablePanelContoller.UpdateButtonState();
        Destroy(gameObject);
        // 这里可以添加播放广告的逻辑
    }

    void OnClaimNowButtonClick()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // 计算当前奖励
        float totalMultiplier = ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        long rewardToAdd = (long)(turnTablePanelContoller.currentReward * totalMultiplier);
        // 启动金币动画
        CreateCoins(rewardToAdd);
        turnTablePanelContoller.UpdateButtonState();
    }

    // 异步播放金币动画
    void CreateCoins(long rewardToAdd)
    {
        // 获取 RewardImg 对应的 Transform
        string imgName = $"RewardImg{currentRewardSegment + 1}_F";
        Transform rewardImgTransform = transform.Find($"RewardImg_F/{imgName}");
        if (rewardImgTransform == null)
        {
            Debug.LogError($"找不到奖励图片：{imgName}");
        }
        // 目标位置，即 readyPanel.TotalCoinImg_F 的位置
        Transform target = readypanelController.TotalCoinImg_F.transform;
        StartCoroutine(AnimateUGUICoins(rewardImgTransform, target,transform.gameObject, rewardToAdd,2f, readypanelController.totalCoinsText));
       
    }

   

   
}
