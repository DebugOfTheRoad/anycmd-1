
namespace Anycmd.AC.ViewModels.GroupViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class RoleAssignGroupTr
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid RoleID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid GroupID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsAssigned { get; set; }

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

        public virtual Guid? CreateUserID { get; set; }

        public virtual string CreateBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}
