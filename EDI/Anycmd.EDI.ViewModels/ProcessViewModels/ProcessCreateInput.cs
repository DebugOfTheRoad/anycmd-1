
namespace Anycmd.EDI.ViewModels.ProcessViewModels
{
    using Host.EDI.ValueObjects;
    using System;
    using System.ComponentModel;
    using Model;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessCreateInput : EntityCreateInput, IProcessCreateInput, IInputModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid OntologyID { get; set; }

        [Required]
        public int NetPort { get; set; }

        public string OrganizationCode { get; set; }
    }
}
