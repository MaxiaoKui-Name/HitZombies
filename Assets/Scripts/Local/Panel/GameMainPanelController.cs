using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonBones;
using Hitzb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;
using Transform = UnityEngine.Transform;

public class GameMainPanelController : UIBase
{
    public Button pauseButton;   // ������ͣ��ť
    public Text coinText;        // ������ʾ��ҵ��ı���
    private bool isPaused = false;
    public TextMeshProUGUI buffFrozenText;
    public Button buffFrozenBtn;
    public TextMeshProUGUI buffBlastText;
    public Button buffBlastBtn;
    public Image buffBlastBack;
    public Image buffForzenBack;
    public Sprite[] buffBlastImages;
    public Sprite[] buffForzenImages;
    public GameObject Forzen_F;

    [Header("��������")]
    public GameObject PanelOne_F;//����ͼƬ
    public GameObject Panel_F;//������
    public GameObject PanelThree_F;
    public GameObject SkillFinger_F1;
    public GameObject SkillFinger_F2;
    public Image GuidArrowL;
    public Image GuidArrowR;
    private Image GuidCircle;
    private Transform Guidfinger_F;

    //private Image Guidfinger;
    private Image GuidText;//��������ͼƬ
    public GameObject FirstNote_F;
    public GameObject TwoNote_F;
    public GameObject ThreeNote_F;
    public GameObject FourNote_F;
    public GameObject FiveNote_F;
    public GameObject SkillNote_F;
    public Image SkillFinger_F;//��ָͼƬ


    public Transform DieImg_F;

    [Header("Spawn Properties")]
    public float bombDropInterval;  // ը��Ͷ�����ʱ��
    public float planeSpeed = 2f;   // �ɻ��ƶ��ٶ�

    public GameObject player;
    public GameObject PausePanel;
    // ����Canvas��RectTransform
    public RectTransform canvasRectTransform;

    public Transform coinspattern_F;
    // ������ڼ�⻬���ı���
    private Vector3 lastMousePosition;
    private float dragThreshold = 100f; // ������ֵ�����Ը�����Ҫ����
    private bool isDragging = false;


    //��
    // ������ť
    public Button specialButton;          // �񱩼��ܰ�ť
    private Image specialBac_F;
    private Image specialButtonImage;     // ��ť��ͼƬ���
    // ��ʱ����ر���
    private bool isButtonActive = false; // ��ť�Ƿ��ڿɵ��״̬
    public bool isSkillActive = false;  // �����Ƿ񼤻�
    private float cooldownTime = 20f;    // ��ȴʱ�� 20 ��
    private float activeTime = 5f;       // ���ܳ���ʱ�� 5 ��
    public Sprite[] Rampages;

    public bool isGuidAnimationPlaying = false; // ��־�Ƿ����ڲ�����������
    private bool hasGuidAnimationPlayed = false; // ��־���������Ƿ��Ѳ��Ź�
    public bool FirstNote_FBool = false;
    public bool TwoNote_FBool = false;
    public bool TwoBeThree = false;
    public bool ThreeNote_FBool = false;
    public bool FourNote_FBool = false;
    public bool FiveNote_FBool = false;
    // ��־�ı��Ƿ���ȫ��ʾ
    private bool isTextFullyDisplayed = false;


    //��Ϯ����
    public UnityArmatureComponent EnemiesComeArmature;
    public UnityArmatureComponent MassiveEnemiesComeArmature;
    public UnityArmatureComponent BossComeArmature;


    //������Ҹ���
    public Transform TopcoinHight_F;
    public UnityArmatureComponent coinHightAmature;
    //�������ĸ���
    public Transform FirstMonsterCoin_F; 

    void Start()
    {
        GetAllChild(transform);
        DOTween.Init();
        // �ҵ��Ӷ����еİ�ť���ı���
        //��������
        canvasRectTransform = transform.parent.GetComponent<RectTransform>();
        GuidArrowL = childDic["GuidArrowL_F"].GetComponent<Image>();
        GuidArrowR = childDic["GuidArrowR_F"].GetComponent<Image>();
        GuidCircle = childDic["GuidCircle_F"].GetComponent<Image>();
        GuidCircle.transform.parent.gameObject.SetActive(false);
        Guidfinger_F = childDic["Guidfinger_F"];
        FirstNote_F = childDic["FirstNote_F"].gameObject;
        FirstNote_F.SetActive(false);
        TwoNote_F = childDic["TwoNote_F"].gameObject;
        TwoNote_F.SetActive(false);
        ThreeNote_F = childDic["ThreeNote_F"].gameObject;
        ThreeNote_F.SetActive(false);
        FourNote_F = childDic["FourNote_F"].gameObject;
        FourNote_F.SetActive(false);
        FiveNote_F = childDic["FiveNote_F"].gameObject;
        FiveNote_F.SetActive(false);
        coinspattern_F = childDic["coinspattern_F"].GetComponent<RectTransform>();
        DieImg_F = childDic["DieImg_F"];
        DieImg_F.gameObject.SetActive(false);

        SkillNote_F = childDic["SkillNote_F"].gameObject;
        SkillFinger_F = childDic["SkillFinger_F"].GetComponent<Image>();
        PanelOne_F = childDic["PanelOne_F"].gameObject;
        Panel_F = childDic["Panel_F"].gameObject;
        Panel_F.transform.gameObject.SetActive(false);
        PanelThree_F = childDic["PanelThree_F"].gameObject;
        PanelThree_F.transform.gameObject.SetActive(false);
        SkillNote_F.gameObject.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);
        SkillFinger_F1 = childDic["SkillFinger_F1"].gameObject;
        SkillFinger_F2 = childDic["SkillFinger_F2"].gameObject;
        SkillFinger_F1.SetActive(false);
        SkillFinger_F2.SetActive(false);

