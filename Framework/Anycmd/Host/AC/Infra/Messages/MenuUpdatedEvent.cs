
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    public class MenuUpdatedEvent : DomainEvent
    {
        #region Ctor
        public MenuUpdatedEvent(MenuBase source, IMenuUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IMenuUpdateInput Input { get; private set; }
    }
}