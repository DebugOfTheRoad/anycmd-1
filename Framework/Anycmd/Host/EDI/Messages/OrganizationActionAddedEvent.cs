
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class OrganizationActionAddedEvent : DomainEvent {
        #region Ctor
        public OrganizationActionAddedEvent(OrganizationAction source) : base(source) { }
        #endregion
    }
}
