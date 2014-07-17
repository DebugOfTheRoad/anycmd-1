
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 系统字典项基类
    /// </summary>
    public abstract class DicItemBase : EntityBase, IDicItem
    {
        private string _code;
        private string _name;
        private Guid _dicID;

        protected DicItemBase()
        {
            IsEnabled = 1;
        }

        public virtual int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Code
        {
            get { return _code; }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (value != _code)
                {
                    _code = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("名称是必须的");
                }
                if (value != _name)
                {
                    _name = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid DicID
        {
            get { return _dicID; }
            set
            {
                if (value != _dicID)
                {
                    if (_dicID != Guid.Empty)
                    {
                        throw new CoreException("不能更改字典项的所属字典");
                    }
                    _dicID = value;
                }
            }
        }
    }
}
