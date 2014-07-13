
namespace Anycmd.Host.AC.Identity.Messages
{
    using Anycmd.AC.Identity;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class AccountLoginedEvent : DomainEvent
    {
        #region Ctor
        public AccountLoginedEvent(AccountBase source) : base(source) { }
        #endregion
    }
}