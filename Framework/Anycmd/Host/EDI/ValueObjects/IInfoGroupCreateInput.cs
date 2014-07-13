using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IInfoGroupCreateInput : IEntityCreateInput
    {
        string Code { get; }
        string Description { get; }
        string Name { get; }
        Guid OntologyID { get; }
        int SortCode { get; }
    }
}
