using Anycmd.EDI;
using Anycmd.Events;

namespace Anycmd.Host.EDI.Messages
{
    public class NodeOrganizationActionAddedEvent : DomainEvent {
        #region Ctor
        public NodeOrganizationActionAddedEvent(NodeOrganizationAction source) : base(source) { }
        #endregion
    }
}
