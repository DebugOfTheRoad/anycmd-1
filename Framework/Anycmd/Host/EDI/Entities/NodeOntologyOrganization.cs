
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;

    /// <summary>
    /// 节点组织结构。将节点和本体和组织结构三者的关系视作实体。
    /// </summary>
    public class NodeOntologyOrganization : NodeOntologyOrganizationBase, IAggregateRoot
    {
        public NodeOntologyOrganization() { }
    }
}
