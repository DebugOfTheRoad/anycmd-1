
namespace Anycmd.AC.ViewModels.GroupViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class AccountAssignGroupTr
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
        public virtual Guid GroupID { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        public virtual string CreateBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? CreateUserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}
