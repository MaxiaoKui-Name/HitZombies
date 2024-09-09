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
    //数据上传服务器
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
    //    www.uploadHandler = new UploadHandlerRaw(bodyData);//上传数据
    //    www.uploadHandler.contentType = "application/json";
    //    www.downloadHandler = new DownloadHandlerBuffer();//下载数据
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
    ///// <param name="num">获取多少个排名</param>
    ///// <param name="skip">跳过的排名数</param>
    ///// <param name="cancellationTokenSource"></param>
    ///// <param name="indexRanking">获取哪一个排行榜</param>
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
    //请求获取数据表
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
    //            //TODO  请求太快数据暂时未更新成功  返回Message  访问太快了
    //            if ((int)json["status"] != 200)
    //            {
    //                Debug.Log($"Config请求下载错误,{www.downloadHandler.text}");
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //        Debug.Log($"Config请求下载失败,1秒后再次尝试请求");
    //        await UniTask.Delay(1000);
    //    }
    //    return (ErrorCode.ERR_Success, www.downloadHandler.text);
    //}
    public static async UniTask<(int, string)> GetConfig(string url)
    {
        try
        {
            // 等待 Addressables 加载 "configdata"
            var handle = Addressables.LoadAssetAsync<TextAsset>(url);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 加载成功，返回内容
                var jsonData = handle.Result.text;
                return (0, jsonData); // 0 表示成功
            }
            else
            {
                // 加载失败，返回错误码和信息
                return (1, $"Failed to load config data from {url}");
            }
        }
        catch (System.Exception ex)
        {
            // 捕获异常，返回错误码和异常信息
            return (2, $"Exception occurred: {ex.Message}");
        }
    }

    //    //请求拿到日志
    //    public static async UniTask<int> POSTLog(LogUnityLogEventHandle.PostLogInfos requset_BusinessRoundInfo)
    //    {
    //        //TODO URL加密
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
    //            Debug.Log($"上传 POSTLog 错误  {www.error}");
    //        }
    //        //LogKeyInfo(www, "上传 POSTLog");
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


