using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
//�ȸ��½ű� // ����������ȸ����߼������¹ؿ������������½�ʬAI��
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
                Debug.Log("Background ͼƬ��Դ���سɹ�");
                callback?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"���� Background ͼƬ��Դʧ�ܣ���Դ·����{backgroundImageName}��\n�쳣 {backgroundHandle.OperationException}");
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
//        // ��ʽ���ã���ֹ���ü�
//        PreventStripping();
//        // ������Դ
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
//            Debug.Log("Background ͼƬ��Դ���سɹ�");
//            GameObject backgroundTexture = backgroundHandle.Result;

//            // ��ȡ���Ͳ����� Run ����������ͼƬ��Դ
//            if (backgroundTexture != null)
//            {
//                GameObject instantiatedBackground = Object.Instantiate(backgroundTexture);
//                instantiatedBackground.transform.position = Vector3.zero;
//            }
//        }
//        else
//        {
//            Debug.LogError($"���� Background ͼƬ��Դʧ�ܣ���Դ·����{backgroundImageName}��\n�쳣 {backgroundHandle.OperationException}");
//        }
//    }

//    void PreventStripping()
//    {
//        // ��ʽ���ã��Է�ֹ���ü�
//        GameObject.Find("Canvas/back");
//    }



//}

