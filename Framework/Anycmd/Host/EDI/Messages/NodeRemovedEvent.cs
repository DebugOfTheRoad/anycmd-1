
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class NodeRemovedEvent : DomainEvent {
        #region Ctor
        public NodeRemovedEvent(NodeBase source) : base(source) { }
        #endregion
    }
}
