
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class ButtonRemovedEvent : DomainEvent
    {
        #region Ctor
        public ButtonRemovedEvent(ButtonBase source)
            : base(source)
        {
        }
        #endregion
    }
}