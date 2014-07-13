using Anycmd.AC.Infra;
using Anycmd.Events;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class DicRemovingEvent: DomainEvent
    {
        #region Ctor
        public DicRemovingEvent(DicBase source)
            : base(source)
        {
        }
        #endregion
    }
}