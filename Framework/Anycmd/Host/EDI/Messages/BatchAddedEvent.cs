
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    public class BatchAddedEvent : DomainEvent
    {
        #region Ctor
        public BatchAddedEvent(IBatch source)
            : base(source)
        {
        }
        #endregion
    }
}
