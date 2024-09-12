using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMainPanelController : UIBase
{
    public Button pauseButton;   // ������ͣ��ť
    public TextMeshProUGUI coinText;        // ������ʾ��ҵ��ı���
    private bool isPaused = false;

    void Start()
    {
        GetAllChild(transform);

        // �ҵ��Ӷ����еİ�ť���ı���
        pauseButton = childDic["pause_Btn_F"].GetComponent<Button>();
        coinText = childDic["valueText_F"].GetComponent<TextMeshProUGUI>();


        // �����ͣ��ť�ĵ���¼�������
        pauseButton.onClick.AddListener(TogglePause);
    }

    void Update()
    {
        // ʵʱ������ʾ�Ľ������
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum}";
    }

    // �л���ͣ�ͼ�����Ϸ��״̬
    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // ��ͣ��Ϸ
        }
        else
        {
            Time.timeScale = 1f; // ������Ϸ
        }
    }
}
