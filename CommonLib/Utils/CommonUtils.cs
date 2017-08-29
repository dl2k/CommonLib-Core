using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonLib.Utils
{
    /// <summary>
    /// 共通工具类
    /// </summary>
    public class CommonUtils
    {
        #region Utils
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime ConvertUnixTime(string timeStamp)
        {
            var dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            var lTime = long.Parse(timeStamp + "0000000");
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 按照指定的分隔符，对集合进行连接操作
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="separator">指定的分隔符</param>
        /// <param name="value">指定的集合</param>
        public static string Join<T>(string separator, IEnumerable<T> value)
        {
            StringBuilder builder = new StringBuilder();
            if (value != null)
            {
                T[] arr = value.ToArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i == 0)
                    {
                        builder.Append(arr[i]);
                    }
                    else
                    {
                        builder.Append(separator);
                        builder.Append(arr[i]);
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 分割数组成为指定个组
        /// </summary>
        /// <param name="infos">待分割的数组</param>
        /// <param name="num">分割后的每组个数(剩余部分追加在最后一组中)</param>
        public static List<List<T>> SplitArray<T>(T[] infos, int num)
        {
            int numOfOneArray = ((infos.Length % num) == 0) ? (infos.Length / num) : (infos.Length / num + (infos.Length % num) / num + 1);
            List<T> items = new List<T>(infos);
            List<List<T>> result = new List<List<T>>();
            for (int i = 0; i < items.Count; i++)
            {
                if (i % numOfOneArray == 0)
                {
                    result.Add(new List<T>() { items[i] });
                }
                else
                {
                    result[result.Count - 1].Add(items[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 模拟js时间
        /// </summary>
        public static long GetTimeLikeJS(DateTime timestamp)
        {
            long left = 621356256000000000;
            long Sticks = (timestamp.Ticks < left) ? (CommonUtils.GetNow().Ticks - left) : (timestamp.Ticks - left);
            Sticks = Math.Abs(Sticks) / 10000 / 1000;
            return Sticks;
        }

        /// <summary>
        /// 对指定的对象集合执行分页操作
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="list">指定的对象集合</param>
        /// <param name="currentPage">当前请求的页码</param>
        /// <param name="pageSize">每页显示的最大记录条数</param>
        /// <param name="pageCount">返回总页码数</param>
        /// <returns>执行分页操作后的对象集合</returns>
        public static T[] ToPage<T>(T[] list, int currentPage, int pageSize, out int pageCount)
        {
            T[] result = new T[0];
            pageCount = 0;
            if (list != null)
            {
                if (pageSize <= 0)
                {
                    pageSize = 1;//throw new ArgumentException("每页显示的最大记录条数必须大于0");
                }
                int recordCount = list.Length;
                if (recordCount % pageSize != 0)
                {
                    pageCount = recordCount / pageSize + 1;
                }
                else
                {
                    pageCount = recordCount / pageSize;
                }
                if (currentPage > pageCount)
                {
                    currentPage = pageCount;
                }
                if (currentPage <= 0)
                {
                    currentPage = 1;
                }
                int start = pageSize * (currentPage - 1);
                int end = pageSize * currentPage;
                int size = Math.Min(end, recordCount);
                result = new T[size - start];
                Array.Copy(list, start, result, 0, size - start);
            }
            return result;
        }

        /// <summary>
        /// 方法参数是否有效
        /// </summary>
        /// <param name="param">方法参数</param>
        public static bool IsParamsVaild(params object[] param)
        {
            bool result = true;
            if (param != null)
            {
                foreach (object item in param)
                {
                    if (item == null)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return result;
        }

        /// <summary>
        /// 从字符串中提取数字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int GetNumFromString(string source)
        {
            source = Regex.Replace(source, @"[^\d]*", "");
            int result;
            if (!int.TryParse(source, out result))
            {
                result = 0;
            }
            return result;
        }

        /// <summary>
        /// 过滤字符串中的换行和空格
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FilterWordwrapAndBlank(string source)
        {
            if (!String.IsNullOrEmpty(source))
            {
                source = source.Replace("\n", "").Replace(" ", "");
            }
            return source;
        }

        /// <summary>
        /// 检查危险字符
        /// </summary>
        /// <param name="sInput"></param>
        /// <returns>检测到危险字符则返回false</returns>
        public static bool SqlSafety(string sInput)
        {
            if (string.IsNullOrEmpty(sInput))
                return true;

            var sInputLower = sInput.ToLower();

            const string pattern = @"or|and|exec|insert|select|delete|update|count|master|truncate|declare|char\(|mid\(|chr\(|'";
            if (Regex.Match(sInputLower, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Success)
                return false;

            return true;
        }

        /// <summary>
        /// OADate转换成datetime
        /// </summary>
        /// <param name="oaDate">转换信息</param>
        public static DateTime OADateToDT(double oaDate)
        {
            DateTime result = DateTime.MinValue;
            try
            {
                result = DateTime.FromOADate(oaDate);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 转换特殊格式时间的方法
        /// </summary>
        public static DateTime ToDTFit(string time)
        {
            DateTime result = DateTime.MinValue;
            if (!DateTime.TryParse(time, out result))
            {
                if (!DateTime.TryParseExact(time, "yyyyMMdd", null, DateTimeStyles.None, out result))
                {
                    if (!DateTime.TryParseExact(time, "yyyyMMddHHmm", null, DateTimeStyles.None, out result))
                    {
                        if (!DateTime.TryParseExact(time, "yyyyMMddHHmmss", null, DateTimeStyles.None, out result))
                        {
                            if (!DateTime.TryParseExact(time, "MM-dd HH:mm", null, DateTimeStyles.None, out result))
                            {
                                if (!DateTime.TryParseExact(time, "MMddHHmm", null, DateTimeStyles.None, out result))
                                {
                                    if (!DateTime.TryParseExact(time, "MM-dd", null, DateTimeStyles.None, out result))
                                    {
                                        if (!DateTime.TryParseExact(time, "HH:mm", null, DateTimeStyles.None, out result))
                                        {
                                            if (!DateTime.TryParseExact(time, "yyyyMM", null, DateTimeStyles.None, out result))
                                            {
                                                if (!DateTime.TryParseExact(time, "MMdd", null, DateTimeStyles.None, out result))
                                                {
                                                    if (!DateTime.TryParseExact(time, "ddHH", null, DateTimeStyles.None, out result))
                                                    {
                                                        if (!DateTime.TryParseExact(time, "ddHHmm", null, DateTimeStyles.None, out result))
                                                        {
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="num">数据</param>
        /// <param name="tag">小数点位数</param>
        public static float Round(float num, int tag)
        {
            float result = num;
            if (num.ToString().EndsWith("5"))
            {
                float temp = (float)Math.Round(num, tag + 1, MidpointRounding.AwayFromZero);
                temp /= temp * (float)Math.Pow(10, tag + 1);
                num = num + temp;
            }
            result = (float)Math.Round(Math.Round(num, tag + 1, MidpointRounding.AwayFromZero), tag, MidpointRounding.AwayFromZero);
            return result;
        }

        /// <summary>
        /// 为敏感信息打马赛克，如果起始位置大于字符串长度，则在最后一个字符加马赛克字符
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="maskChar">马赛克字符</param>
        /// <param name="startIndex">起始位置，从0开始</param>
        /// <param name="maskLength">马赛克长度</param>
        /// <returns></returns>
        public static string MaskString(string source, char maskChar, int startIndex, int maskLength)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            int length = source.Length;
            int start = Math.Min(length - 1, startIndex - 1);
            StringBuilder sb = new StringBuilder(source.Substring(0, start));
            maskLength = Math.Min(length - start, maskLength);
            sb.Append(maskChar, maskLength);
            int end = Math.Min(start + maskLength, length - 1);
            string behindMaskStr = length - startIndex <= 0 ? "" : source.Substring(end, Math.Max(length - start - maskLength, 0));
            sb.Append(behindMaskStr);
            return sb.ToString();
        }

        /// <summary>
        /// 数字格式化
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="tag">小数位数</param>
        /// <returns></returns>
        public static string NumFormat(decimal value, int tag = 2)
        {
            try
            {
                if (value > 100000000)
                {
                    return Math.Round(value / 100000000, tag) + "亿";
                }
                if (value > 10000000)
                {
                    return Math.Round(value / 10000000, tag) + "千万";
                }
                if (value > 1000000)
                {
                    return Math.Round(value / 1000000, tag) + "百万";
                }
                if (value > 10000)
                {
                    return Math.Round(value / 10000, tag) + "万";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("转换失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return value.ToString();
        }

        /// <summary>
        /// 数字格式化
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="tag">小数位数</param>
        /// <param name="unit">返回数据的单位</param>
        /// <returns></returns>
        public static decimal NumFormatEx(decimal value, out string unit, int tag = 2)
        {
            try
            {
                if (value > 100000000)
                {
                    unit = "亿";
                    return Math.Round(value / 100000000, tag);
                }
                if (value > 10000000)
                {
                    unit = "千万";
                    return Math.Round(value / 10000000, tag);
                }
                if (value > 1000000)
                {
                    unit = "百万";
                    return Math.Round(value / 1000000, tag);
                }
                if (value > 10000)
                {
                    unit = "万";
                    return Math.Round(value / 10000, tag);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("转换失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            unit = "";
            return value;
        }
        #endregion


        #region 当前时间/当前日期
        public static DateTime GetNow()
        {
            return DateTime.Now;
        }

        public static DateTime GetToday()
        {
            return DateTime.Today;
        }
        #endregion
    }

}
