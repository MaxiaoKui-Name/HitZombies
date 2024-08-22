using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyPanelController : UIBase
{
    public Button StartGameBtn;

    void Start()
    {
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
        StartGameBtn.gameObject.SetActive(false);
        //切场景
        //切换场景
        LevelManager.Instance.LoadLevel("First");
    }
}
