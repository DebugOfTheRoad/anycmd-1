using Anycmd.AC.Infra;
using Anycmd.Events;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class ResourceTypeRemovingEvent: DomainEvent
    {
        #region Ctor
        public ResourceTypeRemovingEvent(ResourceTypeBase source)
            : base(source)
        {
        }
        #endregion
    }
}