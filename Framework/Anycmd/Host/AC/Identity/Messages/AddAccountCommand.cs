
namespace Anycmd.Host.AC.Identity.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddAccountCommand : AddEntityCommand<IAccountCreateInput>, ISysCommand
    {
        public AddAccountCommand(IAccountCreateInput input)
            : base(input)
        {

        }
    }
}
