using System;
using System.IO;
using System.Net;
using System.Text;
using CommonLib.Utils;
using System.Collections;
using System.Threading.Tasks;
using Renci.SshNet;
using CommonLib.IO;

namespace CommonLib.xHttp
{
    /// <summary>
    /// FTP帮助类
    /// </summary>
    public class xFtpHelper
    {
        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filestream">文件流</param>
        /// <param name="filename">文件名</param>
        /// <param name="ftpPath">文件路径</param>
        /// <param name="ftpUser">FTP服务器用户名</param>
        /// <param name="ftpPassword">FTP服务器用户密码</param>
        public static void UpLoadFile(byte[] filestream, string filename, string ftpPath, string ftpUser, string ftpPassword)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename is empty");

            if (ftpUser == null)
            {
                ftpUser = "";
            }
            if (ftpPassword == null)
            {
                ftpPassword = "";
            }

            //if (!File.Exists(localFile))
            //{
            //    //MyLog.ShowMessage("文件：“" + localFile + "” 不存在！");
            //    return;
            //}

            FtpWebRequest ftpWebRequest = null;
            MemoryStream localFileStream = null;
            Stream requestStream = null;
            try
            {
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(PathHelper.MergeUrl(ftpPath, filename));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpWebRequest.ContentLength = filestream.Length;
                int buffLength = 4096;
                byte[] buff = new byte[buffLength];
                int contentLen;
                //localFileStream = new FileInfo(localFile).OpenRead();
                localFileStream = new MemoryStream(filestream);
                requestStream = ftpWebRequest.GetRequestStream();
                contentLen = localFileStream.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    requestStream.Write(buff, 0, contentLen);
                    contentLen = localFileStream.Read(buff, 0, buffLength);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ftp upload failed", ex);
                throw ex;
                //MyLog.ShowMessage(ex.StackTrace, "FileUpLoad0001");
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }
                if (localFileStream != null)
                {
                    localFileStream.Close();
                }
            }
        }
        #endregion

        #region 文件夹管理

        /// <summary>
        /// 新建目录
        /// </summary>
        /// <param name="ftpPath">FTP服务器地址</param>
        /// <param name="dirName">目录名</param>
        /// <param name="ftpUser">FTP服务器用户名</param>
        /// <param name="ftpPassword">FTP服务器用户密码</param>
        public static void MakeDir(string ftpPath, string dirName, string ftpUser, string ftpPassword)
        {
            try
            {
                //实例化FTP
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(PathHelper.MergeUrl(ftpPath, dirName));

                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                //指定FTP操作类型为创建目录
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                //获取FTP服务器的响应
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                //Logger.Error("ftp upload failed", ex);
                //throw ex;
                //MyLog.ShowMessage(ex.StackTrace, "MakeDir");
            }
        }

        /// <summary>
        /// 检查目录是否存在
        /// </summary>
        /// <param name="ftpPath">FTP服务器地址</param>
        /// <param name="dirName">目录名</param>
        /// <param name="ftpUser">FTP服务器用户名</param>
        /// <param name="ftpPassword">FTP服务器用户密码</param>
        /// <returns></returns>
        public static bool CheckDirectoryExist(string ftpPath, string dirName, string ftpUser, string ftpPassword)
        {
            bool result = false;
            try
            {
                //实例化FTP连接
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));
                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                //指定FTP操作类型为创建目录
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                //获取FTP服务器的响应
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);

                StringBuilder str = new StringBuilder();
                string line = sr.ReadLine();
                while (line != null)
                {
                    str.Append(line);
                    str.Append("|");
                    line = sr.ReadLine();
                }
                string[] datas = str.ToString().Split('|');

                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i].Contains("<DIR>"))
                    {
                        int index = datas[i].IndexOf("<DIR>");
                        string name = datas[i].Substring(index + 5).Trim();
                        if (name == dirName)
                        {
                            result = true;
                            break;
                        }
                    }
                }

                sr.Close();
                sr.Dispose();
                response.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("ftp upload failed", ex);
                throw ex;
            }
            return result;
        }

        #endregion
    }


    public class SFTPHelper
    {
        public static string GetFile(string path, string ip, string port, string user, string pwd)
        {
            string c = "";
            byte[] byt = new byte[0];

            using (SftpClient client = new SftpClient(ip, Int32.Parse(port), user, pwd))
            {
                client.Connect();
                byt = client.ReadAllBytes(path);
            }

            using (StreamReader reader = new StreamReader(new MemoryStream(byt), Encoding.UTF8))
            {
                c = reader.ReadToEnd();
            }

            return c;
        }
    }
    /// <summary>  
    /// SFTP操作类  
    /// </summary>  
    public class SFTPOperation
    {
        #region 字段或属性
        private SftpClient sftp;
        /// <summary>  
        /// SFTP连接状态  
        /// </summary>  
        public bool Connected { get { return sftp.IsConnected; } }
        #endregion

        #region 构造
        /// <summary>  
        /// 构造  
        /// </summary>  
        /// <param name="ip">IP</param>  
        /// <param name="port">端口</param>  
        /// <param name="user">用户名</param>  
        /// <param name="pwd">密码</param>  
        public SFTPOperation(string ip, string port, string user, string pwd)
        {
            sftp = new SftpClient(ip, Int32.Parse(port), user, pwd);
        }
        #endregion

        #region 连接SFTP
        /// <summary>  
        /// 连接SFTP  
        /// </summary>  
        /// <returns>true成功</returns>  
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    sftp.Connect();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("连接SFTP失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region 断开SFTP
        /// <summary>  
        /// 断开SFTP  
        /// </summary>   
        public void Disconnect()
        {
            try
            {
                if (sftp != null && Connected)
                {
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("断开SFTP失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region SFTP上传文件
        /// <summary>  
        /// SFTP上传文件  
        /// </summary>  
        /// <param name="localPath">本地路径</param>  
        /// <param name="remotePath">远程路径</param>  
        public void Put(string localPath, string remotePath)
        {
            try
            {
                using (var file = File.OpenRead(localPath))
                {
                    Connect();
                    sftp.UploadFile(file, remotePath);
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP文件上传失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region SFTP获取文件
        /// <summary>  
        /// SFTP获取文件  
        /// </summary>  
        /// <param name="remotePath">远程路径</param>  
        /// <param name="localPath">本地路径</param>  
        public void Get(string remotePath, string localPath)
        {
            try
            {
                Connect();
                var byt = sftp.ReadAllBytes(remotePath);
                Disconnect();
                File.WriteAllBytes(localPath, byt);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP文件获取失败，原因：{0}", ex.Message));
            }

        }
        #endregion

        #region 删除SFTP文件
        /// <summary>  
        /// 删除SFTP文件   
        /// </summary>  
        /// <param name="remoteFile">远程路径</param>  
        public void Delete(string remoteFile)
        {
            try
            {
                Connect();
                sftp.Delete(remoteFile);
                Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP文件删除失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region 获取SFTP文件列表
        /// <summary>  
        /// 获取SFTP文件列表  
        /// </summary>  
        /// <param name="remotePath">远程目录</param>  
        /// <param name="fileSuffix">文件后缀</param>  
        /// <returns></returns>  
        public ArrayList GetFileList(string remotePath, string fileSuffix)
        {
            try
            {
                Connect();
                var files = sftp.ListDirectory(remotePath);
                Disconnect();
                var objList = new ArrayList();
                foreach (var file in files)
                {
                    string name = file.Name;
                    if (name.Length > (fileSuffix.Length + 1) && fileSuffix == name.Substring(name.Length - fileSuffix.Length))
                    {
                        objList.Add(name);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP文件列表获取失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region 移动SFTP文件
        /// <summary>  
        /// 移动SFTP文件  
        /// </summary>  
        /// <param name="oldRemotePath">旧远程路径</param>  
        /// <param name="newRemotePath">新远程路径</param>  
        public void Move(string oldRemotePath, string newRemotePath)
        {
            try
            {
                Connect();
                sftp.RenameFile(oldRemotePath, newRemotePath);
                Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SFTP文件移动失败，原因：{0}", ex.Message));
            }
        }
        #endregion

    }

}