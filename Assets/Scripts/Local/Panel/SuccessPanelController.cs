using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DragonBones;
using Transform = UnityEngine.Transform;
using System.Collections.Generic;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using System;
using Sequence = DG.Tweening.Sequence;
using Random = UnityEngine.Random;

public class SuccessPanelController : UIBase
{
    [Header("UI References")]
    public Text coinsText;
    public Vector3 chestTransform;
    public Transform coinsPrePar;
    public Transform coinsTextTransform;
    public Button drawButton;
    public Button returnButton;
    public Button claimButton;
    public Image arrow; 
    public List<Transform> multipliers = new List<Transform>(); // 5������Text����
    public float moveDuration = 1f; // Text����ˢ���Ķ�������ʱ��
    public float rotationDuration = 2f; // ��ͷ��ת�ĳ���ʱ��

    [Header("Reward Settings")]
    public int rewardAmount;
    public int TypeChestIndex;
    public int coinCount = 20;
    public int resueCoinAll;
    private List<int> probabilities = new List<int>(); // �洢����ֵ
    void Start()
    {
        //��ʼ���䵯������
        //chestAnimation.PlayStayAnimation();
        gameMainPanelController = FindObjectOfType<GameMainPanelController>();
        GetAllChild(transform);
        coinsPrePar = childDic["CoinAnimation_F"].transform;
        chestTransform = childDic["CoinAnimation_F"].GetComponent<RectTransform>().anchoredPosition;
        coinsText = childDic["CoinText_F"].GetComponent<Text>();
        drawButton = childDic["DrawMultiplierBtn_F"].GetComponent<Button>();
        returnButton = childDic["ReturnHomeButton_F"].GetComponent<Button>();
        claimButton = childDic["ClaimButton_F"].GetComponent<Button>();
        arrow = childDic["wheelJiantou_F"].GetComponent<Image>();
        for (int i = 1; i <= 5; i++)
        {
            string nameText = "Multiplier" + i + "_F";
            multipliers.Add(childDic[nameText].transform);
        }
        resueCoinAll = (int)(ConfigManager.Instance.Tables.TableGlobal.Get(15).IntValue * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total);
        claimButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
        arrow.transform.parent.gameObject.SetActive(false);

        ////���ð�ť����
        //drawButton.onClick.AddListener(OnDrawButtonClicked);
        //returnButton.onClick.AddListener(OnClaimButtonClicked);
        //claimButton.onClick.AddListener(OnAdClaimButtonClicked);

        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // ��ȡ ClickAMature �����䶯�����
        GetClickAnim(transform);
        // �޸İ�ť����¼�������
        drawButton.onClick.AddListener(() => StartCoroutine(OndrawButtonClicked()));
        returnButton.onClick.AddListener(() => StartCoroutine(OnreturnButtonClicked()));
        claimButton.onClick.AddListener(() => StartCoroutine(OnclaimButtonClicked()));
    }

