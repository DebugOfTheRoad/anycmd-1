
namespace Anycmd.EDI.Service.Tests
{
    using DataContracts;
    using Anycmd.Host.EDI.Hecp;
    using Util;
    using ServiceModel.Operations;
    using System;

    public static class CommandExetension
    {
        private static readonly string JSPXPublicKey = "41e711c6-f215-4606-a0bf-9af11cce1d54";
        private static readonly string UIAPublicKey = "69E58EC0-5EB2-4633-9117-B433FC205B8F";
        private static readonly string YXTPublicKey = "87E9DAAB-2EA4-4A99-92BA-6C9DDB0F868C";

        private static readonly string JSPXSecretKey = "DF25BCB5-35E3-41E4-980F-64D916D806FF";
        private static readonly string UIASecretKey = "DF25BCB5-35E3-41E4-980F-64D916D806FF";
        private static readonly string YXTSecretKey = "df25bcb5-35e3-41e4-980f-64d916d806ff";

        /// <summary>
        /// 教师培训节点
        /// </summary>
        /// <returns></returns>
        public static Message JSPXToken(this Message cmd)
        {
            cmd.Credential = CreateToken(JSPXPublicKey, JSPXSecretKey);
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static Message JSPXSignature(this Message cmd)
        {
            Signature(cmd, JSPXPublicKey);
            return cmd;
        }

        /// <summary>
        /// Uia代理节点
        /// </summary>
        /// <returns></returns>
        public static Message UIAToken(this Message cmd)
        {
            cmd.Credential = CreateToken(UIAPublicKey, UIASecretKey);
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static Message UIASignature(this Message cmd)
        {
            Signature(cmd, UIAPublicKey);
            return cmd;
        }

        /// <summary>
        /// 一线通节点
        /// </summary>
        /// <returns></returns>
        public static Message YXTToken(this Message cmd)
        {
            cmd.Credential = CreateToken(YXTPublicKey, YXTSecretKey);
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static Message YXTSignature(this Message cmd)
        {
            Signature(cmd, YXTPublicKey);
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Message TimeOutToken(this Message cmd)
        {
            cmd.Credential = CreateToken(YXTPublicKey, SystemTime.UtcNow().AddMinutes(-6).Ticks, YXTSecretKey);
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static Message TimeOutSignature(this Message cmd)
        {
            Signature(cmd, YXTPublicKey, SystemTime.UtcNow().AddMinutes(-6).Ticks);
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static CredentialData CreateToken(string publicKey, string secretKey)
        {
            return CreateToken(publicKey, SystemTime.UtcNow().Ticks, secretKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static void Signature(Message data, string publicKey)
        {
            Signature(data, publicKey, SystemTime.UtcNow().Ticks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static CredentialData CreateToken(string publicKey, Int64 ticks, string secretKey)
        {
            CredentialData credential = new CredentialData
            {
                ClientType = ClientType.Node.ToName(),
                CredentialType = CredentialType.Token.ToName(),
                ClientID = publicKey,
                Ticks = ticks,
                UserName = "UnitTest"
            };
            credential.Password = TokenObject.Token(publicKey, ticks, secretKey);

            return credential;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static void Signature(Message command, string publicKey, Int64 ticks)
        {
            CredentialData credential = new CredentialData
            {
                ClientType = ClientType.Node.ToName(),
                CredentialType = CredentialType.Signature.ToName(),
                SignatureMethod = SignatureMethod.HMAC_SHA1.ToName(),
                ClientID = publicKey,
                Ticks = ticks,
                UserName = "UnitTest"
            };
            command.Credential = credential;
        }
    }
}
