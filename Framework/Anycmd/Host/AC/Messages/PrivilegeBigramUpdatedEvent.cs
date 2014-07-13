using Anycmd.Events;

namespace Anycmd.Host.AC.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC;

    public class PrivilegeBigramUpdatedEvent : DomainEvent
    {
        #region Ctor
        public PrivilegeBigramUpdatedEvent(PrivilegeBigramBase source, IPrivilegeBigramUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IPrivilegeBigramUpdateInput Input { get; private set; }
    }
}
