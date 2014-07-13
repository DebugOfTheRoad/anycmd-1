
namespace Anycmd.Host
{
    public interface IPasswordEncryptionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawPwd"></param>
        /// <returns></returns>
        string Encrypt(string rawPwd);
    }
}
