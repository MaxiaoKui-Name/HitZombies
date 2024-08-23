using UnityEngine;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LevelData[] levels;  // ���йؿ�����������
    private int currentLevelIndex = 0;

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("��Ч�Ĺؿ�������");
            return;
        }

        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.Load(levelIndex);
            //���������õ���ͬ�ؿ�������
            levelManager.levelData = levels[levelIndex];
          
        }
    }
    //�л��ؿ�
    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levels.Length)
        {
            LoadLevel(currentLevelIndex);
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
