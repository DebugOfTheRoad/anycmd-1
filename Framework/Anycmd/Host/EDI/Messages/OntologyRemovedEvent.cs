
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public sealed class OntologyRemovedEvent : DomainEvent {
        #region Ctor
        public OntologyRemovedEvent(OntologyBase source) : base(source) { }
        #endregion
    }
}
