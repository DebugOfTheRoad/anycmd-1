
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class FunctionUpdatedEvent : DomainEvent
    {
        #region Ctor
        public FunctionUpdatedEvent(FunctionBase source, IFunctionUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IFunctionUpdateInput Input { get; private set; }
    }
}
