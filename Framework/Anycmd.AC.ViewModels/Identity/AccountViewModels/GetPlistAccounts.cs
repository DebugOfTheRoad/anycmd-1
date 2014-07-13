
namespace Anycmd.AC.Identity.ViewModels.AccountViewModels
{
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistAccounts : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public bool? includeDescendants { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string organizationCode { get; set; }
    }
}
