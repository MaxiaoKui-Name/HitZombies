using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnTablePanelContoller : UIBase
{
    public float spinDuration = 3f; // 转动持续时间
    public float spinSpeed = 500f; // 转动速度
    public int segments = 6; // 转盘等分数
    public Button spinButton; // 抽奖按钮
    public Button watchAdButton; // 观看广告按钮
    public Button backButton; // 返回按钮
    public GameObject WheelObj; // 转盘
    public GameObject RewardPanel; // 奖励面板

    public Image pointerImg; // 指针图片
    public TextMeshProUGUI[] segmentTexts; // 存储6个Text组件
    private bool isSpinning = false;
    public int currentReward;

    // 存储每个段的权重
    private List<int> segmentWeights = new List<int>();

    void Start()
    {
        GetAllChild(transform);
        spinButton = childDic["LuckdrawFreeBtn_F"].GetComponent<Button>();
        watchAdButton = childDic["LuckdrawAdBtn_F"].GetComponent<Button>();
        backButton = childDic["TurnBtn_F"].GetComponent<Button>();
        pointerImg = childDic["WheelArrow_F"].GetComponent<Image>();
        WheelObj = childDic["TurnTableMIddle_F"].gameObject;
        spinButton.onClick.AddListener(OnSpinButtonClick);
        watchAdButton.onClick.AddListener(OnWatchAdButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
        UpdateButtonState();
        InitializeSegmentTexts();
        InitializeSegmentWeights();
    }
    void InitializeSegmentTexts()
    {
        Transform turnTableMiddle = WheelObj.transform; // TurnTableMIddle_F
        int textCount = turnTableMiddle.childCount;
        if (textCount < segments)
        {
            Debug.LogError("TurnTableMIddle_F 下的 Text 数量少于 segments 数量！");
            return;
        }

        segmentTexts = new TextMeshProUGUI[segments];
        for (int i = 0; i < segments; i++)
        {
            Transform child = turnTableMiddle.GetChild(i);
            TextMeshProUGUI textComponent = child.GetChild(3).GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                segmentTexts[i] = textComponent;
                segmentTexts[i].text = ConfigManager.Instance.Tables.TableTurntableConfig.Get(i + 1).Money.ToString();
            }
            else
            {
                Debug.LogError($"TurnTableMIddle_F 下的第 {i} 个子物体没有 Text 组件！");
            }
        }
    }

    void InitializeSegmentWeights()
    {
        // 假设每个段的权重存储在TableTurntableConfig中
        for (int i = 1; i <= segments; i++)
        {
            int weight = ConfigManager.Instance.Tables.TableTurntableConfig.Get(i).Weight;
            segmentWeights.Add(weight);
        }
    }

    // 加权随机选择一个段
    int GetWeightedRandomSegment()
    {
        int totalWeight = 0;
        foreach (var weight in segmentWeights)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;
        for (int i = 0; i < segmentWeights.Count; i++)
        {
            cumulativeWeight += segmentWeights[i];
            if (randomValue < cumulativeWeight)
            {
                return i;
            }
        }

        return segments - 1; // 默认返回最后一个段
    }
    public void UpdateButtonState()
    {
        if (AccountManager.Instance.CanUseFreeSpin())
        {
            spinButton.gameObject.SetActive(true);
            watchAdButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
        }
        else
        {
            spinButton.gameObject.SetActive(false);
            watchAdButton.gameObject.SetActive(true);
            backButton.gameObject.SetActive(true);
        }
    }

    void OnSpinButtonClick()
    {
        if (!isSpinning)
        {
            isSpinning = true;
            AccountManager.Instance.UseFreeSpin(); // 使用免费转盘机会
            int segment = GetWeightedRandomSegment();
            StartCoroutine(SpinWheel(segment));
            //UpdateButtonState();
        }
    }

    void OnWatchAdButtonClick()
    {
        if (!isSpinning)
        {
            isSpinning = true;
            int segment = GetWeightedRandomSegment();
            StartCoroutine(SpinWheel(segment));

            //UpdateButtonState();
            //AdManager.Instance.ShowRewardedAd(() => {
            //    isSpinning = true;
            //    int segment = GetWeightedRandomSegment();
            //    StartCoroutine(SpinWheel(segment));
            //    UpdateButtonState();
            //});
        }
    }


    void OnBackButtonClick()
    {
        spinButton.onClick.RemoveListener(OnSpinButtonClick);
        watchAdButton.onClick.RemoveListener(OnWatchAdButtonClick);
        backButton.onClick.RemoveListener(OnBackButtonClick);
        Destroy(gameObject);
    }

    IEnumerator SpinWheel(int targetSegment)
    {
        float singleSegmentAngle = 360f / segments;

        // 添加随机偏移量，提高视觉效果
        float randomOffset = Random.Range(-singleSegmentAngle / 4, singleSegmentAngle / 4);
        float targetAngle = 360f * 5 + (360f - (targetSegment * singleSegmentAngle) - (singleSegmentAngle / 2)) + randomOffset;

        float initialRotation = WheelObj.transform.eulerAngles.z;
        float finalRotation = initialRotation + targetAngle;

        float elapsed = 0f;
        float duration = spinDuration;

        while (elapsed < duration)
        {
            // 使用缓动函数（Ease Out）优化旋转效果
            float t = elapsed / duration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease Out
            float angle = Mathf.Lerp(0, targetAngle, easedT);
            WheelObj.transform.eulerAngles = new Vector3(0, 0, initialRotation + angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保最终旋转角度准确
        WheelObj.transform.eulerAngles = new Vector3(0, 0, finalRotation % 360);
        isSpinning = false;

        Debug.Log($"Final Rotation: {finalRotation % 360}");
        int landedSegment = DetermineRewardSegment(WheelObj.transform.eulerAngles.z); // 0 - 5
        Debug.Log($"Landed Segment: {landedSegment}");
        currentReward = GetWheel(landedSegment);

        // 高亮显示结果段
        HighlightResultSegment(landedSegment);

        ShowRewardPanel(currentReward);
    }

    // 高亮显示结果段，隐藏其他段的高亮图
    void HighlightResultSegment(int segment)
    {
        for (int i = 0; i < segments; i++)
        {
            Transform child = WheelObj.transform.GetChild(i);
            Transform yellowHigh = child.Find("Yellowhigh"); // 假设高亮图命名为Yellowhigh
            if (yellowHigh != null)
            {
                yellowHigh.gameObject.SetActive(i == segment);
            }
            else
            {
                Debug.LogError($"Segment {i} 下没有找到 Yellowhigh 对象！");
            }
        }
    }


    int GetWheel(int landedSegment)
    {
        int rewardNum = 0;
        switch (landedSegment + 1)
        {
            case 1:
                return rewardNum = ConfigManager.Instance.Tables.TableTurntableConfig.Get(landedSegment + 1).Money;
            case 2:
                return rewardNum = ConfigManager.Instance.Tables.TableTurntableConfig.Get(landedSegment + 1).Money;
            case 3:
                return rewardNum = ConfigManager.Instance.Tables.TableTurntableConfig.Get(landedSegment + 1).Money;
            case 4:
                return rewardNum = ConfigManager.Instance.Tables.TableTurntableConfig.Get(landedSegment + 1).Money;
            case 5:
                return rewardNum = ConfigManager.Instance.Tables.TableTurntableConfig.Get(landedSegment + 1).Money;
            case 6:
                return rewardNum = ConfigManager.Instance.Tables.TableTurntableConfig.Get(landedSegment + 1).Money;
            default:
                return rewardNum;
        }
    }
    int DetermineRewardSegment(float angle)
    {
        float singleSegmentAngle = 360f / segments;
        angle = angle % 360f;

        // 计算当前角度所在的段
        int segment = Mathf.FloorToInt(angle / singleSegmentAngle) % segments;
        return segment;
    }


    void ShowRewardPanel(int reward)
    {
        StartCoroutine(ShowRewardPanelCoroutine(reward));
    }

    IEnumerator ShowRewardPanelCoroutine(int reward)
    {
        // 延迟2秒
        yield return new WaitForSeconds(2f);
        CloseBtn(spinButton.gameObject, backButton.gameObject, watchAdButton.gameObject);
        // 实例化奖励面板
        RewardPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/RewardTurablePanel"));
        RewardPanel.transform.SetParent(transform.parent, false);
        RewardPanel.transform.localPosition = Vector3.zero;

        // 获取RewardTurablePanelController并设置当前段
        RewardTurablePanelController rewardController = RewardPanel.GetComponent<RewardTurablePanelController>();
        if (rewardController != null)
        {
            rewardController.currentRewardSegment = DetermineRewardSegment(WheelObj.transform.eulerAngles.z);
        }
        else
        {
            Debug.LogError("RewardPanel 上没有找到 RewardTurablePanelController 组件！");
        }
    }


    void CloseBtn(GameObject spinBtn, GameObject backBtn, GameObject watchAdBtn)
    {
        if (spinBtn.activeSelf)
        {
            spinBtn.SetActive(false);
        }
        if (backBtn.activeSelf)
        {
            backBtn.SetActive(false);
        }
        if (watchAdBtn.activeSelf)
        {
            watchAdBtn.SetActive(false);
        }
    }
}

