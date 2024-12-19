using UnityEngine;
using System;
using UnityEngine.Networking;
using Unity.VisualScripting;
using UnityEditor;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using static ReadyPanelController;
using System.Collections.Generic;

public class AccountManager : Singleton<AccountManager>
{
    public bool isGuid;
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
    private const string PlayerBulletGunKey = "PlayerGunName";
    // 新增：用于管理免费转盘的键值
    private const string LastSpinDateKeyPrefix = "_LastSpinDate";
    private const string FirstRageSkill = "_RageSkill";
    private const string FirstReplaceGun = "_ReplaceGun";
    private const string FirstZeroToOne = "_ZeroToOne";
    private const string WeaponKey = "_WeaponKey";

    private const string HasCompletedGunSelectionTutorialKey = "HasCompletedGunSelectionTutorial";



    // 添加一个用于保存宝箱数据的键名
    private const string ChestDataKey = "ChestData";
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
    public async UniTask LoadOrCreateAccount()
    {
        if (PlayerPrefs.HasKey(AccountIDKey))
        {
            bool hasCompletedTutorial = HasCompletedGunSelectionTutorial();
            // 传递新手引导状态给 PlayInforManager 或其他需要的地方
            PlayInforManager.Instance.playInfor.hasCompletedGunSelectionTutorial = hasCompletedTutorial;

            string accountID = PlayerPrefs.GetString(AccountIDKey);
            PlayerPrefs.SetInt($"{accountID}{PlayerlevelKey}", 1);
            PlayerPrefs.Save();
            string creationDate = PlayerPrefs.GetString(CreationDateKey);
            string lastSignInDateStr = PlayerPrefs.GetString($"{accountID}{LastSignInDateKeyPrefix}");
            string lastSpinDateStr = PlayerPrefs.GetString($"{accountID}{LastSpinDateKeyPrefix}");
            int consecutiveDays = PlayerPrefs.GetInt($"{accountID}{ConsecutiveDaysKeyPrefix}");
            int totalCoins = PlayerPrefs.GetInt($"{accountID}{TotalCoinsKeyPrefix}");
            long coinNum;
            bool parseSuccess = long.TryParse(PlayerPrefs.GetString($"{accountID}{PlayerCoinNumKey}"), out coinNum);
            Debug.Log($"加载账户ID: {accountID}");
            Debug.Log($"加载 coinNum: {coinNum}, 解析成功: {parseSuccess}");
            string bulletName = PlayerPrefs.GetString($"{accountID}{PlayerBulletNameKey}");
            string gunName = PlayerPrefs.GetString($"{accountID}{PlayerBulletGunKey}");
            int playerLevel = PlayerPrefs.GetInt($"{accountID}{PlayerlevelKey}");
            long experiences;
            long.TryParse(PlayerPrefs.GetString($"{accountID}{PlayerexperiencesKey}"), out experiences);
            //PlayerPrefs.SetInt($"{accountID}{PlayerBalstBuffCountKey}", 2);
            //PlayerPrefs.Save();
            int RageSkill = PlayerPrefs.GetInt($"{accountID}{FirstRageSkill}");
            int Replace = PlayerPrefs.GetInt($"{accountID}{FirstReplaceGun}");
            int ZeroToOne = PlayerPrefs.GetInt($"{FirstZeroToOne}{FirstReplaceGun}"); 
            int playerFrozenBuffCount = PlayerPrefs.GetInt($"{accountID}{PlayerFrozenBuffCountKey}");
            int playerBalstBuffCount = PlayerPrefs.GetInt($"{accountID}{PlayerBalstBuffCountKey}");
            DateTime lastSignInDate;
            DateTime lastSpinDate;
            DateTime.TryParse(lastSignInDateStr, out lastSignInDate);
            DateTime.TryParse(lastSpinDateStr, out lastSpinDate);
            // 假设PlayInforManager和相关方法已正确定义
            PlayInforManager.Instance.playInfor.SetPlayerAccount(accountID, creationDate, lastSignInDate, consecutiveDays, coinNum, playerLevel, experiences, playerBalstBuffCount, playerFrozenBuffCount, gunName, bulletName, RageSkill, Replace, ZeroToOne);
            PlayInforManager.Instance.playInfor.lastSpinDate = lastSpinDate;
            GameFlowManager.Instance.currentLevelIndex = playerLevel;
            //TTOD1切枪暂时
            PlayInforManager.Instance.playInfor.SetGun(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation, ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource);

            //PlayInforManager.Instance.playInfor.SetGun(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex - 1).Animation, ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex - 1).Fires[0]).Resource);
            //TTOD复活次数待读表
            PlayInforManager.Instance.playInfor.ResueeCount = (int)(ConfigManager.Instance.Tables.TableGlobal.Get(14).IntValue);
            LoadWeapons(accountID);
            await GameFlowManager.Instance.LoadLevelInitial(GameFlowManager.Instance.currentLevelIndex);
        }
        else
        {
            // 加载第一个关卡
            await GameFlowManager.Instance.LoadLevelInitial(GameFlowManager.Instance.currentLevelIndex);
            //TTOD复活次数待读表
            PlayInforManager.Instance.playInfor.ResueeCount = (int)(ConfigManager.Instance.Tables.TableGlobal.Get(14).IntValue);
            CreateNewAccount();
        }
    }

