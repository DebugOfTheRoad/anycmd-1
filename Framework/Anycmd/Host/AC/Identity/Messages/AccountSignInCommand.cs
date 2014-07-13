
namespace Anycmd.Host.AC.Identity.Messages
{
    using Commands;

    public class AccountSignInCommand : Command, ISysCommand
    {
        public AccountSignInCommand(string loginName, string password, string rememberMe)
        {
            this.LoginName = loginName;
            this.Password = password;
            this.RememberMe = rememberMe;
        }

        public string LoginName { get; private set; }

        public string Password { get; private set; }

        public string RememberMe { get; private set; }
    }
}
