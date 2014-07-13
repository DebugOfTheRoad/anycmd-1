
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class FunctionRemovedEvent : DomainEvent
    {
        #region Ctor
        public FunctionRemovedEvent(FunctionBase source)
            : base(source)
        {
        }
        #endregion
    }
}
