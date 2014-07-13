
namespace Anycmd.EDI.ViewModels {
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistEntity : GetPlistResult {
        /// <summary>
        /// 
        /// </summary>
        public string ontologyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? archiveID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string organizationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? includedescendants { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? translate { get; set; }
    }
}
