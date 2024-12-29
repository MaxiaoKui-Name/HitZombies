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
        StartCoroutine(AnimateRewardText(totalReward, 0f, 3f, rewardText));
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
        StartCoroutine(AnimateRewardText(totalReward,0f,3f, rewardText));
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
        AudioManage.Instance.PlaySFX("button", null);
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
        AudioManage.Instance.PlaySFX("button", null);
        // ���㵱ǰ����
        float totalMultiplier = ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
        long rewardToAdd = (long)(turnTablePanelContoller.currentReward * totalMultiplier);
        // ������Ҷ���
        CreateCoins(rewardToAdd);
        turnTablePanelContoller.UpdateButtonState();
    }

    // �첽���Ž�Ҷ���
    void CreateCoins(long rewardToAdd)
    {
        // ��ȡ RewardImg ��Ӧ�� Transform
        string imgName = $"RewardImg{currentRewardSegment + 1}_F";
        Transform rewardImgTransform = transform.Find($"RewardImg_F/{imgName}");
        if (rewardImgTransform == null)
        {
            Debug.LogError($"�Ҳ�������ͼƬ��{imgName}");
        }
        // Ŀ��λ�ã��� readyPanel.TotalCoinImg_F ��λ��
        Transform target = readypanelController.TotalCoinImg_F.transform;
        StartCoroutine(AnimateUGUICoins(rewardImgTransform, target,transform.gameObject, rewardToAdd,2f, readypanelController.totalCoinsText));
       
    }

   

   
}
