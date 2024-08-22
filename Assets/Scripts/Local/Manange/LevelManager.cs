using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
//切关卡的脚本
public class LevelManager : Singleton<LevelManager>
{
    public LevelData levelData;  // 当前关卡数据
    //调用LoadLevel函数来加载下一个场景
    public void LoadLevel(string levelName)
    {
        Addressables.LoadSceneAsync(levelName, LoadSceneMode.Single);/*.Completed += OnLevelLoaded*/;
    }
    //切换场景后的初始化关卡
    private void OnLevelLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        LoadLevel();
    }
   
    //生成关卡 
    public void LoadLevel()
    {
        // 加载背景
        Addressables.LoadAssetAsync<Sprite>(levelData.backgroundAddress).Completed += OnBackgroundLoaded;

        // 加载并生成敌人
        foreach (var enemyAddress in levelData.enemyAddresses)
        {
            Addressables.LoadAssetAsync<GameObject>(enemyAddress).Completed += OnEnemyLoaded;
        }
    }

    private void OnBackgroundLoaded(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite background = handle.Result;
            // 设置背景
            SetBackground(background);
        }
    }

    private void OnEnemyLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject enemyPrefab = handle.Result;
            // 根据关卡数据生成敌人
            Instantiate(enemyPrefab, levelData.enemySpawnPoints, Quaternion.identity);
        }
    }

    private void SetBackground(Sprite background)
    {
        // 假设你有一个背景图片的GameObject
        GameObject backgroundObject = GameObject.Find("Background");
        if (backgroundObject != null)
        {
            var spriteRenderer = backgroundObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = background;
            }
        }
    }

}

