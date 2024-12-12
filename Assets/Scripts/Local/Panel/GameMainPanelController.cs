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
    public Button pauseButton;   // 引用暂停按钮
    public Text coinText;        // 引用显示金币的文本框
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

    [Header("新手引导")]
    public GameObject PanelOne_F;//遮罩图片
    public GameObject Panel_F;//不遮罩
    public GameObject SkillFinger_F1;
    public GameObject SkillFinger_F2;
    public Image GuidArrowL;
    public Image GuidArrowR;
    private Image GuidCircle;
    private Transform Guidfinger_F;

    //private Image Guidfinger;
    private Image GuidText;//新手引导图片
    public GameObject FirstNote_F;
    public GameObject TwoNote_F;
    public GameObject ThreeNote_F;
    public GameObject FourNote_F;
    public GameObject SkillNote_F;
    public Image SkillFinger_F;//手指图片


    public Image DieImg_F;

    [Header("Spawn Properties")]
    public float bombDropInterval;  // 炸弹投掷间隔时间
    public float planeSpeed = 2f;   // 飞机移动速度

    public GameObject player;
    public GameObject PausePanel;
    // 引用Canvas的RectTransform
    public RectTransform canvasRectTransform;

    public Transform coinspattern_F;
    // 添加用于检测滑动的变量
    private Vector3 lastMousePosition;
    private float dragThreshold = 3f; // 滑动阈值，可以根据需要调整
    private bool isDragging = false;


    //狂暴
    // 新增按钮
    public Button specialButton;          // 狂暴技能按钮
    private Image specialBac_F;
    private Image specialButtonImage;     // 按钮的图片组件
    // 定时器相关变量
    private bool isButtonActive = false; // 按钮是否处于可点击状态
    public bool isSkillActive = false;  // 技能是否激活
    private float cooldownTime = 20f;    // 冷却时间 20 秒
    private float activeTime = 5f;       // 技能持续时间 5 秒
    public Sprite[] Rampages;

    public bool isGuidAnimationPlaying = false; // 标志是否正在播放引导动画
    private bool hasGuidAnimationPlayed = false; // 标志引导动画是否已播放过
    public bool FirstNote_FBool = false;
    public bool TwoNote_FBool = false;

    // 标志文本是否完全显示
    private bool isTextFullyDisplayed = false;
    void Start()
    {
        GetAllChild(transform);

        // 找到子对象中的按钮和文本框
        //新手引导
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
        // 添加暂停按钮的点击事件监听器
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
        // 初始化 specialButton
        specialButton = childDic["specialBtn_F"].GetComponent<Button>();
        specialButtonImage = specialButton.GetComponent<Image>();
        specialButtonImage.sprite = Rampages[0];
        specialBac_F = childDic["specialBac_F"].GetComponent<Image>();
        specialButton.onClick.AddListener(OnSpecialButtonClicked);
        specialButton.interactable = false; // 初始不可点击
        // 开始按钮的冷却协程
        StartCoroutine(SpecialButtonCooldownCoroutine());
        UpdateCoinText();
    }

    void Update()
    {
        // 引导状态下的逻辑
        if (GameManage.Instance.gameState == GameState.Guid)
        {
            // 若尚未播放过引导动画，且没有正在播放动画，则开始播放引导动画
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
            // 当技能激活时，检测点击屏幕发射子弹（此处为其他逻辑，略）
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
    #region [新手引导逻辑]
    /// <summary>
    /// 播放新手引导动画的协程:
    /// 1. 手指图标从初始位置移动至圆圈位置，模拟点击动作。
    /// 2. 手指与圆圈一起往左、右移动，回到初始位置，完成一次往返。
    /// </summary>
   private DG.Tweening.Sequence guidSequence;               // 引导动画序列引用
   private bool isInitialGuideDone = false;     // 首次引导动画是否完成
   private Vector2 initialskillFingerPos;

    private IEnumerator RunGuidAnimation()
    {
        isGuidAnimationPlaying = true;
        Time.timeScale = 0f; // 暂停游戏逻辑，突出引导动画

        // 显示引导界面与元素
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

        // 点击模拟动画（一次）
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(skillFingerRect.DOAnchorPos(initialCirclePos, 0.5f).SetEase(Ease.InOutSine))
                     .Append(skillFingerRect.DOAnchorPos(new Vector2(initialCirclePos.x, initialCirclePos.y + 5), 0.2f).SetEase(Ease.InOutSine))
                     .Append(skillFingerRect.DOAnchorPos(initialCirclePos, 0.2f).SetEase(Ease.InOutSine));

        // 左右移动模拟引导（一次来回）
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

        // 合并首次引导动画序列
        guidSequence = DOTween.Sequence();
        guidSequence.Append(clickSequence)
                    .Append(moveSequence)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        // 首次引导动画完成
                        isGuidAnimationPlaying = false;
                        hasGuidAnimationPlayed = true;
                        isInitialGuideDone = true;
                        // 将 guidSequence 设置为 null，允许启动循环动画
                        guidSequence = null;
                    });

        guidSequence.Play();

        yield return null;
    }

    /// <summary>
    /// 当首次引导动画结束后，检测玩家输入：
    /// - 若玩家按下屏幕并拖动超过阈值距离，则结束引导并开始游戏。
    /// - 若玩家一直不拖动，则播放无限循环的引导动画，直到检测到拖动为止。
    /// </summary>
    private void HandleNewbieGuide()
    {
        // 首次动画完成且无动画播放时，检查玩家拖动行为
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
                    // 检测到玩家拖动
                    EndGuideAndStartGame();
                    isDragging = true;
                }
            }

            //if (Input.GetMouseButtonUp(0))
            //{
            //    // 如果只是点击未拖动，也视为结束引导
            //    if (!isDragging)
            //    {
            //        EndGuideAndStartGame();
            //    }
            //    isDragging = false;
            //}
        }

        // 若首次引导完成后长时间无拖动，则开始循环动画重复提示
        if (isInitialGuideDone && !isGuidAnimationPlaying && guidSequence == null)
        {
            StartLoopGuideAnimation();
        }
    }

    /// <summary>
    /// 启动无限循环的左右往返动画，直到玩家发生拖动行为为止。
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
                    .SetLoops(-1) // 无限循环
                    .SetUpdate(true);
    }

    /// <summary>
    /// 当检测到玩家拖动或点击行为时，结束引导并切换至游戏运行状态。
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

        // 恢复游戏
        Time.timeScale = 1f;
        PanelOne_F.SetActive(false);
        GuidCircle.transform.parent.gameObject.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);

        // 切换游戏状态到运行状态（示例）
        GameManage.Instance.SwitchState(GameState.Running);
        StartCoroutine(ShowFirstNoteAfterDelay());
    }
    #endregion
    #region[新手提示]
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
    /// 将输入的字符串按中文句子结束符号分割成句子列表。
    /// </summary>
    /// <param name="text">要分割的字符串。</param>
    /// <returns>包含各个句子的列表。</returns>
    public static List<string> SplitIntoSentences(string text)
    {
        // 定义中文句子结束符号的正则表达式
        string pattern = @"[^.!?]*[.!?]";

        // 使用正则表达式匹配所有句子
        MatchCollection matches = Regex.Matches(text, pattern);

        List<string> sentences = new List<string>();

        foreach (Match match in matches)
        {
            // 去除可能的空白字符
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
    /// 显示多句提示的协程，逐句逐字显示，每句在2秒内显示完，显示完后删除并显示下一句。
    /// 所有句子显示完后，等待用户点击以隐藏提示。
    /// </summary>
    /// <param name="noteObject">提示的GameObject</param>
    /// <param name="fullTexts">完整的提示句子列表</param>
    /// <returns></returns>
    ///    // 文本框的 RectTransform
    public RectTransform textBox;    // 确保在编辑器中赋值
    private IEnumerator ShowMultipleNotesCoroutine(GameObject noteObject, List<string> fullTexts)
    {
        // 激活提示对象
        noteObject.SetActive(true);

        // 获取提示文本组件
        TextMeshProUGUI noteText = noteObject.GetComponentInChildren<TextMeshProUGUI>();
        if (textBox == null)
        {
            // 假设文本框是第三个子对象（索引从0开始）
            textBox = noteObject.transform.GetChild(2).GetComponentInChildren<RectTransform>();
        }
        if (noteText == null)
        {
            Debug.LogError("未找到TextMeshProUGUI组件！");
            yield break;
        }

        // 计算所有句子的总字符数
        int totalChars = 0;
        foreach (string text in fullTexts)
        {
            totalChars += text.Length;
        }

        // 防止总字符数为0，避免除以零错误
        if (totalChars == 0)
        {
            Debug.LogWarning("fullTexts中没有内容！");
            yield break;
        }

        // 计算每个字符的显示间隔时间
        float totalDuration = 10f; // 总显示时间10秒
        float charInterval = totalDuration / totalChars;

        // 遍历每个句子
        for (int i = 0; i < fullTexts.Count; i++)
        {
            string fullText = fullTexts[i];
            noteText.text = ""; // 清空当前文本

            // 防止句子为空，跳过
            if (string.IsNullOrWhiteSpace(fullText))
            {
                Debug.LogWarning($"fullTexts中的第{i}句为空！");
                continue;
            }

            // 获取文本框的宽度
            float textBoxWidth = textBox.rect.width;

            // 将句子分割成单词列表
            List<string> words = SplitIntoWords(fullText);
            string currentLine = ""; // 当前行的文本

            foreach (string word in words)
            {
                // 测试将当前单词添加到当前行后，是否会超出宽度
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                Vector2 preferredSize = noteText.GetPreferredValues(testLine);

                // 调试日志，帮助确认测量是否正确
                // Debug.Log($"测试行: '{testLine}', 预期宽度: {preferredSize.x}, 文本框宽度: {textBoxWidth}");

                if (preferredSize.x > textBoxWidth)
                {
                    // 当前行已满，将其添加到显示文本并换行
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        // 逐字显示当前行
                        yield return StartCoroutine(DisplayLine(noteText, currentLine, charInterval));
                        noteText.text += "\n"; // 添加换行符
                        currentLine = word; // 将当前单词移到新的一行
                    }
                }
                else
                {
                    // 当前行未满，继续添加单词
                    currentLine = testLine;
                }
            }

            // 显示最后一行
            if (!string.IsNullOrEmpty(currentLine))
            {
                yield return StartCoroutine(DisplayLine(noteText, currentLine, charInterval));
            }

            // 判断是否为最后一句
            bool isLastSentence = (i == fullTexts.Count - 1);

            if (!isLastSentence)
            {
                // 如果不是最后一句，等待玩家点击屏幕以显示下一句
                yield return StartCoroutine(WaitForClick());
                noteText.text = ""; // 清空文本以显示下一句
            }
            else
            {
                // 如果是最后一句，等待玩家点击屏幕后执行隐藏和其他操作
                yield return StartCoroutine(WaitForClick());
                // 隐藏提示对象
                noteObject.SetActive(false);
                FirstNote_FBool = true;

                //// 如果Panel_F是激活状态，则隐藏它
                //if (Panel_F != null && Panel_F.activeSelf)
                //{
                //    Panel_F.SetActive(false);
                //}

                // 如果当前提示对象的名称与TwoNote_F相同，执行特定逻辑
                if (TwoNote_F != null && noteObject.name == TwoNote_F.name)
                {
                    TwoNote_FBool = false;
                    // 如果Panel_F是激活状态，则隐藏它
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
                        Debug.LogWarning("未找到PlayerController组件！");
                    }
                }

                // 标记文字已完全显示
                isTextFullyDisplayed = true;
            }
        }

        // 协程结束
        yield break;
    }

    /// <summary>
    /// 等待玩家点击鼠标左键
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForClick()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                yield break; // 玩家点击后退出等待
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

    //恢复玩家移动
    public IEnumerator ReSetMovePlayer()
    {
        yield return new WaitForSecondsRealtime(1f);
        PreController.Instance.PlayisMove = true;
       
    }
    #endregion[新手提示]
    //修改成用手控制
    //private void HandleNewbieGuide()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0); // 获取第一个触摸点
    //        Vector3 touchPos = touch.position;
    //        Vector2 localPoint;

    //        // 将屏幕点转换为Canvas的本地点
    //        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, touchPos, null, out localPoint);

    //        switch (touch.phase)
    //        {
    //            case TouchPhase.Began:
    //            case TouchPhase.Moved:
    //            case TouchPhase.Stationary:
    //                if (isInside)
    //                {
    //                    // 限制GuidCircle在箭头之间移动
    //                    Vector3 leftLimit = GuidArrowL.rectTransform.anchoredPosition;
    //                    Vector3 rightLimit = GuidArrowR.rectTransform.anchoredPosition;

    //                    float clampedX = Mathf.Clamp(localPoint.x, leftLimit.x, rightLimit.x);
    //                    float clampedY = Mathf.Clamp(localPoint.y, leftLimit.y, rightLimit.y);

    //                    // 设置GuidCircle的位置，保持Y轴不变
    //                    GuidCircle.rectTransform.anchoredPosition = new Vector2(clampedX, GuidCircle.rectTransform.anchoredPosition.y);
    //                    Debug.Log($"GuidCircle 位置: {GuidCircle.rectTransform.anchoredPosition}");

    //                    // 移动玩家
    //                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, Camera.main.nearClipPlane));
    //                    worldPos.z = 0;
    //                    worldPos.y = player.transform.position.y;
    //                    worldPos.x = Mathf.Clamp(worldPos.x, -1.5f, 1.5f);
    //                    player.transform.position = worldPos;
    //                    Debug.Log($"Player 位置: {worldPos}");
    //                }
    //                break;

    //            case TouchPhase.Ended:
    //            case TouchPhase.Canceled:
    //                // 松开手指时，隐藏GuidCircle
    //                GuidCircle.transform.parent.gameObject.SetActive(false);
    //                // 如果需要隐藏Guidfinger，可以取消注释以下代码
    //                // Guidfinger.transform.parent.gameObject.SetActive(false);

    //                AccountManager.Instance.isGuid = true;
    //                GameManage.Instance.SwitchState(GameState.Running);
    //                break;
    //        }
    //    }
    //}
    #region[爆炸和冰冻代码]
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

    // 切换暂停和继续游戏的状态
    public void OnpauseClicked()
    {
        Time.timeScale = 0f; // 暂停游戏
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
        //执行全屏爆炸功能
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
        //执行全屏冰冻功能
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
        GameObject plane = Instantiate(Resources.Load<GameObject>("Prefabs/explode_bomber"), new Vector3(0, -7f, 0), Quaternion.identity);  // 生成飞机在屏幕底部
        Debug.Log("Plane spawned!");
    }
    #endregion
    #region [全屏冰冻逻辑]

    // 冰冻炸弹效果
    private async UniTask FrozenBombEffect(Vector3 ForzenPoint)
    {
        // 播放冰冻效果动画
        Forzen_F.SetActive(true);
        UnityArmatureComponent forzenArmature = Forzen_F.transform.GetChild(0).GetComponentInChildren<UnityArmatureComponent>();
        if (forzenArmature != null)
        {
            forzenArmature.animation.Play("start", 1); // 播放冰冻动画
            await UniTask.Delay(TimeSpan.FromSeconds(forzenArmature.animation.GetState("start")._duration));
        }
        forzenArmature.animation.Play("stay", -1); // 播放冰冻动画
        // 暂停所有敌人和宝箱
        FreezeAllEnemiesAndChests(ForzenPoint, ConfigManager.Instance.Tables.TableTransmitConfig.Get(1).DamageScope);
        // 等待 5 秒
        await UniTask.Delay(5000);
        // 解除冰冻效果
        forzenArmature.animation.Play("end", 1); // 播放冰冻动画
        await UniTask.Delay(TimeSpan.FromSeconds(forzenArmature.animation.GetState("end")._duration));
        UnfreezeAllEnemiesAndChests(ForzenPoint, ConfigManager.Instance.Tables.TableTransmitConfig.Get(1).DamageScope);
        forzenArmature.animation.Play("<None>", -1); // 播放冰冻动画
    }


    private void FreezeAllEnemiesAndChests(Vector3 FreezePos, float width)
    {
        GameManage.Instance.isFrozen = true;
        PreController.Instance.isFrozen = true;
        // 根据冰冻点定义矩形范围
        Vector3 topLeft = new Vector3(FreezePos.x - width / 2, FreezePos.y + width / 2, FreezePos.z);
        Vector3 bottomRight = new Vector3(FreezePos.x + width / 2, FreezePos.y - width / 2, FreezePos.z);
        // 找到并冻结在矩形范围内的敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;

            if (IsWithinRectangle(enemyPos, topLeft, bottomRight))
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead)
                {
                    enemyController.isFrozen = true; // 冻结敌人
                }
            }
        }

        // 找到并冻结在矩形范围内的宝箱
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject chest in chests)
        {
            Vector3 chestPos = chest.transform.position;

            if (IsWithinRectangle(chestPos, topLeft, bottomRight))
            {
                ChestController chestController = chest.GetComponent<ChestController>();
                if (chestController != null && chestController.isVise)
                {
                    chestController.isFrozen = true; // 冻结宝箱
                }
            }
        }
    }

    // 辅助函数，用来检查位置是否在矩形范围内
    private bool IsWithinRectangle(Vector3 position, Vector3 topLeft, Vector3 bottomRight)
    {
        return position.x >= topLeft.x && position.x <= bottomRight.x &&
               position.y <= topLeft.y && position.y >= bottomRight.y;
    }

    // 解除冻结效果
    private void UnfreezeAllEnemiesAndChests(Vector3 FreezePos, float width)
    {
        GameManage.Instance.SetFrozenState(false);
        PreController.Instance.isFrozen = false;
        Debug.Log("解除冰冻=========================！!！");
        // 根据冰冻点定义矩形范围
        Vector3 topLeft = new Vector3(FreezePos.x - width / 2, FreezePos.y + width / 2, FreezePos.z);
        Vector3 bottomRight = new Vector3(FreezePos.x + width / 2, FreezePos.y - width / 2, FreezePos.z);
        // 找到并冻结在矩形范围内的敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;

            if (IsWithinRectangle(enemyPos, topLeft, bottomRight))
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null && !enemyController.isDead)
                {
                    enemyController.isFrozen = false; // 冻结敌人
                }
            }
        }

        // 找到并冻结在矩形范围内的宝箱
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        foreach (GameObject chest in chests)
        {
            Vector3 chestPos = chest.transform.position;

            if (IsWithinRectangle(chestPos, topLeft, bottomRight))
            {
                ChestController chestController = chest.GetComponent<ChestController>();
                if (chestController != null && chestController.isVise)
                {
                    chestController.isFrozen = false; // 冻结宝箱
                }
            }
        }
    }
    #endregion
    #region //原始切枪逻辑

    #region//宝箱技能
    public IEnumerator  ShowSkillGuide(int indexChest)
    {
        // 显示引导并等待引导完成
        yield return StartCoroutine(ShowSkillGuideCoroutine(3));  // 显示狂暴技能引导
    }
    private Vector2 InitialPos = new Vector2(0, 0);
    public Vector2[] SkillGuidPos = new Vector2[]{ new Vector2(42f, -118f), new Vector2(42f, -241), new Vector2(12f, -265) };
    private IEnumerator ShowSkillGuideCoroutine(int indexChest)
    {
        Debug.Log("ShowSkillGuide() 方法被调用");

        if (SkillNote_F == null || SkillFinger_F == null || buffBlastBtn == null)
        {
            Debug.LogError("SkillNote_F、SkillFinger_F 或 buffBlastBtn 未正确赋值。");
            yield break;
        }

        // 暂停游戏
        Time.timeScale = 0f;
        Debug.Log("Time.timeScale 设置为: " + Time.timeScale);

        // 激活 SkillNote_F 和 SkillFinger_F
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

        // 计算目标位置（buffBlastBtn 的位置）
        Debug.Log($"目标位置: {targetPos}");

        // 动画持续时间
        float moveDuration = 0.5f; // 移动持续时间
        float clickDuration = 0.25f; // 点击动画持续时间

        // 动画移动到目标位置
        Debug.Log("移动动画开始");
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(InitialPos, targetPos, elapsed / moveDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = targetPos;
        Debug.Log("移动动画完成");

        // 开始重复点击动画
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(SkillFinger_F.transform.DOScale(SkillFinger_F.transform.localScale * 1.2f, 0.2f).SetEase(Ease.InOutSine))
                     .Append(SkillFinger_F.transform.DOScale(SkillFinger_F.transform.localScale, 0.2f).SetEase(Ease.InOutSine))
                     .SetLoops(-1) // 无限循环
                     .SetUpdate(true); // 使用不缩放时间
        clickSequence.Play();
        Debug.Log("开始重复点击动画");

        // 等待 buffBlastBtn 被点击
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

        // 等待按钮点击
        Debug.Log("等待 buffBlastBtn 被点击");
        while (!buttonClicked)
        {
            yield return null;
        }

        // 停止重复点击动画
        clickSequence.Kill();
        Debug.Log("停止重复点击动画");

        // 隐藏引导
        SkillFinger_F.GetComponent<RectTransform>().anchoredPosition = InitialPos;
        SkillNote_F.SetActive(false);
        SkillFinger_F.gameObject.SetActive(false);
        // 恢复游戏
        Time.timeScale = 1f;
        Debug.Log("Time.timeScale 恢复为: " + Time.timeScale);
    }


    /// <summary>
    /// 隐藏技能提示
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

        // 恢复游戏
        Time.timeScale = 1f;
    }
    #endregion
    #endregion
  

    #region[狂暴技能代码]
    // 狂暴技能按钮点击事件
    private async void OnSpecialButtonClicked()
    {
        if (isButtonActive)
        {
            isButtonActive = false;          // 重置按钮状态
            isSkillActive = true;            // 激活技能
            specialButtonImage.sprite = Rampages[0];
            specialButton.interactable = false; // 按钮不可点击

            // 暂停技能计时
            elapsedTimeSkill = 0f;  // 重置技能计时
            elapsedTimeBtn = 0f;    // 重置冷却计时
            // 引导完成后，继续技能冷却计时
            StartCoroutine(SkillActiveCoroutine());
        }
    }

    public float elapsedTimeBtn;         // 冷却计时
    public float elapsedTimeSkill;       // 技能持续计时
    private IEnumerator SkillActiveCoroutine()
    {
        ragepBJ = GameObject.Find("Player/rage");
        elapsedTimeSkill = 0f;
        isStayAnimationPlayed = false;
        isEndAnimationPlayed = false;
        // 播放 start 动画
        PlayDragonBonesAnimation("start", elapsedTimeSkill);
        // 等待直到 start 动画结束
        while (elapsedTimeSkill < activeTime)
        {
            elapsedTimeSkill += Time.unscaledDeltaTime; // 使用不受 Time.timeScale 影响的时间
                                                        // 在 start 动画播放完成后切换到 stay 动画
            if (!isStayAnimationPlayed)
            {
                PlayDragonBonesAnimation("stay", elapsedTimeSkill);
            }
            if (!isEndAnimationPlayed)
            {
                // 在技能持续时间的最后 1 秒切换到 end 动画
                PlayDragonBonesAnimation("end", elapsedTimeSkill);

            }
            yield return null;
        }
        isSkillActive = false;
    }

    private bool isStayAnimationPlayed = false; // 标志 stay 动画是否已经播放
    private bool isEndAnimationPlayed = false;  // 标志 end 动画是否已经播放
    private UnityArmatureComponent armatureComponent;
    private GameObject ragepBJ ;
    private void PlayDragonBonesAnimation(string animationName, float elapsedTimeSkill)
    {
        // 确保玩家对象存在，并且有 DragonBones 组件
        if (ragepBJ == null)
        {
            Debug.LogError("找不到玩家的 rage 对象！");
            return;
        }
        // 查找玩家对象下的 DragonBones 动画组件
        armatureComponent = ragepBJ.GetComponent<UnityArmatureComponent>();

        if (armatureComponent != null)
        {
            // 播放指定的动画
            if (animationName == "stay")
            {
                if (elapsedTimeSkill > armatureComponent.animation.GetState("start")._duration)
                {
                    armatureComponent.animation.Play("stay", -1);
                    isStayAnimationPlayed = true;
                }
            }
            else if (animationName == "end")
            { // 检查 _duration 是否有效
                if (elapsedTimeSkill > activeTime - 1)
                {
                    armatureComponent.animation.Play("end", 1);
                    Debug.Log($"End Animation Duration: {armatureComponent.animation.GetState("end")._duration}");
                    isEndAnimationPlayed = true; // 确保 end 动画只播放一次
                }
            }
            else
            {
                armatureComponent.animation.Play(animationName, 1);
            }
        }
        else
        {
            Debug.LogError("找不到 DragonBones 动画组件！");
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
            // 显示引导并等待引导完成
            // 显示引导并等待引导完成
            if (PlayInforManager.Instance.playInfor.isFirstSpecial && GameFlowManager.Instance.currentLevelIndex == 1)
            {
                PreController.Instance.isRageSkill = true;
                PlayInforManager.Instance.playInfor.isFirstSpecial = false;
                AccountManager.Instance.SaveAccountData();
                yield return StartCoroutine(ShowSkillGuideCoroutine(3));  // 显示狂暴技能引导
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
        // 获取玩家位置
        if (player == null)
            player = GameObject.Find("Player");
        Vector3 playerPosition = player.transform.position;

        // 获取所有敌人和宝箱
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

        // 按距离从近到远排序
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
                    continue; // 跳过已死亡的敌人
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
                    continue; // 跳过无效的宝箱
                }
            }

            // 计算飞向该目标的子弹的总伤害
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
                // 目标需要更多伤害，发射子弹
                PreController.Instance.SpawnHomingBullet(playerPosition, target.transform);
                return; // 只发射一颗子弹
            }
            else
            {
                // 该目标的飞行子弹已经足够，尝试下一个目标
                continue;
            }
        }
    }
    #endregion[狂暴技能代码]
}
