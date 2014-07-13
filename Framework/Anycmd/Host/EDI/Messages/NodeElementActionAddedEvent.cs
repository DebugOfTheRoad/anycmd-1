
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class NodeElementActionAddedEvent : DomainEvent
    {
        #region Ctor
        public NodeElementActionAddedEvent(NodeElementActionBase source, INodeElementActionCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public INodeElementActionCreateInput Input { get; private set; }
    }
}
