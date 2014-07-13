
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class ProcessUpdatedEvent : DomainEvent {
        #region Ctor
        public ProcessUpdatedEvent(ProcessBase source)
            : base(source) {
        }
        #endregion
    }
}
