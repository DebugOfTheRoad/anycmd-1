
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using System;

    public class RemoveOntologyOrganizationCommand : Command, ISysCommand
    {
        public RemoveOntologyOrganizationCommand(Guid ontologyID, Guid organizationID)
        {
            this.OntologyID = ontologyID;
            this.OrganizationID = organizationID;
        }

        public Guid OntologyID { get; private set; }

        public Guid OrganizationID { get; private set; }
    }
}
