using System;

namespace Anycmd.AC.Infra.ViewModels.PageViewModels
{
    using Host.AC.ValueObjects;
    using Model;

    /// <summary>
    /// 
    /// </summary>
    public class PageButtonCreateInput : EntityCreateInput, IInputModel, IPageButtonCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid PageID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ButtonID { get; set; }
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
