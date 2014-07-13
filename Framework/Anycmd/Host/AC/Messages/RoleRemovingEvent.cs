using Anycmd.AC;
using Anycmd.Events;

namespace Anycmd.Host.AC.Messages
{
    public class RoleRemovingEvent: DomainEvent
    {
        #region Ctor
        public RoleRemovingEvent(RoleBase source)
            : base(source)
        {
        }
        #endregion
    }
}