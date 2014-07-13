using Anycmd.AC.Identity;
using Anycmd.Events;

namespace Anycmd.Host.AC.Identity.Messages
{
    public class AccountUpdatedEvent : DomainEvent
    {
        #region Ctor
        public AccountUpdatedEvent(AccountBase source) : base(source) { }
        #endregion
    }
}