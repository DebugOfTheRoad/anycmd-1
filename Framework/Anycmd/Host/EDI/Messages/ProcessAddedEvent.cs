using Anycmd.EDI;
using Anycmd.Events;

namespace Anycmd.Host.EDI.Messages
{
    public class ProcessAddedEvent: DomainEvent {
        #region Ctor
        public ProcessAddedEvent(ProcessBase source)
            : base(source) {
        }
        #endregion
    }
}
