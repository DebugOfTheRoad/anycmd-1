
namespace Anycmd.AC.ViewModels.RoleViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistGroupRoles : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid groupID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? isAssigned { get; set; }
    }
}
