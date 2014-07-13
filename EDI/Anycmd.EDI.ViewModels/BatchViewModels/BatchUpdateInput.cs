using System;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.BatchViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    /// <summary>
    /// 
    /// </summary>
    public class BatchUpdateInput : IInputModel, IBatchUpdateInput
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }
}
