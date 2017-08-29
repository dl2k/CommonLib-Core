using System;
using System.IO;

namespace CommonLib.IO
{
    public class PathHelper
    {
        /// <summary>
        /// 组合路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="sub">文件名</param>
        /// <returns>文件路径字符串</returns>
        public static string MergePathName(string path, string sub)
        {
            path = path.Trim();
            sub = sub.Trim();
            string p = new string(Path.DirectorySeparatorChar, 1);

            if (!path.EndsWith(p))
            {
                path += Path.DirectorySeparatorChar;
            }

            return path + sub;
        }

        /// <summary>
        /// 获取程序集根目录下的Config文件夹路径
        /// </summary>
        /// <returns>路径字符串</returns>
        public static string GetConfigPath()
        {
            return MergePathName(MapPath, "Config");
        }


        /// <summary>
        /// 获取程序集根目录
        /// </summary>
        public static string MapPath { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }

        /// <summary>
        /// 组合URL
        /// </summary>
        /// <param name="path">url路径</param>
        /// <param name="sub">要合并的路径或文件名等</param>
        /// <returns>url</returns>
        public static string MergeUrl(string path, string sub)
        {
            path = path.Trim();
            sub = sub.Trim();

            if (!path.EndsWith("/"))
            {
                path += '/';
            }

            return path + sub;
        }
    }
}
