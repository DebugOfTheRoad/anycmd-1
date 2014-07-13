
namespace Anycmd.Host.AC.Identity.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateAccountCommand : UpdateEntityCommand<IAccountUpdateInput>, ISysCommand
    {
        public UpdateAccountCommand(IAccountUpdateInput input)
            : base(input)
        {

        }
    }
}
