using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ResuePanelController : UIBase
{
    [Header("UI元素")]
    public Text ResueCoinNumText_F;      // 显示金币数量的文本
    public Button ResueResueBtn_F;       // 复活按钮
    public Transform CoinTarget_F;       // 金币移动的目标位置
    public GameObject CoinPrefab;        // 金币预制体，包含龙骨动画

    // 金币生成数量
    private int coinCount = 10;
    void Start()
    {
        GetAllChild(transform);
        ResueCoinNumText_F = childDic["ResueCoinNumText_F"].GetComponent<Text>();
        ResueResueBtn_F = childDic["ResueResueBtn_F"].GetComponent<Button>();
        CoinTarget_F = childDic["CoinTarget_F"].transform;
        // 添加按钮点击事件监听
        if (ResueResueBtn_F != null)
        {
            ResueResueBtn_F.onClick.AddListener(OnResueBtnClicked);
        }
        else
        {
            Debug.LogError("ResueResueBtn_F 未设置！");
        }
        //TTOD1复活显示的金币数待读表
        ResueCoinNumText_F.text = "9789261e8";
    }

    /// <summary>
    /// 复活按钮点击处理
    /// </summary>
    private void OnResueBtnClicked()
    {
        // 禁用按钮，防止重复点击
        ResueResueBtn_F.interactable = false;
        // 开始生成并移动金币
        //StartCoroutine(GenerateAndMoveCoins());
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.Init();
        GameManage.Instance.SwitchState(GameState.Running);
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    /// <summary>
    /// 生成金币并移动的协程
    /// </summary>
    private IEnumerator GenerateAndMoveCoins()
    {
        // 获取起始位置（ResueCoinNumText_F的位置）
        Vector3 startPos = ResueCoinNumText_F.transform.position;

        // 获取目标位置（CoinTarget_F的位置）
        Vector3 targetPos = CoinTarget_F.position;

        // 记录已完成的金币数量
        int completedCoins = 0;

        for (int i = 0; i < coinCount; i++)
        {
            string CoinName = "gold";
            if (PreController.Instance.CoinPools.TryGetValue(CoinName, out var selectedCoinPool))
            {
                GameObject coinObj = selectedCoinPool.Get();
                coinObj.SetActive(true);
                coinObj.transform.SetParent(transform.parent);
                coinObj.transform.position = startPos;
                var dragonBonesComponent = coinObj.transform.GetChild(0).GetComponent<DragonBones.UnityArmatureComponent>();
                if (dragonBonesComponent != null)
                {
                    dragonBonesComponent.animation.Play("newAnimation", -1);
                }
                //// 生成金币
                //GameObject coin = Instantiate(CoinPrefab, startPos, Quaternion.identity, transform.parent);

                // 确保CoinPrefab包含龙骨动画，并自动播放
                // 如果需要手动触发动画，可以在此处添加相关代码
                // 开始移动金币的协程
                StartCoroutine(MoveCoin(coinObj, targetPos, () =>
                {
                    // 金币移动完成后的回调
                    completedCoins++;
                    // 销毁金币对象
                    Destroy(coinObj);
                    // 如果所有金币都完成移动，销毁面板
                }));

                // 为了让金币分批生成，避免一次性生成过多导致性能问题，可以添加延迟
                yield return new WaitForSeconds(0.05f); // 每隔0.05秒生成一个金币
            }
        }
        if (completedCoins >= coinCount)
        {
            GameManage.Instance.SwitchState(GameState.Running);
            Time.timeScale = 1;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 移动金币的协程
    /// </summary>
    /// <param name="coin">要移动的金币对象</param>
    /// <param name="target">目标位置</param>
    /// <param name="onComplete">移动完成后的回调</param>
    private IEnumerator MoveCoin(GameObject coin, Vector3 target, System.Action onComplete)
    {
        // 飞行持续时间
        float duration = 0.5f; // 1秒
        // 记录经过的时间
        float elapsed = 0f;
        // 起始位置
        Vector3 start = coin.transform.position;
        // 运动曲线（可以根据需求调整）
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = curve.Evaluate(t);
            // 使用贝塞尔曲线或其他更复杂的路径可以实现更自然的飞行轨迹
            // 这里简单使用线性插值
            coin.transform.position = Vector3.Lerp(start, target, curveValue);
            yield return null;
        }
        // 确保金币到达目标位置
        coin.transform.position = target;
        // 调用完成回调
        onComplete?.Invoke();
    }
}
