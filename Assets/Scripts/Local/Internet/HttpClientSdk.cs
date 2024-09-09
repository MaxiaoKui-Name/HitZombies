using Cysharp.Threading.Tasks;
using LitJson;
using Session;
//using Spine;
using System;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class HttpClientSdk
{
    //�����ϴ�������
    //public static void UpdateData(Action<string> onSuccess, Action<string> onFailure, reqParams[] data)
    //{
    //    string baseUrl = WebSocketSessionManager.Instance.url;
    //    string roomId = WebSocketSessionManager.Instance.roomCode;
    //    string url = baseUrl + "/top/update/bacth/roomId";

    //    string jsonBody = JsonMapper.ToJson(data);

    //    UnityWebRequest www = new UnityWebRequest(url, "POST");
    //    byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonBody);
    //    www.uploadHandler = new UploadHandlerRaw(bodyData);
    //    www.uploadHandler.contentType = "application/json";
    //    www.downloadHandler = new DownloadHandlerBuffer();
    //    www.SetRequestHeader("Authorization", WebSocketSessionManager.Instance.Token);

    //    var operation = www.SendWebRequest();
    //    operation.completed += (op) =>
    //    {
    //        if (www.result == UnityWebRequest.Result.Success)
    //        {
    //            string responseText = www.downloadHandler.text;
    //            onSuccess?.Invoke(responseText);
    //        }
    //        else
    //        {
    //            string error = www.error;
    //            string responseText = www.downloadHandler.text;
    //            Debug.Log(error + "  " + responseText);
    //            onFailure?.Invoke(error);
    //        }
    //        www.Dispose();
    //    };
    //}

    //public static void BatchData(Action<string> onSuccess, Action<string> onFailure, string[] data, int indexRanking = 0)
    //{
    //    string baseUrl = WebSocketSessionManager.Instance.url;
    //    string roomId = WebSocketSessionManager.Instance.roomCode;
    //    string url = baseUrl + "/top/batch" + "?indexRanking=" + indexRanking + "&roomId=" + roomId;

    //    string jsonBody = JsonMapper.ToJson(data);
    //    UnityWebRequest www = new UnityWebRequest(url, "POST");
    //    byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonBody);
    //    www.uploadHandler = new UploadHandlerRaw(bodyData);//�ϴ�����
    //    www.uploadHandler.contentType = "application/json";
    //    www.downloadHandler = new DownloadHandlerBuffer();//��������
    //    www.SetRequestHeader("Authorization", WebSocketSessionManager.Instance.Token);
    //    var operation = www.SendWebRequest();
    //    operation.completed += (op) =>
    //    {
    //        if (www.result == UnityWebRequest.Result.Success)
    //        {
    //            var len = www.downloadedBytes;
    //            string responseText = www.downloadHandler.text;
    //            onSuccess?.Invoke(responseText);
    //        }
    //        else
    //        {
    //            string error = www.error;
    //            onFailure?.Invoke(error);
    //        }

    //        www.Dispose();
    //    };
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="num">��ȡ���ٸ�����</param>
    ///// <param name="skip">������������</param>
    ///// <param name="cancellationTokenSource"></param>
    ///// <param name="indexRanking">��ȡ��һ�����а�</param>
    ///// <returns></returns>
    //public static async UniTask<(bool, string)> GetTop(int num, int skip, CancellationTokenSource cancellationTokenSource, int indexRanking = 0)
    //{
    //    string baseUrl = WebSocketSessionManager.Instance.url;
    //    string url = baseUrl + "/top?limit=" + num + "&skip=" + skip + "&indexRanking=" + indexRanking;
    //    UnityWebRequest www = new UnityWebRequest(url, "GET");
    //    www.downloadHandler = new DownloadHandlerBuffer();
    //    www.SetRequestHeader("Authorization", WebSocketSessionManager.Instance.Token);
    //    await www.SendWebRequest().WithCancellation(cancellationTokenSource.Token);
    //    return (www.result == UnityWebRequest.Result.Success, www.downloadHandler.text);
    //}
    //�����ȡ���ݱ�
    //public static async UniTask<(int, string)> GetConfig(string url)
    //{
    //    UnityWebRequest www = new UnityWebRequest(url, "GET");
    //    www.downloadHandler = new DownloadHandlerBuffer();
    //    while (true)
    //    {
    //        await www.SendWebRequest();
    //        if (www.result == UnityWebRequest.Result.Success)
    //        {
    //            var json = JsonMapper.ToObject<JsonData>(www.downloadHandler.text);
    //            //TODO  ����̫��������ʱδ���³ɹ�  ����Message  ����̫����
    //            if ((int)json["status"] != 200)
    //            {
    //                Debug.Log($"Config�������ش���,{www.downloadHandler.text}");
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //        Debug.Log($"Config��������ʧ��,1����ٴγ�������");
    //        await UniTask.Delay(1000);
    //    }
    //    return (ErrorCode.ERR_Success, www.downloadHandler.text);
    //}
    public static async UniTask<(int, string)> GetConfig(string url)
    {
        try
        {
            // �ȴ� Addressables ���� "configdata"
            var handle = Addressables.LoadAssetAsync<TextAsset>(url);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // ���سɹ�����������
                var jsonData = handle.Result.text;
                return (0, jsonData); // 0 ��ʾ�ɹ�
            }
            else
            {
                // ����ʧ�ܣ����ش��������Ϣ
                return (1, $"Failed to load config data from {url}");
            }
        }
        catch (System.Exception ex)
        {
            // �����쳣�����ش�������쳣��Ϣ
            return (2, $"Exception occurred: {ex.Message}");
        }
    }

    //    //�����õ���־
    //    public static async UniTask<int> POSTLog(LogUnityLogEventHandle.PostLogInfos requset_BusinessRoundInfo)
    //    {
    //        //TODO URL����
    //        string url = "http://ap-chengdu.cls.tencentcs.com/tracklog?topic_id=ae85d494-18b5-4342-98d9-1fece68917c3";

    //        string jsonBody = JsonConvert.SerializeObject(requset_BusinessRoundInfo);

    //        UnityWebRequest www = new UnityWebRequest(url, "POST");
    //        byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonBody);
    //        www.uploadHandler = new UploadHandlerRaw(bodyData);
    //        www.uploadHandler.contentType = "application/json";
    //        www.downloadHandler = new DownloadHandlerBuffer();
    //        await www.SendWebRequest();
    //        if (www.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log($"�ϴ� POSTLog ����  {www.error}");
    //        }
    //        //LogKeyInfo(www, "�ϴ� POSTLog");
    //        //return status;
    //        return (int)www.result;
    //    }
    //    public static void InitBig(Action<string> onSuccess, Action<string> onFailure, InItBigBody[] data)
    //    {
    //        string baseUrl = WebSocketSessionManager.Instance.bigurl;
    //        string roomId = WebSocketSessionManager.Instance.roomCode;
    //        string url = baseUrl + "/v2/game/boss/initialize/batch";
    //        string jsonBody = JsonMapper.ToJson(data);
    //        UnityWebRequest www = new UnityWebRequest(url, "POST");
    //        byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonBody);
    //        www.uploadHandler = new UploadHandlerRaw(bodyData);
    //        www.uploadHandler.contentType = "application/json";
    //        www.downloadHandler = new DownloadHandlerBuffer();
    //        www.SetRequestHeader("Authorization", WebSocketSessionManager.Instance.Token);
    //        var operation = www.SendWebRequest();
    //        operation.completed += (op) =>
    //        {
    //            if (www.result == UnityWebRequest.Result.Success)
    //            {
    //                string responseText = www.downloadHandler.text;
    //                onSuccess?.Invoke(responseText);
    //            }
    //            else
    //            {
    //                string error = www.error;
    //                string responseText = www.downloadHandler.text;
    //                Debug.Log(error + "  " + responseText);
    //                onFailure?.Invoke(error);
    //            }

    //            www.Dispose();
    //        };
    //    }

    //    public static void UpdateBig(Action<string> onSuccess, Action<string> onFailure, BigBody[] data)
    //    {
    //        string baseUrl = WebSocketSessionManager.Instance.bigurl;
    //        string roomId = WebSocketSessionManager.Instance.roomCode;
    //        string url = baseUrl + "/v2/game/boss/defeat";
    //        string jsonBody = JsonMapper.ToJson(data);
    //        UnityWebRequest www = new UnityWebRequest(url, "POST");
    //        byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonBody);
    //        www.uploadHandler = new UploadHandlerRaw(bodyData);
    //        www.uploadHandler.contentType = "application/json";
    //        www.downloadHandler = new DownloadHandlerBuffer();
    //        www.SetRequestHeader("Authorization", WebSocketSessionManager.Instance.Token);
    //        var operation = www.SendWebRequest();
    //        operation.completed += (op) =>
    //        {
    //            if (www.result == UnityWebRequest.Result.Success)
    //            {
    //                string responseText = www.downloadHandler.text;
    //                onSuccess?.Invoke(responseText);
    //            }
    //            else
    //            {
    //                string error = www.error;
    //                string responseText = www.downloadHandler.text;
    //                Debug.Log(error + "  " + responseText);
    //                onFailure?.Invoke(error);
    //            }
    //            www.Dispose();
    //        };
    //    }
    //    public static void GetPlayerBig(Action<string> onSuccess, Action<string> onFailure, string playerID)
    //    {
    //        string baseUrl = WebSocketSessionManager.Instance.bigurl;
    //        string roomId = WebSocketSessionManager.Instance.roomCode;
    //        string url = "";
    //#if Develop||TEST
    //        url = baseUrl + "/find-one-by-open-id" + "?openId=" + playerID + "&roomId=" + roomId;
    //#elif Release
    //        url = baseUrl + "/danmubag/find-one-by-open-id" + "?openId=" + playerID + "&roomId=" + roomId;
    //#endif
    //        UnityWebRequest www = UnityWebRequest.Get(url);
    //        //www.uploadHandler.contentType = "application/json";
    //        www.SetRequestHeader("Authorization", WebSocketSessionManager.Instance.Token);
    //        var operation = www.SendWebRequest();
    //        operation.completed += (op) =>
    //        {
    //            if (www.result == UnityWebRequest.Result.Success)
    //            {
    //                string responseText = www.downloadHandler.text;
    //                onSuccess?.Invoke(responseText);
    //            }
    //            else
    //            {
    //                string error = www.error;
    //                onFailure?.Invoke(error);
    //            }

    //            www.Dispose();
    //        };
    //    }

}

public class reqParams
{
    public string indexRanking;
    public string mode;
    public string roomId;
    public string scoreMode;
    public UpdateDataJsonBody[] topRankings;
}
[Serializable]
public class UpdateDataJsonBody
{
    public string appId = "";
    public string avator = "";
    public string indexRanking = "";
    public string key = "";
    public string mode = "";
    public string nickname = "";
    public string roomId = "";
    public double point = 0;
    public int rank = 0;
    public double score = 0;
    public int times = 0;
}

public class InItBigBody
{
    public int bossID = 0;
    public int additionalWeight = 0;
}

public class BigBody
{
    public int bossId;
    public string roomCode;
    public UpdateBigDataBody[] users;
}
public class UpdateBigDataBody
{
    public string appId = "";
    public string avator = "";
    public string indexRanking = "";
    public string key = "";
    public string mode = "";
    public string nickname = "";
    public int point = 0;
    public int rank = 0;
    public string roomId = "";
    public int score = 0;
}

public class GetBigDataBody
{
    public int rid;
    public string number;
    public string expireTime;
}


