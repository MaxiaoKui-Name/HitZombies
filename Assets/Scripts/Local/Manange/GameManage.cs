using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Loading,
    Ready,
    Running,
    Guid,
    NextLevel,
    Resue,
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
    //public BuffDoorController buffDoorController;
    //public int indexChest;
    // 宝箱生成相关变量
    public float delayTime = 10f;     // 宝箱生成的初始延迟时间
    public float chestInterval = 10f; // 每隔10秒生成一个宝箱
    public float nextChestTime;      // 下一次生成宝箱的时间
    public bool isFrozen = false; // 添加冰冻状态变量
    public bool JudgeVic = false; // 添加冰冻状态变量

    public int KilledMonsterNun;
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
        isFrozen = false;
        isFirstSpawnChest = false;
        gameStartTime = 0;
        KilledMonsterNun = 0;
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
    public bool isFirstSpawnChest;
    void Update()
    {
        // 当游戏状态为 Running 时，检查是否需要生成 buff 门和宝箱
        if (gameState == GameState.Running && GameFlowManager.Instance.currentLevelIndex != 0)
        {
            if (isFrozen) return; // 冰冻期间停止所有生成逻辑
            gameStartTime += Time.deltaTime;
            if (gameStartTime >= nextBuffTime)
            {
                SpawnBuffDoor();
                nextBuffTime = gameStartTime + buffInterval; // 更新下次生成 buff 门的时间
            }

            // 检查并生成宝箱逻辑
            if (gameStartTime >= delayTime && !isFirstSpawnChest)
            {
                if (gameStartTime >= nextChestTime)
                {
                    isFirstSpawnChest = true;
                    SpawnChest(); // 生成宝箱
                    nextChestTime = gameStartTime + chestInterval; // 更新下次生成宝箱的时间
                }
            }
            else
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
        ////TTOD2测试使用胜利判断逻辑
        if (JudgeVic)
        {
            if (GameManage.Instance.KilledMonsterNun >= LevelManager.Instance.levelData.WavesEnemyNun)//LevelManager.Instance.levelData.WavesEnemyNun)
            {
                JudgeVic = false;
                //弹出胜利结算面板
                GameManage.Instance.GameOverReset();
                UIManager.Instance.ChangeState(GameState.NextLevel);
                PreController.Instance.TestSuccessful = false;
            }
        }
    }

    // 停止生成 Buff 门的方法
    private void StopBuffGeneration()
    {
        Debug.Log("停止游戏中门的生成行为");
    }

    // 生成宝箱的方法
    public void SpawnChest()
    {
        Vector3 spawnChestPoint = PreController.Instance.RandomPosition(LevelManager.Instance.levelData.enemySpawnPoints);//
        //indexChest = GetCoinIndex();
        if(LevelManager.Instance.levelData.ChestList[0] != null)
        {
        GameObject ChestObj = Instantiate(LevelManager.Instance.levelData.ChestList[0], spawnChestPoint, Quaternion.identity);
        PreController.Instance.FixSortLayer(ChestObj);

        }
    }
    // 生成Buff门的方法
    public void SpawnBuffDoor()
    {
        Vector3 spawnBuffDoorPoint = new Vector3(0, 5.8f, 0f);//
        GameObject BuffDoor = Instantiate(LevelManager.Instance.levelData.buffDoor, spawnBuffDoorPoint, Quaternion.identity);
        PreController.Instance.FixSortLayer(BuffDoor);
        //设置门的初始文本
        BuffDoorController buffDoorController = BuffDoor.GetComponent<BuffDoorController>();
        buffDoorController.GetBuffDoorIDAndText(BuffDoor);
    }
    public void SetFrozenState(bool frozen)
    {
        isFrozen = frozen;

        // 冻结状态变更时，检查是否需要生成 Buff 门和宝箱
        if (!isFrozen)
        {
            // 恢复生成 Buff 门
            nextBuffTime = gameStartTime + buffInterval; // 更新下次生成时间
            isPlaydoor = true; // 允许生成 Buff 门

            // 恢复生成宝箱
            nextChestTime = gameStartTime + chestInterval; // 更新下次生成时间
        }
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
            Time.timeScale = 1f; // 暂停游戏
        }
        if (state == GameState.Guid)
        {
            Time.timeScale = 0f; // 暂停游戏
        }
    }
    public string GetChest(int index)
    {
        switch (index + 1)
        {
            case 1:
                return "BonusMINI";
            case 2:
                return "BonusMINOR";
            case 3:
                return "BonusMAJOR";
            case 4:
                return "BonusMAXI";
            case 5:
                return "BonusGRAND";
            default:
                return null;
        }
    }
    public void GameOverReset()
    {
        PreController.Instance.activeEnemyCount = 0;
      
        for (int i = 0; i < PreController.Instance.IEList.Count; i++)
        {
            //清理所有协程
            IEnumeratorTool.StopCoroutine(PreController.Instance.IEList[i]);
            PreController.Instance.IEList.Clear();
        }
        Init();
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.Init();
    }
}
