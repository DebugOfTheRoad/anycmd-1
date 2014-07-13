
namespace Anycmd.AC.Identity.ViewModels.AccountViewModels
{
    using Anycmd.Host.AC.ValueObjects;

    public class PasswordChangeInput : IPasswordChangeInput
    {
        public string LoginName { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
