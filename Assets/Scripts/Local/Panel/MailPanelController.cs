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
        //��������
        // ��ʼ���������Ϊ0
        RectTransform panelRect = GetComponent<RectTransform>();
        StartCoroutine(PopUpAnimation(panelRect));
        // ��ȡ ClickAMature �����䶯�����
        GetClickAnim(transform);

        //ReturnBtn.onClick.AddListener(Hide);
        //viewMessagesButton.onClick.AddListener(OpenMessagePanel);
        //sendMessageButton.onClick.AddListener(SendMessage);
        ReturnBtn.onClick.AddListener(() => StartCoroutine(OnReturnBtnClicked()));
        viewMessagesButton.onClick.AddListener(() => StartCoroutine(OnviewMessagesButtonClicked()));
        sendMessageButton.onClick.AddListener(() => StartCoroutine(OnsendMessageButtonClicked()));
    }

    /// <summary>
    /// ����ǩ����ť����¼���Э��
    /// </summary>
    private IEnumerator OnReturnBtnClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));
        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(ReturnBtn.GetComponent<RectTransform>(), Hide));
    }
    private IEnumerator OnviewMessagesButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));
        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(viewMessagesButton.GetComponent<RectTransform>(), OpenMessagePanel));
    }
    private IEnumerator OnsendMessageButtonClicked()
    {
        AudioManage.Instance.PlaySFX("button", null);
        // ���ŵ������
        yield return StartCoroutine(HandleButtonClickAnimation(transform));
        // ִ�а�ť�������������ú����߼�
        yield return StartCoroutine(ButtonBounceAnimation(sendMessageButton.GetComponent<RectTransform>(), SendMessage));
    }

    public void Hide()
    {
        ReturnBtn.onClick.RemoveListener(Hide);
        viewMessagesButton.onClick.RemoveListener(OpenMessagePanel);
        sendMessageButton.onClick.RemoveListener(SendMessage);
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
