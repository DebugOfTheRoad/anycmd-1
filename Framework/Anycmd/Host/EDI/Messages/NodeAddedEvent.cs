
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using Anycmd.Host.EDI.ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class NodeAddedEvent : DomainEvent
    {
        #region Ctor
        public NodeAddedEvent(NodeBase source, INodeCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public INodeCreateInput Input { get; private set; }
    }
}
