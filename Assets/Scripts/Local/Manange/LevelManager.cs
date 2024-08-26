using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public LevelData levelData;  // ��ǰ�ؿ�����
    public bool isLoadBack = false;
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    private List<GameObject> bulletPrefabs = new List<GameObject>();


    //����LoadScene������������һ������
    public void LoadScene(string levelName, int levelIndex)
    {
        Addressables.LoadSceneAsync(levelName, LoadSceneMode.Single).Completed += (AsyncOperationHandle<SceneInstance> obj) =>
        {
            isLoadBack = true;
            OnLevelLoaded(obj, levelIndex);
        };
    }

    // �л�������ĳ�ʼ���ؿ�
    private void OnLevelLoaded(AsyncOperationHandle<SceneInstance> obj, int levelIndex)
    {
        LoadLevelAssets(levelIndex);
    }
    public int LevelAll = 2;
    //Get�ؿ�����
    public void Load(int levelIndex, System.Action onLoadComplete)
    {
        isLoadBack = false;
        enemyPrefabs.Clear();
        bulletPrefabs.Clear();
        int loadedLevels = 0; // ����������ɵĹؿ���
        for (int i = 0; i < LevelAll; i++)
        {
            int index = i;
            // ���� LevelData
            Addressables.LoadAssetAsync<LevelData>("LevelData" + index).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameFlowManager.Instance.levels[index] = handle.Result as LevelData;
                    //TTOD1�ڴ����Ӵ����ñ��ȡ�����ݸ�ֵ��LevelData��
                    loadedLevels++;
                    // ������йؿ���������ɣ������ص�
                    if (loadedLevels == LevelAll -1)
                    {
                        Debug.Log(GameFlowManager.Instance.levels[index] + "LevelData");
                        onLoadComplete?.Invoke();
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load LevelData {index}");
                }
            };
        }
    }
  

    public void LoadLevelAssets(int levelIndex)
    {
        isLoadBack = false;
        enemyPrefabs.Clear();
        bulletPrefabs.Clear();
        // ���ز����ɵ���
        for (int i = 0; i < levelData.enemyAddresses.Length; i++)
        {
            Addressables.LoadAssetAsync<GameObject>(levelData.enemyAddresses[i]).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    enemyPrefabs.Add(handle.Result);
                    CheckAndInitializeLevel(levelData);
                }
            };
        }

        // ���ز������ӵ�
        for (int i = 0; i < levelData.bulletAddresses.Length; i++)
        {
            Addressables.LoadAssetAsync<GameObject>(levelData.bulletAddresses[i]).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    bulletPrefabs.Add(handle.Result);
                    CheckAndInitializeLevel(levelData);
                }
            };
        }
    }

    private void CheckAndInitializeLevel(LevelData currentLevel)
    {
        // �����е��˺��ӵ����������ʱ����ʼ���ؿ�
        if (enemyPrefabs.Count == currentLevel.enemyAddresses.Length && bulletPrefabs.Count == currentLevel.bulletAddresses.Length)
        {
            PreController.Instance.Init(enemyPrefabs, bulletPrefabs);
        }
    }


    //��Ϸ��ͼ
    public void OnBackgroundLoaded(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite background = handle.Result;
            //BackgroundScroller scroller = FindObjectOfType<BackgroundScroller>();
            //if (scroller != null)
            //{
            BackgroundScroller.Instance.SetBackground(background);
           // }
        }
    }
}
