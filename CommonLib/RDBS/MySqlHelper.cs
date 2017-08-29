using CommonLib.Utils;
using MySqlSugar;
using CommonLib.Configuration;
using CommonLib.Encrypt;
using System.Collections.Generic;
using System;
using System.Linq;
using CommonLib.IO;

namespace CommonLib.RDBS
{
    public class MySqlHelper
    {
        public static SqlSugarClient GetInstance(string connName = "default")
        {
            // 设置连接字符串
            try
            {
                string connFile = PathHelper.MergePathName(PathHelper.GetConfigPath(), "MYSQLConn.config");
                SQLConnList conns = ConfigManager.GetObjectConfig<SQLConnList>(connFile);
                string connFormat = "Server={0};Port={4};Database={1};Uid={2};Pwd={3};SslMode=none;";
                List<SQLConnConfig> connList = conns.ConnList.Where(x => x.ConnName == connName).Select(x => x).ToList();
                if (connList != null && connList.Count > 0)
                {
                    SQLConnConfig conf = connList[0];
                    IEncryptManager em = EncryptFactory.CreateEncryptManager(EncryptVersion.AES, "SQL@CONN", "CONN@SQL");
                    if (em != null)
                    {
                        string cs = string.Format(connFormat, conf.Server, conf.DataBase, conf.UID, em.DecryptData(conf.Pwd), conf.Port);

                        var db = new SqlSugarClient(cs);
                        db.IsEnableAttributeMapping = true;
                        db.IsEnableLogEvent = true;//Enable log events
                        db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par + "\r\n"); };
                        return db;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("创建数据库链接失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        public static void exec(Func<SqlSugarClient, object> p)
        {
            using (var db = GetInstance("default"))
            {
                p.Invoke(db);
            }
        }

        /// <summary>
        /// 数据库连接列表
        /// </summary>
        class SQLConnList
        {
            /// <summary>
            /// 数据库连接列表
            /// </summary>
            [Node("Conns/Conn", NodeAttribute.NodeType.List)]
            public List<SQLConnConfig> ConnList { get; set; }
        }

        /// <summary>
        /// 数据库连接字符串配置
        /// </summary>
        class SQLConnConfig
        {
            /// <summary>
            /// 连接标识
            /// </summary>
            [Node]
            public string ConnName { get; set; }

            /// <summary>
            /// 数据库地址
            /// </summary>
            [Node]
            public string Server { get; set; }

            /// <summary>
            /// 数据库名称
            /// </summary>
            [Node]
            public string DataBase { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            [Node]
            public string UID { get; set; }

            /// <summary>
            /// 用户密码
            /// </summary>
            [Node]
            public string Pwd { get; set; }

            /// <summary>
            /// 数据库端口
            /// </summary>
            [Node]
            public string Port { get; set; }
        }
    }
}
