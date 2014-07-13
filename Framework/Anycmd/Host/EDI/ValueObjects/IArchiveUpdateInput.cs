using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IArchiveUpdateInput : IEntityUpdateInput
    {
        string Description { get; }
        string Title { get; }
    }
}
