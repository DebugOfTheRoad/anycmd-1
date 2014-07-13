using Anycmd.AC.Identity;
using Anycmd.Events;

namespace Anycmd.Host.AC.Identity.Messages
{
    public class AccountRemovedEvent : DomainEvent
    {
        #region Ctor
        public AccountRemovedEvent(AccountBase source) : base(source) { }
        #endregion
    }
}