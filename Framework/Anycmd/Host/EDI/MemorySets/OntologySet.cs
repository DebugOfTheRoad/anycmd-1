using System;

namespace Anycmd.Host.EDI.MemorySets
{
    using Anycmd.EDI;
    using Anycmd.Rdb;
    using Bus;
    using Entities;
    using Exceptions;
    using Extensions;
    using Hecp;
    using Host;
    using Messages;
    using Model;
    using Repositories;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Util;
    using ValueObjects;
    using ontologyID = System.Guid;
    using organizationID = System.Guid;
    using topicCode = System.String;

    /// <summary>
    /// 
    /// </summary>
    public sealed class OntologySet : IOntologySet
    {
        private readonly Dictionary<string, OntologyDescriptor> _ontologyDicByCode = new Dictionary<string, OntologyDescriptor>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<ontologyID, OntologyDescriptor> _ontologyDicByID = new Dictionary<Guid, OntologyDescriptor>();
        private bool _initialized = false;
        private object locker = new object();

        private readonly Guid _id = Guid.NewGuid();
        private readonly ElementSet elementSet;
        private readonly ActionSet actionSet;
        private readonly InfoGroupSet infoGroupSet;
        private readonly OntologyOrganizationSet organizationSet;
        private readonly TopicSet topics;
        private readonly ArchiveSet archiveSet;

        private readonly NodeHost host;

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        #region Ctor
        /// <summary>
        /// 构造并接入总线
        /// </summary>
        public OntologySet(NodeHost host)
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
            messageDispatcher.Register((IHandler<AddOntologyCommand>)handler);
            messageDispatcher.Register((IHandler<OntologyAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateOntologyCommand>)handler);
            messageDispatcher.Register((IHandler<OntologyUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveOntologyCommand>)handler);
            this.elementSet = new ElementSet(host);
            this.actionSet = new ActionSet(host);
            this.infoGroupSet = new InfoGroupSet(host);
            this.organizationSet = new OntologyOrganizationSet(host);
            this.topics = new TopicSet(host);
            this.archiveSet = new ArchiveSet(host);
        }
        #endregion

        #region this[string ontologyCode]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ontologyCode"></param>
        /// <returns></returns>
        /// <exception cref="CoreException">当本体码非法时抛出</exception>
        public OntologyDescriptor this[string ontologyCode]
        {
            get
            {
                if (ontologyCode == null)
                {
                    throw new ArgumentNullException("ontologyCode");
                }
                if (!_initialized)
                {
                    Init();
                }
                if (!_ontologyDicByCode.ContainsKey(ontologyCode))
                {
                    throw new CoreException("意外的本体码");
                }

                return _ontologyDicByCode[ontologyCode];
            }
        }
        #endregion

        #region this[Guid ontologyID]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <returns></returns>
        /// <exception cref="CoreException">当本体标识非法时抛出</exception>
        public OntologyDescriptor this[ontologyID ontologyID]
        {
            get
            {
                if (!_initialized)
                {
                    Init();
                }
                if (!_ontologyDicByID.ContainsKey(ontologyID))
                {
                    throw new CoreException("意外的本体ID");
                }

                return _ontologyDicByID[ontologyID];
            }
        }
        #endregion

        #region TryGetOntology
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ontologyCode"></param>
        /// <param name="ontology"></param>
        /// <returns></returns>
        public bool TryGetOntology(string ontologyCode, out OntologyDescriptor ontology)
        {
            if (ontologyCode == null)
            {
                throw new ArgumentNullException("ontologyCode");
            }
            if (!_initialized)
            {
                Init();
            }
            if (ontologyCode == null)
            {
                ontology = null;
                return false;
            }
            return _ontologyDicByCode.TryGetValue(ontologyCode, out ontology);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <param name="ontology"></param>
        /// <returns></returns>
        public bool TryGetOntology(ontologyID ontologyID, out OntologyDescriptor ontology)
        {
            if (!_initialized)
            {
                Init();
            }

            return _ontologyDicByID.TryGetValue(ontologyID, out ontology);
        }
        #endregion

        #region TryGetElement
        public bool TryGetElement(ontologyID elementID, out ElementDescriptor element)
        {
            if (!_initialized)
            {
                Init();
            }
            return elementSet.TryGetElement(elementID, out element);
        }
        #endregion

        #region GetElement
        public ElementDescriptor GetElement(ontologyID elementID)
        {
            if (!_initialized)
            {
                Init();
            }
            return elementSet.GetElement(elementID);
        }
        #endregion

        #region GetElements
        public IReadOnlyDictionary<string, ElementDescriptor> GetElements(OntologyDescriptor ontology)
        {
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            if (!_initialized)
            {
                Init();
            }
            return elementSet.GetElements(ontology);
        }
        #endregion

        #region GetActons
        public IReadOnlyDictionary<Verb, ActionState> GetActons(OntologyDescriptor ontology)
        {
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            if (!_initialized)
            {
                Init();
            }
            return actionSet.GetActons(ontology);
        }
        #endregion

        #region GetAction
        public ActionState GetAction(Guid actionID)
        {
            if (!_initialized)
            {
                Init();
            }
            return actionSet.GetAction(actionID);
        }
        #endregion

        #region GetInfoGroups
        public IList<InfoGroupState> GetInfoGroups(OntologyDescriptor ontology)
        {
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            if (!_initialized)
            {
                Init();
            }
            return infoGroupSet.GetInfoGroups(ontology);
        }
        #endregion

        #region GetOntologyOrganizations
        public IReadOnlyDictionary<OrganizationState, OntologyOrganizationState> GetOntologyOrganizations(OntologyDescriptor ontology)
        {
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            if (!_initialized)
            {
                Init();
            }
            return organizationSet[ontology];
        }
        #endregion

        #region GetEventSubjects
        public IReadOnlyDictionary<string, TopicState> GetEventSubjects(OntologyDescriptor ontology)
        {
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            if (!_initialized)
            {
                Init();
            }
            return topics[ontology];
        }
        #endregion

        #region TryGetArchive
        public bool TryGetArchive(Guid archiveID, out ArchiveState archive)
        {
            if (!_initialized)
            {
                Init();
            }
            return archiveSet.TryGetArchive(archiveID, out archive);
        }
        #endregion

        #region GetArchives
        public IReadOnlyCollection<ArchiveState> GetArchives(OntologyDescriptor ontology)
        {
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            if (!_initialized)
            {
                Init();
            }
            return archiveSet.GetArchives(ontology);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        internal void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
        }

        #region IEnumerator
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator<OntologyDescriptor> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _ontologyDicByCode.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _ontologyDicByCode.Values.GetEnumerator();
        }
        #endregion

        #region Init
        private void Init()
        {
            if (!_initialized)
            {
                lock (locker)
                {
                    if (!_initialized)
                    {
                        _ontologyDicByCode.Clear();
                        _ontologyDicByID.Clear();
                        var allOntologies = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetOntologies().OrderBy(s => s.SortCode);
                        foreach (var ontology in allOntologies)
                        {
                            if (!(ontology is OntologyBase))
                            {
                                throw new CoreException("本体模型必须继承OntologyBase基类");
                            }
                            var ontologyDescriptor = new OntologyDescriptor(OntologyState.Create(ontology));
                            _ontologyDicByCode.Add(ontology.Code, ontologyDescriptor);
                            _ontologyDicByID.Add(ontology.Id, ontologyDescriptor);
                        }
                        _initialized = true;
                    }
                }
            }
        }
        #endregion

