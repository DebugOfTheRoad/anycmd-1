
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class ButtonUpdatedEvent : DomainEvent
    {
        #region Ctor
        public ButtonUpdatedEvent(ButtonBase source, IButtonUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IButtonUpdateInput Input { get; private set; }
    }
}