using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IInfoGroupUpdateInput : IEntityUpdateInput
    {
        string Code { get; }
        string Description { get; }
        string Name { get; }
        int SortCode { get; }
    }
}
