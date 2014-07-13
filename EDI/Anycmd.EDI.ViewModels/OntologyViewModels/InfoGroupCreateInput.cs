using System;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.OntologyViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    public class InfoGroupCreateInput : EntityCreateInput, IInfoGroupCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Code { get; set; }
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
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }
}
