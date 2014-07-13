
namespace Anycmd.AC.Infra.ViewModels.OrganizationViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistChildren : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? categoryID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? parentID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string parentCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? includeDescendants { get; set; }
    }
}
