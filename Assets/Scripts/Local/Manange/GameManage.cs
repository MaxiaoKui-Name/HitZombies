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
    //public BuffDoorController buffDoorController;
    //public int indexChest;
    // ����������ر���
    public float delayTime = 10f;     // �������ɵĳ�ʼ�ӳ�ʱ��
    public float chestInterval = 10f; // ÿ��10������һ������
    private float nextChestTime;      // ��һ�����ɱ����ʱ��
    public bool isFrozen = false; // ��ӱ���״̬����
    

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
        isFrozen = false;
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
            if (isFrozen) return; // �����ڼ�ֹͣ���������߼�
            gameStartTime += Time.deltaTime;

            if (gameStartTime >= nextBuffTime)
            {
                SpawnBuffDoor();
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
            //indexChest = GetCoinIndex();
            GameObject ChestObj = Instantiate(LevelManager.Instance.levelData.ChestList[0], spawnChestPoint, Quaternion.identity);
            PreController.Instance.FixSortLayer(ChestObj);
    }
    // ����Buff�ŵķ���
    public void SpawnBuffDoor()
    {
        Vector3 spawnBuffDoorPoint = new Vector3(0, 5.8f, 0f);//
        GameObject BuffDoor = Instantiate(LevelManager.Instance.levelData.buffDoor, spawnBuffDoorPoint, Quaternion.identity);
        PreController.Instance.FixSortLayer(BuffDoor);
        //�����ŵĳ�ʼ�ı�
        BuffDoorController buffDoorController = BuffDoor.GetComponent<BuffDoorController>();
        buffDoorController.GetBuffDoorIDAndText(BuffDoor);
    }
    public void SetFrozenState(bool frozen)
    {
        isFrozen = frozen;

        // ����״̬���ʱ������Ƿ���Ҫ���� Buff �źͱ���
        if (!isFrozen)
        {
            // �ָ����� Buff ��
            nextBuffTime = gameStartTime + buffInterval; // �����´�����ʱ��
            isPlaydoor = true; // �������� Buff ��

            // �ָ����ɱ���
            nextChestTime = gameStartTime + chestInterval; // �����´�����ʱ��
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
    
}
