using Anycmd.EDI;
using Anycmd.Events;

namespace Anycmd.Host.EDI.Messages
{
    public class ElementInfoRuleUpdatedEvent : DomainEvent {
        #region Ctor
        public ElementInfoRuleUpdatedEvent(ElementInfoRule source) : base(source) { }
        #endregion
    }
}
