using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IMenuUpdateInput : IEntityUpdateInput
    {
        Guid AppSystemID { get; }
        string Description { get; }
        string Icon { get; }
        string Name { get; }
        int SortCode { get; }
        string Url { get; }
    }
}
