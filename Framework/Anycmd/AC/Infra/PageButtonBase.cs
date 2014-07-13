
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 页面按钮基类。<see cref="IPageButton"/>
    /// </summary>
    public abstract class PageButtonBase : EntityBase, IPageButton
    {
        private Guid _pageID;
        private Guid _buttonID;

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? FunctionID { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid PageID
        {
            get { return _pageID; }
            set
            {
                if (value != _pageID)
                {
                    if (_pageID != Guid.Empty)
                    {
                        throw new CoreException("不能更改所属页面");
                    }
                    _pageID = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid ButtonID
        {
            get { return _buttonID; }
            set
            {
                if (value != _buttonID)
                {
                    if (_buttonID != Guid.Empty)
                    {
                        throw new CoreException("不能更改关联按钮");
                    }
                    _buttonID = value;
                }
            }
        }
    }
}
