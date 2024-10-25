using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MailPanelController : UIBase
{
    public Button viewMessagesButton;
    public Button sendMessageButton;
    public GameObject MessagePanel;
    public GameObject SendMessagePanel;
    public Button ReturnBtn;
    void Start()
    {
        GetAllChild(transform);
        viewMessagesButton = childDic["MailtemReceiveBtn_F"].GetComponent<Button>();
        sendMessageButton = childDic["MailtemSendBtn_F"].GetComponent<Button>();
        ReturnBtn = childDic["RerturnBtn_F"].GetComponent<Button>();
        ReturnBtn.onClick.AddListener(Hide);
        viewMessagesButton.onClick.AddListener(OpenMessagePanel);
        sendMessageButton.onClick.AddListener(SendMessage);
    }

    public void Hide()
    {
        Destroy(gameObject);
    }

    void OpenMessagePanel()
    {
        if (LevelManager.Instance.levelData != null)
        {
            MessagePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/MessagePanel"));
            MessagePanel.transform.SetParent(transform.parent, false);
            MessagePanel.transform.localPosition = Vector3.zero;
        }
    }
    void SendMessage()
    {
        if (LevelManager.Instance.levelData != null)
        {
            SendMessagePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/SendMessagePanel"));
            SendMessagePanel.transform.SetParent(transform.parent, false);
            SendMessagePanel.transform.localPosition = Vector3.zero;
        }
    }
}
