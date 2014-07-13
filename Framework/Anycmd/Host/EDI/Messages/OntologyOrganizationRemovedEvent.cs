
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class OntologyOrganizationRemovedEvent : DomainEvent {
        #region Ctor
        public OntologyOrganizationRemovedEvent(OntologyOrganizationBase source) : base(source) { }
        #endregion
    }
}
