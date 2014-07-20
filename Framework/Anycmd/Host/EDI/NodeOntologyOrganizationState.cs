
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Exceptions;
    using Hecp;
    using Host;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class NodeOntologyOrganizationState : INodeOntologyOrganization
    {
        private Dictionary<Verb, INodeOrganizationAction> _nodeOrgActionDic;
        private readonly IAppHost host;

        private NodeOntologyOrganizationState(IAppHost host)
        {
            this.host = host;
        }

        public static NodeOntologyOrganizationState Create(IAppHost host, INodeOntologyOrganization nodeOntologyOrg)
        {
            if (nodeOntologyOrg == null)
            {
                throw new ArgumentNullException("nodeOntologyOrg");
            }
            var data = new NodeOntologyOrganizationState(host)
            {
                Actions = nodeOntologyOrg.Actions,
                Id = nodeOntologyOrg.Id,
                NodeID = nodeOntologyOrg.NodeID,
                OntologyID = nodeOntologyOrg.OntologyID,
                OrganizationID = nodeOntologyOrg.OrganizationID
            };
            var nodeOrgActionDic = new Dictionary<Verb, INodeOrganizationAction>();
            data._nodeOrgActionDic = nodeOrgActionDic;
            if (data.Actions != null)
            {
                var nodeOrgActions = host.DeserializeFromString<NodeOrganizationAction[]>(data.Actions);
                if (nodeOrgActions != null)
                {
                    foreach (var orgAction in nodeOrgActions)
                    {
                        var action = host.Ontologies.GetAction(orgAction.ActionID);
                        if (action == null)
                        {
                            throw new CoreException("意外的组织结构动作标识" + orgAction.ActionID);
                        }
                        OntologyDescriptor ontology;
                        if (!host.Ontologies.TryGetOntology(action.OntologyID, out ontology))
                        {
                            throw new CoreException("意外的本体元素本体标识" + action.OntologyID);
                        }
                        OrganizationState org;
                        if (!host.OrganizationSet.TryGetOrganization(orgAction.OrganizationID, out org))
                        {
                            throw new CoreException("意外的组织结构动作组织结构标识" + orgAction.OrganizationID);
                        }
                        var actionDic = host.Ontologies.GetActons(ontology);
                        var verb = actionDic.Where(a => a.Value.Id == orgAction.ActionID).Select(a => a.Key).FirstOrDefault();
                        if (verb == null)
                        {
                            throw new CoreException("意外的本体动作标识" + orgAction.ActionID);
                        }
                        nodeOrgActionDic.Add(verb, orgAction);
                    }
                }
            }
            return data;
        }

        public Guid Id { get; private set; }

        public Guid NodeID { get; private set; }

        public Guid OntologyID { get; private set; }

        public Guid OrganizationID { get; private set; }

        public string Actions { get; private set; }

        public IReadOnlyDictionary<Verb, INodeOrganizationAction> NodeOrganizationActions
        {
            get { return _nodeOrgActionDic; }
        }

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
            if (!(obj is NodeOntologyOrganizationState))
            {
                return false;
            }
            var left = this;
            var right = (NodeOntologyOrganizationState)obj;

            return
                left.Id == right.Id &&
                left.NodeID == right.NodeID &&
                left.OntologyID == right.OntologyID &&
                left.OrganizationID == right.OrganizationID;
        }

        public static bool operator ==(NodeOntologyOrganizationState a, NodeOntologyOrganizationState b)
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

        public static bool operator !=(NodeOntologyOrganizationState a, NodeOntologyOrganizationState b)
        {
            return !(a == b);
        }
    }
}
