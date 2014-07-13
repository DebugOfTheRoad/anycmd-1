using System;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.BatchViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    /// <summary>
    /// 
    /// </summary>
    public class BatchCreateInput : EntityCreateInput, IInputModel, IBatchCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid NodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeDescendants { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }
}
