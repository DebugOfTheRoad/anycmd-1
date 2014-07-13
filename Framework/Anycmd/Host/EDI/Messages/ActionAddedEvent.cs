
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ActionAddedEvent : DomainEvent {
        #region Ctor
        public ActionAddedEvent(ActionBase source) : base(source) { }
        #endregion
    }
}
