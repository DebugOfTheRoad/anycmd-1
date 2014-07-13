
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 节点关心本体元素。节点和本体元素的多对多映射
    /// </summary>
    public class NodeElementCare : NodeElementCareBase, IAggregateRoot
    {
        public NodeElementCare() { }

        public static NodeElementCare Create(INodeElementCareCreateInput input)
        {
            return new NodeElementCare
            {
                Id = input.Id.Value,
                ElementID = input.ElementID,
                NodeID = input.NodeID,
                IsInfoIDItem = input.IsInfoIDItem
            };
        }
    }
}
