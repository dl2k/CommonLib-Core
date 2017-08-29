using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommonLib.Encrypt
{
    /// <summary>
    /// AES加密帮助类
    /// </summary>
    public class AESEncodeHelper : IEncryptManager
    {
        #region 秘钥对
        /// <summary>
        /// 秘钥
        /// </summary>
        private string _Key { get; set; }
        /// <summary>
        /// 盐
        /// </summary>
        private string _Vector { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AESEncodeHelper()
        {
            _Key = "94bank@AES";
            _Vector = "AES@94bank";
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">秘钥</param>
        /// <param name="vector">盐</param>
        public AESEncodeHelper(string key, string vector)
        {
            _Key = key;
            _Vector = vector;
        }
        #endregion

        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <returns>加密后的数据</returns>
        public string EncryptData(string data)
        {
            Byte[] Cryptograph = null;
            try
            {
                Byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                Byte[] bKey = new Byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(_Key.PadRight(bKey.Length)), bKey, bKey.Length);
                Byte[] bVector = new Byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(_Vector.PadRight(bVector.Length)), bVector, bVector.Length);
                Rijndael Aes = Rijndael.Create();

                using (MemoryStream Memory = new MemoryStream())
                {
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                     Aes.CreateEncryptor(bKey, bVector),
                     CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流  
                        Encryptor.Write(plainBytes, 0, plainBytes.Length);
                        Encryptor.FlushFinalBlock();
                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("加密失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return Convert.ToBase64String(Cryptograph);
        }

        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <returns>解密后的数据</returns>
        public string DecryptData(string data)
        {
            Byte[] original = null;
            try
            {
                Byte[] encryptedBytes = Convert.FromBase64String(data);
                Byte[] bKey = new Byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(_Key.PadRight(bKey.Length)), bKey, bKey.Length);
                Byte[] bVector = new Byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(_Vector.PadRight(bVector.Length)), bVector, bVector.Length);
                Rijndael Aes = Rijndael.Create();
                using (MemoryStream Memory = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream Decryptor = new CryptoStream(Memory,
                    Aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                    {
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            Byte[] Buffer = new Byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }
                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("解密失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return Encoding.UTF8.GetString(original);
        }
    }
}
