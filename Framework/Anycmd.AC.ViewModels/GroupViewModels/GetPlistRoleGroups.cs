
namespace Anycmd.AC.ViewModels.GroupViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistRoleGroups : GetPlistResult
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
    }
}
