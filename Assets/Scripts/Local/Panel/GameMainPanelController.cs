using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonBones;
using Hitzb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
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
    public GameObject SkillNote_F;
    public Image SkillFinger_F;//��ָͼƬ


    public Image DieImg_F;

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
    private float dragThreshold = 3f; // ������ֵ�����Ը�����Ҫ����
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

    // ��־�ı��Ƿ���ȫ��ʾ
    private bool isTextFullyDisplayed = false;
    void Start()
    {
        GetAllChild(transform);

        // �ҵ��Ӷ����еİ�ť���ı���
        //��������
        canvasRectTransform = transform.parent.GetComponent<RectTransform>();
        GuidArrowL = childDic["GuidArrowL_F"].GetComponent<Image>();
        GuidArrowR = childDic["GuidArrowR_F"].GetComponent<Image>();
        GuidCircle = childDic["GuidCircle_F"].GetComponent<Image>();
        Guidfinger_F = childDic["Guidfinger_F"];
        FirstNote_F = childDic["FirstNote_F"].gameObject;
        FirstNote_F.SetActive(false);
        TwoNote_F = childDic["TwoNote_F"].gameObject;
        TwoNote_F.SetActive(false);
        ThreeNote_F = childDic["ThreeNote_F"].gameObject;
        ThreeNote_F.SetActive(false);
        FourNote_F = childDic["FourNote_F"].gameObject;
        FourNote_F.SetActive(false);
        coinspattern_F = childDic["coinspattern_F"].GetComponent<RectTransform>();
        DieImg_F = childDic["DieImg_F"].GetComponent<Image>();
        DieImg_F.gameObject.SetActive(false);

        SkillNote_F = childDic["SkillNote_F"].gameObject;
        SkillFinger_F = childDic["SkillFinger_F"].GetComponent<Image>();
        PanelOne_F = childDic["PanelOne_F"].gameObject;
        Panel_F = childDic["Panel_F"].gameObject;
        Panel_F.transform.gameObject.SetActive(false);
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
    }

    void Update()
    {
        // ����״̬�µ��߼�
        if (GameManage.Instance.gameState == GameState.Guid)
        {
            // ����δ���Ź�������������û�����ڲ��Ŷ�������ʼ������������
            if (!isGuidAnimationPlaying && !hasGuidAnimationPlayed)
            {
                StartCoroutine(RunGuidAnimation());
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
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.instance.UnRegist(EventNameDef.UPDATECOIN, (v) => UpdateCoinText());
        coinText = null;
    }

    private IEnumerator Onpause_Btn_FClicked()
    {
        pauseButton.interactable = false;
        yield return StartCoroutine(HandleButtonClickAnimation(transform));
        yield return StartCoroutine(ButtonBounceAnimation(pauseButton.GetComponent<RectTransform>(), OnpauseClicked));
    }

    void UpdateCoinText()
    {
        if (coinText == null) return;
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum:N0}";
    }

    public void UpdateCoinTextWithDOTween(int AddCoin)
    {
        int currentCoin = (int)(PlayInforManager.Instance.playInfor.coinNum);
        float duration = 1f;
        int targetCoin = (int)PlayInforManager.Instance.playInfor.coinNum + AddCoin;

        DOTween.To(() => currentCoin, x =>
        {
            currentCoin = x;
            PlayInforManager.Instance.playInfor.coinNum = currentCoin;
            coinText.text = $"{currentCoin}";
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
   private Vector2 initialskillFingerPos;

    private IEnumerator RunGuidAnimation()
    {
        isGuidAnimationPlaying = true;
        Time.timeScale = 0f; // ��ͣ��Ϸ�߼���ͻ����������

        // ��ʾ����������Ԫ��
        PanelOne_F.SetActive(true);
        GuidCircle.transform.parent.gameObject.SetActive(true);
        Guidfinger_F.gameObject.SetActive(true);
        RectTransform guidCircleRect = GuidCircle.GetComponent<RectTransform>();
        RectTransform skillFingerRect = Guidfinger_F.GetComponent<RectTransform>();
        initialskillFingerPos = skillFingerRect.anchoredPosition;

        Vector2 initialCirclePos = guidCircleRect.anchoredPosition;
        Vector2 initialFingerPos = skillFingerRect.anchoredPosition;

        Vector2 leftPos = GuidArrowL.rectTransform.anchoredPosition;
        Vector2 rightPos = GuidArrowR.rectTransform.anchoredPosition;
        float moveDuration = 1f;

        // ���ģ�⶯����һ�Σ�
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(skillFingerRect.DOAnchorPos(initialCirclePos, 0.5f).SetEase(Ease.InOutSine))
                     .Append(skillFingerRect.DOAnchorPos(new Vector2(initialCirclePos.x, initialCirclePos.y + 5), 0.2f).SetEase(Ease.InOutSine))
                     .Append(skillFingerRect.DOAnchorPos(initialCirclePos, 0.2f).SetEase(Ease.InOutSine));

        // �����ƶ�ģ��������һ�����أ�
        Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(guidCircleRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(skillFingerRect.DOAnchorPos(initialFingerPos, moveDuration).SetEase(Ease.InOutSine));

        // �ϲ��״�������������
        guidSequence = DOTween.Sequence();
        guidSequence.Append(clickSequence)
                    .Append(moveSequence)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        // �״������������
                        isGuidAnimationPlaying = false;
                        hasGuidAnimationPlayed = true;
                        isInitialGuideDone = true;
                        // �� guidSequence ����Ϊ null����������ѭ������
                        guidSequence = null;
                    });

        guidSequence.Play();

        yield return null;
    }

    /// <summary>
    /// ���״��������������󣬼��������룺
    /// - ����Ұ�����Ļ���϶�������ֵ���룬�������������ʼ��Ϸ��
    /// - �����һֱ���϶����򲥷�����ѭ��������������ֱ����⵽�϶�Ϊֹ��
    /// </summary>
    private void HandleNewbieGuide()
    {
        // �״ζ���������޶�������ʱ���������϶���Ϊ
        if (!isGuidAnimationPlaying && hasGuidAnimationPlayed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
                isDragging = false;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 currentMousePosition = Input.mousePosition;
                float distance = Vector3.Distance(currentMousePosition, lastMousePosition);

                if (!isDragging && distance > dragThreshold)
                {
                    // ��⵽����϶�
                    EndGuideAndStartGame();
                    isDragging = true;
                }
            }

            //if (Input.GetMouseButtonUp(0))
            //{
            //    // ���ֻ�ǵ��δ�϶���Ҳ��Ϊ��������
            //    if (!isDragging)
            //    {
            //        EndGuideAndStartGame();
            //    }
            //    isDragging = false;
            //}
        }

        // ���״�������ɺ�ʱ�����϶�����ʼѭ�������ظ���ʾ
        if (isInitialGuideDone && !isGuidAnimationPlaying && guidSequence == null)
        {
            StartLoopGuideAnimation();
        }
    }

    /// <summary>
    /// ��������ѭ������������������ֱ����ҷ����϶���ΪΪֹ��
    /// </summary>
    private void StartLoopGuideAnimation()
    {
        RectTransform guidCircleRect = GuidCircle.GetComponent<RectTransform>();
        RectTransform skillFingerRect = Guidfinger_F.GetComponent<RectTransform>();

        Vector2 initialCirclePos = guidCircleRect.anchoredPosition;
        Vector2 leftPos = GuidArrowL.rectTransform.anchoredPosition;
        Vector2 rightPos = GuidArrowR.rectTransform.anchoredPosition;
        float moveDuration = 1f;

        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(skillFingerRect.DOAnchorPos(initialCirclePos, 0.5f).SetEase(Ease.InOutSine))
                     .Append(skillFingerRect.DOAnchorPos(new Vector2(initialCirclePos.x, initialCirclePos.y + 5), 0.2f).SetEase(Ease.InOutSine))
                     .Append(skillFingerRect.DOAnchorPos(initialCirclePos, 0.2f).SetEase(Ease.InOutSine));


        guidSequence = DOTween.Sequence();
        guidSequence.Append(guidCircleRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(leftPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(rightPos, moveDuration).SetEase(Ease.InOutSine))
                    .Append(guidCircleRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine))
                    .Join(skillFingerRect.DOAnchorPos(initialCirclePos, moveDuration).SetEase(Ease.InOutSine));

        Sequence clickRetrunSequence = DOTween.Sequence();
        clickRetrunSequence.Append(skillFingerRect.DOAnchorPos(initialskillFingerPos, moveDuration).SetEase(Ease.InOutSine));

        Sequence guidSequenceAll = DOTween.Sequence();
        guidSequenceAll.Append(clickSequence)
                    .Append(guidSequence)
                    .Append(clickRetrunSequence)
                    .SetLoops(-1) // ����ѭ��
                    .SetUpdate(true);
    }

    /// <summary>
    /// ����⵽����϶�������Ϊʱ�������������л�����Ϸ����״̬��
    /// </summary>
    private void EndGuideAndStartGame()
    {
        if (guidSequence != null)
        {
            guidSequence.Kill();
            guidSequence = null;
        }

        isGuidAnimationPlaying = false;
        hasGuidAnimationPlayed = true;

        // �ָ���Ϸ
        Time.timeScale = 1f;
        PanelOne_F.SetActive(false);
        GuidCircle.transform.parent.gameObject.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);

        // �л���Ϸ״̬������״̬��ʾ����
        GameManage.Instance.SwitchState(GameState.Running);
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
        yield return new WaitForSecondsRealtime(3f);
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

    public IEnumerator ShowThreeNote()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner3").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        yield return StartCoroutine(ShowMultipleNotesCoroutine(ThreeNote_F, guidanceTexts));
    }

    public void ShowFourNote()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner4").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        StartCoroutine(ShowMultipleNotesCoroutine(FourNote_F, guidanceTexts));
    }


    /// <summary>
    /// ��������ַ��������ľ��ӽ������ŷָ�ɾ����б�
    /// </summary>
    /// <param name="text">Ҫ�ָ���ַ�����</param>
    /// <returns>�����������ӵ��б�</returns>
    public static List<string> SplitIntoSentences(string text)
    {
        // �������ľ��ӽ������ŵ�������ʽ
        string pattern = @"[^.!?]*[.!?]";

        // ʹ��������ʽƥ�����о���
        MatchCollection matches = Regex.Matches(text, pattern);

        List<string> sentences = new List<string>();

        foreach (Match match in matches)
        {
            // ȥ�����ܵĿհ��ַ�
            string sentence = match.Value.Trim();
            if (!string.IsNullOrEmpty(sentence))
            {
                sentences.Add(sentence);
            }
        }

        return sentences;
    }
    public List<string> SplitIntoWords(string text)
    {
        return new List<string>(text.Split(' '));
    }
    /// <summary>
    /// ��ʾ�����ʾ��Э�̣����������ʾ��ÿ����2������ʾ�꣬��ʾ���ɾ������ʾ��һ�䡣
    /// ���о�����ʾ��󣬵ȴ��û������������ʾ��
    /// </summary>
    /// <param name="noteObject">��ʾ��GameObject</param>
    /// <param name="fullTexts">��������ʾ�����б�</param>
    /// <returns></returns>
    ///    // �ı���� RectTransform
    public RectTransform textBox;    // ȷ���ڱ༭���и�ֵ
    private IEnumerator ShowMultipleNotesCoroutine(GameObject noteObject, List<string> fullTexts)
    {
        // ������ʾ����
        noteObject.SetActive(true);

        // ��ȡ��ʾ�ı����
        TextMeshProUGUI noteText = noteObject.GetComponentInChildren<TextMeshProUGUI>();
        if (textBox == null)
        {
            // �����ı����ǵ������Ӷ���������0��ʼ��
            textBox = noteObject.transform.GetChild(2).GetComponentInChildren<RectTransform>();
        }
        if (noteText == null)
        {
            Debug.LogError("δ�ҵ�TextMeshProUGUI�����");
            yield break;
        }

        // �������о��ӵ����ַ���
        int totalChars = 0;
        foreach (string text in fullTexts)
        {
            totalChars += text.Length;
        }

        // ��ֹ���ַ���Ϊ0��������������
        if (totalChars == 0)
        {
            Debug.LogWarning("fullTexts��û�����ݣ�");
            yield break;
        }

        // ����ÿ���ַ�����ʾ���ʱ��
        float totalDuration = 10f; // ����ʾʱ��10��
        float charInterval = totalDuration / totalChars;

        // ����ÿ������
        for (int i = 0; i < fullTexts.Count; i++)
        {
            string fullText = fullTexts[i];
            noteText.text = ""; // ��յ�ǰ�ı�

            // ��ֹ����Ϊ�գ�����
            if (string.IsNullOrWhiteSpace(fullText))
            {
                Debug.LogWarning($"fullTexts�еĵ�{i}��Ϊ�գ�");
                continue;
            }

            // ��ȡ�ı���Ŀ��
            float textBoxWidth = textBox.rect.width;

            // �����ӷָ�ɵ����б�
            List<string> words = SplitIntoWords(fullText);
            string currentLine = ""; // ��ǰ�е��ı�

            foreach (string word in words)
            {
                // ���Խ���ǰ������ӵ���ǰ�к��Ƿ�ᳬ�����
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                Vector2 preferredSize = noteText.GetPreferredValues(testLine);

                // ������־������ȷ�ϲ����Ƿ���ȷ
                // Debug.Log($"������: '{testLine}', Ԥ�ڿ��: {preferredSize.x}, �ı�����: {textBoxWidth}");

                if (preferredSize.x > textBoxWidth)
                {
                    // ��ǰ��������������ӵ���ʾ�ı�������
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        // ������ʾ��ǰ��
                        yield return StartCoroutine(DisplayLine(noteText, currentLine, charInterval));
                        noteText.text += "\n"; // ��ӻ��з�
                        currentLine = word; // ����ǰ�����Ƶ��µ�һ��
                    }
                }
                else
                {
                    // ��ǰ��δ����������ӵ���
                    currentLine = testLine;
                }
            }

            // ��ʾ���һ��
            if (!string.IsNullOrEmpty(currentLine))
            {
                yield return StartCoroutine(DisplayLine(noteText, currentLine, charInterval));
            }

            // �ж��Ƿ�Ϊ���һ��
            bool isLastSentence = (i == fullTexts.Count - 1);

            if (!isLastSentence)
            {
                // ����������һ�䣬�ȴ���ҵ����Ļ����ʾ��һ��
                yield return StartCoroutine(WaitForClick());
                noteText.text = ""; // ����ı�����ʾ��һ��
            }
            else
            {
                // ��������һ�䣬�ȴ���ҵ����Ļ��ִ�����غ���������
                yield return StartCoroutine(WaitForClick());
                // ������ʾ����
                noteObject.SetActive(false);
                FirstNote_FBool = true;

                //// ���Panel_F�Ǽ���״̬����������
                //if (Panel_F != null && Panel_F.activeSelf)
                //{
                //    Panel_F.SetActive(false);
                //}

                // �����ǰ��ʾ�����������TwoNote_F��ͬ��ִ���ض��߼�
                if (TwoNote_F != null && noteObject.name == TwoNote_F.name)
                {
                    TwoNote_FBool = false;
                    // ���Panel_F�Ǽ���״̬����������
                    if (Panel_F != null && Panel_F.activeSelf)
                    {
                        Panel_F.SetActive(false);
                    }
                    Time.timeScale = 1f;
                    PlayerController playerController = FindObjectOfType<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.DisPlayHight();
                        StartCoroutine(ReSetMovePlayer());
                    }
                    else
                    {
                        Debug.LogWarning("δ�ҵ�PlayerController�����");
                    }
                }

                // �����������ȫ��ʾ
                isTextFullyDisplayed = true;
            }
        }

        // Э�̽���
        yield break;
    }

    /// <summary>
    /// �ȴ���ҵ��������
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForClick()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                yield break; // ��ҵ�����˳��ȴ�
            }
            yield return null;
        }
    }

    private IEnumerator DisplayLine(TextMeshProUGUI noteText, string line, float charInterval)
    {
        foreach (char c in line)
        {
            noteText.text += c;
            yield return new WaitForSecondsRealtime(charInterval);
        }
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

        // ���� SkillNote_F �� SkillFinger_F
        SkillNote_F.SetActive(true);
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
                yield return StartCoroutine(ShowSkillGuideCoroutine(3));  // ��ʾ�񱩼�������
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
