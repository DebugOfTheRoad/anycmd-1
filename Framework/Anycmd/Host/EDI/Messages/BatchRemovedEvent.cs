using Anycmd.EDI;
using Anycmd.Events;

namespace Anycmd.Host.EDI.Messages
{
    public class BatchRemovedEvent : DomainEvent
    {
        #region Ctor
        public BatchRemovedEvent(IBatch source)
            : base(source)
        {
        }
        #endregion
    }
}
