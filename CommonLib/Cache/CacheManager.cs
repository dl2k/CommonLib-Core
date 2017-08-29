using ServiceStack.Redis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using CommonLib.Configuration;
using CommonLib.Redis;
using Newtonsoft.Json;
using CommonLib.ExtensionMethod;
using CommonLib.Utils;

namespace CommonLib.Cache
{
    /// <summary>
    /// 缓存相关操作
    /// </summary>
    public class CacheManager
    {
        /// <summary>
        /// Formatting
        /// </summary>
        const string formatting = "[{0}]:[{1}]";
        /// <summary>
        /// FormattingV
        /// </summary>
        const string formattingV = "[{0}]:[{1}]V";
        /// <summary>
        /// FormattingPage
        /// </summary>
        const string FormattingPage = "[{0}]:[{1}]:[{2}]";

        ///// <summary>
        ///// 插入web环境缓存
        ///// </summary>
        ///// <param name="namespaces">命名空间</param>
        ///// <param name="key">缓存key</param>
        ///// <param name="value">要插入的数据</param>
        ///// <param name="ttl">过期时间</param>
        //public static void PutWebCache(string namespaces, string key, string value, int ttl = 10)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
        //        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
        //        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
        //        key = string.Format(formatting, namespaces, key);

        //        HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, ttl));//
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("web环境插入缓存失败，原因：{0} {1}", ex.Message, ex.StackTrace));
        //    }
        //}

        ///// <summary>
        ///// 获取缓存
        ///// </summary>
        ///// <param name="namespaces">命名空间</param>
        ///// <param name="key">key</param>
        ///// <returns>缓存数据</returns>
        //public static string GetWebCache(string namespaces, string key)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
        //        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
        //        key = string.Format(formatting, namespaces, key);
        //        return (string)HttpRuntime.Cache.Get(key);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("web环境获取缓存失败，原因：{0} {1}", ex.Message, ex.StackTrace));
        //    }
        //}

        ///// <summary>
        ///// web环境删除缓存
        ///// </summary>
        ///// <param name="namespaces">命名空间</param>
        ///// <param name="key">缓存key</param>
        ///// <returns></returns>
        //public static object RemoveWebCache(string namespaces, string key)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
        //        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
        //        key = string.Format(formatting, namespaces, key);
        //        return HttpRuntime.Cache.Remove(key);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("web环境删除缓存失败，原因：{0} {1}", ex.Message, ex.StackTrace));
        //    }
        //}

