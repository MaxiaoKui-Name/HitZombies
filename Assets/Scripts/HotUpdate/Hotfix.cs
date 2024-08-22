using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
//热更新脚本 // 在这里添加热更新逻辑，如新关卡、新武器、新僵尸AI等
public class Hotfix : MonoBehaviour
{
    public void Run()
    {
        Load(HandleLoadComplete);
    }

    public void Load(System.Action<GameObject> callback)
    {
        Debug.Log("Hello, 1111111");

        string backgroundImageName = "cc";
        AsyncOperationHandle<GameObject> backgroundHandle = Addressables.LoadAssetAsync<GameObject>(backgroundImageName);

        backgroundHandle.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Background 图片资源加载成功");
                callback?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"加载 Background 图片资源失败，资源路径：{backgroundImageName}，\n异常 {backgroundHandle.OperationException}");
                callback?.Invoke(null);
            }
        };
    }

    private void HandleLoadComplete(GameObject backgroundTexture)
    {
        if (backgroundTexture != null)
        {
            GameObject instantiatedBackground = Instantiate(backgroundTexture);
            instantiatedBackground.transform.position = Vector3.zero;
        }
    }
}

//public class Hello
//{
//    public IEnumerable<object> AOTMetaAssemblyFiles { get; private set; }

//    public async void Run()
//    {
//        // 显式调用，防止被裁剪
//        PreventStripping();
//        // 加载资源
//        Load();
//    }

//    public async Task Load()
//    {
//        Debug.Log("Hello, 1111111");

//        string backgroundImageName = "Background";
//        AsyncOperationHandle<GameObject> backgroundHandle = Addressables.LoadAssetAsync<GameObject>(backgroundImageName);
//        await backgroundHandle.Task;

//        if (backgroundHandle.Status == AsyncOperationStatus.Succeeded)
//        {
//            Debug.Log("Background 图片资源加载成功");
//            GameObject backgroundTexture = backgroundHandle.Result;

//            // 获取类型并调用 Run 方法，传递图片资源
//            if (backgroundTexture != null)
//            {
//                GameObject instantiatedBackground = Object.Instantiate(backgroundTexture);
//                instantiatedBackground.transform.position = Vector3.zero;
//            }
//        }
//        else
//        {
//            Debug.LogError($"加载 Background 图片资源失败，资源路径：{backgroundImageName}，\n异常 {backgroundHandle.OperationException}");
//        }
//    }

//    void PreventStripping()
//    {
//        // 显式调用，以防止被裁剪
//        GameObject.Find("Canvas/back");
//    }



//}

