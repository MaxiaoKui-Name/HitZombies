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
    public GameObject buffDoorPrefab;  // Buff �ŵ�Ԥ����
    public Transform spawnPoint;       // Buff �����ɵ�λ��
    public bool isGetdoor;
    public bool isPlaydoor;
    public float gameStartTime = 0;      // ��¼��Ϸ��ʼ��ʱ��
    public float nextBuffTime;           // �´����� buff �ŵ�ʱ��
    public float buffInterval;     // ÿ������������һ�� buff ��
    public GameState gameState;
    public BuffDoorController buffDoorController;

    // ����������ر���
    public float delayTime = 10f;     // �������ɵĳ�ʼ�ӳ�ʱ��
    public float chestInterval = 10f; // ÿ��10������һ������
    private float nextChestTime;      // ��һ�����ɱ����ʱ��

    protected override void Awake()
    {
        gameState = GameState.Loading;
    }

    void Start()
    {
        // ��ʼ����Ϸ
        //Init();
    }

    public void Init()
    {
        isGetdoor = false;
        isPlaydoor = false;
        buffInterval = ConfigManager.Instance.Tables.TableDoorgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Interval / 1000f;
        delayTime = ConfigManager.Instance.Tables.TableBoxgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Delay / 1000f;
        chestInterval = ConfigManager.Instance.Tables.TableBoxgenerate.Get(GameFlowManager.Instance.currentLevelIndex).Interval / 1000f;
        nextBuffTime = gameStartTime + buffInterval; // ��ʼ���´����� buff �ŵ�ʱ��
        nextChestTime = delayTime + chestInterval; // ��ʼ���´����ɱ����ʱ��
    }

    // ע���¼�
    public void AddEvent()
    {
        // ע���¼�������
        //EventDispatcher.instance.Regist(EventNameDef.READY_GAME, (v) => ReadyGame());
        //EventDispatcher.instance.Regist(EventNameDef.RUNNING_GAME, (v) => StartGame());
        //EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => OverGame());
    }

    void Update()
    {
        // ����Ϸ״̬Ϊ Running ʱ������Ƿ���Ҫ���� buff �źͱ���
        if (gameState == GameState.Running)
        {
            gameStartTime += Time.deltaTime;

            // ��鲢���� Buff ���߼�
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
                nextBuffTime = gameStartTime + buffInterval; // �����´����� buff �ŵ�ʱ��
            }

            // ��鲢���ɱ����߼�
            if (gameStartTime >= delayTime)
            {
                if (gameStartTime >= nextChestTime)
                {
                    SpawnChest(); // ���ɱ���
                    nextChestTime = gameStartTime + chestInterval; // �����´����ɱ����ʱ��
                }
            }
        }

        // ����Ϸ״̬Ϊ GameOver ʱ��ֹͣ���� Buff �źͱ���
        if (gameState == GameState.GameOver)
        {
            StopBuffGeneration();
        }
    }

    // ֹͣ���� Buff �ŵķ���
    private void StopBuffGeneration()
    {
        Debug.Log("ֹͣ��Ϸ���ŵ�������Ϊ");
    }

    // ���ɱ���ķ���
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
        // ����Ϸ״̬�л�Ϊ Running ʱ��������Ϸ��ʼʱ��
        if (state == GameState.Running)
        {
            gameStartTime = 0f; // ������Ϸ��ʼʱ��
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
