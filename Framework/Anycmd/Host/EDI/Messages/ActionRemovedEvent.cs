
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ActionRemovedEvent : DomainEvent {
        #region Ctor
        public ActionRemovedEvent(ActionBase source) : base(source) { }
        #endregion
    }
}
