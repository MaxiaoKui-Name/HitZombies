using cfg;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LevelData[] levels;  // ���йؿ�����������
    public int currentLevelIndex = 0;
    public int EnemyTotalNum;
    
    void Init()
    {
        levels = new LevelData[LevelManager.Instance.LevelAll];  // ��ʼ�����飬LevelAll Ϊ�ؿ�����
    }
    //���عؿ�0����
    public async UniTask LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("��Ч�Ĺؿ�������");
            return;
        }
        if (LevelManager.Instance != null)
        {
            // ���� Load ������ȷ������ɺ��ټ���
            await LevelManager.Instance.Load(levelIndex, async () =>
            {
                // ȷ�� levels[levelIndex] �Ѿ���ɸ�ֵ
                LevelManager.Instance.levelData = levels[levelIndex];
                LevelManager.Instance.levelData.LevelIndex = levelIndex;
                LevelDataClear(levels[levelIndex]);
                // �ȴ� SetLevelData ���
                await SetLevelData(LevelManager.Instance.levelData, levelIndex);

                // ִ�н��������߼�
                for (int i = 0; i < LevelManager.Instance.levelData.backgroundAddress.Count; i++)
                {
                    Addressables.LoadAssetAsync<Sprite>(LevelManager.Instance.levelData.backgroundAddress[i])
                    .Completed += LevelManager.Instance.OnBackgroundLoaded;
                }
            });
        }
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
    }
     public async UniTask SetLevelData(LevelData levelData,int levelIndex)
    {
        //TTOD���ñ�ֵ
        string backName1 = ConfigManager.Instance.Tables.TableLevelConfig.Get(1).Resource;
        levelData.backgroundAddress.Add(backName1);
        for(int i = (10 * levelIndex) + 1;i <= (levelIndex + 1) * 10; i++)
        {
            levelData.Monsterwaves.Add(i);
            List<int> EnemyCount = new List<int>();
            int WaveEnemyNumAll = 0;
            for(int j = 0; j < 4; j++)
            {
                EnemyCount.Add(GetMonsterId(i, j));
                WaveEnemyNumAll += GetMonsterId(i, j);
            }
            levelData.WaveEnemyAllNumList.Add(WaveEnemyNumAll);
            levelData.WaveEnemyCountDic.Add(i, EnemyCount);
        }

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
        //TTOD��ӱ��������ӵ�Ԥ����("Bullet");// 
        levelData.GunBulletList.Add("bullet_04");
        levelData.CoinList.Add("gold");
        //levelData.GunBulletList.Add(ConfigManager.Instance.Tables.TableBulletResskillConfig.Get(20000).Resource);
        //levelData.GunBulletList.Add(ConfigManager.Instance.Tables.TableBulletResskillConfig.Get(20000).Resource);
        //levelData.GunBulletList.Add(ConfigManager.Instance.Tables.TableBulletResskillConfig.Get(20000).Resource);
    }

    
    //TTOD����WavesenEmiesDic�ļ�ֵ�������Ӧ��Ԥ��������
    public string GetSpwanPre(int Idindex)
    {
        switch (Idindex)
        {
            case 1:
                return "zombie_001";
                break;
            case 2:
                return "zombie_002";
                break;
            case 3:
                return "zombieelite_004";
                break;
            case 4:
                return "zombieelite_003";
                break;
            case 5:
                return "zombieelite_005";
                break;
            case 100:
                return "boss_blue";
                break;
        }
        return null;

    }
    private int GetMonsterId(int waveKey, int index)
    {
        var enemyConfig = ConfigManager.Instance.Tables.TableLevelConfig.Get(waveKey);
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
    public void NextLevel()
    {
        //TTOD����ͨ�����UI��ʾ�߼����Լ��л���һ����Ϸ
        currentLevelIndex++;
        if (currentLevelIndex < levels.Length)
        {
            Debug.Log("�ؿ�ָ��" + currentLevelIndex);
            LevelManager.Instance.levelData = levels[currentLevelIndex];
            Addressables.LoadAssetAsync<Sprite>(levels[currentLevelIndex].backgroundAddress[0]).Completed += LevelManager.Instance.OnBackgroundLoaded;
            LevelManager.Instance.LoadLevelAssets(currentLevelIndex);
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
