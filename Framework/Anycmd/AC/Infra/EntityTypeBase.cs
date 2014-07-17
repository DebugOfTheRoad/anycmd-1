
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 系统模型基类<see cref="IEntityType"/>
    /// </summary>
    public abstract class EntityTypeBase : EntityBase, IEntityType
    {
        private string _code;
        private string _codespace;
        private string _name;

        public virtual string Codespace
        {
            get { return _codespace; }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (value != _codespace)
                {
                    _codespace = value;
                }
            }
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
        public virtual bool IsOrganizational { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid DatabaseID { get; set; }
        // TODO:验证数据库架构
        /// <summary>
        /// 
        /// </summary>
        public virtual string SchemaName { get; set; }
        // TODO:验证数据库表
        /// <summary>
        /// 
        /// </summary>
        public virtual string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int EditWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int EditHeight { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

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
        /// 
        /// </summary>
        public virtual Guid DeveloperID { get; set; }
    }
}
