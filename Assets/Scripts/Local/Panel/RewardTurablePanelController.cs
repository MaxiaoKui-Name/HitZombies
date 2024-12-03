using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RewardTurablePanelController : UIBase
{
    public Text rewardText; // ������������
    public Button claimX3Button; // �ۿ���潱����3��ť
    public Button claimNowButton; // ����3ѡ�񷵻ذ�ť
    public TurnTablePanelContoller turnTablePanelContoller;
    private ReadyPanelController readypanelController;

    // ����ͼƬ����
    public GameObject[] rewardImages = new GameObject[6];
    // ��ǰ������
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

        // ��ʼʱ��ʾ��Ӧ����ͼƬ����������ͼƬ
        DisplayRewardImage(currentRewardSegment);

        // ʹ���Զ���Ķ�����ʾ��������
        float totalReward = turnTablePanelContoller.currentReward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        StartCoroutine(AnimateRewardText(totalReward, 2f));

        claimNowButton.gameObject.SetActive(false);
        // �����첽������3�����ʾ claimNowButton
        StartCoroutine(ShowClaimNowButtonWithDelay(3f));
        claimX3Button.onClick.AddListener(OnClaimX3ButtonClick);
        claimNowButton.onClick.AddListener(OnClaimNowButtonClick);
    }

    // ���������������� ShowRewardPanelCoroutine �е���
    public void InitializeReward()
    {
        // ȷ�� rewardImages �Ѿ���ʼ��
        InitializeRewardImages();

        // ��ʾ��Ӧ����ͼƬ����������ͼƬ
        DisplayRewardImage(currentRewardSegment);

        // ʹ���Զ���Ķ�����ʾ��������
        float totalReward = turnTablePanelContoller.currentReward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        StartCoroutine(AnimateRewardText(totalReward, 3f));

        claimNowButton.gameObject.SetActive(false);
        // �����첽������3�����ʾ claimNowButton
        StartCoroutine(ShowClaimNowButtonWithDelay(3f));
    }

    void InitializeRewardImages()
    {
        for (int i = 0; i < 6; i++)
        {
            string imgName = $"RewardImg{i + 1}_F";
            // ���� RewardImg_F �� RewardTurablePanel ���Ӷ��󣬲��� RewardImg1_F �� RewardImg6_F �����Ӷ���
            Transform imgTransform = transform.Find($"RewardImg_F/{imgName}");
            if (imgTransform != null)
            {
                rewardImages[i] = imgTransform.gameObject;
            }
            else
            {
                Debug.LogError($"�Ҳ�������ͼƬ��{imgName}");
            }
        }
    }

    // ��ʾ��Ӧ�Ľ���ͼƬ����������ͼƬ
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
    /// �첽�����������ӳ���ʾ claimNowButton
    /// </summary>
    /// <param name="delaySeconds">�ӳٵ�����</param>
    /// <returns></returns>
    private IEnumerator ShowClaimNowButtonWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        claimNowButton.gameObject.SetActive(true);
    }

    void OnClaimX3ButtonClick()
    {
        // �����3��Ľ���
        float totalMultiplier = ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        float rewardToAdd = turnTablePanelContoller.currentReward * totalMultiplier * 3;
        PlayInforManager.Instance.playInfor.AddCoins((int)rewardToAdd);
        readypanelController.totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        gameObject.SetActive(false);
        turnTablePanelContoller.UpdateButtonState();
        Destroy(gameObject);
        // ���������Ӳ��Ź����߼�
    }

    void OnClaimNowButtonClick()
    {
        // ���㵱ǰ����
        float totalMultiplier = ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        float rewardToAdd = turnTablePanelContoller.currentReward * totalMultiplier;
        PlayInforManager.Instance.playInfor.AddCoins((int)rewardToAdd);
        readypanelController.totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        turnTablePanelContoller.UpdateButtonState();
        // ������Ҷ���
        StartCoroutine(AnimateCoins());
    }

    // �첽���Ž�Ҷ���
    IEnumerator AnimateCoins()
    {
        // ��ȡ RewardImg ��Ӧ�� Transform
        string imgName = $"RewardImg{currentRewardSegment + 1}_F";
        Transform rewardImgTransform = transform.Find($"RewardImg_F/{imgName}");
        if (rewardImgTransform == null)
        {
            Debug.LogError($"�Ҳ�������ͼƬ��{imgName}");
            yield break;
        }

        // Ŀ��λ�ã��� readyPanel.TotalCoinImg_F ��λ��
        Transform target = readypanelController.TotalCoinImg_F.transform;

        // ���ؽ�� Prefab
        GameObject coinPrefab = Resources.Load<GameObject>("Prefabs/Coin/newgold");
        if (coinPrefab == null)
        {
            Debug.LogError("�Ҳ������Prefab��");
            yield break;
        }

        int coinCount = 5;
        for (int i = 0; i < coinCount; i++)
        {
            // ���ƫ������ʹ��ҴӲ�ͬλ�õ���
            Vector3 randomOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            Vector3 spawnPosition = rewardImgTransform.position + randomOffset;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform.parent);

            // ʹ�� DOTween �ƶ���ҵ�Сƫ��λ�ã�Ȼ�����Ŀ��λ��
            Vector3 intermediatePosition = spawnPosition + new Vector3(Random.Range(-80f, 80f), Random.Range(-80f, 80f), 0);

            float moveDuration1 = 0.25f;
            float moveDuration2 = 1f;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(coin.transform.DOMove(intermediatePosition, moveDuration1).SetEase(Ease.OutQuad));
            sequence.Append(coin.transform.DOMove(target.position, moveDuration2).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                Destroy(coin);
            });

            yield return new WaitForSeconds(0.2f);
        }

        // ������ɺ����ٽ������
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    // �Զ�������ֹ��������������� Slot ��
    private float displayedValue = 0f;

    IEnumerator AnimateRewardText(float targetValue, float duration)
    {
        // ������ʾֵ
        displayedValue = 0f;

        // ʹ��DOTween����һ����0��targetValue�Ķ���
        Tween tween = DOTween.To(() => displayedValue, x => {
            displayedValue = x;
            rewardText.text = Mathf.FloorToInt(displayedValue).ToString();
        }, targetValue, duration)
        .SetEase(Ease.OutCubic) // ʹ�û���������ʹ�����
        .SetUpdate(true); // ȷ��������Time.timeScaleΪ0ʱ��Ȼ���У������Ҫ��

        yield return tween.WaitForCompletion();

        // ȷ������ֵ׼ȷ
        rewardText.text = targetValue.ToString();
    }

}
