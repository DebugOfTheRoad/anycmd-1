using Anycmd.AC.Infra;
using Anycmd.Events;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class FunctionRemovingEvent: DomainEvent
    {
        #region Ctor
        public FunctionRemovingEvent(FunctionBase source)
            : base(source)
        {
        }
        #endregion
    }
}
