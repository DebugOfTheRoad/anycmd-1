
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class OntologyUpdatedEvent : DomainEvent
    {
        #region Ctor
        public OntologyUpdatedEvent(OntologyBase source, IOntologyUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IOntologyUpdateInput Input { get; private set; }
    }
}
