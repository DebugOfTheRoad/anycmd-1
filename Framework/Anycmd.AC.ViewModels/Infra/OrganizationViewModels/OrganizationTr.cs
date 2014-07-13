
namespace Anycmd.AC.Infra.ViewModels.OrganizationViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class OrganizationTr
    {
        public OrganizationTr() { }

        public static OrganizationTr Create(OrganizationState organization)
        {
            return new OrganizationTr
            {
                CategoryCode = organization.CategoryCode,
                Code = organization.Code,
                CreateOn = organization.CreateOn,
                Id = organization.Id,
                IsEnabled = organization.IsEnabled,
                Name = organization.Name,
                ParentCode = organization.ParentCode,
                ParentName = organization.Parent.Name,
                SortCode = organization.SortCode
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ParentCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ParentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string CategoryCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}
