
namespace Anycmd.AC.Infra.ViewModels.ButtonViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class ButtonTr
    {
        public ButtonTr() { }

        public static ButtonTr Create(ButtonState button)
        {
            return new ButtonTr
            {
                Code = button.Code,
                CreateOn = button.CreateOn,
                Icon = button.Icon,
                Id = button.Id,
                IsEnabled = button.IsEnabled,
                Name = button.Name,
                CategoryCode = button.CategoryCode,
                SortCode = button.SortCode
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        public virtual string CategoryCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}
