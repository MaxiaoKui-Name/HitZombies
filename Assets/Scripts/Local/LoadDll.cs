using Cysharp.Threading.Tasks;
using DragonBones;
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

    // ��ʼ������Զ�������ļ�
    public async UniTask InitAddressable()
    {
        initScenePanelController = GameObject.Find("UICanvas/InitScenePanel(Clone)").GetComponent<InitScenePanelController>();
        var initAddress = Addressables.InitializeAsync(false);
        await initAddress.Task;
        if (initAddress.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError("��ʼ��ʧ��");
            StartGame();
            return;
        }
        CheckUpdateAsset();
        Addressables.Release(initAddress);
    }

    private Assembly _hotUpdateAss;

    public IEnumerable<object> AOTMetaAssemblyFiles { get; private set; }

    // ���ظ��³���
    async void StartGame()
    {
#if UNITY_EDITOR
        string hotUpdateName = "HotUpdate";
        _hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == hotUpdateName);
#else
        string hotUpdateName = "HotUpdate.dll";
        Debug.Log($"�첽������ԴKey·����{hotUpdateName}");
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(hotUpdateName);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"�첽������Դʧ�ܣ���ԴKey·����{hotUpdateName}��\n�쳣 {handle.OperationException}");
            return; // ��ֹ����������������ò����ڵĳ���
        }

        Debug.Log($"�첽������Դ��С��{handle.Result.dataSize}");
        _hotUpdateAss = Assembly.Load(handle.Result.bytes);
#endif

        if (_hotUpdateAss == null)
        {
            Debug.LogError("�޷����� HotUpdate ���򼯣�����ʧ�ܡ�");
            return;
        }

        await Task.Yield();

    }

    //private void LoadMetadataForAOTAssemblies()
    //{
    //    HomologousImageMode mode = HomologousImageMode.SuperSet;
    //    foreach (var aotDllName in AOTMetaAssemblyFiles)
    //    {
    //        byte[] dllBytes = ReadBytesFromStreamingAssets((string)aotDllName);
    //        LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
    //        Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
    //    }
    //}

    //    private byte[] ReadBytesFromStreamingAssets(string abName)
    //    {
    //        Debug.Log($"ReadAllBytes name: {abName}");
    //#if UNITY_ANDROID
    //        // Androidƽ̨��ȡassets�ļ��Ĵ���...
    //#else
    //        return File.ReadAllBytes(Application.streamingAssetsPath + "/" + abName);
    //#endif
    //    }

    List<long> downLoadSizeList;
    List<string> downLoadKeyList;

    private async void CheckUpdateAsset()
    {

        var checkCatLogUpdate = Addressables.CheckForCatalogUpdates(false);
        await checkCatLogUpdate.Task;
        if (checkCatLogUpdate.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("������ʧ��");

        }
        downLoadKeyList = checkCatLogUpdate.Result;
        if (downLoadKeyList.Count <= 0)
        {
            Debug.Log("�޿ɸ������ݣ�ֱ�ӽ�����Ϸ...");
            //initScenePanelController.UpdateText("�޿ɸ������ݣ�ֱ�ӽ�����Ϸ...");
            // ����Э���𽥽��������ӵ�ǰ���ȼ�����100%
            initScenePanelController.LoadAnim_F.animation.Play("loading", -1);  // ���趯����Ϊ "click"
            await SmoothProgressBar(1f, 1f);  // Ŀ�����Ϊ100%������ʱ��1��
            StartGame();
            successfullyLoaded = true;
            return;
        }
        else
        {
            Debug.Log($"��{downLoadKeyList.Count}����Դ��Ҫ����");
            CheckUpdateAssetSize();
        }
        Addressables.Release(checkCatLogUpdate);
    }

    long totalSize = 0;
    List<IResourceLocator> resourceLocators;

    private async void CheckUpdateAssetSize()
    {
        Debug.Log("����У�������Դ��С...");
        var updateCatLog = Addressables.UpdateCatalogs(true, downLoadKeyList, false);
        await updateCatLog.Task;
        if (updateCatLog.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("������Դ�б�ʧ��");
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
        Debug.Log($"��ȡ�������ش�С��{totalSize / 1048579f} M");
        Addressables.Release(operationHandle);

        if (totalSize <= 0)
        {
            Debug.Log("�޿ɸ�������");
            StartGame();
            successfullyLoaded = true;
            return;
        }
        successfullyLoaded = false;
        Debug.Log($"��{downLoadKeyList.Count}����Դ��Ҫ����");
        progressIndex = 0;
        DownloadAsset();
    }

    public int progressIndex = 0;
    public float curProgressSize;
    public bool successfullyLoaded = false;

    // �� DownloadAsset ������ʵʱ���½�����
    private async void DownloadAsset()
    {
        Debug.Log("���ڸ�����Դ...");
        for (int i = progressIndex; i < resourceLocators.Count; i++)
        {
            var item = resourceLocators[i];
            AsyncOperationHandle asyncOperationHandle = Addressables.DownloadDependenciesAsync(item.Keys);

            // ѭ��ֱ����Դ�������
            while (!asyncOperationHandle.IsDone)
            {
                float progress = asyncOperationHandle.PercentComplete;

                // ���㵱ǰ���ؽ���
                curProgressSize = downLoadSizeList.Take(i).Sum() + downLoadSizeList[i] * progress;

                // ���½�����
                initScenePanelController.UpdateProgressBar(curProgressSize / totalSize);

                // ��ӡ��־���ڵ���
                Debug.Log($"{item} ;progress��{progress}; downLoadSizeList:{downLoadSizeList[i]}...");
                Debug.Log(curProgressSize / (totalSize * 1.0f) + "���ڸ�����Դ...");
                await Task.Yield(); // �ȴ���һ֡����ִ��
            }

            // �ж������Ƿ�ɹ�
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                successfullyLoaded = true;
                Debug.Log($"���سɹ���{item}...");
            }
            else
            {
                Debug.LogError($"����ʧ�ܣ�{item}����ʾ���԰�ť�����ص���{progressIndex}����Դ...");
                progressIndex = i;
                break;
            }
        }

        Debug.Log("�������");
        StartGame(); // ������ɺ�������Ϸ
    }
    // Э�̣�ƽ�����½�����
    private async UniTask SmoothProgressBar(float targetProgress, float duration)
    {
        float startProgress = 0; // ��ȡ��ǰ����
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = Mathf.Lerp(startProgress, targetProgress, elapsedTime / duration);
            initScenePanelController.UpdateProgressBar(progress); // ���½�����
            elapsedTime += Time.deltaTime; // ����ʱ��
            await UniTask.Yield(); // �ȴ���һ֡
        }
        // ȷ������������ֵΪ100%
        initScenePanelController.UpdateProgressBar(targetProgress);
    }
}
