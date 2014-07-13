
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 操作基类。<see cref="IFunction"/>
    /// </summary>
    public abstract class FunctionBase : EntityBase, IFunction
    {
        private Guid _resourceTypeID;
        private string _code;

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

        public virtual bool IsManaged { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid ResourceTypeID
        {
            get { return _resourceTypeID; }
            set
            {
                if (value != _resourceTypeID)
                {
                    if (value == Guid.Empty)
                    {
                        throw new CoreException("必须指定资源");
                    }
                    if (_resourceTypeID != Guid.Empty)
                    {
                        throw new CoreException("所属资源不能修改");
                    }
                    _resourceTypeID = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid DeveloperID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
    }
}
