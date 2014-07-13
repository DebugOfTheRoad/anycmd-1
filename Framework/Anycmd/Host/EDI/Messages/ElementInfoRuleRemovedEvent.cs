using Anycmd.EDI;
using Anycmd.Events;

namespace Anycmd.Host.EDI.Messages
{
    public class ElementInfoRuleRemovedEvent : DomainEvent {
        #region Ctor
        public ElementInfoRuleRemovedEvent(ElementInfoRule source) : base(source) { }
        #endregion
    }
}
