using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class TurnTablePanelContoller : UIBase
{

    public float spinDuration = 3f; // 转动持续时间
    public float spinSpeed = 500f; // 转动速度
    public int segments = 6; // 转盘等分数
    public Button spinButton;//抽奖按钮
    public Button watchAdButton;//观看广告按钮
    public Button backButton;//返回按钮
    public GameObject WheelObj;//转盘
    public GameObject RewardPanel;//奖励面板
   
    public Image pointerImg; // 指针图片
    public TextMeshProUGUI[] segmentTexts; // 添加这个数组来存储6个Text组件
    private bool isSpinning = false;
    public int currentReward;

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
            int segment = Random.Range(0, segments);
            StartCoroutine(SpinWheel(segment));
            //UpdateButtonState();
        }
    }

    void OnWatchAdButtonClick()
    {
        if (!isSpinning)
        {
            isSpinning = true;
            int segment = Random.Range(0, segments);
            StartCoroutine(SpinWheel(segment));
           
            //UpdateButtonState();
            //AdManager.Instance.ShowRewardedAd(() => {
            //    isSpinning = true;
            //    int segment = Random.Range(0, segments);
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
        //TTOD添加底层亮
        WheelObj.transform.GetChild(landedSegment-1).transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        ShowRewardPanel(currentReward);
    }




    int GetWheel(int landedSegment)
    {
        int rewardNum = 0;
        switch(landedSegment + 1)
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

        // 直接将角度除以每个分段的角度，取整后取模得到分段
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
        RewardPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/RewardPanel"));
        RewardPanel.transform.SetParent(transform.parent, false);
        RewardPanel.transform.localPosition = Vector3.zero;
    }
    void CloseBtn(GameObject spineBtn, GameObject backBtn, GameObject watchAdBtn)
    {
        if (spineBtn.activeSelf)
        {
            spineBtn.SetActive(false);
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
