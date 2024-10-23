using UnityEngine;
using System;
using UnityEngine.Networking;

public class AccountManager : Singleton<AccountManager>
{
    private const string AccountIDKey = "PlayerAccountID";
    private const string CreationDateKey = "PlayerCreationDate";
    private const string LastSignInDateKeyPrefix = "_LastSignInDate";
    private const string ConsecutiveDaysKeyPrefix = "_ConsecutiveDays";
    private const string TotalCoinsKeyPrefix = "_TotalCoins";
    private const string PlayerCoinNumKey = "PlayerCoinNum";
    private const string PlayerlevelKey = "Playerlevel";
    private const string PlayerexperiencesKey = "Playerexperiences";
    private const string PlayerFrozenBuffCountKey = "PlayerFrozenBuffCount";
    private const string PlayerBalstBuffCountKey = "PlayerBalstBuffCount";
    private const string PlayerBulletNameKey = "PlayerBulletName";

    //void Awake()
    //{
    //    // 确保AccountManager在场景切换时不被销毁
    //    DontDestroyOnLoad(gameObject);
    //    LoadOrCreateAccount();
    //}

    /// <summary>
    /// 加载现有账户或创建新账户
    /// </summary>
    /// <summary>
    /// 加载现有账户或创建新账户
    /// </summary>
    public void LoadOrCreateAccount()
    {
        if (PlayerPrefs.HasKey(AccountIDKey))
        {
            string accountID = PlayerPrefs.GetString(AccountIDKey);
            string creationDate = PlayerPrefs.GetString(CreationDateKey);
            string lastSignInDateStr = PlayerPrefs.GetString($"{accountID}{LastSignInDateKeyPrefix}");
            int consecutiveDays = PlayerPrefs.GetInt($"{accountID}{ConsecutiveDaysKeyPrefix}", 0);
            int totalCoins = PlayerPrefs.GetInt($"{accountID}{TotalCoinsKeyPrefix}", 0);
            long coinNum;
            bool parseSuccess = long.TryParse(PlayerPrefs.GetString($"{accountID}{PlayerCoinNumKey}"), out coinNum);
            Debug.Log($"加载账户ID: {accountID}");
            Debug.Log($"加载 coinNum: {coinNum}, 解析成功: {parseSuccess}");
            string bulletName = PlayerPrefs.GetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBulletNameKey}");
            //long.TryParse(PlayerPrefs.GetString($"{accountID}{PlayerCoinNumKey}"), out coinNum);
            int Playerlevel = PlayerPrefs.GetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerlevelKey}", PlayInforManager.Instance.playInfor.level);
            long experiences;
            long.TryParse(PlayerPrefs.GetString($"{accountID}{PlayerexperiencesKey}"), out experiences);
            int PlayerFrozenBuffCount = PlayerPrefs.GetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerFrozenBuffCountKey}", PlayInforManager.Instance.playInfor.FrozenBuffCount);
            int PlayerBalstBuffCount = PlayerPrefs.GetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBalstBuffCountKey}", PlayInforManager.Instance.playInfor.BalstBuffCount);
            DateTime lastSignInDate;
            DateTime.TryParse(lastSignInDateStr, out lastSignInDate);
            PlayInforManager.Instance.playInfor.SetPlayerAccount(accountID, creationDate, lastSignInDate, consecutiveDays, coinNum, Playerlevel, experiences, PlayerFrozenBuffCount,PlayerBalstBuffCount);
            PlayInforManager.Instance.playInfor.currentGun.bulletType = bulletName;
        }
        else
        {
            CreateNewAccount();
        }
    }

    /// <summary>
    /// 创建新账户并保存到PlayerPrefs
    /// </summary>
    private void CreateNewAccount()
    {
        PlayInforManager.Instance.playInfor.SetGun(LevelManager.Instance.levelData.GunBulletList[2]);
        string newID = GenerateUniqueID();
        string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //TTOD1初始血量待更改
        PlayInforManager.Instance.playInfor.SetPlayerAccount(newID, creationDate, DateTime.MinValue, 0, 40000, ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Lv, ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Exp, 0, 0);
        // 保存到PlayerPrefs
        PlayerPrefs.SetString(AccountIDKey, PlayInforManager.Instance.playInfor.accountID);
        PlayerPrefs.SetString(CreationDateKey, PlayInforManager.Instance.playInfor.creationDate);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{LastSignInDateKeyPrefix}", PlayInforManager.Instance.playInfor.lastSignInDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{ConsecutiveDaysKeyPrefix}", PlayInforManager.Instance.playInfor.consecutiveDays);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{TotalCoinsKeyPrefix}", PlayInforManager.Instance.playInfor.totalCoins);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerCoinNumKey}", PlayInforManager.Instance.playInfor.coinNum.ToString());
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerlevelKey}", PlayInforManager.Instance.playInfor.level);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerexperiencesKey}", PlayInforManager.Instance.playInfor.experiences.ToString());
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerFrozenBuffCountKey}", PlayInforManager.Instance.playInfor.FrozenBuffCount);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBalstBuffCountKey}", PlayInforManager.Instance.playInfor.BalstBuffCount);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBulletNameKey}", PlayInforManager.Instance.playInfor.currentGun.bulletType);
        PlayerPrefs.Save();
        Debug.Log("新账户已创建:");
        Debug.Log("账户ID: " + PlayInforManager.Instance.playInfor.accountID);
        Debug.Log("创建日期: " + PlayInforManager.Instance.playInfor.creationDate);
        Debug.Log($"coinNum: {PlayInforManager.Instance.playInfor.coinNum}");
    }
