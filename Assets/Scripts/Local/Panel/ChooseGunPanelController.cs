using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ChooseGunPanelController : UIBase
{
    [Header("换枪")]
    public Button RedBoxBtn_F; // 红框按钮
    public Image ChooseFinger_F; // 手指图片
    public Button ChooseMaxBtn_F; // Max按钮
    public TextMeshProUGUI ChooseGunNote_Text; // 换枪提示文本
    public GameObject ChooseGun_F; // 枪展示图片的父物体

    [Header("Gun Button Settings")]
    public GameObject GunButtonPrefab; // 枪械按钮预制体
    public Transform GunButtonContainer; // 枪械按钮容器
    public float CircleRadius = 200f; // 圆环半径
    public float RotationSpeed = 100f; // 圆环旋转速度

    private bool isFingerAnimationActive = true;
    private List<Button> gunButtons = new List<Button>();

    private bool isDragging = false;
    private Vector2 previousMousePosition;
    private bool isFirstTime = true;

    // 新增部分：用于管理每个位置的按钮
    private Dictionary<int, List<Button>> positionButtons = new Dictionary<int, List<Button>>();
    private int totalGunButtons = 6; // 圆环上固定的位置数量
    private int currentStep = 0; // 当前旋转步数
    private float stepAngle = 360f / 6; // 每步的旋转角度，即60度
    private int maxStep = 0; // 最大旋转步数

    void Start()
    {
        GetAllChild(transform);
        // 获取UI子元素
        ChooseFinger_F = childDic["Choosefinger_F"].GetComponent<Image>();
        RedBoxBtn_F = childDic["RedBoxBtn_F"].GetComponent<Button>();
        ChooseGunNote_Text = childDic["ChooseGunNoteText_F"].GetComponent<TextMeshProUGUI>();
        ChooseMaxBtn_F = childDic["ChooseMaxBtn_F"].GetComponent<Button>();
        GunButtonContainer = childDic["GunImg_F"];
        // 初始状态设置

        // 设置初始文本
        ChooseGunNote_Text.text = "请长按红框按钮以选择武器";

        // 添加事件监听
        ChooseMaxBtn_F.onClick.AddListener(OnChooseMaxBtnClicked);

        // 检查是否是第一次显示选择枪械面板
        isFirstTime = !AccountManager.Instance.HasCompletedGunSelectionTutorial();
        if (isFirstTime)
        {
            StartFingerAnimation();
            RedBoxBtn_F.gameObject.AddComponent<RedBoxButtonHandler>().Initialize(this);
        }
        else
        {
            ChooseFinger_F.gameObject.SetActive(false);
        }
        RedBoxBtn_F.gameObject.SetActive(false);
        ChooseFinger_F.gameObject.SetActive(false);
        ChooseGunNote_Text.transform.parent.gameObject.SetActive(false);
        ChooseMaxBtn_F.gameObject.SetActive(false);
        // 动态生成枪械按钮
        GenerateGunButtons();
        // 设置枪械按钮点击事件
        foreach (var gunButton in gunButtons)
        {
            gunButton.onClick.AddListener(() => OnGunButtonClicked(gunButton));
        }
    }

    // 开始手指的上下循环动画
    private void StartFingerAnimation()
    {
        Sequence fingerSequence = DOTween.Sequence();
        fingerSequence.Append(ChooseFinger_F.transform.DOLocalMoveY(ChooseFinger_F.transform.localPosition.y + 5f, 0.5f).SetEase(Ease.InOutSine));
        fingerSequence.Append(ChooseFinger_F.transform.DOLocalMoveY(ChooseFinger_F.transform.localPosition.y - 5f, 0.5f).SetEase(Ease.InOutSine));
        fingerSequence.SetLoops(-1);
    }

    // 处理红框按钮的长按事件
    public void OnRedBoxBtnLongPressed()
    {
        // 停止手指动画
        DOTween.Kill(ChooseFinger_F.transform);
        isFingerAnimationActive = false;
        ChooseFinger_F.gameObject.SetActive(false);
        // 隐藏红框按钮
        RedBoxBtn_F.gameObject.SetActive(false);
        // 显示ChooseMax按钮
        ChooseMaxBtn_F.gameObject.SetActive(true);
        // 更改提示文本
        ChooseGunNote_Text.text = "Click Max";
        if (isFirstTime)
        {
            // 标记新手引导已完成
            AccountManager.Instance.SetCompletedGunSelectionTutorial();
            PlayInforManager.Instance.playInfor.hasCompletedGunSelectionTutorial = true;
        }
    }

    // 选择Max按钮点击事件
    private void OnChooseMaxBtnClicked()
    {
        if (PlayInforManager.Instance.AllGunName.Count > 0)
        {
            string selectedGunType = PlayInforManager.Instance.AllGunName[0];
            string correspondingBulletType = PlayInforManager.Instance.GunToBulletMap[selectedGunType];

            // 更新当前武器信息
            PlayInforManager.Instance.playInfor.currentGun.gunName = selectedGunType;
            PlayInforManager.Instance.playInfor.currentGun.bulletType = correspondingBulletType;

            // 更新玩家信息
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.ReplaceGunDragon();
            }

            // 保存账户数据
            AccountManager.Instance.SaveAccountData();

            // 关闭选择界面
            CloseChooseGunPanel();
        }
        else
        {
            Debug.LogError("AllGunName列表为空，无法选择武器！");
        }
    }

    // 枪械按钮点击事件
    private void OnGunButtonClicked(Button gunButton)
    {
        string selectedGunName = gunButton.GetComponent<Image>().sprite.name;
        string correspondingBulletType = PlayInforManager.Instance.GunToBulletMap[selectedGunName];

        // 更新当前武器信息
        PlayInforManager.Instance.playInfor.currentGun.gunName = selectedGunName;
        PlayInforManager.Instance.playInfor.currentGun.bulletType = correspondingBulletType;

        // 更新玩家信息
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.ReplaceGunDragon();
        }
        // 保存账户数据
        AccountManager.Instance.SaveAccountData();
        // 关闭选择界面
        CloseChooseGunPanel();
    }


    // 关闭选择枪械面板的方法
    private void CloseChooseGunPanel()
    {
        // 隐藏选择枪械面板
        Destroy(gameObject);
    }

    // 动态生成枪械按钮并排列在圆环内
    private void GenerateGunButtons()
    {
        // 清空现有按钮
        foreach (Transform child in GunButtonContainer)
        {
            Destroy(child.gameObject);
        }
        gunButtons.Clear();
        positionButtons.Clear();

        int gunCount = PlayInforManager.Instance.AllGunName.Count;
        if (gunCount == 0)
        {
            Debug.Log("AllGunName列表为空，无法生成枪械按钮！");
            return;
        }

        // 计算每个固定位置的角度
        float angleStep = 360f / totalGunButtons;

        // 找到每个位置上最大的按钮数量，用于限制旋转步数
        maxStep = 0;

        for (int i = 0; i < gunCount; i++)
        {
            // 实例化枪械按钮
            GameObject gunButtonObj = Instantiate(GunButtonPrefab, GunButtonContainer);
            gunButtonObj.name = $"GunButton_{i + 1}";
            Button gunButton = gunButtonObj.GetComponent<Button>();
            gunButtons.Add(gunButton);

            // 设置枪械图片
            Image gunImage = gunButtonObj.GetComponent<Image>();
            string gunName = PlayInforManager.Instance.AllGunName[i];
            if (i < PlayInforManager.Instance.AllstarGunImg.Count)
            {
                gunImage.sprite = PlayInforManager.Instance.AllstarGunImg[i];
            }
            else
            {
                Debug.LogWarning($"AllstarGunImg列表中未找到索引为 {i} 的Sprite！");
            }

            // 设置枪械价格
            TextMeshProUGUI gunPriceText = gunButtonObj.transform.Find("GunPriceText").GetComponent<TextMeshProUGUI>();
            int gunPrice = GetGunPrice(gunName);
            gunPriceText.text = gunPrice.ToString();

            // 计算枪械按钮的位置
            int positionIndex = i % totalGunButtons; // 计算当前枪械应放置的固定位置索引
            float angle = positionIndex * angleStep;
            float radian = angle * Mathf.Deg2Rad;
            Vector2 position = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * CircleRadius;
            gunButtonObj.GetComponent<RectTransform>().anchoredPosition = position;

            // 设置枪械按钮不随父物体旋转，保持正立
            gunButtonObj.GetComponent<RectTransform>().rotation = Quaternion.identity;

            // 将按钮添加到对应的位置列表中
            if (!positionButtons.ContainsKey(positionIndex))
            {
                positionButtons[positionIndex] = new List<Button>();
            }
            positionButtons[positionIndex].Add(gunButton);

            // 更新最大步数
            if (positionButtons[positionIndex].Count > maxStep)
            {
                maxStep = positionButtons[positionIndex].Count;
            }
        }

        // 初始化按钮可见性，显示每个位置的第一个按钮，其余隐藏
        foreach (var kvp in positionButtons)
        {
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                if (i == currentStep)
                {
                    kvp.Value[i].gameObject.SetActive(true);
                }
                else
                {
                    kvp.Value[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private int GetGunPrice(string gunName)
    {
        // 根据gunName获取价格，假设有一个配置表
        // 这里以固定值为例
        return 100;
    }

    // 更新：处理拖动以旋转枪械圆环
    void Update()
    {
        HandleGunRotation();
    }

    private float currentAngle = 0f;
    private float accumulatedAngle = 0f; // 累计旋转角度

    private void HandleGunRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 delta = currentMousePosition - previousMousePosition;

            float rotationDelta = delta.x * RotationSpeed * Time.deltaTime;

            // 检查是否到达最大或最小步数，限制旋转
            if ((currentStep <= 0 && rotationDelta > 0) || (currentStep >= maxStep - 1 && rotationDelta < 0))
            {
                rotationDelta = 0f;
                accumulatedAngle = 0f;
                return;
            }

            currentAngle += rotationDelta;
            accumulatedAngle += rotationDelta;

            GunButtonContainer.Rotate(0, 0, -rotationDelta);

            // 确保枪械按钮不随父物体旋转，保持正立
            foreach (Transform gunButton in GunButtonContainer)
            {
                gunButton.rotation = Quaternion.identity;
            }

            previousMousePosition = currentMousePosition;

            // 检测是否旋转超过一个步数（60度）
            if (Mathf.Abs(accumulatedAngle) >= stepAngle)
            {
                int step = (int)(accumulatedAngle / stepAngle);
                accumulatedAngle -= step * stepAngle;

                // 更新currentStep，限制在0到maxStep - 1之间
                currentStep -= step;
                currentStep = Mathf.Clamp(currentStep, 0, maxStep - 1);

                // 更新按钮可见性
                UpdateButtonVisibility();
            }
        }
    }

    /// <summary>
    /// 更新按钮的可见性，确保每个位置只有一个按钮可见
    /// </summary>
    private void UpdateButtonVisibility()
    {
        foreach (var kvp in positionButtons)
        {
            int positionIndex = kvp.Key;
            List<Button> buttons = kvp.Value;
            int buttonCount = buttons.Count;

            if (buttonCount == 0)
                continue;

            for (int i = 0; i < buttonCount; i++)
            {
                if (i == currentStep)
                {
                    buttons[i].gameObject.SetActive(true);
                }
                else
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    //是否是新手引导进行的UI显示
    public void ShowChooseGunPanel(bool isFirst)
    {
        this.gameObject.SetActive(true); // 确保面板本身被激活
        if (!isFirst)
        {
            ChooseGunNote_Text.transform.parent.gameObject.SetActive(true);
            ChooseMaxBtn_F.transform.gameObject.SetActive(true);
            ChooseGunNote_Text.text = "Click Max";
        }
        else
        {
            ChooseGunNote_Text.transform.parent.gameObject.SetActive(true);
            ChooseGunNote_Text.text = "Click Max";
            StartFingerAnimation();
        }
    }
}
