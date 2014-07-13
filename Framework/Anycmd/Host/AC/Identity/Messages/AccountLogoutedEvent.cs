
namespace Anycmd.Host.AC.Identity.Messages
{
    using Anycmd.AC.Identity;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class AccountLogoutedEvent : DomainEvent
    {
        #region Ctor
        public AccountLogoutedEvent(AccountBase source) : base(source) { }
        #endregion
    }
}