
namespace Anycmd.AC.ViewModels.PrivilegeViewModels
{
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistAccountOrganizationPrivileges : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string organizationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? includeDescendants { get; set; }
    }
}
