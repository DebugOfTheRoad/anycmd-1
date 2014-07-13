
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 组织结构实体
    /// </summary>
    public class Organization : OrganizationBase, IAggregateRoot
    {
        #region Ctor
        public Organization()
        {
            IsEnabled = 1;
        }
        #endregion

        public static Organization Create(IOrganizationCreateInput input)
        {
            return new Organization
            {
                Id = input.Id.Value,
                AccountingID = input.AccountingID,
                Address = input.Address,
                AssistantLeadershipID = input.AssistantLeadershipID,
                AssistantManagerID = input.AssistantManagerID,
                Bank = input.Bank,
                BankAccount = input.BankAccount,
                CashierID = input.CashierID,
                CategoryCode = input.CategoryCode,
                Code = input.Code,
                Description = input.Description,
                Fax = input.Fax,
                Icon = input.Icon,
                IsEnabled = input.IsEnabled,
                InnerPhone = input.InnerPhone,
                FinancialID = input.FinancialID,
                LeadershipID = input.LeadershipID,
                Name = input.Name,
                OuterPhone = input.OuterPhone,
                ParentCode = input.ParentCode,
                Postalcode = input.PostalCode,
                PrincipalID = input.PrincipalID,
                PrivilegeState = null,
                ManagerID = input.ManagerID,
                ShortName = input.ShortName,
                SortCode = input.SortCode,
                WebPage = input.WebPage
            };
        }

        public void Update(IOrganizationUpdateInput input)
        {
            this.AccountingID = input.AccountingID;
            this.Address = input.Address;
            this.AssistantLeadershipID = input.AssistantLeadershipID;
            this.AssistantManagerID = input.AssistantManagerID;
            this.Bank = input.Bank;
            this.CashierID = input.CashierID;
            this.CategoryCode = input.CategoryCode;
            this.Code = input.Code;
            this.Description = input.Description;
            this.Fax = input.Fax;
            this.FinancialID = input.FinancialID;
            this.Icon = input.Icon;
            this.InnerPhone = input.InnerPhone;
            this.IsEnabled = input.IsEnabled;
            this.LeadershipID = input.LeadershipID;
            this.ManagerID = input.ManagerID;
            this.Name = input.Name;
            this.OuterPhone = input.OuterPhone;
            this.ParentCode = input.ParentCode;
            this.Postalcode = input.ParentCode;
            this.PrincipalID = input.PrincipalID;
            this.ShortName = input.ShortName;
            this.SortCode = input.SortCode;
            this.WebPage = input.WebPage;
        }
    }
}
