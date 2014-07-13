
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class InfoGroupAddedEvent : DomainEvent {
        #region Ctor
        public InfoGroupAddedEvent(InfoGroupBase source) : base(source) { }
        #endregion
    }
}
