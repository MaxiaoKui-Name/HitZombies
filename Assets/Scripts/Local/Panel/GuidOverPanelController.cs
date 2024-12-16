using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidOverPanelController : UIBase
{
    public GameObject GuidOverNote_F;
    public RectTransform textBox;    // ȷ���ڱ༭���и�ֵ
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
        // ������ʾ����
        noteObject.SetActive(true);

        // ��ȡ��ʾ�ı����
        TextMeshProUGUI noteText = noteObject.GetComponentInChildren<TextMeshProUGUI>();
        if (textBox == null)
        {
            // �����ı����ǵ������Ӷ���������0��ʼ��
            textBox = noteObject.transform.GetChild(1).GetComponentInChildren<RectTransform>();
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
                Destroy(gameObject);
            }
        }

        // Э�̽���
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
                yield break; // ��ҵ�����˳��ȴ�
            }
            yield return null;
        }
    }
}
