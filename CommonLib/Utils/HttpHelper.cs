using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace CommonLib.Utils
{
    public static class HttpHelper
    {
        /// <summary>
        /// 用户代理
        /// </summary>
        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";

        private static readonly string ContentType = "application/x-www-form-urlencoded";

        /// <summary>
        /// http get 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="headers">header信息</param>
        /// <param name="paramValues">参数键值对</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <param name="statusCode">响应状态</param>
        /// <param name="statusMessage">响应描述</param>
        /// <returns>请求结果</returns>
        public static string httpGet(string url, Dictionary<string, string> headers, Dictionary<string, string> paramValues, string encoding, int readTimeoutMs, out HttpStatusCode statusCode, out string statusMessage, string contentType = null)
        {
            string encodedContent = encodingParam(paramValues, encoding);

            url += (string.IsNullOrEmpty(encodedContent) ? "" : ("?" + encodedContent));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            if (string.IsNullOrEmpty(contentType))
                request.ContentType = ContentType + ";charset=" + encoding;
            else
                request.ContentType = contentType + ";charset=" + encoding;
            setHeaders(request, headers, encoding);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            statusCode = response.StatusCode;
            statusMessage = response.StatusDescription;
            if (response.StatusCode == HttpStatusCode.OK)
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return resp;
        }

        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="headers">header信息</param>
        /// <param name="paramValues">参数键值对</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <param name="statusCode">响应状态</param>
        /// <param name="statusMessage">响应描述</param>
        /// <returns>请求结果</returns>
        public static string httpPost(string url, Dictionary<string, string> headers, Dictionary<string, string> paramValues, string encoding, int readTimeoutMs, out HttpStatusCode statusCode, out string statusMessage, string contentType = null)
        {
            var encodedContent = encodingParam(paramValues, encoding);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            if (string.IsNullOrEmpty(contentType))
                request.ContentType = ContentType + ";charset=" + encoding;
            else
                request.ContentType = contentType + ";charset=" + encoding;
            setHeaders(request, headers, encoding);
            var postData = Encoding.GetEncoding(encoding).GetBytes(encodedContent);
            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            statusCode = response.StatusCode;
            statusMessage = response.StatusDescription;
            if (response.StatusCode == HttpStatusCode.OK)

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return resp;
        }

        /// <summary>
        /// Https Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="paramValues"></param>
        /// <param name="encoding"></param>
        /// <param name="readTimeoutMs"></param>
        /// <param name="statusCode">响应状态</param>
        /// <param name="statusMessage">响应描述</param>
        /// <returns></returns>
        public static string httpsPost(string url, Dictionary<string, string> headers, Dictionary<string, string> paramValues, string encoding, int readTimeoutMs, out HttpStatusCode statusCode, out string statusMessage, string contentType = null)
        {
            var encodedContent = encodingParam(paramValues, encoding);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            if (string.IsNullOrEmpty(contentType))
                request.ContentType = ContentType + ";charset=" + encoding;
            else
                request.ContentType = contentType + ";charset=" + encoding;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => { return true; };
            setHeaders(request, headers, encoding);
            var postData = Encoding.GetEncoding(encoding).GetBytes(encodedContent);
            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            statusCode = response.StatusCode;
            statusMessage = response.StatusDescription;
            if (response.StatusCode == HttpStatusCode.OK)

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return resp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <param name="readTimeoutMs"></param>
        /// <param name="statusCode">响应状态</param>
        /// <param name="statusMessage">响应描述</param>
        /// <returns></returns>
        public static string httpsPost(string url, Dictionary<string, string> headers, string data, string encoding, int readTimeoutMs, out HttpStatusCode statusCode, out string statusMessage, string contentType = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            if (string.IsNullOrEmpty(contentType))
                request.ContentType = ContentType + ";charset=" + encoding;
            else
                request.ContentType = contentType + ";charset=" + encoding;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => { return true; };
            setHeaders(request, headers, encoding);

            var postData = Encoding.GetEncoding(encoding).GetBytes(data);
            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            statusCode = response.StatusCode;
            statusMessage = response.StatusDescription;
            if (response.StatusCode == HttpStatusCode.OK)

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return resp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <param name="readTimeoutMs"></param>
        /// <param name="statusCode">响应状态</param>
        /// <param name="statusMessage">响应描述</param>
        /// <returns></returns>
        public static string httpsPost(string url, string data, string encoding, int readTimeoutMs, out HttpStatusCode statusCode, out string statusMessage, string contentType = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            if (string.IsNullOrEmpty(contentType))
                request.ContentType = ContentType + ";charset=" + encoding;
            else
                request.ContentType = contentType + ";charset=" + encoding;
            var postData = Encoding.GetEncoding(encoding).GetBytes(data);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => { return true; };
            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            statusCode = response.StatusCode;
            statusMessage = response.StatusDescription;
            if (response.StatusCode == HttpStatusCode.OK)

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return resp;
        }

        private static void setHeaders(HttpWebRequest request, Dictionary<string, string> headers, string encoding)
        {
            if (null != headers)
            {
                if (headers.ContainsKey("Accept"))
                {
                    request.Accept = headers["Accept"];
                    headers.Remove("Accept");
                }
                if (headers.ContainsKey("Connection"))
                {
                    request.Connection = headers["Connection"];
                    headers.Remove("Connection");
                }
                if (headers.ContainsKey("Content-Length"))
                {
                    long contentLength = 0;
                    long.TryParse(headers["Content-Length"], out contentLength);
                    request.ContentLength = contentLength;
                    headers.Remove("Content-Length");
                }
                if (headers.ContainsKey("Content-Type"))
                {
                    request.ContentType = headers["Content-Type"];
                    headers.Remove("Content-Type");
                }
                if (headers.ContainsKey("Expect"))
                {
                    request.Expect = headers["Expect"];
                    headers.Remove("Expect");
                }
                if (headers.ContainsKey("Date"))
                {
                    DateTime dt = CommonUtils.GetNow();
                    DateTime.TryParse(headers["Date"], out dt);
                    request.Date = dt;
                    headers.Remove("Date");
                }
                if (headers.ContainsKey("Host"))
                {
                    request.Host = headers["Host"];
                    headers.Remove("Host");
                }
                if (headers.ContainsKey("If-Modified-Since"))
                {
                    DateTime dt = CommonUtils.GetNow();
                    DateTime.TryParse(headers["If-Modified-Since"], out dt);
                    request.IfModifiedSince = dt;
                    headers.Remove("If-Modified-Since");
                }
                if (headers.ContainsKey("Range"))
                {
                    int range = 0;
                    int.TryParse(headers["Range"], out range);
                    request.AddRange(range);
                    headers.Remove("Range");
                }
                if (headers.ContainsKey("Referer"))
                {
                    request.Referer = headers["Referer"];
                    headers.Remove("Referer");
                }
                if (headers.ContainsKey("User-Agent"))
                {
                    request.UserAgent = headers["User-Agent"];
                    headers.Remove("User-Agent");
                }
                if (headers.ContainsKey("Transfer-Encoding"))
                {
                    request.TransferEncoding = headers["Transfer-Encoding"];
                    headers.Remove("Transfer-Encoding");
                }

                foreach (String key in headers.Keys)
                {
                    if (!WebHeaderCollection.IsRestricted(key))
                        request.Headers.Add(key, headers[key]);
                }
            }
        }

        private static string encodingParam(Dictionary<string, string> paramValues, string encoding)
        {
            StringBuilder sb = new StringBuilder();
            if (null == paramValues)
                return null;

            int i = 0;
            foreach (string key in paramValues.Keys)
            {
                sb.Append(key);
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(paramValues[key], Encoding.GetEncoding(encoding)));
                if (i++ != paramValues.Count)
                    sb.Append("&");
            }
            return sb.ToString();
        }

        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="data">提交的数据</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <param name="statusCode">响应状态</param>
        /// <param name="statusMessage">响应描述</param>
        /// <returns></returns>
        public static string httpPost(string url, string data, string encoding, int readTimeoutMs, out HttpStatusCode statusCode, out string statusMessage, string contentType = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            if (string.IsNullOrEmpty(contentType))
                request.ContentType = ContentType + ";charset=" + encoding;
            else
                request.ContentType = contentType + ";charset=" + encoding;

            var postData = Encoding.GetEncoding(encoding).GetBytes(data);
            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            statusCode = response.StatusCode;
            statusMessage = response.StatusDescription;
            if (response.StatusCode == HttpStatusCode.OK)

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return resp;
        }

        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="headers">header信息</param>
        /// <param name="data">参数</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <param name="statusCode">响应状态</param>
        /// <param name="statusMessage">响应描述</param>
        /// <returns>请求结果</returns>
        public static string httpPost(string url, Dictionary<string, string> headers, string data, string encoding, int readTimeoutMs, out HttpStatusCode statusCode, out string statusMessage, string contentType = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            if (string.IsNullOrEmpty(contentType))
                request.ContentType = ContentType + ";charset=" + encoding;
            else
                request.ContentType = contentType + ";charset=" + encoding;
            setHeaders(request, headers, encoding);

            var postData = Encoding.GetEncoding(encoding).GetBytes(data);
            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            statusCode = response.StatusCode;
            statusMessage = response.StatusDescription;
            if (response.StatusCode == HttpStatusCode.OK)

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return resp;
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
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeOut;
                request.UserAgent = UserAgent;
                request.Method = "HEAD";
                request.Referer = refer;
                request.CookieContainer = cc;
                HttpWebResponse httpWebResponse = request.GetResponse() as HttpWebResponse;
                if (httpWebResponse != null)
                    return httpWebResponse.StatusCode;
                return HttpStatusCode.ExpectationFailed;
            }
            catch
            {
                return HttpStatusCode.ExpectationFailed;
            }
        }
    }

}
