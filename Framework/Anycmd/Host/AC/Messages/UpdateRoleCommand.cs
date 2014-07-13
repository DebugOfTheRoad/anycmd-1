
namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateRoleCommand : UpdateEntityCommand<IRoleUpdateInput>, ISysCommand
    {
        public UpdateRoleCommand(IRoleUpdateInput input)
            : base(input)
        {

        }
    }
}