/// <summary>
/// 生成唯一的账户ID
/// </summary>
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
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{LastSignInDateKeyPrefix}");
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{ConsecutiveDaysKeyPrefix}");
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{TotalCoinsKeyPrefix}");
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{PlayerCoinNumKey}");
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{PlayerlevelKey}");
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{PlayerexperiencesKey}");
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{PlayerFrozenBuffCountKey}");
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBalstBuffCountKey}");
        PlayerPrefs.DeleteKey($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBulletNameKey}");
        PlayerPrefs.Save();
        Debug.Log("账户已重置。");
    }

    /// <summary>
    /// 处理每日签到
    /// </summary>
    public bool SignIn(out int reward)
    {
        DateTime today = DateTime.Today;
        if (PlayInforManager.Instance.playInfor.lastSignInDate == today)
        {
            // 今日已签到
            reward = 0;
            return false;
        }
        // 检查上次签到是否为昨天
        if (PlayInforManager.Instance.playInfor.lastSignInDate == today.AddDays(-1))
        {
            PlayInforManager.Instance.playInfor.consecutiveDays += 1;
        }
        else
        {
            PlayInforManager.Instance.playInfor.consecutiveDays = 1; // 如果不连续则重置
        }
        PlayInforManager.Instance.playInfor.lastSignInDate = today;

        // 确定奖励
        reward = GetDailyReward(PlayInforManager.Instance.playInfor.consecutiveDays);
        PlayInforManager.Instance.playInfor.AddCoins((int)(reward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(PlayInforManager.Instance.playInfor.level).Total));
        // 保存更新后的数据
        SaveAccountData();
        Debug.Log($"已于 {today} 签到。奖励: {reward} 金币。连续签到天数: {PlayInforManager.Instance.playInfor.consecutiveDays}");
        return true;
    }

    /// <summary>
    /// 根据连续签到天数获取奖励
    /// </summary>
    private int GetDailyReward(int consecutiveDays)
    {
        int multiple = 0; 
        // TTOD1奖励后面改读表
        switch (consecutiveDays)
        {
            case 1:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(1).Money;
                return multiple; // 第1天
            case 2:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(2).Money;
                return multiple; // 第2天
            case 3:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(3).Money;
                return multiple;// 第3天
            case 4:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(4).Money;
                return multiple; // 第4天
            case 5:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(5).Money;
                return multiple; // 第5天（额外奖励）
            default:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(5).Money;
                return multiple; // 超过5天后重置为第1天奖励
        }
    }

    /// <summary>
    /// 保存账户数据到PlayerPrefs
    /// </summary>
    private void SaveAccountData()
    {
        PlayerPrefs.SetString(AccountIDKey, PlayInforManager.Instance.playInfor.accountID);
        PlayerPrefs.SetString(CreationDateKey, PlayInforManager.Instance.playInfor.creationDate);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{LastSignInDateKeyPrefix}", PlayInforManager.Instance.playInfor.lastSignInDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{ConsecutiveDaysKeyPrefix}", PlayInforManager.Instance.playInfor.consecutiveDays);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{TotalCoinsKeyPrefix}", PlayInforManager.Instance.playInfor.totalCoins);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerCoinNumKey}", PlayInforManager.Instance.playInfor.coinNum.ToString());
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerlevelKey}", PlayInforManager.Instance.playInfor.level);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerexperiencesKey}", PlayInforManager.Instance.playInfor.experiences.ToString());
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerFrozenBuffCountKey}", PlayInforManager.Instance.playInfor.FrozenBuffCount);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBalstBuffCountKey}", PlayInforManager.Instance.playInfor.BalstBuffCount);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 获取总金币数量
    /// </summary>
    public int GetTotalCoins()
    {
        return PlayInforManager.Instance.playInfor.totalCoins;
    }
    public bool CanSignIn()
    {
        DateTime today = DateTime.Today;
        return PlayInforManager.Instance.playInfor.lastSignInDate.Date != today;
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

