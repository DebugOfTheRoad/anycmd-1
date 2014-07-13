using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IOrganizationUpdateInput : IEntityUpdateInput
    {
        /// <summary>
        /// 会计
        /// </summary>
        Guid? AccountingID { get; }
        string Address { get; }
        Guid? AssistantLeadershipID { get; }
        Guid? AssistantManagerID { get; }
        string Bank { get; }
        string BankAccount { get; }
        Guid? CashierID { get; }
        string CategoryCode { get; }
        string Code { get; }
        string Description { get; }
        string Fax { get; }
        Guid? FinancialID { get; }
        string Icon { get; }
        string InnerPhone { get; }
        int IsEnabled { get; }
        Guid? LeadershipID { get; }
        Guid? ManagerID { get; }
        string Name { get; }
        string OuterPhone { get; }
        string ParentCode { get; }
        string PostalCode { get; }
        Guid? PrincipalID { get; }
        string ShortName { get; }
        int SortCode { get; }
        string WebPage { get; }
    }
}
