using DragonBones;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyPanelController : UIBase
{
    public Button StartGameBtn;
    public UIManager uIManager;
    public Button CheckBtn;
    public TextMeshProUGUI totalCoinsText;
    private GameObject CheckUIPanel; // 初始化页面
                                     // 用于金币动画的引用
    public UnityEngine.Transform coinStartTransform; // 通过 Inspector 指定或动态获取
    private Coroutine coinAnimationCoroutine;
    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        GetAllChild(transform);
        StartGameBtn = childDic["ReadystartGame_F"].GetComponent<Button>();
        CheckBtn = childDic["CheckBtn_F"].GetComponent<Button>();
        totalCoinsText = childDic["totalCoinsText_F"].GetComponent<TextMeshProUGUI>();
        //totalCoinsText.text = 
        // 为按钮添加监听
        StartGameBtn.onClick.AddListener(OnStartGameButtonClicked);
        CheckBtn.onClick.AddListener(OnCheckonClicked);

        // 初始化金币显示
        UpdateTotalCoinsUI(AccountManager.Instance.GetTotalCoins());
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
            CheckUIPanel.transform.SetParent(transform, false);
            CheckUIPanel.transform.localPosition = Vector3.zero;
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
        int start = PlayInforManager.Instance.playInfor.totalCoins - reward;
        int end = PlayInforManager.Instance.playInfor.totalCoins;
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
    public void AnimateCoin(Vector3 startPosition, Vector3 endPosition)
    {
        string CoinName = "gold";
        if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
        {
            GameObject coinObj = selectedCoinPool.Get();
            coinObj.transform.position = startPosition;
            UnityArmatureComponent coinArmature = coinObj.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
            if (coinArmature != null)
            {
                coinArmature.animation.Play("newAnimation", -1);
            }
            Gold gold = coinObj.GetComponent<Gold>();
            gold.StartMoveCoin(selectedCoinPool, coinObj, endPosition, 1f);
        }
    }
}
