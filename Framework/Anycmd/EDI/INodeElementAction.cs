
namespace Anycmd.EDI {
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface INodeElementAction {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid ActionID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid ElementID { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsAllowed { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsAudit { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid NodeID { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime? CreateOn { get; }
    }
}
