using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidOverPanelController : UIBase
{
    public GameObject GuidOverNote_F;
    public RectTransform textBox;    // 确保在编辑器中赋值
    void Start()
    {
        GetAllChild(transform);
        GuidOverNote_F = childDic["GuidOverNote_F"].gameObject;
        GuidOverNote_F.SetActive(true);

    }
    private IEnumerator ShowFirstNoteAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        ShowFirstNote();
    }

    public void ShowFirstNote()
    {
        string guidanceText = ConfigManager.Instance.Tables.TableLanguageConfig.Get("Beginner6").Yingwen;
        List<string> guidanceTexts = SplitIntoSentences(guidanceText);
        StartCoroutine(ShowMultipleNotesCoroutine(GuidOverNote_F, guidanceTexts));
    }
    // Update is called once per frame
    private IEnumerator ShowMultipleNotesCoroutine(GameObject noteObject, List<string> fullTexts)
    {
        // 激活提示对象
        noteObject.SetActive(true);

        // 获取提示文本组件
        TextMeshProUGUI noteText = noteObject.GetComponentInChildren<TextMeshProUGUI>();
        if (textBox == null)
        {
            // 假设文本框是第三个子对象（索引从0开始）
            textBox = noteObject.transform.GetChild(1).GetComponentInChildren<RectTransform>();
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
                Destroy(gameObject);
            }
        }

        // 协程结束
        yield break;
    }
    private IEnumerator DisplayLine(TextMeshProUGUI noteText, string line, float charInterval)
    {
        foreach (char c in line)
        {
            noteText.text += c;
            yield return new WaitForSecondsRealtime(charInterval);
        }
    }
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
}
