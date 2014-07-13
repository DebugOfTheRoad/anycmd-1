
namespace Anycmd.AC.ViewModels.PrivilegeViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistFunctionByRoleID : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid roleID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? isAssigned { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid appSystemID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? resourceTypeID { get; set; }
    }
}
