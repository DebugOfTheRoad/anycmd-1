
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;
    using System;

    public interface IOrganizationUpdateInput : IEntityUpdateInput
    {
        string Address { get; }
        string CategoryCode { get; }
        string Code { get; }
        string Description { get; }
        string Fax { get; }
        string Icon { get; }
        string InnerPhone { get; }
        int IsEnabled { get; }
        string Name { get; }
        string Postalcode { get; }
        string OuterPhone { get; }
        string ParentCode { get; }
        string ShortName { get; }
        int SortCode { get; }
        string WebPage { get; }
        Guid? ContractorID { get; }
    }
}
