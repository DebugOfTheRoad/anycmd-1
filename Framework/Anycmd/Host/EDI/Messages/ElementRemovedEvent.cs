
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ElementRemovedEvent : DomainEvent {
        #region Ctor
        public ElementRemovedEvent(ElementBase source) : base(source) { }
        #endregion
    }
}
