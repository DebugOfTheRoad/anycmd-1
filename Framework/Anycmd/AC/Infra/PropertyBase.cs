
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 系统字段基类<see cref="IProperty"/>
    /// </summary>
    public abstract class PropertyBase : EntityBase, IProperty
    {
        private string _code;
        private Guid _entityTypeID;
        private string _name;

        protected PropertyBase()
        {
        }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid EntityTypeID
        {
            get { return _entityTypeID; }
            set
            {
                if (_entityTypeID != value && _entityTypeID != Guid.Empty)
                {
                    throw new CoreException("不能更改字段的所属模型");
                }
                _entityTypeID = value;
            }
        }
        public virtual Guid? ForeignPropertyID
        {
            get;
            set;
        }
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
        public virtual string GroupCode { get; set; }
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
        public virtual string GuideWords { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Tooltip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int? MaxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? DicID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsDetailsShow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsDeveloperOnly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsInput { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string InputType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsTotalLine { get; set; }
    }
}
