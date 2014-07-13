using Anycmd.AC.Identity;
using Anycmd.Events;

namespace Anycmd.Host.AC.Identity.Messages
{
    public class DeveloperUpdatedEvent : DomainEvent
    {
        #region Ctor
        public DeveloperUpdatedEvent(AccountBase source) : base(source) { }
        #endregion
    }
}
