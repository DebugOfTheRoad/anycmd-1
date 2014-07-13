
namespace Anycmd.AC.ViewModels.RoleViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class MenuAssignRoleTr
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid MenuID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid RoleID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsAssigned { get; set; }

        public virtual int PrivilegeOrientation { get; set; }

        public virtual string PrivilegeConstraint { get; set; }

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

        public virtual Guid? CreateUserID { get; set; }

        public virtual string CreateBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}
