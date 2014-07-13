
namespace Anycmd.EDI {
    using Model;
    using System;

    public class NodeOntologyOrganizationBase : EntityBase, IAggregateRoot, INodeOntologyOrganization {
        public Guid NodeID { get; set; }

        public Guid OntologyID { get; set; }

        public Guid OrganizationID { get; set; }

        public string Actions { get; set; }
    }
}
