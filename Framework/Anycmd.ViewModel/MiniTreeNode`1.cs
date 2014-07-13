
using System;
using System.Collections.Generic;

namespace AnyCmd.ViewModel {
    /// <summary>
    /// miniui的tree节点模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MiniTreeNode<T> : IViewModel {
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? ParentID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual IList<T> children { get; set; }
    }
}
