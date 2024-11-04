using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject InitScenePanel; // 初始化页面
    private GameObject ReadyPanel; // 准备页面
    private GameObject GameMainPanel; // 准备页面
    private GameObject GameSuccessPanel; // 准备页面
    public int LevelTotal = 1;

    protected override void Awake()
    {
        // 调用基类的Awake方法
        base.Awake();
        // 在这里添加你自己的初始化逻辑
        ChangeState(GameState.Loading, 0);
    }
    void Start()
    {
        InitializeGame().Forget();
    }

    private async UniTask InitializeGame()
    {
        // 首先切换到加载状态
        LoadDll.Instance.InitAddressable();
        await UniTask.WaitUntil(() => LoadDll.Instance.successfullyLoaded);
        await ConfigManager.Instance.Init();
        //初始玩家信息
        PlayInforManager.Instance.Init();
        // 加载第一个关卡
        await GameFlowManager.Instance.LoadLevelInitial(0);
        ChangeState(GameState.Running,0);
        GameManage.Instance.Init();
        //ParticleManager.Instance.Init();
        //将配置表里的关卡数据写到Level
        //GameManage.Instance.Init();
        // 设置屏幕分辨率
        TrySetResolution(750, 1660);//ConfigManager.Instance.Tables.TableGlobalResConfig.Get(1).IntValue
    }


    public async UniTask ChangeState(GameState state,int currrntLevel)
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
                GameRunning(currrntLevel);
                break;
            case GameState.NextLevel:
                GameNextLev();
                break;
            case GameState.GameOver:
                // GameBalance();
                break;
        }
        if (currrntLevel == 0 && state == GameState.Running && !AccountManager.Instance.isGuid)
            return;
        GameManage.Instance.SwitchState(state);
    }

    private void GameLoad()
    {
        InitScenePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/InitScenePanel"));
        InitScenePanel.transform.SetParent(transform, false);
        InitScenePanel.transform.localPosition = Vector3.zero;
        // 等待加载数据表完成后再切换到 Ready 状态
    }
    private void GameNextLev()
    {
        GameSuccessPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/SuccessPanel"));
        GameSuccessPanel.transform.SetParent(transform, false);
        GameSuccessPanel.transform.localPosition = Vector3.zero;
        // 等待加载数据表完成后再切换到 Ready 状态
    }

    private void GameReady()
    {
        //AccountManager.Instance.ResetAccount();
        //AccountManager.Instance.LoadOrCreateAccount();
        GameMainPanel.SetActive(false);
        //Destroy(GameMainPanel);
        ReadyPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/ReadyPanel"));
        ReadyPanel.transform.SetParent(transform, false);
        ReadyPanel.transform.localPosition = Vector3.zero;
    }

    private async UniTask GameRunning(int currrntLevel)
    {
        //Destroy(ReadyPanel);
        AccountManager.Instance.ResetAccount();
        AccountManager.Instance.LoadOrCreateAccount();
        if(currrntLevel == 0)
        {
            Destroy(InitScenePanel);
            await LevelManager.Instance.LoadScene("First", currrntLevel);
            GameManage.Instance.SwitchState(GameState.Guid);
        }
        else
        {
            Destroy(ReadyPanel);
        }
        InfiniteScroll.Instance.baseScrollSpeed = 0.5f;// ConfigManager.Instance.Tables.TableGlobal.Get(6).IntValue;
        InfiniteScroll.Instance.baseGrowthRate = InfiniteScroll.Instance.baseScrollSpeed / 40;
        GameMainPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/GameMainPanel"));
        GameMainPanel.transform.SetParent(transform, false);
        GameMainPanel.transform.localPosition = Vector3.zero;
        // 其他运行状态的初始化操作
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
