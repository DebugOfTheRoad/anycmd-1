
namespace Anycmd.Host.EDI.MemorySets
{
    using Anycmd.EDI;
    using Anycmd.Repositories;
    using Bus;
    using Entities;
    using Exceptions;
    using Extensions;
    using Hecp;
    using Messages;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ValueObjects;
    using elementID = System.Guid;
    using isCare = System.Boolean;
    using ontologyID = System.Guid;

    /// <summary>
    /// 节点上下文访问接口默认实现
    /// </summary>
    public sealed class NodeSet : INodeSet
    {
        private readonly Dictionary<string, NodeDescriptor>
            _allNodesByID = new Dictionary<string, NodeDescriptor>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, NodeDescriptor>
            _allNodesByPublicKey = new Dictionary<string, NodeDescriptor>(StringComparer.OrdinalIgnoreCase);
        private NodeDescriptor _selfNode = null;
        private NodeDescriptor _centerNode = null;
        private bool initialized = false;
        private object locker = new object();

        private readonly Guid _id = Guid.NewGuid();
        private readonly NodeCareSet nodeCareSet;
        private readonly NodeElementActionSet actionSet;
        private readonly OrganizationSet organizationSet;

        private readonly NodeHost host;

        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// 构造并接入总线
        /// </summary>
        public NodeSet(NodeHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
            this.nodeCareSet = new NodeCareSet(host);
            this.actionSet = new NodeElementActionSet(host);
            this.organizationSet = new OrganizationSet(host);
            var messageDispatcher = host.AppHost.MessageDispatcher;
            if (messageDispatcher == null)
            {
                throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.AppHost.Name));
            }
            var handler = new MessageHandler(this);
            messageDispatcher.Register((IHandler<AddNodeCommand>)handler);
            messageDispatcher.Register((IHandler<NodeAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateNodeCommand>)handler);
            messageDispatcher.Register((IHandler<NodeUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveNodeCommand>)handler);
            messageDispatcher.Register((IHandler<NodeRemovedEvent>)handler);
        }

        public NodeDescriptor CenterNode
        {
            get
            {
                if (!initialized)
                {
                    Init();
                }
                if (_centerNode == null)
                {
                    throw new CoreException("尚没有设定中心节点，请先设定中心节点");
                }

                return _centerNode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public NodeDescriptor ThisNode
        {
            get
            {
                if (!initialized)
                {
                    Init();
                }
                if (_selfNode == null)
                {
                    throw new CoreException("尚没有设定这个节点，请先设定这个节点");
                }

                return _selfNode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool TryGetNodeByID(string nodeID, out NodeDescriptor node)
        {
            if (nodeID == null)
            {
                throw new ArgumentNullException("nodeID");
            }
            if (!initialized)
            {
                Init();
            }
            return _allNodesByID.TryGetValue(nodeID, out node);
        }

        public bool TryGetNodeByPublicKey(string publicKey, out NodeDescriptor node)
        {
            if (publicKey == null)
            {
                throw new ArgumentNullException("publicKey");
            }
            if (!initialized)
            {
                Init();
            }
            return _allNodesByPublicKey.TryGetValue(publicKey, out node);
        }

        #region GetNodeElementActions
        public IReadOnlyDictionary<Verb, NodeElementActionState> GetNodeElementActions(NodeDescriptor node, ElementDescriptor element)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (!initialized)
            {
                Init();
            }
            return actionSet[node, element];
        }
        #endregion

        public IEnumerable<ElementDescriptor> GetInfoIDElements(NodeDescriptor node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (!initialized)
            {
                Init();
            }
            return nodeCareSet.GetInfoIDElements(node);
        }

        public bool IsInfoIDElement(NodeDescriptor node, ElementDescriptor element)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (!initialized)
            {
                Init();
            }
            return nodeCareSet.IsInfoIDElement(node, element);
        }

        public IReadOnlyCollection<NodeElementCareState> GetNodeElementCares(NodeDescriptor node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (!initialized)
            {
                Init();
            }
            return nodeCareSet.GetNodeElementCares(node);
        }

        public IReadOnlyCollection<NodeOntologyCareState> GetNodeOntologyCares(NodeDescriptor node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (!initialized)
            {
                Init();
            }
            return nodeCareSet.GetNodeOntologyCares(node);
        }

        public IEnumerable<NodeOntologyCareState> GetNodeOntologyCares()
        {
            if (!initialized)
            {
                Init();
            }
            return nodeCareSet.GetNodeOntologyCares();
        }

        public bool IsCareforElement(NodeDescriptor node, ElementDescriptor element)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (!initialized)
            {
                Init();
            }
            return nodeCareSet.IsCareforElement(node, element);
        }

        public bool IsCareForOntology(NodeDescriptor node, OntologyDescriptor ontology)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            if (!initialized)
            {
                Init();
            }
            return nodeCareSet.IsCareForOntology(node, ontology);
        }

        public IReadOnlyDictionary<OrganizationState, NodeOntologyOrganizationState> GetNodeOntologyOrganizations(NodeDescriptor node, OntologyDescriptor ontology)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            if (!initialized)
            {
                Init();
            }
            return organizationSet[node, ontology];
        }

        public IEnumerable<NodeOntologyOrganizationState> GetNodeOntologyOrganizations()
        {
            if (!initialized)
            {
                Init();
            }
            return organizationSet.GetNodeOntologyOrganizations();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Refresh()
        {
            if (initialized)
            {
                initialized = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator<NodeDescriptor> GetEnumerator()
        {
            if (!initialized)
            {
                Init();
            }
            return _allNodesByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!initialized)
            {
                Init();
            }
            return _allNodesByID.Values.GetEnumerator();
        }

        /// <summary>
        /// 初始化节点上下文
        /// </summary>
        private void Init()
        {
            if (!initialized)
            {
                lock (locker)
                {
                    if (!initialized)
                    {
                        _allNodesByID.Clear();
                        _allNodesByPublicKey.Clear();
                        var allNodes = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetNodes();
                        foreach (var node in allNodes)
                        {
                            if (!(node is NodeBase))
                            {
                                throw new CoreException("节点模型必须继承NodeBase基类");
                            }
                            var nodeState = NodeState.Create(node);
                            var descriptor = new NodeDescriptor(nodeState);
                            _allNodesByID.Add(node.Id.ToString(), descriptor);
                            if (_allNodesByPublicKey.ContainsKey(node.PublicKey))
                            {
                                throw new CoreException("重复的公钥" + node.PublicKey);
                            }
                            _allNodesByPublicKey.Add(node.PublicKey, descriptor);
                            if (node.Id.ToString().Equals(HostConfig.Instance.ThisNodeID, StringComparison.OrdinalIgnoreCase))
                            {
                                _selfNode = descriptor;
                            }
                            if (node.Id.ToString().Equals(HostConfig.Instance.CenterNodeID, StringComparison.OrdinalIgnoreCase))
                            {
                                _centerNode = descriptor;
                            }
                        }
                        initialized = true;
                    }
                }
            }
        }

        #region MessageHandler
        private class MessageHandler :
            IHandler<AddNodeCommand>,
            IHandler<NodeAddedEvent>,
            IHandler<UpdateNodeCommand>,
            IHandler<NodeUpdatedEvent>,
            IHandler<RemoveNodeCommand>,
            IHandler<NodeRemovedEvent>
        {
            private readonly NodeSet set;

            public MessageHandler(NodeSet set)
            {
                this.set = set;
            }

            public void Handle(AddNodeCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(NodeAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateNodeAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(INodeCreateInput input, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _allNodesByID = set._allNodesByID;
                var _allNodesByPublicKey = set._allNodesByPublicKey;
                var nodeRepository = host.GetRequiredService<IRepository<Node>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                Node entity;
                lock (locker)
                {
                    NodeDescriptor node;
                    if (host.Nodes.TryGetNodeByID(input.Id.Value.ToString(), out node))
                    {
                        throw new ValidationException("已经存在");
                    }
                    if (host.Nodes.Any(a => a.Node.Code.Equals(input.Code)))
                    {
                        throw new ValidationException("重复的编码");
                    }

                    entity = Node.Create(input);

                    var state = new NodeDescriptor(NodeState.Create(entity));
                    _allNodesByID.Add(entity.Id.ToString(), state);
                    _allNodesByPublicKey.Add(entity.PublicKey, state);
                    if (isCommand)
                    {
                        try
                        {
                            nodeRepository.Add(entity);
                            nodeRepository.Context.Commit();
                        }
                        catch
                        {
                            _allNodesByID.Remove(entity.Id.ToString());
                            _allNodesByPublicKey.Remove(entity.PublicKey);
                            nodeRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeAddedEvent(entity, input));
                }
            }

            private class PrivateNodeAddedEvent : NodeAddedEvent
            {
                public PrivateNodeAddedEvent(NodeBase source, INodeCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateNodeCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(NodeUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateNodeUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(INodeUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _allNodesByID = set._allNodesByID;
                var _allNodesByPublicKey = set._allNodesByPublicKey;
                var nodeRepository = host.GetRequiredService<IRepository<Node>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (host.Nodes.Any(a => a.Node.Code.Equals(input.Code) && a.Node.Id != input.Id))
                {
                    throw new ValidationException("重复的编码");
                }
                Node entity;
                bool stateChanged = false;
                lock (locker)
                {
                    NodeDescriptor node;
                    if (!host.Nodes.TryGetNodeByID(input.Id.ToString(), out node))
                    {
                        throw new NotExistException();
                    }
                    entity = nodeRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }
                    var bkState = new NodeDescriptor(NodeState.Create(entity));

                    entity.Update(input);

                    var newState = new NodeDescriptor(NodeState.Create(entity));
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            nodeRepository.Update(entity);
                            nodeRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            nodeRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeUpdatedEvent(entity, input));
                }
            }

            private void Update(NodeDescriptor state)
            {
                var host = set.host;
                var locker = set.locker;
                var _allNodesByID = set._allNodesByID;
                var _allNodesByPublicKey = set._allNodesByPublicKey;
                var oldState = _allNodesByID[state.Node.Id.ToString()];
                _allNodesByID[state.Node.Id.ToString()] = state;
                if (!_allNodesByPublicKey.ContainsKey(state.Node.PublicKey))
                {
                    _allNodesByPublicKey.Add(state.Node.PublicKey, state);
                    _allNodesByPublicKey.Remove(oldState.Node.PublicKey);
                }
                else
                {
                    _allNodesByPublicKey[state.Node.PublicKey] = state;
                }
            }

            private class PrivateNodeUpdatedEvent : NodeUpdatedEvent
            {
                public PrivateNodeUpdatedEvent(NodeBase source, INodeUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveNodeCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(NodeRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateNodeRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid nodeID, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _allNodesByID = set._allNodesByID;
                var _allNodesByPublicKey = set._allNodesByPublicKey;
                var nodeRepository = host.GetRequiredService<IRepository<Node>>();
                NodeDescriptor bkState;
                if (!host.Nodes.TryGetNodeByID(nodeID.ToString(), out bkState))
                {
                    return;
                }
                Node entity;
                lock (locker)
                {
                    entity = nodeRepository.GetByKey(nodeID);
                    if (entity == null)
                    {
                        return;
                    }
                    _allNodesByID.Remove(entity.Id.ToString());
                    _allNodesByPublicKey.Remove(entity.PublicKey);
                    if (isCommand)
                    {
                        try
                        {
                            nodeRepository.Remove(entity);
                            nodeRepository.Context.Commit();
                        }
                        catch
                        {
                            _allNodesByID.Add(entity.Id.ToString(), bkState);
                            _allNodesByPublicKey.Add(entity.PublicKey, bkState);
                            nodeRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeRemovedEvent(entity));
                }
            }

            private class PrivateNodeRemovedEvent : NodeRemovedEvent
            {
                public PrivateNodeRemovedEvent(NodeBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion

        // 内部类
        #region NodeElementActionSet
        private sealed class NodeElementActionSet
        {
            private readonly Dictionary<NodeDescriptor, Dictionary<ElementDescriptor, Dictionary<Verb, NodeElementActionState>>> _nodeElementActionDic = new Dictionary<NodeDescriptor, Dictionary<ElementDescriptor, Dictionary<Verb, NodeElementActionState>>>();
            private bool initialized = false;

            private readonly Guid _id = Guid.NewGuid();
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal NodeElementActionSet(NodeHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.AppHost.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.AppHost.Name));
                }
                var handler = new MessageHandler(this);
                messageDispatcher.Register((IHandler<AddNodeElementActionCommand>)handler);
                messageDispatcher.Register((IHandler<NodeElementActionAddedEvent>)handler);
                messageDispatcher.Register((IHandler<RemoveNodeElementActionCommand>)handler);
                messageDispatcher.Register((IHandler<NodeElementActionRemovedEvent>)handler);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="element"></param>
            /// <returns></returns>
            public Dictionary<Verb, NodeElementActionState> this[NodeDescriptor node, ElementDescriptor element]
            {
                get
                {
                    if (!initialized)
                    {
                        Init();
                    }
                    if (!_nodeElementActionDic.ContainsKey(node))
                    {
                        return new Dictionary<Verb, NodeElementActionState>();
                    }
                    if (!_nodeElementActionDic[node].ContainsKey(element))
                    {
                        return new Dictionary<Verb, NodeElementActionState>();
                    }

                    return _nodeElementActionDic[node][element];
                }
            }

            public void Refresh()
            {
                if (initialized)
                {
                    initialized = false;
                }
            }

            #region Init
            /// <summary>
            /// 初始化信息分组上下文
            /// </summary>
            private void Init()
            {
                if (!initialized)
                {
                    lock (this)
                    {
                        if (!initialized)
                        {
                            _nodeElementActionDic.Clear();
                            var nodeElementActions = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetNodeElementActions();
                            foreach (var item in nodeElementActions)
                            {
                                NodeDescriptor node;
                                host.Nodes.TryGetNodeByID(item.NodeID.ToString(), out node);
                                ElementDescriptor element = OntologyDescriptor.SingleElement(item.ElementID);
                                if (!_nodeElementActionDic.ContainsKey(node))
                                {
                                    _nodeElementActionDic.Add(node, new Dictionary<ElementDescriptor, Dictionary<Verb, NodeElementActionState>>());
                                }
                                if (!_nodeElementActionDic[node].ContainsKey(element))
                                {
                                    _nodeElementActionDic[node].Add(element, new Dictionary<Verb, NodeElementActionState>());
                                }
                                var state = NodeElementActionState.Create(item);
                                var action = element.Ontology.Actions.Values.First(a => a.Id == item.ActionID);
                                _nodeElementActionDic[node][element].Add(action.ActionVerb, state);
                            }
                            initialized = true;
                        }
                    }
                }
            }

            #endregion

            private class MessageHandler :
                IHandler<AddNodeElementActionCommand>,
                IHandler<NodeElementActionAddedEvent>,
                IHandler<RemoveNodeElementActionCommand>,
                IHandler<NodeElementActionRemovedEvent>
            {
                private readonly NodeElementActionSet set;

                public MessageHandler(NodeElementActionSet set)
                {
                    this.set = set;
                }

                public void Handle(AddNodeElementActionCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(NodeElementActionAddedEvent message)
                {
                    if (message.GetType() == typeof(PrivateNodeElementActionAddedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(INodeElementActionCreateInput input, bool isCommand)
                {
                    var host = set.host;
                    var _nodeElementActionDic = set._nodeElementActionDic;
                    var repository = host.AppHost.GetRequiredService<IRepository<NodeElementAction>>();
                    NodeElementAction entity;
                    lock (this)
                    {
                        NodeDescriptor node;
                        if (!host.Nodes.TryGetNodeByID(input.NodeID.ToString(), out node))
                        {
                            throw new ValidationException("意外的节点标识" + input.NodeID);
                        }
                        ElementDescriptor element;
                        if (!host.Ontologies.TryGetElement(input.ElementID, out element))
                        {
                            throw new ValidationException("意外的本体元素标识" + input.ElementID);
                        }
                        if (!_nodeElementActionDic.ContainsKey(node))
                        {
                            _nodeElementActionDic.Add(node, new Dictionary<ElementDescriptor, Dictionary<Verb, NodeElementActionState>>());
                        }
                        if (!_nodeElementActionDic[node].ContainsKey(element))
                        {
                            _nodeElementActionDic[node].Add(element, new Dictionary<Verb, NodeElementActionState>());
                        }
                        entity = new NodeElementAction
                        {
                            Id = input.Id.Value,
                            ActionID = input.ActionID,
                            ElementID = input.ElementID,
                            IsAllowed = input.IsAllowed,
                            IsAudit = input.IsAudit,
                            NodeID = input.NodeID
                        };
                        var state = NodeElementActionState.Create(entity);
                        var action = element.Ontology.Actions.Values.FirstOrDefault(a => a.Id == input.ActionID);
                        if (action == null)
                        {
                            throw new ValidationException("意外的本体动作标识" + input.ActionID);
                        }
                        _nodeElementActionDic[node][element].Add(action.ActionVerb, state);
                        if (isCommand)
                        {
                            try
                            {
                                repository.Add(entity);
                                repository.Context.Commit();
                            }
                            catch
                            {
                                if (_nodeElementActionDic.ContainsKey(node) && _nodeElementActionDic[node].ContainsKey(element) && _nodeElementActionDic[node][element].ContainsKey(action.ActionVerb))
                                {
                                    _nodeElementActionDic[node][element].Remove(action.ActionVerb);
                                }
                                repository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeElementActionAddedEvent(entity, input));
                    }
                }

                private class PrivateNodeElementActionAddedEvent : NodeElementActionAddedEvent
                {
                    public PrivateNodeElementActionAddedEvent(NodeElementActionBase source, INodeElementActionCreateInput input)
                        : base(source, input)
                    {

                    }
                }

                public void Handle(RemoveNodeElementActionCommand message)
                {
                    this.Handle(message.EntityID, true);
                }

                public void Handle(NodeElementActionRemovedEvent message)
                {
                    if (message.GetType() == typeof(PrivateNodeElementActionRemovedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Source.Id, false);
                }

                private void Handle(Guid nodeElementActionID, bool isCommand)
                {
                    var host = set.host;
                    var _nodeElementActionDic = set._nodeElementActionDic;
                    var repository = host.AppHost.GetRequiredService<IRepository<NodeElementAction>>();
                    NodeElementAction entity;
                    lock (this)
                    {
                        bool exist = false;
                        NodeElementActionState bkState = null;
                        NodeDescriptor node = null;
                        ElementDescriptor element = null;
                        foreach (var item in _nodeElementActionDic)
                        {
                            foreach (var item1 in item.Value)
                            {
                                foreach (var item2 in item1.Value)
                                {
                                    if (item2.Value.Id == nodeElementActionID)
                                    {
                                        exist = true;
                                        bkState = item2.Value;
                                        break;
                                    }
                                }
                                if (exist)
                                {
                                    element = item1.Key;
                                    break;
                                }
                            }
                            if (exist)
                            {
                                node = item.Key;
                                break;
                            }
                        }
                        if (!exist)
                        {
                            return;
                        }
                        entity = repository.GetByKey(nodeElementActionID);
                        if (entity == null)
                        {
                            return;
                        }
                        if (_nodeElementActionDic.ContainsKey(node) && _nodeElementActionDic[node].ContainsKey(element))
                        {
                            var action = element.Ontology.Actions.Values.FirstOrDefault(a => a.Id == entity.ActionID);
                            _nodeElementActionDic[node][element].Remove(action.ActionVerb);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                repository.Remove(entity);
                                repository.Context.Commit();
                            }
                            catch
                            {
                                var action = element.Ontology.Actions.Values.FirstOrDefault(a => a.Id == entity.ActionID);
                                if (_nodeElementActionDic.ContainsKey(node) && _nodeElementActionDic[node].ContainsKey(element) && !_nodeElementActionDic[node][element].ContainsKey(action.ActionVerb))
                                {
                                    _nodeElementActionDic[node][element].Add(action.ActionVerb, bkState);
                                }
                                repository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeElementActionRemovedEvent(entity));
                    }
                }

                private class PrivateNodeElementActionRemovedEvent : NodeElementActionRemovedEvent
                {
                    public PrivateNodeElementActionRemovedEvent(NodeElementAction source)
                        : base(source)
                    {

                    }
                }
            }
        }
        #endregion

        // 内部类
        #region NodeCareSet
        /// <summary>
        /// 节点关心本体和节点关心本体元素
        /// </summary>
        private sealed class NodeCareSet
        {
            private readonly Dictionary<NodeDescriptor, IDictionary<ontologyID, isCare>> ontologyCareDic = new Dictionary<NodeDescriptor, IDictionary<ontologyID, isCare>>();
            private readonly Dictionary<NodeDescriptor, IDictionary<elementID, isCare>> elementCareDic = new Dictionary<NodeDescriptor, IDictionary<elementID, isCare>>();
            private readonly Dictionary<NodeDescriptor, List<NodeOntologyCareState>> nodeOntologyCareList = new Dictionary<NodeDescriptor, List<NodeOntologyCareState>>();
            private readonly Dictionary<NodeDescriptor, List<NodeElementCareState>> nodeElementCareList = new Dictionary<NodeDescriptor, List<NodeElementCareState>>();
            private readonly Dictionary<NodeDescriptor, HashSet<ElementDescriptor>> nodeInfoIDElements = new Dictionary<NodeDescriptor, HashSet<ElementDescriptor>>();
            private bool initialized = false;
            private readonly object _locker = new object();
            private readonly Guid _id = Guid.NewGuid();
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal NodeCareSet(NodeHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.AppHost.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.AppHost.Name));
                }
                var handler = new MessageHandler(this);
                messageDispatcher.Register((IHandler<AddNodeOntologyCareCommand>)handler);
                messageDispatcher.Register((IHandler<NodeOntologyCareAddedEvent>)handler);
                messageDispatcher.Register((IHandler<RemoveNodeOntologyCareCommand>)handler);
                messageDispatcher.Register((IHandler<NodeOntologyCareRemovedEvent>)handler);
                messageDispatcher.Register((IHandler<AddNodeElementCareCommand>)handler);
                messageDispatcher.Register((IHandler<NodeElementCareAddedEvent>)handler);
                messageDispatcher.Register((IHandler<UpdateNodeElementCareCommand>)handler);
                messageDispatcher.Register((IHandler<NodeElementCareUpdatedEvent>)handler);
                messageDispatcher.Register((IHandler<RemoveNodeElementCareCommand>)handler);
                messageDispatcher.Register((IHandler<NodeElementCareRemovedEvent>)handler);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            public IEnumerable<ElementDescriptor> GetInfoIDElements(NodeDescriptor node)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }
                if (!initialized)
                {
                    Init();
                }
                return nodeInfoIDElements[node];
            }

            public bool IsInfoIDElement(NodeDescriptor node, ElementDescriptor element)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                if (!initialized)
                {
                    Init();
                }
                return nodeInfoIDElements.ContainsKey(node) && nodeInfoIDElements[node].Contains(element);
            }

            /// <summary>
            /// 判断本节点是否关心给定的本体元素
            /// </summary>
            /// <param name="node"></param>
            /// <param name="element">本体元素码</param>
            /// <returns>True表示关心，False表示不关心</returns>
            public bool IsCareforElement(NodeDescriptor node, ElementDescriptor element)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                if (!initialized)
                {
                    Init();
                }
                if (!elementCareDic.ContainsKey(node))
                {
                    return false;
                }
                if (!ontologyCareDic[node].ContainsKey(element.Element.OntologyID))
                {
                    return false;
                }
                if (!elementCareDic[node].ContainsKey(element.Element.Id))
                {
                    return false;
                }

                return elementCareDic[node][element.Element.Id];
            }

            /// <summary>
            /// 判断本节点是否关心给定的本体
            /// </summary>
            /// <param name="node"></param>
            /// <param name="ontology"></param>
            /// <returns>True表示关心，False表示不关心</returns>
            public bool IsCareForOntology(NodeDescriptor node, OntologyDescriptor ontology)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }
                if (ontology == null)
                {
                    throw new ArgumentNullException("ontology");
                }
                if (!initialized)
                {
                    Init();
                }
                if (!ontologyCareDic.ContainsKey(node))
                {
                    return false;
                }
                if (!ontologyCareDic[node].ContainsKey(ontology.Ontology.Id))
                {
                    return false;
                }

                return ontologyCareDic[node][ontology.Ontology.Id];
            }

            public IReadOnlyCollection<NodeOntologyCareState> GetNodeOntologyCares(NodeDescriptor node)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }
                if (!initialized)
                {
                    Init();
                }
                if (!nodeOntologyCareList.ContainsKey(node))
                {
                    return new List<NodeOntologyCareState>();
                }
                return nodeOntologyCareList[node];
            }

            public IEnumerable<NodeOntologyCareState> GetNodeOntologyCares()
            {
                if (!initialized)
                {
                    Init();
                }
                foreach (var g in nodeOntologyCareList)
                {
                    foreach (var item in g.Value)
                    {
                        yield return item;
                    }
                }
            }

            public IReadOnlyCollection<NodeElementCareState> GetNodeElementCares(NodeDescriptor node)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }
                if (!initialized)
                {
                    Init();
                }
                if (!nodeElementCareList.ContainsKey(node))
                {
                    return new List<NodeElementCareState>();
                }
                return nodeElementCareList[node];
            }

            #region Init
            private void Init()
            {
                if (!initialized)
                {
                    lock (_locker)
                    {
                        if (!initialized)
                        {
                            ontologyCareDic.Clear();
                            elementCareDic.Clear();
                            nodeOntologyCareList.Clear();
                            nodeElementCareList.Clear();
                            nodeInfoIDElements.Clear();
                            var nodeOntologyCareStates = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetNodeOntologyCares().Select(a => NodeOntologyCareState.Create(a));
                            var nodeElementCareStates = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetNodeElementCares().Select(a => NodeElementCareState.Create(a));
                            foreach (var node in NodeHost.Instance.Nodes)
                            {
                                var node1 = node;
                                nodeOntologyCareList.Add(node, nodeOntologyCareStates.Where(a => a.NodeID == node1.Node.Id).ToList());
                                var node2 = node;
                                nodeElementCareList.Add(node, nodeElementCareStates.Where(a => a.NodeID == node2.Node.Id).ToList());
                            }

                            foreach (var ontology in NodeHost.Instance.Ontologies)
                            {
                                foreach (var element in NodeHost.Instance.Ontologies[ontology.Ontology.Id].Elements.Values)
                                {
                                    foreach (var node in NodeHost.Instance.Nodes)
                                    {
                                        if (element == null)
                                        {
                                            return;
                                        }
                                        if (!ontologyCareDic.ContainsKey(node))
                                        {
                                            ontologyCareDic.Add(node, new Dictionary<ontologyID, isCare>());
                                        }
                                        if (!ontologyCareDic[node].ContainsKey(element.Element.OntologyID))
                                        {
                                            var element1 = element;
                                            ontologyCareDic[node].Add(element.Element.OntologyID, nodeOntologyCareList[node]
                                                .Any(s => s.OntologyID == element1.Element.OntologyID));
                                        }
                                        if (!elementCareDic.ContainsKey(node))
                                        {
                                            elementCareDic.Add(node, new Dictionary<elementID, isCare>());
                                        }
                                        if (!nodeInfoIDElements.ContainsKey(node))
                                        {
                                            nodeInfoIDElements.Add(node, new HashSet<ElementDescriptor>());
                                            nodeInfoIDElements[node].Add(ontology.IdElement);
                                        }
                                        if (!elementCareDic[node].ContainsKey(element.Element.Id))
                                        {
                                            var element2 = element;
                                            var nodeElementCare = nodeElementCareList[node].FirstOrDefault(f => f.ElementID == element2.Element.Id);
                                            elementCareDic[node].Add(element.Element.Id, nodeElementCare != null);
                                            if (nodeElementCare != null && nodeElementCare.IsInfoIDItem)
                                            {
                                                nodeInfoIDElements[node].Add(element);
                                            }
                                        }
                                    }
                                }
                            }
                            initialized = true;
                        }
                    }
                }
            }
            #endregion

            private class MessageHandler :
                IHandler<AddNodeOntologyCareCommand>,
                IHandler<NodeOntologyCareAddedEvent>,
                IHandler<RemoveNodeOntologyCareCommand>,
                IHandler<NodeOntologyCareRemovedEvent>,
                IHandler<AddNodeElementCareCommand>,
                IHandler<NodeElementCareAddedEvent>,
                IHandler<UpdateNodeElementCareCommand>,
                IHandler<NodeElementCareUpdatedEvent>,
                IHandler<RemoveNodeElementCareCommand>,
                IHandler<NodeElementCareRemovedEvent>
            {
                private readonly NodeCareSet set;

                public MessageHandler(NodeCareSet set)
                {
                    this.set = set;
                }

                public void Handle(AddNodeOntologyCareCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(NodeOntologyCareAddedEvent message)
                {
                    if (message.GetType() == typeof(PrivateNodeOntologyCareAddedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(INodeOntologyCareCreateInput input, bool isCommand)
                {
                    var host = set.host;
                    var nodeOntologyCareList = set.nodeOntologyCareList;
                    var ontologyCareDic = set.ontologyCareDic;
                    var repository = host.AppHost.GetRequiredService<IRepository<NodeOntologyCare>>();
                    NodeDescriptor bNode;
                    if (!host.Nodes.TryGetNodeByID(input.NodeID.ToString(), out bNode))
                    {
                        throw new ValidationException("意外的节点标识" + input.NodeID);
                    }
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(input.OntologyID, out ontology))
                    {
                        throw new ValidationException("意外的本体标识" + input.OntologyID);
                    }
                    NodeOntologyCare entity;
                    lock (this)
                    {
                        if (nodeOntologyCareList[bNode].Any(a => a.OntologyID == input.OntologyID && a.NodeID == input.NodeID))
                        {
                            throw new ValidationException("给定的节点已关心给定的本体，无需重复关心");
                        }
                        entity = NodeOntologyCare.Create(input);
                        var state = NodeOntologyCareState.Create(entity);
                        if (!nodeOntologyCareList.ContainsKey(bNode))
                        {
                            nodeOntologyCareList.Add(bNode, new List<NodeOntologyCareState>());
                        }
                        if (!nodeOntologyCareList[bNode].Contains(state))
                        {
                            nodeOntologyCareList[bNode].Add(state);
                        }
                        if (!ontologyCareDic.ContainsKey(bNode))
                        {
                            ontologyCareDic.Add(bNode, new Dictionary<ontologyID, isCare>());
                        }
                        if (!ontologyCareDic[bNode].ContainsKey(input.OntologyID))
                        {
                            ontologyCareDic[bNode].Add(input.OntologyID, true);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                repository.Add(entity);
                                repository.Context.Commit();
                            }
                            catch
                            {
                                if (nodeOntologyCareList.ContainsKey(bNode) && nodeOntologyCareList[bNode].Contains(state))
                                {
                                    nodeOntologyCareList[bNode].Remove(state);
                                }
                                if (ontologyCareDic.ContainsKey(bNode) && ontologyCareDic[bNode].ContainsKey(input.OntologyID))
                                {
                                    ontologyCareDic[bNode].Remove(input.OntologyID);
                                }
                                repository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeOntologyCareAddedEvent(entity, input));
                    }
                }

                private class PrivateNodeOntologyCareAddedEvent : NodeOntologyCareAddedEvent
                {
                    public PrivateNodeOntologyCareAddedEvent(NodeOntologyCareBase source, INodeOntologyCareCreateInput input)
                        : base(source, input)
                    {

                    }
                }

                public void Handle(RemoveNodeOntologyCareCommand message)
                {
                    this.Handle(message.EntityID, true);
                }

                public void Handle(NodeOntologyCareRemovedEvent message)
                {
                    if (message.GetType() == typeof(PrivateNodeOntologyCareRemovedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Source.Id, false);
                }

                private void Handle(Guid nodeOntologyCareID, bool isCommand)
                {
                    var host = set.host;
                    var nodeOntologyCareList = set.nodeOntologyCareList;
                    var ontologyCareDic = set.ontologyCareDic;
                    var repository = host.AppHost.GetRequiredService<IRepository<NodeOntologyCare>>();
                    NodeOntologyCare entity;
                    lock (this)
                    {
                        NodeOntologyCareState bkState = null;
                        NodeDescriptor bNode = null;
                        foreach (var item in nodeOntologyCareList)
                        {
                            foreach (var item1 in item.Value)
                            {
                                if (item1.Id == nodeOntologyCareID)
                                {
                                    bkState = item1;
                                    break;
                                }
                            }
                            if (bkState != null)
                            {
                                bNode = item.Key;
                            }
                        }
                        if (bkState == null)
                        {
                            return;
                        }
                        entity = repository.GetByKey(nodeOntologyCareID);
                        if (entity == null)
                        {
                            return;
                        }
                        nodeOntologyCareList[bNode].Remove(bkState);
                        ontologyCareDic[bNode].Remove(bkState.OntologyID);
                        try
                        {
                            if (isCommand)
                            {
                                repository.Remove(entity);
                                repository.Context.Commit();
                            }
                        }
                        catch
                        {
                            nodeOntologyCareList[bNode].Add(bkState);
                            ontologyCareDic[bNode].Add(bkState.OntologyID, true);
                            repository.Context.Rollback();
                            throw;
                        }
                    }

                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeOntologyCareRemovedEvent(entity));
                    }
                }

                private class PrivateNodeOntologyCareRemovedEvent : NodeOntologyCareRemovedEvent
                {
                    public PrivateNodeOntologyCareRemovedEvent(NodeOntologyCareBase source) : base(source) { }

                }

                public void Handle(AddNodeElementCareCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(NodeElementCareAddedEvent message)
                {
                    if (message.GetType() == typeof(PrivateNodeElementCareAddedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(INodeElementCareCreateInput input, bool isCommand)
                {
                    var host = set.host;
                    var nodeElementCareList = set.nodeElementCareList;
                    var elementCareDic = set.elementCareDic;
                    var repository = host.AppHost.GetRequiredService<IRepository<NodeElementCare>>();
                    NodeDescriptor bNode;
                    if (!host.Nodes.TryGetNodeByID(input.NodeID.ToString(), out bNode))
                    {
                        throw new ValidationException("意外的节点标识" + input.NodeID);
                    }
                    ElementDescriptor element;
                    if (!host.Ontologies.TryGetElement(input.ElementID, out element))
                    {
                        throw new ValidationException("意外的本体元素标识" + input.ElementID);
                    }
                    NodeElementCare entity;
                    lock (this)
                    {
                        if (nodeElementCareList[bNode].Any(a => a.ElementID == input.ElementID && a.NodeID == input.NodeID))
                        {
                            throw new ValidationException("给定的节点已关心给定的本体元素，无需重复关心");
                        }
                        entity = NodeElementCare.Create(input);
                        var state = NodeElementCareState.Create(entity);
                        if (!nodeElementCareList.ContainsKey(bNode))
                        {
                            nodeElementCareList.Add(bNode, new List<NodeElementCareState>());
                        }
                        if (!nodeElementCareList[bNode].Contains(state))
                        {
                            nodeElementCareList[bNode].Add(state);
                        }
                        if (!elementCareDic.ContainsKey(bNode))
                        {
                            elementCareDic.Add(bNode, new Dictionary<elementID, isCare>());
                        }
                        if (!elementCareDic[bNode].ContainsKey(input.ElementID))
                        {
                            elementCareDic[bNode].Add(input.ElementID, true);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                repository.Add(entity);
                                repository.Context.Commit();
                            }
                            catch
                            {
                                if (nodeElementCareList.ContainsKey(bNode) && nodeElementCareList[bNode].Contains(state))
                                {
                                    nodeElementCareList[bNode].Remove(state);
                                }
                                if (elementCareDic.ContainsKey(bNode) && elementCareDic[bNode].ContainsKey(input.ElementID))
                                {
                                    elementCareDic[bNode].Remove(input.ElementID);
                                }
                                repository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeElementCareAddedEvent(entity, input));
                    }
                }

                private class PrivateNodeElementCareAddedEvent : NodeElementCareAddedEvent
                {
                    public PrivateNodeElementCareAddedEvent(NodeElementCareBase source, INodeElementCareCreateInput input)
                        : base(source, input)
                    {

                    }
                }

                public void Handle(UpdateNodeElementCareCommand message)
                {
                    this.Handle(message.NodeElementCareID, message.IsInfoIDItem, true);
                }

                public void Handle(NodeElementCareUpdatedEvent message)
                {
                    if (message.GetType() == typeof(PrivateNodeElementCareUpdatedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Source.Id, message.IsInfoIDItem, false);
                }

                private void Handle(Guid nodeElementCareID, bool isInfoIDItem, bool isCommand)
                {
                    var host = set.host;
                    var nodeElementCareList = set.nodeElementCareList;
                    var elementCareDic = set.elementCareDic;
                    var nodeInfoIDElements = set.nodeInfoIDElements;
                    var repository = host.AppHost.GetRequiredService<IRepository<NodeElementCare>>();
                    NodeElementCare entity;
                    lock (this)
                    {
                        NodeElementCareState bkState = null;
                        NodeDescriptor bNode = null;
                        foreach (var item in nodeElementCareList)
                        {
                            foreach (var item1 in item.Value)
                            {
                                if (item1.Id == nodeElementCareID)
                                {
                                    bkState = item1;
                                    break;
                                }
                            }
                            if (bkState != null)
                            {
                                bNode = item.Key;
                            }
                        }
                        if (bkState == null)
                        {
                            throw new NotExistException();
                        }
                        entity = repository.GetByKey(nodeElementCareID);
                        if (entity == null)
                        {
                            throw new NotExistException();
                        }
                        ElementDescriptor element;
                        if (!host.Ontologies.TryGetElement(entity.ElementID, out element))
                        {
                            throw new ValidationException("意外的本体元素标识" + entity.ElementID);
                        }
                        entity.IsInfoIDItem = isInfoIDItem;
                        var newState = NodeElementCareState.Create(entity);
                        nodeElementCareList[bNode].Remove(bkState);
                        nodeElementCareList[bNode].Add(newState);
                        nodeInfoIDElements[bNode].Add(element);
                        try
                        {
                            if (isCommand)
                            {
                                repository.Update(entity);
                                repository.Context.Commit();
                            }
                        }
                        catch
                        {
                            nodeElementCareList[bNode].Remove(newState);
                            nodeElementCareList[bNode].Add(bkState);
                            nodeInfoIDElements[bNode].Remove(element);
                            repository.Context.Rollback();
                            throw;
                        }
                    }

                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeElementCareUpdatedEvent(entity));
                    }
                }

                private class PrivateNodeElementCareUpdatedEvent : NodeElementCareUpdatedEvent
                {
                    public PrivateNodeElementCareUpdatedEvent(NodeElementCareBase source)
                        : base(source)
                    {

                    }
                }

                public void Handle(RemoveNodeElementCareCommand message)
                {
                    this.Handle(message.EntityID, true);
                }

                public void Handle(NodeElementCareRemovedEvent message)
                {
                    if (message.GetType() == typeof(PrivateNodeElementCareRemovedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Source.Id, false);
                }

                private void HandleElement(Guid nodeElementCareID, bool isCommand)
                {
                    var host = set.host;
                    var nodeElementCareList = set.nodeElementCareList;
                    var elementCareDic = set.elementCareDic;
                    var nodeInfoIDElements = set.nodeInfoIDElements;
                    var repository = host.AppHost.GetRequiredService<IRepository<NodeElementCare>>();
                    NodeElementCare entity;
                    lock (this)
                    {
                        NodeElementCareState bkState = null;
                        NodeDescriptor bNode = null;
                        foreach (var item in nodeElementCareList)
                        {
                            foreach (var item1 in item.Value)
                            {
                                if (item1.Id == nodeElementCareID)
                                {
                                    bkState = item1;
                                    break;
                                }
                            }
                            if (bkState != null)
                            {
                                bNode = item.Key;
                            }
                        }
                        if (bkState == null)
                        {
                            return;
                        }
                        entity = repository.GetByKey(nodeElementCareID);
                        if (entity == null)
                        {
                            return;
                        }
                        ElementDescriptor element;
                        if (!host.Ontologies.TryGetElement(entity.ElementID, out element))
                        {
                            throw new ValidationException("意外的本体元素标识" + entity.ElementID);
                        }
                        nodeElementCareList[bNode].Remove(bkState);
                        elementCareDic[bNode].Remove(bkState.ElementID);
                        bool isInfoIDElement = false;
                        if (nodeInfoIDElements.ContainsKey(bNode) && nodeInfoIDElements[bNode].Contains(element))
                        {
                            isInfoIDElement = true;
                            nodeInfoIDElements[bNode].Remove(element);
                        }
                        try
                        {
                            if (isCommand)
                            {
                                repository.Remove(entity);
                                repository.Context.Commit();
                            }
                        }
                        catch
                        {
                            nodeElementCareList[bNode].Add(bkState);
                            elementCareDic[bNode].Add(bkState.ElementID, true);
                            if (isInfoIDElement)
                            {
                                nodeInfoIDElements[bNode].Add(element);
                            }
                            repository.Context.Rollback();
                            throw;
                        }
                    }

                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateNodeElementCareRemovedEvent(entity));
                    }
                }

                private class PrivateNodeElementCareRemovedEvent : NodeElementCareRemovedEvent
                {
                    public PrivateNodeElementCareRemovedEvent(NodeElementCareBase source) : base(source) { }

                }
            }
        }
        #endregion

        // 内部类
        #region OrganizationSet
        private sealed class OrganizationSet
        {
            private readonly Dictionary<NodeDescriptor, Dictionary<OntologyDescriptor, Dictionary<OrganizationState, NodeOntologyOrganizationState>>>
                _dic = new Dictionary<NodeDescriptor, Dictionary<OntologyDescriptor, Dictionary<OrganizationState, NodeOntologyOrganizationState>>>();
            private bool initialized = false;
            private object locker = new object();
            private readonly Guid _id = Guid.NewGuid();
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            public OrganizationSet(NodeHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.AppHost.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.AppHost.Name));
                }
                // TODO:接入总线
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name = "node"></param>
            /// <param name="ontology"></param>
            /// <returns>key为组织结构码</returns>
            public Dictionary<OrganizationState, NodeOntologyOrganizationState> this[NodeDescriptor node, OntologyDescriptor ontology]
            {
                get
                {
                    if (node == null)
                    {
                        throw new ArgumentNullException("node");
                    }
                    if (ontology == null)
                    {
                        throw new ArgumentNullException("ontology");
                    }
                    if (!initialized)
                    {
                        Init();
                    }
                    if (!_dic.ContainsKey(node))
                    {
                        return new Dictionary<OrganizationState, NodeOntologyOrganizationState>();
                    }
                    if (!_dic[node].ContainsKey(ontology))
                    {
                        return new Dictionary<OrganizationState, NodeOntologyOrganizationState>();
                    }

                    return _dic[node][ontology];
                }
            }

            public IEnumerable<NodeOntologyOrganizationState> GetNodeOntologyOrganizations()
            {
                if (!initialized)
                {
                    Init();
                }
                foreach (var gg in _dic.Values)
                {
                    foreach (var g in gg.Values)
                    {
                        foreach (var item in g.Values)
                        {
                            yield return item;
                        }
                    }
                }
            }

            private void Init()
            {
                if (!initialized)
                {
                    lock (locker)
                    {
                        if (!initialized)
                        {
                            _dic.Clear();
                            var ontologyOrgs = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetNodeOntologyOrganizations();
                            foreach (var nodeOntologyOrg in ontologyOrgs)
                            {
                                OrganizationState org;
                                NodeDescriptor node;
                                OntologyDescriptor ontology;
                                NodeHost.Instance.Nodes.TryGetNodeByID(nodeOntologyOrg.NodeID.ToString(), out node);
                                NodeHost.Instance.Ontologies.TryGetOntology(nodeOntologyOrg.OntologyID, out ontology);
                                if (NodeHost.Instance.AppHost.OrganizationSet.TryGetOrganization(nodeOntologyOrg.OrganizationID, out org))
                                {
                                    if (!_dic.ContainsKey(node))
                                    {
                                        _dic.Add(node, new Dictionary<OntologyDescriptor, Dictionary<OrganizationState, NodeOntologyOrganizationState>>());
                                    }
                                    if (!_dic[node].ContainsKey(ontology))
                                    {
                                        _dic[node].Add(ontology, new Dictionary<OrganizationState, NodeOntologyOrganizationState>());
                                    }
                                    var nodeOntologyOrgState = NodeOntologyOrganizationState.Create(nodeOntologyOrg);
                                    _dic[node][ontology].Add(org, nodeOntologyOrgState);
                                }
                                else
                                {
                                    // TODO:移除废弃的组织结构
                                }
                            }
                            initialized = true;
                        }
                    }
                }
            }
        }
        #endregion
    }
}