using Anycmd.AC.Identity;
using Anycmd.Events;

namespace Anycmd.Host.AC.Identity.Messages
{
    public class AccountAddedEvent : DomainEvent
    {
        #region Ctor
        public AccountAddedEvent(AccountBase source) : base(source) { }
        #endregion
    }
}