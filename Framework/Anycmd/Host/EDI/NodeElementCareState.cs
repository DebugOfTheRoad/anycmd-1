
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using System;

    public sealed class NodeElementCareState : INodeElementCare
    {
        private NodeElementCareState() { }

        public static NodeElementCareState Create(INodeElementCare nodeElementCare)
        {
            if (nodeElementCare == null)
            {
                throw new ArgumentNullException("nodeElementCare");
            }
            return new NodeElementCareState
            {
                ElementID = nodeElementCare.ElementID,
                Id = nodeElementCare.Id,
                IsInfoIDItem = nodeElementCare.IsInfoIDItem,
                NodeID = nodeElementCare.NodeID
            };
        }

        public Guid Id { get; private set; }

        public Guid ElementID { get; private set; }

        public Guid NodeID { get; private set; }

        public bool IsInfoIDItem { get; private set; }
    }
}
