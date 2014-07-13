
namespace Anycmd.AC.Identity.ViewModels.AccountViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistGroupAccounts : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid? groupID { get; set; }
    }
}
