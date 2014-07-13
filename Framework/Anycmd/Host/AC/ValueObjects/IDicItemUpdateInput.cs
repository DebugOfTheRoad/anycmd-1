using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IDicItemUpdateInput : IEntityUpdateInput
    {
        string Code { get; }
        string Description { get; }
        int IsEnabled { get; }
        string Name { get; }
        int SortCode { get; }
    }
}
