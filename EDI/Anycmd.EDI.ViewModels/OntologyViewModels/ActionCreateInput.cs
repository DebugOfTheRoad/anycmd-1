using System;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.OntologyViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    public class ActionCreateInput : EntityCreateInput, IActionCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Verb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string IsAllowed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string IsAudit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool IsPersist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }
}
