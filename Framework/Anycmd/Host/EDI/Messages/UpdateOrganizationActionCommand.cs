
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateOrganizationActionCommand : UpdateEntityCommand<IOrganizationActionUpdateInput>, ISysCommand
    {
        public UpdateOrganizationActionCommand(IOrganizationActionUpdateInput input)
            : base(input)
        {

        }
    }
}
