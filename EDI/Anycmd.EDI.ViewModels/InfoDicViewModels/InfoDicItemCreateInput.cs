using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.InfoDicViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    /// <summary>
    /// 
    /// </summary>
    public class InfoDicItemCreateInput : EntityCreateInput, IInputModel, IInfoDicItemCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid InfoDicID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Level { get; set; }
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
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public int IsEnabled { get; set; }
    }
}
