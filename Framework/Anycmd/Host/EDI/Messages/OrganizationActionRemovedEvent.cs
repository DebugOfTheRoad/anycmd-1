
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class OrganizationActionRemovedEvent : DomainEvent {
        #region Ctor
        public OrganizationActionRemovedEvent(OrganizationAction source) : base(source) { }
        #endregion
    }
}
