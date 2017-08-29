using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonLib.Encrypt
{
    /// <summary>
    /// 加解密类型接口
    /// </summary>
    public interface IEncryptManager
    {
        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <returns>加密后的数据</returns>
        string EncryptData(string data);

        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <returns>解密后的数据</returns>
        string DecryptData(string data);
    }

    /// <summary>
    /// 加解密工厂
    /// </summary>
    public class EncryptFactory
    {
        /// <summary>
        /// 加解密版本配置字典
        /// </summary>
        private static Dictionary<EncryptVersion, string> _EncryptVerDic = new Dictionary<EncryptVersion, string>()
        {
            {EncryptVersion.AES,"CommonLib.Encrypt.AESEncodeHelper, CommonLib"},
            {EncryptVersion.AES2,"CommonLib.Encrypt.AES2Helper, CommonLib"}

        };

        /// <summary>
        /// 创建加解密版本类型对象
        /// </summary>
        /// <param name="ver">加解密版本</param>
        /// <param name="args">加解密类型构造函数所需要的参数数组</param>
        /// <returns>加解密类型对象</returns>
        public static IEncryptManager CreateEncryptManager(EncryptVersion ver, params object[] args)
        {
            if (_EncryptVerDic.ContainsKey(ver))
            {
                Type type = Type.GetType(_EncryptVerDic[ver]);
                if (type.IsInterface || !type.GetInterfaces().Contains(typeof(IEncryptManager)))
                    return null;

                IEncryptManager manager = Activator.CreateInstance(type, args) as IEncryptManager;
                return manager;
            }

            return null;
        }
    }

    /// <summary>
    /// 加解密版本枚举
    /// </summary>
    public enum EncryptVersion
    {
        /// <summary>
        /// AES加解密 
        /// </summary>
        AES = 2,

        /// <summary>
        /// AES2加解密 
        /// </summary>
        AES2 = 3,
    }
}
