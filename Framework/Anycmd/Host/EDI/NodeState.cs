
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Exceptions;
    using Hecp;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class NodeState : INode
    {
        private Dictionary<OntologyDescriptor, Dictionary<Verb, INodeAction>> _nodeActionDic;

        private NodeState() { }

        public static NodeState Create(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            var data = new NodeState
            {
                Abstract = node.Abstract,
                Actions = node.Actions,
                AnycmdApiAddress = node.AnycmdApiAddress,
                AnycmdWSAddress = node.AnycmdWSAddress,
                BeatPeriod = node.BeatPeriod,
                Code = node.Code,
                CreateOn = node.CreateOn,
                Email = node.Email,
                Icon = node.Icon,
                Id = node.Id,
                IsDistributeEnabled = node.IsDistributeEnabled,
                IsEnabled = node.IsEnabled,
                IsExecuteEnabled = node.IsExecuteEnabled,
                IsProduceEnabled = node.IsProduceEnabled,
                IsReceiveEnabled = node.IsReceiveEnabled,
                Mobile = node.Mobile,
                Name = node.Name,
                Organization = node.Organization,
                PublicKey = node.PublicKey,
                QQ = node.QQ,
                SecretKey = node.SecretKey,
                SortCode = node.SortCode,
                Steward = node.Steward,
                Telephone = node.Telephone,
                TransferID = node.TransferID
            };
            var nodeActionDic = new Dictionary<OntologyDescriptor, Dictionary<Verb, INodeAction>>();
            data._nodeActionDic = nodeActionDic;
            if (data.Actions != null)
            {
                var nodeActions = NodeHost.Instance.AppHost.DeserializeFromString<NodeAction[]>(data.Actions);
                if (nodeActions != null)
                {
                    foreach (var nodeAction in nodeActions)
                    {
                        var action = NodeHost.Instance.Ontologies.GetAction(nodeAction.ActionID);
                        if (action == null)
                        {
                            throw new CoreException("意外的本体动作标识" + nodeAction.ActionID);
                        }
                        OntologyDescriptor ontology;
                        if (!NodeHost.Instance.Ontologies.TryGetOntology(action.OntologyID, out ontology))
                        {
                            throw new CoreException("意外的本体元素本体标识" + action.OntologyID);
                        }
                        if (!nodeActionDic.ContainsKey(ontology))
                        {
                            nodeActionDic.Add(ontology, new Dictionary<Verb, INodeAction>());
                        }
                        var actionDic = NodeHost.Instance.Ontologies.GetActons(ontology);
                        var verb = actionDic.Where(a => a.Value.Id == nodeAction.ActionID).Select(a => a.Key).FirstOrDefault();
                        if (verb == null)
                        {
                            throw new CoreException("意外的本体动作标识" + nodeAction.ActionID);
                        }
                        nodeActionDic[ontology].Add(verb, nodeAction);
                    }
                }
            }
            return data;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Code { get; private set; }

        public string Actions { get; private set; }

        public IReadOnlyDictionary<OntologyDescriptor, Dictionary<Verb, INodeAction>> NodeActions
        {
            get { return _nodeActionDic; }
        }

        public string Abstract { get; private set; }

        public string Organization { get; private set; }

        public string Steward { get; private set; }

        public string Telephone { get; private set; }

        public string Email { get; private set; }

        public string Mobile { get; private set; }

        public string QQ { get; private set; }

        public string Icon { get; private set; }

        public int IsEnabled { get; private set; }

        public bool IsExecuteEnabled { get; private set; }

        public bool IsProduceEnabled { get; private set; }

        public bool IsReceiveEnabled { get; private set; }

        public bool IsDistributeEnabled { get; private set; }

        public Guid TransferID { get; private set; }

        public string AnycmdApiAddress { get; private set; }

        public string AnycmdWSAddress { get; private set; }

        public int? BeatPeriod { get; private set; }

        public string PublicKey { get; private set; }

        public string SecretKey { get; private set; }

        public int SortCode { get; private set; }

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
            var value = obj as NodeState;
            if (value == null)
            {
                return false;
            }
            var left = this;
            var right = value;

            return left.Id == right.Id &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.Abstract == right.Abstract &&
                left.Organization == right.Organization &&
                left.Steward == right.Steward &&
                left.Telephone == right.Telephone &&
                left.Email == right.Email &&
                left.Mobile == right.Mobile &&
                left.QQ == right.QQ &&
                left.Icon == right.Icon &&
                left.IsEnabled == right.IsEnabled &&
                left.IsExecuteEnabled == right.IsExecuteEnabled &&
                left.IsProduceEnabled == right.IsProduceEnabled &&
                left.IsReceiveEnabled == right.IsReceiveEnabled &&
                left.IsDistributeEnabled == right.IsDistributeEnabled &&
                left.TransferID == right.TransferID &&
                left.AnycmdApiAddress == right.AnycmdApiAddress &&
                left.AnycmdWSAddress == right.AnycmdWSAddress &&
                left.BeatPeriod == right.BeatPeriod &&
                left.PublicKey == right.PublicKey &&
                left.SortCode == right.SortCode;
        }

        public static bool operator ==(NodeState a, NodeState b)
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

        public static bool operator !=(NodeState a, NodeState b)
        {
            return !(a == b);
        }
    }
}
