
namespace Anycmd.Host.AC.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC;
    using Anycmd.Events;

    public class GroupUpdatedEvent : DomainEvent
    {
        #region Ctor
        public GroupUpdatedEvent(GroupBase source, IGroupUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IGroupUpdateInput Input { get; private set; }
    }
}