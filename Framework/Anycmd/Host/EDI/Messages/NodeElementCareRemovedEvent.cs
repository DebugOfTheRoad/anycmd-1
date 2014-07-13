
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class NodeElementCareRemovedEvent : DomainEvent {
        #region Ctor
        public NodeElementCareRemovedEvent(NodeElementCareBase source) : base(source) { }
        #endregion
    }
}
