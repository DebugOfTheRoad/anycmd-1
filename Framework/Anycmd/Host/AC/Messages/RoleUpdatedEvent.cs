
namespace Anycmd.Host.AC.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC;
    using Anycmd.Events;

    public class RoleUpdatedEvent : DomainEvent
    {
        #region Ctor
        public RoleUpdatedEvent(RoleBase source, IRoleUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IRoleUpdateInput Input { get; private set; }
    }
}