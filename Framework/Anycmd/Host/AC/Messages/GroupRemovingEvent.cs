using Anycmd.AC;
using Anycmd.Events;

namespace Anycmd.Host.AC.Messages
{
    public class GroupRemovingEvent: DomainEvent
    {
        #region Ctor
        public GroupRemovingEvent(GroupBase source)
            : base(source)
        {
        }
        #endregion
    }
}