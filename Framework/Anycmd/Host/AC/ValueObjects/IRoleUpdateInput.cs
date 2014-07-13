using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IRoleUpdateInput : IEntityUpdateInput
    {
        string CategoryCode { get; }
        string Description { get; }
        string Icon { get; }
        int IsEnabled { get; }
        string Name { get; }
        int SortCode { get; }
    }
}
