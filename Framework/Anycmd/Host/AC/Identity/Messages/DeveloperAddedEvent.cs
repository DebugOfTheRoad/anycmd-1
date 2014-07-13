
namespace Anycmd.Host.AC.Identity.Messages
{
    using Anycmd.AC.Identity;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class DeveloperAddedEvent : DomainEvent
    {
        #region Ctor
        public DeveloperAddedEvent(DeveloperID source) : base(source) { }
        #endregion
    }
}
