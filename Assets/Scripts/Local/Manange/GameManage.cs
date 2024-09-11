using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject buffDoorPrefab; // Buff 门的预制体
    public Transform spawnPoint;      // Buff 门生成的位置
    public bool isGetdoor;
    public bool isPlaydoor;
    public float gameStartTime = 0;      // 记录游戏开始的时间
    public float nextBuffTime;       // 下次生成 buff 门的时间
    public float buffInterval = 30f; // 每隔多少秒生成一次 buff 门
    public GameState gameState;
    public BuffDoorController buffDoorController;

    protected override void Awake()
    {
        gameState = GameState.Loading;
    }

    void Start()
    {
        // 初始化
        //Init();
    }

    public void Init()
    {
        isGetdoor = false;
        isPlaydoor = false;
        nextBuffTime = gameStartTime + buffInterval; // 初始化下次生成 buff 门的时间
    }

    // 注册事件
    public void AddEvent()
    {
        ////准备游戏
        //EventDispatcher.instance.Regist(EventNameDef.READY_GAME, (v) => ReadyGame());
        ////开始游戏
        //EventDispatcher.instance.Regist(EventNameDef.RUNNING_GAME, (v) => StartGame());
        ////结束游戏
        //EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => OverGame());
    }



    void Update()
    {
        // 当游戏状态为 Running 时，检查是否需要生成 buff 门
        if (gameState == GameState.Running)
        {
            // 如果游戏刚刚进入 Running 状态，设置开始时间
            gameStartTime += Time.deltaTime;
            if (isGetdoor)
            {
                buffDoorPrefab = GameObject.Find("BuffDoor");
                buffDoorController = buffDoorPrefab.GetComponent<BuffDoorController>();
                isGetdoor = false;
                isPlaydoor = true;
            }
            // 检查是否到了生成 buff 门的时间
            if (gameStartTime >= nextBuffTime && isPlaydoor)
            {
                isPlaydoor = false;
                buffDoorController.SpawnBuffDoor();
                nextBuffTime = gameStartTime + buffInterval; // 更新下次生成 buff 门的时间
            }
        }

        // 当游戏状态为 GameOver 时，停止生成 buff 门
        if (gameState == GameState.GameOver)
        {
            StopBuffGeneration();
        }
    }


    // 停止生成 Buff 门的方法
    private void StopBuffGeneration()
    {
        // 你可以在这里执行任何需要的操作来停止游戏中的生成行为
        Debug.Log("停止游戏中门的生成行为");
    }

    public bool JudgeState(GameState state)
    {
        return gameState == state;
    }

    public void SwitchState(GameState state)
    {
        gameState = state;
        if (state == GameState.Loading)
        {
            Init();
        }
        // 当游戏状态切换为 Running 时，重置游戏开始时间
        if (state == GameState.Running)
        {
            gameStartTime = 0f; // 重置游戏开始时间
        }
    }
}
