using System.IO;

namespace EasyFramework
{
    /// <summary>
    /// 文件助手类
    /// </summary>
    public static class FileHelper
    {
        public static void Delete(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public static void Write(string file, byte[] bytes)
        {
            using (FileStream fsDes = File.Create(file))
            {
                fsDes.Write(bytes, 0, bytes.Length);
                fsDes.Flush();
                fsDes.Close();
            }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public static long GetFileSize(string file)
        {
            if (File.Exists(file))
            {
                FileInfo info = new FileInfo(file);
                return info.Length;
            }
            return 0;
        }

        /// <summary>
        /// 获得路径下面的所有文件（遍历子目录）
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="isAllDirectories"></param>
        /// <returns></returns>
        public static string[] GetFiles(string folder, bool isAllDirectories = true)
        {
            if (isAllDirectories)
                return Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            return Directory.GetFiles(folder);
        }

        #region Directory

        /// <summary>
        /// 创建一个文件夹路径
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="isDeletOldPath">如果检测到旧的路径，是否删除</param>
        public static void CreateDirectory(string directory, bool isDeletOldPath = false)
        {
            if (Directory.Exists(directory))
            {
                if (isDeletOldPath)
                {
                    Directory.Delete(directory, true);
                    Directory.CreateDirectory(directory);
                }
            }
            else
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// 清除文件夹下的所有文件
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static void ClearDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }
            string[] directories = Directory.GetDirectories(directory);
            for (int j = 0; j < directories.Length; j++)
            {
                Directory.Delete(directories[j], true);
            }
            string[] files = Directory.GetFiles(directory);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }


        public static void CopyFolder(string fromPath, string toPath)
        {
            CopyFolder(fromPath, toPath, false, "", null);
        }

        public static void CopyFolder(string fromPath, string toPath, bool deleteOther)
        {
            CopyFolder(fromPath, toPath, deleteOther, "", null);
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="fromPath">源文件夹</param>
        /// <param name="toPath">复制目标文件夹</param>
        /// <param name="deleteOther">是否删除源文件夹不存在的文件</param>
        /// <param name="progressAction">返回当前复制的文件路径跟进度</param>
        public static void CopyFolder(string fromPath, string toPath, bool deleteOther = false, string ignoreSuffix = "", System.Action<string, float> progressAction = null)
        {
            CreateDirectory(toPath);


            if (deleteOther)
            {
                string[] toFolders = Directory.GetDirectories(toPath, "*", SearchOption.AllDirectories);
                foreach (string floder in toFolders)
                {
                    string fromFolder = fromPath + floder.Replace(toPath, "");
                    if (!Directory.Exists(fromFolder))
                    {
                        DeleteDirectory(floder);
                    }
                }
            }

            string[] fromFolders = Directory.GetDirectories(fromPath, "*", SearchOption.AllDirectories);
            foreach (string floder in fromFolders)
            { 
                string toFolder = toPath + floder.Replace(fromPath, "");
                CreateDirectory(toFolder);
            }

            if (deleteOther)
            {
                string[] toFiles = Directory.GetFiles(toPath, "*.*", SearchOption.AllDirectories);
                foreach (string file in toFiles)
                {
                    if (!string.IsNullOrEmpty(ignoreSuffix) && file.EndsWith(ignoreSuffix))
                        continue;

                    string fromFile = fromPath + file.Replace(toPath, "");
                    if (!File.Exists(fromFile))
                    {
                        Delete(file);
                    }
                }
            }

            string[] fromFiles = Directory.GetFiles(fromPath, "*.*", SearchOption.AllDirectories);
            float index = 0;
            foreach (string file in fromFiles)
            {
                string toFile = toPath + file.Replace(fromPath, "");
                File.Copy(file, toFile, true);
                progressAction?.Invoke(file, index / fromFiles.Length);
                index++;
            }
        }

        #endregion


    }
}