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
        pauseButton = transform.Find("pause_F/pause_Btn").GetComponent<Button>();
        coinText = transform.Find("coins_F/valueText").GetComponent<TextMeshProUGUI>();


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
