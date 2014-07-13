
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;
    using System;

    public interface IAppSystemCreateInput : IEntityCreateInput
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
