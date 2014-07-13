
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class NodeActionAddedEvent : DomainEvent {
        #region Ctor
        public NodeActionAddedEvent(NodeAction source) : base(source) { }
        #endregion
    }
}
