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
    public List<LevelData> levels;  // ���йؿ�����������
    public int currentLevelIndex = 0;
    public int EnemyTotalNum;
    
    void Init()
    {
        levels = new List<LevelData>();  // ��ʼ�����飬LevelAll Ϊ�ؿ�����
    }
    //���عؿ�0����
    //public async UniTask LoadLevel(int levelIndex)
    //{
    //    if (levelIndex < 0 || levelIndex >= levels.Length)
    //    {
    //        Debug.LogError("��Ч�Ĺؿ�������");
    //        return;
    //    }
    //    if (LevelManager.Instance != null)
    //    {
    //        // ���� Load ������ȷ������ɺ��ټ���
    //        await LevelManager.Instance.Load(levelIndex, async () =>
    //        {
    //            // ȷ�� levels[levelIndex] �Ѿ���ɸ�ֵ
    //            LevelManager.Instance.levelData = levels[levelIndex];
    //            LevelManager.Instance.levelData.LevelIndex = levelIndex;
    //            LevelDataClear(levels[levelIndex]);
    //            // �ȴ� SetLevelData ���
    //            await SetLevelData(LevelManager.Instance.levelData, levelIndex);

    //            // ִ�н��������߼�
    //            for (int i = 0; i < LevelManager.Instance.levelData.backgroundAddress.Count; i++)
    //            {
    //                int index = i;  // �����ֲ��������浱ǰ�� i
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
            Debug.LogError("��Ч�Ĺؿ�������");
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
                    // ����һ�������б����ڼ������б���ͼƬ
                    List<UniTask> loadTasks = new List<UniTask>();

                    for (int i = 0; i < LevelManager.Instance.levelData.backgroundAddress.Count; i++)
                    {
                        int index = i;  // �����ֲ��������浱ǰ�� i
                        loadTasks.Add(LoadBackgroundAsync(LevelManager.Instance.levelData.backgroundAddress[index], index));
                    }

                    // �ȴ����б���ͼƬ�������
                    await UniTask.WhenAll(loadTasks);

                    // �������
                    tcs.TrySetResult();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"LoadLevel �첽�ص������쳣: {ex}");
                    tcs.TrySetException(ex);
                }
            });

            // �ȴ����м��ز������
            await tcs.Task;
            Debug.Log("��ʼ���ؿ����=============");
        }
    }
    //TTOD1�л��ؿ�������
    public async UniTask LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogError("��Ч�Ĺؿ�������");
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
        //TTOD���ñ�ֵ
        string backName1 = "Road";// ConfigManager.Instance.Tables.TableLevelConfig.Get(1).Resource;
        for (int i = 0; i < 2; i++)
        {
            levelData.backgroundAddress.Add(backName1 + i);
        }
        //TTOD1��ӱ��������ӵ�Ԥ����("Bullet")�Լ���Ӧ��ǹ;
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
        //���ֹ�����
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
            Debug.Log("���ֹصĹ���������" + levelData.WavesEnemyNun);
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
            Debug.Log("��ǰ�صĹ���������" + levelData.WavesEnemyNun);
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
        #region//ԭ���Ĵ���
        ////TTOD���ñ�ֵ
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
        ////TTOD1��ӱ��������ӵ�Ԥ����("Bullet")�Լ���Ӧ��ǹ;
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


    //TTOD����WavesenEmiesDic�ļ�ֵ�������Ӧ��Ԥ��������
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
    //�л��ؿ�
    public async UniTask NextLevel()
    {
        //TTOD1����ͨ�����UI��ʾ�߼����Լ��л���һ����Ϸ  �Ƿ�ͨ���л������ķ�ʽ
        //currentLevelIndex++;
        if (currentLevelIndex < levels.Count)
        {
            Debug.Log("�ؿ�ָ��" + currentLevelIndex);
            //���ر�������
            await LoadLevel(currentLevelIndex);
            GameManage.Instance.KilledMonsterNun = 0;
            PlayInforManager.Instance.playInfor.attackSpFac = 0;
            //���ݼ��صı������ݿ�ʼ��Ϸ����
            LevelManager.Instance.LoadLevelAssets(currentLevelIndex);
            LevelManager.Instance.CheckAndInitializeLevel();
        }
        else
        {
            Debug.Log("���йؿ���ɣ�����ȸ���...");
            // ����������Ե����ȸ����߼�
            CheckForUpdates();
        }
    }

    private void CheckForUpdates()
    {
        // ʹ��HybirdCLR��Addressables�����ȸ��¼�������
        // ����ʵ�����������ǵĸ��²��Ժͷ������߼�
    }
}
