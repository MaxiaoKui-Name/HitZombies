using cfg;
using Cysharp.Threading.Tasks;
using LitJson;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using System.IO;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
    private readonly string url = "https://assets.youraigame.com/files_";
    public bool successfullyLoaded = false;
    private Tables tables;
    public Tables Tables { get => tables; }

    public async UniTask Init()
    {
#if Develop
        (int err, string str) = await HttpClientSdk.GetConfig(url + "1.1.1");
        TryUpdateConfig(str);
#elif TEST
        (int err, string str) = await HttpClientSdk.GetConfig(url + "0.0.0");
        TryUpdateConfig(str);
#else
        (int err, string str) = await HttpClientSdk.GetConfig("configdata");
        TryUpdateConfig(str);
#endif
        tables = new cfg.Tables(LoadByteBuf);
        ConfigHelper.jsonnode = null;
        successfullyLoaded = true;
    }

    private void TryUpdateConfig(string str)
    {
        // ���Խ����ص��ַ�������Ϊ JsonData
        JsonData re = JsonMapper.ToObject<JsonData>(str);
        File.WriteAllText(Application.streamingAssetsPath + "/Json/configdata.json", JsonMapper.ToJson(re));
        // �������ñ�
    }

    private static JSONNode LoadByteBuf(string file)
    {
        // �ӱ����ļ���ȡ JSON ���ݣ���ת��Ϊ JSONNode ����
        return ConfigHelper.ReadJson(file);
    }

    protected override void Awake()
    {
        // ��������Է���������ʼ���߼�
    }
}

