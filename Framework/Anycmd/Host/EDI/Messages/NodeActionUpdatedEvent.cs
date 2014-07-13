
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class NodeActionUpdatedEvent : DomainEvent {
        #region Ctor
        public NodeActionUpdatedEvent(NodeAction source)
            : base(source) {
            this.IsAllowed = source.IsAllowed;
            this.IsAudit = source.IsAudit;
        }
        #endregion

        public string IsAllowed { get; private set; }
        public string IsAudit { get; private set; }
    }
}
