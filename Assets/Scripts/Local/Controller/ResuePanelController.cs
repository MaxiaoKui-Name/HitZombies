using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Cysharp.Threading.Tasks;
using DragonBones;
using Transform = UnityEngine.Transform;
using JetBrains.Annotations;
public class ResuePanelController : UIBase
{
    [Header("UIԪ��")]
    public Text ResueCoinNumText_F;      // ��ʾ����������ı�
    public Button ResueResueBtn_F;       // ���ť
    public Transform CoinTarget_F;       // ����ƶ���Ŀ��λ��
    public GameObject CoinPrefab;        // ���Ԥ���壬�������Ƕ���
    public int resueCoinAll;
    // �����������
    private int coinCount = 20;
    void Start()
    {
        GetAllChild(transform);
        ResueCoinNumText_F = childDic["ResueCoinNumText_F"].GetComponent<Text>();
        ResueResueBtn_F = childDic["ResueResueBtn_F"].GetComponent<Button>();
        CoinTarget_F = childDic["CoinTarget_F"].transform;
        // ��Ӱ�ť����¼�����
        if (ResueResueBtn_F != null)
        {
            ResueResueBtn_F.onClick.AddListener(OnResueBtnClicked);
        }
        else
        {
            Debug.LogError("ResueResueBtn_F δ���ã�");
        }
        resueCoinAll = (int)(ConfigManager.Instance.Tables.TableGlobal.Get(15).IntValue * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total);
        //TTOD1������ʾ�Ľ����������
        ResueCoinNumText_F.text = $"{resueCoinAll:N0}";
    }

    /// <summary>
    /// ���ť�������
    /// </summary>
    private void OnResueBtnClicked()
    {
        // ���ð�ť����ֹ�ظ����
        ResueResueBtn_F.interactable = false;
        // ��ʼ���ɲ��ƶ����
        GenerateAndMoveCoins();
    }

    private async UniTask GenerateAndMoveCoins()
    {
        GameMainPanelController gameMainPanelController = FindObjectOfType<GameMainPanelController>();
      
        await GenerateAndMoveCoinsCoroutine(gameMainPanelController);
        // �ȴ����н���ƶ����
        await UniTask.Delay(TimeSpan.FromSeconds(3f), ignoreTimeScale: true); // �����ƶ�ʱ������ӳ�
        // �����߼�
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.Init();
        GameManage.Instance.SwitchState(GameState.Running);
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    private async UniTask GenerateAndMoveCoinsCoroutine(GameMainPanelController gameMainPanelController)
    {
        bool isPlayTextAni = false;
        for (int i = 1; i <= coinCount; i++)
        {
            string CoinName = "NewGold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.SetActive(true);
                RectTransform coinRect = coinObj.GetComponent<RectTransform>();
                coinRect.anchoredPosition = ResueResueBtn_F.GetComponent<RectTransform>().anchoredPosition;
                // ���Ŷ���
                UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
                if (coinArmature != null)
                {
                    coinArmature.animation.Play("newAnimation", -1);
                }
                // ��ȡGold����������ƶ��߼�
                Gold gold = coinObj.GetComponent<Gold>();
                gold.AwaitMovePanel(new Vector3(-210.5F,745F,0), 0.5f);
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
                // �ȴ�0.05������������һ�����
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), ignoreTimeScale: true);
            }
        }
    }

}
