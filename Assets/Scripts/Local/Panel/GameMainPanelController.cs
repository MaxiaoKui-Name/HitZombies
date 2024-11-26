using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonBones;
using Hitzb;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;
using Sequence = DG.Tweening.Sequence;

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
    public GameObject Panel_F;
    public GameObject SkillFinger_F1;
    public GameObject SkillFinger_F2;
    public Image GuidArrowL;
    public Image GuidArrowR;
    private Image GuidCircle;
    //private Image Guidfinger;
    private Image GuidText;
    public GameObject FirstNote_F;
    public GameObject TwoNote_F;
    public GameObject ThreeNote_F;
    public GameObject FourNote_F;
    public GameObject SkillNote_F;
    public Image SkillFinger_F;

    //[Header("��ǹ")]
    //public Button RedBoxBtn_F;
    //public Image ChooseFinger_F;
    //public Button ChooseMaxBtn_F;
    //public GameObject ChooseGunNote_F;
    //public GameObject ChooseGun_F;

    public Image DieImg_F;

    [Header("Spawn Properties")]
    public float bombDropInterval;  // ը��Ͷ�����ʱ��
    public float planeSpeed = 2f;   // �ɻ��ƶ��ٶ�

    public GameObject player;
    // ����Canvas��RectTransform
    public RectTransform canvasRectTransform;

    public Vector3 coinspattern_F;
    // ������ڼ�⻬���ı���
    private Vector3 lastMousePosition;
    private float dragThreshold = 0.01f; // ������ֵ�����Ը�����Ҫ����
    private bool isDragging = false;


    //��
    // ������ť
    public Button specialButton;          // �񱩼��ܰ�ť
    private Image specialButtonImage;     // ��ť��ͼƬ���
    // ��ʱ����ر���
    private bool isButtonActive = false; // ��ť�Ƿ��ڿɵ��״̬
    public bool isSkillActive = false;  // �����Ƿ񼤻�
    private float cooldownTime = 20f;    // ��ȴʱ�� 20 ��
    private float activeTime = 5f;       // ���ܳ���ʱ�� 5 ��
    void Start()
    {
        GetAllChild(transform);
        // �ҵ��Ӷ����еİ�ť���ı���
        //��������
        canvasRectTransform = transform.parent.GetComponent<RectTransform>();
        GuidArrowL = childDic["GuidArrowL_F"].GetComponent<Image>();
        GuidArrowR = childDic["GuidArrowR_F"].GetComponent<Image>();
        GuidCircle = childDic["GuidCircle_F"].GetComponent<Image>();
        FirstNote_F = childDic["FirstNote_F"].gameObject;
        FirstNote_F.SetActive(false);
        TwoNote_F = childDic["TwoNote_F"].gameObject;
        TwoNote_F.SetActive(false);
        ThreeNote_F = childDic["ThreeNote_F"].gameObject;
        ThreeNote_F.SetActive(false);
        FourNote_F = childDic["FourNote_F"].gameObject;
        FourNote_F.SetActive(false);
        coinspattern_F = childDic["coinspattern_F"].GetComponent<RectTransform>().anchoredPosition;
        DieImg_F = childDic["DieImg_F"].GetComponent<Image>();
        DieImg_F.gameObject.SetActive(false);

        //ChooseFinger_F = childDic["Choosefinger_F"].GetComponent<Image>();
        //RedBoxBtn_F = childDic["RedBoxBtn_F"].GetComponent<Button>();
        //ChooseGunNote_F = childDic["ChooseGunNote_F"].gameObject;
        //ChooseMaxBtn_F = childDic["ChooseMaxBtn_F"].GetComponent<Button>();
        //ChooseGun_F = childDic["ChooseGun_F"].gameObject;
        SkillNote_F = childDic["SkillNote_F"].gameObject;
        SkillFinger_F = childDic["SkillFinger_F"].GetComponent<Image>();
        Panel_F = childDic["Panel_F"].gameObject;
        //RedBoxBtn_F.gameObject.SetActive(false);
        //ChooseGunNote_F.SetActive(false);
        //ChooseMaxBtn_F.gameObject.SetActive(false);
        //ChooseGun_F.gameObject.SetActive(false);
        SkillNote_F.gameObject.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);
        SkillFinger_F1 = childDic["SkillFinger_F1"].gameObject;
        SkillFinger_F2 = childDic["SkillFinger_F2"].gameObject;
        SkillFinger_F1.SetActive(false);
        SkillFinger_F2.SetActive(false);

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
        pauseButton.onClick.AddListener(TogglePause);
        buffFrozenBtn.onClick.AddListener(ToggleFrozen);
        buffBlastBtn.onClick.AddListener(ToggleBlast);
        //if (GameFlowManager.Instance.currentLevelIndex == 0)
        //{
        //    buffFrozenBtn.interactable = false;
        //    buffBlastBtn.interactable = false;
        //}
    
        if (GameFlowManager.Instance.currentLevelIndex == 0)
        {
            pauseButton.transform.parent.gameObject.SetActive(false);
            Panel_F.transform.gameObject.SetActive(false);
        }
        if (GameFlowManager.Instance.currentLevelIndex != 0)
        {
            Panel_F.transform.gameObject.SetActive(false);
            GuidArrowL.transform.parent.gameObject.SetActive(false);
        }
        //// ���RedBoxBtn_F���¼�����
        //RedBoxBtn_F.gameObject.AddComponent<RedBoxButtonHandler>().Initialize(this);
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
        specialButton.onClick.AddListener(OnSpecialButtonClicked);
        specialButtonImage.color = new Color(34, 32, 32, 255);
        specialButton.interactable = false; // ��ʼ���ɵ��
        // ��ʼ��ť����ȴЭ��
        StartCoroutine(SpecialButtonCooldownCoroutine());
        UpdateCoinText();
    }
    public bool isGuidAnimationPlaying = false; // ��־�Ƿ����ڲ�����������
    private bool hasGuidAnimationPlayed = false; // ��־���������Ƿ��Ѳ��Ź�
    void Update()
    {

        // ���������߼�
        if (GameManage.Instance.gameState == GameState.Guid)
        {
            if (!isGuidAnimationPlaying && !hasGuidAnimationPlayed)
            {
                StartCoroutine(RunGuidCircleAnimation());
            }
            else if (!isGuidAnimationPlaying && hasGuidAnimationPlayed)
            {
                HandleNewbieGuide();

            }
        }
        if (GameManage.Instance.gameState == GameState.Running)
        {
            // �����ܼ���ʱ�������Ļ����������ӵ�
            if (isSkillActive && Input.GetMouseButtonDown(0))
            {
                FireHomingBullet();
            }
        }

    }
    #region//�񱩼�����Ĵ���
    private void OnDestroy()
    {
        EventDispatcher.instance.UnRegist(EventNameDef.UPDATECOIN, (v) => UpdateCoinText());
        // ȷ������ʱ������������ٵ�����
        coinText = null;
    }

    void UpdateCoinText()
    {
        if (coinText == null)
        {
            Debug.LogWarning("coinText is null or destroyed");
            return; // ����������������ٵ����
        }
        // ʵʱ������ʾ�Ľ������
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum:N0}";
    }
    /// <summary>
    /// ���½���ı�����ӹ�������
    /// </summary>
    public void UpdateCoinTextWithDOTween(int AddCoin)
    {
        // ����UI��ʾ����Ч��
        int currentCoin = (int)(PlayInforManager.Instance.playInfor.coinNum); // �������ӵĽ����
        float duration = 1f; // ��������ʱ��
        int targetCoin = (int)PlayInforManager.Instance.playInfor.coinNum + AddCoin;

        DOTween.To(() => currentCoin, x =>
        {
            currentCoin = x;
            PlayInforManager.Instance.playInfor.coinNum = currentCoin;
            coinText.text = $"{currentCoin}";
        }, targetCoin, duration)
        .SetEase(Ease.Linear)
        .SetUpdate(true); // ʹ�ò�����ʱ�䣬ȷ������ͣʱ������Ȼ����); // Ensure it continues to update even when time is paused
    }


    private IEnumerator RunGuidCircleAnimation()
    {
        isGuidAnimationPlaying = true; // ��Ƕ������ڲ���
        // ��ͣ��Ϸ
        Time.timeScale = 0f;
        Debug.Log("Time.timeScale ����Ϊ: " + Time.timeScale);
        // ��ȡ RectTransform
        RectTransform guidCircleRect = GuidCircle.GetComponent<RectTransform>();

        // ��ȡ��߽���ұ߽��λ��
        Vector2 leftLimit = GuidArrowL.rectTransform.anchoredPosition;
        Vector2 rightLimit = GuidArrowR.rectTransform.anchoredPosition;
        Vector2 initialPos = guidCircleRect.anchoredPosition;

        // ��������
        float moveDuration = 1f; // ÿ���ƶ��ĳ���ʱ��

        // ʹ�� DOTween ������������
        Sequence guidSequence = DOTween.Sequence();
        guidSequence.Append(guidCircleRect.DOAnchorPos(new Vector2(leftLimit.x, initialPos.y), moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(new Vector2(rightLimit.x, initialPos.y), moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialPos, moveDuration).SetEase(Ease.InOutSine))
                    .SetUpdate(true) // ʹ�ò�����ʱ�䣬ȷ������ͣʱ������Ȼ����
                    .OnComplete(() =>
                    {
                        Debug.Log("���������������");
                        isGuidAnimationPlaying = false; // ����������ɣ��������������
                        hasGuidAnimationPlayed = true;   // ������������Ѳ���
                    });
        guidSequence.Play();
        // �ȴ��������
        yield return new WaitUntil(() => !isGuidAnimationPlaying);
        // ���������߼�������������ָ���Ϸʱ�䣨�����Ҫ��
        // Time.timeScale = 0f; // ������ͣ״̬
        Debug.Log("����������������ʼ����������");
    }
    private Vector3 targetPosition;

    private void HandleNewbieGuide()
    {
        // ��������������ڲ��ţ��򲻴����������
        if (isGuidAnimationPlaying)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            // ��¼��갴��ʱ��λ��
            lastMousePosition = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            float distance = Vector3.Distance(currentMousePosition, lastMousePosition);

            if (!isDragging && distance > dragThreshold)
            {
                // ��⵽������Ϊ�������л���Ϸ״̬
                GuidCircle.transform.parent.gameObject.SetActive(false);
                StartCoroutine(ShowFirstNoteAfterDelay()); // ����Ϸ��ʼ���ӳ�2����ʾFirstNote_F
                GameManage.Instance.SwitchState(GameState.Running);
                isDragging = true; // ȷ��ֻ�л�һ��
            }

            if (!isDragging)
            {
                // ��ס������ʱ����������ƶ�
                Vector3 mousePos = Input.mousePosition;
                Vector2 localPoint;

                // ����Ļ��ת��ΪCanvas�ı��ص�
                bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePos, null, out localPoint);
                if (isInside)
                {
                    // ����GuidCircle�ڼ�ͷ֮���ƶ�
                    Vector3 leftLimit = GuidArrowL.rectTransform.anchoredPosition;
                    Vector3 rightLimit = GuidArrowR.rectTransform.anchoredPosition;

                    float clampedX = Mathf.Clamp(localPoint.x, leftLimit.x, rightLimit.x);
                    // �������y��λ�ñ��ֲ���
                    GuidCircle.rectTransform.anchoredPosition = new Vector2(clampedX, GuidCircle.rectTransform.anchoredPosition.y);

                    // �ƶ����
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
                    if (player == null)
                        player = GameObject.Find("Player");
                    worldPos.z = 0;
                    worldPos.y = player.transform.position.y;
                    worldPos.x = Mathf.Clamp(worldPos.x, -1.5f, 1.5f);
                    player.transform.position = worldPos;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
            {
                // ���û�л�����ֻ���ɿ�ʱ�л�״̬
                GuidCircle.transform.parent.gameObject.SetActive(false);
                GameManage.Instance.SwitchState(GameState.Running);
            }
            // �����϶�״̬
            isDragging = false;
        }
    }
    public bool FirstNote_FBool = false;
    #region   //�°����ֹ�����޸Ĵ���
    private IEnumerator ShowFirstNoteAfterDelay()
    {
        //Debug.Log("ShowFirstNoteAfterDelay ��ʼ");
        yield return new WaitForSecondsRealtime(2f);
        ShowFirstNote();
    }
    // ��ʾ FirstNote_F
    public void ShowFirstNote()
    {
        Debug.Log("ShowFirstNote ������");
        StartCoroutine(ShowNoteCoroutine(FirstNote_F));
    }

    // ��ʾ TwoNote_F
    public void ShowTwoNote()
    {
        StartCoroutine(ShowNoteCoroutine(TwoNote_F));
    }

    // ��ʾ ThreeNote_F
    public void ShowThreeNote()
    {
        StartCoroutine(ShowNoteCoroutine(ThreeNote_F));
    }

    // ��ʾ FourNote_F
    public void ShowFourNote()
    {
        StartCoroutine(ShowNoteCoroutine(FourNote_F));
    }

    private IEnumerator ShowNoteCoroutine(GameObject noteObject)
    {
        Debug.Log($"{noteObject.name} ����Ϊ����");
        noteObject.SetActive(true);
        float elapsedTime = 0f;
        float duration = 6f;
        bool noteHidden = false;

        while (elapsedTime < duration && !noteHidden)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (noteObject.name == "FirstNote_F")
                {
                    FirstNote_FBool = true;
                }
                noteObject.SetActive(false);
                noteHidden = true;
                yield break;
            }
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!noteHidden)
        {
            if (noteObject.name == "FirstNote_F")
            {
                FirstNote_FBool = true;
            }
            noteObject.SetActive(false);
          
        }
    }
    #endregion
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
    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f; // ��ͣ��Ϸ
        }
        else
        {
            Time.timeScale = 1f; // ������Ϸ
        }
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
            FrozenBombEffect(new Vector3(0,3,0)).Forget();
        }
    }
    #region ȫ����ը�߼�
    public async UniTask SpawnPlane()
    {
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // ���ɷɻ�����Ļ�ײ�
        Debug.Log("Plane spawned!");
        //await MovePlaneAndDropBombs(plane);
        //if (plane != null)
        //{
        //    Destroy(plane);
        //}
    }
    //// �ɻ��ƶ���Ͷ��ը�����첽����
    //private async UniTask MovePlaneAndDropBombs(GameObject plane)
    //{
    //    float dropTime = 0f;
    //    bool isThrow = false;
    //    while (plane != null && plane.activeSelf && plane.transform.position.y < 6f)
    //    {
    //        plane.transform.Translate(Vector3.up * planeSpeed * Time.deltaTime);
    //        // TTOD1Ͷ��ը���߼�
    //        if (plane.transform.position.y > 0 && !isThrow)
    //        {
    //            isThrow = true;
    //            Vector3 bombPosition = PreController.Instance.RandomPosition(plane.transform.position);
    //            DropBomb(bombPosition).Forget();
    //        }
    //        // Yield control to allow other operations
    //        await UniTask.Yield();
    //    }
    //}
    //// Ͷ��ը�����첽��
    //private async UniTask DropBomb(Vector3 planePosition)
    //{
    //    GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/explode_01"), planePosition, Quaternion.identity);
    //    await BombExplosion(bomb, ConfigManager.Instance.Tables.TableTransmitConfig.Get(2).DamageScope);
    //}

    //// ը����ը��������������ˣ��첽��
    //private async UniTask BombExplosion(GameObject bomb, float width)
    //{
    //    UnityArmatureComponent bombArmature = bomb.GetComponentInChildren<UnityArmatureComponent>();
    //    if (bombArmature != null)
    //    {
    //        bombArmature.animation.Play("fly", 1); // ����һ�η��ж���
    //    }
    //    // ��ȡը��λ��
    //    Vector3 bombPos = bomb.transform.position;

    //    // ������η�Χ�����ϽǺ����½�
    //    Vector3 topLeft = new Vector3(bombPos.x - width / 2, bombPos.y + width / 2, bombPos.z);
    //    Vector3 bottomRight = new Vector3(bombPos.x + width / 2, bombPos.y - width / 2, bombPos.z);
    //    float DamageNum = ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TableBoxcontent.Get(7).Fires[0]).AtkRate * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total;
    //    // �ҵ���ը�پ��η�Χ�ڵĵ���
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //    foreach (GameObject enemy in enemies)
    //    {
    //        Vector3 enemyPos = enemy.transform.position;

    //        if (IsWithinRectangle(enemyPos, topLeft, bottomRight) && enemy.activeSelf)
    //        {
    //            EnemyController enemyController = enemy.GetComponent<EnemyController>();
    //            if (enemyController != null && !enemyController.isDead && enemyController.isVise)
    //            {
    //                enemyController.Enemycoins2 = 1;
    //                //TTOD1����ϵ���˻�������ϵ�� ����
    //                enemyController.TakeDamage(DamageNum, enemy); // �Ե�����ɼ��ߵ��˺�
    //            }
    //        }
    //    }
    //    // �ҵ��ھ��η�Χ�ڵı��䱬ը��ʧ
    //    GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
    //    foreach (GameObject chest in chests)
    //    {
    //        Vector3 chestPos = chest.transform.position;
    //        if (IsWithinRectangle(chestPos, topLeft, bottomRight))
    //        {
    //            ChestController chestController = chest.GetComponent<ChestController>();
    //            if (chestController != null && chestController.isVise)
    //            {
    //                chestController.TakeDamage(DamageNum, chest); // ���ᱦ��
    //            }
    //        }
    //    }
    //    await UniTask.Delay(1000);
    //    Destroy(bomb);
    //}
    #endregion
    #region ȫ�������߼�

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

    #region �����������߼�

    // �������������� ChooseFinger_F �Ķ���
    //public void StartChooseFingerAnimation()
    //{
    //    Debug.Log("StartChooseFingerAnimation called");
    //    ChooseFinger_F.gameObject.SetActive(true);
    //    StartCoroutine(FingerMoveLoop());
    //}

    //// ����������ֹͣ ChooseFinger_F �Ķ���
    //public void StopChooseFingerAnimation()
    //{
    //    Debug.Log("StopChooseFingerAnimation called");
    //    StopAllCoroutines();
    //    ChooseFinger_F.GetComponent<RectTransform>().anchoredPosition = Vector3.zero; // ����λ��
    //    ChooseFinger_F.gameObject.SetActive(false);
    //}

    //// Э�̣�ChooseFinger_F ѭ���ƶ���ʹ�÷�����ʱ�䣩
    //private IEnumerator FingerMoveLoop()
    //{
    //    Debug.Log("FingerMoveLoop started");
    //    RectTransform fingerRect = ChooseFinger_F.GetComponent<RectTransform>();
    //    Vector2 originalPos = fingerRect.anchoredPosition;
    //    Vector2 targetPos1 = originalPos + new Vector2(0, 40f);
    //    Vector2 targetPos2 = originalPos - new Vector2(0, 40f);
    //    while (true)
    //    {
    //        // �����ƶ�60
    //        yield return StartCoroutine(MoveFinger(fingerRect, originalPos, targetPos1, 0.5f));
    //        yield return StartCoroutine(MoveFinger(fingerRect, targetPos1, originalPos, 0.5f));
    //        // ֹͣ1�루ʹ�� WaitForSecondsRealtime��
    //        yield return new WaitForSecondsRealtime(1f);
    //        // �����ƶ���ԭλ
    //        yield return StartCoroutine(MoveFinger(fingerRect, originalPos, targetPos2, 0.5f));
    //        yield return StartCoroutine(MoveFinger(fingerRect, targetPos2, originalPos, 0.5f));
    //        // ֹͣ1�루ʹ�� WaitForSecondsRealtime��
    //        yield return new WaitForSecondsRealtime(1f);
    //    }
    //}

    //// Э�̣��ƶ� ChooseFinger_F��ʹ�÷�����ʱ�䣩
    //private IEnumerator MoveFinger(RectTransform finger, Vector2 from, Vector2 to, float duration)
    //{
    //    Debug.Log($"MoveFinger from {from} to {to} over {duration} seconds");
    //    float elapsed = 0f;
    //    while (elapsed < duration)
    //    {
    //        finger.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
    //        elapsed += Time.unscaledDeltaTime; // ʹ�÷�����ʱ��
    //        yield return null;
    //    }
    //    finger.anchoredPosition = to;
    //    Debug.Log($"MoveFinger to {to} completed");
    //}

    //// �����������ȴ� RedBoxBtn_F �ĳ���
    //public async UniTask WaitForRedBoxLongPress()
    //{
    //    var tcs = new UniTaskCompletionSource();

    //    // ��ȡ RedBoxButtonHandler ��������ûص�
    //    RedBoxButtonHandler handler = RedBoxBtn_F.GetComponent<RedBoxButtonHandler>();
    //    if (handler != null)
    //    {
    //        void OnLongPressHandler()
    //        {
    //            tcs.TrySetResult();
    //        }

    //        handler.OnLongPress += OnLongPressHandler;
    //        await tcs.Task;
    //        handler.OnLongPress -= OnLongPressHandler;
    //    }
    //}
    #endregion

    #region//���似��
    public async void ShowSkillGuide(int indexChest)
    {
        StartCoroutine(ShowSkillGuideCoroutine(indexChest));
    }
    private Vector2 InitialPos = new Vector2(0, 0);
    private IEnumerator ShowSkillGuideCoroutine(int indexChest)
    {
        Debug.Log("ShowSkillGuide() ����������");

        if (SkillNote_F == null || SkillFinger_F == null || buffBlastBtn == null)
        {
            Debug.LogError("SkillNote_F��SkillFinger_F �� buffBlastBtn δ��ȷ��ֵ��");
            yield break;
        }

        // ��ͣ��Ϸ
        Time.timeScale = 0f;
        Debug.Log("Time.timeScale ����Ϊ: " + Time.timeScale);
        // ��¼��ʼλ��
        // ���� SkillNote_F �� SkillFinger_F
        SkillNote_F.SetActive(true);
        Vector2 targetPos = new Vector2(22, -104);
        if (indexChest == 1)
        {
            SkillFinger_F1.SetActive(true);
            targetPos = new Vector2(42f, -118f);
            Vector2 startPos = SkillFinger_F.GetComponent<RectTransform>().anchoredPosition;
            InitialPos = startPos;
        }
        if (indexChest == 2)
        {
            SkillFinger_F2.SetActive(true);
            targetPos = new Vector2(42f, -241);
        }
        SkillFinger_F.gameObject.SetActive(true);
        Debug.Log("SkillFinger_F ����״̬: " + SkillFinger_F.gameObject.activeSelf);


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
        Vector3 originalScale = SkillFinger_F.GetComponent<RectTransform>().localScale;
        // ��ʼ�ظ��������
        // ʹ�� DOTween ����һ������ѭ���ĵ��������������Ϊʹ�ò�����ʱ��
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(SkillFinger_F.transform.DOScale(originalScale * 1.2f, 0.2f).SetEase(Ease.InOutSine))
                    .Append(SkillFinger_F.transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutSine))
                    .SetLoops(-1) // ����ѭ��
                    .SetUpdate(true); // ʹ�ò�����ʱ��
        clickSequence.Play();
        Debug.Log("��ʼ�ظ��������");

        // �ȴ� buffBlastBtn �����
        bool buttonClicked = false;
        void OnBuffBlastBtnClicked()
        {
            buttonClicked = true;
            buffBlastBtn.onClick.RemoveListener(OnBuffBlastBtnClicked);
        }

        buffBlastBtn.onClick.AddListener(OnBuffBlastBtnClicked);
        Debug.Log("�ȴ� buffBlastBtn �����");
        while (!buttonClicked)
        {
            yield return null;
        }

        // ֹͣ�ظ��������
        clickSequence.Kill();
        Debug.Log("ֹͣ�ظ��������");

        // ���������߼�����������ָ��Ԫ�أ��ָ���Ϸ��
        SkillNote_F.SetActive(false);
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
    #endregion//�񱩼�����Ĵ���


    // �񱩼��ܰ�ť����¼�
    private void OnSpecialButtonClicked()
    {
        if (isButtonActive)
        {
            isButtonActive = false;          // ���ð�ť״̬
            isSkillActive = true;            // �����
            specialButtonImage.color = new Color(34,32,32,255);
            specialButton.interactable = false; // ��ť���ɵ��

            // ��ʼ���ܳ���ʱ���Э��
            StartCoroutine(SkillActiveCoroutine());
        }
    }
    public float elapsedTimeBtn;         // ��ȴ��ʱ
    public float elapsedTimeSkill;       // ���ܳ�����ʱ
    private IEnumerator SkillActiveCoroutine()
    {
        elapsedTimeSkill = 0f;
        while (elapsedTimeSkill < activeTime)
        {
            elapsedTimeSkill += Time.unscaledDeltaTime; // ʹ�ò��� Time.timeScale Ӱ���ʱ��
            yield return null;
        }
        isSkillActive = false;
    }

    private IEnumerator SpecialButtonCooldownCoroutine()
    {
        while (true)
        {
            // �ȴ���ȴʱ��20��
            elapsedTimeBtn = 0f;
            while (elapsedTimeBtn < cooldownTime)
            {
                elapsedTimeBtn += Time.unscaledDeltaTime; // ʹ�ò��� Time.timeScale Ӱ���ʱ��
                yield return null;
            }

            // ���ť
            //specialButtonImage.color = Color.blue;
            specialButton.interactable = true;
            isButtonActive = true;

            // �ȴ���ť�����
            while (isButtonActive)
            {
                yield return null;
            }

            // �ȴ����ܳ���ʱ�����
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
}
