using Anycmd.AC.Infra;
using Anycmd.Events;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class EntityTypeRemovingEvent: DomainEvent
    {
        #region Ctor
        public EntityTypeRemovingEvent(EntityTypeBase source)
            : base(source)
        {
        }
        #endregion
    }
}
