using cfg;
using Cysharp.Threading.Tasks;
using LitJson;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using System.IO;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
    // 远端配置文件地址，请根据实际需求修改
    private readonly string url = "https://assets.youraigame.com/files_";

    // 是否成功加载配置标识
    public bool successfullyLoaded = false;

    // 配置表
    private Tables tables;
    public Tables Tables { get => tables; }

    /// <summary>
    /// 初始化配置管理器
    /// 根据不同的编译条件获取相应的远程配置数据，并将其写入本地文件进行读取和加载
    /// </summary>
    public async UniTask Init()
    {
#if Develop
        // 在开发环境下获取特定版本的配置文件
        (int err, string str) = await HttpClientSdk.GetConfig(url + "1.1.1");
        TryUpdateConfig(str);
#elif TEST
        // 在测试环境下获取特定版本的配置文件
        (int err, string str) = await HttpClientSdk.GetConfig(url + "0.0.0");
        TryUpdateConfig(str);
#else
        // 在正式环境下获取默认配置文件（configdataNew）
        (int err, string str) = await HttpClientSdk.GetConfig("configdata");
        TryUpdateConfig(str);
#endif
        // 使用加载方法初始化配置表
        tables = new cfg.Tables(LoadByteBuf);

        // 清空静态变量中的数据
        ConfigHelper.jsonnode = null;

        // 标记已成功加载
        successfullyLoaded = true;
    }

    /// <summary>
    /// 尝试将字符串解析为 Json，并写入本地文件
    /// 针对Android平台使用可写路径
    /// </summary>
    private void TryUpdateConfig(string str)
    {
        Debug.Log("内容" + str);
        // 将返回的字符串解析为 JsonData 对象
        JsonData re = JsonMapper.ToObject<JsonData>(str);
        string path = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            // 在Android平台，使用persistentDataPath进行写入
           path = Path.Combine(Application.persistentDataPath, "Json/configdata.json");

        }
        else
        {
            // 其他平台使用StreamingAssets路径
             path = Path.Combine(Application.streamingAssetsPath, "Json/configdata.json");
        }

        // 确保目录存在
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 将获取的 Json 数据写入指定路径的 configdata.json 文件中
        File.WriteAllText(path, JsonMapper.ToJson(re));

        // 此处如有需要，可进一步处理写入后的数据
    }

    /// <summary>
    /// 从本地文件读取 JSON 数据并转换为 JSONNode 对象，用于配置表的加载
    /// </summary>
    private static JSONNode LoadByteBuf(string file)
    {
        //string path = "";
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    // 在Android平台，从persistentDataPath读取
        //    path = Path.Combine(Application.persistentDataPath, "Json", file);

        //}
        //else
        //{
        //    // 其他平台，从StreamingAssets读取
        //     path = Path.Combine(Application.streamingAssetsPath, "Json", file);
        //}
        // 调用工具方法从指定文件读取 JSON，并返回 JSONNode 对象
        return ConfigHelper.ReadJson(file);
    }

    protected override void Awake()
    {
        // 如有额外的初始化逻辑，可在此处添加
    }
}
