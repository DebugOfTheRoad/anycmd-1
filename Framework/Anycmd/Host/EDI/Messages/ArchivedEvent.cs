
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ArchivedEvent : DomainEvent {
        #region Ctor
        public ArchivedEvent(ArchiveBase source)
            : base(source) {
        }
        #endregion
    }
}
