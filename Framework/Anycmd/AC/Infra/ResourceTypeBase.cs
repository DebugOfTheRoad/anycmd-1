using System;

namespace Anycmd.AC.Infra
{
    using Model;

    /// <summary>
    /// 资源基类
    /// </summary>
    public abstract class ResourceTypeBase : EntityBase, IResourceType
    {
        private string _code;

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

        public virtual Guid AppSystemID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int AllowEdit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int AllowDelete { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
    }
}
