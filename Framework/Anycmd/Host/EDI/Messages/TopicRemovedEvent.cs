
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class TopicRemovedEvent : DomainEvent {
        #region Ctor
        public TopicRemovedEvent(TopicBase source) : base(source) { }
        #endregion
    }
}
