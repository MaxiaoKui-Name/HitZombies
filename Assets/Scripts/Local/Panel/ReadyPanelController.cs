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
        StartGameBtn.gameObject.SetActive(false);
        //�г���
        //�л�����
        LevelManager.Instance.LoadLevel("First");
    }
}
