using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DragonBones;
using Transform = UnityEngine.Transform;
using System.Collections.Generic;
using Unity.VisualScripting;


public class SuccessPanelController : UIBase
{
    [Header("UI References")]
    //public ChestAnimation chestAnimation;
    public Text coinsText;
    public Vector3 chestTransform;
    public Transform coinsPrePar;
    public Transform coinsTextTransform;
    //public turntableManager
    [Header("Reward Settings")]
    public int rewardAmount; // 宝箱奖励数值


    public List<GameObject> multiplierTexts = new List<GameObject>(); // 5个倍数Text
    public Button drawButton;
    public Button returnButton;
    public Button claimButton;
    public Image arrow; // 指向选中的倍数的箭头
    public RectTransform[] multiplierRects; // 5个倍数Text的RectTransform
    public float rotationDuration = 2f;
    public SuccessPanelController successPanelController;

    private int selectedMultiplierIndex;
    private int[] multipliers = { 1, 2, 3, 4, 5 }; // 示例倍数，可以根据需求修改
    void Start()
    {
        // 开始宝箱弹跳动画
        // chestAnimation.PlayStayAnimation();
        GetAllChild(transform);
        coinsPrePar = childDic["CoinAnimation_F"].transform;
        for (int i = 1; i <= 5; i++)
        {
            string nameText = "Multiplier" + i + "_F";
            multiplierTexts.Add(childDic[nameText].gameObject);
        }
        chestTransform = childDic["CoinAnimation_F"].GetComponent<RectTransform>().anchoredPosition; 
        coinsText = childDic["CoinText_F"].GetComponent<Text>();
        drawButton = childDic["DrawMultiplierBtn_F"].GetComponent<Button>();
        returnButton = childDic["ReturnHomeButton_F"].GetComponent<Button>();
        claimButton = childDic["ClaimButton_F"].GetComponent<Button>();
        arrow = childDic["wheelJiantou_F"].GetComponent<Image>();
        // 设置按钮监听
        drawButton.onClick.AddListener(OnDrawButtonClicked);
        returnButton.onClick.AddListener(OnReturnButtonClicked);
        claimButton.onClick.AddListener(OnClaimButtonClicked);
        // 初始状态
        claimButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
        arrow.transform.parent.gameObject.SetActive(false);
    }

    public void OnChestOpenComplete()
    {
        coinsText.text = rewardAmount.ToString();
        // 显示转盘UI
        ShowTurntable();
    }

    

 
   
    
    void OnDrawButtonClicked()
    {
        returnButton.onClick.RemoveListener(OnDrawButtonClicked);
        returnButton.gameObject.SetActive(false);
        OnAdWatched(true);
        // 观看广告
        //AdManager.Instance.ShowRewardedAd(OnAdWatched);
        // 隐藏返回主页按钮
    }

    void OnAdWatched(bool success)
    {
        if (success)
        {
            // 显示领取按钮
            Debug.Log("观看广告完成，显示领取按钮");
            drawButton.onClick.RemoveListener(OnDrawButtonClicked);
            drawButton.gameObject.SetActive(false);
            // 随机选择一个倍数
            selectedMultiplierIndex = Random.Range(0, multipliers.Length);
            int selectedMultiplier = multipliers[selectedMultiplierIndex];
         
            claimButton.gameObject.SetActive(true);
        }
        else
        {
            // 广告未完成，显示返回按钮
            returnButton.gameObject.SetActive(true);
        }
    }

    void OnReturnButtonClicked()
    {
        // 返回主页
        StartCoroutine(ReturnToHomeAfterDelay(0.5f));

    }

    void OnClaimButtonClicked()
    {
        // 播放金币飞行动画
        // StartCoroutine(FlyCoins());
        StartCoroutine(ReturnToHomeAfterDelay(0.5f));
        returnButton.gameObject.SetActive(true);
    }
    IEnumerator ReturnToHomeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToHome();
    }

    public void ReturnToHome()
    {
        // 实现返回主页逻辑，例如加载主菜单场景
        GameFlowManager.Instance.currentLevelIndex++;
        PlayInforManager.Instance.playInfor.level = GameFlowManager.Instance.currentLevelIndex;
        PlayInforManager.Instance.playInfor.SetGun(LevelManager.Instance.levelData.GunBulletList[AccountManager.Instance.GetTransmitID(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0])]);
        AccountManager.Instance.SaveAccountData();
        PlayInforManager.Instance.playInfor.attackSpFac = 0;
        UIManager.Instance.ChangeState(GameState.Ready);
        Destroy(gameObject);
    }
    public void ShowTurntable()
    {
        arrow.transform.parent.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(true);
    }


}
