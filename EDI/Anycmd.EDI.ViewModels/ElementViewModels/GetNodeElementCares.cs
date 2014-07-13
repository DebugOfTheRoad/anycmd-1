
namespace Anycmd.EDI.ViewModels.ElementViewModels {
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetNodeElementCares : GetPlistResult {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid nodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid ontologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? isAssigned { get; set; }
    }
}
