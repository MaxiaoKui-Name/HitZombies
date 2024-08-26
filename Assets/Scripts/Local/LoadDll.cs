using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class LoadDll : Singleton<LoadDll>
{
    public InitScenePanelController initScenePanelController;

    private void Awake()
    {
        XLog.Init();
    }

    void Start()
    {

        
    }

    // 初始化加载远端配置文件
    public async void InitAddressable()
    {
        initScenePanelController = GameObject.Find("UICanvas/InitScenePanel(Clone)").GetComponent<InitScenePanelController>();
        var initAddress = Addressables.InitializeAsync(false);
        await initAddress.Task;
        if (initAddress.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError("初始化失败");
            StartGame();
            return;
        }

        CheckUpdateAsset();
        Addressables.Release(initAddress);
    }

    private Assembly _hotUpdateAss;

    public IEnumerable<object> AOTMetaAssemblyFiles { get; private set; }

    // 加载更新程序集
    async void StartGame()
    {
#if UNITY_EDITOR
        string hotUpdateName = "HotUpdate";
        _hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == hotUpdateName);
#else
        string hotUpdateName = "HotUpdate.dll";
        Debug.Log($"异步加载资源Key路径：{hotUpdateName}");
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(hotUpdateName);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"异步加载资源失败，资源Key路径：{hotUpdateName}，\n异常 {handle.OperationException}");
            return; // 终止操作，避免继续调用不存在的程序集
        }

        Debug.Log($"异步加载资源大小：{handle.Result.dataSize}");
        _hotUpdateAss = Assembly.Load(handle.Result.bytes);
#endif

        if (_hotUpdateAss == null)
        {
            Debug.LogError("无法加载 HotUpdate 程序集，启动失败。");
            return;
        }

        await Task.Yield();

    }

    private void LoadMetadataForAOTAssemblies()
    {
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            byte[] dllBytes = ReadBytesFromStreamingAssets((string)aotDllName);
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

    private byte[] ReadBytesFromStreamingAssets(string abName)
    {
        Debug.Log($"ReadAllBytes name: {abName}");
#if UNITY_ANDROID
        // Android平台读取assets文件的代码...
#else
        return File.ReadAllBytes(Application.streamingAssetsPath + "/" + abName);
#endif
    }

    List<long> downLoadSizeList;
    List<string> downLoadKeyList;

    private async void CheckUpdateAsset()
    {
        var checkCatLogUpdate = Addressables.CheckForCatalogUpdates(false);
        await checkCatLogUpdate.Task;
        if (checkCatLogUpdate.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("检测更新失败");

        }
        downLoadKeyList = checkCatLogUpdate.Result;
        if (downLoadKeyList.Count <= 0)
        {
            Debug.Log("无可更新内容，直接进入游戏...");
            initScenePanelController.UpdateText("无可更新内容，直接进入游戏...");
            StartGame();
            successfullyLoaded = true;
            return;
        }
        else
        {
            Debug.Log($"有{downLoadKeyList.Count}个资源需要更新");
            CheckUpdateAssetSize();
        }
        Addressables.Release(checkCatLogUpdate);
    }

    long totalSize = 0;
    List<IResourceLocator> resourceLocators;

    private async void CheckUpdateAssetSize()
    {
        Debug.Log("正在校验更新资源大小...");
        var updateCatLog = Addressables.UpdateCatalogs(true, downLoadKeyList, false);
        await updateCatLog.Task;
        if (updateCatLog.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("更新资源列表失败");
            return;
        }
        resourceLocators = updateCatLog.Result;
        Addressables.Release(updateCatLog);

        AsyncOperationHandle<long> operationHandle = default;
        foreach (var item in resourceLocators)
        {
            operationHandle = Addressables.GetDownloadSizeAsync(item.Keys);
            await operationHandle.Task;
            downLoadSizeList.Add(operationHandle.Result);
            totalSize += operationHandle.Result;
        }
        Debug.Log($"获取到的下载大小：{totalSize / 1048579f} M");
        Addressables.Release(operationHandle);

        if (totalSize <= 0)
        {
            Debug.Log("无可更新内容");
            StartGame();
            successfullyLoaded = true;
            return;
        }
        successfullyLoaded = false;
        Debug.Log($"有{downLoadKeyList.Count}个资源需要更新");
        progressIndex = 0;
        DownloadAsset();
    }

    public int progressIndex = 0;
    public float curProgressSize;
    public bool successfullyLoaded = false;

    private async void DownloadAsset()
    {
        Debug.Log("正在更新资源...");
        for (int i = progressIndex; i < resourceLocators.Count; i++)
        {
            var item = resourceLocators[i];
            AsyncOperationHandle asyncOperationHandle = Addressables.DownloadDependenciesAsync(item.Keys);

            while (!asyncOperationHandle.IsDone)
            {
                float progress = asyncOperationHandle.PercentComplete;
                curProgressSize = downLoadSizeList.Take(i).Sum() + downLoadSizeList[i] * progress;
                initScenePanelController.UpdateProgressBar(curProgressSize / totalSize);
                Debug.Log($"{item} ;progress：{progress}; downLoadSizeList:{downLoadSizeList[i]}...");
                Debug.Log(curProgressSize / (totalSize * 1.0f) + "正在更新资源...");
                await Task.Yield();
            }

            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                 successfullyLoaded = true;
                 Debug.Log($"下载成功：{item}...");
            }
            else
            {
                Debug.LogError($"下载失败：{item}，显示重试按钮，下载到第{progressIndex}个资源...");
                progressIndex = i;
                break;
            }
        }

        Debug.Log("下载完成");
        StartGame();
    }

   
}
