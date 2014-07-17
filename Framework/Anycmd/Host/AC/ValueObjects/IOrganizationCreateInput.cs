
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;
    using System;

    public interface IOrganizationCreateInput : IEntityCreateInput
    {
        string Address { get; }
        string CategoryCode { get; }
        string Code { get; }
        string Description { get; }
        string Fax { get; }
        string Icon { get; }
        Guid? ContractorID { get; }
        string InnerPhone { get; }
        int IsEnabled { get; }
        string Name { get; }
        string OuterPhone { get; }
        string ParentCode { get; }
        string PostalCode { get; }
        string ShortName { get; }
        int SortCode { get; }
        string WebPage { get; }
    }
}
