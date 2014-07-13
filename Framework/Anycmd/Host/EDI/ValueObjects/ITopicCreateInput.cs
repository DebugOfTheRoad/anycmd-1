using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface ITopicCreateInput : IEntityCreateInput
    {
        string Code { get; }
        string Description { get; }
        string Name { get; }
        bool IsAllowed { get; }
        Guid OntologyID { get; }
    }
}
