
namespace Anycmd.AC
{
    using Exceptions;
    using Model;

    /// <summary>
    /// 角色基类
    /// </summary>
    public abstract class RoleBase : EntityBase, IRole
    {
        private string _name;

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        public virtual int NumberID { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string CategoryCode { get; set; }

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
        public virtual string Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int AllowEdit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int AllowDelete { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
    }
}
