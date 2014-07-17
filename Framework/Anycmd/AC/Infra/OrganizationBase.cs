
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 组织结构基类<see cref="IOrganization"/>
    /// </summary>
    public abstract class OrganizationBase : EntityBase, IOrganization
    {
        private string _code;
        private string _name;

        #region Ctor
        protected OrganizationBase() { }
        #endregion

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ParentCode { get; set; }

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
        public virtual string ShortName { get; set; }

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
        /// 包工头标识
        /// </summary>
        public Guid? ContractorID { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        public virtual int DeletionStateCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string CategoryCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string OuterPhone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string InnerPhone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Fax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Postalcode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string WebPage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
    }
}
