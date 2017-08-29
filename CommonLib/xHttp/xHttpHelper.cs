using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using CommonLib.Utils;

namespace CommonLib.xHttp
{
    /// <summary>
    /// http帮助类
    /// </summary>
    public class xHttpHelper
    {
        /// <summary>
        /// 用户代理
        /// </summary>
        public static readonly string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";

        /// <summary>
        /// http get 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="headers">header信息</param>
        /// <param name="paramValues">参数键值对</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <returns>请求结果</returns>
        public static HttpResult httpGet(string url, Dictionary<string, string> headers, Dictionary<string, string> paramValues, string encoding, int readTimeoutMs)
        {
            HttpStatusCode statusCode;
            string statusMsg;
            string resp = HttpHelper.httpGet(url, headers, paramValues, encoding, readTimeoutMs, out statusCode, out statusMsg);
            return new HttpResult(statusCode, resp);
        }

        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="headers">header信息</param>
        /// <param name="paramValues">参数键值对</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <returns>请求结果</returns>
        public static HttpResult httpPost(string url, Dictionary<string, string> headers, Dictionary<string, string> paramValues, string encoding, int readTimeoutMs)
        {
            HttpStatusCode statusCode;
            string statusMsg;
            string resp = HttpHelper.httpPost(url, headers, paramValues, encoding, readTimeoutMs, out statusCode, out statusMsg);
            return new HttpResult(statusCode, resp);
        }

        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="data">提交的数据</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <returns></returns>
        public static HttpResult httpPost(string url, string data, string encoding, int readTimeoutMs)
        {
            HttpStatusCode statusCode;
            string statusMsg;
            string resp = HttpHelper.httpPost(url, data, encoding, readTimeoutMs, out statusCode, out statusMsg);
            return new HttpResult(statusCode, resp);
        }

        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="headers">header信息</param>
        /// <param name="data">参数</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <returns>请求结果</returns>
        public static HttpResult httpPost(string url, Dictionary<string, string> headers, string data, string encoding, int readTimeoutMs)
        {
            HttpStatusCode statusCode;
            string statusMsg;
            string resp = HttpHelper.httpPost(url, headers, data, encoding, readTimeoutMs, out statusCode, out statusMsg);
            return new HttpResult(statusCode, resp);
        }

        /// <summary>
        /// 获取http状态枚举
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="timeOut"></param>
        /// <param name="cc">cookie</param>
        /// <param name="refer">返回链接</param>
        /// <returns></returns>
        public static HttpStatusCode HeadHttpCode(string url, int timeOut = 5, CookieContainer cc = null, string refer = null)
        {
            return HttpHelper.HeadHttpCode(url, timeOut, cc, refer);
        }
    }

    /// <summary>
    /// http响应封装
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">返回状态码</param>
        /// <param name="response">返回数据</param>
        public HttpResult(HttpStatusCode code, string response)
        {
            this.code = (int)code;
            this.content = response;
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public string content { get; set; }
    }
}