    /// <summary>
    /// ����ǩ����ť����¼���Э��
    /// </summary>
    private IEnumerator OndrawButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(drawButton.GetComponent<RectTransform>(), OnDrawButtonClicked));
    }
    private IEnumerator OnreturnButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(returnButton.GetComponent<RectTransform>(), OnClaimButtonClicked));
    }
    private IEnumerator OnclaimButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));

        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(claimButton.GetComponent<RectTransform>(), OnAdClaimButtonClicked));
    }




    public void OnChestOpenComplete(int CoinBase, int TypeIndex)
    {
        coinsText.text = CoinBase.ToString();
        TypeChestIndex = TypeIndex;

        //����TypeIndex���ñ����͸���
        SetMultipliers(TypeIndex);
        SetProbabilities(TypeIndex);
        //��ʾת��UI
        ShowTurntable();
    }

    void OnDrawButtonClicked()
    {
        returnButton.onClick.RemoveListener(OnDrawButtonClicked);
        drawButton.onClick.RemoveListener(OnDrawButtonClicked);
        returnButton.gameObject.SetActive(false);
        OnAdWatched(true);
    }

    void OnAdWatched(bool success)
    {
        if (success)
        {
            //��ʾ��ȡ��ť
            Debug.Log("�ۿ������ɣ���ʾ��ȡ��ť");
            drawButton.gameObject.SetActive(false);
            claimButton.gameObject.SetActive(true);
        }
        else
        {
            //���δ��ɣ���ʾ���ذ�ť
            returnButton.gameObject.SetActive(true);
        }
    }

    void OnClaimButtonClicked()
    {
        returnButton.onClick.RemoveListener(OnClaimButtonClicked);
        StartCoroutine(ClaimAfterDelay(3f));
    }

    void OnAdClaimButtonClicked()
    {
        claimButton.onClick.RemoveListener(OnAdClaimButtonClicked);
        GenerateAndMoveCoins();
        StartCoroutine(ClaimAfterDelay(3f));
    }

    IEnumerator ClaimAfterDelay(float delay)
    {
        GenerateAndMoveCoinsCoroutine(gameMainPanelController);
        yield return new WaitForSeconds(delay);
        ReturnToHome();
    }

    public void ReturnToHome()
    {
        //ʵ�ַ�����ҳ�߼�������������˵�����
        Destroy(gameMainPanelController.gameObject);
        //TTOD1�˴���ӱ�����������ǹ������\
        // ���뵱ǰ�ؿ�������ǹ�����͵�AllGunName�Ŀ�ͷ
        string gunType = $"{ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation}-{ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Star}";
        if (!PlayInforManager.Instance.AllGunName.Contains(gunType))
        {
            PlayInforManager.Instance.AllGunName.Insert(0, gunType); // ʹ��Insert(0, gunType)�����������Ͳ��뵽�б�Ŀ�ͷ

        }
        //ÿ��������Ӧ�ӵ���Դ�洢���ֵ���
        string bulletType = ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource;
        PlayInforManager.Instance.GunToBulletMap[gunType] = bulletType;
        GameFlowManager.Instance.currentLevelIndex++;
        PlayInforManager.Instance.playInfor.level = GameFlowManager.Instance.currentLevelIndex;
        //TTOD1��ʱ�л����¹ؿ����ݵ�ǹ
        PlayInforManager.Instance.playInfor.SetGun(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation, ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.ReplaceGunDragon();

        AccountManager.Instance.SaveAccountData();
        UIManager.Instance.ChangeState(GameState.Ready);
        Destroy(gameObject);
    }

    public void ShowTurntable()
    {
        arrow.transform.parent.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(true);
        //��5������Text�����������ˢ��
        foreach (Transform multiplier in multipliers)
        {
            multiplier.DOLocalMoveX(multiplier.localPosition.x + 50, moveDuration).SetLoops(-1, LoopType.Yoyo);
        }
    }

    //����TypeIndex����ÿ��������ֵ

    void SetMultipliers(int TypeIndex)
    {
        for (int i = 0; i < multipliers.Count; i++)
        {
            float multiplierValue = CalculateMultiplierValue(TypeIndex, i);
            multipliers[i].GetChild(0).GetComponent<Text>().text = multiplierValue.ToString();
        }
    }

    void SetProbabilities(int TypeIndex)
    {
        probabilities = ConfigManager.Instance.Tables.TableSettlementConfig.Get(TypeIndex).Probability;
    }

    float CalculateMultiplierValue(int TypeIndex, int index)
    {
        //����TypeIndex��index���㱶��ֵ
        return ConfigManager.Instance.Tables.TableSettlementConfig.Get(TypeIndex).Multiplier[index]; // ʾ���������㹫ʽ
    }
    public GameMainPanelController gameMainPanelController;
    private async UniTask GenerateAndMoveCoins()
    {
        await GenerateAndMoveCoinsCoroutine(gameMainPanelController);
        // �ȴ����н���ƶ����
        await UniTask.Delay(TimeSpan.FromSeconds(2f), ignoreTimeScale: true); // �����ƶ�ʱ������ӳ�
    }

    private async UniTask GenerateAndMoveCoinsCoroutine(GameMainPanelController gameMainPanelController)
    {
        bool isPlayTextAni = false;
        GameObject coinPrefab = Resources.Load<GameObject>("Prefabs/Coin/newgold");
        if (coinPrefab == null)
        {
            Debug.LogError("�Ҳ������Prefab��");
        }

        int coinCount = 5;
        for (int i = 0; i < coinCount; i++)
        {
            // ���ƫ������ʹ��ҴӲ�ͬλ�õ���
            Vector3 randomOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            Vector3 spawnPosition = returnButton.GetComponent<RectTransform>().position + randomOffset;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform.parent);
            // ���Ŷ���
            UnityArmatureComponent coinArmature = coin.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }
            if (!isPlayTextAni)
            {
                isPlayTextAni = true;
                // �����µĽ����
                //int newCoinTotal = resueCoinAll - coinCount;
                if (gameMainPanelController != null)
                {
                    gameMainPanelController.UpdateCoinTextWithDOTween(resueCoinAll);
                }
            }
            // ʹ�� DOTween �ƶ���ҵ�Сƫ��λ�ã�Ȼ�����Ŀ��λ��
            Vector3 intermediatePosition = spawnPosition + new Vector3(Random.Range(-80f, 80f), Random.Range(-80f, 80f), 0);

            float moveDuration1 = 0.25f;
            float moveDuration2 = 1f;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(coin.transform.DOMove(intermediatePosition, moveDuration1).SetEase(Ease.OutQuad));
            sequence.Append(coin.transform.DOMove(gameMainPanelController.coinspattern_F.position, moveDuration2).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                Destroy(coin);
            });
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), ignoreTimeScale: true);
        }
        // ������ɺ����ٽ������
        //yield return new WaitForSeconds(2f);
        //for (int i = 1; i <= coinCount; i++)
        //{
        //    string CoinName = "NewGold";
        //    if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
        //    {
        //        GameObject coinObj = selectedCoinPool.Get();
        //        coinObj.SetActive(true);
        //        RectTransform coinRect = coinObj.GetComponent<RectTransform>();
        //        coinRect.anchoredPosition = claimButton.GetComponent<RectTransform>().anchoredPosition;
        //        // ���Ŷ���
        //        UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        //        if (coinArmature != null)
        //        {
        //            coinArmature.animation.Play("newAnimation", -1);
        //        }
        //        // ��ȡGold����������ƶ��߼�
        //        Gold gold = coinObj.GetComponent<Gold>();
        //        gold.AwaitMovePanel(new Vector3(-210.5f, 745f, 0), 0.5f);
        //    }
        //    if (!isPlayTextAni)
        //    {
        //        isPlayTextAni = true;
        //        // �����µĽ����
        //        //int newCoinTotal = resueCoinAll - coinCount;
        //        if (gameMainPanelController != null)
        //        {
        //            gameMainPanelController.UpdateCoinTextWithDOTween(resueCoinAll);
        //        }
        //    }
        //    // �ȴ�0.05������������һ�����
        //    await UniTask.Delay(TimeSpan.FromSeconds(0.05f), ignoreTimeScale: true);
        //}
    }
}
