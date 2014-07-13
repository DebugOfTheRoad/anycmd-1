
namespace Anycmd.AC
{
    using Model;

    /// <summary>
    /// 工作组基类。
    /// </summary>
    public abstract class GroupBase : EntityBase, IGroup
    {
        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual int IsEnabled { get; set; }

        public virtual int? PrivilegeState { get; set; }

        /// <summary>
        /// 类型鉴别码：AC表示角色组+用户组；工作组分型表示它可以用来作为任何其它类型资源的组，只要定义相应的TypeCode就行。
        /// </summary>
        public virtual string TypeCode { get; set; }

        public virtual string OrganizationCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

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
