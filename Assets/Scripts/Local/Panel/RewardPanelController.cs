using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanelController : UIBase
{
    public Text rewardText;//������������
    public Button claimX3Button;//�ۿ���潱����3��ť
    public Button claimNowButton;//����3ѡ�񷵻ذ�ť
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
        // ��ʼʱ���� claimNowButton
        AnimateRewardText(turnTablePanelContoller.currentReward, 0f, 2f, rewardText);
        //rewardText.text = "��" + turnTablePanelContoller.currentReward.ToString();
        claimNowButton.gameObject.SetActive(false);
        // �����첽������3�����ʾ claimNowButton
        StartCoroutine(ShowClaimNowButtonWithDelay(3f));

        //claimX3Button.onClick.AddListener(OnClaimX3ButtonClick);
        //claimNowButton.onClick.AddListener(OnClaimNowButtonClick);
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // ��ȡ ClickAMature �����䶯�����
        GetClickAnim(transform);
        // �޸İ�ť����¼�������
        claimX3Button.onClick.AddListener(() => StartCoroutine(OnclaimX3ButtonClicked()));
        claimNowButton.onClick.AddListener(() => StartCoroutine(OnclaimNowButtonClicked()));
    }

    /// <summary>
    /// ����ǩ����ť����¼���Э��
    /// </summary>
    private IEnumerator OnclaimX3ButtonClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(claimX3Button.GetComponent<RectTransform>(), OnClaimX3ButtonClick));
    }
    private IEnumerator OnclaimNowButtonClicked()
    {
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(claimNowButton.GetComponent<RectTransform>(), OnClaimNowButtonClick));
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
        PlayInforManager.Instance.playInfor.AddCoins((int)(turnTablePanelContoller.currentReward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total * 3));
        readypanelController.totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        gameObject.SetActive(false);
        turnTablePanelContoller.UpdateButtonState();
        Destroy(gameObject); 
        //TTOD1ʹ���첽���Ž�Ҷ���
        //AdManager.Instance.ShowRewardedAd(() => {
        //    PlayerData.Instance.AddCoins(currentReward * 3);
        //    rewardPanel.SetActive(false);
        //    // ������������ӽ�����ȡ�ɹ�����ʾ
        //});
    }
    void OnClaimNowButtonClick()
    {
        PlayInforManager.Instance.playInfor.AddCoins((int)(turnTablePanelContoller.currentReward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total));
        readypanelController.totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        gameObject.SetActive(false);
        turnTablePanelContoller.UpdateButtonState();
        Destroy(gameObject);    
        //TTOD1ʹ���첽���Ž�Ҷ���
        // StartCoroutine(PlayCoinCollectAnimation());
    }

    //IEnumerator PlayCoinCollectAnimation()
    //{
    //    // ʵ�ֽ���ռ�����������ʹ��LeanTween
    //    // ������һ�����Prefab��Ŀ��λ��
    //    GameObject coinPrefab = Resources.Load<GameObject>("CoinPrefab"); // ȷ����Resources�ļ�������CoinPrefab
    //    Transform target = pointer.transform; // ����ҷ���ָ��λ��

    //    int coinCount = 10;
    //    for (int i = 0; i < coinCount; i++)
    //    {
    //        GameObject coin = Instantiate(coinPrefab, rewardPanel.transform.position, Quaternion.identity);
    //        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50), 0);
    //        Vector3 destination = target.position + randomOffset;

    //        // ʹ��LeanTween�ƶ�
    //        LeanTween.move(coin, destination, 1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
    //            Destroy(coin);
    //        });

    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}
}
