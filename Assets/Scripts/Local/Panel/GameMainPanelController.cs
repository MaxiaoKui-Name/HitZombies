using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMainPanelController : UIBase
{
    public Button pauseButton;   // 引用暂停按钮
    public TextMeshProUGUI coinText;        // 引用显示金币的文本框
    private bool isPaused = false;

    void Start()
    {
        GetAllChild(transform);

        // 找到子对象中的按钮和文本框
        pauseButton = transform.Find("pause_F/pause_Btn").GetComponent<Button>();
        coinText = transform.Find("coins_F/valueText").GetComponent<TextMeshProUGUI>();


        // 添加暂停按钮的点击事件监听器
        pauseButton.onClick.AddListener(TogglePause);
    }

    void Update()
    {
        // 实时更新显示的金币数量
        coinText.text = $"{PlayInforManager.Instance.playInfor.coinNum}";
    }

    // 切换暂停和继续游戏的状态
    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // 暂停游戏
        }
        else
        {
            Time.timeScale = 1f; // 继续游戏
        }
    }
}
