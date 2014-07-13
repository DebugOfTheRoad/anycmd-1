
namespace Anycmd.Host.AC.ValueObjects
{
    public interface IPasswordChangeInput
    {
        string LoginName { get; }
        string OldPassword { get; }
        string NewPassword { get; }
    }
}
