using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IGroupUpdateInput : IEntityUpdateInput
    {
        string CategoryCode { get; }
        string Description { get; }
        int IsEnabled { get; }
        string OrganizationCode { get; }
        string Name { get; }
        string ShortName { get; }
        int SortCode { get; }
        string TypeCode { get; }
    }
}
