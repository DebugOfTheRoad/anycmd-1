using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IAppSystemUpdateInput : IEntityUpdateInput, IManagedPropertyValues
    {
        string Code { get; }
        string Description { get; }
        string Icon { get; }
        string ImageUrl { get; }
        int IsEnabled { get; }
        string Name { get; }
        Guid PrincipalID { get; }
        int SortCode { get; }
        string SSOAuthAddress { get; }
    }
}