        #region MessageHandler
        private class MessageHandler : 
            IHandler<AddOntologyCommand>,
            IHandler<OntologyAddedEvent>,
            IHandler<UpdateOntologyCommand>,
            IHandler<OntologyUpdatedEvent>,
            IHandler<RemoveOntologyCommand>
        {
            private readonly OntologySet ontologySet;

            public MessageHandler(OntologySet ontologySet)
            {
                this.ontologySet = ontologySet;
            }

            public void Handle(AddOntologyCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(OntologyAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateOntologyAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IOntologyCreateInput input, bool isCommand)
            {
                var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                Ontology entity;
                lock (this)
                {
                    OntologyDescriptor ontology;
                    if (NodeHost.Instance.Ontologies.TryGetOntology(input.Id.Value, out ontology))
                    {
                        throw new ValidationException("给定的标识标识的记录已经存在");
                    }

                    entity = Ontology.Create(input);

                    var descriptor = new OntologyDescriptor(OntologyState.Create(entity));
                    if (!ontologySet._ontologyDicByCode.ContainsKey(entity.Code))
                    {
                        ontologySet._ontologyDicByCode.Add(entity.Code, descriptor);
                    }
                    if (!ontologySet._ontologyDicByID.ContainsKey(entity.Id))
                    {
                        ontologySet._ontologyDicByID.Add(entity.Id, descriptor);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            ontologyRepository.Add(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (ontologySet._ontologyDicByCode.ContainsKey(entity.Code))
                            {
                                ontologySet._ontologyDicByCode.Remove(entity.Code);
                            }
                            if (ontologySet._ontologyDicByID.ContainsKey(entity.Id))
                            {
                                ontologySet._ontologyDicByID.Remove(entity.Id);
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    ontologySet.host.AppHost.MessageDispatcher.DispatchMessage(new PrivateOntologyAddedEvent(entity, input));
                }
            }

            private class PrivateOntologyAddedEvent : OntologyAddedEvent
            {
                public PrivateOntologyAddedEvent(OntologyBase source, IOntologyCreateInput input)
                    : base(source, input)
                {

                }
            }

            public void Handle(UpdateOntologyCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(OntologyUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateOntologyUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IOntologyUpdateInput input, bool isCommand)
            {
                var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                OntologyDescriptor bkState;
                if (!NodeHost.Instance.Ontologies.TryGetOntology(input.Id, out bkState))
                {
                    throw new NotExistException();
                }
                Ontology entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    OntologyDescriptor ontology;
                    if (!NodeHost.Instance.Ontologies.TryGetOntology(input.Id, out ontology))
                    {
                        throw new NotExistException();
                    }
                    if (NodeHost.Instance.Ontologies.TryGetOntology(input.Code, out ontology) && ontology.Ontology.Id != input.Id)
                    {
                        throw new ValidationException("重复的编码");
                    }
                    entity = ontologyRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = new OntologyDescriptor(OntologyState.Create(entity));
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            ontologyRepository.Update(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    ontologySet.host.AppHost.MessageDispatcher.DispatchMessage(new PrivateOntologyUpdatedEvent(entity, input));
                }
            }

            private void Update(OntologyDescriptor state)
            {
                var oldState = ontologySet._ontologyDicByID[state.Ontology.Id];
                ontologySet._ontologyDicByID[state.Ontology.Id] = state;
                if (!ontologySet._ontologyDicByCode.ContainsKey(state.Ontology.Code))
                {
                    ontologySet._ontologyDicByCode.Add(state.Ontology.Code, state);
                    ontologySet._ontologyDicByCode.Remove(oldState.Ontology.Code);
                }
                else
                {
                    ontologySet._ontologyDicByCode[oldState.Ontology.Code] = state;
                }
            }

            private class PrivateOntologyUpdatedEvent : OntologyUpdatedEvent
            {
                public PrivateOntologyUpdatedEvent(OntologyBase source, IOntologyUpdateInput input)
                    : base(source, input)
                {

                }
            }

            public void Handle(RemoveOntologyCommand message)
            {
                var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                OntologyDescriptor ontology;
                if (!NodeHost.Instance.Ontologies.TryGetOntology(message.EntityID, out ontology))
                {
                    return;
                }
                Ontology entity = ontologyRepository.GetByKey(message.EntityID);
                if (entity == null)
                {
                    return;
                }
                if (ontology.Elements != null && ontology.Elements.Count > 0)
                {
                    throw new ValidationException("本体下有本体元素时不能删除");
                }
                var archives = ontology.GetArchives();
                if (archives != null && archives.Count > 0)
                {
                    throw new ValidationException("本体下有归档记录时不能删除");
                }
                if (ontology.Organizations != null && ontology.Organizations.Count > 0)
                {
                    throw new ValidationException("本体下有建立的组织结构时不能删除");
                }
                if (ontology.Actions != null && ontology.Actions.Count > 0)
                {
                    throw new ValidationException("本体下定义了动作时不能删除");
                }
                if (ontology.Topics != null && ontology.Topics.Count > 0)
                {
                    throw new ValidationException("本体下定义了事件主题时不能删除");
                }
                if (ontology.Processes != null && ontology.Processes.Count() > 0)
                {
                    throw new ValidationException("本体下有进程记录时不能删除");
                }
                if (ontology.InfoGroups != null && ontology.InfoGroups.Count() > 0)
                {
                    throw new ValidationException("本体下建立了信息组时不能删除");
                }
                if (NodeHost.Instance.Nodes.GetNodeOntologyCares().Any(a => a.OntologyID == entity.Id))
                {
                    throw new ValidationException("有节点关心了该本体时不能删除");
                }
                if (NodeHost.Instance.Nodes.GetNodeOntologyOrganizations().Any(a => a.OntologyID == entity.Id))
                {
                    throw new ValidationException("有节点关心了该本体下的组织结构时不能删除");
                }
                if (NodeHost.Instance.AppHost.GetRequiredService<IRepository<Batch>>().FindAll().Any(a => a.OntologyID == entity.Id))
                {
                    throw new ValidationException("本体下有批处理记录时不能删除");
                }
                var bkState = ontologySet._ontologyDicByID[entity.Id];
                lock (ontologySet.locker)
                {
                    try
                    {
                        if (ontologySet._ontologyDicByID.ContainsKey(entity.Id))
                        {
                            ontologySet._ontologyDicByID.Remove(entity.Id);
                        }
                        if (ontologySet._ontologyDicByCode.ContainsKey(entity.Code))
                        {
                            ontologySet._ontologyDicByCode.Remove(entity.Code);
                        }
                        ontologyRepository.Remove(entity);
                        ontologyRepository.Context.Commit();
                    }
                    catch
                    {
                        if (!ontologySet._ontologyDicByID.ContainsKey(entity.Id))
                        {
                            ontologySet._ontologyDicByID.Add(entity.Id, bkState);
                        }
                        if (!ontologySet._ontologyDicByCode.ContainsKey(entity.Code))
                        {
                            ontologySet._ontologyDicByCode.Add(entity.Code, bkState);
                        }
                        ontologyRepository.Context.Rollback();
                        throw;
                    }
                }
                ontologySet.host.AppHost.PublishEvent(new OntologyRemovedEvent(entity));
                ontologySet.host.AppHost.CommitEventBus();
            }
        }
        #endregion

        // 内部类
        #region ElementSet
        private sealed class ElementSet
        {
            #region Private Fields
            private readonly Dictionary<Guid, ElementDescriptor> _elementDicByID = new Dictionary<Guid, ElementDescriptor>();
            private readonly Dictionary<OntologyDescriptor, Dictionary<string, ElementDescriptor>> _elementDicByOntology = new Dictionary<OntologyDescriptor, Dictionary<string, ElementDescriptor>>();
            private bool initialized = false;
            private readonly object locker = new object();
            private readonly Guid _id = Guid.NewGuid();
            #endregion
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            #region Ctor
            internal ElementSet(NodeHost host)
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
                messageDispatcher.Register(new CreateElementCommandHandler(this));
                messageDispatcher.Register(new UpdateElementCommandHandler(this));
                messageDispatcher.Register(new DeleteElementCommandHandler(this));
                messageDispatcher.Register(new CreateSystemElementCommandHandler(this));
            }
            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="elementID"></param>
            /// <param name="element"></param>
            /// <returns></returns>
            public bool TryGetElement(Guid elementID, out ElementDescriptor element)
            {
                if (!initialized)
                {
                    Init();
                }
                return _elementDicByID.TryGetValue(elementID, out element);
            }

            /// <summary>
            /// 根据ID获取本体元素，包括启用和禁用的本体元素
            /// </summary>
            /// <param name="elementID"></param>
            /// <returns></returns>
            public ElementDescriptor GetElement(Guid elementID)
            {
                if (!initialized)
                {
                    Init();
                }
                return !_elementDicByID.ContainsKey(elementID) ? null : _elementDicByID[elementID];
            }

            /// <summary>
            /// 根据模型码索引字段，不区分大小写
            /// </summary>
            /// <param name="ontology">本体</param>
            /// <returns>不会返回null，返回无元素的空字典</returns>
            public Dictionary<string, ElementDescriptor> GetElements(OntologyDescriptor ontology)
            {
                if (!initialized)
                {
                    Init();
                }
                if (!_elementDicByOntology.ContainsKey(ontology))
                {
                    return new Dictionary<string, ElementDescriptor>(StringComparer.OrdinalIgnoreCase);
                }

                return _elementDicByOntology[ontology];
            }

            internal void Refresh()
            {
                if (initialized)
                {
                    initialized = false;
                }
            }

            #region Init
            private void Init()
            {
                if (!initialized)
                {
                    lock (locker)
                    {
                        if (!initialized)
                        {
                            _elementDicByOntology.Clear();
                            _elementDicByID.Clear();
                            var allElements = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetElements();
                            foreach (var element in allElements)
                            {
                                if (!_elementDicByID.ContainsKey(element.Id))
                                {
                                    var descriptor = new ElementDescriptor(ElementState.Create(element));
                                    _elementDicByID.Add(element.Id, descriptor);
                                    OntologyDescriptor ontology;
                                    NodeHost.Instance.Ontologies.TryGetOntology(element.OntologyID, out ontology);
                                    if (!_elementDicByOntology.ContainsKey(ontology))
                                    {
                                        _elementDicByOntology.Add(ontology, new Dictionary<string, ElementDescriptor>(StringComparer.OrdinalIgnoreCase));
                                    }
                                    if (!_elementDicByOntology[ontology].ContainsKey(element.Code))
                                    {
                                        _elementDicByOntology[ontology].Add(element.Code, descriptor);
                                    }
                                }
                            }
                            initialized = true;
                        }
                    }
                }
            }
            #endregion

            #region CreateElementCommandHandler
            private class CreateElementCommandHandler : IHandler<AddElementCommand>
            {
                private readonly ElementSet elementSet;

                public CreateElementCommandHandler(ElementSet elementSet)
                {
                    this.elementSet = elementSet;
                }

                public void Handle(AddElementCommand message)
                {
                    var elementRepository = NodeHost.Instance.GetRequiredService<IRepository<Element>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    ElementDescriptor element;
                    if (NodeHost.Instance.Ontologies.TryGetElement(message.Input.Id.Value, out element))
                    {
                        throw new ValidationException("给定的标识标识的记录已经存在");
                    }
                    OntologyDescriptor ontology;
                    if (!NodeHost.Instance.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
                    {
                        throw new ValidationException("意外的本体标识" + message.Input.OntologyID);
                    }
                    if (ontology.Elements.ContainsKey(message.Input.Code))
                    {
                        throw new ValidationException("重复的编码");
                    }

                    Element entity = Element.Create(message.Input);

                    lock (elementSet.locker)
                    {
                        var descriptor = new ElementDescriptor(ElementState.Create(entity));
                        elementSet._elementDicByID.Add(entity.Id, descriptor);
                        if (!elementSet._elementDicByOntology.ContainsKey(ontology))
                        {
                            elementSet._elementDicByOntology.Add(ontology, new Dictionary<string, ElementDescriptor>(StringComparer.OrdinalIgnoreCase));
                        }
                        elementSet._elementDicByOntology[ontology].Add(entity.Code, descriptor);
                        try
                        {
                            elementRepository.Add(entity);
                            elementRepository.Context.Commit();
                        }
                        catch
                        {
                            elementSet._elementDicByID.Remove(entity.Id);
                            if (elementSet._elementDicByOntology.ContainsKey(ontology) && elementSet._elementDicByOntology[ontology].ContainsKey(entity.Code))
                            {
                                elementSet._elementDicByOntology[ontology].Remove(entity.Code);
                            }
                            elementRepository.Context.Rollback();
                            throw;
                        }
                    }
                    elementSet.host.AppHost.PublishEvent(new ElementAddedEvent(entity));
                    elementSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion

            #region CreateSystemElementCommandHandler
            private class CreateSystemElementCommandHandler : IHandler<AddSystemElementCommand>
            {
                private readonly ElementSet elementSet;

                public CreateSystemElementCommandHandler(ElementSet elementSet)
                {
                    this.elementSet = elementSet;
                }

                public void Handle(AddSystemElementCommand message)
                {
                    // 配置运行时本体元素
                    var elementEntities = new List<ElementCreateInput>();
                    foreach (var ontology in elementSet.host.Ontologies)
                    {
                        foreach (var item in ElementDescriptor.SystemElementCodes)
                        {
                            if (!ontology.Elements.ContainsKey(item.Key))
                            {
                                var entity = CreateElement(ontology, item.Value);
                                elementEntities.Add(entity);
                            }
                        }
                    }
                    if (elementEntities.Count > 0)
                    {
                        foreach (var entity in elementEntities)
                        {
                            elementSet.host.AppHost.Handle(new AddElementCommand(entity));
                        }
                    }
                }

                private ElementCreateInput CreateElement(OntologyDescriptor ontology, DbField field)
                {
                    ElementCreateInput element = new ElementCreateInput()
                    {
                        Id = Guid.NewGuid(),
                        AllowFilter = true,
                        AllowSort = true,
                        Code = field.Name,
                        Description = "系统本体元素",
                        FieldCode = field.Name,
                        GroupID = null,
                        Icon = null,
                        InfoDicID = null,
                        InputHeight = null,
                        InputType = null,
                        InputWidth = null,
                        IsDetailsShow = false,
                        IsEnabled = 1,
                        IsExport = false,
                        IsGridColumn = false,
                        IsImport = false,
                        IsInfoIDItem = false,
                        IsInput = false,
                        IsTotalLine = false,
                        MaxLength = field.MaxLength,
                        Name = field.Name,
                        OntologyID = ontology.Ontology.Id,
                        Ref = null,
                        Regex = null,
                        OType = string.Empty,
                        DbType = string.Empty,
                        Nullable = true,
                        SortCode = 10000,
                        Width = 100
                    };

                    return element;
                }

                private class ElementCreateInput : EntityCreateInput, IElementCreateInput
                {
                    public string OType { get; set; }
                    public bool Nullable { get; set; }

                    public bool AllowFilter { get; set; }

                    public bool AllowSort { get; set; }

                    public topicCode Code { get; set; }

                    public topicCode DbType { get; set; }

                    public topicCode Description { get; set; }

                    public topicCode FieldCode { get; set; }

                    public organizationID? GroupID { get; set; }

                    public topicCode Icon { get; set; }

                    public organizationID? InfoDicID { get; set; }

                    public int? InputHeight { get; set; }

                    public topicCode InputType { get; set; }

                    public int? InputWidth { get; set; }

                    public bool IsDetailsShow { get; set; }

                    public int IsEnabled { get; set; }

                    public bool IsExport { get; set; }

                    public bool IsGridColumn { get; set; }

                    public bool IsImport { get; set; }

                    public bool IsInfoIDItem { get; set; }

                    public bool IsInput { get; set; }

                    public bool IsTotalLine { get; set; }

                    public int? MaxLength { get; set; }

                    public topicCode Name { get; set; }

                    public organizationID OntologyID { get; set; }

                    public topicCode Ref { get; set; }

                    public topicCode Regex { get; set; }

                    public int SortCode { get; set; }

                    public int Width { get; set; }

                    public Guid? ForeignElementID { get; set; }

                    public string Tooltip { get; set; }
                }
            }
            #endregion

            #region UpdateElementCommandHandler
            private class UpdateElementCommandHandler : IHandler<UpdateElementCommand>
            {
                private readonly ElementSet elementSet;

                public UpdateElementCommandHandler(ElementSet elementSet)
                {
                    this.elementSet = elementSet;
                }

                public void Handle(UpdateElementCommand message)
                {
                    var elementRepository = NodeHost.Instance.GetRequiredService<IRepository<Element>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    ElementDescriptor element;
                    if (!NodeHost.Instance.Ontologies.TryGetElement(message.Input.Id, out element))
                    {
                        throw new NotExistException();
                    }
                    if (element.Ontology.Elements.ContainsKey(message.Input.Code) && element.Ontology.Elements[message.Input.Code].Element.Id != message.Input.Id)
                    {
                        throw new ValidationException("重复的编码");
                    }
                    var entity = elementRepository.GetByKey(message.Input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }
                    var bkState = elementSet._elementDicByID[entity.Id];

                    entity.Update(message.Input);

                    var newState = new ElementDescriptor(ElementState.Create(entity));
                    bool stateChanged = newState != bkState;
                    lock (elementSet.locker)
                    {
                        try
                        {
                            if (stateChanged)
                            {
                                Update(newState);
                            }
                            elementRepository.Update(entity);
                            elementRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            elementRepository.Context.Rollback();
                            throw;
                        }
                        if (stateChanged)
                        {
                            elementSet.host.AppHost.PublishEvent(new ElementUpdatedEvent(entity));
                            elementSet.host.AppHost.CommitEventBus();
                        }
                    }
                }

                private void Update(ElementDescriptor state)
                {
                    var oldKey = elementSet._elementDicByID[state.Element.Id].Element.Code;
                    elementSet._elementDicByID[state.Element.Id] = state;
                    if (!elementSet._elementDicByOntology[state.Ontology].ContainsKey(state.Element.Code))
                    {
                        elementSet._elementDicByOntology[state.Ontology].Add(state.Element.Code, state);
                        elementSet._elementDicByOntology[state.Ontology].Remove(oldKey);
                    }
                    else
                    {
                        elementSet._elementDicByOntology[state.Ontology][state.Element.Code] = state;
                    }
                }
            }
            #endregion

            private class DeleteElementCommandHandler : IHandler<RemoveElementCommand>
            {
                private readonly ElementSet elementSet;

                public DeleteElementCommandHandler(ElementSet elementSet)
                {
                    this.elementSet = elementSet;
                }

                public void Handle(RemoveElementCommand message)
                {
                    var elementRepository = NodeHost.Instance.GetRequiredService<IRepository<Element>>();
                    ElementDescriptor element;
                    if (!NodeHost.Instance.Ontologies.TryGetElement(message.EntityID, out element))
                    {
                        return;
                    }
                    Element entity = elementRepository.GetByKey(message.EntityID);
                    if (entity == null)
                    {
                        return;
                    }
                    var bkState = elementSet._elementDicByID[entity.Id];
                    lock (elementSet.locker)
                    {
                        try
                        {
                            elementSet._elementDicByID.Remove(entity.Id);
                            elementSet._elementDicByOntology[bkState.Ontology].Remove(bkState.Element.Code);
                            elementRepository.Remove(entity);
                            elementRepository.Context.Commit();
                        }
                        catch
                        {
                            elementSet._elementDicByID.Add(entity.Id, bkState);
                            elementSet._elementDicByOntology[bkState.Ontology].Add(bkState.Element.Code, bkState);
                            elementRepository.Context.Rollback();
                            throw;
                        }
                    }
                    elementSet.host.AppHost.PublishEvent(new ElementRemovedEvent(entity));
                    elementSet.host.AppHost.CommitEventBus();
                }
            }
        }
        #endregion

        // 内部类
        #region ActionSet
        private sealed class ActionSet
        {
            private readonly Dictionary<OntologyDescriptor, Dictionary<Verb, ActionState>> _actionDicByVerb = new Dictionary<OntologyDescriptor, Dictionary<Verb, ActionState>>();
            private readonly Dictionary<Guid, ActionState> _actionsByID = new Dictionary<organizationID, ActionState>();
            private bool initialized = false;
            private object locker = new object();
            private readonly Guid _id = Guid.NewGuid();
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal ActionSet(NodeHost host)
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
                messageDispatcher.Register(new CreateActionCommandHandler(this));
                messageDispatcher.Register(new UpdateActionCommandHandler(this));
                messageDispatcher.Register(new DeleteActionCommandHandler(this));
            }

            #region GetActons
            public IReadOnlyDictionary<Verb, ActionState> GetActons(OntologyDescriptor ontology)
            {
                if (ontology == null)
                {
                    throw new ArgumentNullException("ontology");
                }
                if (!initialized)
                {
                    Init();
                }
                return _actionDicByVerb[ontology];
            }
            #endregion

            #region GetAction
            public ActionState GetAction(Guid actionID)
            {
                if (!initialized)
                {
                    Init();
                }
                if (!_actionsByID.ContainsKey(actionID))
                {
                    return null;
                }
                return _actionsByID[actionID];
            }
            #endregion

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
                    lock (locker)
                    {
                        if (!initialized)
                        {
                            _actionDicByVerb.Clear();
                            _actionsByID.Clear();
                            var actions = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetActions();
                            var nodeElementActions = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetNodeElementActions();
                            foreach (var ontology in host.Ontologies)
                            {
                                if (!_actionDicByVerb.ContainsKey(ontology))
                                {
                                    _actionDicByVerb.Add(ontology, new Dictionary<Verb, ActionState>());
                                }
                                foreach (var item in actions.Where(a => a.OntologyID == ontology.Ontology.Id))
                                {
                                    var actionState = ActionState.Create(item);
                                    if (_actionDicByVerb[ontology].ContainsKey(actionState.ActionVerb))
                                    {
                                        throw new CoreException("意外重复的本体动作动词" + item.Verb);
                                    }
                                    _actionsByID.Add(item.Id, actionState);
                                    _actionDicByVerb[ontology].Add(actionState.ActionVerb, actionState);
                                }
                            }
                            initialized = true;
                        }
                    }
                }
            }

            #region CreateActionCommandHandler
            private class CreateActionCommandHandler : IHandler<AddActionCommand>
            {
                private readonly ActionSet actionSet;

                public CreateActionCommandHandler(ActionSet actionSet)
                {
                    this.actionSet = actionSet;
                }

                public void Handle(AddActionCommand message)
                {
                    var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Verb))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    if (actionSet._actionsByID.ContainsKey(message.Input.Id.Value))
                    {
                        throw new ValidationException("给定的标识标识的记录已经存在");
                    }
                    OntologyDescriptor ontology;
                    if (!actionSet.host.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
                    {
                        throw new ValidationException("非法的本体标识");
                    }
                    if (ontology.Actions.ContainsKey(new Verb(message.Input.Verb)))
                    {
                        throw new ValidationException("重复的动词");
                    }

                    var entity = Action.Create(message.Input);

                    lock (actionSet.locker)
                    {
                        try
                        {
                            var state = ActionState.Create(entity);
                            actionSet._actionsByID.Add(entity.Id, state);
                            actionSet._actionDicByVerb[ontology].Add(new Verb(entity.Verb), state);
                            ontologyRepository.Context.RegisterNew(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            actionSet._actionsByID.Remove(entity.Id);
                            actionSet._actionDicByVerb[ontology].Remove(new Verb(entity.Verb));
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    actionSet.host.AppHost.PublishEvent(new ActionAddedEvent(entity));
                    actionSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion

            #region UpdateActionCommandHandler
            private class UpdateActionCommandHandler : IHandler<UpdateActionCommand>
            {
                private readonly ActionSet actionSet;

                public UpdateActionCommandHandler(ActionSet actionSet)
                {
                    this.actionSet = actionSet;
                }

                public void Handle(UpdateActionCommand message)
                {
                    var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Verb))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    bool exist = false;
                    OntologyDescriptor ontology = null;
                    foreach (var item in NodeHost.Instance.Ontologies)
                    {
                        if (item.Actions.Values.Any(a => a.Id == message.Input.Id))
                        {
                            exist = true;
                            ontology = item;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        throw new NotExistException();
                    }
                    var verb = new Verb(message.Input.Verb);
                    if (ontology.Actions.ContainsKey(verb) && ontology.Actions[verb].Id != message.Input.Id)
                    {
                        throw new ValidationException("重复的编码");
                    }
                    var entity = ontologyRepository.Context.Query<Action>().FirstOrDefault(a => a.Id == message.Input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }
                    var bkState = ActionState.Create(entity);

                    entity.Update(message.Input);

                    var newState = ActionState.Create(entity);
                    bool stateChanged = newState != bkState;
                    lock (actionSet.locker)
                    {
                        try
                        {
                            if (stateChanged)
                            {
                                Update(newState);
                            }
                            ontologyRepository.Context.RegisterModified(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    if (stateChanged)
                    {
                        actionSet.host.AppHost.PublishEvent(new ActionUpdatedEvent(entity));
                        actionSet.host.AppHost.CommitEventBus();
                    }
                }

                private void Update(ActionState state)
                {
                    var ontology = actionSet.host.Ontologies[state.OntologyID];
                    var newVerb = state.ActionVerb;
                    var oldVerb = actionSet._actionsByID[state.Id].ActionVerb;
                    actionSet._actionsByID[state.Id] = state;
                    if (!actionSet._actionDicByVerb[ontology].ContainsKey(newVerb))
                    {
                        actionSet._actionDicByVerb[ontology].Add(newVerb, state);
                        actionSet._actionDicByVerb[ontology].Remove(oldVerb);
                    }
                    else
                    {
                        actionSet._actionDicByVerb[ontology][newVerb] = state;
                    }
                }
            }
            #endregion

            #region DeleteActionCommandHandler
            private class DeleteActionCommandHandler : IHandler<RemoveActionCommand>
            {
                private readonly ActionSet actionSet;

                public DeleteActionCommandHandler(ActionSet actionSet)
                {
                    this.actionSet = actionSet;
                }

                public void Handle(RemoveActionCommand message)
                {
                    var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                    bool exist = false;
                    OntologyDescriptor ontology = null;
                    foreach (var item in NodeHost.Instance.Ontologies)
                    {
                        if (item.Actions.Values.Any(a => a.Id == message.EntityID))
                        {
                            ontology = item;
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        return;
                    }
                    var entity = ontologyRepository.Context.Query<Action>().FirstOrDefault(a => a.Id == message.EntityID);
                    if (entity == null)
                    {
                        return;
                    }
                    var bkState = ActionState.Create(entity);
                    lock (actionSet.locker)
                    {
                        try
                        {
                            actionSet._actionsByID.Remove(entity.Id);
                            actionSet._actionDicByVerb[ontology].Remove(bkState.ActionVerb);
                            ontologyRepository.Context.RegisterDeleted(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            actionSet._actionsByID.Add(entity.Id, bkState);
                            actionSet._actionDicByVerb[ontology].Add(bkState.ActionVerb, bkState);
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    actionSet.host.AppHost.PublishEvent(new ActionRemovedEvent(entity));
                    actionSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion

            #endregion
        }
        #endregion

        // 内部类
        #region InfoGroupSet
        private sealed class InfoGroupSet
        {
            private readonly Dictionary<OntologyDescriptor, IList<InfoGroupState>>
                _dic = new Dictionary<OntologyDescriptor, IList<InfoGroupState>>();
            private bool initialized = false;
            private readonly object locker = new object();
            private readonly Guid _id = new Guid();
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal InfoGroupSet(NodeHost host)
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
                messageDispatcher.Register(new CreateInfoGroupCommandHandler(this));
                messageDispatcher.Register(new UpdateInfoGroupCommandHandler(this));
                messageDispatcher.Register(new DeleteInfoGroupCommandHandler(this));
            }

            public IList<InfoGroupState> GetInfoGroups(OntologyDescriptor ontology)
            {
                if (!initialized)
                {
                    Init();
                }
                return !_dic.ContainsKey(ontology) ? new List<InfoGroupState>() : _dic[ontology];
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
                    lock (locker)
                    {
                        if (!initialized)
                        {
                            _dic.Clear();
                            var infoGroups = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetInfoGroups().OrderBy(a => a.SortCode);
                            foreach (var ontology in NodeHost.Instance.Ontologies)
                            {
                                _dic.Add(ontology, new List<InfoGroupState>());
                                foreach (var infoGroup in infoGroups.Where(a => a.OntologyID == ontology.Ontology.Id))
                                {
                                    _dic[ontology].Add(InfoGroupState.Create(infoGroup));
                                }
                            }
                            initialized = true;
                        }
                    }
                }
            }
            #endregion

            #region CreateInfoGroupCommandHandler
            private class CreateInfoGroupCommandHandler : IHandler<AddInfoGroupCommand>
            {
                private readonly InfoGroupSet infoGroupSet;

                public CreateInfoGroupCommandHandler(InfoGroupSet infoGroupSet)
                {
                    this.infoGroupSet = infoGroupSet;
                }

                public void Handle(AddInfoGroupCommand message)
                {
                    var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    OntologyDescriptor ontology;
                    if (!NodeHost.Instance.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
                    {
                        throw new ValidationException("非法的本体标识" + message.Input.OntologyID);
                    }
                    if (ontology.InfoGroups.Any(a => a.Id == message.Input.Id))
                    {
                        throw new CoreException("给定的标识标识的工作组已经存在");
                    }
                    if (ontology.InfoGroups.Any(a => string.Equals(a.Code, message.Input.Code, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new ValidationException("重复的编码");
                    }

                    InfoGroup entity = InfoGroup.Create(message.Input);

                    lock (infoGroupSet.locker)
                    {
                        try
                        {
                            if (!infoGroupSet._dic.ContainsKey(ontology))
                            {
                                infoGroupSet._dic.Add(ontology, new List<InfoGroupState>());
                            }
                            if (!infoGroupSet._dic[ontology].Any(a => a.Id == entity.Id))
                            {
                                infoGroupSet._dic[ontology].Add(InfoGroupState.Create(entity));
                            }
                            ontologyRepository.Context.RegisterNew(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (infoGroupSet._dic.ContainsKey(ontology))
                            {
                                var item = infoGroupSet._dic[ontology].FirstOrDefault(a => a.Id == entity.Id);
                                if (item != null)
                                {
                                    infoGroupSet._dic[ontology].Remove(item);
                                }
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    infoGroupSet.host.AppHost.PublishEvent(new InfoGroupAddedEvent(entity));
                    infoGroupSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion

            #region UpdateInfoGroupCommandHandler
            private class UpdateInfoGroupCommandHandler : IHandler<UpdateInfoGroupCommand>
            {
                private readonly InfoGroupSet infoGroupSet;

                public UpdateInfoGroupCommandHandler(InfoGroupSet infoGroupSet)
                {
                    this.infoGroupSet = infoGroupSet;
                }

                public void Handle(UpdateInfoGroupCommand message)
                {
                    var ontologyRepository = infoGroupSet.host.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    InfoGroup entity;
                    bool stateChanged = false;
                    lock (infoGroupSet.locker)
                    {
                        entity = ontologyRepository.Context.Query<InfoGroup>().FirstOrDefault(a => a.Id == message.Input.Id);
                        if (entity == null)
                        {
                            throw new NotExistException();
                        }
                        OntologyDescriptor ontology;
                        if (!infoGroupSet.host.Ontologies.TryGetOntology(entity.OntologyID, out ontology))
                        {
                            throw new ValidationException("非法的本体标识" + entity.OntologyID);
                        }
                        if (ontology.InfoGroups.Any(a => string.Equals(a.Code, message.Input.Code, StringComparison.OrdinalIgnoreCase) && a.Id != entity.OntologyID))
                        {
                            throw new ValidationException("重复的编码");
                        }

                        var bkState = InfoGroupState.Create(entity);

                        entity.Update(message.Input);

                        var newState = InfoGroupState.Create(entity);
                        stateChanged = newState != bkState;
                        try
                        {
                            if (stateChanged)
                            {
                                Update(newState);
                            }
                            ontologyRepository.Context.RegisterModified(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    if (stateChanged)
                    {
                        infoGroupSet.host.AppHost.PublishEvent(new InfoGroupUpdatedEvent(entity));
                        infoGroupSet.host.AppHost.CommitEventBus();
                    }
                }

                private void Update(InfoGroupState state)
                {
                    OntologyDescriptor ontology = NodeHost.Instance.Ontologies[state.OntologyID];
                    var item = infoGroupSet._dic[ontology].First(a => a.Id == state.Id);
                    infoGroupSet._dic[ontology].Remove(item);
                    infoGroupSet._dic[ontology].Add(state);
                }
            }
            #endregion

            #region DeleteInfoGroupCommandHandler
            private class DeleteInfoGroupCommandHandler : IHandler<RemoveInfoGroupCommand>
            {
                private readonly InfoGroupSet infoGroupSet;

                public DeleteInfoGroupCommandHandler(InfoGroupSet infoGroupSet)
                {
                    this.infoGroupSet = infoGroupSet;
                }

                public void Handle(RemoveInfoGroupCommand message)
                {
                    var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                    InfoGroup entity = ontologyRepository.Context.Query<InfoGroup>().FirstOrDefault(a => a.Id == message.EntityID);
                    if (entity == null)
                    {
                        return;
                    }
                    var bkState = InfoGroupState.Create(entity);
                    lock (infoGroupSet.locker)
                    {
                        try
                        {
                            infoGroupSet._dic[NodeHost.Instance.Ontologies[bkState.OntologyID]].Remove(bkState);
                            ontologyRepository.Context.RegisterDeleted(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            infoGroupSet._dic[NodeHost.Instance.Ontologies[bkState.OntologyID]].Add(bkState);
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    infoGroupSet.host.AppHost.PublishEvent(new InfoGroupRemovedEvent(entity));
                    infoGroupSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion
        }
        #endregion

        // 内部类
        #region OntologyOrganizationSet
        private sealed class OntologyOrganizationSet
        {
            private readonly Dictionary<OntologyDescriptor, Dictionary<OrganizationState, OntologyOrganizationState>>
                _dic = new Dictionary<OntologyDescriptor, Dictionary<OrganizationState, OntologyOrganizationState>>();
            private bool initialized = false;
            private readonly object locker = new object();
            private readonly Guid _id = Guid.NewGuid();
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal OntologyOrganizationSet(NodeHost host)
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
                messageDispatcher.Register((IHandler<AddOntologyOrganizationCommand>)handler);
                messageDispatcher.Register((IHandler<OntologyOrganizationAddedEvent>)handler);
                messageDispatcher.Register((IHandler<RemoveOntologyOrganizationCommand>)handler);
                messageDispatcher.Register((IHandler<OntologyOrganizationRemovedEvent>)handler);
                messageDispatcher.Register((IHandler<AddOrganizationActionCommand>)handler);
                messageDispatcher.Register((IHandler<OrganizationActionAddedEvent>)handler);
                messageDispatcher.Register((IHandler<RemoveOrganizationActionCommand>)handler);
                messageDispatcher.Register((IHandler<OrganizationActionRemovedEvent>)handler);
                messageDispatcher.Register((IHandler<RemoveOrganizationActionCommand>)handler);
                messageDispatcher.Register((IHandler<OrganizationActionRemovedEvent>)handler);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="ontology"></param>
            /// <returns>key为组织结构码</returns>
            public Dictionary<OrganizationState, OntologyOrganizationState> this[OntologyDescriptor ontology]
            {
                get
                {
                    if (!initialized)
                    {
                        Init();
                    }
                    if (!_dic.ContainsKey(ontology))
                    {
                        return new Dictionary<OrganizationState, OntologyOrganizationState>();
                    }

                    return _dic[ontology];
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
                            var ontologyOrgs = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetOntologyOrganizations();
                            foreach (var ontologyOrg in ontologyOrgs)
                            {
                                OrganizationState org;
                                OntologyDescriptor ontology;
                                if (!host.Ontologies.TryGetOntology(ontologyOrg.OntologyID, out ontology))
                                {
                                    throw new CoreException("意外的本体组织结构本体标识" + ontologyOrg.OntologyID);
                                }
                                if (host.AppHost.OrganizationSet.TryGetOrganization(ontologyOrg.OrganizationID, out org))
                                {
                                    if (!_dic.ContainsKey(ontology))
                                    {
                                        _dic.Add(ontology, new Dictionary<OrganizationState, OntologyOrganizationState>());
                                    }
                                    var ontologyOrgState = OntologyOrganizationState.Create(ontologyOrg);
                                    if (!_dic[ontology].ContainsKey(org))
                                    {
                                        _dic[ontology].Add(org, ontologyOrgState);
                                    }
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

            private class MessageHandler :
                IHandler<AddOntologyOrganizationCommand>,
                IHandler<OntologyOrganizationAddedEvent>,
                IHandler<RemoveOntologyOrganizationCommand>,
                IHandler<OntologyOrganizationRemovedEvent>,
                IHandler<AddOrganizationActionCommand>,
                IHandler<OrganizationActionAddedEvent>,
                IHandler<UpdateOrganizationActionCommand>,
                IHandler<OrganizationActionUpdatedEvent>,
                IHandler<RemoveOrganizationActionCommand>,
                IHandler<OrganizationActionRemovedEvent>
            {
                private readonly OntologyOrganizationSet set;

                public MessageHandler(OntologyOrganizationSet set)
                {
                    this.set = set;
                }

                public void Handle(AddOntologyOrganizationCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(OntologyOrganizationAddedEvent message)
                {
                    if (message.GetType() == typeof(PrivateOntologyOrganizationAddedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(IOntologyOrganizationCreateInput input, bool isCommand)
                {
                    var _dic = set._dic;
                    var host = set.host;
                    var repository = host.AppHost.GetRequiredService<IRepository<OntologyOrganization>>();
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(input.OntologyID, out ontology))
                    {
                        throw new CoreException("意外的本体标识" + input.OntologyID);
                    }
                    OrganizationState org;
                    if (!host.AppHost.OrganizationSet.TryGetOrganization(input.OrganizationID, out org))
                    {
                        throw new CoreException("意外的组织结构标识" + input.OrganizationID);
                    }
                    OntologyOrganization entity;
                    lock (this)
                    {
                        if (_dic.ContainsKey(ontology) && _dic[ontology].ContainsKey(org))
                        {
                            return;
                        }
                        entity = new OntologyOrganization
                        {
                            Id = input.Id.Value,
                            OntologyID = input.OntologyID,
                            OrganizationID = input.OrganizationID,
                            Actions = null
                        };
                        var ontologyOrgState = OntologyOrganizationState.Create(entity);
                        if (!_dic.ContainsKey(ontology))
                        {
                            _dic.Add(ontology, new Dictionary<OrganizationState, OntologyOrganizationState>());
                        }
                        _dic[ontology].Add(org, ontologyOrgState);
                        if (isCommand)
                        {
                            try
                            {
                                repository.Add(entity);
                                repository.Context.Commit();
                            }
                            catch
                            {
                                _dic[ontology].Remove(org);
                                repository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateOntologyOrganizationAddedEvent(entity, input));
                    }
                }

                private class PrivateOntologyOrganizationAddedEvent : OntologyOrganizationAddedEvent
                {
                    public PrivateOntologyOrganizationAddedEvent(OntologyOrganizationBase source, IOntologyOrganizationCreateInput input)
                        : base(source, input)
                    {

                    }
                }

                public void Handle(RemoveOntologyOrganizationCommand message)
                {
                    this.Handle(message.OntologyID, message.OrganizationID, true);
                }

                public void Handle(OntologyOrganizationRemovedEvent message)
                {
                    if (message.GetType() == typeof(PrivateOntologyOrganizationRemovedEvent))
                    {
                        return;
                    }
                    var entity = message.Source as OntologyOrganizationBase;
                    this.Handle(entity.OntologyID, entity.OrganizationID, false);
                }

                private void Handle(Guid ontologyID, Guid organizationID, bool isCommand)
                {
                    var _dic = set._dic;
                    var host = set.host;
                    var repository = host.AppHost.GetRequiredService<IRepository<OntologyOrganization>>();
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(ontologyID, out ontology))
                    {
                        throw new ValidationException("意外的本体标识" + ontologyID);
                    }
                    OntologyOrganization entity;
                    lock (this)
                    {
                        OrganizationState org = null;
                        OntologyOrganizationState bkState = null;
                        foreach (var item in _dic)
                        {
                            foreach (var item1 in item.Value)
                            {
                                if (item1.Value.OrganizationID == organizationID)
                                {
                                    bkState = item1.Value;
                                    org = item1.Key;
                                    break;
                                }
                            }
                            if (bkState != null)
                            {
                                break;
                            }
                        }
                        if (bkState == null)
                        {
                            return;
                        }
                        entity = repository.GetByKey(bkState.Id);
                        if (entity == null)
                        {
                            return;
                        }
                        _dic[ontology].Remove(org);
                        if (isCommand)
                        {
                            try
                            {
                                repository.Remove(entity);
                                repository.Context.Commit();
                            }
                            catch
                            {
                                _dic[ontology].Add(org, bkState);
                                repository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.AppHost.MessageDispatcher.DispatchMessage(new PrivateOntologyOrganizationRemovedEvent(entity));
                    }
                }

                private class PrivateOntologyOrganizationRemovedEvent : OntologyOrganizationRemovedEvent
                {
                    public PrivateOntologyOrganizationRemovedEvent(OntologyOrganizationBase source) : base(source) { }
                }

                public void Handle(AddOrganizationActionCommand message)
                {
                    // TODO:
                }

                public void Handle(OrganizationActionAddedEvent message)
                {
                    // TODO:
                }

                public void Handle(UpdateOrganizationActionCommand message)
                {
                    // TODO:
                }

                public void Handle(OrganizationActionUpdatedEvent message)
                {
                    // TODO:
                }

                public void Handle(RemoveOrganizationActionCommand message)
                {
                    // TODO:
                }

                public void Handle(OrganizationActionRemovedEvent message)
                {
                    // TODO:
                }
            }
        }
        #endregion

        // 内部类
        #region TopicSet
        private sealed class TopicSet
        {
            private readonly Dictionary<OntologyDescriptor, Dictionary<topicCode, TopicState>>
                _dic = new Dictionary<OntologyDescriptor, Dictionary<topicCode, TopicState>>();

            private bool initialized = false;
            private readonly object locker = new object();
            private readonly Guid _id = Guid.NewGuid();
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal TopicSet(NodeHost host)
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
                messageDispatcher.Register(new CreateTopicCommandHandler(this));
                messageDispatcher.Register(new UpdateTopicCommandHandler(this));
                messageDispatcher.Register(new DeleteTopicCommandHandler(this));
            }

            public Dictionary<string, TopicState> this[OntologyDescriptor ontology]
            {
                get
                {
                    if (!initialized)
                    {
                        Init();
                    }
                    if (!_dic.ContainsKey(ontology))
                    {
                        return new Dictionary<string, TopicState>(StringComparer.OrdinalIgnoreCase);
                    }

                    return _dic[ontology];
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
                    lock (locker)
                    {
                        if (!initialized)
                        {
                            _dic.Clear();
                            var list = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetTopics();
                            foreach (var item in list)
                            {
                                var ontology = NodeHost.Instance.Ontologies[item.OntologyID];
                                if (!_dic.ContainsKey(ontology))
                                {
                                    _dic.Add(ontology, new Dictionary<topicCode, TopicState>(StringComparer.OrdinalIgnoreCase));
                                }
                                var state = TopicState.Create(item);
                                _dic[ontology].Add(item.Code, state);
                            }
                            initialized = true;
                        }
                    }
                }
            }
            #endregion

            #region CreateTopicCommandHandler
            private class CreateTopicCommandHandler : IHandler<AddTopicCommand>
            {
                private readonly TopicSet topicSet;

                public CreateTopicCommandHandler(TopicSet topicSet)
                {
                    this.topicSet = topicSet;
                }

                public void Handle(AddTopicCommand message)
                {
                    var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    OntologyDescriptor ontology;
                    if (!NodeHost.Instance.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
                    {
                        throw new ValidationException("非法的本体标识" + message.Input.OntologyID);
                    }
                    if (ontology.Topics.ContainsKey(message.Input.Code))
                    {
                        throw new ValidationException("重复的编码");
                    }
                    if (ontology.Topics.Any(a => a.Value.Id == message.Input.Id.Value))
                    {
                        throw new ValidationException("给定标识的记录已经存在");
                    }
                    Topic entity = Topic.Create(message.Input);
                    lock (topicSet.locker)
                    {
                        try
                        {
                            topicSet._dic[ontology].Add(entity.Code, TopicState.Create(entity));
                            ontologyRepository.Context.RegisterNew(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            topicSet._dic[ontology].Remove(entity.Code);
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    topicSet.host.AppHost.PublishEvent(new TopicAddedEvent(entity));
                    topicSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion

            #region UpdateTopicCommandHandler
            private class UpdateTopicCommandHandler : IHandler<UpdateTopicCommand>
            {
                private readonly TopicSet topicSet;

                public UpdateTopicCommandHandler(TopicSet topicSet)
                {
                    this.topicSet = topicSet;
                }

                public void Handle(UpdateTopicCommand message)
                {
                    var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    TopicState topic = null;
                    OntologyDescriptor ontology = null;
                    foreach (var item in topicSet._dic)
                    {
                        foreach (var t in item.Value.Values)
                        {
                            if (t.Id == message.Input.Id)
                            {
                                topic = t;
                                ontology = item.Key;
                                break;
                            }
                        }
                    }
                    if (topic == null)
                    {
                        throw new NotExistException();
                    }
                    if (ontology.Topics.ContainsKey(message.Input.Code) && message.Input.Id != ontology.Topics[message.Input.Code].Id)
                    {
                        throw new ValidationException("重复的编码");
                    }
                    var entity = ontologyRepository.Context.Query<Topic>().FirstOrDefault(a => a.Id == message.Input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }
                    var bkState = TopicState.Create(entity);
                    entity.Update(message.Input);
                    var newState = TopicState.Create(entity);
                    bool stateChanged = newState != bkState;
                    lock (topicSet.locker)
                    {
                        try
                        {
                            if (stateChanged)
                            {
                                if (!topicSet._dic[ontology].ContainsKey(newState.Code))
                                {
                                    topicSet._dic[ontology].Add(newState.Code, newState);
                                    topicSet._dic[ontology].Remove(bkState.Code);
                                }
                                else
                                {
                                    topicSet._dic[ontology][newState.Code] = newState;
                                }
                            }
                            ontologyRepository.Context.RegisterModified(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                if (!topicSet._dic[ontology].ContainsKey(bkState.Code))
                                {
                                    topicSet._dic[ontology].Add(bkState.Code, bkState);
                                    topicSet._dic[ontology].Remove(newState.Code);
                                }
                                else
                                {
                                    topicSet._dic[ontology][bkState.Code] = bkState;
                                }
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    topicSet.host.AppHost.PublishEvent(new TopicUpdatedEvent(entity));
                    topicSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion

            #region DeleteTopicCommandHandler
            private class DeleteTopicCommandHandler : IHandler<RemoveTopicCommand>
            {
                private readonly TopicSet topicSet;

                public DeleteTopicCommandHandler(TopicSet topicSet)
                {
                    this.topicSet = topicSet;
                }

                public void Handle(RemoveTopicCommand message)
                {
                    var ontologyRepository = NodeHost.Instance.GetRequiredService<IRepository<Ontology>>();
                    TopicState topic = null;
                    OntologyDescriptor ontology = null;
                    foreach (var item in topicSet._dic)
                    {
                        foreach (var t in item.Value.Values)
                        {
                            if (t.Id == message.EntityID)
                            {
                                topic = t;
                                ontology = item.Key;
                                break;
                            }
                        }
                    }
                    if (topic == null)
                    {
                        return;
                    }
                    var entity = ontologyRepository.Context.Query<Topic>().FirstOrDefault(a => a.Id == message.EntityID);
                    if (entity == null)
                    {
                        return;
                    }
                    var bkState = TopicState.Create(entity);
                    lock (topicSet.locker)
                    {
                        try
                        {
                            topicSet._dic[ontology].Remove(bkState.Code);
                            ontologyRepository.Context.RegisterDeleted(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!topicSet._dic[ontology].ContainsKey(bkState.Code))
                            {
                                topicSet._dic[ontology].Add(bkState.Code, bkState);
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    topicSet.host.AppHost.PublishEvent(new TopicRemovedEvent(entity));
                    topicSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion
        }
        #endregion

        // 内部类
        #region ArchiveSet
        private sealed class ArchiveSet
        {
            private readonly Dictionary<Guid, ArchiveState> _dicByID = new Dictionary<Guid, ArchiveState>();
            private readonly Dictionary<OntologyDescriptor, List<ArchiveState>> _dicByOntology = new Dictionary<OntologyDescriptor, List<ArchiveState>>();
            private bool initialized = false;
            private readonly object locker = new object();
            private readonly Guid _id = Guid.NewGuid();
            private readonly NodeHost host;

            public Guid Id
            {
                get { return _id; }
            }

            /// <summary>
            /// 构造并接入总线
            /// </summary>
            internal ArchiveSet(NodeHost host)
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
                messageDispatcher.Register(new CreateArchiveCommandHandler(this));
                messageDispatcher.Register(new UpdateArchiveCommandHandler(this));
                messageDispatcher.Register(new DeleteArchiveCommandHandler(this));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="archiveID"></param>
            /// <param name="archive"></param>
            /// <returns></returns>
            public bool TryGetArchive(Guid archiveID, out ArchiveState archive)
            {
                if (!initialized)
                {
                    Init();
                }
                return _dicByID.TryGetValue(archiveID, out archive);
            }

            public IReadOnlyCollection<ArchiveState> GetArchives(OntologyDescriptor ontology)
            {
                if (!initialized)
                {
                    Init();
                }
                if (!_dicByOntology.ContainsKey(ontology))
                {
                    return new List<ArchiveState>();
                }
                return _dicByOntology[ontology];
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

            private void Init()
            {
                if (!initialized)
                {
                    lock (locker)
                    {
                        if (!initialized)
                        {
                            _dicByID.Clear();
                            _dicByOntology.Clear();
                            var list = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetArchives();
                            foreach (var entity in list)
                            {
                                var archive = ArchiveState.Create(ArchiveState.Create(entity));
                                _dicByID.Add(entity.Id, archive);
                                if (!_dicByOntology.ContainsKey(archive.Ontology))
                                {
                                    _dicByOntology.Add(archive.Ontology, new List<ArchiveState>());
                                }
                                _dicByOntology[archive.Ontology].Add(archive);
                            }
                            initialized = true;
                        }
                    }
                }
            }

            #region CreateArchiveCommandHandler
            private class CreateArchiveCommandHandler : IHandler<AddArchiveCommand>
            {
                private readonly ArchiveSet archiveSet;

                public CreateArchiveCommandHandler(ArchiveSet archiveSet)
                {
                    this.archiveSet = archiveSet;
                }

                public void Handle(AddArchiveCommand message)
                {
                    var archiveRepository = NodeHost.Instance.GetRequiredService<IRepository<Archive>>();
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    ArchiveState archive;
                    if (NodeHost.Instance.Ontologies.TryGetArchive(message.Input.Id.Value, out archive))
                    {
                        throw new ValidationException("给定标识的归档记录已经存在");
                    }
                    OntologyDescriptor ontology;
                    if (!NodeHost.Instance.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
                    {
                        throw new ValidationException("意外的本体标识" + message.Input.OntologyID);
                    }
                    int numberID = archiveRepository.FindAll().Where(a => a.OntologyID == message.Input.OntologyID).OrderByDescending(a => a.NumberID).Select(a => a.NumberID).FirstOrDefault() + 1;

                    var entity = Archive.Create(message.Input);

                    if (string.IsNullOrEmpty(entity.RdbmsType))
                    {
                        entity.RdbmsType = RdbmsType.SqlServer.ToName();// 默认为SqlServer数据库
                    }
                    lock (archiveSet.locker)
                    {
                        try
                        {
                            var state = ArchiveState.Create(ArchiveState.Create(entity));
                            state.Archive(numberID);
                            entity.ArchiveOn = state.ArchiveOn;
                            entity.NumberID = state.NumberID;
                            entity.FilePath = state.FilePath;
                            if (!archiveSet._dicByID.ContainsKey(entity.Id))
                            {
                                archiveSet._dicByID.Add(entity.Id, state);
                            }
                            if (!archiveSet._dicByOntology.ContainsKey(ontology))
                            {
                                archiveSet._dicByOntology.Add(ontology, new List<ArchiveState>());
                            }
                            if (!archiveSet._dicByOntology[ontology].Contains(state))
                            {
                                archiveSet._dicByOntology[ontology].Add(state);
                            }
                            archiveRepository.Add(entity);
                            archiveRepository.Context.Commit();
                        }
                        catch
                        {
                            if (archiveSet._dicByID.ContainsKey(entity.Id))
                            {
                                archiveSet._dicByID.Remove(entity.Id);
                            }
                            if (archiveSet._dicByOntology.ContainsKey(ontology) && archiveSet._dicByOntology[ontology].Any(a => a.Id == entity.Id))
                            {
                                var item = archiveSet._dicByOntology[ontology].First(a => a.Id == entity.Id);
                                archiveSet._dicByOntology[ontology].Remove(item);
                            }
                            archiveRepository.Context.Rollback();
                            throw;
                        }
                    }
                    archiveSet.host.AppHost.PublishEvent(new ArchivedEvent(entity));
                    archiveSet.host.AppHost.CommitEventBus();
                }
            }
            #endregion

            #region UpdateArchiveCommandHandler
            private class UpdateArchiveCommandHandler : IHandler<UpdateArchiveCommand>
            {
                private readonly ArchiveSet archiveSet;

                public UpdateArchiveCommandHandler(ArchiveSet archiveSet)
                {
                    this.archiveSet = archiveSet;
                }

                public void Handle(UpdateArchiveCommand message)
                {
                    var archiveRepository = NodeHost.Instance.GetRequiredService<IRepository<Archive>>();
                    ArchiveState archive;
                    if (!NodeHost.Instance.Ontologies.TryGetArchive(message.Input.Id, out archive))
                    {
                        throw new NotExistException();
                    }
                    var entity = archiveRepository.GetByKey(message.Input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }
                    var bkState = ArchiveState.Create(ArchiveState.Create(entity));

                    entity.Update(message.Input);

                    var newState = ArchiveState.Create(ArchiveState.Create(entity));
                    bool stateChanged = newState != bkState;
                    lock (archiveSet.locker)
                    {
                        try
                        {
                            if (stateChanged)
                            {
                                Update(newState);
                            }
                            archiveRepository.Update(entity);
                            archiveRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            archiveRepository.Context.Rollback();
                            throw;
                        }
                    }
                    if (stateChanged)
                    {
                        archiveSet.host.AppHost.PublishEvent(new ArchiveUpdatedEvent(entity));
                        archiveSet.host.AppHost.CommitEventBus();
                    }
                }

                private void Update(ArchiveState state)
                {
                    OntologyDescriptor ontology;
                    if (!archiveSet.host.Ontologies.TryGetOntology(state.OntologyID, out ontology))
                    {
                        throw new CoreException("意外的归档本体标识" + state.OntologyID);
                    }
                    archiveSet._dicByID[state.Id] = state;
                    if (archiveSet._dicByOntology.ContainsKey(ontology) && archiveSet._dicByOntology[ontology].Any(a => a.Id == state.Id))
                    {
                        var item = archiveSet._dicByOntology[ontology].First(a => a.Id == state.Id);
                        archiveSet._dicByOntology[ontology].Remove(item);
                        archiveSet._dicByOntology[ontology].Add(state);
                    }
                }
            }
            #endregion

            #region DeleteArchiveCommandHandler
            private class DeleteArchiveCommandHandler : IHandler<RemoveArchiveCommand>
            {
                private readonly ArchiveSet archiveSet;

                public DeleteArchiveCommandHandler(ArchiveSet archiveSet)
                {
                    this.archiveSet = archiveSet;
                }

                public void Handle(RemoveArchiveCommand message)
                {
                    var archiveRepository = NodeHost.Instance.GetRequiredService<IRepository<Archive>>();
                    ArchiveState archive;
                    if (!NodeHost.Instance.Ontologies.TryGetArchive(message.EntityID, out archive))
                    {
                        return;
                    }
                    Archive entity = archiveRepository.GetByKey(message.EntityID);
                    if (entity == null)
                    {
                        return;
                    }
                    var bkState = ArchiveState.Create(ArchiveState.Create(entity));
                    lock (archiveSet.locker)
                    {
                        try
                        {
                            archiveSet._dicByID.Remove(entity.Id);
                            if (archiveSet._dicByOntology.ContainsKey(archive.Ontology))
                            {
                                var item = archiveSet._dicByOntology[archive.Ontology].FirstOrDefault(a => a.Id == archive.Id);
                                if (item != null)
                                {
                                    archiveSet._dicByOntology[archive.Ontology].Remove(item);
                                }
                            }
                            archive.Ontology.EntityProvider.DropArchive(archive);
                            archiveRepository.Remove(entity);
                            archiveRepository.Context.Commit();
                        }
                        catch
                        {
                            archiveSet._dicByID.Add(entity.Id, bkState);
                            if (!archiveSet._dicByOntology.ContainsKey(archive.Ontology))
                            {
                                archiveSet._dicByOntology.Add(archive.Ontology, new List<ArchiveState>());
                            }
                            var item = archiveSet._dicByOntology[archive.Ontology].FirstOrDefault(a => a.Id == archive.Id);
                            if (item == null)
                            {
                                archiveSet._dicByOntology[archive.Ontology].Add(bkState);
                            }
                            archiveRepository.Context.Rollback();
                            throw;
                        }
                    }
                    archiveSet.host.AppHost.CommandBus.Publish(new ArchiveDeletedEvent(entity));
                    archiveSet.host.AppHost.CommandBus.Commit();
                }
            }
            #endregion
        }
        #endregion
    }
}