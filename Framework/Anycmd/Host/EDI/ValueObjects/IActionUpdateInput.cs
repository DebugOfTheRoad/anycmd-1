using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IActionUpdateInput : IEntityUpdateInput
    {
        string Description { get; }
        string IsAllowed { get; }
        string IsAudit { get; }
        bool IsPersist { get; }
        string Name { get; }
        int SortCode { get; }
        string Verb { get; }
    }
}
