using UnityEngine;
using System;
using UnityEngine.Networking;

public class AccountManager : Singleton<AccountManager>
{
    // PlayerPrefs键名
    private const string AccountIDKey = "PlayerAccountID";
    private const string CreationDateKey = "PlayerCreationDate";

    // 当前账户信息
    public PlayerAccount currentAccount;

    //void Awake()
    //{
    //    // 确保AccountManager在场景切换时不被销毁
    //    DontDestroyOnLoad(gameObject);
    //    LoadOrCreateAccount();
    //}

    /// <summary>
    /// 加载现有账户或创建新账户
    /// </summary>
    public  void LoadOrCreateAccount()
    {
        // 检查PlayerPrefs中是否已有账户ID
        if (PlayerPrefs.HasKey(AccountIDKey))
        {
            string storedID = PlayerPrefs.GetString(AccountIDKey);
            string storedDate = PlayerPrefs.GetString(CreationDateKey);
            currentAccount = new PlayerAccount(storedID, storedDate);
            Debug.Log("加载现有账户:");
            Debug.Log("账户ID: " + currentAccount.accountID);
            Debug.Log("创建日期: " + currentAccount.creationDate);
        }
        else
        {
            // 如果没有现有账户，创建新账户
            CreateNewAccount();
        }
    }

    /// <summary>
    /// 创建新账户并保存到PlayerPrefs
    /// </summary>
    private void CreateNewAccount()
    {
        string newID = GenerateUniqueID();
        string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        currentAccount = new PlayerAccount(newID, creationDate);

        // 保存到PlayerPrefs
        PlayerPrefs.SetString(AccountIDKey, currentAccount.accountID);
        PlayerPrefs.SetString(CreationDateKey, currentAccount.creationDate);
        PlayerPrefs.Save();

        Debug.Log("创建新账户:");
        Debug.Log("账户ID: " + currentAccount.accountID);
        Debug.Log("创建日期: " + currentAccount.creationDate);
    }

    /// <summary>
    /// 生成唯一的账户ID
    /// </summary>
    /// <returns>唯一ID字符串</returns>
    private string GenerateUniqueID()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// 重置账户信息（用于测试或重置功能）
    /// </summary>
    public void ResetAccount()
    {
        PlayerPrefs.DeleteKey(AccountIDKey);
        PlayerPrefs.DeleteKey(CreationDateKey);
        PlayerPrefs.Save();
        Debug.Log("账户已重置。");
        LoadOrCreateAccount();
    }
}
//using UnityEngine;
//using UnityEngine.Networking;
//using System.Collections;

//public class PlayerAccountManager : MonoBehaviour
//{
//    private const string PlayerAccountKey = "PlayerAccount"; // 用于本地存储玩家账号的键
//    private string playerAccount; // 玩家账号

//    void Start()
//    {
//        // 检查本地是否已存储账号
//        if (PlayerPrefs.HasKey(PlayerAccountKey))
//        {
//            // 加载已存在的账号
//            playerAccount = PlayerPrefs.GetString(PlayerAccountKey);
//            Debug.Log("本地已存在账号: " + playerAccount);
//            StartCoroutine(SendPlayerAccountToServer(playerAccount)); // 向服务器发送账号
//        }
//        else
//        {
//            // 如果没有本地账号，向服务器请求分配新账号
//            StartCoroutine(RequestNewPlayerAccount());
//        }
//    }

//    // 请求服务器生成新玩家账号
//    IEnumerator RequestNewPlayerAccount()
//    {
//        string url = "http://yourserver.com/account_handler.php"; // 替换为你的服务器地址

//        UnityWebRequest www = UnityWebRequest.Post(url, "");
//        yield return www.SendWebRequest();

//        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
//        {
//            Debug.LogError("请求账号失败: " + www.error);
//        }
//        else
//        {
//            // 解析服务器返回的JSON数据
//            string jsonResponse = www.downloadHandler.text;
//            AccountResponse response = JsonUtility.FromJson<AccountResponse>(jsonResponse);

//            if (response.status == "success")
//            {
//                // 保存新生成的账号
//                playerAccount = response.player_id;
//                PlayerPrefs.SetString(PlayerAccountKey, playerAccount);
//                PlayerPrefs.Save(); // 保存账号到本地
//                Debug.Log("新账号已生成: " + playerAccount);
//            }
//            else
//            {
//                Debug.LogError("账号生成失败: " + response.message);
//            }
//        }
//    }

//    // 向服务器发送本地已有的玩家账号
//    IEnumerator SendPlayerAccountToServer(string playerId)
//    {
//        string url = "http://yourserver.com/account_handler.php"; // 替换为你的服务器地址

//        WWWForm form = new WWWForm();
//        form.AddField("player_id", playerId);

//        UnityWebRequest www = UnityWebRequest.Post(url, form);
//        yield return www.SendWebRequest();

//        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
//        {
//            Debug.LogError("账号检查失败: " + www.error);
//        }
//        else
//        {
//            // 解析服务器返回的响应
//            string jsonResponse = www.downloadHandler.text;
//            AccountResponse response = JsonUtility.FromJson<AccountResponse>(jsonResponse);

//            if (response.status == "success")
//            {
//                Debug.Log("服务器确认账号: " + response.player_id);
//            }
//            else
//            {
//                Debug.LogError("服务器未能确认账号: " + response.message);
//            }
//        }
//    }

//    // 用于解析服务器返回的JSON数据
//    [System.Serializable]
//    public class AccountResponse
//    {
//        public string status;
//        public string player_id;
//        public string message;
//    }
//}

