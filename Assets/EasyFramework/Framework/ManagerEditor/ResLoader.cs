#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace EasyFramework.Editor
{
    public class ResLoader : Singleton<ResLoader>, IResLoader
    {

        private readonly Dictionary<string, string> _assetsMap = new Dictionary<string, string>();

        public ResLoader()
        {
            InitAssetPath();
        }

        public GameObject CreateGameObject(string abName, Transform parent = null)
        {
            GameObject go = LoadAsset<GameObject>(abName);
            return go != null ? GameObject.Instantiate(go, parent) : null;
        }
        public Object LoadObject(string abName) => LoadAsset<Object>(abName);
        public TextAsset LoadTextAsset(string abName) => LoadAsset<TextAsset>(abName);
        public AudioClip LoadAudioClip(string abName) => LoadAsset<AudioClip>(abName);
        public Texture2D LoadTexture2D(string abName) => LoadAsset<Texture2D>(abName);
        public SpriteAtlas LoadSpriteAtlas(string abName) => LoadAsset<SpriteAtlas>(abName);
        public Material LoadMaterial(string abName) => LoadAsset<Material>(abName);

        public T LoadAsset<T>(string abName) where T : Object
        {
            return LoadAssetAtPath<T>(abName) ?? Resources.Load<T>(abName);
        }

        public T[] LoadAllAssets<T>(string abName) where T : Object
        {
            return LoadAllAssetsAtPath<T>(abName) ?? Resources.LoadAll<T>(abName);
        }

        public static T LoadAssetAtPathInEditor<T>(string path) where T : Object
        {

            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private void InitAssetPath()
        {
            string basePath = "Assets/StreamingAssets/Config";
            string[] files = Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories);
            files = StringHelper.FiltrateMetaFile(files);
            foreach (string file in files)
            {
                if (file.EndsWith(".tpsheet"))
                    continue;

                string fileName = StringHelper.GetFileNameWithoutExtension(file);
                //Log.Info(fileName, StringHelper.GetFileNameWithoutSuffix(file));
                if (!_assetsMap.ContainsKey(fileName))
                {
                    _assetsMap.Add(fileName, file);
                }
                else
                {
                    Log.Info("key repeat:", fileName);
                }
            }
        }

        private T LoadAssetAtPath<T>(string abName) where T : Object
        {
            if (_assetsMap.Count == 0)
                InitAssetPath();
            if (_assetsMap.ContainsKey(abName))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_assetsMap[abName]);
            }
            return null;
        }

        private T[] LoadAllAssetsAtPath<T>(string abName) where T : Object
        {
            if (_assetsMap.Count == 0)
                InitAssetPath();

            if (_assetsMap.ContainsKey(abName))
            {
                Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(_assetsMap[abName]);
                List<T> tmp = new List<T>();
                foreach (Object o in objects)
                {
                    T t = o as T;
                    if (t != null)
                        tmp.Add(t);
                }
                return tmp.ToArray();
            }
            return null;
        }
    }
}
#endif
