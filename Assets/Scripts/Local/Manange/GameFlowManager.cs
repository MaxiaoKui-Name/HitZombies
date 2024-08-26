using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LevelData[] levels;  // ���йؿ�����������
    private int currentLevelIndex = 0;
    public int EnemyTotalNum;
    

    void Awake()
    {
        // ���� LevelAll ��һ����������ʾ�ؿ�����
        levels = new LevelData[LevelManager.Instance.LevelAll];  // ��ʼ�����飬LevelAll Ϊ�ؿ�����
    }

    public void LoadLevel(int levelIndex)
    {
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
