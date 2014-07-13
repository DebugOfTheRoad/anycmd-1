
namespace Anycmd.Util
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// 令牌对象。
    /// <remarks>此对象不专门为序列化设计若要序列化应使用Anycmd.DataContract程序集中的TokenData</remarks>
    /// </summary>
    public sealed class TokenObject
    {
        private TokenObject() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenString">令牌字符串</param>
        /// <param name="appID">应用系统ID</param>
        /// <param name="ticks">时间戳</param>
        public static TokenObject Create(string tokenString, string appID, Int64 ticks)
        {
            return new TokenObject
            {
                TokenString = tokenString,
                AppID = appID,
                Ticks = ticks
            };
        }

        /// <summary>
        /// 令牌字符串
        /// </summary>
        public string TokenString { get; private set; }

        /// <summary>
        /// 公钥。
        /// </summary>
        public string AppID { get; private set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public Int64 Ticks { get; private set; }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="secretKey">私钥</param>
        /// <returns></returns>
        public bool IsValid(string secretKey)
        {
            if (string.IsNullOrEmpty(AppID))
            {
                return false;
            }
            else if (string.IsNullOrEmpty(TokenString))
            {
                return false;
            }
            var myToken = Token(this.AppID, this.Ticks, secretKey);

            return myToken == TokenString;
        }

        /// <summary>
        /// 计算并返回令牌字符串
        /// </summary>
        /// <param name="appID">公钥</param>
        /// <param name="ticks"></param>
        /// <param name="secretKey">私钥</param>
        /// <returns></returns>
        public static string Token(string appID, Int64 ticks, string secretKey)
        {
            var s = (string.Format("{0}{1}{2}", appID, ticks.ToString(), secretKey)).ToLower();// 转化为小写
            var crypto = new MD5CryptoServiceProvider();
            byte[] bytes = crypto.ComputeHash(Encoding.UTF8.GetBytes(s));
            var sb = new StringBuilder();
            foreach (byte num in bytes)
            {
                sb.AppendFormat("{0:x2}", num);
            }

            return sb.ToString();
        }
    }
}
