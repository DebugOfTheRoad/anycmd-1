using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IArchiveCreateInput : IEntityCreateInput
    {
        string Description { get; }
        Guid OntologyID { get; }
        string RdbmsType { get; }
        string Title { get; }
    }
}
