using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib.ExtensionMethod;
using CommonLib.Redis;

namespace CommonLib.TLock
{
    public interface ILockContrain
    {
        bool ContainsKey(string key);
        string this[string index] { get; set; }
    }

    public enum LockerType
    {
        MemLocker,
        RedisLocker,
    }

    class RedisLocker : ILockContrain
    {
        const string hashKey = "RedisDistributedLocker";

        public bool ContainsKey(string key)
        {
            return RedisHelper.HashContainsEntry(hashKey, key);
        }

        public string this[string key]
        {
            get
            {
                return RedisHelper.GetValueFromHash(hashKey, key);
            }
            set
            {
                RedisHelper.SetEntryInHash(hashKey, key, value);
            }
        }
    }

    class MemLocker : ILockContrain
    {
        Dictionary<String, String> locker { get; set; }

        public MemLocker(Dictionary<String, String> l)
        {
            locker = l;
        }

        public bool ContainsKey(string key)
        {
            return locker.ContainsKey(key);
        }

        public string this[string index]
        {
            get
            {
                return locker[index];
            }
            set
            {
                locker[index] = value;
            }
        }
    }

    public class TLockHelper
    {
        public TLockHelper(LockerType type)
        {
            switch (type)
            {
                case LockerType.MemLocker:
                    lockDic = new MemLocker(dicLock);
                    break;
                case LockerType.RedisLocker:
                    lockDic = new RedisLocker();
                    break;
            }
        }

        static Dictionary<String, String> dicLock = new Dictionary<String, String>();

        ILockContrain lockDic { get; set; }

        static string keyFormat = "[{0}][{1}]";

        List<String> all = new List<string>();
        bool islock = false;

        public string TryLock(String type, string id, int lockTime = 15)
        {
            try
            {
                string key = string.Format(keyFormat, type, id);

                lock (dicLock)
                {
                    string v = string.Empty;

                    if (lockDic.ContainsKey(key))
                        v = lockDic[key];

                    if (!string.IsNullOrEmpty(v))
                    {
                        lockObj l = JsonConvert.DeserializeObject<lockObj>(v);
                        if (!l.isExpire())
                            return "";

                        v = string.Empty;
                    }

                    lockObj o = new lockObj();
                    o.Type = type;
                    o.Id = id;
                    o.CreatedTime = DateTime.Now;
                    o.LockTime = lockTime;

                    string s = string.Format("{0}|{1}", key, o.CreatedTime.Ticks).ToBase64String();

                    o.LockID = s;

                    lockDic[key] = JsonConvert.SerializeObject(o);

                    all.Add(o.LockID);

                    if (!islock) islock = true;

                    return o.LockID;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public void FreeLock(string lockID)
        {
            try
            {
                string s = lockID.Base64ToString();
                string key = s.Split('|')[0];

                lock (dicLock)
                {
                    if (!lockDic.ContainsKey(key))
                        return;

                    string v = lockDic[key];

                    if (string.IsNullOrEmpty(v))
                        return;


                    lockObj l = JsonConvert.DeserializeObject<lockObj>(v);
                    if (l.LockID != lockID)
                        return;

                    lockDic[key] = string.Empty;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void FreeAllLock()
        {
            lock (dicLock)
                if (islock)
                    foreach (string lockID in all)
                        try
                        {
                            string s = lockID.Base64ToString();
                            string key = s.Split('|')[0];

                            if (!lockDic.ContainsKey(key))
                                return;

                            string v = lockDic[key];

                            if (string.IsNullOrEmpty(v))
                                return;


                            lockObj l = JsonConvert.DeserializeObject<lockObj>(v);
                            if (l.LockID != lockID)
                                return;

                            lockDic[key] = string.Empty;
                        }
                        catch (Exception ex)
                        {

                        }
        }


        class lockObj
        {
            public string Type { get; set; }
            public string Id { get; set; }
            public DateTime CreatedTime { get; set; }
            public int LockTime { get; set; }
            public string LockID { get; set; }

            public bool isExpire()
            {
                return CreatedTime.AddMinutes(LockTime) < DateTime.Now;
            }
        }
    }
}
