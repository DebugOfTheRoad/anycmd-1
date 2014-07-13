
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class OrganizationActionUpdatedEvent  : DomainEvent {
        #region Ctor
        public OrganizationActionUpdatedEvent(OrganizationAction source) : base(source) {
        }
        #endregion
    }
}
