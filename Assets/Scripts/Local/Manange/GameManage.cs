using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameState
{
    Loading,
    Ready,
    Running,
    NextLevel,
    GameOver
}

public class GameManage : Singleton<GameManage>
{
     void Start()
    {
        
    }
    public  void Init()
    {
        //ע��ʱ��
        AddEvent();
    }
    //ע���¼�
    public void AddEvent()
    {
        ////׼����Ϸ
        //EventDispatcher.instance.Regist(EventNameDef.READY_GAME, (v) => ReadyGame());
        ////��ʼ��Ϸ
        //EventDispatcher.instance.Regist(EventNameDef.RUNNING_GAME, (v) => StartGame());
        ////������Ϸ
        //EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => OverGame());
    }
    public GameState gameState;

    protected override void Awake()
    {
        gameState = GameState.Loading;
    }

    public bool JudgeState(GameState state)
    {
        return gameState == state;
    }

    public void SwitchState(GameState state)
    {
        gameState = state;
    }
    void Update()
    {
        if (gameState == GameState.Running)
        {
            if (PreController.Instance.KillEnemyNun >= GameFlowManager.Instance.EnemyTotalNum)
            {
                //UIManager.Instance.ChangeState(GameState.NextLevel);
                //GameFlowManager.Instance.NextLevel();
            }
        }
    }

}
