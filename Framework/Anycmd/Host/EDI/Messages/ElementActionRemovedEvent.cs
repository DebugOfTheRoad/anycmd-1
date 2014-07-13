
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ElementActionRemovedEvent : DomainEvent {
        #region Ctor
        public ElementActionRemovedEvent(ElementAction source)
            : base(source) {
        }
        #endregion
    }
}
