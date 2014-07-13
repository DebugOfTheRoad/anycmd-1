
namespace Anycmd.Host
{
    using Util;

    public class PasswordEncryptionService : IPasswordEncryptionService
    {
        private readonly AppHost host;

        public PasswordEncryptionService()
        {
            this.host = AppHost.Instance;
        }

        public PasswordEncryptionService(AppHost host)
        {
            this.host = host;
        }

        public string Encrypt(string rawPwd)
        {
            return EncryptionHelper.Hash(rawPwd);
        }
    }
}
