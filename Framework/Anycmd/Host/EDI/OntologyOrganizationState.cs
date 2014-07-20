
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Exceptions;
    using Hecp;
    using Host;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class OntologyOrganizationState : IOntologyOrganization
    {
        private Dictionary<Verb, IOrganizationAction> _orgActionDic;
        private readonly IAppHost host;

        private OntologyOrganizationState(IAppHost host)
        {
            this.host = host;
        }

        public static OntologyOrganizationState Create(IAppHost host, IOntologyOrganization ontologyOrganization)
        {
            if (ontologyOrganization == null)
            {
                throw new ArgumentNullException("ontologyOrganization");
            }
            var data = new OntologyOrganizationState(host)
            {
                Id = ontologyOrganization.Id,
                Actions = ontologyOrganization.Actions,
                OntologyID = ontologyOrganization.OntologyID,
                OrganizationID = ontologyOrganization.OrganizationID
            };
            var orgActionDic = new Dictionary<Verb, IOrganizationAction>();
            data._orgActionDic = orgActionDic;
            if (data.Actions != null)
            {
                var orgActions = host.DeserializeFromString<OrganizationAction[]>(data.Actions);
                if (orgActions != null)
                {
                    foreach (var orgAction in orgActions)
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
                        orgActionDic.Add(verb, orgAction);
                    }
                }
            }
            return data;
        }

        public Guid Id { get; private set; }

        public Guid OntologyID { get; private set; }

        public Guid OrganizationID { get; private set; }

        public string Actions { get; private set; }

        public IReadOnlyDictionary<Verb, IOrganizationAction> OrganizationActions
        {
            get { return _orgActionDic; }
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
            if (!(obj is OntologyOrganizationState))
            {
                return false;
            }
            var left = this;
            var right = (OntologyOrganizationState)obj;

            return
                left.Id == right.Id &&
                left.OntologyID == right.OntologyID &&
                left.OrganizationID == right.OrganizationID;
        }

        public static bool operator ==(OntologyOrganizationState a, OntologyOrganizationState b)
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

        public static bool operator !=(OntologyOrganizationState a, OntologyOrganizationState b)
        {
            return !(a == b);
        }
    }
}
