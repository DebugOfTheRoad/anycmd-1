using Anycmd.EDI;
using System;

namespace Anycmd.Host.EDI
{
    public sealed class NodeOntologyCareState : INodeOntologyCare
    {
        private NodeOntologyCareState() { }

        public static NodeOntologyCareState Create(INodeOntologyCare nodeOntologyCare)
        {
            if (nodeOntologyCare == null)
            {
                throw new ArgumentNullException("nodeOntologyCare");
            }
            return new NodeOntologyCareState
            {
                CreateOn = nodeOntologyCare.CreateOn,
                Id = nodeOntologyCare.Id,
                NodeID = nodeOntologyCare.NodeID,
                OntologyID = nodeOntologyCare.OntologyID
            };
        }

        public Guid Id { get; private set; }

        public Guid NodeID { get; private set; }

        public Guid OntologyID { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!(obj is NodeOntologyCareState))
            {
                return false;
            }
            var left = this;
            var right = (NodeOntologyCareState)obj;

            return
                left.Id == right.Id &&
                left.NodeID == right.NodeID &&
                left.OntologyID == right.OntologyID;
        }

        public static bool operator ==(NodeOntologyCareState a, NodeOntologyCareState b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(NodeOntologyCareState a, NodeOntologyCareState b)
        {
            return !(a == b);
        }
    }
}
