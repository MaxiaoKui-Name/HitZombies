using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageltemController : UIBase
{
    public TextMeshProUGUI messageContentText; // 显示消息内容的Text
    // public TextMeshProUGUI timestampText; // 显示发送时间的Text
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
