//***********************************************************
// 描述：这是一个功能代码
// 作者：阿福 
// 创建时间：2023-11-16 17:54:42
// 版 本：1.0
//***********************************************************
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace WebSocket.Editor
{
    public static class DevelopScriptingDefineSymbols
    {
        private const string EnableDebugScriptingDefineSymbol = "DEVELOP_DEBUG";
        private const string EnableReleaseScriptingDefineSymbol = "DEVELOP_RELEASE_TT";


        private static readonly string[] AllDevelopScriptingDefineSymbols = new string[]
        {
            EnableDebugScriptingDefineSymbol,
            EnableReleaseScriptingDefineSymbol,

        };

        /// <summary>
        /// 禁用所有日志脚本宏定义。
        /// </summary>
        [MenuItem("3DDots/Develop/Disable All Develop", false, 30)]
        public static void DisableAllDevelop()
        {
            foreach (string aboveLogScriptingDefineSymbol in AllDevelopScriptingDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
            }
        }

        /// <summary>
        /// 开启Debug脚本宏定义。
        /// </summary>
        [MenuItem("3DDots/Develop/Enable Develop_Debug", false, 31)]
        public static void EnableDebug()
        {
            SetScriptingDefineSymbol(EnableDebugScriptingDefineSymbol);
        }

        /// <summary>
        /// 开启Debug脚本宏定义。
        /// </summary>
        [MenuItem("3DDots/Develop/Enable Develop_Release_TT", false, 32)]
        public static void EnableRelease()
        {
            SetScriptingDefineSymbol(EnableReleaseScriptingDefineSymbol);
        }

        /// <summary>
        /// 设置脚本宏定义。
        /// </summary>
        /// <param name="scriptingDefineSymbol">要设置的开发脚本宏定义。</param>
        public static void SetScriptingDefineSymbol(string scriptingDefineSymbol)
        {
            if (string.IsNullOrEmpty(scriptingDefineSymbol))
            {
                return;
            }

            foreach (string i in AllDevelopScriptingDefineSymbols)
            {
                if (i == scriptingDefineSymbol)
                {
                    DisableAllDevelop();
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(scriptingDefineSymbol);
                    return;
                }
            }
        }


    }
}