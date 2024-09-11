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
    public GameObject buffDoorPrefab; // Buff �ŵ�Ԥ����
    public Transform spawnPoint;      // Buff �����ɵ�λ��
    public bool isGetdoor;
    public bool isPlaydoor;
    public float gameStartTime = 0;      // ��¼��Ϸ��ʼ��ʱ��
    public float nextBuffTime;       // �´����� buff �ŵ�ʱ��
    public float buffInterval = 30f; // ÿ������������һ�� buff ��
    public GameState gameState;
    public BuffDoorController buffDoorController;

    protected override void Awake()
    {
        gameState = GameState.Loading;
    }

    void Start()
    {
        // ��ʼ��
        //Init();
    }

    public void Init()
    {
        isGetdoor = false;
        isPlaydoor = false;
        nextBuffTime = gameStartTime + buffInterval; // ��ʼ���´����� buff �ŵ�ʱ��
    }

    // ע���¼�
    public void AddEvent()
    {
        ////׼����Ϸ
        //EventDispatcher.instance.Regist(EventNameDef.READY_GAME, (v) => ReadyGame());
        ////��ʼ��Ϸ
        //EventDispatcher.instance.Regist(EventNameDef.RUNNING_GAME, (v) => StartGame());
        ////������Ϸ
        //EventDispatcher.instance.Regist(EventNameDef.GAME_OVER, (v) => OverGame());
    }



    void Update()
    {
        // ����Ϸ״̬Ϊ Running ʱ������Ƿ���Ҫ���� buff ��
        if (gameState == GameState.Running)
        {
            // �����Ϸ�ոս��� Running ״̬�����ÿ�ʼʱ��
            gameStartTime += Time.deltaTime;
            if (isGetdoor)
            {
                buffDoorPrefab = GameObject.Find("BuffDoor");
                buffDoorController = buffDoorPrefab.GetComponent<BuffDoorController>();
                isGetdoor = false;
                isPlaydoor = true;
            }
            // ����Ƿ������� buff �ŵ�ʱ��
            if (gameStartTime >= nextBuffTime && isPlaydoor)
            {
                isPlaydoor = false;
                buffDoorController.SpawnBuffDoor();
                nextBuffTime = gameStartTime + buffInterval; // �����´����� buff �ŵ�ʱ��
            }
        }

        // ����Ϸ״̬Ϊ GameOver ʱ��ֹͣ���� buff ��
        if (gameState == GameState.GameOver)
        {
            StopBuffGeneration();
        }
    }


    // ֹͣ���� Buff �ŵķ���
    private void StopBuffGeneration()
    {
        // �����������ִ���κ���Ҫ�Ĳ�����ֹͣ��Ϸ�е�������Ϊ
        Debug.Log("ֹͣ��Ϸ���ŵ�������Ϊ");
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
        // ����Ϸ״̬�л�Ϊ Running ʱ��������Ϸ��ʼʱ��
        if (state == GameState.Running)
        {
            gameStartTime = 0f; // ������Ϸ��ʼʱ��
        }
    }
}
