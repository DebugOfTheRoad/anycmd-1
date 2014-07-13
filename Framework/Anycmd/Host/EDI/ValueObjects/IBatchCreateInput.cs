using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IBatchCreateInput : IEntityCreateInput
    {
        string Description { get; }
        bool? IncludeDescendants { get; }
        Guid NodeID { get; }
        Guid OntologyID { get; }
        string OrganizationCode { get; }
        string Title { get; }
        string Type { get; }
    }
}
