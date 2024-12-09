using cfg;
using Cysharp.Threading.Tasks;
using LitJson;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using System.IO;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
    // Զ�������ļ���ַ�������ʵ�������޸�
    private readonly string url = "https://assets.youraigame.com/files_";

    // �Ƿ�ɹ��������ñ�ʶ
    public bool successfullyLoaded = false;

    // ���ñ�
    private Tables tables;
    public Tables Tables { get => tables; }

    /// <summary>
    /// ��ʼ�����ù�����
    /// ���ݲ�ͬ�ı���������ȡ��Ӧ��Զ���������ݣ�������д�뱾���ļ����ж�ȡ�ͼ���
    /// </summary>
    public async UniTask Init()
    {
#if Develop
        // �ڿ��������»�ȡ�ض��汾�������ļ�
        (int err, string str) = await HttpClientSdk.GetConfig(url + "1.1.1");
        TryUpdateConfig(str);
#elif TEST
        // �ڲ��Ի����»�ȡ�ض��汾�������ļ�
        (int err, string str) = await HttpClientSdk.GetConfig(url + "0.0.0");
        TryUpdateConfig(str);
#else
        // ����ʽ�����»�ȡĬ�������ļ���configdataNew��
        (int err, string str) = await HttpClientSdk.GetConfig("configdata");
        TryUpdateConfig(str);
#endif
        // ʹ�ü��ط�����ʼ�����ñ�
        tables = new cfg.Tables(LoadByteBuf);

        // ��վ�̬�����е�����
        ConfigHelper.jsonnode = null;

        // ����ѳɹ�����
        successfullyLoaded = true;
    }

    /// <summary>
    /// ���Խ��ַ�������Ϊ Json����д�뱾���ļ�
    /// ���Androidƽ̨ʹ�ÿ�д·��
    /// </summary>
    private void TryUpdateConfig(string str)
    {
        Debug.Log("����" + str);
        // �����ص��ַ�������Ϊ JsonData ����
        JsonData re = JsonMapper.ToObject<JsonData>(str);
        string path = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            // ��Androidƽ̨��ʹ��persistentDataPath����д��
           path = Path.Combine(Application.persistentDataPath, "Json/configdata.json");

        }
        else
        {
            // ����ƽ̨ʹ��StreamingAssets·��
             path = Path.Combine(Application.streamingAssetsPath, "Json/configdata.json");
        }

        // ȷ��Ŀ¼����
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // ����ȡ�� Json ����д��ָ��·���� configdata.json �ļ���
        File.WriteAllText(path, JsonMapper.ToJson(re));

        // �˴�������Ҫ���ɽ�һ������д��������
    }

    /// <summary>
    /// �ӱ����ļ���ȡ JSON ���ݲ�ת��Ϊ JSONNode �����������ñ�ļ���
    /// </summary>
    private static JSONNode LoadByteBuf(string file)
    {
        //string path = "";
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    // ��Androidƽ̨����persistentDataPath��ȡ
        //    path = Path.Combine(Application.persistentDataPath, "Json", file);

        //}
        //else
        //{
        //    // ����ƽ̨����StreamingAssets��ȡ
        //     path = Path.Combine(Application.streamingAssetsPath, "Json", file);
        //}
        // ���ù��߷�����ָ���ļ���ȡ JSON�������� JSONNode ����
        return ConfigHelper.ReadJson(file);
    }

    protected override void Awake()
    {
        // ���ж���ĳ�ʼ���߼������ڴ˴����
    }
}
