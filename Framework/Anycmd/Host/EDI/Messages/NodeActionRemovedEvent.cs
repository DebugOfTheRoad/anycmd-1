
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class NodeActionRemovedEvent : DomainEvent {
        #region Ctor
        public NodeActionRemovedEvent(NodeAction source) : base(source) { }
        #endregion
    }
}
