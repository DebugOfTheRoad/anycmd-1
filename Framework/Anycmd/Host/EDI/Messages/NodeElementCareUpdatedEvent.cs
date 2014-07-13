
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class NodeElementCareUpdatedEvent : DomainEvent {
        #region Ctor
        public NodeElementCareUpdatedEvent(NodeElementCareBase source) : base(source) {
            this.IsInfoIDItem = source.IsInfoIDItem;
        }
        #endregion

        public bool IsInfoIDItem { get; private set; }
    }
}
