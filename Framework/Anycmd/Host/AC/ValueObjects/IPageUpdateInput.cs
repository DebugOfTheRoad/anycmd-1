using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IPageUpdateInput : IEntityUpdateInput
    {
        string Icon { get; }
        string Tooltip { get; }
    }
}
