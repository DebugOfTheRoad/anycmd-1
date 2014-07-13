using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IBatchUpdateInput : IEntityUpdateInput
    {
        string Description { get; set; }
        string Title { get; set; }
    }
}
