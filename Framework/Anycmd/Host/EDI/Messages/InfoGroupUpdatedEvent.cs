
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class InfoGroupUpdatedEvent : DomainEvent {
        #region Ctor
        public InfoGroupUpdatedEvent(InfoGroupBase source)
            : base(source) {
        }
        #endregion
    }
}
