using CommonLib.Cache;
using CommonLib.Configuration;
using CommonLib.IO;
using CommonLib.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CommonLib.xHttp
{
    public delegate string IHandler(SortedDictionary<string, string> dic, string parnterid);

    public interface IApiHandler
    {
        Dictionary<string, IHandler> Dic { get; }
        int GetTimeout(string method);
    }

    public class xHttpApiHandler
    {
        static string NS = "xHttpApiHandler";

        Dictionary<string, ParnterInfo> PDic { get; set; }
        IApiHandler Handler { get; set; }

        public static xHttpApiHandler Init(string configfile, IApiHandler handle)
        {
            if (string.IsNullOrEmpty(configfile))
                throw new
                Exception("configfile 不能为空");

            if (handle == null)
                throw new
                Exception("handle 不能为空");

            string confile = PathHelper.MergePathName(PathHelper.GetConfigPath(), configfile);
            ApiConfig config = ConfigManager.GetObjectConfig<ApiConfig>(confile);

            Dictionary<string, ParnterInfo> d = new Dictionary<string, ParnterInfo>();

            foreach (ParnterInfo pi in config.Parnter)
                d.Add(pi.ParnterKey, pi);

            return new xHttpApiHandler() { PDic = d, Handler = handle };
        }

        private static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            //Logger.Info(sBuilder.ToString());
            return sBuilder.ToString();
        }

        //MD5验签
        private static bool checkSignMD5(SortedDictionary<string, string> sParaTemp, String md5_key)
        {

            if (sParaTemp == null)
            {
                return false;
            }
            String sign;
            if (!sParaTemp.TryGetValue("sign", out sign))
            {
                return false;
            }

            // 生成签名原串
            String sign_src = genSignData(sParaTemp);

            sign_src += "&key=" + md5_key;
            try
            {
                return sign.Equals(getMd5Hash(sign_src));
            }
            catch (Exception e)
            {
                return false;
            }
        }

        // 生成签名原串
        public static String genSignData(SortedDictionary<string, string> sParaTemp)
        {
            StringBuilder content = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in sParaTemp)
            {
                //"sign"不参与签名
                if ("sign".Equals(temp.Key))
                {
                    continue;
                }
                // 空串不参与签名
                if (string.IsNullOrEmpty(temp.Value))
                {
                    continue;
                }
                content.Append("&" + temp.Key + "=" + temp.Value);
            }
            String signSrc = content.ToString();
            if (signSrc.StartsWith("&"))
            {
                signSrc = signSrc.Substring(1);
            }
            return signSrc;
        }

        public string Handle(Stream inputstream, string query)
        {
            string limitkey = "";
            int timeout = 0;
            try
            {
                String line = null;
                using (StreamReader reader = new StreamReader(inputstream, Encoding.UTF8))
                    line = reader.ReadToEnd();

                string postcontent = line;
                string getcontent = query.Length > 0 ? query.Remove(0, 1) : "";

                Logger.Info(@"xHttpApiHandler.Handle {0} {1}", postcontent, getcontent);

                var ss = postcontent.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
.Select(part => part.Split('='))
.ToDictionary(split => split[0], split => HttpUtility.UrlDecode(split[1]));

                SortedDictionary<string, string> sArray = new SortedDictionary<string, string>(ss);

                Logger.Info(JsonConvert.SerializeObject(sArray));

                if (!sArray.ContainsKey("parnterkey")) return xHttpApiErrorJson.Create(-2, "商户KEY不能为空").ToJson();
                string pk = sArray["parnterkey"].Trim();
                if (string.IsNullOrEmpty(pk))
                    return xHttpApiErrorJson.Create(-2, "商户KEY不能为空").ToJson();

                if (!PDic.ContainsKey(pk))
                    return xHttpApiErrorJson.Create(-2, "商户KEY不存在").ToJson();
                ParnterInfo pi = PDic[pk];

                if (!sArray.ContainsKey("method")) return xHttpApiErrorJson.Create(-2, "method字段不能为空").ToJson();
                string method = sArray["method"].Trim().ToLower();
                if (string.IsNullOrEmpty(method))
                    return xHttpApiErrorJson.Create(-2, "method字段不能为空").ToJson();
                if (!Handler.Dic.ContainsKey(method))
                    return xHttpApiErrorJson.Create(-2, "method不存在").ToJson();

                limitkey = string.Format("{0}_{1}", pk, method);
                timeout = Handler.GetTimeout(method);

                string v = CacheManager.GetRedisCache(NS, limitkey);

                if (!string.IsNullOrEmpty(v))
                    return xHttpApiErrorJson.Create(-1, "调用过于频繁").ToJson();

                if (!checkSignMD5(sArray, pi.ParnterSecert))
                    return xHttpApiErrorJson.Create(-10, "校验失败").ToJson();

                return Handler.Dic[method].Invoke(sArray, pi.ParnterId);
            }
            catch (Exception ex)
            {
                Logger.Error("xHttpApiHandler.Handle", ex);
                return xHttpApiErrorJson.Create(-99, "接口异常").ToJson();
            }
            finally
            {
                if (!string.IsNullOrEmpty(limitkey))
                    CacheManager.PutRedisCache(NS, limitkey, limitkey, timeout);
            }
        }
    }

    public class xHttpApiJson
    {
        public int Code { get; set; }
        public string Data { get; set; }

        public static xHttpApiJson Create()
        {
            return new xHttpApiJson() { Code = 0, Data = "ok" };
        }

        public static xHttpApiJson Create(string data)
        {
            return new xHttpApiJson() { Code = 0, Data = data };
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static xHttpApiJson Create(object p)
        {
            return new xHttpApiJson() { Code = 0, Data = JsonConvert.SerializeObject(p) };
        }
    }

    public class xHttpApiErrorJson
    {
        public int Code { get; set; }
        public string ErrMessage { get; set; }

        public static xHttpApiErrorJson Create()
        {
            return new xHttpApiErrorJson() { Code = -1, ErrMessage = "error" };
        }

        public static xHttpApiErrorJson Create(int code, string msg)
        {
            return new xHttpApiErrorJson() { Code = code, ErrMessage = msg };
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    class ApiConfig
    {
        [Node("Parnters/Parnter", NodeAttribute.NodeType.List)]
        public List<ParnterInfo> Parnter { get; set; }
    }

    class ParnterInfo
    {
        [Node]
        public string ParnterId { get; set; }

        [Node]
        public string ParnterKey { get; set; }

        [Node]
        public string ParnterSecert { get; set; }

    }

}
