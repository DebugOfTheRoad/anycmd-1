
namespace Anycmd.EDI.ViewModels.ProcessViewModels
{
    using Host.EDI.ValueObjects;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessUpdateInput : IProcessUpdateInput, IInfoModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
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
    }
}
