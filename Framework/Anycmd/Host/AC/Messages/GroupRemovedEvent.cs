
namespace Anycmd.Host.AC.Messages
{
    using Anycmd.AC;
    using Anycmd.Events;

    public class GroupRemovedEvent : DomainEvent
    {
        #region Ctor
        public GroupRemovedEvent(GroupBase source)
            : base(source)
        {
        }
        #endregion
    }
}