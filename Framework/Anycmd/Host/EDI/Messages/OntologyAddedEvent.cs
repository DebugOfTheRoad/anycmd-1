
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class OntologyAddedEvent : DomainEvent
    {
        #region Ctor
        public OntologyAddedEvent(OntologyBase source, IOntologyCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IOntologyCreateInput Input { get; private set; }
    }
}
