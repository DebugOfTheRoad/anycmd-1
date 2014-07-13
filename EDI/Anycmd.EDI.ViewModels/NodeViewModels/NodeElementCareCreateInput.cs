using System;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.NodeViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    /// <summary>
    /// 
    /// </summary>
    public class NodeElementCareCreateInput : EntityCreateInput, INodeElementCareCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid NodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid ElementID { get; set; }

        public bool IsInfoIDItem { get; set; }
    }
}
