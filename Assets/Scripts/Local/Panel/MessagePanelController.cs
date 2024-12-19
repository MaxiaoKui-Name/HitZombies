using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class MessagePanelController : UIBase
{
    public Button returnBtn;
    public Transform messagesContent; // ScrollView的Content
    public GameObject messagePrefab;


    void Start()
    {
        GetAllChild(transform);
        returnBtn = childDic["RerturnBtn_F"].GetComponent<Button>();
        messagesContent = childDic["Content_F"].GetComponent<Transform>(); 
        messagePrefab = Resources.Load<GameObject>("Prefabs/UIPannel/Messageltem");
        returnBtn.onClick.AddListener(Hide);
        PopulateMessages();
        // 订阅消息更新事件，以便实时更新
        MessageManager.Instance.OnMessagesUpdated += PopulateMessages;
    }
    public void Hide()
    {
        AudioManage.Instance.PlaySFX("button", null);
        MessageManager.Instance.OnMessagesUpdated -= PopulateMessages;
        returnBtn.onClick.RemoveListener(Hide);
        Destroy(gameObject);
    }
    void PopulateMessages()
    {
        // 清除现有消息
        foreach (Transform child in messagesContent)
        {
            Destroy(child.gameObject);
        }
        // 获取所有消息并按时间排序（已经在MessageManager中排序）
        List<Message> messages = MessageManager.Instance.GetAllMessages();
        foreach (var msg in messages)
        {
            GameObject msgObj = Instantiate(messagePrefab, messagesContent);
            // 假设MessagePrefab有一个脚本来设置内容
            MessageltemController controller = msgObj.GetComponent<MessageltemController>();
            if (controller != null)
            {
                controller.SetMessage(msg);
            }

            if (!msg.IsRead)
            {
                // 标记为已读
                MessageManager.Instance.MarkMessageAsRead(msg.MessageID);
            }
        }
        // 更新红色指示器
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel != null)
        {
            readyPanel.UpdateRedIndicator();
        }
    }
}
