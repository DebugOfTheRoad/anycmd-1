
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ElementAddedEvent : DomainEvent {
        #region Ctor
        public ElementAddedEvent(ElementBase source) : base(source) { }
        #endregion
    }
}
