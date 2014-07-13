
namespace Anycmd.Host.AC.Messages
{
    using Anycmd.AC;
    using Model;
    using ValueObjects;

    public class GroupAddedEvent : EntityAddedEvent<IGroupCreateInput>
    {
        #region Ctor
        public GroupAddedEvent(GroupBase source, IGroupCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}
