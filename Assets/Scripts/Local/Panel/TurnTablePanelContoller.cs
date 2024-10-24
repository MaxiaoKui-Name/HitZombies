using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        Destroy(gameObject);
    }

    IEnumerator SpinWheel(int targetSegment)
    {
        float totalSpin = 360 * 5; // 转盘旋转5圈
        float currentSpin = 0;
        float singleSegmentAngle = 360f / segments;
        float targetAngle = 360f * 5 - (targetSegment * singleSegmentAngle) + singleSegmentAngle / 2;

        float initialRotation = WheelObj.transform.eulerAngles.z;
        float finalRotation = initialRotation + targetAngle;

        while (currentSpin < targetAngle)
        {
            float spinThisFrame = spinSpeed * Time.deltaTime;
            WheelObj.transform.Rotate(0, 0, spinThisFrame);
            currentSpin += spinThisFrame;
            yield return null;
        }

        WheelObj.transform.eulerAngles = new Vector3(0, 0, finalRotation % 360);
        isSpinning = false;

        // 确定奖励
        int landedSegment = DetermineRewardSegment(WheelObj.transform.eulerAngles.z); //0 -5
        currentReward = GetWheel(landedSegment);
        ShowRewardPanel(currentReward);
    }
    int GetWheel(int landedSegment)
    {
        int rewardNum = 0;
        switch(landedSegment + 1)
        {
            case 1:
                return rewardNum = 1;
            case 2:
                return rewardNum = 1;
            case 3:
                return rewardNum = 1;
            case 4:
                return rewardNum = 1;
            case 5:
                return rewardNum = 1;
            case 6:
                return rewardNum = 1;
            default:
                return rewardNum;
        }
           
    }
    int DetermineRewardSegment(float angle)
    {
        float singleSegmentAngle = 360f / segments;
        int segment = Mathf.FloorToInt((360f - angle + singleSegmentAngle / 2) / singleSegmentAngle) % segments;
        return segment;
    }

    void ShowRewardPanel(int reward)
    {
        CloseBtn(spinButton.gameObject,backButton.gameObject, watchAdButton.gameObject);
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
