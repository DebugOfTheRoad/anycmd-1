using Anycmd.EDI;
using System;

namespace Anycmd.Host.EDI
{
    public sealed class NodeElementActionState : INodeElementAction
    {
        private NodeElementActionState() { }

        public static NodeElementActionState Create(INodeElementAction nodeElementAction)
        {
            if (nodeElementAction == null)
            {
                throw new ArgumentNullException("nodeElementAction");
            }
            return new NodeElementActionState
            {
                ActionID = nodeElementAction.ActionID,
                CreateOn = nodeElementAction.CreateOn,
                ElementID = nodeElementAction.ElementID,
                Id = nodeElementAction.Id,
                IsAllowed = nodeElementAction.IsAllowed,
                IsAudit = nodeElementAction.IsAudit,
                NodeID = nodeElementAction.NodeID
            };
        }

        public Guid Id { get; private set; }

        public Guid ActionID { get; private set; }

        public Guid ElementID { get; private set; }

        public bool IsAllowed { get; private set; }

        public bool IsAudit { get; private set; }

        public Guid NodeID { get; private set; }

        public DateTime? CreateOn { get; private set; }
    }
}
