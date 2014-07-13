
namespace Anycmd.AC.Infra.ViewModels.DicViewModels
{
    using Anycmd.Host.AC.ValueObjects;
    using Model;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DicUpdateInput : IInputModel, IDicUpdateInput
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
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SortCode { get; set; }
    }
}
