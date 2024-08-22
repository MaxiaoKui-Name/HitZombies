using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    private GameObject InitScenePanel;//初始化场景
    private GameObject ReadyPanel;//准备场景

    private void Start()
    {
        ChangeState(GameState.Loading);
        LoadDll.Instance.InitAddressable();
        //Screen.SetResolution(557, 990, false);
        TrySetResolution(750, 1660);
    }
    //public void ShowInputRC()
    //{
    //    inputRoomCodePanel = Instantiate(Resources.Load<GameObject>("UI/InputRoomCodePanel"));
    //    inputRoomCodePanel.transform.SetParent(transform, false);
    //    inputRoomCodePanel.transform.localPosition = Vector3.zero;
    //}

    public void ChangeState(GameState state)
    {
        switch (state)
        {
            case GameState.Loading:
                GameLoad();
                break;
            case GameState.Ready:
                GameReady();
                break;
                //case GameState.Gaming:
                //    GameGaming();
                //    break;
                //case GameState.Balance:
                //    GameBalance();
                //    break;
        }
        GameManage.Instance.SwitchState(state);
    }

    private async void GameLoad()
    {
        InitScenePanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/InitScenePanel"));
        InitScenePanel.transform.SetParent(transform, false);
        InitScenePanel.transform.localPosition = Vector3.zero;
        //TTOD加载数据表成功后在出现ReadyPanel
        await UniTask.WhenAll(UniTask.Delay(3000), UniTask.WaitUntil(() => { return LoadDll.Instance.successfullyLoaded; }));
        ChangeState(GameState.Ready);
    }
    private void GameReady()
    {
        InitScenePanel.SetActive(false);
        ReadyPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UIPannel/ReadyPanel"));
        ReadyPanel.transform.SetParent(transform, false);
        ReadyPanel.transform.localPosition = Vector3.zero;
    }
    //private void GameGaming()
    //{
    //    Destroy(waitPanel);
    //    gameMainPanel = Instantiate(Resources.Load<GameObject>("UI/GameMainPanel"));
    //    gameMainPanel.transform.SetParent(transform, false);
    //    gameMainPanel.transform.localPosition = Vector3.zero;
    //    giftMapPanel = Instantiate(Resources.Load<GameObject>("UI/GiftMapPanel"));
    //    giftMapPanel.transform.SetParent(transform, false);
    //    giftMapPanel.transform.localPosition = Vector3.zero;
    //    worldRankPanel = Instantiate(Resources.Load<GameObject>("UI/WorldRankPanelBefore"));
    //    worldRankPanel.transform.SetParent(transform, false);
    //    worldRankPanel.transform.localPosition = Vector3.zero;
    //    fairyStickHitPanel = Instantiate(Resources.Load<GameObject>("UI/FairyStickHitPanel"));
    //    fairyStickHitPanel.transform.SetParent(transform, false);
    //    fairyStickHitPanel.transform.localPosition = Vector3.zero;
    //    playerJoinPanel = Instantiate(Resources.Load<GameObject>("UI/PlayerJoinPanel"));
    //    playerJoinPanel.transform.SetParent(transform, false);
    //    playerJoinPanel.transform.localPosition = Vector3.zero;
    //    playerFansClubPanel = Instantiate(Resources.Load<GameObject>("UI/PlayerFansClubPanel"));
    //    playerFansClubPanel.transform.SetParent(transform, false);
    //    playerFansClubPanel.transform.localPosition = Vector3.zero;
    //    giftHitPanel = Instantiate(Resources.Load<GameObject>("UI/GiftHitPanel"));
    //    giftHitPanel.transform.SetParent(transform, false);
    //    giftHitPanel.transform.localPosition = Vector3.zero;
    //    bigBrotherPanel = Instantiate(Resources.Load<GameObject>("UI/BigBrotherPanel"));
    //    bigBrotherPanel.transform.SetParent(transform, false);
    //    bigBrotherPanel.transform.localPosition = Vector3.zero;
    //}
    //private void GameBalance()
    //{
    //    Destroy(gameMainPanel);
    //    Destroy(fairyStickHitPanel);
    //    Destroy(playerJoinPanel);
    //    Destroy(giftHitPanel);
    //    Destroy(bigBrotherPanel);
    //    Destroy(giftMapPanel);
    //    Destroy(worldRankPanel);
    //    Destroy(playerFansClubPanel);
    //    gameOverPanel = Instantiate(Resources.Load<GameObject>("UI/GameOverPanel"));
    //    gameOverPanel.transform.SetParent(transform, false);
    //    gameOverPanel.transform.localPosition = Vector3.zero;
    //    worldRankPane2 = Instantiate(Resources.Load<GameObject>("UI/WorldRankPanel"));
    //    worldRankPane2.transform.SetParent(transform, false);
    //    worldRankPane2.transform.localPosition = Vector3.zero;
    //    rankPanel = Instantiate(Resources.Load<GameObject>("UI/RankPanel"));
    //    rankPanel.transform.SetParent(transform, false);
    //    rankPanel.transform.localPosition = Vector3.zero;
    //}
    //protected override void OnAwake()
    //{

    //}
    //public string GetName(string str, int length = 5)
    //{
    //    if (str.Length > length)
    //    {
    //        str = str.Substring(0, length);
    //    }
    //    return str;
    //}
    public void TrySetResolution(int width, int height)
    {
        float RealScale = 0.562626f;
        Vector2 curmaxWH;
        //2020以上版本 API 会自动更新
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
