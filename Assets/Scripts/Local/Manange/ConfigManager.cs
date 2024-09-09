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
        // 尝试将返回的字符串解析为 JsonData
        JsonData re = JsonMapper.ToObject<JsonData>(str);
        File.WriteAllText(Application.streamingAssetsPath + "/Json/configdata.json", JsonMapper.ToJson(re));
        // 加载配置表
    }

    private static JSONNode LoadByteBuf(string file)
    {
        // 从本地文件读取 JSON 数据，并转换为 JSONNode 对象
        return ConfigHelper.ReadJson(file);
    }

    protected override void Awake()
    {
        // 在这里可以放置其他初始化逻辑
    }
}

