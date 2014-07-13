
namespace Anycmd.AC.ViewModels.PrivilegeViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class RoleAssignFunctionTr
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsAssigned { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid RoleID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid FunctionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid AppSystemID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string AppSystemCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid ResourceTypeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ResourceCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ResourceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string FunctionCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string PrivilegeConstraint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int PrivilegeOrientation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? CreateUserID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string CreateBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}
