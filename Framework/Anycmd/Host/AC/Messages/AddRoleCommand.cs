
namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddRoleCommand : AddEntityCommand<IRoleCreateInput>, ISysCommand
    {
        public AddRoleCommand(IRoleCreateInput input)
            : base(input)
        {

        }
    }
}
