using System;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.OntologyViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    public class TopicCreateInput : EntityCreateInput, ITopicCreateInput
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
        public string Description { get; set; }

        public bool IsAllowed { get; set; }
    }
}
