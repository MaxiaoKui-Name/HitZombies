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
    public int LevelAll = 1;


    //调用LoadScene函数来加载下一个场景
    public void LoadScene(string levelName, int levelIndex)
    {
        Addressables.LoadSceneAsync(levelName, LoadSceneMode.Single).Completed += (AsyncOperationHandle<SceneInstance> obj) =>
        {
            isLoadBack = true;
            OnLevelLoaded(obj, levelIndex);
        };
    }

    // 切换场景后的初始化关卡
    private void OnLevelLoaded(AsyncOperationHandle<SceneInstance> obj, int levelIndex)
    {
        LoadLevelAssets(levelIndex);
    }
    //Get关卡数据
    public void Load(int levelIndex, System.Action onLoadComplete)
    {
        isLoadBack = false;
        enemyPrefabs.Clear();
        bulletPrefabs.Clear();
        int loadedLevels = 0; // 计数加载完成的关卡数
        for (int i = 0; i < LevelAll; i++)
        {
            int index = i;
            // 加载 LevelData
            Addressables.LoadAssetAsync<LevelData>("LevelData" + index).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameFlowManager.Instance.levels[index] = handle.Result as LevelData;
                    loadedLevels++;
                    // 如果所有关卡都加载完成，触发回调
                    if (loadedLevels == LevelAll - 1)
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
        // 加载并生成敌人、
        foreach (var Enemykey in levelData.WavesenEmiesDic.Keys)
        {
            for (int i = 0; i < levelData.WavesenEmiesDic[Enemykey].Count; i++)
            {
                string enemyName = GameFlowManager.Instance.GetSpwanPre(i);
                Addressables.LoadAssetAsync<GameObject>(enemyName).Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        enemyPrefabs.Add(handle.Result);
                        CheckAndInitializeLevel(levelData.WavesenEmiesDic[Enemykey].Count);
                    }
                };
            }

        }
        // 加载并生成子弹
        foreach (var Bulletkey in levelData.GunBulletDic.Keys)
        {
            string bulletName = Bulletkey;
            Addressables.LoadAssetAsync<GameObject>(bulletName).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    bulletPrefabs.Add(handle.Result);
                    CheckAndInitializeLevel(levelData.GunBulletDic.Count);
                }
            };
        }

    }
       

    private void CheckAndInitializeLevel(int currentWavePreNm)
    {
        // 当所有敌人和子弹都加载完毕时，初始化关卡
        if (enemyPrefabs.Count == currentWavePreNm && bulletPrefabs.Count == currentWavePreNm)
        {
            PreController.Instance.Init(enemyPrefabs, bulletPrefabs);
        }
    }


    //游戏地图
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
