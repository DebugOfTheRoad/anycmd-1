
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddOrganizationActionCommand: AddEntityCommand<IOrganizationActionCreateInput>, ISysCommand
    {
        public AddOrganizationActionCommand(IOrganizationActionCreateInput input)
            : base(input)
        {

        }
    }
}
