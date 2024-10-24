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
    private GameObject CheckUIPanel; // 签到面板
    private GameObject TurnTablePanel; // 转盘面板
                                     // 用于金币动画的引用
    public UnityEngine.Transform coinStartTransform; // 通过 Inspector 指定或动态获取
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
        // 判断是否每日是否首次登录
        UpdateRedNote();
        StartGameBtn.onClick.AddListener(OnStartGameButtonClicked);
        CheckBtn.onClick.AddListener(OnCheckonClicked);
        TurntableBtn.onClick.AddListener(OnWheelonClicked);
        // 初始化金币显示
        UpdateTotalCoinsUI(AccountManager.Instance.GetTotalCoins());
    }
    public void UpdateRedNote()
    {
        // 如果玩家可以签到，则显示 RedNoteImg，否则隐藏
        RedNoteImg.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        //CheckBtn.gameObject.SetActive(AccountManager.Instance.CanSignIn());
        TurntableRedNoteImg.gameObject.SetActive(AccountManager.Instance.CanSignIn());
       //TurntableBtn.gameObject.SetActive(AccountManager.Instance.CanSignIn());
    }
    // Update is called once per frame
    void Update()
    {

    }

    // 按钮点击时调用的方法
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
    /// 使用动画效果更新总金币的 UI
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
    /// 数字滚动动画的协程
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
    /// 处理金币的动画
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
    //    // 获取主画布
    //    Canvas canvas = GetComponentInParent<Canvas>();
    //    if (canvas == null)
    //    {
    //        Debug.LogError("主画布未找到！");
    //        return;
    //    }

    //    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    //    Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

    //    for (int i = 0; i < coinBase; i++)
    //    {
    //        // 实例化金币预制体，并将其父对象设置为主画布
    //        GameObject coinObj = Instantiate(Resources.Load<GameObject>("Prefabs/GoldCanvas"));
    //        if (coinObj == null)
    //        {
    //            Debug.LogError("GoldCanvas 预制体未找到或无法实例化！");
    //            continue;
    //        }

    //        // 获取 RectTransform 组件
    //        RectTransform coinRect = coinObj.GetComponent<RectTransform>();
    //        if (coinRect == null)
    //        {
    //            Debug.LogError("GoldCanvas 预制体缺少 RectTransform 组件！");
    //            Destroy(coinObj);
    //            continue;
    //        }

    //        // 将世界坐标转换为画布的局部坐标
    //        Vector2 localStartPosition;
    //        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
    //            RectTransformUtility.WorldToScreenPoint(uiCamera, startPosition),
    //            uiCamera, out localStartPosition);
    //        coinRect.anchoredPosition = localStartPosition;

    //        // 播放金币动画（如果有）
    //        UnityArmatureComponent coinArmature = coinObj.transform.GetComponent<UnityArmatureComponent>();
    //        if (coinArmature != null)
    //        {
    //            coinArmature.animation.Play("newAnimation", -1);
    //        }

    //        // 获取金币脚本并启动移动协程
    //        Gold gold = coinObj.transform.GetComponent<Gold>();
    //        if (gold != null)
    //        {
    //            gold.StartMoveCoin(coinObj, endPosition, 1f, canvasRect, uiCamera);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("金币预制体缺少 Gold 脚本！");
    //            Destroy(coinObj); // 如果缺少脚本，销毁金币对象
    //            continue;
    //        }
    //        // 每个金币之间的延迟
    //        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
    //    }
    public async UniTask AnimateCoin(Vector3 startPosition, Vector3 endPosition, int coinBase)
    {
        // 获取主画布
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("主画布未找到！");
            return;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        // 将 endPosition 从世界坐标转换为屏幕坐标，再转换为画布的局部坐标
        Vector2 localEndPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
            RectTransformUtility.WorldToScreenPoint(uiCamera, endPosition),
            uiCamera, out localEndPosition);

        for (int i = 0; i < coinBase; i++)
        {
            // 实例化金币预制体，并将其父对象设置为主画布
            GameObject coinObj = Instantiate(Resources.Load<GameObject>("Prefabs/GoldCanvas"), canvas.transform);
            if (coinObj == null)
            {
                Debug.LogError("GoldCanvas 预制体未找到或无法实例化！");
                continue;
            }

            // 获取 RectTransform 组件
            RectTransform coinRect = coinObj.transform.GetComponent<RectTransform>();
            if (coinRect == null)
            {
                Debug.LogError("GoldCanvas 预制体缺少 RectTransform 组件！");
                Destroy(coinObj);
                continue;
            }

            // 将世界坐标 startPosition 转换为画布的局部坐标
            Vector2 localStartPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                RectTransformUtility.WorldToScreenPoint(uiCamera, startPosition),
                uiCamera, out localStartPosition);
            coinRect.anchoredPosition = localStartPosition;

            // 播放金币动画（如果有）
            UnityArmatureComponent coinArmature = coinObj.transform.GetChild(1).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }

            // 获取金币脚本并启动移动协程
            Gold gold = coinObj.transform.GetChild(1).GetComponent<Gold>();
            if (gold != null)
            {
                gold.StartMoveCoin(coinRect, localEndPosition, 1f); // 假设 StartMoveCoin 接受 RectTransform 和局部 endPosition
            }
            else
            {
                Debug.LogWarning("金币预制体缺少 Gold 脚本！");
                Destroy(coinObj); // 如果缺少脚本，销毁金币对象
                continue;
            }

            // 每个金币之间的延迟
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
    }
}
