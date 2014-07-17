
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
                Address = input.Address,
                CategoryCode = input.CategoryCode,
                Code = input.Code,
                Description = input.Description,
                Fax = input.Fax,
                Icon = input.Icon,
                IsEnabled = input.IsEnabled,
                InnerPhone = input.InnerPhone,
                Name = input.Name,
                OuterPhone = input.OuterPhone,
                ParentCode = input.ParentCode,
                Postalcode = input.PostalCode,
                ShortName = input.ShortName,
                SortCode = input.SortCode,
                WebPage = input.WebPage,
                ContractorID=input.ContractorID
            };
        }

        public void Update(IOrganizationUpdateInput input)
        {
            this.Address = input.Address;
            this.CategoryCode = input.CategoryCode;
            this.Code = input.Code;
            this.Description = input.Description;
            this.Fax = input.Fax;
            this.Icon = input.Icon;
            this.InnerPhone = input.InnerPhone;
            this.IsEnabled = input.IsEnabled;
            this.Name = input.Name;
            this.OuterPhone = input.OuterPhone;
            this.ParentCode = input.ParentCode;
            this.Postalcode = input.Postalcode;
            this.ShortName = input.ShortName;
            this.SortCode = input.SortCode;
            this.WebPage = input.WebPage;
            this.ContractorID = input.ContractorID;
        }
    }
}
