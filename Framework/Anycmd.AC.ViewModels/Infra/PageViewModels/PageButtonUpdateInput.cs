using System;

namespace Anycmd.AC.Infra.ViewModels.PageViewModels
{
    using Host.AC.ValueObjects;
    using Model;

    /// <summary>
    /// 
    /// </summary>
    public class PageButtonUpdateInput : IInputModel, IPageButtonUpdateInput
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? FunctionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsEnabled { get; set; }
    }
}
