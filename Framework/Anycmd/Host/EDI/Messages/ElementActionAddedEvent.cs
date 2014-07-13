
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ElementActionAddedEvent : DomainEvent
    {
        #region Ctor
        public ElementActionAddedEvent(ElementAction source)
            : base(source)
        {
        }
        #endregion
    }
}
