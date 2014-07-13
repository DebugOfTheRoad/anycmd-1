using Anycmd.EDI;
using Anycmd.Events;

namespace Anycmd.Host.EDI.Messages
{
    public class NodeOrganizationActionRemovedEvent : DomainEvent {
        #region Ctor
        public NodeOrganizationActionRemovedEvent(NodeOrganizationAction source) : base(source) { }
        #endregion
    }
}
