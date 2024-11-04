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
    public int rewardAmount = 100; // ���佱����ֵ


    public List<GameObject> multiplierTexts = new List<GameObject>(); // 5������Text
    public Button drawButton;
    public Button returnButton;
    public Button claimButton;
    public Image arrow; // ָ��ѡ�еı����ļ�ͷ
    public RectTransform[] multiplierRects; // 5������Text��RectTransform
    public float rotationDuration = 2f;
    public SuccessPanelController successPanelController;

    private int selectedMultiplierIndex;
    private int[] multipliers = { 1, 2, 3, 4, 5 }; // ʾ�����������Ը��������޸�
    void Start()
    {
        // ��ʼ���䵯������
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
        // ���ð�ť����
        drawButton.onClick.AddListener(OnDrawButtonClicked);
        returnButton.onClick.AddListener(OnReturnButtonClicked);
        claimButton.onClick.AddListener(OnClaimButtonClicked);
        // ��ʼ״̬
        claimButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
        arrow.transform.parent.gameObject.SetActive(false);
    }

    public void OnChestOpenComplete(int originalAmount)
    {
        coinsText.text = originalAmount.ToString();
        // ��ʾת��UI
        ShowTurntable();
    }

    IEnumerator FlyCoins()
    {
        // ʵ�������
        string CoinName = "gold";
        if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject coin = Instantiate(selectedCoinPool.Get(), chestTransform.position, Quaternion.identity, coinsPrePar);
                Gold gold = coin.GetComponent<Gold>();
                coin.GetComponent<UnityArmatureComponent>().animation.Play("newAnimation", -1);
                // ʹ��DOTween�ƶ���ҵ����Textλ��
                yield return new WaitForSeconds(0.5f); // �ȴ�������ʼ
                coin.transform.DOMove(coinsText.GetComponent<RectTransform>().anchoredPosition, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    Destroy(coin);
                });
            }
        }
        // ���½��Text
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
        // ʵ�ַ�����ҳ�߼�������������˵�����
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
        // �ۿ����
        //AdManager.Instance.ShowRewardedAd(OnAdWatched);
        // ���ط�����ҳ��ť
    }

    void OnAdWatched(bool success)
    {
        if (success)
        {
            // ��ʾ��ȡ��ť
            Debug.Log("�ۿ������ɣ���ʾ��ȡ��ť");
            drawButton.onClick.RemoveListener(OnDrawButtonClicked);
            drawButton.gameObject.SetActive(false);
            // ���ѡ��һ������
            selectedMultiplierIndex = Random.Range(0, multipliers.Length);
            int selectedMultiplier = multipliers[selectedMultiplierIndex];
            // ����ת����ת����
            PlayTurntableAnimation(selectedMultiplierIndex);
            // ���½����ֵ
            UpdateCoinsWithMultiplier(selectedMultiplier);
            claimButton.gameObject.SetActive(true);
        }
        else
        {
            // ���δ��ɣ���ʾ���ذ�ť
            returnButton.gameObject.SetActive(true);
        }
    }

    void OnReturnButtonClicked()
    {
        // ������ҳ
        StartCoroutine(ReturnToHomeAfterDelay(0.5f));

    }

    void OnClaimButtonClicked()
    {

        // ���Ž�ҷ��ж���
        StartCoroutine(FlyCoins());
    }

    void PlayTurntableAnimation(int targetIndex)
    {
        float anglePerSegment = 360f / multipliers.Length;
        float targetAngle = 360f * 3 + targetIndex * anglePerSegment; // ��ת3Ȧ����Ŀ��λ��

        // ʹ��DOTween��תת��
        transform.DORotate(new Vector3(0, 0, -targetAngle), rotationDuration, RotateMode.FastBeyond360)
                 .SetEase(Ease.OutCubic)
                 .OnComplete(() =>
                 {
                     // ȷ����ͷָ����ȷλ��
                     arrow.transform.DOLocalRotate(new Vector3(0, 0, targetIndex * anglePerSegment), 0.5f);
                 });

        // ����ת�������ֵ��ƶ�����
        foreach (var rect in multiplierRects)
        {
            rect.DOAnchorPosX(0, 0.5f).SetDelay(rotationDuration).SetEase(Ease.OutBack);
        }
    }
}
