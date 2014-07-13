
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class NodeOntologyCareRemovedEvent : DomainEvent {
        #region Ctor
        public NodeOntologyCareRemovedEvent(NodeOntologyCareBase source) : base(source) { }
        #endregion
    }
}
