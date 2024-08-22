using UnityEngine;
//管理玩家的游戏进度
public class ProgressManager : Singleton<ProgressManager>
{
    
    private int currentLevel;


    public void SaveProgress(int level)
    {
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.Save();
    }

    public int LoadProgress()
    {
        return PlayerPrefs.GetInt("CurrentLevel", 1);
    }
}
