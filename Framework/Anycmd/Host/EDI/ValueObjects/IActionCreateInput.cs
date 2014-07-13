
namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;
    using System;

    public interface IActionCreateInput : IEntityCreateInput
    {
        string Description { get; }
        string IsAllowed { get; }
        string IsAudit { get; }
        bool IsPersist { get; }
        string Name { get; }
        Guid OntologyID { get; }
        int SortCode { get; }
        string Verb { get; }
    }
}
