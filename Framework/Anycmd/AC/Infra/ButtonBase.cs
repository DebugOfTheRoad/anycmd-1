
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;

    /// <summary>
    /// 按钮基类。
    /// </summary>
    public abstract class ButtonBase : EntityBase, IButton
    {
        private string _code;
        private string _name;

        protected ButtonBase()
        {
            IsEnabled = 1;
        }

        /// <summary>
        /// 是否启用
        /// </remarks>
        /// </summary>
        public virtual int IsEnabled { get; set; }
        /// <summary>
        /// 
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
                value = value.Trim();
                if (value != _code)
                {
                    _code = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name {
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
        public virtual string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string CategoryCode { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }
    }
}
