
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class InfoGroupRemovedEvent : DomainEvent {
        #region Ctor
        public InfoGroupRemovedEvent(InfoGroupBase source) : base(source) { }
        #endregion
    }
}
