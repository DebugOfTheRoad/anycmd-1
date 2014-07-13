
namespace Anycmd.Host.AC.Identity.Messages
{
    using Anycmd.AC.Identity;
    using Events;

    /// <summary>
    /// 
    /// </summary>
    public class PasswordUpdatedEvent : DomainEvent
    {
        #region Ctor
        public PasswordUpdatedEvent(AccountBase source)
            : base(source)
        {
            this.Password = source.Password;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; private set; }
    }
}
