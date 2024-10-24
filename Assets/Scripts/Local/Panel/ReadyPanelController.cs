using Cysharp.Threading.Tasks;
using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class ReadyPanelController : UIBase
{
    public Button StartGameBtn;
    public UIManager uIManager;
    public Button CheckBtn;
    public Button TurntableBtn;
    public TextMeshProUGUI totalCoinsText;
    private GameObject CheckUIPanel; // ǩ�����
    private GameObject TurnTablePanel; // ת�����
                                     // ���ڽ�Ҷ���������
    public UnityEngine.Transform coinStartTransform; // ͨ�� Inspector ָ����̬��ȡ
    private Coroutine coinAnimationCoroutine;
    public Image RedNoteImg;
    public Image TurntableRedNoteImg;
    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        GetAllChild(transform);
        RedNoteImg = childDic["RedNote_F"].GetComponent<Image>();
        TurntableRedNoteImg  = childDic["TurntableRedNote_F"].GetComponent<Image>();
        StartGameBtn = childDic["ReadystartGame_F"].GetComponent<Button>();
        TurntableBtn = childDic["TurntableBtn_F "].GetComponent<Button>();
        CheckBtn = childDic["CheckBtn_F"].GetComponent<Button>();
        totalCoinsText = childDic["totalCoinsText_F"].GetComponent<TextMeshProUGUI>();
        totalCoinsText.text = PlayInforManager.Instance.playInfor.coinNum.ToString();
        // �ж��Ƿ�ÿ���Ƿ��״ε�¼
        UpdateRedNote();
        StartGameBtn.onClick.AddListener(OnStartGameButtonClicked);
        CheckBtn.onClick.AddListener(OnCheckonClicked);
        TurntableBtn.onClick.AddListener(OnWheelonClicked);
        // ��ʼ�������ʾ
        UpdateTotalCoinsUI(AccountManager.Instance.GetTotalCoins());
    }
    public void UpdateRedNote()
    {
        // �����ҿ���ǩ��������ʾ RedNoteImg����������
        RedNoteImg.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        //CheckBtn.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        TurntableRedNoteImg.gameObject.SetActive(AccountManager.Instance.CanSignIn());
       //TurntableBtn.gameObject.SetActive(AccountManager.Instance.CanSignIn());
    }
    // Update is called once per frame
    void Update()
    {

    }

    // ��ť���ʱ���õķ���
    void OnStartGameButtonClicked()
    {
        if(LevelManager.Instance.levelData != null)
        {
            StartGameBtn.gameObject.SetActive(false);
            uIManager.ChangeState(GameState.Running);
            InfiniteScroll.Instance.baseScrollSpeed = 0.5f;// ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
            InfiniteScroll.Instance.baseGrowthRate = InfiniteScroll.Instance.baseScrollSpeed / 40;
            LevelManager.Instance.LoadScene("First", 0);
        }
     
    }
    void OnCheckonClicked()
    {
        if (LevelManager.Instance.levelData != null)
        {
            CheckUIPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/CheckUIPanel"));
            CheckUIPanel.transform.SetParent(transform.parent, false);
            CheckUIPanel.transform.localPosition = Vector3.zero;
        }
    }
    void OnWheelonClicked()
    {
        if (LevelManager.Instance.levelData != null)
        {
            TurnTablePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/TurnTablePanel"));
            TurnTablePanel.transform.SetParent(transform.parent, false);
            TurnTablePanel.transform.localPosition = Vector3.zero;
        }
    }
    /// <summary>
    /// ʹ�ö���Ч�������ܽ�ҵ� UI
    /// </summary>
    public void UpdateTotalCoinsUI(int reward)
    {
        if (coinAnimationCoroutine != null)
        {
            StopCoroutine(coinAnimationCoroutine);
        }
        int start = (int)(PlayInforManager.Instance.playInfor.coinNum - reward);
        int end = (int)(PlayInforManager.Instance.playInfor.coinNum);
        coinAnimationCoroutine = StartCoroutine(RollingNumber(totalCoinsText, start, end, 1f));
    }

    /// <summary>
    /// ���ֹ���������Э��
    /// </summary>
    private IEnumerator RollingNumber(TextMeshProUGUI textMesh, int start, int end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int current = Mathf.RoundToInt(Mathf.Lerp(start, end, t));
            textMesh.text = current.ToString();
            yield return null;
        }
        textMesh.text = end.ToString();
    }

    /// <summary>
    /// �����ҵĶ���
    /// </summary>
    //public async UniTask AnimateCoin(Vector3 startPosition, Vector3 endPosition, int coinBase)
    //{
    //    string CoinName = "gold";
    //    for (int i = 0; i < coinBase; i++)
    //    {
    //        GameObject coinObj = Instantiate(Resources.Load<GameObject>("Prefabs/gold"));
    //        coinObj.transform.position = startPosition;
    //        UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
    //        if (coinArmature != null)
    //        {
    //            coinArmature.animation.Play("newAnimation", -1);
    //        }
    //        Gold gold = coinObj.GetComponent<Gold>();
    //        await gold.StartMoveCoin(coinObj, endPosition, 1f);
    //        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
    //    }
    //}

    //public async UniTask AnimateCoin(Vector3 startPosition, Vector3 endPosition, int coinBase)
    //{
    //    // ��ȡ������
    //    Canvas canvas = GetComponentInParent<Canvas>();
    //    if (canvas == null)
    //    {
    //        Debug.LogError("������δ�ҵ���");
    //        return;
    //    }

    //    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    //    Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

    //    for (int i = 0; i < coinBase; i++)
    //    {
    //        // ʵ�������Ԥ���壬�����丸��������Ϊ������
    //        GameObject coinObj = Instantiate(Resources.Load<GameObject>("Prefabs/GoldCanvas"));
    //        if (coinObj == null)
    //        {
    //            Debug.LogError("GoldCanvas Ԥ����δ�ҵ����޷�ʵ������");
    //            continue;
    //        }

    //        // ��ȡ RectTransform ���
    //        RectTransform coinRect = coinObj.GetComponent<RectTransform>();
    //        if (coinRect == null)
    //        {
    //            Debug.LogError("GoldCanvas Ԥ����ȱ�� RectTransform �����");
    //            Destroy(coinObj);
    //            continue;
    //        }

    //        // ����������ת��Ϊ�����ľֲ�����
    //        Vector2 localStartPosition;
    //        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
    //            RectTransformUtility.WorldToScreenPoint(uiCamera, startPosition),
    //            uiCamera, out localStartPosition);
    //        coinRect.anchoredPosition = localStartPosition;

    //        // ���Ž�Ҷ���������У�
    //        UnityArmatureComponent coinArmature = coinObj.transform.GetComponent<UnityArmatureComponent>();
    //        if (coinArmature != null)
    //        {
    //            coinArmature.animation.Play("newAnimation", -1);
    //        }

    //        // ��ȡ��ҽű��������ƶ�Э��
    //        Gold gold = coinObj.transform.GetComponent<Gold>();
    //        if (gold != null)
    //        {
    //            gold.StartMoveCoin(coinObj, endPosition, 1f, canvasRect, uiCamera);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("���Ԥ����ȱ�� Gold �ű���");
    //            Destroy(coinObj); // ���ȱ�ٽű������ٽ�Ҷ���
    //            continue;
    //        }
    //        // ÿ�����֮����ӳ�
    //        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
    //    }
    public async UniTask AnimateCoin(Vector3 startPosition, Vector3 endPosition, int coinBase)
    {
        // ��ȡ������
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("������δ�ҵ���");
            return;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        // �� endPosition ����������ת��Ϊ��Ļ���꣬��ת��Ϊ�����ľֲ�����
        Vector2 localEndPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
            RectTransformUtility.WorldToScreenPoint(uiCamera, endPosition),
            uiCamera, out localEndPosition);

        for (int i = 0; i < coinBase; i++)
        {
            // ʵ�������Ԥ���壬�����丸��������Ϊ������
            GameObject coinObj = Instantiate(Resources.Load<GameObject>("Prefabs/GoldCanvas"), canvas.transform);
            if (coinObj == null)
            {
                Debug.LogError("GoldCanvas Ԥ����δ�ҵ����޷�ʵ������");
                continue;
            }

            // ��ȡ RectTransform ���
            RectTransform coinRect = coinObj.transform.GetComponent<RectTransform>();
            if (coinRect == null)
            {
                Debug.LogError("GoldCanvas Ԥ����ȱ�� RectTransform �����");
                Destroy(coinObj);
                continue;
            }

            // ���������� startPosition ת��Ϊ�����ľֲ�����
            Vector2 localStartPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                RectTransformUtility.WorldToScreenPoint(uiCamera, startPosition),
                uiCamera, out localStartPosition);
            coinRect.anchoredPosition = localStartPosition;

            // ���Ž�Ҷ���������У�
            UnityArmatureComponent coinArmature = coinObj.transform.GetChild(1).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }

            // ��ȡ��ҽű��������ƶ�Э��
            Gold gold = coinObj.transform.GetChild(1).GetComponent<Gold>();
            if (gold != null)
            {
                gold.StartMoveCoin(coinRect, localEndPosition, 1f); // ���� StartMoveCoin ���� RectTransform �;ֲ� endPosition
            }
            else
            {
                Debug.LogWarning("���Ԥ����ȱ�� Gold �ű���");
                Destroy(coinObj); // ���ȱ�ٽű������ٽ�Ҷ���
                continue;
            }

            // ÿ�����֮����ӳ�
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
    }
}