        //�����Լ��������
        TopcoinHight_F = childDic["TopcoinHight_F"];
        coinHightAmature = TopcoinHight_F.GetChild(0).GetComponent<UnityArmatureComponent>();
        //TopcoinHight_F.gameObject.SetActive(false);
        FirstMonsterCoin_F = childDic["FirstMonsterCoin_F"];
        //Guidfinger = childDic["Guidfinger_F"].GetComponent<Image>();
        GuidText = childDic["GuidText_F"].GetComponent<Image>();
        pauseButton = childDic["pause_Btn_F"].GetComponent<Button>();
        coinText = childDic["valueText_F"].GetComponent<Text>();
        buffFrozenText = childDic["Frozentimes_F"].GetComponent<TextMeshProUGUI>();
        buffFrozenBtn = childDic["FrozenBtn_F"].GetComponent<Button>();
        buffBlastText = childDic["Blastimes_F"].GetComponent<TextMeshProUGUI>();
        buffBlastBtn = childDic["BlastBtn_F"].GetComponent<Button>();
        Forzen_F = childDic["Forzen_F"].gameObject;
        Forzen_F.SetActive(false);
        buffBlastBack = buffBlastBtn.GetComponent<Image>();
        buffForzenBack = buffFrozenBtn.GetComponent<Image>();
        buffBlastBack.sprite = buffBlastImages[0];
        buffForzenBack.sprite = buffForzenImages[0];
        buffBlastText.text = "0";
        buffFrozenText.text = "0";
        EventDispatcher.instance.Regist(EventNameDef.UPDATECOIN, (v) => UpdateCoinText());
        // �����ͣ��ť�ĵ���¼�������
        pauseButton.onClick.AddListener(() => StartCoroutine(Onpause_Btn_FClicked()));
        buffFrozenBtn.onClick.AddListener(ToggleFrozen);
        buffBlastBtn.onClick.AddListener(ToggleBlast);
        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            childDic["BuffUI_F"].gameObject.SetActive(false);
        }

        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            pauseButton.transform.parent.gameObject.SetActive(false);
            PanelOne_F.transform.gameObject.SetActive(false);
        }
        if (GameFlowManager.Instance.currentLevelIndex != 0)
        {
            PanelOne_F.transform.gameObject.SetActive(false);
            GuidArrowL.transform.parent.gameObject.SetActive(false);
        }
        if (PlayerPrefs.HasKey("PlayerAccountID"))
        {
            string accountID = PlayerPrefs.GetString("PlayerAccountID");
            PlayInforManager.Instance.playInfor.BalstBuffCount = PlayerPrefs.GetInt($"{accountID}PlayerBalstBuffCount");
            PlayInforManager.Instance.playInfor.FrozenBuffCount = PlayerPrefs.GetInt($"{accountID}PlayerFrozenBuffCount");
            UpdateBuffText(PlayerPrefs.GetInt($"{accountID}PlayerFrozenBuffCount"), PlayerPrefs.GetInt($"{accountID}PlayerBalstBuffCount"));
        }
        // ��ʼ�� specialButton
        specialButton = childDic["specialBtn_F"].GetComponent<Button>();
        specialButtonImage = specialButton.GetComponent<Image>();
        specialButtonImage.sprite = Rampages[0];
        specialBac_F = childDic["specialBac_F"].GetComponent<Image>();
        specialButton.onClick.AddListener(OnSpecialButtonClicked);
        specialButton.interactable = false; // ��ʼ���ɵ��
        // ��ʼ��ť����ȴЭ��
        StartCoroutine(SpecialButtonCooldownCoroutine());
        UpdateCoinText();

        //��Ϯ����
        EnemiesComeArmature = childDic["Enemies_F"].GetChild(0).GetComponent<UnityArmatureComponent>();
        MassiveEnemiesComeArmature = childDic["MassiveEnemies_F"].GetChild(0).GetComponent<UnityArmatureComponent>();
        BossComeArmature = childDic["bossincoming_F"].GetChild(0).GetComponent<UnityArmatureComponent>();
        EnemiesComeArmature.transform.parent.gameObject.SetActive(false);
        MassiveEnemiesComeArmature.transform.parent.gameObject.SetActive(false);
        BossComeArmature.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        // ����״̬�µ��߼�
        if (GameManage.Instance.gameState == GameState.Guid)
        {
            // ����δ���Ź�������������û�����ڲ��Ŷ�������ʼ������������
            if (!isGuidAnimationPlaying && !hasGuidAnimationPlayed)
            {
                StartCoroutine(RunGuidAnimationWithDelay());
            }
            else if (!isGuidAnimationPlaying && hasGuidAnimationPlayed)
            {
                HandleNewbieGuide();
            }
        }

        if (GameManage.Instance.gameState == GameState.Running)
        {
            // �����ܼ���ʱ���������Ļ�����ӵ����˴�Ϊ�����߼����ԣ�
            if (isSkillActive && Input.GetMouseButtonDown(0))
            {
                FireHomingBullet();
            }
            if (GameManage.Instance.clickCount)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    clickCount++;
                }
            }
        }
        if (FistMonsterHightarmatureComponent != null && FistMonsterHightarmatureComponent.animation.isPlaying)
        {
            // ʹ��unscaledDeltaTimeȷ����������ͣʱ��Ȼ����
            float deltaTime = Time.timeScale == 0f ? Time.unscaledDeltaTime : Time.deltaTime;
            FistMonsterHightarmatureComponent.animation.AdvanceTime(deltaTime);
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.instance.UnRegist(EventNameDef.UPDATECOIN, (v) => UpdateCoinText());
        coinText = null;
    }

    private IEnumerator Onpause_Btn_FClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        pauseButton.interactable = false;
        yield return StartCoroutine(HandleButtonClickAnimation(transform));
        yield return StartCoroutine(ButtonBounceAnimation(pauseButton.GetComponent<RectTransform>(), OnpauseClicked));
    }

    void UpdateCoinText()
    {
        if (coinText == null) return;
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum:N0}";
    }

    public void UpdateCoinTextWithDOTween(long AddCoin, float durations)
    {
        long currentCoin = PlayInforManager.Instance.playInfor.coinNum;
        float duration = durations;
        long targetCoin = PlayInforManager.Instance.playInfor.coinNum + AddCoin;

        DOTween.To(() => currentCoin, x =>
        {
            currentCoin = x;
            PlayInforManager.Instance.playInfor.coinNum = currentCoin;
            coinText.text = $"{currentCoin:N0}";
        }, targetCoin, duration)
        .SetEase(Ease.Linear)
        .SetUpdate(true);
    }
    #region [���������߼�]
    /// <summary>
    /// ������������������Э��:
    /// 1. ��ָͼ��ӳ�ʼλ���ƶ���ԲȦλ�ã�ģ����������
    /// 2. ��ָ��ԲȦһ���������ƶ����ص���ʼλ�ã����һ��������
    /// </summary>
   private DG.Tweening.Sequence guidSequence;               // ����������������
   private bool isInitialGuideDone = false;     // �״����������Ƿ����

    private IEnumerator RunGuidAnimationWithDelay()
    {
        isGuidAnimationPlaying = true;
        PanelOne_F.SetActive(true);
        GuidCircle.transform.parent.gameObject.SetActive(true);
        Guidfinger_F.gameObject.SetActive(true);
        PreController.Instance.PlayisMove = false;
        // �ӳ�1����ִ����������
        yield return new WaitForSecondsRealtime(0.01f);
        // ִ����������
        RunGuidAnimation();
    }
    private void RunGuidAnimation()
    {
        // ��ͣ��Ϸ�߼���ͻ����������
        Time.timeScale = 0f;  // ��Ϸ��ͣ��ȷ��ִֻ����������
        // ��ȡ RectTransform
        RectTransform guidCircleRect = GuidCircle.GetComponent<RectTransform>();
        RectTransform skillFingerRect = Guidfinger_F.GetComponent<RectTransform>();

        // ��ʼλ��
        Vector2 initialCirclePos = guidCircleRect.anchoredPosition;
        Vector2 initialSkillFingerPos = skillFingerRect.anchoredPosition;

        // ���Ҽ�ͷ��λ��
        Vector2 leftPos = GuidArrowL.rectTransform.anchoredPosition;
        leftPos.x += 50f;
        Vector2 rightPos = GuidArrowR.rectTransform.anchoredPosition;
        rightPos.x -= 50f;

        // ��������ʱ��
        float moveDuration = 0.25f;

        // �����������
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(skillFingerRect.DOAnchorPos(new Vector2(initialSkillFingerPos.x, initialSkillFingerPos.y + 5), 0.1f).SetEase(Ease.InOutSine))
                      .Append(skillFingerRect.DOAnchorPos(initialSkillFingerPos, 0.1f).SetEase(Ease.InOutSine));

        // �ƶ���������
        Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(guidCircleRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                     .Join(skillFingerRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                     .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                     .Join(skillFingerRect.DOAnchorPos(initialSkillFingerPos, moveDuration).SetEase(Ease.InOutSine))
                     .Append(guidCircleRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                     .Join(skillFingerRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                     .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                     .Join(skillFingerRect.DOAnchorPos(initialSkillFingerPos, moveDuration).SetEase(Ease.InOutSine));

        // �ϲ���������
        Sequence guidSequence1 = DOTween.Sequence();
        guidSequence1.Append(clickSequence)
                    .Append(moveSequence)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        // �״�����������ɺ���߼�
                        isGuidAnimationPlaying = false;
                        hasGuidAnimationPlayed = true;
                        isInitialGuideDone = true;
                        // �� guidSequence ����Ϊ null����������ѭ������
                        guidSequence = null;
                        //guidSequence1.Kill();
                        //guidSequence1 = null;
                    });
        // ���Ŷ�������
        guidSequence1.Play();
    }

    /// <summary>
    /// ���״��������������󣬼��������룺
    /// - ����Ұ�����Ļ���϶�������ֵ���룬�������������ʼ��Ϸ��
    /// - �����һֱ���϶����򲥷�����ѭ��������������ֱ����⵽�϶�Ϊֹ��
    /// </summary>
    private Vector3 initialMousePosition; // ��ʼ�϶�λ��
    // �����������ڿ����Ƿ���PlayerController�����ƶ�
    private bool isControlTransferred = false;
    private void HandleNewbieGuide()
    {
        // 1. �״ζ���������޶�������ʱ���������϶���Ϊ
        if (!isGuidAnimationPlaying && hasGuidAnimationPlayed && !isControlTransferred)
        {
            // ����ʱƽ̨���
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // �����豸����
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            initialMousePosition = touch.position;
                            isDragging = false; // ��ʼ����ʱ���϶���־��Ϊ��
                            break;

                        case TouchPhase.Moved:
                            Vector3 currentTouchPosition = touch.position;
                            // ���� ��ɫ���϶�ʵʱ�ƶ� ���� ֻ���϶�ʱ�ƶ����
                            if (!isDragging)
                            {
                                Vector3 delta = currentTouchPosition - initialMousePosition;
                                float distanceSquared = delta.sqrMagnitude;
                                if (distanceSquared > dragThreshold)
                                {
                                    isDragging = true; // �ﵽ�϶���ֵ����ʼ�϶�
                                }
                            }

                            if (isDragging)
                            {
                                MovePlayerToCurrentPosition(currentTouchPosition);
                            }

                            // ����϶������Ƿ񳬹���ֵ
                            if (isDragging)
                            {
                                Vector3 delta = currentTouchPosition - initialMousePosition;
                                float distanceSquared = delta.sqrMagnitude;
                                if (distanceSquared > dragThreshold * dragThreshold)
                                {
                                    // ������ֵ���ر�����
                                    EndGuideAndTransferControl(currentTouchPosition);
                                }
                            }
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if (!isDragging)
                            {
                                // ��ҵ�����ͷţ�û���϶�
                                if (guidSequence == null)
                                {
                                    StartLoopGuideAnimation();
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                // ����¼�����
                if (Input.GetMouseButtonDown(0))
                {
                    initialMousePosition = Input.mousePosition;
                    isDragging = false; // �����ʱ����־��Ϊ��
                }

                if (Input.GetMouseButton(0))
                {
                    Vector3 currentMousePosition = Input.mousePosition;
                    // ���� ��ɫ���϶�ʵʱ�ƶ� ���� ֻ���϶�ʱ�ƶ����
                    if (!isDragging)
                    {
                        Vector3 delta = currentMousePosition - initialMousePosition;
                        float distanceSquared = delta.sqrMagnitude;
                        if (distanceSquared > dragThreshold)
                        {
                            isDragging = true; // �ﵽ�϶���ֵ����ʼ�϶�
                        }
                    }

                    if (isDragging)
                    {
                        MovePlayerToCurrentPosition(currentMousePosition);
                    }

                    // ����϶������Ƿ񳬹���ֵ
                    if (isDragging)
                    {
                        Vector3 delta = currentMousePosition - initialMousePosition;
                        float distanceSquared = delta.sqrMagnitude;
                        if (distanceSquared > dragThreshold * dragThreshold)
                        {
                            // ������ֵ���ر�����
                            EndGuideAndTransferControl(currentMousePosition);
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (!isDragging)
                    {
                        // ��ҵ�����ͷţ�û���϶�
                        if (guidSequence == null)
                        {
                            StartLoopGuideAnimation();
                        }
                    }
                }
            }
        }

        // 2. ���״�������ɺ�ʱ�����϶�����ʼѭ�������ظ���ʾ
        if (isInitialGuideDone && !isGuidAnimationPlaying && guidSequence == null && !isControlTransferred)
        {
            Debug.Log("Starting loop guide animation");
            StartLoopGuideAnimation();
        }
    }

    /// <summary>
    /// ��ɫʵʱ�ƶ�����ǰ����/���λ�ã�ֻ�ƶ�X�ᣬ���ౣ�ֲ��䣩��
    /// </summary>
    private void MovePlayerToCurrentPosition(Vector3 screenPosition)
    {
        // ��ȡ��Ҷ���
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("Player����δ�ҵ���");
            return;
        }

        // ��ȡ�������
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("�������δ�ҵ���");
            return;
        }

        // ����Ļ����ת��Ϊ��������
        Vector3 screenPos = new Vector3(screenPosition.x, screenPosition.y, cam.nearClipPlane);
        Vector3 targetWorldPos = cam.ScreenToWorldPoint(screenPos);

        // ֻ�� x ���Ͻ����ƶ���y��z λ�ñ��ֲ��䣨�ɸ�������Ķ���
        Vector3 currentPos = playerObj.transform.position;
        Vector3 endPos = new Vector3(targetWorldPos.x, currentPos.y, currentPos.z);

        playerObj.transform.position = endPos;
    }

    /// <summary>
    /// ��������ѭ������������������ֱ����ҷ����϶���ΪΪֹ��
    /// </summary>
    private void StartLoopGuideAnimation()
    {
        RectTransform guidCircleRect = GuidCircle.GetComponent<RectTransform>();
        RectTransform skillFingerRect = Guidfinger_F.GetComponent<RectTransform>();

        Vector2 initialCirclePos = guidCircleRect.anchoredPosition;
        Vector2 initialskillFingerPos = skillFingerRect.anchoredPosition;
        Vector2 leftPos = GuidArrowL.rectTransform.anchoredPosition;
        leftPos.x += 50f;
        Vector2 rightPos = GuidArrowR.rectTransform.anchoredPosition;
        rightPos.x -= 50f;
        float moveDuration = 0.25f;

        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(skillFingerRect.DOAnchorPos(new Vector2(initialskillFingerPos.x, initialskillFingerPos.y + 5), 0.1f).SetEase(Ease.InOutSine))
                     .Append(skillFingerRect.DOAnchorPos(initialskillFingerPos, 0.1f).SetEase(Ease.InOutSine));


        guidSequence = DOTween.Sequence();
        guidSequence.Append(guidCircleRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(initialskillFingerPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(initialskillFingerPos, moveDuration).SetEase(Ease.InOutSine));

        Sequence guidSequenceAll = DOTween.Sequence();
        guidSequenceAll.Append(clickSequence)
                    .Append(guidSequence)
                    .SetLoops(-1) // ����ѭ��
                    .SetUpdate(true);
    }

    /// <summary>
    /// ����⵽����϶�������Ϊʱ�������������л�����Ϸ����״̬��
    /// </summary>
    private void EndGuideAndTransferControl(Vector3 currentPosition)
    {
        PreController.Instance.PlayisMove = true;
        if (guidSequence != null)
        {
            guidSequence.Kill();
            guidSequence = null;
        }
        isGuidAnimationPlaying = false;
        hasGuidAnimationPlayed = true;
        isInitialGuideDone = true;
        isControlTransferred = true; // ��ǿ���Ȩ��ת��
        // �ָ���Ϸ
        Time.timeScale = 1f;
        PanelOne_F.SetActive(false);
        GuidCircle.transform.parent.gameObject.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);

        // �л���Ϸ״̬������״̬
        GameManage.Instance.SwitchState(GameState.Running);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        // ��鵱ǰ�Ƿ��г���������
        bool isTouching = false;
        Vector3 inputPosition = Vector3.zero;
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // �����豸����
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    isTouching = true;
                    inputPosition = touch.position;
                }
            }
        }
        else
        {
            // ����¼�����
            if (Input.GetMouseButton(0))
            {
                isTouching = true;
                inputPosition = Input.mousePosition;
            }
        }
        if (isTouching && playerController != null)
        {
            // ����ǰ����λ�ô��ݸ�PlayerController��ȷ���޷�ӹ�
            playerController.StartHandlingInput(inputPosition);
        }

        StartCoroutine(ShowFirstNoteAfterDelay());
    }
    #endregion
    #region[������ʾ]
    private IEnumerator ShowFirstNoteAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        ShowFirstNote();
    }

    public void ShowFirstNote()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner1").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        StartCoroutine(ShowMultipleNotesCoroutine(FirstNote_F, guidanceTexts));
    }
    public void ShowTwoNote1()
    {
        TwoNote_FBool = true;
        StartCoroutine(ShowTwoNoteAfterDelay());
    }
    public IEnumerator ShowTwoNoteAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0f);
        Time.timeScale = 0f;
        Panel_F.SetActive(true);
        ShowTwoNote2();
    }
    public void ShowTwoNote2()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner2").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.PlayHight();
        StartCoroutine(ShowMultipleNotesCoroutine(TwoNote_F, guidanceTexts));
    }

    private const string StartAnimation = "start";
    private const string StayAnimation = "stay";
    //ShowThreeNotec��������
    //public void SetHight(Vector2 tartPos)
    //{
    //    coinHight_F.gameObject.SetActive(true);
    //    coinHight_F.GetComponent<RectTransform>().anchoredPosition = tartPos;
    //    PlayHight();
    //}
    #region[������Ҹ���]
    private UnityArmatureComponent FistMonsterHightarmatureComponent;
    public void SetFirstMonsterCoinPos(Vector2 targetPos)
    {
        FirstMonsterCoin_F.gameObject.SetActive(true);
        FirstMonsterCoin_F.transform.GetComponent<RectTransform>().anchoredPosition = targetPos;
    }
    public void PlayHight(UnityArmatureComponent hightObj)
    {
        if(hightObj == FirstMonsterCoin_F.GetChild(0).GetComponent<UnityArmatureComponent>()) 
        {
            FistMonsterHightarmatureComponent = hightObj;
        }
        if (hightObj != null)
        {
            // ���Ķ�������¼�
            hightObj.AddDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
            // ����һ��start����
            hightObj.animation.Play(StartAnimation, 1); // 1��ʾ����һ��
        }
    }
    private void OnAnimationComplete(object sender, EventObject eventObject)
    {
         if (eventObject.animationState.name == StartAnimation)
         {
            // 3. armature.display ��һ�� GameObject������ GetComponent
            GameObject armatureGameObject = eventObject.armature.display as GameObject;
            if (armatureGameObject != null)
            {
                // 4. �ڸ� GameObject �ϻ�ȡ UnityArmatureComponent
                UnityArmatureComponent hightObj = armatureGameObject.GetComponent<UnityArmatureComponent>();
                if (hightObj != null)
                {
                    // 5. ����ѭ������
                    hightObj.animation.Play(StayAnimation, 0);  // 0��ʾ����ѭ��
                }
            }
         }
    }
    //TTOD1 ����ҵ����ʧʱ��������
    public void DisPlayHight(UnityArmatureComponent hightObj)
    {
        if (hightObj != null)
        {
            hightObj.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationComplete);
            hightObj.animation.Play("<None>");
            if (FistMonsterHightarmatureComponent != null)
                FistMonsterHightarmatureComponent = null;
            hightObj.transform.parent.gameObject.SetActive(false);
        }
    }
    #endregion[������Ҹ���]

    #region[������Ҷ���]
    /// ��ʼѭ���Ŵ�/�ָ�����
    /// </summary>
    // ��¼��������������ж�ʲôʱ��ֹͣѭ��
    [HideInInspector] public int totalCoins;   // �������ɵĽ������
    [HideInInspector] public int arrivedCoins; // �ѵ���Ŀ��λ�õĽ����
    private Coroutine blinkCoroutine;  // ���ڴ洢ѭ��Э�̵����ã��Ա���ʱֹͣ
    public void StartCoinEffectBlink()
    {
        // ���Ѿ���ѭ���������ظ�����
        if (blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(CoinEffectBlinkLoop());
        }
    }

    /// <summary>
    /// ֹͣѭ���Ŵ�/�ָ�����
    /// </summary>
    public void StopCoinEffectBlink()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        // �ָ�Ϊ������С
        if (coinText != null)
        {
            coinText.rectTransform.localScale = Vector3.one;
        }
        if (coinspattern_F != null)
        {
            RectTransform patternRect = coinspattern_F.GetComponent<RectTransform>();
            patternRect.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// ѭ���� coinText �� coinspattern_F �������Ŷ�����1.2 -> 1
    /// </summary>
    private IEnumerator CoinEffectBlinkLoop()
    {
        while (true)
        {
            // �Ŵ� 1.2
            yield return StartCoroutine(ScaleOverTime(coinText, 1.2f, 0.1f));
            yield return StartCoroutine(ScaleOverTime(coinspattern_F.gameObject, 1.2f, 0.1f));

            // ���Ż� 1
            yield return StartCoroutine(ScaleOverTime(coinText, 0.7f, 0.1f));
            yield return StartCoroutine(ScaleOverTime(coinspattern_F.gameObject, 0.7f, 0.1f));
        }
    }
    private IEnumerator ScaleOverTime(Text uiText, float targetScale, float duration)
    {
        if (uiText == null) yield break;

        RectTransform rt = uiText.rectTransform;
        Vector3 initialScale = rt.localScale;
        Vector3 finalScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rt.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.localScale = finalScale;
    }

    private IEnumerator ScaleOverTime(GameObject obj, float targetScale, float duration)
    {
        if (obj == null) yield break;
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector3 initialScale = rt.localScale;
        Vector3 finalScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rt.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.localScale = finalScale;
    }

    /// <summary>
    /// ����ö��ҵ���Ŀ��λ�ú󣬵��ô˷���
    /// </summary>
    public void OnCoinArrived(EnemyController enemyController)
    {
        arrivedCoins++;
        if (arrivedCoins == 1 && enemyController.isSpecialHealth)
        {
            UpdateCoinTextWithDOTween(enemyController.Enemycoins1 - enemyController.Enemycoins2,0.5f);
        }
        // ���ȫ����Ҷ��ѵ���Ŀ��λ�ã���ֹͣѭ������
        if (arrivedCoins >= totalCoins)
        {
            //TTOD1����Ȼ�����ظ���
            DisPlayHight(coinHightAmature);
            PanelThree_F.SetActive(false);
            StopCoinEffectBlink();
        }
    }
    #endregion[������Ҷ���]
    public IEnumerator ShowThreeNote(Vector2 backPos)
    {
        Time.timeScale = 0f;
        SetFirstMonsterCoinPos(backPos);
        PlayHight(FirstMonsterCoin_F.GetChild(0).GetComponent<UnityArmatureComponent>());
        // ��ͣ��Ϸʱ��
        yield return StartCoroutine(ShowThreeNoteAfterDelay(backPos));
    }
    private IEnumerator ShowThreeNoteAfterDelay(Vector2 backPos)
    {
        ThreeNote_FBool = true;
        PanelThree_F.SetActive(true);
        PanelThree panelThree = FindObjectOfType<PanelThree>();
        //Vector2 newbackPos = new Vector2(backPos.x - 55, backPos.y - 24);
        panelThree.UpdateHole(backPos, new Vector2(365f, 181f));
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner3").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        yield return StartCoroutine(ShowMultipleNotesCoroutine(ThreeNote_F, guidanceTexts));

    }
    public IEnumerator ShowFourNote()
    {
        FourNote_FBool = true;
        PanelOne_F.SetActive(true);
        Time.timeScale = 0f;
        // ��SpendMoney��EarMoneyʹ��ָ����ɫ��ǩ��������
        long spendMoney = PlayInforManager.Instance.playInfor.SpendMoney;
        long earMoney = PlayInforManager.Instance.playInfor.EarMoney;
        string guidanceText =
            $"A massive wave of monsters! Let me do the math.You just spent <color=#D3692A><size=38>{spendMoney}</size></color> money but earned <color=#D3692A><size=38>{earMoney}</size></color> gold coins. Your profit is:<color=#D3692A><size=38>{earMoney - spendMoney}</size></color> ! Wow, that's impressive!";
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        yield return StartCoroutine(ShowMultipleNotesCoroutine(FourNote_F, guidanceTexts));
    }

    private IEnumerator ShowFiveNoteAfterDelay()
    {
        yield return new WaitForSecondsRealtime(5f);
        ShowFiveNote();
    }

    //��ʾ5
    public void ShowFiveNote()
    {
        FiveNote_FBool = true;
        Time.timeScale = 0f;
        PanelOne_F.SetActive(true);
        ShowFiveNote2();
    }
    public void ShowFiveNote2()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner5").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        StartCoroutine(ShowMultipleNotesCoroutine(FiveNote_F, guidanceTexts));
    }
   
    public RectTransform textBox;    // ȷ���ڱ༭���и�ֵ

    // ��������������ڸ����û��ĵ������
    private int clickCount = 0;
    public IEnumerator ShowMultipleNotesCoroutine(
        GameObject noteObject,
        List<string> fullTexts
    )
    {
        // 1. �������
        noteObject.SetActive(true);
        GameManage.Instance.clickCount = true;
        if (FourNote_F != null && noteObject.name == FourNote_F.name)
        {
            // 1a. ��ʼ��Ч�����ȴ������
            yield return StartCoroutine(ShakeOnceCoroutine(noteObject.transform.GetChild(1).gameObject, duration: 0.3f, magnitude: 40f));
        }
        // 2. ��ȡ��ʾ�ı������TextMeshProUGUI��
        TextMeshProUGUI noteText = noteObject.GetComponentInChildren<TextMeshProUGUI>();
        if (noteText == null)
        {
            Debug.LogError("δ�ҵ� TextMeshProUGUI �����");
            yield break;
        }

        // 3. ��δָ�� textBox�����Զ���ȡ
        if (textBox == null)
        {
            // ����㼶�ṹ�У���3���ӽڵ�����һ��RectTransform
            if (SkillNote_F != null && noteObject.name == SkillNote_F.name)
            {
                textBox = noteObject.transform.GetChild(1).GetComponentInChildren<RectTransform>();
            }
            else
            {
                textBox = noteObject.transform.GetChild(2).GetComponentInChildren<RectTransform>();
            }
        }

        // 4. ÿ���ַ���ʾ��ʱ����
        float charInterval = 0.04f;

        // 5. �������о���
        for (int i = 0; i < fullTexts.Count; i++)
        {
            string fullText = fullTexts[i];
            if (string.IsNullOrEmpty(fullText)) continue;

            // (A) ����ʾ�¾���ǰ��������ı�
            noteText.text = "";

            // (B) ����������ʾ�þ䣨�ɵ������ => �������䲹ȫ��
            yield return StartCoroutine(
                TypeSentenceCoroutine(
                    noteText,
                    fullText,
                    textBox.rect.width,
                    charInterval
                )
            );

            // (C) �ȴ���ҡ��ٵ��һ�¡�������ʾ��һ��
            //     ���� �����Ҳ��������ͣ���ڵ�ǰ��
            yield return StartCoroutine(WaitForNextClick());

            // (D) ����Ѿ������һ�䣬ִ�н����߼�
            if (i == fullTexts.Count - 1)
            {
                // ��ȫ���һ�䲢ִ�н�������
                noteText.text = fullText;

                // �ȴ�����ٴε����ִ�к����߼�
                //yield return StartCoroutine(WaitForNextClick());
                noteObject.SetActive(false);
                GameManage.Instance.clickCount = false;
                if (SkillNote_F != null && noteObject.name == SkillNote_F.name)
                {
                    StartCoroutine(SkillNoteNoteCompletion(noteObject));
                }
                else
                {
                    HandleNoteCompletion(noteObject); // ��ĺ����߼�
                }
            }
        }
    }

    /// <summary>
    /// ���С����ִ��һ���䣨���Զ����У��������������ȫ���䡱��
    /// </summary>
    private IEnumerator TypeSentenceCoroutine(
        TextMeshProUGUI noteText,
        string sentence,
        float textBoxWidth,
        float charInterval
    )
    {
        // ���
        noteText.text = "";

        // ���Ϊ�������б�
        List<string> words = SplitIntoWords(sentence);

        // ����ƴ������ʾ����
        string displayedSoFar = "";
        // ��ǰ������
        string currentLine = "";

        // �Ƿ�Ҫ����ʣ����У����䣩
        bool skipRemaining = false;

        // ��1���������ƴ�ӣ�����Ƿ���Ҫ����
        for (int w = 0; w < words.Count; w++)
        {
            if (skipRemaining) break;

            // ���԰���һ������ƴ������
            string testLine = string.IsNullOrEmpty(currentLine)
                ? words[w]
                : currentLine + " " + words[w];

            // ������һ�е����ȿ��
            Vector2 preferredSize = noteText.GetPreferredValues(displayedSoFar + testLine);

            // ������ textBox ��� => �Ȱѡ���һ�С�������ʾ
            if (preferredSize.x > textBoxWidth)
            {
                if (!string.IsNullOrEmpty(currentLine))
                {
                    // ������ʾ��һ��
                    yield return StartCoroutine(
                        DisplayLine(
                            noteText,
                            displayedSoFar,
                            currentLine,
                            charInterval,
                            onClickSkip: () =>
                            {
                                // �����ҵ�� => ������������ʣ��
                                skipRemaining = true;
                            }
                        )
                    );

                    if (skipRemaining) break;

                    // ������һ����ʾ��� => ����
                    displayedSoFar += currentLine + "\n";
                }
                // ������һ�У��ӵ�ǰ���ʿ�ʼ
                currentLine = words[w];
            }
            else
            {
                // û��������ƴ�� currentLine
                currentLine = testLine;
            }
        }

        // ��2������ѭ�����������������һ������û��ʾ
        if (!string.IsNullOrEmpty(currentLine) && !skipRemaining)
        {
            yield return StartCoroutine(
                DisplayLine(
                    noteText,
                    displayedSoFar,
                    currentLine,
                    charInterval,
                    onClickSkip: () =>
                    {
                        skipRemaining = true;
                    }
                )
            );

            if (!skipRemaining)
            {
                // ��û�б����� => ����һ���ۼ�
                displayedSoFar += currentLine;
            }
        }

        // ��3���������� => ֱ�����䲹ȫ
        if (skipRemaining)
        {
            noteText.text = sentence;
        }
        else
        {
            // �����������������������
            noteText.text = displayedSoFar;
        }
    }

    /// <summary>
    /// ���м���Э�̡���������ʾ���У������ҵ�� => ���̲�ȫ���в����Ҫ�������䡣
    /// </summary>
    private IEnumerator DisplayLine(
       TextMeshProUGUI noteText,
       string displayedSoFar,
       string lineToDisplay,
       float charInterval,
       System.Action onClickSkip
    )
    {
        // ʹ�� StringBuilder �Ż��ַ���ƴ��
        StringBuilder lineBuffer = new StringBuilder();

        for (int i = 0; i < lineToDisplay.Length; i++)
        {
            // ����Ƿ�����������
            if (clickCount > 0)
            {
                clickCount--; // ����һ�ε��
                onClickSkip?.Invoke();
                // ֱ�Ӱѱ���ʣ��ȫ����ʾ�����������ַŴ�
                string remaining = lineToDisplay.Substring(i);
                string processedRemaining = WrapDigitsWithSize(remaining);
                noteText.text = displayedSoFar + lineBuffer.ToString() + processedRemaining;
                yield break;
            }

            char currentChar = lineToDisplay[i];

            if (currentChar == '<')
            {
                // �����ı���ǩ
                int tagEndIndex = lineToDisplay.IndexOf('>', i);
                if (tagEndIndex == -1)
                {
                    // û�ҵ� '>'���͵���ͨ�ַ�
                    lineBuffer.Append(currentChar);
                }
                else
                {
                    // һ���԰����� <...> ��ǩ����
                    string fullTag = lineToDisplay.Substring(i, tagEndIndex - i + 1);
                    lineBuffer.Append(fullTag);
                    i = tagEndIndex; // ������ǩ��β
                }
            }
            else
            {
                if (char.IsDigit(currentChar))
                {
                    // �����ַ�������Ƿ�����������
                    int start = i;
                    while (i < lineToDisplay.Length && char.IsDigit(lineToDisplay[i]))
                    {
                        i++;
                    }
                    int length = i - start;
                    string number = lineToDisplay.Substring(start, length);
                    // ��ӷŴ��ǩ
                    lineBuffer.Append($"<size=38>{number}</size>");
                    i--; // ������������Ϊforѭ�����Զ�����
                }
                else
                {
                    // ��ͨ�ַ�
                    lineBuffer.Append(currentChar);
                }

                // �ȴ�һС��ʱ��
                yield return new WaitForSecondsRealtime(charInterval);
            }

            // ÿ����һ���ַ�/��ǩ���͸��� Text
            noteText.text = displayedSoFar + lineBuffer.ToString();
        }
    }

    /// <summary>
    /// ���ַ����е������ַ���<size=38>��ǩ������������ȷ����Ӱ���������ı���ǩ
    /// </summary>
    /// <param name="input">�����ַ���</param>
    /// <returns>�������ַ���</returns>
    private string WrapDigitsWithSize(string input)
    {
        StringBuilder result = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            if (input[i] == '<')
            {
                // �����ı���ǩ
                int tagEndIndex = input.IndexOf('>', i);
                if (tagEndIndex == -1)
                {
                    // û�ҵ� '>'���͵���ͨ�ַ�
                    result.Append(input[i]);
                    i++;
                }
                else
                {
                    // һ���԰����� <...> ��ǩ����
                    string fullTag = input.Substring(i, tagEndIndex - i + 1);
                    result.Append(fullTag);
                    i = tagEndIndex + 1;
                }
            }
            else if (char.IsDigit(input[i]))
            {
                // �����ַ�������Ƿ�����������
                int start = i;
                while (i < input.Length && char.IsDigit(input[i]))
                {
                    i++;
                }
                int length = i - start;
                string number = input.Substring(start, length);
                // ��ӷŴ��ǩ
                result.Append($"<size=38>{number}</size>");
            }
            else
            {
                // ��ͨ�ַ�
                result.Append(input[i]);
                i++;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// �ȴ���ҡ����һ�¡����ټ�������ǿ������һ�ε���ǡ��µĵ������
    /// </summary>
    private IEnumerator WaitForNextClick()
    {
        // ����Ƿ��Ѿ���δ����ĵ��
        if (clickCount > 0)
        {
            clickCount--; // ����һ�ε��
            yield break;
        }

        // 1) �����ʱ��һ��ڰ�����꣨������һ�ε��˻�û�ɿ���
        //    ��ô�ȵ������֣�����һ�γ������ж�Ϊ���ε��
        while (Input.GetMouseButton(0))
        {
            yield return null;
        }

        // 2) �ȴ���һ�������İ���
        while (clickCount == 0)
        {
            yield return null;
        }

        // 3) ����һ�ε��
        clickCount--;
    }
    // ��Э��
    private IEnumerator ShakeOnceCoroutine(GameObject target, float duration, float magnitude)
    {
        Vector3 originalPosition = target.transform.localPosition;
        float halfDuration = duration / 2f;
        float timer = 0f;

        // ����һ����������ƫ������ȷ�����������϶��б仯
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        Vector3 offset = randomDirection * magnitude;

        // ��ԭλ���ƶ���ƫ��λ��
        while (timer < halfDuration)
        {
            float t = timer / halfDuration;
            target.transform.localPosition = Vector3.Lerp(originalPosition, originalPosition + offset, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        // ȷ���ﵽƫ��λ��
        target.transform.localPosition = originalPosition + offset;

        // ��ƫ��λ�÷��ص�ԭλ��
        timer = 0f;
        while (timer < halfDuration)
        {
            float t = timer / halfDuration;
            target.transform.localPosition = Vector3.Lerp(originalPosition + offset, originalPosition, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        // ȷ���ص�ԭλ��
        target.transform.localPosition = originalPosition;
    }
    private void HandleNoteCompletion(GameObject noteObject)
    {
        if (FirstNote_F != null && noteObject.name == FirstNote_F.name)
        {
            StartCoroutine(PlayEnemyNote(EnemiesComeArmature));
            StartCoroutine(SpawnPowerBuffDoorDelay());
        }

        if (PanelOne_F != null && PanelOne_F.activeSelf)
        {
            PanelOne_F.SetActive(false);
        }

        if (TwoNote_F != null && noteObject.name == TwoNote_F.name)
        {
            TwoNote_FBool = false;
            if (Panel_F != null && Panel_F.activeSelf)
            {
                Panel_F.SetActive(false);
            }
            Time.timeScale = 1f;
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DisPlayHight();
            }
        }

        if (ThreeNote_F != null && noteObject.name == ThreeNote_F.name)
        {
            //if (PanelThree_F != null && PanelThree_F.activeSelf)
            //{
            //    PanelThree_F.SetActive(false);
            //}
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                StartCoroutine(ReSetMovePlayer());
            }
            DisPlayHight(FirstMonsterCoin_F.GetChild(0).GetComponent<UnityArmatureComponent>());
            ThreeNote_FBool = false;
            StartCoroutine(PlayEnemyNote(MassiveEnemiesComeArmature));
        }

        if (FourNote_F != null && noteObject.name == FourNote_F.name)
        {
            Time.timeScale = 1f;
            StartCoroutine(PlayEnemyNote(BossComeArmature));
            GameObject Boss = Instantiate(Resources.Load<GameObject>("Prefabs/Boss"));
            EnemyController enemyController = Boss.GetComponent<EnemyController>();
            enemyController.isInitialBoss = true;
            Boss.transform.position = PreController.Instance.EnemyPoint;
            StartCoroutine(ShowFiveNoteAfterDelay());
        }

        if (FiveNote_F != null && noteObject.name == FiveNote_F.name)
        {
            Time.timeScale = 1f;
        }
      
    }
    private IEnumerator SkillNoteNoteCompletion(GameObject noteObject)
    {
        yield return StartCoroutine(ShowSkillGuideCoroutine(3));  // ��ʾ�񱩼�������
    }


    //���Ź�����Ϯ����
    private IEnumerator PlayEnemyNote(UnityArmatureComponent armatureComponent)
    {
        AudioManage.Instance.PlaySFX("bossshow", null);
        armatureComponent.transform.parent.gameObject.SetActive(true);
        armatureComponent.animation.Play("newAnimation");
        yield return new WaitForSecondsRealtime(armatureComponent.animation.GetState("newAnimation")._duration);
        if(armatureComponent == EnemiesComeArmature)
        {
            StartCoroutine(FirstNote_FBoolDelay());

        }
        if (armatureComponent == MassiveEnemiesComeArmature)
        {
            StartCoroutine(TwoBeThreeDelay());
        }
        armatureComponent.animation.Play("<None>");
        armatureComponent.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator FirstNote_FBoolDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        FirstNote_FBool = true;
    }
    private IEnumerator TwoBeThreeDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        TwoBeThree = true;
    }
    private IEnumerator SpawnPowerBuffDoorDelay()
    {
        yield return new WaitForSecondsRealtime(10f);
        SpawnPowerBuffDoor();
    }
    //����ǿ����
    public void SpawnPowerBuffDoor()
    {
        Vector3 spawnPowerBuffDoorPoint = new Vector3(0, 5.8f, 0f);//
        GameObject PowerBuffDoor = Instantiate(Resources.Load<GameObject>("Prefabs/Skill/SpecialBuffDoor"), spawnPowerBuffDoorPoint, Quaternion.identity);
        PreController.Instance.FixSortLayer(PowerBuffDoor);
    }
    //�ָ�����ƶ�
    public IEnumerator ReSetMovePlayer()
    {
        yield return new WaitForSecondsRealtime(1f);
        PreController.Instance.PlayisMove = true;
       
    }
    #endregion[������ʾ]
    //�޸ĳ����ֿ���
    //private void HandleNewbieGuide()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0); // ��ȡ��һ��������
    //        Vector3 touchPos = touch.position;
    //        Vector2 localPoint;

    //        // ����Ļ��ת��ΪCanvas�ı��ص�
    //        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, touchPos, null, out localPoint);

    //        switch (touch.phase)
    //        {
    //            case TouchPhase.Began:
    //            case TouchPhase.Moved:
    //            case TouchPhase.Stationary:
    //                if (isInside)
    //                {
    //                    // ����GuidCircle�ڼ�ͷ֮���ƶ�
    //                    Vector3 leftLimit = GuidArrowL.rectTransform.anchoredPosition;
    //                    Vector3 rightLimit = GuidArrowR.rectTransform.anchoredPosition;

    //                    float clampedX = Mathf.Clamp(localPoint.x, leftLimit.x, rightLimit.x);
    //                    float clampedY = Mathf.Clamp(localPoint.y, leftLimit.y, rightLimit.y);

    //                    // ����GuidCircle��λ�ã�����Y�᲻��
    //                    GuidCircle.rectTransform.anchoredPosition = new Vector2(clampedX, GuidCircle.rectTransform.anchoredPosition.y);
    //                    Debug.Log($"GuidCircle λ��: {GuidCircle.rectTransform.anchoredPosition}");

    //                    // �ƶ����
    //                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, Camera.main.nearClipPlane));
    //                    worldPos.z = 0;
    //                    worldPos.y = player.transform.position.y;
    //                    worldPos.x = Mathf.Clamp(worldPos.x, -1.5f, 1.5f);
    //                    player.transform.position = worldPos;
    //                    Debug.Log($"Player λ��: {worldPos}");
    //                }
    //                break;

    //            case TouchPhase.Ended:
    //            case TouchPhase.Canceled:
    //                // �ɿ���ָʱ������GuidCircle
    //                GuidCircle.transform.parent.gameObject.SetActive(false);
    //                // �����Ҫ����Guidfinger������ȡ��ע�����´���
    //                // Guidfinger.transform.parent.gameObject.SetActive(false);

    //                AccountManager.Instance.isGuid = true;
    //                GameManage.Instance.SwitchState(GameState.Running);
    //                break;
    //        }
    //    }
    //}
    #region[��ը�ͱ�������]
    void UapdateBuffBack(int FrozenBuffCount, int BalstBuffCount)
    {
        if (FrozenBuffCount > 0)
            buffForzenBack.sprite = buffForzenImages[1];
        else
            buffForzenBack.sprite = buffForzenImages[0];
        if (BalstBuffCount > 0)
            buffBlastBack.sprite = buffBlastImages[1];
        else
            buffBlastBack.sprite = buffBlastImages[0];
    }

    // �л���ͣ�ͼ�����Ϸ��״̬
    public void OnpauseClicked()
    {
        Time.timeScale = 0f; // ��ͣ��Ϸ
        PausePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/InPausePanel"));
        PausePanel.transform.SetParent(transform.parent, false);
        PausePanel.transform.localPosition = Vector3.zero;
    }
    public void UpdateBuffText(int FrozenBuffCount, int BalstBuffCount)
    {
        buffFrozenText.text = $"{FrozenBuffCount}";
        buffBlastText.text = $"{BalstBuffCount}";
        UapdateBuffBack(FrozenBuffCount, BalstBuffCount);
    }
    void ToggleBlast()
    {
        //ִ��ȫ����ը����
        if (PlayInforManager.Instance.playInfor.BalstBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.BalstBuffCount--;
            if (GameFlowManager.Instance.currentLevelIndex == 0)
            {
                HideSkillGuide();
            }
            UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
            SpawnPlane().Forget();
        }
    }
    void ToggleFrozen()
    {
        //ִ��ȫ����������
        if (PlayInforManager.Instance.playInfor.FrozenBuffCount > 0)
        {
            PlayInforManager.Instance.playInfor.FrozenBuffCount--;
            if (GameFlowManager.Instance.currentLevelIndex == 0)
            {
                HideSkillGuide();
            }
            UpdateBuffText(PlayInforManager.Instance.playInfor.FrozenBuffCount, PlayInforManager.Instance.playInfor.BalstBuffCount);
            FrozenBombEffect(new Vector3(0, 3, 0)).Forget();
        }
    }
   
    public async UniTask SpawnPlane()
    {
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // ���ɷɻ�����Ļ�ײ�
        Debug.Log("Plane spawned!");
    }
    #endregion
    #region [ȫ�������߼�]

    // ����ը��Ч��
    private async UniTask FrozenBombEffect(Vector3 ForzenPoint)
    {
        // ���ű���Ч������
        Forzen_F.SetActive(true);
        UnityArmatureComponent forzenArmature = Forzen_F.transform.GetChild(0).GetComponentInChildren<UnityArmatureComponent>();
        if (forzenArmature != null)
        {
            forzenArmature.animation.Play("start", 1); // ���ű�������
            await UniTask.Delay(TimeSpan.FromSeconds(forzenArmature.animation.GetState("start")._duration));
        }
        forzenArmature.animation.Play("stay", -1); // ���ű�������
        // ��ͣ���е��˺ͱ���
        FreezeAllEnemiesAndChests(ForzenPoint, ConfigManager.Instance.Tables.TableTransmitConfig.Get(1).DamageScope);
        // �ȴ� 5 ��
        await UniTask.Delay(5000);
        // �������Ч��
        forzenArmature.animation.Play("end", 1); // ���ű�������
        await UniTask.Delay(TimeSpan.FromSeconds(forzenArmature.animation.GetState("end")._duration));
        UnfreezeAllEnemiesAndChests(ForzenPoint, ConfigManager.Instance.Tables.TableTransmitConfig.Get(1).DamageScope);
        forzenArmature.animation.Play("<None>", -1); // ���ű�������
    }


    private void FreezeAllEnemiesAndChests(Vector3 FreezePos, float width)
    {
        GameManage.Instance.isFrozen = true;
        PreController.Instance.isFrozen = true;
        // ���ݱ����㶨����η�Χ
        Vector3 topLeft = new Vector3(FreezePos.x - width / 2, FreezePos.y + width / 2, FreezePos.z);
        Vector3 bottomRight = new Vector3(FreezePos.x + width / 2, FreezePos.y - width / 2, FreezePos.z);
        // �ҵ��������ھ��η�Χ�ڵĵ���
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;

            if (IsWithinRectangle(enemyPos, topLeft, bottomRight))
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead)
                {
                    enemyController.isFrozen = true; // �������
                }
            }
        }

        // �ҵ��������ھ��η�Χ�ڵı���
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject chest in chests)
        {
            Vector3 chestPos = chest.transform.position;

            if (IsWithinRectangle(chestPos, topLeft, bottomRight))
            {
                ChestController chestController = chest.GetComponent<ChestController>();
                if (chestController != null && chestController.isVise)
                {
                    chestController.isFrozen = true; // ���ᱦ��
                }
            }
        }
    }

    // �����������������λ���Ƿ��ھ��η�Χ��
    private bool IsWithinRectangle(Vector3 position, Vector3 topLeft, Vector3 bottomRight)
    {
        return position.x >= topLeft.x && position.x <= bottomRight.x &&
               position.y <= topLeft.y && position.y >= bottomRight.y;
    }

    // �������Ч��
    private void UnfreezeAllEnemiesAndChests(Vector3 FreezePos, float width)
    {
        GameManage.Instance.SetFrozenState(false);
        PreController.Instance.isFrozen = false;
        Debug.Log("�������=========================��!��");
        // ���ݱ����㶨����η�Χ
        Vector3 topLeft = new Vector3(FreezePos.x - width / 2, FreezePos.y + width / 2, FreezePos.z);
        Vector3 bottomRight = new Vector3(FreezePos.x + width / 2, FreezePos.y - width / 2, FreezePos.z);
        // �ҵ��������ھ��η�Χ�ڵĵ���
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;

            if (IsWithinRectangle(enemyPos, topLeft, bottomRight))
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead)
                {
                    enemyController.isFrozen = false; // �������
                }
            }
        }

        // �ҵ��������ھ��η�Χ�ڵı���
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject chest in chests)
        {
            Vector3 chestPos = chest.transform.position;

            if (IsWithinRectangle(chestPos, topLeft, bottomRight))
            {
                ChestController chestController = chest.GetComponent<ChestController>();
                if (chestController != null && chestController.isVise)
                {
                    chestController.isFrozen = false; // ���ᱦ��
                }
            }
        }
    }
    #endregion
    #region //ԭʼ��ǹ�߼�

    #region//���似��
    public IEnumerator  ShowSkillGuide(int indexChest)
    {
        // ��ʾ�������ȴ��������
        yield return StartCoroutine(ShowSkillGuideCoroutine(3));  // ��ʾ�񱩼�������
    }
    private Vector2 InitialPos = new Vector2(0, 0);
    public Vector2[] SkillGuidPos = new Vector2[]{ new Vector2(42f, -118f), new Vector2(42f, -241), new Vector2(12f, -265) };

    public IEnumerator SkillNoteNote()
    {
        Time.timeScale = 0f;
        string guidanceText = "Too many enemies! Quickly tap the button below to trigger Bounty Frenzy, then tap the screen rapidly to clear them out!";//ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner9").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        yield return StartCoroutine(ShowMultipleNotesCoroutine(SkillNote_F, guidanceTexts));
    }
    private IEnumerator ShowSkillGuideCoroutine(int indexChest)
    {
        Debug.Log("Time.timeScale ����Ϊ: " + Time.timeScale);
        Vector2 targetPos = new Vector2(22, -104);
        if (indexChest == 1)
        {
            SkillFinger_F1.SetActive(true);
            targetPos = SkillGuidPos[0];
            Vector2 startPos = SkillFinger_F.GetComponent<RectTransform>().anchoredPosition;
            InitialPos = startPos;
        }
        if (indexChest == 2)
        {
            SkillFinger_F2.SetActive(true);
            targetPos = SkillGuidPos[1];
        }
        if (indexChest == 3)
        {
            SkillFinger_F2.SetActive(true);
            targetPos = SkillGuidPos[2];
        }
        SkillFinger_F.gameObject.SetActive(true);

        // ����Ŀ��λ�ã�buffBlastBtn ��λ�ã�
        Debug.Log($"Ŀ��λ��: {targetPos}");

        // ��������ʱ��
        float moveDuration = 0.5f; // �ƶ�����ʱ��
        float clickDuration = 0.25f; // �����������ʱ��

        // �����ƶ���Ŀ��λ��
        Debug.Log("�ƶ�������ʼ");
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(InitialPos, targetPos, elapsed / moveDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = targetPos;
        Debug.Log("�ƶ��������");

        // ��ʼ�ظ��������
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(SkillFinger_F.transform.DOScale(SkillFinger_F.transform.localScale * 1.2f, 0.2f).SetEase(Ease.InOutSine))
                     .Append(SkillFinger_F.transform.DOScale(SkillFinger_F.transform.localScale, 0.2f).SetEase(Ease.InOutSine))
                     .SetLoops(-1) // ����ѭ��
                     .SetUpdate(true); // ʹ�ò�����ʱ��
        clickSequence.Play();
        Debug.Log("��ʼ�ظ��������");

        // �ȴ� buffBlastBtn �����
        bool buttonClicked = false;
        void OnBuffBlastBtnClicked()
        {
            buttonClicked = true;
            switch (indexChest)
            {
                case 1:
                    buffBlastBtn.onClick.RemoveListener(OnBuffBlastBtnClicked);
                    break;
                case 3:
                    specialButton.onClick.RemoveListener(OnBuffBlastBtnClicked);
                    break;
            }
        }

        switch (indexChest)
        {
            case 1:
                buffBlastBtn.onClick.AddListener(OnBuffBlastBtnClicked);
                break;
            case 3:
                specialButton.onClick.AddListener(OnBuffBlastBtnClicked);
                break;
        }

        // �ȴ���ť���
        Debug.Log("�ȴ� buffBlastBtn �����");
        while (!buttonClicked)
        {
            yield return null;
        }

        // ֹͣ�ظ��������
        clickSequence.Kill();
        Debug.Log("ֹͣ�ظ��������");

        // ��������
        SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = InitialPos;
        SkillNote_F.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);
        // �ָ���Ϸ
        Time.timeScale = 1f;
        Debug.Log("Time.timeScale �ָ�Ϊ: " + Time.timeScale);
    }


    /// <summary>
    /// ���ؼ�����ʾ
    /// </summary>
    public void HideSkillGuide()
    {
        if (SkillNote_F != null)
            SkillNote_F.SetActive(false);
        if (SkillFinger_F != null)
        {
            SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = InitialPos;
            SkillFinger_F.gameObject.SetActive(false);
        }
        SkillFinger_F1.SetActive(false);
        SkillFinger_F2.SetActive(false);

        // �ָ���Ϸ
        Time.timeScale = 1f;
    }
    #endregion
    #endregion
  

    #region[�񱩼��ܴ���]
    // �񱩼��ܰ�ť����¼�
    private async void OnSpecialButtonClicked()
    {
        if (isButtonActive)
        {
            isButtonActive = false;          // ���ð�ť״̬
            isSkillActive = true;            // �����
            specialButtonImage.sprite = Rampages[0];
            specialButton.interactable = false; // ��ť���ɵ��

            // ��ͣ���ܼ�ʱ
            elapsedTimeSkill = 0f;  // ���ü��ܼ�ʱ
            elapsedTimeBtn = 0f;    // ������ȴ��ʱ
            // ������ɺ󣬼���������ȴ��ʱ
            StartCoroutine(SkillActiveCoroutine());
        }
    }

    public float elapsedTimeBtn;         // ��ȴ��ʱ
    public float elapsedTimeSkill;       // ���ܳ�����ʱ
    private IEnumerator SkillActiveCoroutine()
    {
        ragepBJ = GameObject.Find("Player/rage");
        elapsedTimeSkill = 0f;
        isStayAnimationPlayed = false;
        isEndAnimationPlayed = false;
        // ���� start ����
        PlayDragonBonesAnimation("start", elapsedTimeSkill);
        // �ȴ�ֱ�� start ��������
        while (elapsedTimeSkill < activeTime)
        {
            elapsedTimeSkill += Time.unscaledDeltaTime; // ʹ�ò��� Time.timeScale Ӱ���ʱ��
                                                        // �� start ����������ɺ��л��� stay ����
            if (!isStayAnimationPlayed)
            {
                PlayDragonBonesAnimation("stay", elapsedTimeSkill);
            }
            if (!isEndAnimationPlayed)
            {
                // �ڼ��ܳ���ʱ������ 1 ���л��� end ����
                PlayDragonBonesAnimation("end", elapsedTimeSkill);

            }
            yield return null;
        }
        isSkillActive = false;
    }

    private bool isStayAnimationPlayed = false; // ��־ stay �����Ƿ��Ѿ�����
    private bool isEndAnimationPlayed = false;  // ��־ end �����Ƿ��Ѿ�����
    private UnityArmatureComponent armatureComponent;
    private GameObject ragepBJ ;
    private void PlayDragonBonesAnimation(string animationName, float elapsedTimeSkill)
    {
        // ȷ����Ҷ�����ڣ������� DragonBones ���
        if (ragepBJ == null)
        {
            Debug.LogError("�Ҳ�����ҵ� rage ����");
            return;
        }
        // ������Ҷ����µ� DragonBones �������
        armatureComponent = ragepBJ.GetComponent<UnityArmatureComponent>();

        if (armatureComponent != null)
        {
            // ����ָ���Ķ���
            if (animationName == "stay")
            {
                if (elapsedTimeSkill > armatureComponent.animation.GetState("start")._duration)
                {
                    armatureComponent.animation.Play("stay", -1);
                    isStayAnimationPlayed = true;
                }
            }
            else if (animationName == "end")
            { // ��� _duration �Ƿ���Ч
                if (elapsedTimeSkill > activeTime - 1)
                {
                    armatureComponent.animation.Play("end", 1);
                    Debug.Log($"End Animation Duration: {armatureComponent.animation.GetState("end")._duration}");
                    isEndAnimationPlayed = true; // ȷ�� end ����ֻ����һ��
                }
            }
            else
            {
                armatureComponent.animation.Play(animationName, 1);
            }
        }
        else
        {
            Debug.LogError("�Ҳ��� DragonBones ���������");
        }
    }

    private IEnumerator SpecialButtonCooldownCoroutine()
    {
        while (true)
        {
            // Start of cooldown, reset fillAmount to 0
            specialBac_F.fillAmount = 0f;

            // Wait for cooldown time (20 seconds)
            elapsedTimeBtn = 0f;
            while (elapsedTimeBtn < cooldownTime)
            {
                elapsedTimeBtn += Time.unscaledDeltaTime; // Use unscaled time for accuracy
                specialBac_F.fillAmount = elapsedTimeBtn / cooldownTime; // Update fill amount
                yield return null;
            }

            // Ensure fillAmount is set to 1 at the end of cooldown
            specialBac_F.fillAmount = 1f;

            // Activate button
            specialButtonImage.sprite = Rampages[1];
            specialButton.interactable = true;
            isButtonActive = true;
            // ��ʾ�������ȴ��������
            // ��ʾ�������ȴ��������
            if (PlayInforManager.Instance.playInfor.isFirstSpecial && GameFlowManager.Instance.currentLevelIndex == 1)
            {
                PreController.Instance.isRageSkill = true;
                PlayInforManager.Instance.playInfor.isFirstSpecial = false;
                AccountManager.Instance.SaveAccountData();
                yield return StartCoroutine(SkillNoteNote());  // ��ʾ�񱩼�������
                PreController.Instance.isRageSkill = false;
            }

            // Wait for button to be clicked
            while (isButtonActive)
            {
                yield return null;
            }

            // Skill is active, reset fillAmount if needed
            specialBac_F.fillAmount = 0f;

            // Wait for skill active time
            while (isSkillActive)
            {
                yield return null;
            }
        }
    }


    private void FireHomingBullet()
    {
        // ��ȡ���λ��
        if (player == null)
            player = GameObject.Find("Player");
        Vector3 playerPosition = player.transform.position;

        // ��ȡ���е��˺ͱ���
        List<GameObject> allTargets = new List<GameObject>();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeInHierarchy)
            {
                allTargets.Add(enemy);
            }
        }
        foreach (GameObject chest in chests)
        {
            if (chest.activeInHierarchy && chest.transform.position.y > 1.3)
            {
                allTargets.Add(chest);
            }
        }

        // ������ӽ���Զ����
        allTargets.Sort((a, b) =>
        {
            float distA = Vector3.Distance(playerPosition, a.transform.position);
            float distB = Vector3.Distance(playerPosition, b.transform.position);
            return distA.CompareTo(distB);
        });

        foreach (GameObject target in allTargets)
        {
            float targetHealth = 0f;
            if (target.CompareTag("Enemy"))
            {
                EnemyController enemyController = target.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead)
                {
                    targetHealth = enemyController.health;
                }
                else
                {
                    continue; // �����������ĵ���
                }
            }
            else if (target.CompareTag("Chest"))
            {
                ChestController chestController = target.GetComponent<ChestController>();
                if (chestController != null)
                {
                    targetHealth = chestController.chestHealth;
                }
                else
                {
                    continue; // ������Ч�ı���
                }
            }

            // ��������Ŀ����ӵ������˺�
            float totalIncomingDamage = 0f;
            foreach (BulletController bullet in PreController.Instance.flyingBullets)
            {
                if (bullet.target == target.transform)
                {
                    totalIncomingDamage += bullet.firepower;
                }
            }

            if (totalIncomingDamage < targetHealth)
            {
                // Ŀ����Ҫ�����˺��������ӵ�
                PreController.Instance.SpawnHomingBullet(playerPosition, target.transform);
                return; // ֻ����һ���ӵ�
            }
            else
            {
                // ��Ŀ��ķ����ӵ��Ѿ��㹻��������һ��Ŀ��
                continue;
            }
        }
    }
    #endregion[�񱩼��ܴ���]
}
