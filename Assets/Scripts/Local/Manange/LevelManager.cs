using Cysharp.Threading.Tasks;
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
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<GameObject> bulletPrefabs = new List<GameObject>();
    public List<GameObject> CoinPrefabs = new List<GameObject>();
    public int LevelAll = 1;


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
    private async UniTask OnLevelLoaded(AsyncOperationHandle<SceneInstance> obj, int levelIndex)
    {
        GameManage.Instance.isGetdoor = true;
        await LoadLevelAssets(levelIndex);
        CheckAndInitializeLevel();
    }
    //Get�ؿ�����
    public async UniTask Load(int levelIndex, System.Action onLoadComplete)
    {
        isLoadBack = false;
        int loadedLevels = 0; // ����������ɵĹؿ���
        for (int i = 0; i < LevelAll; i++)
        {
            int index = i;
            // �ж��ٸ��ؿ����ж��ٸ� LevelData
           Addressables.LoadAssetAsync<LevelData>("LevelData" + index).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameFlowManager.Instance.levels[index] = handle.Result as LevelData;
                    // ������йؿ���������ɣ������ص�
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


    //public async UniTask LoadLevelAssets(int levelIndex)
    //{

    //    int totalTasks = levelData.WavesenEmiesDic.Keys.Count;
    //    int completedTasks = 0;

    //    // ���ز����ɵ���
    //    foreach (var keyName in levelData.WavesenEmiesDic.Keys)
    //    {
    //        List<List<int>> enemyTypes = levelData.WavesenEmiesDic[keyName];
    //        for (int q = 0; q < enemyTypes.Count; q++)
    //        {
    //            List<int> enemyList = enemyTypes[q];
    //            for (int j = 0; j < enemyList.Count; j++)
    //            {
    //                if (enemyList[j] != 0)
    //                {
    //                    string enemyName = GameFlowManager.Instance.GetSpwanPre(enemyList[j]);
    //                    if(enemyName != null)
    //                    {
    //                        Addressables.LoadAssetAsync<GameObject>(enemyName).Completed += handle =>
    //                        {
    //                            if (handle.Status == AsyncOperationStatus.Succeeded)
    //                            {
    //                                if (!enemyPrefabs.Contains(handle.Result))
    //                                {
    //                                    enemyPrefabs.Add(handle.Result);
    //                                }
    //                            }
    //                        };
    //                    }

    //                }
    //            }
    //        }
    //    }
    //    // ���ز������ӵ�
    //    foreach (var Bulletkey in levelData.GunBulletList)
    //    {
    //        string bulletName = Bulletkey;
    //        Addressables.LoadAssetAsync<GameObject>(bulletName).Completed += handle =>
    //        {
    //            if (handle.Status == AsyncOperationStatus.Succeeded)
    //            {
    //                bulletPrefabs.Add(handle.Result);
    //                completedTasks++;
    //            }
    //        };
    //    }

    //}

    public async UniTask LoadLevelAssets(int levelIndex)
    {
        enemyPrefabs.Clear();
        bulletPrefabs.Clear();
        CoinPrefabs.Clear();
        levelData.ChestList.Clear();
        levelData.ChestUIList.Clear();  
        List<UniTask> loadTasks = new List<UniTask>();
        var loadTask1 = Addressables.LoadAssetAsync<GameObject>("buffdoorup");
        loadTasks.Add(loadTask1.Task.AsUniTask().ContinueWith(handle =>
        {
            if (loadTask1.Status == AsyncOperationStatus.Succeeded && loadTask1.Result != null)
            {
                levelData.PowbuffDoor = loadTask1.Result;
            }
            else
            {
                Debug.LogWarning($"Failed to load PowbuffDoor: ");
            }
        }));
        var loadTask2 = Addressables.LoadAssetAsync<GameObject>("BuffDoor");
        loadTasks.Add(loadTask2.Task.AsUniTask().ContinueWith(handle =>
        {
            if (loadTask2.Status == AsyncOperationStatus.Succeeded && loadTask2.Result != null)
            {
                levelData.buffDoor = loadTask2.Result;
            }
            else
            {
                Debug.LogWarning($"Failed to load PowbuffDoor: ");
            }
        }));

        // ���ز����ɵ���
        foreach (var keyName in levelData.WavesenEmiesDic.Keys)
        {
            List<List<int>> enemyTypes = levelData.WavesenEmiesDic[keyName];//�洢���й���ID�ļ�ֵ
            for (int q = 0; q < enemyTypes.Count; q++)
            {
                List<int> enemyList = enemyTypes[q];//ÿ����4��Monster��List
                for (int j = 0; j < enemyList.Count; j++)
                {
                    if (enemyList[j] == 0) continue;//����Id
                    string enemyName = GameFlowManager.Instance.GetSpwanPre(enemyList[j]);
                    if (enemyName != null)
                    {
                        var loadTask = Addressables.LoadAssetAsync<GameObject>(enemyName);
                        loadTasks.Add(loadTask.Task.AsUniTask().ContinueWith(handle =>
                        {
                            if (loadTask.Status == AsyncOperationStatus.Succeeded && loadTask.Result != null)
                            {
                                if (!enemyPrefabs.Contains(loadTask.Result))
                                {
                                    enemyPrefabs.Add(loadTask.Result);
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to load enemy prefab: {enemyName}");
                            }
                        }));
                    }
                }
            }
        }

        // ���ز������ӵ�
        foreach (var Bulletkey in levelData.GunBulletList)
        {
            string bulletName = Bulletkey.bulletType;
            var loadTask = Addressables.LoadAssetAsync<GameObject>(bulletName);
            loadTasks.Add(loadTask.Task.AsUniTask().ContinueWith(handle =>
            {
                if (loadTask.Status == AsyncOperationStatus.Succeeded && loadTask.Result != null)
                {
                    bulletPrefabs.Add(loadTask.Result);
                }
                else
                {
                    Debug.LogWarning($"Failed to load bullet prefab: {bulletName}");
                }
            }));
        }
        foreach (var Coinkey in levelData.CoinList)
        {
            string CoinName = Coinkey;
            var loadTask = Addressables.LoadAssetAsync<GameObject>(CoinName);
            loadTasks.Add(loadTask.Task.AsUniTask().ContinueWith(handle =>
            {
                if (loadTask.Status == AsyncOperationStatus.Succeeded && loadTask.Result != null)
                {
                    CoinPrefabs.Add(loadTask.Result);
                }
                else
                {
                    Debug.LogWarning($"Failed to load bullet prefab: {CoinName}");
                }
            }));
        }
        for(int k = 0; k < 1; k++)
        {
            string ChestName = GameManage.Instance.GetChest(k);
            var loadTask = Addressables.LoadAssetAsync<GameObject>(ChestName);
            loadTasks.Add(loadTask.Task.AsUniTask().ContinueWith(handle =>
            {
                if (loadTask.Status == AsyncOperationStatus.Succeeded && loadTask.Result != null)
                {
                    levelData.ChestList.Add(loadTask.Result);
                }
                else
                {
                    Debug.LogWarning($"Failed to load bullet prefab: {ChestName}");
                }
            }));
        }
        for (int k = 0; k < 5; k++)
        {
            string ChestName = GameManage.Instance.GetChest(k) + "UI"; ;
            var loadTask = Addressables.LoadAssetAsync<GameObject>(ChestName);
            loadTasks.Add(loadTask.Task.AsUniTask().ContinueWith(handle =>
            {
                if (loadTask.Status == AsyncOperationStatus.Succeeded && loadTask.Result != null)
                {
                    levelData.ChestUIList.Add(loadTask.Result);
                }
                else
                {
                    Debug.LogWarning($"Failed to load bullet prefab: {ChestName}");
                }
            }));
        }
        // �ȴ�������Դ�������
        await UniTask.WhenAll(loadTasks);
    }


    private async UniTask CheckAndInitializeLevel()
    {
        await PreController.Instance.Init(enemyPrefabs, bulletPrefabs, CoinPrefabs);
    }


    //��Ϸ��ͼ
    public void OnBackgroundLoaded(AsyncOperationHandle<Sprite> handle,int index)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite background = handle.Result;
            //BackgroundScroller scroller = FindObjectOfType<BackgroundScroller>();
            //if (scroller != null)
            //{
            InfiniteScroll.Instance.SetBackground(background, index);
           // }
        }
    }
}
