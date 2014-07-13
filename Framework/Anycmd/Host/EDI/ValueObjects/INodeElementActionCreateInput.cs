using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface INodeElementActionCreateInput : IEntityCreateInput
    {
        Guid NodeID { get; }

        /// <summary>
        /// 
        /// </summary>
        Guid ElementID { get; }

        /// <summary>
        /// 
        /// </summary>
        Guid ActionID { get; }
        /// <summary>
        /// 是否需要审核
        /// </summary>
        bool IsAudit { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsAllowed { get; }
    }
}
