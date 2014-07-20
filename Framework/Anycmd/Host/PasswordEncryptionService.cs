
namespace Anycmd.Host
{
    using Util;

    public class PasswordEncryptionService : IPasswordEncryptionService
    {
        private readonly IAppHost host;

        public PasswordEncryptionService(IAppHost host)
        {
            this.host = host;
        }

        public string Encrypt(string rawPwd)
        {
            return EncryptionHelper.Hash(rawPwd);
        }
    }
}
