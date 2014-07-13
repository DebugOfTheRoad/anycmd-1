
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 节点关心本体。将节点和本体的关系视作实体。
    /// </summary>
    public class NodeOntologyCare : NodeOntologyCareBase, IAggregateRoot
    {
        public NodeOntologyCare() { }

        public static NodeOntologyCare Create(INodeOntologyCareCreateInput input)
        {
            return new NodeOntologyCare
            {
                Id = input.Id.Value,
                NodeID = input.NodeID,
                OntologyID = input.OntologyID
            };
        }
    }
}
