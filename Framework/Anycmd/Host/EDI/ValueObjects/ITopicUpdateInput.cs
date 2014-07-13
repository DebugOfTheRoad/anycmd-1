using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface ITopicUpdateInput : IEntityUpdateInput
    {
        string Code { get; }
        string Description { get; }
        string Name { get; }
    }
}
