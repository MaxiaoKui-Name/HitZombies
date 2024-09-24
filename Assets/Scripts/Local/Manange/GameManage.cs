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
    public GameObject buffDoorPrefab;  // Buff 门的预制体
    public Transform spawnPoint;       // Buff 门生成的位置
    public bool isGetdoor;
    public bool isPlaydoor;
    public float gameStartTime = 0;      // 记录游戏开始的时间
    public float nextBuffTime;           // 下次生成 buff 门的时间
    public float buffInterval;     // 每隔多少秒生成一次 buff 门
    public GameState gameState;
    public BuffDoorController buffDoorController;

    // 宝箱生成相关变量
    public float delayTime = 10f;     // 宝箱生成的初始延迟时间
    public float chestInterval = 10f; // 每隔10秒生成一个宝箱
    private float nextChestTime;      // 下一次生成宝箱的时间

    protected override void Awake()
    {
        gameState = GameState.Loading;
    }

    void Start()
    {
        // 初始化游戏
        //Init();
    }

    public void Init()
    {
        isGetdoor = false;
        isPlaydoor = false;
        buffInterval = ConfigManager.Instance.Tables.TableDoorgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Interval / 1000f;
        delayTime = ConfigManager.Instance.Tables.TableBoxgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Delay / 1000f;
        chestInterval = ConfigManager.Instance.Tables.TableBoxgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Interval / 1000f;
        nextBuffTime = gameStartTime + buffInterval; // 初始化下次生成 buff 门的时间
        nextChestTime = delayTime + chestInterval; // 初始化下次生成宝箱的时间
    }

    // 注册事件
    public void AddEvent()
    {
        // 注册事件的例子
        //EventDispatcher.instance.Regist(EventNameDef.READY_GAME, (v) => ReadyGame());
        //EventDispatcher.instance.Regist(EventNameDef.RUNNING_GAME, (v) => StartGame());
        //EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => OverGame());
    }

    void Update()
    {
        // 当游戏状态为 Running 时，检查是否需要生成 buff 门和宝箱
        if (gameState == GameState.Running)
        {
            gameStartTime += Time.deltaTime;

            // 检查并生成 Buff 门逻辑
            if (isGetdoor)
            {
                buffDoorPrefab = GameObject.Find("BuffDoor");
                buffDoorController = buffDoorPrefab.GetComponent<BuffDoorController>();
                isGetdoor = false;
                isPlaydoor = true;
            }

            if (gameStartTime >= nextBuffTime && isPlaydoor)
            {
                isPlaydoor = false;
                buffDoorController.SpawnBuffDoor();
                nextBuffTime = gameStartTime + buffInterval; // 更新下次生成 buff 门的时间
            }

            // 检查并生成宝箱逻辑
            if (gameStartTime >= delayTime)
            {
                if (gameStartTime >= nextChestTime)
                {
                    SpawnChest(); // 生成宝箱
                    nextChestTime = gameStartTime + chestInterval; // 更新下次生成宝箱的时间
                }
            }
        }

        // 当游戏状态为 GameOver 时，停止生成 Buff 门和宝箱
        if (gameState == GameState.GameOver)
        {
            StopBuffGeneration();
        }
    }

    // 停止生成 Buff 门的方法
    private void StopBuffGeneration()
    {
        Debug.Log("停止游戏中门的生成行为");
    }

    // 生成宝箱的方法
    private void SpawnChest()
    {
            Vector3 spawnChestPoint = PreController.Instance.RandomPosition(LevelManager.Instance.levelData.enemySpawnPoints);//
            int chestId = Random.Range(0, 2);
            GameObject ChestObj = Instantiate(LevelManager.Instance.levelData.ChestList[chestId], spawnChestPoint, Quaternion.identity);
            PreController.Instance.FixSortLayer(ChestObj);
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
          
        }
        // 当游戏状态切换为 Running 时，重置游戏开始时间
        if (state == GameState.Running)
        {
            gameStartTime = 0f; // 重置游戏开始时间
        }
    }
    public string GetChest(int index)
    {
        switch (index + 1)
        {
            case 1:
                return "Bonus_gold";
            case 2:
                return "Bonus_green";
            case 3:
                return "Bonus_purple";
            default:
                return null;
        }
    }
}
