using System;

namespace Anycmd.EDI
{
    using Host.AC;

    public interface INodeOrganizationAction {
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
        Guid OrganizationID { get; }
        /// <summary>
        /// 
        /// </summary>
        string IsAllowed { get; }
        /// <summary>
        /// 
        /// </summary>
        string IsAudit { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime? CreateOn { get; }

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
