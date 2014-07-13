
namespace Anycmd.Host.AC.Messages
{
    using Anycmd.AC;
    using Anycmd.Events;

    public class PrivilegeBigramRemovedEvent : DomainEvent
    {
        #region Ctor
        public PrivilegeBigramRemovedEvent(PrivilegeBigramBase source)
            : base(source)
        {
        }
        #endregion
    }
}
