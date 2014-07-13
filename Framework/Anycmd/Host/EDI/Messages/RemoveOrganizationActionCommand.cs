
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveOrganizationActionCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveOrganizationActionCommand(Guid ontologyOrganizationActionID)
            : base(ontologyOrganizationActionID)
        {

        }
    }
}
