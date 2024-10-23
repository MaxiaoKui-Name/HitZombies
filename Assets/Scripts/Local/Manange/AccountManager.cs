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
    //    // ȷ��AccountManager�ڳ����л�ʱ��������
    //    DontDestroyOnLoad(gameObject);
    //    LoadOrCreateAccount();
    //}

    /// <summary>
    /// ���������˻��򴴽����˻�
    /// </summary>
    /// <summary>
    /// ���������˻��򴴽����˻�
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
            Debug.Log($"�����˻�ID: {accountID}");
            Debug.Log($"���� coinNum: {coinNum}, �����ɹ�: {parseSuccess}");
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
    /// �������˻������浽PlayerPrefs
    /// </summary>
    private void CreateNewAccount()
    {
        PlayInforManager.Instance.playInfor.SetGun(LevelManager.Instance.levelData.GunBulletList[2]);
        string newID = GenerateUniqueID();
        string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //TTOD1��ʼѪ��������
        PlayInforManager.Instance.playInfor.SetPlayerAccount(newID, creationDate, DateTime.MinValue, 0, 40000, ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Lv, ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Exp, 0, 0);
        // ���浽PlayerPrefs
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
        Debug.Log("���˻��Ѵ���:");
        Debug.Log("�˻�ID: " + PlayInforManager.Instance.playInfor.accountID);
        Debug.Log("��������: " + PlayInforManager.Instance.playInfor.creationDate);
        Debug.Log($"coinNum: {PlayInforManager.Instance.playInfor.coinNum}");
    }
/// <summary>
/// ����Ψһ���˻�ID
/// </summary>
private string GenerateUniqueID()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// �����˻���Ϣ�����ڲ��Ի����ù��ܣ�
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
        Debug.Log("�˻������á�");
    }

    /// <summary>
    /// ����ÿ��ǩ��
    /// </summary>
    public bool SignIn(out int reward)
    {
        DateTime today = DateTime.Today;
        if (PlayInforManager.Instance.playInfor.lastSignInDate == today)
        {
            // ������ǩ��
            reward = 0;
            return false;
        }
        // ����ϴ�ǩ���Ƿ�Ϊ����
        if (PlayInforManager.Instance.playInfor.lastSignInDate == today.AddDays(-1))
        {
            PlayInforManager.Instance.playInfor.consecutiveDays += 1;
        }
        else
        {
            PlayInforManager.Instance.playInfor.consecutiveDays = 1; // ���������������
        }
        PlayInforManager.Instance.playInfor.lastSignInDate = today;

        // ȷ������
        reward = GetDailyReward(PlayInforManager.Instance.playInfor.consecutiveDays);
        PlayInforManager.Instance.playInfor.AddCoins((int)(reward * ConfigManager.Instance.Tables.TablePlayerConfig.Get(PlayInforManager.Instance.playInfor.level).Total));
        // ������º������
        SaveAccountData();
        Debug.Log($"���� {today} ǩ��������: {reward} ��ҡ�����ǩ������: {PlayInforManager.Instance.playInfor.consecutiveDays}");
        return true;
    }

    /// <summary>
    /// ��������ǩ��������ȡ����
    /// </summary>
    private int GetDailyReward(int consecutiveDays)
    {
        int multiple = 0; 
        // TTOD1��������Ķ���
        switch (consecutiveDays)
        {
            case 1:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(1).Money;
                return multiple; // ��1��
            case 2:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(2).Money;
                return multiple; // ��2��
            case 3:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(3).Money;
                return multiple;// ��3��
            case 4:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(4).Money;
                return multiple; // ��4��
            case 5:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(5).Money;
                return multiple; // ��5�죨���⽱����
            default:
                multiple = ConfigManager.Instance.Tables.TableDailyConfig.Get(5).Money;
                return multiple; // ����5�������Ϊ��1�콱��
        }
    }

    /// <summary>
    /// �����˻����ݵ�PlayerPrefs
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
    /// ��ȡ�ܽ������
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
//    private const string PlayerAccountKey = "PlayerAccount"; // ���ڱ��ش洢����˺ŵļ�
//    private string playerAccount; // ����˺�

//    void Start()
//    {
//        // ��鱾���Ƿ��Ѵ洢�˺�
//        if (PlayerPrefs.HasKey(PlayerAccountKey))
//        {
//            // �����Ѵ��ڵ��˺�
//            playerAccount = PlayerPrefs.GetString(PlayerAccountKey);
//            Debug.Log("�����Ѵ����˺�: " + playerAccount);
//            StartCoroutine(SendPlayerAccountToServer(playerAccount)); // ������������˺�
//        }
//        else
//        {
//            // ���û�б����˺ţ������������������˺�
//            StartCoroutine(RequestNewPlayerAccount());
//        }
//    }

//    // �������������������˺�
//    IEnumerator RequestNewPlayerAccount()
//    {
//        string url = "http://yourserver.com/account_handler.php"; // �滻Ϊ��ķ�������ַ

//        UnityWebRequest www = UnityWebRequest.Post(url, "");
//        yield return www.SendWebRequest();

//        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
//        {
//            Debug.LogError("�����˺�ʧ��: " + www.error);
//        }
//        else
//        {
//            // �������������ص�JSON����
//            string jsonResponse = www.downloadHandler.text;
//            AccountResponse response = JsonUtility.FromJson<AccountResponse>(jsonResponse);

//            if (response.status == "success")
//            {
//                // ���������ɵ��˺�
//                playerAccount = response.player_id;
//                PlayerPrefs.SetString(PlayerAccountKey, playerAccount);
//                PlayerPrefs.Save(); // �����˺ŵ�����
//                Debug.Log("���˺�������: " + playerAccount);
//            }
//            else
//            {
//                Debug.LogError("�˺�����ʧ��: " + response.message);
//            }
//        }
//    }

//    // ����������ͱ������е�����˺�
//    IEnumerator SendPlayerAccountToServer(string playerId)
//    {
//        string url = "http://yourserver.com/account_handler.php"; // �滻Ϊ��ķ�������ַ

//        WWWForm form = new WWWForm();
//        form.AddField("player_id", playerId);

//        UnityWebRequest www = UnityWebRequest.Post(url, form);
//        yield return www.SendWebRequest();

//        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
//        {
//            Debug.LogError("�˺ż��ʧ��: " + www.error);
//        }
//        else
//        {
//            // �������������ص���Ӧ
//            string jsonResponse = www.downloadHandler.text;
//            AccountResponse response = JsonUtility.FromJson<AccountResponse>(jsonResponse);

//            if (response.status == "success")
//            {
//                Debug.Log("������ȷ���˺�: " + response.player_id);
//            }
//            else
//            {
//                Debug.LogError("������δ��ȷ���˺�: " + response.message);
//            }
//        }
//    }

//    // ���ڽ������������ص�JSON����
//    [System.Serializable]
//    public class AccountResponse
//    {
//        public string status;
//        public string player_id;
//        public string message;
//    }
//}

