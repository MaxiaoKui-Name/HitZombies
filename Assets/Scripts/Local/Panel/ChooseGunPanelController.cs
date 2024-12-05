using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ChooseGunPanelController : UIBase
{
    [Header("��ǹ")]
    public Button RedBoxBtn_F; // ���ť
    public Image ChooseFinger_F; // ��ָͼƬ
    public Button ChooseMaxBtn_F; // Max��ť
    public TextMeshProUGUI ChooseGunNote_Text; // ��ǹ��ʾ�ı�
    public GameObject ChooseGun_F; // ǹչʾͼƬ�ĸ�����

    [Header("Gun Button Settings")]
    public GameObject GunButtonPrefab; // ǹе��ťԤ����
    public Transform GunButtonContainer; // ǹе��ť����
    public float CircleRadius = 200f; // Բ���뾶
    public float RotationSpeed = 100f; // Բ����ת�ٶ�

    private bool isFingerAnimationActive = true;
    private List<Button> gunButtons = new List<Button>();

    private bool isDragging = false;
    private Vector2 previousMousePosition;
    private bool isFirstTime = true;

    // �������֣����ڹ���ÿ��λ�õİ�ť
    private Dictionary<int, List<Button>> positionButtons = new Dictionary<int, List<Button>>();
    private int totalGunButtons = 6; // Բ���Ϲ̶���λ������
    private int currentStep = 0; // ��ǰ��ת����
    private float stepAngle = 360f / 6; // ÿ������ת�Ƕȣ���60��
    private int maxStep = 0; // �����ת����

    void Start()
    {
        GetAllChild(transform);
        // ��ȡUI��Ԫ��
        ChooseFinger_F = childDic["Choosefinger_F"].GetComponent<Image>();
        RedBoxBtn_F = childDic["RedBoxBtn_F"].GetComponent<Button>();
        ChooseGunNote_Text = childDic["ChooseGunNoteText_F"].GetComponent<TextMeshProUGUI>();
        ChooseMaxBtn_F = childDic["ChooseMaxBtn_F"].GetComponent<Button>();
        GunButtonContainer = childDic["GunImg_F"];
        // ��ʼ״̬����

        // ���ó�ʼ�ı�
        ChooseGunNote_Text.text = "�볤�����ť��ѡ������";

        // ����¼�����
        ChooseMaxBtn_F.onClick.AddListener(OnChooseMaxBtnClicked);

        // ����Ƿ��ǵ�һ����ʾѡ��ǹе���
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
        // ��̬����ǹе��ť
        GenerateGunButtons();
        // ����ǹе��ť����¼�
        foreach (var gunButton in gunButtons)
        {
            gunButton.onClick.AddListener(() => OnGunButtonClicked(gunButton));
        }
    }

    // ��ʼ��ָ������ѭ������
    private void StartFingerAnimation()
    {
        Sequence fingerSequence = DOTween.Sequence();
        fingerSequence.Append(ChooseFinger_F.transform.DOLocalMoveY(ChooseFinger_F.transform.localPosition.y + 5f, 0.5f).SetEase(Ease.InOutSine));
        fingerSequence.Append(ChooseFinger_F.transform.DOLocalMoveY(ChooseFinger_F.transform.localPosition.y - 5f, 0.5f).SetEase(Ease.InOutSine));
        fingerSequence.SetLoops(-1);
    }

    // ������ť�ĳ����¼�
    public void OnRedBoxBtnLongPressed()
    {
        // ֹͣ��ָ����
        DOTween.Kill(ChooseFinger_F.transform);
        isFingerAnimationActive = false;
        ChooseFinger_F.gameObject.SetActive(false);
        // ���غ��ť
        RedBoxBtn_F.gameObject.SetActive(false);
        // ��ʾChooseMax��ť
        ChooseMaxBtn_F.gameObject.SetActive(true);
        // ������ʾ�ı�
        ChooseGunNote_Text.text = "Click Max";
        if (isFirstTime)
        {
            // ����������������
            AccountManager.Instance.SetCompletedGunSelectionTutorial();
            PlayInforManager.Instance.playInfor.hasCompletedGunSelectionTutorial = true;
        }
    }

    // ѡ��Max��ť����¼�
    private void OnChooseMaxBtnClicked()
    {
        if (PlayInforManager.Instance.AllGunName.Count > 0)
        {
            string selectedGunType = PlayInforManager.Instance.AllGunName[0];
            string correspondingBulletType = PlayInforManager.Instance.GunToBulletMap[selectedGunType];

            // ���µ�ǰ������Ϣ
            PlayInforManager.Instance.playInfor.currentGun.gunName = selectedGunType;
            PlayInforManager.Instance.playInfor.currentGun.bulletType = correspondingBulletType;

            // ���������Ϣ
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.ReplaceGunDragon();
            }

            // �����˻�����
            AccountManager.Instance.SaveAccountData();

            // �ر�ѡ�����
            CloseChooseGunPanel();
        }
        else
        {
            Debug.LogError("AllGunName�б�Ϊ�գ��޷�ѡ��������");
        }
    }

    // ǹе��ť����¼�
    private void OnGunButtonClicked(Button gunButton)
    {
        string selectedGunName = gunButton.GetComponent<Image>().sprite.name;
        string correspondingBulletType = PlayInforManager.Instance.GunToBulletMap[selectedGunName];

        // ���µ�ǰ������Ϣ
        PlayInforManager.Instance.playInfor.currentGun.gunName = selectedGunName;
        PlayInforManager.Instance.playInfor.currentGun.bulletType = correspondingBulletType;

        // ���������Ϣ
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.ReplaceGunDragon();
        }
        // �����˻�����
        AccountManager.Instance.SaveAccountData();
        // �ر�ѡ�����
        CloseChooseGunPanel();
    }


    // �ر�ѡ��ǹе���ķ���
    private void CloseChooseGunPanel()
    {
        // ����ѡ��ǹе���
        Destroy(gameObject);
    }

    // ��̬����ǹе��ť��������Բ����
    private void GenerateGunButtons()
    {
        // ������а�ť
        foreach (Transform child in GunButtonContainer)
        {
            Destroy(child.gameObject);
        }
        gunButtons.Clear();
        positionButtons.Clear();

        int gunCount = PlayInforManager.Instance.AllGunName.Count;
        if (gunCount == 0)
        {
            Debug.Log("AllGunName�б�Ϊ�գ��޷�����ǹе��ť��");
            return;
        }

        // ����ÿ���̶�λ�õĽǶ�
        float angleStep = 360f / totalGunButtons;

        // �ҵ�ÿ��λ�������İ�ť����������������ת����
        maxStep = 0;

        for (int i = 0; i < gunCount; i++)
        {
            // ʵ����ǹе��ť
            GameObject gunButtonObj = Instantiate(GunButtonPrefab, GunButtonContainer);
            gunButtonObj.name = $"GunButton_{i + 1}";
            Button gunButton = gunButtonObj.GetComponent<Button>();
            gunButtons.Add(gunButton);

            // ����ǹеͼƬ
            Image gunImage = gunButtonObj.GetComponent<Image>();
            string gunName = PlayInforManager.Instance.AllGunName[i];
            if (i < PlayInforManager.Instance.AllstarGunImg.Count)
            {
                gunImage.sprite = PlayInforManager.Instance.AllstarGunImg[i];
            }
            else
            {
                Debug.LogWarning($"AllstarGunImg�б���δ�ҵ�����Ϊ {i} ��Sprite��");
            }

            // ����ǹе�۸�
            TextMeshProUGUI gunPriceText = gunButtonObj.transform.Find("GunPriceText").GetComponent<TextMeshProUGUI>();
            int gunPrice = GetGunPrice(gunName);
            gunPriceText.text = gunPrice.ToString();

            // ����ǹе��ť��λ��
            int positionIndex = i % totalGunButtons; // ���㵱ǰǹеӦ���õĹ̶�λ������
            float angle = positionIndex * angleStep;
            float radian = angle * Mathf.Deg2Rad;
            Vector2 position = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * CircleRadius;
            gunButtonObj.GetComponent<RectTransform>().anchoredPosition = position;

            // ����ǹе��ť���游������ת����������
            gunButtonObj.GetComponent<RectTransform>().rotation = Quaternion.identity;

            // ����ť��ӵ���Ӧ��λ���б���
            if (!positionButtons.ContainsKey(positionIndex))
            {
                positionButtons[positionIndex] = new List<Button>();
            }
            positionButtons[positionIndex].Add(gunButton);

            // ���������
            if (positionButtons[positionIndex].Count > maxStep)
            {
                maxStep = positionButtons[positionIndex].Count;
            }
        }

        // ��ʼ����ť�ɼ��ԣ���ʾÿ��λ�õĵ�һ����ť����������
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
        // ����gunName��ȡ�۸񣬼�����һ�����ñ�
        // �����Թ̶�ֵΪ��
        return 100;
    }

    // ���£������϶�����תǹеԲ��
    void Update()
    {
        HandleGunRotation();
    }

    private float currentAngle = 0f;
    private float accumulatedAngle = 0f; // �ۼ���ת�Ƕ�

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

            // ����Ƿ񵽴�������С������������ת
            if ((currentStep <= 0 && rotationDelta > 0) || (currentStep >= maxStep - 1 && rotationDelta < 0))
            {
                rotationDelta = 0f;
                accumulatedAngle = 0f;
                return;
            }

            currentAngle += rotationDelta;
            accumulatedAngle += rotationDelta;

            GunButtonContainer.Rotate(0, 0, -rotationDelta);

            // ȷ��ǹе��ť���游������ת����������
            foreach (Transform gunButton in GunButtonContainer)
            {
                gunButton.rotation = Quaternion.identity;
            }

            previousMousePosition = currentMousePosition;

            // ����Ƿ���ת����һ��������60�ȣ�
            if (Mathf.Abs(accumulatedAngle) >= stepAngle)
            {
                int step = (int)(accumulatedAngle / stepAngle);
                accumulatedAngle -= step * stepAngle;

                // ����currentStep��������0��maxStep - 1֮��
                currentStep -= step;
                currentStep = Mathf.Clamp(currentStep, 0, maxStep - 1);

                // ���°�ť�ɼ���
                UpdateButtonVisibility();
            }
        }
    }

    /// <summary>
    /// ���°�ť�Ŀɼ��ԣ�ȷ��ÿ��λ��ֻ��һ����ť�ɼ�
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

    //�Ƿ��������������е�UI��ʾ
    public void ShowChooseGunPanel(bool isFirst)
    {
        this.gameObject.SetActive(true); // ȷ����屾������
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
