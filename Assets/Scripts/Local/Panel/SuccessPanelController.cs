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
    public Transform chestTransform;
    public Transform coinsPrePar;
    public Transform coinsTextTransform;
    //public turntableManager
    [Header("Reward Settings")]
    public int rewardAmount = 100; // 宝箱奖励数值


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
            multiplierTexts.Add(childDic["nameText"].gameObject);
        }
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

    public void OnChestOpenComplete(int originalAmount)
    {
        coinsText.text = originalAmount.ToString();
        // 显示转盘UI
        ShowTurntable();
    }

    IEnumerator FlyCoins()
    {
        // 实例化金币
        string CoinName = "gold";
        if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject coin = Instantiate(selectedCoinPool.Get(), chestTransform.position, Quaternion.identity, coinsPrePar);
                Gold gold = coin.GetComponent<Gold>();
                coin.GetComponent<UnityArmatureComponent>().animation.Play("newAnimation", -1);
                // 使用DOTween移动金币到金币Text位置
                yield return new WaitForSeconds(0.5f); // 等待动画开始
                coin.transform.DOMove(coinsText.GetComponent<RectTransform>().anchoredPosition, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    Destroy(coin);
                });
            }
        }
        // 更新金币Text
        StartCoroutine(AnimateCoinsText(0, rewardAmount, 1f));
        claimButton.onClick.RemoveListener(OnDrawButtonClicked);
        claimButton.gameObject.SetActive(false);
    }

    IEnumerator AnimateCoinsText(int start, int end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int current = Mathf.RoundToInt(Mathf.Lerp(start, end, elapsed / duration));
            coinsText.text = current.ToString();
            yield return null;
        }
        coinsText.text = end.ToString();
    }

    public void UpdateCoinsWithMultiplier(int multiplier)
    {
        int originalAmount = rewardAmount;
        int targetAmount = originalAmount * multiplier;
        StartCoroutine(AnimateCoinsText(originalAmount, targetAmount, 1f));
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
        UIManager.Instance.ChangeState(GameState.Ready, GameFlowManager.Instance.currentLevelIndex);
        Destroy(gameObject);
    }
    public void ShowTurntable()
    {
        arrow.transform.parent.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(true);
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
            // 播放转盘旋转动画
            PlayTurntableAnimation(selectedMultiplierIndex);
            // 更新金币数值
            UpdateCoinsWithMultiplier(selectedMultiplier);
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
        StartCoroutine(FlyCoins());
    }

    void PlayTurntableAnimation(int targetIndex)
    {
        float anglePerSegment = 360f / multipliers.Length;
        float targetAngle = 360f * 3 + targetIndex * anglePerSegment; // 旋转3圈加上目标位置

        // 使用DOTween旋转转盘
        transform.DORotate(new Vector3(0, 0, -targetAngle), rotationDuration, RotateMode.FastBeyond360)
                 .SetEase(Ease.OutCubic)
                 .OnComplete(() =>
                 {
                     // 确保箭头指向正确位置
                     arrow.transform.DOLocalRotate(new Vector3(0, 0, targetIndex * anglePerSegment), 0.5f);
                 });

        // 播放转盘上文字的移动动画
        foreach (var rect in multiplierRects)
        {
            rect.DOAnchorPosX(0, 0.5f).SetDelay(rotationDuration).SetEase(Ease.OutBack);
        }
    }
}
