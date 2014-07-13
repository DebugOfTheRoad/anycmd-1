
namespace Anycmd.AC.Infra.ViewModels.PageViewModels
{
    using Host.AC.ValueObjects;
    using Model;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class PageUpdateInput : IInputModel, IPageUpdateInput
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tooltip { get; set; }
    }
}
