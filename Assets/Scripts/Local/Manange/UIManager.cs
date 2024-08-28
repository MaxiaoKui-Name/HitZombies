using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject InitScenePanel; // ��ʼ������
    private GameObject ReadyPanel; // ׼������
    public int LevelTotal = 1;

   void Awake()
    {
        ChangeState(GameState.Loading);
    }
    void Start()
    {
        InitializeGame().Forget();
    }

    private async UniTask InitializeGame()
    {
        // �����л�������״̬
        LoadDll.Instance.InitAddressable();
        await UniTask.WaitUntil(() => LoadDll.Instance.successfullyLoaded);
        ChangeState(GameState.Ready);
        ConfigManager.Instance.Init();

        // ���ص�һ���ؿ�
        GameFlowManager.Instance.LoadLevel(0);

        // ������Ļ�ֱ���
        TrySetResolution(750, 1660);
    }


    public async UniTask ChangeState(GameState state)
    {
        switch (state)
        {
            case GameState.Loading:
                GameLoad();
                break;
            case GameState.Ready:
                GameReady();
                break;
            case GameState.Running:
                GameRunning();
                break;
            case GameState.NextLevel:
                // GameBalance();
                break;
        }
        GameManage.Instance.SwitchState(state);
    }

    private void GameLoad()
    {
        InitScenePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/InitScenePanel"));
        InitScenePanel.transform.SetParent(transform, false);
        InitScenePanel.transform.localPosition = Vector3.zero;

        // �ȴ��������ݱ���ɺ����л��� Ready ״̬
    }

    private void GameReady()
    {
        Destroy(InitScenePanel);
        ReadyPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/ReadyPanel"));
        ReadyPanel.transform.SetParent(transform, false);
        ReadyPanel.transform.localPosition = Vector3.zero;
    }

    private void GameRunning()
    {
        Destroy(ReadyPanel);
        // ��������״̬�ĳ�ʼ������
    }

    public void TrySetResolution(int width, int height)
    {
        float RealScale = 0.562626f;
        Vector2 curmaxWH;
        curmaxWH.x = Screen.currentResolution.width;
        curmaxWH.y = Screen.currentResolution.height;
        width = Mathf.Min(width, (int)curmaxWH.x);
        height = Mathf.Min(height, (int)curmaxWH.y);

        float givenWidth = width;
        float givenHeight = height;
        float givenRatio = givenWidth / givenHeight;
        int maxRatioWidth;
        int maxRatioHeight;
        if (givenRatio > System.Math.Round(RealScale, 2))
        {
            maxRatioWidth = (int)(givenHeight * RealScale);
            maxRatioHeight = (int)givenHeight;
        }
        else
        {
            maxRatioWidth = (int)givenWidth;
            maxRatioHeight = (int)(givenWidth / RealScale);
        }
        maxRatioWidth = maxRatioWidth - maxRatioWidth % 2;
        maxRatioHeight = maxRatioHeight - maxRatioHeight % 2;

        Screen.SetResolution(maxRatioWidth, maxRatioHeight, false);
    }
}
