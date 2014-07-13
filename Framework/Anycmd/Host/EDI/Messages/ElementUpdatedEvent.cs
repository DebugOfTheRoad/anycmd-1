
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ElementUpdatedEvent : DomainEvent {
        #region Ctor
        public ElementUpdatedEvent(ElementBase source)
            : base(source) {
        }
        #endregion
    }
}
