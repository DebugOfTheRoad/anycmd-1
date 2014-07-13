using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface INodeOntologyCareCreateInput : IEntityCreateInput
    {
        Guid NodeID { get; }
        Guid OntologyID { get; }
    }
}
