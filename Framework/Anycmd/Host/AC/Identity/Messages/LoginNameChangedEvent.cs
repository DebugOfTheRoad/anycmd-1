
namespace Anycmd.Host.AC.Identity.Messages
{
    using Anycmd.AC.Identity;
    using Events;

    public class LoginNameChangedEvent : DomainEvent
    {
        #region Ctor
        public LoginNameChangedEvent(AccountBase source)
            : base(source)
        {
        }
        #endregion
    }
}
