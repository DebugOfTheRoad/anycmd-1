
namespace Anycmd.AC.Infra.ViewModels.ButtonViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class PageAssignButtonTr
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? FunctionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string FunctionCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string FunctionName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid PageID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid ButtonID { get; set; }

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
        public virtual int ButtonIsEnabled { get; set; }

        public virtual int FunctionIsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsAssigned { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }
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
