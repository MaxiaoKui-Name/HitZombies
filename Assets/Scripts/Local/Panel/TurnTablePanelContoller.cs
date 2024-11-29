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

    public float spinDuration = 3f; // ת������ʱ��
    public float spinSpeed = 500f; // ת���ٶ�
    public int segments = 6; // ת�̵ȷ���
    public Button spinButton;//�齱��ť
    public Button watchAdButton;//�ۿ���水ť
    public Button backButton;//���ذ�ť
    public GameObject WheelObj;//ת��
    public GameObject RewardPanel;//�������
   
    public Image pointerImg; // ָ��ͼƬ
    public TextMeshProUGUI[] segmentTexts; // �������������洢6��Text���
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
            Debug.LogError("TurnTableMIddle_F �µ� Text �������� segments ������");
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
                Debug.LogError($"TurnTableMIddle_F �µĵ� {i} ��������û�� Text �����");
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
            AccountManager.Instance.UseFreeSpin(); // ʹ�����ת�̻���
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

        // ������ƫ����������Ӿ�Ч��
        float randomOffset = Random.Range(-singleSegmentAngle / 4, singleSegmentAngle / 4);
        float targetAngle = 360f * 5 + (360f - (targetSegment * singleSegmentAngle) - (singleSegmentAngle / 2)) + randomOffset;

        float initialRotation = WheelObj.transform.eulerAngles.z;
        float finalRotation = initialRotation + targetAngle;

        float elapsed = 0f;
        float duration = spinDuration;

        while (elapsed < duration)
        {
            // ʹ�û���������Ease Out���Ż���תЧ��
            float t = elapsed / duration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease Out
            float angle = Mathf.Lerp(0, targetAngle, easedT);
            WheelObj.transform.eulerAngles = new Vector3(0, 0, initialRotation + angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ȷ��������ת�Ƕ�׼ȷ
        WheelObj.transform.eulerAngles = new Vector3(0, 0, finalRotation % 360);
        isSpinning = false;

        Debug.Log($"Final Rotation: {finalRotation % 360}");
        int landedSegment = DetermineRewardSegment(WheelObj.transform.eulerAngles.z); // 0 - 5
        Debug.Log($"Landed Segment: {landedSegment}");
        currentReward = GetWheel(landedSegment);
        //TTOD��ӵײ���
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

        // ֱ�ӽ��Ƕȳ���ÿ���ֶεĽǶȣ�ȡ����ȡģ�õ��ֶ�
        int segment = Mathf.FloorToInt(angle / singleSegmentAngle) % segments;
        return segment;
    }

    void ShowRewardPanel(int reward)
    {
        StartCoroutine(ShowRewardPanelCoroutine(reward));
    }
    IEnumerator ShowRewardPanelCoroutine(int reward)
    {
        // �ӳ�2��
        yield return new WaitForSeconds(2f);
        CloseBtn(spinButton.gameObject, backButton.gameObject, watchAdButton.gameObject);
        // ʵ�����������
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
