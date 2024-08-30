using cfg;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LevelData[] levels;  // ���йؿ�����������
    private int currentLevelIndex = 0;
    public int EnemyTotalNum;
    
    void Init()
    {
        levels = new LevelData[LevelManager.Instance.LevelAll];  // ��ʼ�����飬LevelAll Ϊ�ؿ�����
    }
    
    public void LoadLevel(int levelIndex)
    {
        tables = ConfigManager.Instance.Tables;
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("��Ч�Ĺؿ�������");
            return;
        }

        if (LevelManager.Instance != null)
        {
            // ���� Load ������ȷ������ɺ��ټ���
            LevelManager.Instance.Load(levelIndex, () =>
            {
                // ȷ�� levels[levelIndex] �Ѿ���ɸ�ֵ
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
        //TTOD���ñ�ֵ
        LevelData levelData = null;
        levelData.backgroundAddress.Add(ConfigManager.Instance.Tables.TableSectionReslevelConfig.Get(0).Resource);
        //levelData.GunAddresses = ConfigManager.Instance.Tables.TableSectionReslevelConfig.Get(0).Weapon;
        //��skills��id��
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
    //TTOD����WavesenEmiesDic�ļ�ֵ�������Ӧ��Ԥ��������
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
