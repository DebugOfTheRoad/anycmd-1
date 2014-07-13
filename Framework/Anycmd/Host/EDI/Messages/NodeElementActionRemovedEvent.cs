
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class NodeElementActionRemovedEvent : DomainEvent {
        #region Ctor
        public NodeElementActionRemovedEvent(NodeElementActionBase source) : base(source) { }
        #endregion
    }
}
