
namespace Anycmd.AC.Infra
{
    using Model;

    /// <summary>
    /// 系统字典基类
    /// </summary>
    public abstract class DicBase : EntityBase, IDic
    {
        private string _code;

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
        /// 字典名
        /// </summary>
        public virtual string Name { get; set; }

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
