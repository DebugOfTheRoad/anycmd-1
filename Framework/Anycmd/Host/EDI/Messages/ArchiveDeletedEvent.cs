
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ArchiveDeletedEvent : DomainEvent {
        #region Ctor
        public ArchiveDeletedEvent(ArchiveBase source) : base(source) { }
        #endregion
    }
}
