using Anycmd.EDI;
using Anycmd.Events;

namespace Anycmd.Host.EDI.Messages
{
    public class ElementInfoRuleAddedEvent : DomainEvent {
        #region Ctor
        public ElementInfoRuleAddedEvent(ElementInfoRule source) : base(source) { }
        #endregion
    }
}
