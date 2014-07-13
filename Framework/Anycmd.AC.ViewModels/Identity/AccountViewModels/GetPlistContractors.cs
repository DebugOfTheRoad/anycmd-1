
namespace Anycmd.AC.Identity.ViewModels.ContractorViewModels
{
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistContractors : GetPlistResult
    {
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
