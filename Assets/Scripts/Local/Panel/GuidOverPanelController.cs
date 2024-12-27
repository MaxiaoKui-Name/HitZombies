using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GuidOverPanelController : UIBase
{
    public GameObject GuidOverNote_F;
    public RectTransform textBox;    // 确保在编辑器中赋值
    private int clickCount = 0;

    void Start()
    {
        GetAllChild(transform);
        GuidOverNote_F = childDic["GuidOverNote_F"].gameObject;
        GuidOverNote_F.SetActive(false);
        GameManage.Instance.clickCount = true;
        StartCoroutine(ShowFirstNoteAfterDelay());
    }
    private void Update()
    {
        if (GameManage.Instance.clickCount)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickCount++;
            }
        }
    }
    private IEnumerator ShowFirstNoteAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        ShowFirstNote();
    }

    public void ShowFirstNote()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner6").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        StartCoroutine(ShowMultipleNotesCoroutine(GuidOverNote_F, guidanceTexts));
    }
    // Update is called once per frame
    public IEnumerator ShowMultipleNotesCoroutine(
       GameObject noteObject,
       List<string> fullTexts
   )
    {
        // 1. 激活面板
        noteObject.SetActive(true);
        // 2. 获取提示文本组件（TextMeshProUGUI）
        TextMeshProUGUI noteText = noteObject.GetComponentInChildren<TextMeshProUGUI>();
        if (noteText == null)
        {
            Debug.LogError("未找到 TextMeshProUGUI 组件！");
            yield break;
        }

        // 3. 若未指定 textBox，则自动获取
        if (textBox == null)
        {
            textBox = noteObject.transform.GetChild(2).GetComponentInChildren<RectTransform>();
        }

        // 4. 每个字符显示的时间间隔
        float charInterval = 0.04f;

        // 5. 遍历所有句子
        for (int i = 0; i < fullTexts.Count; i++)
        {
            string fullText = fullTexts[i];
            if (string.IsNullOrEmpty(fullText)) continue;

            // (A) 在显示新句子前，先清空文本
            noteText.text = "";

            // (B) 逐行逐字显示该句（可点击跳过 => 立刻整句补全）
            yield return StartCoroutine(
                TypeSentenceCoroutine(
                    noteText,
                    fullText,
                    textBox.rect.width,
                    charInterval
                )
            );

            // (C) 等待玩家“再点击一下”后再显示下一句
            //     —— 如果玩家不点击，就停留在当前句
            yield return StartCoroutine(WaitForNextClick());

            // (D) 如果已经是最后一句，执行结束逻辑
            if (i == fullTexts.Count - 1)
            {
                // 补全最后一句并执行结束操作
                noteText.text = fullText;
                //// 等待玩家再次点击，执行后续逻辑
                //yield return StartCoroutine(WaitForNextClick());
                noteObject.SetActive(false);
                GameManage.Instance.clickCount = false;
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 逐行、逐字打出一整句（可自动换行）。点击则“立即补全整句”。
    /// </summary>
    private IEnumerator TypeSentenceCoroutine(
        TextMeshProUGUI noteText,
        string sentence,
        float textBoxWidth,
        float charInterval
    )
    {
        // 清空
        noteText.text = "";

        // 拆分为“单词列表”
        List<string> words = SplitIntoWords(sentence);

        // 用于拼接已显示的行
        string displayedSoFar = "";
        // 当前行内容
        string currentLine = "";

        // 是否要跳过剩余的行（整句）
        bool skipRemaining = false;

        // （1）逐个单词拼接，检测是否需要换行
        for (int w = 0; w < words.Count; w++)
        {
            if (skipRemaining) break;

            // 尝试把下一个单词拼到行里
            string testLine = string.IsNullOrEmpty(currentLine)
                ? words[w]
                : currentLine + " " + words[w];

            // 计算这一行的优先宽度
            Vector2 preferredSize = noteText.GetPreferredValues(displayedSoFar + testLine);

            // 若超出 textBox 宽度 => 先把“上一行”逐字显示
            if (preferredSize.x > textBoxWidth)
            {
                if (!string.IsNullOrEmpty(currentLine))
                {
                    // 逐字显示上一行
                    yield return StartCoroutine(
                        DisplayLine(
                            noteText,
                            displayedSoFar,
                            currentLine,
                            charInterval,
                            onClickSkip: () =>
                            {
                                // 如果玩家点击 => 立刻跳过整句剩余
                                skipRemaining = true;
                            }
                        )
                    );

                    if (skipRemaining) break;

                    // 否则上一行显示完毕 => 换行
                    displayedSoFar += currentLine + "\n";
                }
                // 换到下一行，从当前单词开始
                currentLine = words[w];
            }
            else
            {
                // 没超宽，继续拼到 currentLine
                currentLine = testLine;
            }
        }

        // （2）单词循环结束后，若还有最后一行内容没显示
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
                // 若没有被跳过 => 把这一行累加
                displayedSoFar += currentLine;
            }
        }

        // （3）如果点击过 => 直接整句补全
        if (skipRemaining)
        {
            noteText.text = sentence;
        }
        else
        {
            // 否则就是正常的逐字输出完毕
            noteText.text = displayedSoFar;
        }
    }

    /// <summary>
    /// “行级”协程——逐字显示本行；如果玩家点击 => 立刻补全本行并标记要跳过整句。
    /// </summary>
    private IEnumerator DisplayLine(
       TextMeshProUGUI noteText,
       string displayedSoFar,
       string lineToDisplay,
       float charInterval,
       System.Action onClickSkip
    )
    {
        // 使用 StringBuilder 优化字符串拼接
        StringBuilder lineBuffer = new StringBuilder();

        for (int i = 0; i < lineToDisplay.Length; i++)
        {
            // 检测是否有跳过输入
            if (clickCount > 0)
            {
                clickCount--; // 消耗一次点击
                onClickSkip?.Invoke();
                // 直接把本行剩余全部显示，并处理数字放大
                string remaining = lineToDisplay.Substring(i);
                string processedRemaining = WrapDigitsWithSize(remaining);
                noteText.text = displayedSoFar + lineBuffer.ToString() + processedRemaining;
                yield break;
            }
            char currentChar = lineToDisplay[i];
            if (currentChar == '<')
            {
                // 处理富文本标签
                int tagEndIndex = lineToDisplay.IndexOf('>', i);
                if (tagEndIndex == -1)
                {
                    // 没找到 '>'，就当普通字符
                    lineBuffer.Append(currentChar);
                }
                else
                {
                    // 一次性把整个 <...> 标签加入
                    string fullTag = lineToDisplay.Substring(i, tagEndIndex - i + 1);
                    lineBuffer.Append(fullTag);
                    i = tagEndIndex; // 跳到标签结尾
                }
            }
            else
            {
                if (char.IsDigit(currentChar))
                {
                    // 数字字符，检测是否是连续数字
                    int start = i;
                    while (i < lineToDisplay.Length && char.IsDigit(lineToDisplay[i]))
                    {
                        i++;
                    }
                    int length = i - start;
                    string number = lineToDisplay.Substring(start, length);
                    // 添加放大标签
                    lineBuffer.Append($"<size=38>{number}</size>");
                    i--; // 调整索引，因为for循环会自动增加
                }
                else
                {
                    // 普通字符
                    lineBuffer.Append(currentChar);
                }

                // 等待一小段时间
                yield return new WaitForSecondsRealtime(charInterval);
            }

            // 每新增一个字符/标签，就更新 Text
            noteText.text = displayedSoFar + lineBuffer.ToString();
        }
    }

    /// <summary>
    /// 将字符串中的数字字符用<size=38>标签包裹起来，并确保不影响其他富文本标签
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>处理后的字符串</returns>
    private string WrapDigitsWithSize(string input)
    {
        StringBuilder result = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            if (input[i] == '<')
            {
                // 处理富文本标签
                int tagEndIndex = input.IndexOf('>', i);
                if (tagEndIndex == -1)
                {
                    // 没找到 '>'，就当普通字符
                    result.Append(input[i]);
                    i++;
                }
                else
                {
                    // 一次性把整个 <...> 标签加入
                    string fullTag = input.Substring(i, tagEndIndex - i + 1);
                    result.Append(fullTag);
                    i = tagEndIndex + 1;
                }
            }
            else if (char.IsDigit(input[i]))
            {
                // 数字字符，检测是否是连续数字
                int start = i;
                while (i < input.Length && char.IsDigit(input[i]))
                {
                    i++;
                }
                int length = i - start;
                string number = input.Substring(start, length);
                // 添加放大标签
                result.Append($"<size=38>{number}</size>");
            }
            else
            {
                // 普通字符
                result.Append(input[i]);
                i++;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 等待玩家“点击一下”后再继续。（强调：这一次点击是“新的点击”）
    /// </summary>
    private IEnumerator WaitForNextClick()
    {
        // 检查是否已经有未处理的点击
        if (clickCount > 0)
        {
            clickCount--; // 消耗一次点击
            yield break;
        }

        // 1) 如果此时玩家还在按着鼠标（比如上一次点了还没松开）
        //    那么先等他松手，避免一次长按被判定为两次点击
        while (Input.GetMouseButton(0))
        {
            yield return null;
        }

        // 2) 等待下一次真正的按下
        while (clickCount == 0)
        {
            yield return null;
        }

        // 3) 消耗一次点击
        clickCount--;
    }

}
