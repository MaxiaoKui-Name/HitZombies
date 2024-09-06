using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyPanelController : UIBase
{
    public Button StartGameBtn;
    public UIManager uIManager;
    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        GetAllChild(transform);
        StartGameBtn = childDic["ReadystartGame_F"].GetComponent<Button>();
        // 为按钮添加监听
        StartGameBtn.onClick.AddListener(OnStartGameButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 按钮点击时调用的方法
    void OnStartGameButtonClicked()
    {
        if(LevelManager.Instance.levelData != null)
        {
            StartGameBtn.gameObject.SetActive(false);
            uIManager.ChangeState(GameState.Running);
            LevelManager.Instance.LoadScene("First", 0);
        }
     
    }
}
