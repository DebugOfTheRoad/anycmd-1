
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddOntologyOrganizationCommand: AddEntityCommand<IOntologyOrganizationCreateInput>, ISysCommand
    {
        public AddOntologyOrganizationCommand(IOntologyOrganizationCreateInput input)
            : base(input)
        {

        }
    }
}
