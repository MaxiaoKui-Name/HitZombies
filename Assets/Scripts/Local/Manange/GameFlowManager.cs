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

                // �ȴ� SetLevelData ���
                await SetLevelData(LevelManager.Instance.levelData);

                // ִ�н��������߼�
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
        //TTOD���ñ�ֵ
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
