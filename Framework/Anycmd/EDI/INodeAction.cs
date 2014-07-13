
namespace Anycmd.EDI {
    using Host.AC;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface INodeAction {
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
        Guid ActionID { get; }
        /// <summary>
        /// 
        /// </summary>
        string IsAllowed { get; }
        /// <summary>
        /// 
        /// </summary>
        string IsAudit { get; }

        /// <summary>
        /// 是否允许
        /// </summary>
        AllowType AllowType { get; }

        /// <summary>
        /// 是否需要审核
        /// </summary>
        AuditType AuditType { get; }
    }
}
