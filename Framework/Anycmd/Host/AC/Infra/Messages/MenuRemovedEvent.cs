
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    public class MenuRemovedEvent : DomainEvent
    {
        #region Ctor
        public MenuRemovedEvent(MenuBase source)
            : base(source)
        {
        }
        #endregion
    }
}