using System;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.OntologyViewModels
{
    using Host.EDI.ValueObjects;

    public class TopicUpdateInput : ITopicUpdateInput
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
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
        public string Description { get; set; }
    }
}
