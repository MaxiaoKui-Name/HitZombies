using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanelController : UIBase
{
    public TextMeshProUGUI rewardText;//������������
    public Button claimX3Button;//�ۿ���潱����3��ť
    public Button claimNowButton;//����3ѡ�񷵻ذ�ť
    public TurnTablePanelContoller turnTablePanelContoller;
    void Start()
    {
        GetAllChild(transform);
        turnTablePanelContoller = FindObjectOfType<TurnTablePanelContoller>();
        rewardText = childDic["RewardText_F"].GetComponent<TextMeshProUGUI>();
        claimX3Button = childDic["ClaimAdBtn_F"].GetComponent<Button>();
        claimNowButton = childDic["ClaimRuturnBtn_F"].GetComponent<Button>();
        // ��ʼʱ���� claimNowButton
        rewardText.text = turnTablePanelContoller.currentReward.ToString();
        claimNowButton.gameObject.SetActive(false);
        // �����첽������3�����ʾ claimNowButton
        StartCoroutine(ShowClaimNowButtonWithDelay(3f));
        claimX3Button.onClick.AddListener(OnClaimX3ButtonClick);
        claimNowButton.onClick.AddListener(OnClaimNowButtonClick);
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
        PlayInforManager.Instance.playInfor.AddCoins(turnTablePanelContoller.currentReward * 3);
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
        //PlayerData.Instance.AddCoins(currentReward);
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
