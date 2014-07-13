
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class NodeOntologyCareAddedEvent : DomainEvent
    {
        #region Ctor
        public NodeOntologyCareAddedEvent(NodeOntologyCareBase source, INodeOntologyCareCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public INodeOntologyCareCreateInput Input { get; private set; }
    }
}
