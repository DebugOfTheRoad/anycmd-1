
namespace Anycmd.EDI.ViewModels.ElementViewModels {
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistElements : GetPlistResult {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid? ontologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? groupID { get; set; }
    }
}
