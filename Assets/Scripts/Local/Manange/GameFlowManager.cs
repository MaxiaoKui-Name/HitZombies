using cfg;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public List<LevelData> levels;  // 所有关卡的配置数据
    public int currentLevelIndex = 0;
    public int EnemyTotalNum;
    
    void Init()
    {
        levels = new List<LevelData>();  // 初始化数组，LevelAll 为关卡总数
    }
    //加载关卡0数据
    //public async UniTask LoadLevel(int levelIndex)
    //{
    //    if (levelIndex < 0 || levelIndex >= levels.Length)
    //    {
    //        Debug.LogError("无效的关卡索引！");
    //        return;
    //    }
    //    if (LevelManager.Instance != null)
    //    {
    //        // 调用 Load 方法并确保其完成后再继续
    //        await LevelManager.Instance.Load(levelIndex, async () =>
    //        {
    //            // 确保 levels[levelIndex] 已经完成赋值
    //            LevelManager.Instance.levelData = levels[levelIndex];
    //            LevelManager.Instance.levelData.LevelIndex = levelIndex;
    //            LevelDataClear(levels[levelIndex]);
    //            // 等待 SetLevelData 完成
    //            await SetLevelData(LevelManager.Instance.levelData, levelIndex);

    //            // 执行接下来的逻辑
    //            for (int i = 0; i < LevelManager.Instance.levelData.backgroundAddress.Count; i++)
    //            {
    //                int index = i;  // 创建局部变量保存当前的 i
    //                Addressables.LoadAssetAsync<Sprite>(LevelManager.Instance.levelData.backgroundAddress[index])
    //                .Completed += handle => LevelManager.Instance.OnBackgroundLoaded(handle, index);
    //            }
    //        });
    //    }
    //}

    public async UniTask LoadLevelInitial(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        if (levelIndex < 0)
        {
            Debug.LogError("无效的关卡索引！");
            return;
        }
        if (LevelManager.Instance != null)
        {
            var tcs = new UniTaskCompletionSource();

            await LevelManager.Instance.Load(levelIndex, async () =>
            {
                try
                {
                    LevelDataClear(levels[levelIndex]);
                    LevelManager.Instance.levelData = levels[levelIndex];
                    LevelManager.Instance.levelData.LevelIndex = levelIndex;
                    await SetLevelData(LevelManager.Instance.levelData, levelIndex);
                    // 创建一个任务列表，用于加载所有背景图片
                    List<UniTask> loadTasks = new List<UniTask>();

                    for (int i = 0; i < LevelManager.Instance.levelData.backgroundAddress.Count; i++)
                    {
                        int index = i;  // 创建局部变量保存当前的 i
                        loadTasks.Add(LoadBackgroundAsync(LevelManager.Instance.levelData.backgroundAddress[index], index));
                    }

                    // 等待所有背景图片加载完成
                    await UniTask.WhenAll(loadTasks);

                    // 完成任务
                    tcs.TrySetResult();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"LoadLevel 异步回调发生异常: {ex}");
                    tcs.TrySetException(ex);
                }
            });

            // 等待所有加载操作完成
            await tcs.Task;
            Debug.Log("初始化关卡完成=============");
        }
    }
    //TTOD1切换关卡待调用
    public async UniTask LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogError("无效的关卡索引！");
            return;
        }
        if (LevelManager.Instance != null)
        {
            LevelDataClear(levels[levelIndex]);
            LevelManager.Instance.levelData = levels[levelIndex];
            LevelManager.Instance.levelData.LevelIndex = levelIndex;
            await SetLevelData(LevelManager.Instance.levelData, levelIndex);
        }
    }

    public async UniTask LoadBackgroundAsync(string address, int index)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(address);
        await handle.ToUniTask();
        LevelManager.Instance.OnBackgroundLoaded(handle, index);
    }
    void LevelDataClear(LevelData levelData)
    {
        levelData.backgroundAddress.Clear();
        levelData.GunBulletList.Clear();
        levelData.Monsterwaves.Clear();
        levelData.WaveEnemyCountDic.Clear();
        levelData.WaveEnemyAllNumList.Clear();
        levelData.WavesenEmiesDic.Clear();
        levelData.CoinList.Clear();
        levelData.ChestList.Clear();
        levelData.WavesEnemyNun = 0;
    }
     public async UniTask SetLevelData(LevelData levelData,int levelIndex)
    {
        //TTOD配置表赋值
        string backName1 = "Road";// ConfigManager.Instance.Tables.TableLevelConfig.Get(1).Resource;
        for (int i = 0; i < 2; i++)
        {
            levelData.backgroundAddress.Add(backName1 + i);
        }
        //TTOD1添加本关所有子弹预制体("Bullet")以及对应的枪;
        levelData.GunBulletList.Add(new Gun(
            ConfigManager.Instance.Tables.TableTransmitConfig.Get(20000).Note,
            ConfigManager.Instance.Tables.TableTransmitConfig.Get(20000).Resource
        ));

        levelData.GunBulletList.Add(new Gun(
            ConfigManager.Instance.Tables.TableTransmitConfig.Get(20100).Note,
            ConfigManager.Instance.Tables.TableTransmitConfig.Get(20100).Resource
        ));

        levelData.GunBulletList.Add(new Gun(
            ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).Note,
            ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).Resource
        ));
        levelData.CoinList.Add("NewGold");
        //新手关配置
        if (levelIndex == 0)
        {
            for (int i = 1; i <= 10; i++)
            {
                levelData.Monsterwaves.Add(i);
                List<int> EnemyCount = new List<int>();
                int WaveEnemyNumAll = 0;
                for (int j = 0; j < 4; j++)
                {
                    EnemyCount.Add(GetMonsterId(i, j, levelIndex));
                    WaveEnemyNumAll += GetMonsterId(i, j, levelIndex);
                }
                levelData.WaveEnemyAllNumList.Add(WaveEnemyNumAll);
                levelData.WaveEnemyCountDic.Add(i, EnemyCount);
            }
            foreach (var num in levelData.WaveEnemyAllNumList)
            {
                levelData.WavesEnemyNun += num;
            }
            Debug.Log("新手关的怪物总数量" + levelData.WavesEnemyNun);
            for (int i = 0; i < levelData.Monsterwaves.Count; i++)
            {
                int index = levelData.Monsterwaves[i];
                List<List<int>> enemiesForWave = new List<List<int>>
            {
                ConfigManager.Instance.Tables.TableBeginnerConfig.Get(index).Monster1,
                ConfigManager.Instance.Tables.TableBeginnerConfig.Get(index).Monster2,
                ConfigManager.Instance.Tables.TableBeginnerConfig.Get(index).Monster3,
                ConfigManager.Instance.Tables.TableBeginnerConfig.Get(index).Monster4,
            };
                levelData.WavesenEmiesDic.Add(index, enemiesForWave);
            }
        }
        else
        {
            for (int i = (10 * (levelIndex - 1)) + 1; i <= ((levelIndex - 1) + 1) * 10; i++)
            {
                levelData.Monsterwaves.Add(i);
                List<int> EnemyCount = new List<int>();
                int WaveEnemyNumAll = 0;
                for (int j = 0; j < 4; j++)
                {
                    EnemyCount.Add(GetMonsterId(i, j, levelIndex));
                    WaveEnemyNumAll += GetMonsterId(i, j, levelIndex);
                }
                levelData.WaveEnemyAllNumList.Add(WaveEnemyNumAll);
                levelData.WaveEnemyCountDic.Add(i, EnemyCount);
            }
            foreach (var num in levelData.WaveEnemyAllNumList)
            {
                levelData.WavesEnemyNun += num;
            }
            Debug.Log("当前关的怪物总数量" + levelData.WavesEnemyNun);
            for (int i = 0; i < levelData.Monsterwaves.Count; i++)
            {
                int index = levelData.Monsterwaves[i];
                List<List<int>> enemiesForWave = new List<List<int>>
            {
                ConfigManager.Instance.Tables.TableLevelConfig.Get(index).Monster1,
                ConfigManager.Instance.Tables.TableLevelConfig.Get(index).Monster2,
                ConfigManager.Instance.Tables.TableLevelConfig.Get(index).Monster3,
                ConfigManager.Instance.Tables.TableLevelConfig.Get(index).Monster4,
            };
                levelData.WavesenEmiesDic.Add(index, enemiesForWave);
            }
        }
        #region//原来的代码
        ////TTOD配置表赋值
        //string backName1 = "Road";// ConfigManager.Instance.Tables.TableLevelConfig.Get(1).Resource;
        //for(int i = 0;i < 2; i++)
        //{
        //    levelData.backgroundAddress.Add(backName1 + i);
        //}
        //for(int i = (10 * (levelIndex -1)) + 1;i <= ((levelIndex - 1) + 1) * 10; i++)
        //{
        //    levelData.Monsterwaves.Add(i);
        //    List<int> EnemyCount = new List<int>();
        //    int WaveEnemyNumAll = 0;
        //    for(int j = 0; j < 4; j++)
        //    {
        //        EnemyCount.Add(GetMonsterId(i, j));
        //        WaveEnemyNumAll += GetMonsterId(i, j);
        //    }
        //    levelData.WaveEnemyAllNumList.Add(WaveEnemyNumAll);
        //    levelData.WaveEnemyCountDic.Add(i, EnemyCount);
        //}

        //for (int i = 0; i < levelData.Monsterwaves.Count; i++)
        //{
        //    int index = levelData.Monsterwaves[i];
        //    List<List<int>> enemiesForWave = new List<List<int>>
        //    {
        //        ConfigManager.Instance.Tables.TableLevelConfig.Get(index).Monster1,
        //        ConfigManager.Instance.Tables.TableLevelConfig.Get(index).Monster2,
        //        ConfigManager.Instance.Tables.TableLevelConfig.Get(index).Monster3,
        //        ConfigManager.Instance.Tables.TableLevelConfig.Get(index).Monster4,
        //    };
        //    levelData.WavesenEmiesDic.Add(index, enemiesForWave);
        //}
        ////TTOD1添加本关所有子弹预制体("Bullet")以及对应的枪;
        //levelData.GunBulletList.Add(new Gun(
        //    ConfigManager.Instance.Tables.TableTransmitConfig.Get(20000).Note,
        //    ConfigManager.Instance.Tables.TableTransmitConfig.Get(20000).Resource
        //));

        //levelData.GunBulletList.Add(new Gun(
        //    ConfigManager.Instance.Tables.TableTransmitConfig.Get(20100).Note,
        //    ConfigManager.Instance.Tables.TableTransmitConfig.Get(20100).Resource
        //));

        //levelData.GunBulletList.Add(new Gun(
        //    ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).Note,
        //    ConfigManager.Instance.Tables.TableTransmitConfig.Get(20200).Resource
        //));
        //levelData.CoinList.Add("gold");
        #endregion
    }


    //TTOD根据WavesenEmiesDic的键值来发射对应的预制体类型
    public string GetSpwanPre(int Idindex)
    {
        switch (Idindex)
        {
            case 1:
                return "NormalMonster";
                break;
            case 2:
                return "BasketMonster";
                break;
            case 3:
                return "SteelMonster";
                break;
            case 4:
                return "HulkMonster";
                break;
        }
        return null;

    }
    private int GetMonsterId(int waveKey, int index,int levelIndex)
    {
        if (levelIndex == 0)
        {
            var enemyConfig = ConfigManager.Instance.Tables.TableBeginnerConfig.Get(waveKey);
            switch (index)
            {
                case 0:
                    return (int)Random.Range(enemyConfig.QuantityMin1, enemyConfig.QuantityMax1);
                case 1:
                    return (int)Random.Range(enemyConfig.QuantityMin2, enemyConfig.QuantityMax2);
                case 2:
                    return (int)Random.Range(enemyConfig.QuantityMin3, enemyConfig.QuantityMax3);
                case 3:
                    return (int)Random.Range(enemyConfig.QuantityMin4, enemyConfig.QuantityMax4);
                default:
                    return 0;
            }
        }
        else
        {
            var enemyConfig = ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey);
            float Numcoefficient = ConfigManager.Instance.Tables.TableDanConfig.Get(GameFlowManager.Instance.currentLevelIndex).NumberCoefficient;
            switch (index)
            {
                case 0:
                    return (int)(Random.Range(enemyConfig.QuantityMin1, enemyConfig.QuantityMax1) * Numcoefficient); ;
                case 1:
                    return (int)(Random.Range(enemyConfig.QuantityMin2, enemyConfig.QuantityMax2) * Numcoefficient);
                case 2:
                    return (int)(Random.Range(enemyConfig.QuantityMin4, enemyConfig.QuantityMax3) * Numcoefficient);
                case 3:
                    return (int)(Random.Range(enemyConfig.QuantityMin4, enemyConfig.QuantityMax4) * Numcoefficient);
                default:
                    return 0;
            }
        }
       
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
    public async UniTask NextLevel()
    {
        //TTOD1增加通关完成UI显示逻辑、以及切换下一关游戏  是否通过切换场景的方式
        //currentLevelIndex++;
        if (currentLevelIndex < levels.Count)
        {
            Debug.Log("关卡指数" + currentLevelIndex);
            //加载本关数据
            await LoadLevel(currentLevelIndex);
            GameManage.Instance.KilledMonsterNun = 0;
            PlayInforManager.Instance.playInfor.attackSpFac = 0;
            //根据加载的本关数据开始游戏进程
            LevelManager.Instance.LoadLevelAssets(currentLevelIndex);
            LevelManager.Instance.CheckAndInitializeLevel();
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
