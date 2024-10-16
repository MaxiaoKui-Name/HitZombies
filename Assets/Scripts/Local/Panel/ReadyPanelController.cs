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
        // Ϊ��ť��Ӽ���
        StartGameBtn.onClick.AddListener(OnStartGameButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ��ť���ʱ���õķ���
    void OnStartGameButtonClicked()
    {
        if(LevelManager.Instance.levelData != null)
        {
            StartGameBtn.gameObject.SetActive(false);
            uIManager.ChangeState(GameState.Running);
            InfiniteScroll.Instance.baseScrollSpeed =  ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
            InfiniteScroll.Instance.baseGrowthRate = InfiniteScroll.Instance.baseScrollSpeed / 40;
            LevelManager.Instance.LoadScene("First", 0);
        }
     
    }
}
