using DG.Tweening.Plugins.Core.PathCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace cfg
{
    public class ConfigHelper
    {
#if UNITY_EDITOR

        [MenuItem("MergeJson/Merge")]
        private static void MergeJson()
        {
            //ShellHelper.RunInfo("gen.bat", "E:\\pzy\\unity project\\IceSea\\LuBanTools");
            string Json = "{";
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/LubanConfigs");
            var fileInfos = directoryInfo.GetFiles();
            foreach (var fileInfo in fileInfos)
            {
                var str = File.ReadAllText(fileInfo.FullName, System.Text.Encoding.UTF8);
                if (!string.IsNullOrWhiteSpace(str))
                {
                    if (fileInfo.Name.Split('.').Length > 2) continue;
                    string key = fileInfo.Name.Split('.')[0];
                    Json += $"\"{key}\":{str},";
                }
            }
            Json = Json.TrimEnd(',') + "}";

            //查询路径是否存在不存在就创建
            if (!Directory.Exists(Application.streamingAssetsPath + "/Json"))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Json");
            }

            //写
            File.WriteAllText(Application.streamingAssetsPath + "/Json/configdata.json", Json, System.Text.Encoding.UTF8);
            UnityEngine.Debug.Log("------------Json合并完成-----------");
        }
#endif
        public static JObject jsonnode;
        /// <summary>
        /// 获取信息
        /// </summary>

    public static JSONNode ReadJson(string Name)
    {
            //Name += ".json";
            if (jsonnode == null)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    // 在Android平台，从persistentDataPath读取
                    var strdata = File.ReadAllText(Application.persistentDataPath + "/Json/configdata.json", System.Text.Encoding.UTF8);
                    jsonnode = JObject.Parse(strdata);

                }
                else
                {
                    var strdata = File.ReadAllText(Application.streamingAssetsPath + "/Json/configdata.json", System.Text.Encoding.UTF8);
                    jsonnode = JObject.Parse(strdata);
                }
              
            }
            if (jsonnode[Name] != null)
            {
                return JSON.Parse(jsonnode[Name].ToString());
            }
            return null;
        }



        public static class ShellHelper
        {
            public static void Run(string cmd, string workDirectory, List<string> environmentVars = null)
            {
                System.Diagnostics.Process process = new();
                try
                {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                string app = "bash";
                string splitChar = ":";
                string arguments = "-c";
#elif UNITY_EDITOR_WIN

#endif
                    string app = "cmd.exe";
                    string splitChar = ";";
                    string arguments = "/c";
                    ProcessStartInfo start = new ProcessStartInfo(app);

                    if (environmentVars != null)
                    {
                        foreach (string var in environmentVars)
                        {
                            start.EnvironmentVariables["PATH"] += (splitChar + var);
                        }
                    }

                    process.StartInfo = start;
                    start.Arguments = arguments + " \"" + cmd + "\"";
                    start.CreateNoWindow = true;
                    start.ErrorDialog = true;
                    start.UseShellExecute = false;
                    start.WorkingDirectory = workDirectory;

                    if (start.UseShellExecute)
                    {
                        start.RedirectStandardOutput = false;
                        start.RedirectStandardError = false;
                        start.RedirectStandardInput = false;
                    }
                    else
                    {
                        start.RedirectStandardOutput = true;
                        start.RedirectStandardError = true;
                        start.RedirectStandardInput = true;
                        start.StandardOutputEncoding = System.Text.Encoding.UTF8;
                        start.StandardErrorEncoding = System.Text.Encoding.UTF8;
                    }

                    bool endOutput = false;
                    bool endError = false;

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                        {
                            UnityEngine.Debug.Log(args.Data);
                        }
                        else
                        {
                            endOutput = true;
                        }
                    };

                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                        {
                            UnityEngine.Debug.LogError(args.Data);
                        }
                        else
                        {
                            endError = true;
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    while (!endOutput || !endError)
                    {
                    }

                    process.CancelOutputRead();
                    process.CancelErrorRead();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
                finally
                {
                    process.Close();
                }
            }

            public static void RunInfo(string FileName, string WorkingDirectory)
            {
                System.Diagnostics.Process process = new();
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = true;
                    startInfo.FileName = FileName;
                    startInfo.WorkingDirectory = WorkingDirectory;

                    startInfo.UseShellExecute = true;




                    process.StartInfo = startInfo;

                    bool endOutput = false;
                    bool endError = false;

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                        {
                            UnityEngine.Debug.Log(args.Data);
                        }
                        else
                        {
                            endOutput = true;
                        }
                    };

                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                        {
                            UnityEngine.Debug.LogError(args.Data);
                        }
                        else
                        {
                            endError = true;
                        }
                    };

                    process.Start();

                    process.WaitForExit();

                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($" Luban数据生成错误  {e}");
                    process.Close();
                }
                finally
                {
                }
                UnityEngine.Debug.Log($"<color=#00FF00> --------- Luban数据生成成功 ---------- </color>");
                process.Close();
            }
        }
    }
}