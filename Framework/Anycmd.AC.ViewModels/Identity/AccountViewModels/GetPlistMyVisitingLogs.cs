
namespace Anycmd.AC.Identity.ViewModels.AccountViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistMyVisitingLogs : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? leftVisitOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? rightVisitOn { get; set; }
    }
}
