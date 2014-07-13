﻿
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 菜单基类
    /// </summary>
    public abstract class MenuBase : EntityBase, IMenu
    {
        private Guid _appsystemID;

        public virtual Guid AppSystemID
        {
            get { return _appsystemID; }
            set
            {
                if (value != _appsystemID)
                {
                    if (_appsystemID != Guid.Empty)
                    {
                        throw new ValidationException("应用系统不能更改");
                    }
                    _appsystemID = value;
                }
            }
        }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? ParentID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int? AllowEdit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int? AllowDelete { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
    }
}