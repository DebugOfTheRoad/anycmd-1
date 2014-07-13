using Anycmd.EDI;
using Anycmd.Events;

namespace Anycmd.Host.EDI.Messages
{
    public class BatchUpdatedEvent : DomainEvent
    {
        #region Ctor
        public BatchUpdatedEvent(IBatch source)
            : base(source)
        {
        }
        #endregion
    }
}
