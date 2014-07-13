
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    public class AppSystemRemovingEvent: DomainEvent
    {
        #region Ctor
        public AppSystemRemovingEvent(AppSystemBase source)
            : base(source)
        {
        }
        #endregion
    }
}
