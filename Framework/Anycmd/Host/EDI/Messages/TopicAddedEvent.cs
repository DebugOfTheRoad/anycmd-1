
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class TopicAddedEvent : DomainEvent {
        #region Ctor
        public TopicAddedEvent(TopicBase source) : base(source) { }
        #endregion
    }
}
