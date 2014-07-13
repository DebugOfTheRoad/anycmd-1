
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class NodeElementCareAddedEvent : DomainEvent {
        #region Ctor
        public NodeElementCareAddedEvent(NodeElementCareBase source, INodeElementCareCreateInput input) : base(source) {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public INodeElementCareCreateInput Input { get; private set; }
    }
}
