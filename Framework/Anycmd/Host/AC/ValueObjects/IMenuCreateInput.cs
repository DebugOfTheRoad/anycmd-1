using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IMenuCreateInput : IEntityCreateInput
    {
        Guid AppSystemID { get; }
        string Description { get; }
        string Icon { get; }
        string Name { get; }
        Guid? ParentID { get; }
        int SortCode { get; }
        string Url { get; }
    }
}
