
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 权限应用系统基类
    /// </summary>
    public abstract class AppSystemBase : EntityBase, IAppSystem
    {
        private string _code;
        private string _name;

        protected AppSystemBase()
        {
            IsEnabled = 1;
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public virtual string Code
        {
            get { return _code; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("编码是必须的");
                }
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
        /// 单点登录Http认证接口地址
        /// </summary>
        public virtual string SSOAuthAddress { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public virtual Guid PrincipalID { get; set; }

        /// <summary>
        /// 系统名称
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
                _name = value;
            }
        }

        /// <summary>
        /// 系统图标Url
        /// </summary>
        public virtual string ImageUrl { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// 允许编辑
        /// </summary>
        public virtual int AllowEdit { get; set; }

        /// <summary>
        /// 允许删除
        /// </summary>
        public virtual int AllowDelete { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        public virtual string Description { get; set; }
    }
}
