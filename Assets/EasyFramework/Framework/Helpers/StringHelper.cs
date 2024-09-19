using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EasyFramework
{
    public static class StringHelper
    {
        ///// <summary>
        ///// 把秒 转换成 时间格式的字符串 如：01:10:56
        ///// </summary>
        ///// <param name="second">秒</param>
        ///// <returns></returns>
        //public static string ConvertSecondToData(int second)
        //{
        //    TimeSpan ts = new TimeSpan(0, 0, second);
        //    return string.Format("{0}:{1}:{2}", ts.Hours <= 9 ? "0" + ts.Hours : "" + ts.Hours,
        //         ts.Minutes <= 9 ? "0" + ts.Minutes : "" + ts.Minutes,
        //         ts.Seconds <= 9 ? "0" + ts.Seconds : "" + ts.Seconds);
        //}

        /// <summary>
        /// 过滤掉.meta文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string[] FiltrateMetaFile(string[] files)
        {
            return FiltrateFile(files, ".meta");
        }

        /// <summary>
        /// 过滤掉含有指定字符串的文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="filtrateName"></param>
        /// <returns></returns>
        public static string[] FiltrateFile(string[] files, string filtrateName)
        {
            List<string> result = new List<string>();
            foreach (string file in files)
            {
                if (file.IndexOf(filtrateName) > 0)
                    continue;
                result.Add(file);
            }
            return result.ToArray();
        }

        /// <summary>
        /// 获得路径或者文件夹的文件名，有后缀
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileName(string file)
        {
            if (file == null)
            {
                return null;
            }
            file = file.Replace("\\", "/");
            int num = file.LastIndexOf("/");
            if (num > 0)
            {
                return file.Substring(num + 1, file.Length - num - 1);
            }
            return file;
        }

        /// <summary>
        /// 获得路径或者文件夹的文件名，有后缀
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                file = file.Replace("\\", "/");
                int num = file.LastIndexOf("/");
                if (num > 0)
                    file = file.Substring(num + 1, file.Length - num - 1);

                num = file.LastIndexOf(".");
                if (num > 0)
                    file = file.Substring(0, num);
            }
            return file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string Combine(string path1, string path2)
        {
            if (!path1.EndsWith("/"))
            {
                path1 += "/";
            }
            if (path2.StartsWith("/"))
            {
                path2 = path2.Substring(1);
            }
            return path1 + path2;
        }

        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
        public static bool IsUnsign(string value)
        {
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }
        public static bool isTel(string strInput)
        {
            return Regex.IsMatch(strInput, @"\d{3}-\d{8}|\d{4}-\d{7}");
        }
        
        /// <summary>
        /// 从字符串中提取数字，支持非纯数字型字符串
        /// </summary>
        /// <returns>正整数</returns>
        public static int ToInt(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
            string result = Regex.Replace(str, @"[^0-9]+", "");
            return int.Parse(result);
        }
        
        public static short ToShort(this string str)
        {
            string result = Regex.Replace(str, @"[^0-9]+", "");
            return short.Parse(result);
        }

        /// <summary>
        /// 中文字符描述集合
        /// </summary>
        static readonly char[] ChineseCollection = { '零', '一', '二', '三', '四', '五', '六', '七', '八', '九',
            '十','百','千','万','亿' };
        /// <summary>
        /// 数字转中文
        /// 按照【个、十、百、千、万、亿】，只考虑正整数和零
        /// </summary>
        public static string NumberToChinese(int number)
        {
            //商和余数
            int quotient = 0;//商
            int remainder = 0;//余数
            number = Math.Abs(number);
            if (number < 10)
            {
                //0~9
                return ChineseCollection[number].ToString();
            }
            if (number < 100)
            {
                //10~99之间
                quotient = number / 10;
                remainder = number % 10;//考虑到 余数为0时特殊处理
                return ChineseCollection[quotient].ToString() + ChineseCollection[10] + (remainder == 0 ? "" : ChineseCollection[remainder].ToString());
            }
            if (number < 1000)
            {
                //100~999
                quotient = number / 100;
                remainder = number % 100;
                if (remainder == 0)
                {
                    return ChineseCollection[quotient].ToString() + ChineseCollection[11];
                }
                string handleRemainder = "";
                if (remainder > 0 && remainder < 10)
                {
                    handleRemainder = ChineseCollection[0].ToString();//加上零 如 502
                }
                return ChineseCollection[quotient].ToString() + ChineseCollection[11] + handleRemainder + NumberToChinese(remainder);
            }
            if (number < 10000)
            {
                //1000~9999
                quotient = number / 1000;
                remainder = number % 1000;
                if (remainder == 0)
                {
                    return ChineseCollection[quotient].ToString() + ChineseCollection[12];
                }
                string handleRemainder = "";
                if (remainder > 0 && remainder < 100)
                {
                    handleRemainder = ChineseCollection[0].ToString();//加上零 如 502
                }
                return ChineseCollection[quotient].ToString() + ChineseCollection[12] + handleRemainder + NumberToChinese(remainder);
            }
            if (number < 100000000)
            {
                //10000~9999 9999
                quotient = number / 10000;
                remainder = number % 10000;
                if (remainder == 0)
                {
                    return NumberToChinese(quotient) + ChineseCollection[13];
                }
                string handleRemainder = "";
                if (remainder > 0 && remainder < 1000)
                {
                    handleRemainder = ChineseCollection[0].ToString();//加上零 如 502
                }
                return NumberToChinese(quotient) + ChineseCollection[13] + handleRemainder + NumberToChinese(remainder);
            }
            else
            {
                //一亿或以上
                quotient = number / 100000000;
                remainder = number % 100000000;
                if (remainder == 0)
                {
                    return NumberToChinese(quotient) + ChineseCollection[14];
                }
                string handleRemainder = "";
                if (remainder > 0 && remainder < 1000)
                {
                    handleRemainder = ChineseCollection[0].ToString();//加上零 如 502
                }
                return NumberToChinese(quotient) + ChineseCollection[14] + handleRemainder + NumberToChinese(remainder);
            }
        }
    }
}