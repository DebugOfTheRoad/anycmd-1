
namespace Anycmd.AC
{
    using Exceptions;
    using Model;

    /// <summary>
    /// 组。工作组是资源组，AC类型的工作组是账户和角色资源的资源组。加入AC工作组的账户自动逻辑地得到组中的角色。
    /// </summary>
    public abstract class GroupBase : EntityBase, IGroup
    {
        private string _name;
        private string _typeCode;

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 类型鉴别码：AC表示角色组+用户组；工作组分型表示它可以用来作为任何其它类型资源的组，只要定义相应的TypeCode就行。
        /// </summary>
        public virtual string TypeCode {
            get { return _typeCode; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("组类型码是必须的");
                }
                if (value != _typeCode)
                {
                    _typeCode = value;
                }
            }
        }

        public virtual string OrganizationCode { get; set; }

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
        /// 
        /// </summary>
        public virtual string CategoryCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
    }
}