        /// <summary>
        /// 插入缓存到Redis服务器
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <param name="value">要插入的数据</param>
        /// <param name="ttl">过期时间(秒)</param>
        public static void PutRedisCache(string namespaces, string key, string value, int ttl = 10, long db = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient(db))
                {
                    if (ttl > 0)
                        client.SetValue(key, value, new TimeSpan(0, 0, ttl));
                    else
                        client.SetValue(key, value);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("插入缓存到Redis服务器失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 获取Redis中的缓存数据
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存数据</returns>
        public static string GetRedisCache(string namespaces, string key, long db = 0)
        {
            try
            {
                string result = string.Empty;
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient(db))
                {
                    result = client.GetValue(key);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取Redis中的缓存数据失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 获取Redis中的缓存数据
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存数据</returns>
        public static T GetRedisCache<T>(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                string value = RedisHelper.GetValue(key);
                if (string.IsNullOrWhiteSpace(value))
                    return default(T);
                else
                    return JsonHelper.JsonDeserialize<T>(value);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取Redis中的缓存数据失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        public static void PutRedisCache<T>(string namespaces, string key, T value, int ttl = 10, long db = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                string v = value.Equals(default(T)) ? "" : JsonHelper.JsonSerializer<T>(value);
                if (value == null) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient(db))
                {
                    if (ttl > 0)
                        client.SetValue(key, v, new TimeSpan(0, 0, ttl));
                    else
                        client.SetValue(key, v);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("插入缓存到Redis服务器失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }


        /// <summary>
        /// 删除redis中的缓存数据
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public static bool RemoveRedisCache(string namespaces, string key, long db = 0)
        {
            try
            {
                bool result = false;
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient(db))
                {
                    result = client.Remove(key);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("删除Redis中的缓存数据失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="namespaces"></param>
        /// <param name="ids"></param>
        /// <param name="makeIds"></param>
        /// <param name="getCacheObject"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static List<T> GetRedisCacheObjectList<T>(string namespaces, List<string> ids, Func<T, string> makeIds, Func<List<string>, List<T>> getCacheObject, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (ids == null || ids.Count <= 0) throw new ArgumentNullException();
                List<string> strList;
                List<string> cacheKeys = new List<string>();//拼接RedisKey
                ids.ForEach(s =>
                {
                    cacheKeys.Add(string.Format(formatting, namespaces, s));
                });
                strList = RedisHelper.GetValues(cacheKeys);//获取RedisObject
                List<T> resultObj = new List<T>();
                Dictionary<string, T> resultDic = new Dictionary<string, T>();
                strList.ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        T temp = JsonConvert.DeserializeObject<T>(s);
                        if (temp != null)
                        {
                            resultObj.Add(temp);
                            string skey = makeIds(temp);
                            if (!string.IsNullOrEmpty(skey))
                                resultDic[skey] = temp;
                        }
                    }
                });
                List<string> resultIds = null;
                if (makeIds != null)//拼接结果集的ID
                {
                    resultIds = resultObj.Select(makeIds).ToList();
                }
                List<string> otherIds = ids.Except(resultIds).ToList();//原始ID和结果集ID取差集
                if (otherIds != null && otherIds.Count != 0)
                {
                    if (getCacheObject != null)
                    {
                        List<T> otherObject = getCacheObject(otherIds);//差集从数据库取对象
                        Dictionary<string, string> dicOtherObjectJson = new Dictionary<string, string>();
                        if (otherObject != null && otherObject.Count > 0)
                        {
                            otherObject.ForEach(o =>
                            {
                                string json = JsonConvert.SerializeObject(o);
                                string sid = makeIds(o);
                                string key = string.Format(formatting, namespaces, sid);
                                strList.Add(json);
                                dicOtherObjectJson[key] = json;
                                if (!string.IsNullOrEmpty(sid))
                                    resultDic[sid] = o;
                            });
                            using (RedisClient client = RedisHelper.GetClient())
                            {
                                foreach (var key in dicOtherObjectJson.Keys)
                                {
                                    string objJson = dicOtherObjectJson[key];
                                    client.SetValue(key, objJson, new TimeSpan(0, 0, ttl));
                                }
                            }
                        }
                    }
                }
                List<T> result = new List<T>();
                foreach (var ID in ids)
                {
                    if (resultDic.ContainsKey(ID))
                        result.Add(resultDic[ID]);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取Redis中的缓存数据失败，原因：{0} {1}", ex.Message, ex.StackTrace));

            }
        }

        /// <summary>
        /// 插入缓存流到Redis中
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <param name="stream">要插入的数据流</param>
        /// <param name="ttl">过期时间</param>
        public static void PutRedisStreamCache(string namespaces, string key, byte[] stream, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (stream.Length <= 0) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                RedisHelper.Set(key, stream, new TimeSpan(0, 0, ttl));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("插入缓存流到Redis中失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 获取Redis中的缓存流
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存流</returns>
        public static byte[] GetRedisStreamCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                return RedisHelper.Get(key);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取Redis中的缓存流失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 删除Redis中的缓存流
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存流</returns>
        public static bool RemoveRedisStreamCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                return RedisHelper.Remove(key);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("删除Redis中的缓存流失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        public static ICache GetCache(string namespaces)
        {
            if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
            return new CacheItem(namespaces);
        }
    }

    public interface ICache
    {
        //void PutWebCache(string key, string value, int ttl = 10);
        //string GetWebCache(string key);
        void PutRedisCache(string key, string value, int ttl = 10);
        string GetRedisCache(string key);
    }

    class CacheItem : ICache
    {
        string ns = "";

        public CacheItem(string namespaces)
        {
            ns = namespaces;
        }

        //public void PutWebCache(string key, string value, int ttl = 10)
        //{
        //    if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
        //    if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
        //    CacheManager.PutWebCache(ns, key, value, ttl);
        //}

        //public string GetWebCache(string key)
        //{
        //    if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
        //    return CacheManager.GetWebCache(ns, key);
        //}

        public void PutRedisCache(string key, string value, int ttl = 10)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
            CacheManager.PutRedisCache(ns, key, value, ttl);
        }

        public string GetRedisCache(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            return CacheManager.GetRedisCache(ns, key);
        }

        public void PutCache(string namespaces, string key, string value, int ttl = 60)
        {
            if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();

            //CacheManager.PutWebCache(namespaces, key, value, ttl);
            CacheManager.PutRedisCache(namespaces, key, value, ttl);
        }

        public string GetCache(string namespaces, string key)
        {
            //string value = CacheManager.GetWebCache(namespaces, key);
            //if (string.IsNullOrEmpty(value))
            string value = CacheManager.GetRedisCache(namespaces, key);
            //if (!string.IsNullOrEmpty(value))
            //    CacheManager.PutWebCache(namespaces, key, value);
            return value;
        }
    }
}