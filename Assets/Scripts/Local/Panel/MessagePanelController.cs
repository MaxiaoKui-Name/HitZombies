using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class MessagePanelController : UIBase
{
    public Button returnBtn;
    public Transform messagesContent; // ScrollView��Content
    public GameObject messagePrefab;


    void Start()
    {
        GetAllChild(transform);
        returnBtn = childDic["RerturnBtn_F"].GetComponent<Button>();
        messagesContent = childDic["Content_F"].GetComponent<Transform>(); 
        messagePrefab = Resources.Load<GameObject>("Prefabs/UIPannel/Messageltem");
        returnBtn.onClick.AddListener(Hide);
        PopulateMessages();
        // ������Ϣ�����¼����Ա�ʵʱ����
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
        // ���������Ϣ
        foreach (Transform child in messagesContent)
        {
            Destroy(child.gameObject);
        }
        // ��ȡ������Ϣ����ʱ�������Ѿ���MessageManager������
        List<Message> messages = MessageManager.Instance.GetAllMessages();
        foreach (var msg in messages)
        {
            GameObject msgObj = Instantiate(messagePrefab, messagesContent);
            // ����MessagePrefab��һ���ű�����������
            MessageltemController controller = msgObj.GetComponent<MessageltemController>();
            if (controller != null)
            {
                controller.SetMessage(msg);
            }

            if (!msg.IsRead)
            {
                // ���Ϊ�Ѷ�
                MessageManager.Instance.MarkMessageAsRead(msg.MessageID);
            }
        }
        // ���º�ɫָʾ��
        ReadyPanelController readyPanel = FindObjectOfType<ReadyPanelController>();
        if (readyPanel != null)
        {
            readyPanel.UpdateRedIndicator();
        }
    }
}
