using UnityEngine;
using System;
using UnityEngine.Networking;

public class AccountManager : Singleton<AccountManager>
{
    // PlayerPrefs����
    private const string AccountIDKey = "PlayerAccountID";
    private const string CreationDateKey = "PlayerCreationDate";

    // ��ǰ�˻���Ϣ
    public PlayerAccount currentAccount;

    //void Awake()
    //{
    //    // ȷ��AccountManager�ڳ����л�ʱ��������
    //    DontDestroyOnLoad(gameObject);
    //    LoadOrCreateAccount();
    //}

    /// <summary>
    /// ���������˻��򴴽����˻�
    /// </summary>
    public  void LoadOrCreateAccount()
    {
        // ���PlayerPrefs���Ƿ������˻�ID
        if (PlayerPrefs.HasKey(AccountIDKey))
        {
            string storedID = PlayerPrefs.GetString(AccountIDKey);
            string storedDate = PlayerPrefs.GetString(CreationDateKey);
            currentAccount = new PlayerAccount(storedID, storedDate);
            Debug.Log("���������˻�:");
            Debug.Log("�˻�ID: " + currentAccount.accountID);
            Debug.Log("��������: " + currentAccount.creationDate);
        }
        else
        {
            // ���û�������˻����������˻�
            CreateNewAccount();
        }
    }

    /// <summary>
    /// �������˻������浽PlayerPrefs
    /// </summary>
    private void CreateNewAccount()
    {
        string newID = GenerateUniqueID();
        string creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        currentAccount = new PlayerAccount(newID, creationDate);

        // ���浽PlayerPrefs
        PlayerPrefs.SetString(AccountIDKey, currentAccount.accountID);
        PlayerPrefs.SetString(CreationDateKey, currentAccount.creationDate);
        PlayerPrefs.Save();

        Debug.Log("�������˻�:");
        Debug.Log("�˻�ID: " + currentAccount.accountID);
        Debug.Log("��������: " + currentAccount.creationDate);
    }

    /// <summary>
    /// ����Ψһ���˻�ID
    /// </summary>
    /// <returns>ΨһID�ַ���</returns>
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
        PlayerPrefs.Save();
        Debug.Log("�˻������á�");
        LoadOrCreateAccount();
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

