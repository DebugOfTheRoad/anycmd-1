
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    public class ButtonRemovingEvent: DomainEvent
    {
        #region Ctor
        public ButtonRemovingEvent(ButtonBase source)
            : base(source)
        {
        }
        #endregion
    }
}