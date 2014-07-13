using Anycmd.AC.Infra;
using Anycmd.Events;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class PageRemovingEvent: DomainEvent
    {
        #region Ctor
        public PageRemovingEvent(PageBase source)
            : base(source)
        {
        }
        #endregion
    }
}
