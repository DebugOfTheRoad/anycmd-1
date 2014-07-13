using Anycmd.AC.Infra;
using Anycmd.Events;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class MenuRemovingEvent: DomainEvent
    {
        #region Ctor
        public MenuRemovingEvent(MenuBase source)
            : base(source)
        {
        }
        #endregion
    }
}