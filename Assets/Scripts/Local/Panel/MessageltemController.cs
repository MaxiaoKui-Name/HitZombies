using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageltemController : UIBase
{
    public TextMeshProUGUI messageContentText; // ��ʾ��Ϣ���ݵ�Text
    // public TextMeshProUGUI timestampText; // ��ʾ����ʱ���Text
    void Start()
    {
        GetAllChild(transform);
       // messageContentText = childDic["MessageText_F"].GetComponent<TextMeshProUGUI>();
    }
    public void SetMessage(Message msg)
    {
       // messageContentText = childDic["MessageText_F"].GetComponent<TextMeshProUGUI>();
        messageContentText.text = msg.Content;
        //timestampText.text = msg.SendTime;
    }
}
