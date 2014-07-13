
namespace Anycmd.EDI {
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface INodeTopic {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid NodeID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid TopicID { get; }
    }
}
