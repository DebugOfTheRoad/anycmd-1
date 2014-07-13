
namespace Anycmd.Host.AC.Identity.Messages
{
    using Anycmd.AC.Identity;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class DeveloperRemovedEvent : DomainEvent
    {
        #region Ctor
        public DeveloperRemovedEvent(DeveloperID source) : base(source) { }
        #endregion
    }
}