    /// <summary>
    /// 创建新账户并保存到PlayerPrefs
    /// </summary>
    private void CreateNewAccount()
    {
        //初始设置玩家的枪
        PlayInforManager.Instance.playInfor.SetGun(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation, ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource);
        string newID = GenerateUniqueID();
        string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // 初始化玩家信息
        long initialCoin = 200000000;// ConfigManager.Instance.Tables.TableGlobal.Get(1).LongValue;
        PlayInforManager.Instance.playInfor.SetPlayerAccount(newID, creationDate, DateTime.MinValue, 0, initialCoin, ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Lv, ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Exp, 0, 0, ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation, ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource, 1, 1,1);
        // 保存到PlayerPrefs
        PlayerPrefs.SetString(AccountIDKey, PlayInforManager.Instance.playInfor.accountID);
        PlayerPrefs.SetString(CreationDateKey, PlayInforManager.Instance.playInfor.creationDate);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{LastSignInDateKeyPrefix}", PlayInforManager.Instance.playInfor.lastSignInDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{LastSpinDateKeyPrefix}", PlayInforManager.Instance.playInfor.lastSpinDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{ConsecutiveDaysKeyPrefix}", PlayInforManager.Instance.playInfor.consecutiveDays);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{TotalCoinsKeyPrefix}", PlayInforManager.Instance.playInfor.totalCoins);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerCoinNumKey}", PlayInforManager.Instance.playInfor.coinNum.ToString());
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerlevelKey}", PlayInforManager.Instance.playInfor.level);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerexperiencesKey}", PlayInforManager.Instance.playInfor.experiences.ToString());
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerFrozenBuffCountKey}", PlayInforManager.Instance.playInfor.FrozenBuffCount);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBalstBuffCountKey}", PlayInforManager.Instance.playInfor.BalstBuffCount);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBulletNameKey}", ConfigManager.Instance.Tables.TableTransmitConfig.Get(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0]).Resource);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBulletGunKey}", ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Animation);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{FirstRageSkill}", PlayInforManager.Instance.playInfor.isFirstSpecial ? 1 : 0);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{FirstReplaceGun}", PlayInforManager.Instance.playInfor.isFirstReplaceGun ? 1 : 0);
        PlayerPrefs.SetInt($"{PlayInforManager.Instance.playInfor.accountID}{FirstZeroToOne}", PlayInforManager.Instance.playInfor.FirstZeroToOne ? 1 : 0);

        // 不设置HasCompletedGunSelectionTutorialKey，默认未完成
        PlayerPrefs.DeleteKey(HasCompletedGunSelectionTutorialKey);

        SaveWeapons(newID);
        PlayerPrefs.Save();
        Debug.Log("新账户已创建:");
        Debug.Log("账户ID: " + PlayInforManager.Instance.playInfor.accountID);
        Debug.Log("创建日期: " + PlayInforManager.Instance.playInfor.creationDate);
        Debug.Log($"coinNum: {PlayInforManager.Instance.playInfor.coinNum}");
    }

    public int GetTransmitID(int trasnmitID)
    {
        switch (trasnmitID)
        {
            case 20000:
                return 0;
            case 20100:
                return 1;
            case 20200:
                return 2;
            default:
                return 0;
        }
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
        string accountID = PlayerPrefs.GetString(AccountIDKey, "");
        PlayerPrefs.DeleteKey(AccountIDKey);
        PlayerPrefs.DeleteKey(CreationDateKey);
        PlayerPrefs.DeleteKey($"{accountID}{LastSignInDateKeyPrefix}");
        PlayerPrefs.DeleteKey($"{accountID}{LastSpinDateKeyPrefix}");
        PlayerPrefs.DeleteKey($"{accountID}{ConsecutiveDaysKeyPrefix}");
        PlayerPrefs.DeleteKey($"{accountID}{TotalCoinsKeyPrefix}");
        PlayerPrefs.DeleteKey($"{accountID}{PlayerCoinNumKey}");
        PlayerPrefs.DeleteKey($"{accountID}{PlayerlevelKey}");
        PlayerPrefs.DeleteKey($"{accountID}{PlayerexperiencesKey}");
        PlayerPrefs.DeleteKey($"{accountID}{PlayerFrozenBuffCountKey}");
        PlayerPrefs.DeleteKey($"{accountID}{PlayerBalstBuffCountKey}");
        PlayerPrefs.DeleteKey($"{accountID}{PlayerBulletNameKey}");
        PlayerPrefs.DeleteKey($"{accountID}{PlayerBulletGunKey}");
        PlayerPrefs.DeleteKey($"{accountID}{FirstRageSkill}");
        PlayerPrefs.DeleteKey($"{accountID}{FirstZeroToOne}");
        PlayerPrefs.DeleteKey($"{accountID}{WeaponKey}");
        PlayerPrefs.Save();
        Debug.Log("账户已重置。");
    }
    #region//玩家签到
    /// <summary>
    /// 判断玩家是否可以进行免费转盘
    /// </summary>
    public bool CanUseFreeSpin()
    {
        DateTime today = DateTime.Today;
        return PlayInforManager.Instance.playInfor.lastSpinDate.Date != today;
    }

    /// <summary>
    /// 使用免费转盘机会
    /// </summary>
    public void UseFreeSpin()
    {
        PlayInforManager.Instance.playInfor.lastSpinDate = DateTime.Today;
        SaveAccountData();
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
        reward = (int)(GetDailyReward(PlayInforManager.Instance.playInfor.consecutiveDays) * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total);
        // 保存更新后的数据
        SaveAccountData();
        Debug.Log($"已于 {today} 签到。奖励: {reward} 金币。连续签到天数: {PlayInforManager.Instance.playInfor.consecutiveDays}");
        return true;
    }

    /// <summary>
    /// 根据连续签到天数获取奖励
    /// </summary>
    public int GetDailyReward(int consecutiveDays)
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
    #endregion//玩家签到相关代码结束
    /// <summary>
    /// 保存账户数据到PlayerPrefs
    /// </summary>
    public void SaveAccountData()
    {
        string accountID = PlayInforManager.Instance.playInfor.accountID;
        PlayerPrefs.SetString(AccountIDKey, PlayInforManager.Instance.playInfor.accountID);
        PlayerPrefs.SetString(CreationDateKey, PlayInforManager.Instance.playInfor.creationDate);
        PlayerPrefs.SetString($"{accountID}{LastSignInDateKeyPrefix}", PlayInforManager.Instance.playInfor.lastSignInDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetString($"{accountID}{LastSpinDateKeyPrefix}", PlayInforManager.Instance.playInfor.lastSpinDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt($"{accountID}{ConsecutiveDaysKeyPrefix}", PlayInforManager.Instance.playInfor.consecutiveDays);
        PlayerPrefs.SetInt($"{accountID}{TotalCoinsKeyPrefix}", PlayInforManager.Instance.playInfor.totalCoins);
        PlayerPrefs.SetString($"{accountID}{PlayerCoinNumKey}", PlayInforManager.Instance.playInfor.coinNum.ToString());
        PlayerPrefs.SetInt($"{accountID}{PlayerlevelKey}", PlayInforManager.Instance.playInfor.level);
        PlayerPrefs.SetString($"{accountID}{PlayerexperiencesKey}", PlayInforManager.Instance.playInfor.experiences.ToString());
        PlayerPrefs.SetInt($"{accountID}{PlayerFrozenBuffCountKey}", PlayInforManager.Instance.playInfor.FrozenBuffCount);
        PlayerPrefs.SetInt($"{accountID}{PlayerBalstBuffCountKey}", PlayInforManager.Instance.playInfor.BalstBuffCount);
        PlayerPrefs.SetString($"{accountID}{PlayerBulletNameKey}", PlayInforManager.Instance.playInfor.currentGun.bulletType);
        PlayerPrefs.SetString($"{accountID}{PlayerBulletGunKey}", PlayInforManager.Instance.playInfor.currentGun.gunName);
        PlayerPrefs.SetInt($"{accountID}{FirstRageSkill}", PlayInforManager.Instance.playInfor.isFirstSpecial ? 1 : 0);
        PlayerPrefs.SetInt($"{accountID}{FirstReplaceGun}", PlayInforManager.Instance.playInfor.isFirstReplaceGun ? 1 : 0);
        PlayerPrefs.SetInt($"{accountID}{FirstZeroToOne}", PlayInforManager.Instance.playInfor.FirstZeroToOne ? 1 : 0); 
        // 保存新手引导状态
        if (PlayInforManager.Instance.playInfor.hasCompletedGunSelectionTutorial)
        {
            PlayerPrefs.SetInt(HasCompletedGunSelectionTutorialKey, 1);
        }
        else
        {
            PlayerPrefs.DeleteKey(HasCompletedGunSelectionTutorialKey);
        }

        SaveWeapons(accountID);
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

    //武器存储相关代码
    // 保存武器数据到 PlayerPrefs
    private void SaveWeapons(string accountID)
    {
        string weaponsString = string.Join(",", PlayInforManager.Instance.AllGunName); // 将武器ID列表转化为一个以逗号分隔的字符串
        PlayerPrefs.SetString($"{accountID}{WeaponKey}", weaponsString);
        PlayerPrefs.Save(); // 确保数据被写入
    }


    // TTOD1用户存在则从 PlayerPrefs 中加载武器数据
    public void LoadWeapons(string accountID)
    {
        //TTOD1测试用
        string weaponsString1 = "weapon1-1,weapon1-2,weapon1-3,weapon1-4,weapon1-5,weapon2-1,weapon2-2,weapon2-3,weapon2-4,weapon2-5"; // 将武器ID列表转化为一个以逗号分隔的字符串
        PlayerPrefs.SetString($"{accountID}{WeaponKey}", weaponsString1);
        PlayerPrefs.Save(); // 确保数据被写入

        string weaponsString = PlayerPrefs.GetString($"{accountID}{WeaponKey}", ""); // 获取保存的武器数据字符串
        if (!string.IsNullOrEmpty(weaponsString)) // 如果数据存在
        {
            string[] weaponIDs = weaponsString.Split(','); // 根据逗号分隔字符串
            foreach (var weaponID in weaponIDs)
            {
                //playerWeapons存储的weapon1-1这样的枪素材
                PlayInforManager.Instance.AllGunName.Add(weaponID); // 将字符串解析为武器ID并加入列表
            }
        }
    }



    //宝箱ui
    // 保存宝箱数据
    public void SaveChestData(List<ChestData> chestDataList)
    {
        string accountID = PlayInforManager.Instance.playInfor.accountID;
        // 将chestDataList序列化为JSON字符串
        string jsonData = JsonConvert.SerializeObject(chestDataList);
        // 保存到PlayerPrefs
        PlayerPrefs.SetString($"{accountID}_{ChestDataKey}", jsonData);
        PlayerPrefs.Save();
    }

    // 加载宝箱数据
    public List<ChestData> LoadChestData()
    {
        string accountID = PlayInforManager.Instance.playInfor.accountID;
        // 从PlayerPrefs中读取JSON字符串
        string jsonData = PlayerPrefs.GetString($"{accountID}_{ChestDataKey}", "");
        if (!string.IsNullOrEmpty(jsonData))
        {
            // 反序列化为List<ChestData>
            List<ChestData> chestDataList = JsonConvert.DeserializeObject<List<ChestData>>(jsonData);
            return chestDataList;
        }
        else
        {
            // 如果没有数据，返回一个新的列表
            return new List<ChestData>();
        }
    }



    /// <summary>
    /// 检查是否完成了枪械选择的新手引导
    /// </summary>
    public bool HasCompletedGunSelectionTutorial()
    {
        return PlayerPrefs.HasKey(HasCompletedGunSelectionTutorialKey);
    }

    /// <summary>
    /// 标记已完成枪械选择的新手引导
    /// </summary>
    public void SetCompletedGunSelectionTutorial()
    {
        PlayerPrefs.SetInt(HasCompletedGunSelectionTutorialKey, 1);
        PlayerPrefs.Save();
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

