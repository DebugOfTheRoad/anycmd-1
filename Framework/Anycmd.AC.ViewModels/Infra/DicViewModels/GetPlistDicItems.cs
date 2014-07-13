
namespace Anycmd.AC.Infra.ViewModels.DicViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistDicItems : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? dicID { get; set; }
    }
}
