using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[System.Serializable]
public class Message
{
    public string MessageID;
    public string Content;
    public string SendTime;
    public bool IsRead;
    // 构造函数
    public Message(string messageID, string content, string sendTime, bool isRead)
    {
        MessageID = messageID;
        Content = content;
        SendTime = sendTime;
        IsRead = isRead;
    }
}

[System.Serializable]
public class MessageList
{
    public List<Message> messages;
}

public class MessageManager : Singleton<MessageManager>
{
    public string serverURL = "https://yourserver.com/api/messages";
    private string authToken;

    public List<Message> messages = new List<Message>();

    public event Action OnMessagesUpdated;

    void Start()
    {
        //authToken = PlayerPrefs.GetString("AuthToken");
        //if (!string.IsNullOrEmpty(authToken))
        //{
        //    FetchMessages();
        //    // 设置定时轮询
        //    InvokeRepeating("FetchMessages", 60f, 60f); // 每60秒获取一次
        //}
        //else
        //{
        //    Debug.LogError("Auth token is missing");
        //}
        //TTOD1手动创建新的消息 需要更改服务器
        if(PlayerPrefs.HasKey("PlayerAccountID"))
        {
            Message newMessage = new Message(
          messageID: PlayerPrefs.GetString("PlayerAccountID"), // 生成唯一的MessageID
          content: "今日版本上新",
          sendTime: DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), // 当前时间
          isRead: false
           );
            messages.Add(newMessage);
            // 触发更新事件
            OnMessagesUpdated?.Invoke();
        }
    }

    public void FetchMessages()
    {
        StartCoroutine(GetMessagesCoroutine());
    }

    // 获取消息
    IEnumerator GetMessagesCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get(serverURL);
        www.SetRequestHeader("Authorization", "Bearer " + authToken);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching messages: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            MessageList messageList = JsonUtility.FromJson<MessageList>(json);
            messages = messageList.messages;
            // 排序（如果服务器未排序）
            messages.Sort((x, y) => DateTime.Parse(y.SendTime).CompareTo(DateTime.Parse(x.SendTime)));

            // 触发更新事件
            OnMessagesUpdated?.Invoke();
        }
    }

    public void MarkMessageAsRead(string messageID)
    {
        StartCoroutine(MarkAsReadCoroutine(messageID));
    }

    IEnumerator MarkAsReadCoroutine(string messageID)
    {
        string url = serverURL + "/markAsRead";
        WWWForm form = new WWWForm();
        form.AddField("messageID", messageID);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("Authorization", "Bearer " + authToken);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error marking message as read: " + www.error);
        }
        else
        {
            Debug.Log("Message marked as read");
            // 更新本地数据
            Message msg = messages.Find(m => m.MessageID == messageID);
            if (msg != null)
            {
                msg.IsRead = true;
                // 触发更新事件
                OnMessagesUpdated?.Invoke();
            }
        }
    }

    public int GetUnreadCount()
    {
        int count = 0;
        foreach (var msg in messages)
        {
            if (!msg.IsRead)
                count++;
        }
        return count;
    }

    public List<Message> GetAllMessages()
    {
        return new List<Message>(messages);
    }

    public List<Message> GetUnreadMessages()
    {
        return messages.FindAll(m => !m.IsRead);
    }
}


