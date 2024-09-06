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
        //注册时间
        AddEvent();
    }
    //注册事件
    public void AddEvent()
    {
        ////准备游戏
        //EventDispatcher.instance.Regist(EventNameDef.READY_GAME, (v) => ReadyGame());
        ////开始游戏
        //EventDispatcher.instance.Regist(EventNameDef.RUNNING_GAME, (v) => StartGame());
        ////结束游戏
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
