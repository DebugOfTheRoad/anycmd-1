
namespace Anycmd.Host.AC.Messages
{
    using Anycmd.AC;
    using Anycmd.Events;

    public class RoleRemovedEvent : DomainEvent
    {
        #region Ctor
        public RoleRemovedEvent(RoleBase source)
            : base(source)
        {
        }
        #endregion
    }
}