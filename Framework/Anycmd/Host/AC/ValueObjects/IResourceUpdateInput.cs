using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IResourceUpdateInput : IEntityUpdateInput
    {
        string Code { get; }
        string Description { get; }
        string Icon { get; }
        string Name { get; }
        int SortCode { get; }
    }
}
