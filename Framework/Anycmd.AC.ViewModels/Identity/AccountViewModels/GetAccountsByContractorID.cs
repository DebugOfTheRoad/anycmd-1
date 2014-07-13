
namespace Anycmd.AC.Identity.ViewModels.AccountViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetAccountsByContractorID : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid? contractorID { get; set; }
    }
}
