
namespace Anycmd.AC.Infra.ViewModels.EntityTypeViewModels
{
    using Anycmd.Host.AC.ValueObjects;
    using Model;
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 
    /// </summary>
    public class PropertyUpdateInput : IInputModel, IPropertyUpdateInput
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ForeignPropertyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? MaxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? DicID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GuideWords { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDetailsShow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeveloperOnly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InputType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsInput { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsTotalLine { get; set; }
    }
}
