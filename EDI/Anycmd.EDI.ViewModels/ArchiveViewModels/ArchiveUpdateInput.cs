using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.ArchiveViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    /// <summary>
    /// 
    /// </summary>
    public class ArchiveUpdateInput : IInputModel, IArchiveUpdateInput
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [StringLength(200)]
        [DisplayName("标题")]
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        [DisplayName("备注")]
        public string Description { get; set; }
    }
}
