using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
public class SendMessagePanelController : UIBase
{
    [Header("UI Elements")]
    public Button ReturnButton;    // ������ť
    public TMP_InputField feedbackInputField; // �����
    public Button sendButton;        // ���Ͱ�ť

    [Header("Backend Settings")]
    [Tooltip("��̨���շ�����URL")]
    public string feedbackURL = "https://your-backend-server.com/api/feedback";

    void Start()
    {
        GetAllChild(transform);
        ReturnButton = childDic["RerturnBtn_F"].GetComponent<Button>();
        feedbackInputField = childDic["FeedbackInputField_F"].GetComponent<TMP_InputField>();
        sendButton = childDic["SendBtn_F"].GetComponent<Button>();
        if (sendButton != null)
            sendButton.onClick.AddListener(SendFeedback);
        if (ReturnButton != null)
            ReturnButton.onClick.AddListener(ClosePanel);
    }
    void ClosePanel()
    {
        ReturnButton.onClick.RemoveListener(ClosePanel);
        sendButton.onClick.RemoveListener(SendFeedback);
        Destroy(gameObject);
    }

    // ���ͷ���
    void SendFeedback()
    {
        string feedbackText = feedbackInputField.text.Trim();
        if (string.IsNullOrEmpty(feedbackText))
        {
            // �����ڴ���ʾ��ʾ�û���������
            Debug.Log("�������ݲ���Ϊ�գ�");
            return;
        }

        StartCoroutine(PostFeedback(feedbackText));
    }

    // TTOD1���ʹ�����Э�̷���POST���󵽺�̨
    IEnumerator PostFeedback(string feedback)
    {
        // ����������
        WWWForm form = new WWWForm();
        form.AddField("feedback", feedback);
        form.AddField("device", SystemInfo.deviceModel);
        form.AddField("platform", Application.platform.ToString());
        form.AddField("gameVersion", Application.version);
        form.AddField("timestamp", System.DateTime.UtcNow.ToString("o"));

        using (UnityWebRequest www = UnityWebRequest.Post(feedbackURL, form))
        {
            // �������󲢵ȴ���Ӧ
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("���ͷ���ʧ��: " + www.error);
                // �����ڴ���ʾ����ʧ�ܵ���ʾ���û�
            }
            else
            {
                Debug.Log("�������ͳɹ���");
                // ��������
                feedbackInputField.text = "";
            }
        }
    }
}
