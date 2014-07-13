
namespace Anycmd.AC.ViewModels.RoleViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class RoleTr
    {
        public RoleTr() { }

        public static RoleTr Create(RoleState role)
        {
            return new RoleTr
            {
                CategoryCode = role.CategoryCode,
                CreateOn = role.CreateOn,
                Icon = role.Icon,
                Id = role.Id,
                IsEnabled = role.IsEnabled,
                Name = role.Name,
                SortCode = role.SortCode
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }

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
