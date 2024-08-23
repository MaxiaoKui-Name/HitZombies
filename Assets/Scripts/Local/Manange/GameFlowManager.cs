using UnityEngine;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LevelData[] levels;  // 所有关卡的配置数据
    private int currentLevelIndex = 0;

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("无效的关卡索引！");
            return;
        }

        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.Load(levelIndex);
            //根据索引拿到不同关卡的数据
            levelManager.levelData = levels[levelIndex];
          
        }
    }
    //切换关卡
    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levels.Length)
        {
            LoadLevel(currentLevelIndex);
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
