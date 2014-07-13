
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class TopicUpdatedEvent : DomainEvent {
        /// <summary>
        /// 
        /// </summary>
        #region Ctor
        public TopicUpdatedEvent(TopicBase source)
            : base(source) {
        }
        #endregion
    }
}