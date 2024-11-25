using UnityEngine;
using System;
using UnityEngine.Networking;
using Unity.VisualScripting;
using UnityEditor;
using Cysharp.Threading.Tasks;

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
    // ���������ڹ������ת�̵ļ�ֵ
    private const string LastSpinDateKeyPrefix = "_LastSpinDate";

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
    public async UniTask LoadOrCreateAccount()
    {
        if (PlayerPrefs.HasKey(AccountIDKey))
        {
            
            string accountID = PlayerPrefs.GetString(AccountIDKey);
            //PlayerPrefs.SetInt($"{accountID}{PlayerlevelKey}", 1);
            //PlayerPrefs.Save();
            string creationDate = PlayerPrefs.GetString(CreationDateKey);
            string lastSignInDateStr = PlayerPrefs.GetString($"{accountID}{LastSignInDateKeyPrefix}");
            string lastSpinDateStr = PlayerPrefs.GetString($"{accountID}{LastSpinDateKeyPrefix}");
            int consecutiveDays = PlayerPrefs.GetInt($"{accountID}{ConsecutiveDaysKeyPrefix}");
            int totalCoins = PlayerPrefs.GetInt($"{accountID}{TotalCoinsKeyPrefix}");
            long coinNum;
            bool parseSuccess = long.TryParse(PlayerPrefs.GetString($"{accountID}{PlayerCoinNumKey}"), out coinNum);
            Debug.Log($"�����˻�ID: {accountID}");
            Debug.Log($"���� coinNum: {coinNum}, �����ɹ�: {parseSuccess}");
            string bulletName = PlayerPrefs.GetString($"{accountID}{PlayerBulletNameKey}");
            string gunName = PlayerPrefs.GetString($"{accountID}{PlayerBulletGunKey}");
            int playerLevel = PlayerPrefs.GetInt($"{accountID}{PlayerlevelKey}");
            long experiences;
            long.TryParse(PlayerPrefs.GetString($"{accountID}{PlayerexperiencesKey}"), out experiences);
            PlayerPrefs.SetInt($"{accountID}{PlayerFrozenBuffCountKey}", 2);
            PlayerPrefs.Save();

            int playerFrozenBuffCount = PlayerPrefs.GetInt($"{accountID}{PlayerFrozenBuffCountKey}");
            int playerBalstBuffCount = PlayerPrefs.GetInt($"{accountID}{PlayerBalstBuffCountKey}");
            DateTime lastSignInDate;
            DateTime lastSpinDate;
            DateTime.TryParse(lastSignInDateStr, out lastSignInDate);
            DateTime.TryParse(lastSpinDateStr, out lastSpinDate);

            // ����PlayInforManager����ط�������ȷ����
            PlayInforManager.Instance.playInfor.SetPlayerAccount(accountID, creationDate, lastSignInDate, consecutiveDays, coinNum, playerLevel, experiences, playerBalstBuffCount, playerFrozenBuffCount, bulletName, gunName);
            PlayInforManager.Instance.playInfor.lastSpinDate = lastSpinDate;
            GameFlowManager.Instance.currentLevelIndex = playerLevel;
            //TTOD�������������
            PlayInforManager.Instance.playInfor.ResueeCount = (int)(ConfigManager.Instance.Tables.TableGlobal.Get(14).IntValue);
            await GameFlowManager.Instance.LoadLevelInitial(GameFlowManager.Instance.currentLevelIndex);
        }
        else
        {
            // ���ص�һ���ؿ�
            await GameFlowManager.Instance.LoadLevelInitial(GameFlowManager.Instance.currentLevelIndex);
            //TTOD�������������
            PlayInforManager.Instance.playInfor.ResueeCount = (int)(ConfigManager.Instance.Tables.TableGlobal.Get(14).IntValue);
            CreateNewAccount();
        }
    }

    /// <summary>
    /// �������˻������浽PlayerPrefs
    /// </summary>
    private void CreateNewAccount()
    {
        PlayInforManager.Instance.playInfor.SetGun(LevelManager.Instance.levelData.GunBulletList[GetTransmitID(ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Fires[0])]);
        string newID = GenerateUniqueID();
        string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // ��ʼ�������Ϣ
        long initialCoin = (long)ConfigManager.Instance.Tables.TableGlobal.Get(1).IntValue;
        PlayInforManager.Instance.playInfor.SetPlayerAccount(newID, creationDate, DateTime.MinValue, 0, initialCoin, ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Lv, ConfigManager.Instance.Tables.TablePlayerConfig.Get(0).Exp, 0, 0, LevelManager.Instance.levelData.GunBulletList[2].bulletType, LevelManager.Instance.levelData.GunBulletList[2].gunName);
        // ���浽PlayerPrefs
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
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBulletNameKey}", LevelManager.Instance.levelData.GunBulletList[2].bulletType);
        PlayerPrefs.SetString($"{PlayInforManager.Instance.playInfor.accountID}{PlayerBulletGunKey}", LevelManager.Instance.levelData.GunBulletList[2].gunName);
        PlayerPrefs.Save();
        Debug.Log("���˻��Ѵ���:");
        Debug.Log("�˻�ID: " + PlayInforManager.Instance.playInfor.accountID);
        Debug.Log("��������: " + PlayInforManager.Instance.playInfor.creationDate);
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
        PlayerPrefs.Save();
        Debug.Log("�˻������á�");
    }

    /// <summary>
    /// �ж�����Ƿ���Խ������ת��
    /// </summary>
    public bool CanUseFreeSpin()
    {
        DateTime today = DateTime.Today;
        return PlayInforManager.Instance.playInfor.lastSpinDate.Date != today;
    }

    /// <summary>
    /// ʹ�����ת�̻���
    /// </summary>
    public void UseFreeSpin()
    {
        PlayInforManager.Instance.playInfor.lastSpinDate = DateTime.Today;
        SaveAccountData();
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
        reward = (int)(GetDailyReward(PlayInforManager.Instance.playInfor.consecutiveDays) * ConfigManager.Instance.Tables.TablePlayerConfig.Get(GameFlowManager.Instance.currentLevelIndex).Total);
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

