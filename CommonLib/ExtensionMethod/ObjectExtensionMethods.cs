﻿using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using CommonLib.Utils;

namespace CommonLib.ExtensionMethod
{
    /// <summary>
    /// Object的扩展方法
    /// </summary>
    public static class ObjectExtensionMethods
    {
        #region TOJSON
        /// <summary>
        /// 扩展方法：将对象序列化为JSON
        /// </summary>
        /// <param name="pCaller">调用者</param>
        /// <returns>对象序列化为JSON的字符串</returns>
        public static string ToJSON(this object pCaller)
        {
            if (pCaller == null) { return "null"; }

            if (pCaller.GetType().Name == "DataTable" || pCaller.GetType().Name == "DataSet")
            {
                //DataTable序列化
                return JsonHelper.JsonSerializer(pCaller, JsonFormatting.Indented);
            }
            else
            {
                //其它直接序列化
                return JsonHelper.JsonSerializer(pCaller);
            }
        }

        #endregion

        #region TOINT
        /// <summary>
        /// int 类型转换的方法
        /// </summary>
        /// <param name="id">string 值</param>
        /// <returns>返回转换后的值</returns>
        public static int ToInt(this object obj, int DefaultValue = 0)
        {
            if (obj != null && obj != DBNull.Value)
            {
                var str = obj.ToString();
                var index = str.IndexOf('.');
                var temp = index > 0 ? str.Substring(0, index) : str;
                int.TryParse(temp, out DefaultValue);
            }

            return DefaultValue;
        }
        #endregion

        #region TODATETIME
        /// <summary>
        /// datetime 类型转换的方法
        /// </summary>
        /// <param name="id">string 值</param>
        /// <returns>返回转换后的值</returns>
        public static DateTime ToDateTime(this object time)
        {
            DateTime result;
            if (time != null && DateTime.TryParse(time.ToString(), out result))
            {
                return result;
            }
            return DateTime.MinValue;
        }
        #endregion

        #region TOdecimal
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj, decimal DefaultValue = 0m)
        {
            if (obj != null && obj != DBNull.Value)
                decimal.TryParse(obj.ToString(), out DefaultValue);

            return DefaultValue;
        }
        #endregion

        #region ToBytes

        /// <summary>
        /// 获取可序列化对象的二进制数据
        /// </summary>
        /// <param name="pValue">可序列化对象</param>
        /// <returns></returns>
        public static byte[] GetBytes(this object pValue)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, pValue);
                return ms.ToArray();
            }
        }
        #endregion

        #region 判断GUID是否为空GUID
        /// <summary>
        /// 判断是否为Guid.Empty
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsEmptyGuid(this Guid guid)
        {
            return guid == Guid.Empty;
        }
        #endregion

        #region ToMoney
        /// <summary>
        /// decimal 转货币型，保留两位小数，舍去后面的数值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static decimal ToMoney(this decimal d)
        {
            d = decimal.Floor(d * 100) / 100;
            return Math.Round(d, 2);
        }

        /// <summary>
        /// decimal 转货币型字符串，保留两位小数，舍去后面的数值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToMoneyString(this decimal d)
        {
            d = decimal.Floor(d * 100) / 100;
            string s = d.ToString("f2");
            return s;
        }

        #endregion
    }
}
