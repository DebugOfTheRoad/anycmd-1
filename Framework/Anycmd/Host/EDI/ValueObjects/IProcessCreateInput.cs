using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IProcessCreateInput : IEntityCreateInput
    {
        string Type { get; }

        string Name { get; }

        int NetPort { get; }

        int IsEnabled { get; }

        Guid OntologyID { get; }

        string OrganizationCode { get; }

        string Description { get; }
    }
}
