using cfg;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LevelData[] levels;  // 所有关卡的配置数据
    public int currentLevelIndex = 0;
    public int EnemyTotalNum;
    
    void Init()
    {
        levels = new LevelData[LevelManager.Instance.LevelAll];  // 初始化数组，LevelAll 为关卡总数
    }
    //加载关卡0数据
    public async UniTask LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("无效的关卡索引！");
            return;
        }

        if (LevelManager.Instance != null)
        {
            // 调用 Load 方法并确保其完成后再继续
            await LevelManager.Instance.Load(levelIndex, async () =>
            {
                // 确保 levels[levelIndex] 已经完成赋值
                LevelManager.Instance.levelData = levels[levelIndex];

                // 等待 SetLevelData 完成
                await SetLevelData(LevelManager.Instance.levelData);

                // 执行接下来的逻辑
                for (int i = 0; i < LevelManager.Instance.levelData.backgroundAddress.Count; i++)
                {
                    Addressables.LoadAssetAsync<Sprite>(LevelManager.Instance.levelData.backgroundAddress[i])
                    .Completed += LevelManager.Instance.OnBackgroundLoaded;
                }
            });
        }
    }

     public async UniTask SetLevelData(LevelData levelData)
    {
        //TTOD配置表赋值
        Debug.Log(ConfigManager.Instance.Tables.TableSectionReslevelConfig.Get(0).Resource + "beijing================");
        string backName1 = ConfigManager.Instance.Tables.TableSectionReslevelConfig.Get(0).Resource;
        levelData.backgroundAddress.Add(backName1);
        levelData.Monsterwaves = ConfigManager.Instance.Tables.TableSectionReslevelConfig.Get(0).Map;
        for (int i = 0; i < levelData.Monsterwaves.Count; i++)
        {
            int index = levelData.Monsterwaves[i];
            List<List<int>> enemiesForWave = new List<List<int>>
            {
                ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Monster1,
                ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Monster2,
                ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Monster3,
                ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Monster4,
                ConfigManager.Instance.Tables.TableNestReslevelConfig.Get(index).Brushout5
            };
            levelData.WavesenEmiesDic.Add(index, enemiesForWave);
        }
        //TTOD添加本关所有子弹预制体("Bullet");// 
        levelData.GunBulletList.Add("bullet_04");
        levelData.CoinList.Add("gold");
        //levelData.GunBulletList.Add(ConfigManager.Instance.Tables.TableBulletResskillConfig.Get(20000).Resource);
        //levelData.GunBulletList.Add(ConfigManager.Instance.Tables.TableBulletResskillConfig.Get(20000).Resource);
        //levelData.GunBulletList.Add(ConfigManager.Instance.Tables.TableBulletResskillConfig.Get(20000).Resource);
    }

    
    //TTOD根据WavesenEmiesDic的键值来发射对应的预制体类型
    public string GetSpwanPre(int Idindex)
    {
        switch (Idindex)
        {
            case 1:
                return "zombie_001";
                break;
            case 2:
                return "zombieelite_004";
                break;
            case 3:
                return "zombieelite_003";
                break;
            case 4:
                return "zombieelite_005";
                break;
            case 5:
                return "boss_blue";
                break;
            case 6:
                return "zombie_002";
                break;
            case 7:
                return "boss";
            case 8:
                return "jingying";
                break;
            case 9:
                return "jingying";
                break;
        }
        return null;

    }
    public string GetBulletPre(int Idindex)
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
