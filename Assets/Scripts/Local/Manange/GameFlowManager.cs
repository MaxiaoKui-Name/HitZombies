using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LevelData[] levels;  // 所有关卡的配置数据
    private int currentLevelIndex = 0;
    public int EnemyTotalNum;
    

    void Awake()
    {
        // 假设 LevelAll 是一个常量，表示关卡总数
        levels = new LevelData[LevelManager.Instance.LevelAll];  // 初始化数组，LevelAll 为关卡总数
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("无效的关卡索引！");
            return;
        }

        if (LevelManager.Instance != null)
        {
            // 调用 Load 方法并确保其完成后再继续
            LevelManager.Instance.Load(levelIndex, () =>
            {
                // 确保 levels[levelIndex] 已经完成赋值
                LevelManager.Instance.levelData = levels[levelIndex];
                EnemyTotalNum = 0;

                for (int i = 0; i < LevelManager.Instance.levelData.enemyNum.Length; i++)
                {
                    EnemyTotalNum += LevelManager.Instance.levelData.enemyNum[i];
                }

                Addressables.LoadAssetAsync<Sprite>(LevelManager.Instance.levelData.backgroundAddress[levelIndex]).Completed += LevelManager.Instance.OnBackgroundLoaded;
            });
        }
    }

    //切换关卡
    public void NextLevel()
    {
        //TTOD增加通关完成UI显示逻辑、以及切换下一关游戏
        currentLevelIndex++;
        if (currentLevelIndex < levels.Length)
        {
            Debug.Log("关卡指数" + currentLevelIndex);
            LevelManager.Instance.levelData = levels[currentLevelIndex];
            Addressables.LoadAssetAsync<Sprite>(levels[currentLevelIndex].backgroundAddress[0]).Completed += LevelManager.Instance.OnBackgroundLoaded;
            LevelManager.Instance.LoadLevelAssets(currentLevelIndex);
        }
        else
        {
            Debug.Log("所有关卡完成！检查热更新...");
            // 在这里你可以调用热更新逻辑
            CheckForUpdates();
        }
    }

    private void CheckForUpdates()
    {
        // 使用HybirdCLR和Addressables进行热更新检查和下载
        // 具体实现依赖于你们的更新策略和服务器逻辑
    }
}
