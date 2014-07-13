
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateOrganizationCommand : UpdateEntityCommand<IOrganizationUpdateInput>, ISysCommand
    {
        public UpdateOrganizationCommand(IOrganizationUpdateInput input)
            : base(input)
        {

        }
    }
}
