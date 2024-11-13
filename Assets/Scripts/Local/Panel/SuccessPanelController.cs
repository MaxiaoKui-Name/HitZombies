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
    public int rewardAmount; // ���佱����ֵ


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
            multiplierTexts.Add(childDic[nameText].gameObject);
        }
        chestTransform = childDic["CoinAnimation_F"].GetComponent<RectTransform>().anchoredPosition; 
        coinsText = childDic["CoinText_F"].GetComponent<Text>();
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

    public void OnChestOpenComplete()
    {
        coinsText.text = rewardAmount.ToString();
        // ��ʾת��UI
        ShowTurntable();
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
        // ʵ�ַ�����ҳ�߼�������������˵�����
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
