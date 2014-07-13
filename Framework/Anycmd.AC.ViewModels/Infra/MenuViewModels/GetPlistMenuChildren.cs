
namespace Anycmd.AC.Infra.ViewModels.MenuViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistMenuChildren : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? parentID { get; set; }
    }
}
