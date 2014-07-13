using Anycmd.AC.Infra;
using Anycmd.Events;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class OrganizationRemovingEvent: DomainEvent
    {
        #region Ctor
        public OrganizationRemovingEvent(OrganizationBase source)
            : base(source)
        {
        }
        #endregion
    }
}
