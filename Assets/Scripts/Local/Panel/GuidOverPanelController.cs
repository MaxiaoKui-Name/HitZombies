using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GuidOverPanelController : UIBase
{
    public GameObject GuidOverNote_F;
    public RectTransform textBox;    // ȷ���ڱ༭���и�ֵ
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
        // 1. �������
        noteObject.SetActive(true);
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
            textBox = noteObject.transform.GetChild(2).GetComponentInChildren<RectTransform>();
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
                //// �ȴ�����ٴε����ִ�к����߼�
                //yield return StartCoroutine(WaitForNextClick());
                noteObject.SetActive(false);
                GameManage.Instance.clickCount = false;
                Destroy(gameObject);
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

}
