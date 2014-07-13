
namespace Anycmd.AC.Infra.ViewModels.AppSystemViewModels
{
    using Host.AC.ValueObjects;
    using Model;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class FunctionCreateInput : EntityCreateInput, IInputModel, IFunctionCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid ResourceTypeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool IsManaged { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid DeveloperID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int SortCode { get; set; }
    }
}
