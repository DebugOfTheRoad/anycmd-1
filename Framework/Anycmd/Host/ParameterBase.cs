
namespace Anycmd.Host
{
    using Model;

    /// <summary>
    /// 系统参数模型基类
    /// </summary>
    public abstract class ParameterBase : EntityBase, IParameter, IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string GroupCode { get; protected set; }
        /// <summary>
        /// 类型鉴别码
        /// </summary>
        public virtual string CategoryCode { get; protected set; }
        /// <summary>
        /// 编码
        /// </summary>
        public virtual string Code { get; protected set; }
        /// <summary>
        /// 参数名
        /// </summary>
        public virtual string Name { get; protected set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public virtual string Value { get; set; }

        public virtual int SortCode { get; set; }

        public virtual string Description { get; set; }
    }
}
