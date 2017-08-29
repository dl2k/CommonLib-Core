using CommonLib.RDBS;
using CommonLib.Utils;
using MySqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Logger.Info("Hello World!");

            AdminUser au = null;
            using (SqlSugarClient db = MySqlHelper.GetInstance())
            {
                au = db.Queryable<AdminUser>().Where(k => k.IsDeleted == false && k.UserName == "danielliu").FirstOrDefault();

            }

            Console.ReadKey();
        }
    }



    public class AdminUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 加密密码
        /// </summary>
        public String PassCode { get; set; }

        /// <summary>
        /// 微信openid
        /// </summary>
        public String OpenId { get; set; }

        /// <summary>
        /// 权限 json数据  角色数组
        /// </summary>
        public String Role { get; set; }

        /// <summary>
        /// 状态 0 初始 1 锁定
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 注册ip
        /// </summary>
        public string RegIp { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
