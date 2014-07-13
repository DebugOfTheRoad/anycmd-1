
namespace Anycmd.Host.AC.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC;
    using Anycmd.Events;

    public class PrivilegeBigramAddedEvent : DomainEvent
    {
        #region Ctor
        public PrivilegeBigramAddedEvent(PrivilegeBigramBase source, IPrivilegeBigramCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IPrivilegeBigramCreateInput Input { get; private set; }
    }
}
