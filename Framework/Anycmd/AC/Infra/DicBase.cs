
namespace Anycmd.AC.Infra
{
    using Exceptions;
    using Model;

    /// <summary>
    /// 系统字典基类
    /// </summary>
    public abstract class DicBase : EntityBase, IDic
    {
        private string _code;
        private string _name;

        protected DicBase()
        {
            IsEnabled = 1;
        }

        /// <summary>
        /// 字典有效状态
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 字典码
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
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
    }
}
