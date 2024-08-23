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
    public void LoadScene(string levelName)
    {
        Addressables.LoadSceneAsync(levelName, LoadSceneMode.Single).Completed += OnLevelLoaded;
    }

    //�л�������ĳ�ʼ���ؿ�
    private void OnLevelLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
       
        GameFlowManager.Instance.LoadLevel(0);
    }

    //���ɹؿ� 
    public void Load(int levelIndex)
    {
        isLoadBack = false;
        enemyPrefabs.Clear();
        bulletPrefabs.Clear();
        Addressables.LoadAssetAsync<LevelData>("LevelData" + (levelIndex+1)).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // �����ص��� ScriptableObject ת��Ϊ LevelData ���Ͳ��洢�� levels �б���
                GameFlowManager.Instance.levels[levelIndex] = handle.Result as LevelData;
            }
            else
            {
                Debug.LogError($"Failed to load LevelData{levelIndex}");
            }
        };
        LoadLevelAssets(levelIndex);
      
    }

    private void LoadLevelAssets(int levelIndex)
    {
        var currentLevel = GameFlowManager.Instance.levels[levelIndex];
        // ���ر���
        Addressables.LoadAssetAsync<Sprite>(currentLevel.backgroundAddress[levelIndex]).Completed += OnBackgroundLoaded;

        // ���ز����ɵ���
        for (int i = 0; i < currentLevel.enemyAddresses.Length; i++)
        {
            Addressables.LoadAssetAsync<GameObject>(currentLevel.enemyAddresses[i]).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    enemyPrefabs.Add(handle.Result);
                    CheckAndInitializeLevel(currentLevel);
                }
            };
        }

        // ���ز������ӵ�
        for (int i = 0; i < currentLevel.bulletAddresses.Length; i++)
        {
            Addressables.LoadAssetAsync<GameObject>(currentLevel.bulletAddresses[i]).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    bulletPrefabs.Add(handle.Result);
                    CheckAndInitializeLevel(currentLevel);
                }
            };
        }
    }

    private void CheckAndInitializeLevel(LevelData currentLevel)
    {
        // �����е��˺��ӵ����������ʱ����ʼ���ؿ�
        if (enemyPrefabs.Count == currentLevel.enemyAddresses.Length && bulletPrefabs.Count == currentLevel.bulletAddresses.Length)
        {
            PreController.Instance.Init(currentLevel, enemyPrefabs, bulletPrefabs);
        }
    }


    //��Ϸ��ͼ
    private void OnBackgroundLoaded(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite background = handle.Result;
            BackgroundScroller scroller = FindObjectOfType<BackgroundScroller>();
            if (scroller != null)
            {
                isLoadBack = true;
                scroller.SetBackground(background);
            }
        }
    }
}
