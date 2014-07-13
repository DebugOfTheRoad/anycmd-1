
namespace Anycmd.AC.Infra.ViewModels.LogViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistOperationLogs : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? leftCreateOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? rightCreateOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? targetID { get; set; }
    }
}
