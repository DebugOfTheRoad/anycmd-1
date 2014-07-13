
namespace Anycmd.AC.ViewModels.RoleViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistFunctionRoles : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid functionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? isAssigned { get; set; }
    }
}
