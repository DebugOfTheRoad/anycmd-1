
namespace Anycmd.AC.ViewModels.RoleViewModels
{
    using System;

    /// <summary>
    /// 为账户分配角色时使用的模型
    /// </summary>
    public class AccountAssignRoleTr
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid AccountID { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual Guid RoleID { get; set; }

        public virtual bool IsAssigned { get; set; }

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

        public virtual string CreateBy { get; set; }

        public virtual Guid? CreateUserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}
