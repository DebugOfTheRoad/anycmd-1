
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class AppSystemRemovedEvent : DomainEvent
    {
        #region Ctor
        public AppSystemRemovedEvent(AppSystemBase source)
            : base(source)
        {
        }
        #endregion
    }
}
