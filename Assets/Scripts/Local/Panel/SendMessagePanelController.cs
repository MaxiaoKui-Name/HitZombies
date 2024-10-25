using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
public class SendMessagePanelController : UIBase
{
    [Header("UI Elements")]
    public Button ReturnButton;    // 反馈按钮
    public TMP_InputField feedbackInputField; // 输入框
    public Button sendButton;        // 发送按钮

    [Header("Backend Settings")]
    [Tooltip("后台接收反馈的URL")]
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

    // 发送反馈
    void SendFeedback()
    {
        string feedbackText = feedbackInputField.text.Trim();
        if (string.IsNullOrEmpty(feedbackText))
        {
            // 可以在此显示提示用户输入内容
            Debug.Log("反馈内容不能为空！");
            return;
        }

        StartCoroutine(PostFeedback(feedbackText));
    }

    // TTOD1发送待更改协程发送POST请求到后台
    IEnumerator PostFeedback(string feedback)
    {
        // 创建表单数据
        WWWForm form = new WWWForm();
        form.AddField("feedback", feedback);
        form.AddField("device", SystemInfo.deviceModel);
        form.AddField("platform", Application.platform.ToString());
        form.AddField("gameVersion", Application.version);
        form.AddField("timestamp", System.DateTime.UtcNow.ToString("o"));

        using (UnityWebRequest www = UnityWebRequest.Post(feedbackURL, form))
        {
            // 发送请求并等待响应
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("发送反馈失败: " + www.error);
                // 可以在此显示发送失败的提示给用户
            }
            else
            {
                Debug.Log("反馈发送成功！");
                // 清空输入框
                feedbackInputField.text = "";
            }
        }
    }
}
