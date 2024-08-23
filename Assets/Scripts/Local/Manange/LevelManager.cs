using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public LevelData levelData;  // 当前关卡数据
    public bool isLoadBack = false;
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    private List<GameObject> bulletPrefabs = new List<GameObject>();

    //调用LoadScene函数来加载下一个场景
    public void LoadScene(string levelName)
    {
        Addressables.LoadSceneAsync(levelName, LoadSceneMode.Single).Completed += OnLevelLoaded;
    }

    //切换场景后的初始化关卡
    private void OnLevelLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
       
        GameFlowManager.Instance.LoadLevel(0);
    }

    //生成关卡 
    public void Load(int levelIndex)
    {
        isLoadBack = false;
        enemyPrefabs.Clear();
        bulletPrefabs.Clear();
        Addressables.LoadAssetAsync<LevelData>("LevelData" + (levelIndex+1)).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 将加载到的 ScriptableObject 转换为 LevelData 类型并存储到 levels 列表中
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
        // 加载背景
        Addressables.LoadAssetAsync<Sprite>(currentLevel.backgroundAddress[levelIndex]).Completed += OnBackgroundLoaded;

        // 加载并生成敌人
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

        // 加载并生成子弹
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
        // 当所有敌人和子弹都加载完毕时，初始化关卡
        if (enemyPrefabs.Count == currentLevel.enemyAddresses.Length && bulletPrefabs.Count == currentLevel.bulletAddresses.Length)
        {
            PreController.Instance.Init(currentLevel, enemyPrefabs, bulletPrefabs);
        }
    }


    //游戏地图
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
