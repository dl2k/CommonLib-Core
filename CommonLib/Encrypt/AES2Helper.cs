using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CommonLib.Encrypt
{
    class AES2Helper : IEncryptManager
    {
        #region 秘钥对
        /// <summary>
        /// 秘钥
        /// </summary>
        private string _Key { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AES2Helper()
        {
            _Key = "commonlib@AES256";
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">秘钥</param>
        /// <param name="vector">盐</param>
        public AES2Helper(string key)
        {
            _Key = key;// key.PadRight(16).Substring(0, 16);
        }
        #endregion
        public string EncryptData(string data)
        {
            Byte[] Cryptograph = null;
            try
            {            // 256-AES key    
                byte[] keyArray = GetKey(_Key);// UTF8Encoding.UTF8.GetBytes(_Key);
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(data);

                byte[] iv = new byte[16]; //UTF8Encoding.UTF8.GetBytes("0000000000000000");// new byte[16];
                for (int i = 0; i < 16; i++)
                    iv[i] = 0;

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.PKCS7;
                //rDel.KeySize = 256;
                //rDel.BlockSize = 128;
                rDel.Key = keyArray;
                rDel.IV = iv;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                Cryptograph = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("加密失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return Convert.ToBase64String(Cryptograph, 0, Cryptograph.Length);
        }

        private byte[] GetKey(string _Key)
        {
            byte[] b = new byte[32];

            byte[] bb = UTF8Encoding.UTF8.GetBytes(_Key);
            int c = bb.Length;


            for (int i = 0; i < 32; i++)
            {
                if (i < c)
                    b[i] = bb[i];
                else
                    b[i] = 0;
            }

            return b;
        }

        public string DecryptData(string data)
        {
            Byte[] original = null;
            try
            {
                // 256-AES key    
                byte[] keyArray = GetKey(_Key);//UTF8Encoding.UTF8.GetBytes(_Key);
                byte[] toEncryptArray = Convert.FromBase64String(data);

                byte[] iv = new byte[16]; //UTF8Encoding.UTF8.GetBytes("0000000000000000");// new byte[16];
                for (int i = 0; i < 16; i++)
                    iv[i] = 0;

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                //rDel.KeySize = 256;
                //rDel.BlockSize = 128;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.PKCS7;
                rDel.IV = iv;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                original = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("解密失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return Encoding.UTF8.GetString(original);
        }
    }
}
