
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class ProcessRemovedEvent : DomainEvent {
        #region Ctor
        public ProcessRemovedEvent(ProcessBase source) : base(source) { }
        #endregion
    }
}
