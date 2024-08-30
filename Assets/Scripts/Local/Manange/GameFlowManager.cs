using cfg;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LevelData[] levels;  // 所有关卡的配置数据
    private int currentLevelIndex = 0;
    public int EnemyTotalNum;
    
    void Init()
    {
        levels = new LevelData[LevelManager.Instance.LevelAll];  // 初始化数组，LevelAll 为关卡总数
    }
    
    public void LoadLevel(int levelIndex)
    {
        tables = ConfigManager.Instance.Tables;
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
                SetLevelData(levelIndex);
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
    public Tables tables;
    public void SetLevelData(int levelIndex)
    {
        //TTOD配置表赋值
        LevelData levelData = null;
        levelData.backgroundAddress.Add(ConfigManager.Instance.Tables.TableSectionReslevelConfig.Get(0).Resource);
        //levelData.GunAddresses = ConfigManager.Instance.Tables.TableSectionReslevelConfig.Get(0).Weapon;
        //将skills的id与
        for (int i = 0; i < levelData.GunAddresses.Count; i++)
        {
            int index = levelData.GunAddresses[i];
            List<int> BulletIndex = tables.TableLevelResequipmentConfi.Get(index).Skills;
            foreach (var idindex in BulletIndex)
            {
                List<int> FiresList = tables.TableSkillResskillConfig.Get(idindex).Fires;
                for (int j = 0; j < FiresList.Count; j++)
                {
                    string bulletResName = tables.TableBulletResskillConfig.Get(FiresList[j]).Resource;
                    levelData.GunBulletDic.Add(bulletResName, BulletIndex);
                }
            }
        }
        levelData.Monsterwaves = ConfigManager.Instance.Tables.TableSectionReslevelConfig.Get(0).Map;
        for (int i = 0; i < levelData.Monsterwaves.Count; i++)
        {
            int index = levelData.Monsterwaves[i];
            levelData.WavesenEmiesDic.Add(index, ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Brushout1);
            levelData.WavesenEmiesDic.Add(index, ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Brushout2);
            levelData.WavesenEmiesDic.Add(index, ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Brushout3);
            levelData.WavesenEmiesDic.Add(index, ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Brushout4);
            levelData.WavesenEmiesDic.Add(index, ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Brushout5);
        }
    }
    //TTOD根据WavesenEmiesDic的键值来发射对应的预制体类型
    public string GetSpwanPre(int Idindex)
    {
        switch (Idindex)
        {
            case 1:
                return "Cuipi";
                break;
            case 2:
                return "jincheng";
                break;
            case 3:
                return "yuancheng";
                break;
            case 4:
                return "jingying";
                break;
            case 5:
                return "boss";
                break;
        }
        return null;

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
