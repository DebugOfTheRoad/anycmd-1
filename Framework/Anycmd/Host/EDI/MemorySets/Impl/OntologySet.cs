using System;

namespace Anycmd.Host.EDI.MemorySets.Impl
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

        private readonly IAppHost host;

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
        public OntologySet(IAppHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
            var messageDispatcher = host.MessageDispatcher;
            if (messageDispatcher == null)
            {
                throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
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
                        var allOntologies = host.GetRequiredService<INodeHostBootstrap>().GetOntologies().OrderBy(s => s.SortCode);
                        foreach (var ontology in allOntologies)
                        {
                            if (!(ontology is OntologyBase))
                            {
                                throw new CoreException("本体模型必须继承OntologyBase基类");
                            }
                            var ontologyDescriptor = new OntologyDescriptor(host, OntologyState.Create(ontology));
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
            private readonly OntologySet set;

            public MessageHandler(OntologySet set)
            {
                this.set = set;
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
                var host = set.host;
                var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
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
                    if (host.Ontologies.TryGetOntology(input.Id.Value, out ontology))
                    {
                        throw new ValidationException("给定的标识标识的记录已经存在");
                    }

                    entity = Ontology.Create(input);

                    var descriptor = new OntologyDescriptor(host, OntologyState.Create(entity));
                    if (!set._ontologyDicByCode.ContainsKey(entity.Code))
                    {
                        set._ontologyDicByCode.Add(entity.Code, descriptor);
                    }
                    if (!set._ontologyDicByID.ContainsKey(entity.Id))
                    {
                        set._ontologyDicByID.Add(entity.Id, descriptor);
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
                            if (set._ontologyDicByCode.ContainsKey(entity.Code))
                            {
                                set._ontologyDicByCode.Remove(entity.Code);
                            }
                            if (set._ontologyDicByID.ContainsKey(entity.Id))
                            {
                                set._ontologyDicByID.Remove(entity.Id);
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateOntologyAddedEvent(entity, input));
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
                var host = set.host;
                var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                OntologyDescriptor bkState;
                if (!host.Ontologies.TryGetOntology(input.Id, out bkState))
                {
                    throw new NotExistException();
                }
                Ontology entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(input.Id, out ontology))
                    {
                        throw new NotExistException();
                    }
                    if (host.Ontologies.TryGetOntology(input.Code, out ontology) && ontology.Ontology.Id != input.Id)
                    {
                        throw new ValidationException("重复的编码");
                    }
                    entity = ontologyRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = new OntologyDescriptor(host, OntologyState.Create(entity));
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
                    host.MessageDispatcher.DispatchMessage(new PrivateOntologyUpdatedEvent(entity, input));
                }
            }

            private void Update(OntologyDescriptor state)
            {
                var oldState = set._ontologyDicByID[state.Ontology.Id];
                set._ontologyDicByID[state.Ontology.Id] = state;
                if (!set._ontologyDicByCode.ContainsKey(state.Ontology.Code))
                {
                    set._ontologyDicByCode.Add(state.Ontology.Code, state);
                    set._ontologyDicByCode.Remove(oldState.Ontology.Code);
                }
                else
                {
                    set._ontologyDicByCode[oldState.Ontology.Code] = state;
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
                var host = set.host;
                var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                OntologyDescriptor ontology;
                if (!host.Ontologies.TryGetOntology(message.EntityID, out ontology))
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
                if (host.Nodes.GetNodeOntologyCares().Any(a => a.OntologyID == entity.Id))
                {
                    throw new ValidationException("有节点关心了该本体时不能删除");
                }
                if (host.Nodes.GetNodeOntologyOrganizations().Any(a => a.OntologyID == entity.Id))
                {
                    throw new ValidationException("有节点关心了该本体下的组织结构时不能删除");
                }
                if (host.GetRequiredService<IRepository<Batch>>().FindAll().Any(a => a.OntologyID == entity.Id))
                {
                    throw new ValidationException("本体下有批处理记录时不能删除");
                }
                var bkState = set._ontologyDicByID[entity.Id];
                lock (set.locker)
                {
                    try
                    {
                        if (set._ontologyDicByID.ContainsKey(entity.Id))
                        {
                            set._ontologyDicByID.Remove(entity.Id);
                        }
                        if (set._ontologyDicByCode.ContainsKey(entity.Code))
                        {
                            set._ontologyDicByCode.Remove(entity.Code);
                        }
                        ontologyRepository.Remove(entity);
                        ontologyRepository.Context.Commit();
                    }
                    catch
                    {
                        if (!set._ontologyDicByID.ContainsKey(entity.Id))
                        {
                            set._ontologyDicByID.Add(entity.Id, bkState);
                        }
                        if (!set._ontologyDicByCode.ContainsKey(entity.Code))
                        {
                            set._ontologyDicByCode.Add(entity.Code, bkState);
                        }
                        ontologyRepository.Context.Rollback();
                        throw;
                    }
                }
                set.host.PublishEvent(new OntologyRemovedEvent(entity));
                set.host.CommitEventBus();
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
            private readonly IAppHost host;

            public Guid Id
            {
                get { return _id; }
            }

            #region Ctor
            internal ElementSet(IAppHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
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
                            var allElements = host.GetRequiredService<INodeHostBootstrap>().GetElements();
                            foreach (var element in allElements)
                            {
                                if (!_elementDicByID.ContainsKey(element.Id))
                                {
                                    var descriptor = new ElementDescriptor(host, ElementState.Create(host, element));
                                    _elementDicByID.Add(element.Id, descriptor);
                                    OntologyDescriptor ontology;
                                    host.Ontologies.TryGetOntology(element.OntologyID, out ontology);
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
                private readonly ElementSet set;

                public CreateElementCommandHandler(ElementSet set)
                {
                    this.set = set;
                }

                public void Handle(AddElementCommand message)
                {
                    var host = set.host;
                    var elementRepository = host.GetRequiredService<IRepository<Element>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    ElementDescriptor element;
                    if (host.Ontologies.TryGetElement(message.Input.Id.Value, out element))
                    {
                        throw new ValidationException("给定的标识标识的记录已经存在");
                    }
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
                    {
                        throw new ValidationException("意外的本体标识" + message.Input.OntologyID);
                    }
                    if (ontology.Elements.ContainsKey(message.Input.Code))
                    {
                        throw new ValidationException("重复的编码");
                    }

                    Element entity = Element.Create(message.Input);

                    lock (set.locker)
                    {
                        var descriptor = new ElementDescriptor(host, ElementState.Create(host, entity));
                        set._elementDicByID.Add(entity.Id, descriptor);
                        if (!set._elementDicByOntology.ContainsKey(ontology))
                        {
                            set._elementDicByOntology.Add(ontology, new Dictionary<string, ElementDescriptor>(StringComparer.OrdinalIgnoreCase));
                        }
                        set._elementDicByOntology[ontology].Add(entity.Code, descriptor);
                        try
                        {
                            elementRepository.Add(entity);
                            elementRepository.Context.Commit();
                        }
                        catch
                        {
                            set._elementDicByID.Remove(entity.Id);
                            if (set._elementDicByOntology.ContainsKey(ontology) && set._elementDicByOntology[ontology].ContainsKey(entity.Code))
                            {
                                set._elementDicByOntology[ontology].Remove(entity.Code);
                            }
                            elementRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.PublishEvent(new ElementAddedEvent(entity));
                    set.host.CommitEventBus();
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
                            elementSet.host.Handle(new AddElementCommand(entity));
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
                private readonly ElementSet set;

                public UpdateElementCommandHandler(ElementSet set)
                {
                    this.set = set;
                }

                public void Handle(UpdateElementCommand message)
                {
                    var host = set.host;
                    var elementRepository = host.GetRequiredService<IRepository<Element>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    ElementDescriptor element;
                    if (!host.Ontologies.TryGetElement(message.Input.Id, out element))
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
                    var bkState = set._elementDicByID[entity.Id];

                    entity.Update(message.Input);

                    var newState = new ElementDescriptor(host, ElementState.Create(host, entity));
                    bool stateChanged = newState != bkState;
                    lock (set.locker)
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
                            set.host.PublishEvent(new ElementUpdatedEvent(entity));
                            set.host.CommitEventBus();
                        }
                    }
                }

                private void Update(ElementDescriptor state)
                {
                    var oldKey = set._elementDicByID[state.Element.Id].Element.Code;
                    set._elementDicByID[state.Element.Id] = state;
                    if (!set._elementDicByOntology[state.Ontology].ContainsKey(state.Element.Code))
                    {
                        set._elementDicByOntology[state.Ontology].Add(state.Element.Code, state);
                        set._elementDicByOntology[state.Ontology].Remove(oldKey);
                    }
                    else
                    {
                        set._elementDicByOntology[state.Ontology][state.Element.Code] = state;
                    }
                }
            }
            #endregion

            private class DeleteElementCommandHandler : IHandler<RemoveElementCommand>
            {
                private readonly ElementSet set;

                public DeleteElementCommandHandler(ElementSet set)
                {
                    this.set = set;
                }

                public void Handle(RemoveElementCommand message)
                {
                    var host = set.host;
                    var elementRepository = host.GetRequiredService<IRepository<Element>>();
                    ElementDescriptor element;
                    if (!host.Ontologies.TryGetElement(message.EntityID, out element))
                    {
                        return;
                    }
                    Element entity = elementRepository.GetByKey(message.EntityID);
                    if (entity == null)
                    {
                        return;
                    }
                    var bkState = set._elementDicByID[entity.Id];
                    lock (set.locker)
                    {
                        try
                        {
                            set._elementDicByID.Remove(entity.Id);
                            set._elementDicByOntology[bkState.Ontology].Remove(bkState.Element.Code);
                            elementRepository.Remove(entity);
                            elementRepository.Context.Commit();
                        }
                        catch
                        {
                            set._elementDicByID.Add(entity.Id, bkState);
                            set._elementDicByOntology[bkState.Ontology].Add(bkState.Element.Code, bkState);
                            elementRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.PublishEvent(new ElementRemovedEvent(entity));
                    set.host.CommitEventBus();
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
            private readonly IAppHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal ActionSet(IAppHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
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
                            var actions = host.GetRequiredService<INodeHostBootstrap>().GetActions();
                            var nodeElementActions = host.GetRequiredService<INodeHostBootstrap>().GetNodeElementActions();
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
                private readonly ActionSet set;

                public CreateActionCommandHandler(ActionSet set)
                {
                    this.set = set;
                }

                public void Handle(AddActionCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Verb))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    if (set._actionsByID.ContainsKey(message.Input.Id.Value))
                    {
                        throw new ValidationException("给定的标识标识的记录已经存在");
                    }
                    OntologyDescriptor ontology;
                    if (!set.host.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
                    {
                        throw new ValidationException("非法的本体标识");
                    }
                    if (ontology.Actions.ContainsKey(new Verb(message.Input.Verb)))
                    {
                        throw new ValidationException("重复的动词");
                    }

                    var entity = Action.Create(message.Input);

                    lock (set.locker)
                    {
                        try
                        {
                            var state = ActionState.Create(entity);
                            set._actionsByID.Add(entity.Id, state);
                            set._actionDicByVerb[ontology].Add(new Verb(entity.Verb), state);
                            ontologyRepository.Context.RegisterNew(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            set._actionsByID.Remove(entity.Id);
                            set._actionDicByVerb[ontology].Remove(new Verb(entity.Verb));
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    host.PublishEvent(new ActionAddedEvent(entity));
                    host.CommitEventBus();
                }
            }
            #endregion

            #region UpdateActionCommandHandler
            private class UpdateActionCommandHandler : IHandler<UpdateActionCommand>
            {
                private readonly ActionSet set;

                public UpdateActionCommandHandler(ActionSet actionSet)
                {
                    this.set = actionSet;
                }

                public void Handle(UpdateActionCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Verb))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    bool exist = false;
                    OntologyDescriptor ontology = null;
                    foreach (var item in host.Ontologies)
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
                    lock (set.locker)
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
                        set.host.PublishEvent(new ActionUpdatedEvent(entity));
                        set.host.CommitEventBus();
                    }
                }

                private void Update(ActionState state)
                {
                    var ontology = set.host.Ontologies[state.OntologyID];
                    var newVerb = state.ActionVerb;
                    var oldVerb = set._actionsByID[state.Id].ActionVerb;
                    set._actionsByID[state.Id] = state;
                    if (!set._actionDicByVerb[ontology].ContainsKey(newVerb))
                    {
                        set._actionDicByVerb[ontology].Add(newVerb, state);
                        set._actionDicByVerb[ontology].Remove(oldVerb);
                    }
                    else
                    {
                        set._actionDicByVerb[ontology][newVerb] = state;
                    }
                }
            }
            #endregion

            #region DeleteActionCommandHandler
            private class DeleteActionCommandHandler : IHandler<RemoveActionCommand>
            {
                private readonly ActionSet set;

                public DeleteActionCommandHandler(ActionSet set)
                {
                    this.set = set;
                }

                public void Handle(RemoveActionCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    bool exist = false;
                    OntologyDescriptor ontology = null;
                    foreach (var item in host.Ontologies)
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
                    lock (set.locker)
                    {
                        try
                        {
                            set._actionsByID.Remove(entity.Id);
                            set._actionDicByVerb[ontology].Remove(bkState.ActionVerb);
                            ontologyRepository.Context.RegisterDeleted(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            set._actionsByID.Add(entity.Id, bkState);
                            set._actionDicByVerb[ontology].Add(bkState.ActionVerb, bkState);
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    host.PublishEvent(new ActionRemovedEvent(entity));
                    host.CommitEventBus();
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
            private readonly IAppHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal InfoGroupSet(IAppHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
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
                            var infoGroups = host.GetRequiredService<INodeHostBootstrap>().GetInfoGroups().OrderBy(a => a.SortCode);
                            foreach (var ontology in host.Ontologies)
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
                private readonly InfoGroupSet set;

                public CreateInfoGroupCommandHandler(InfoGroupSet set)
                {
                    this.set = set;
                }

                public void Handle(AddInfoGroupCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
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

                    lock (set.locker)
                    {
                        try
                        {
                            if (!set._dic.ContainsKey(ontology))
                            {
                                set._dic.Add(ontology, new List<InfoGroupState>());
                            }
                            if (!set._dic[ontology].Any(a => a.Id == entity.Id))
                            {
                                set._dic[ontology].Add(InfoGroupState.Create(entity));
                            }
                            ontologyRepository.Context.RegisterNew(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (set._dic.ContainsKey(ontology))
                            {
                                var item = set._dic[ontology].FirstOrDefault(a => a.Id == entity.Id);
                                if (item != null)
                                {
                                    set._dic[ontology].Remove(item);
                                }
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.PublishEvent(new InfoGroupAddedEvent(entity));
                    set.host.CommitEventBus();
                }
            }
            #endregion

            #region UpdateInfoGroupCommandHandler
            private class UpdateInfoGroupCommandHandler : IHandler<UpdateInfoGroupCommand>
            {
                private readonly InfoGroupSet set;

                public UpdateInfoGroupCommandHandler(InfoGroupSet set)
                {
                    this.set = set;
                }

                public void Handle(UpdateInfoGroupCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    InfoGroup entity;
                    bool stateChanged = false;
                    lock (set.locker)
                    {
                        entity = ontologyRepository.Context.Query<InfoGroup>().FirstOrDefault(a => a.Id == message.Input.Id);
                        if (entity == null)
                        {
                            throw new NotExistException();
                        }
                        OntologyDescriptor ontology;
                        if (!set.host.Ontologies.TryGetOntology(entity.OntologyID, out ontology))
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
                        set.host.PublishEvent(new InfoGroupUpdatedEvent(entity));
                        set.host.CommitEventBus();
                    }
                }

                private void Update(InfoGroupState state)
                {
                    OntologyDescriptor ontology = set.host.Ontologies[state.OntologyID];
                    var item = set._dic[ontology].First(a => a.Id == state.Id);
                    set._dic[ontology].Remove(item);
                    set._dic[ontology].Add(state);
                }
            }
            #endregion

            #region DeleteInfoGroupCommandHandler
            private class DeleteInfoGroupCommandHandler : IHandler<RemoveInfoGroupCommand>
            {
                private readonly InfoGroupSet set;

                public DeleteInfoGroupCommandHandler(InfoGroupSet set)
                {
                    this.set = set;
                }

                public void Handle(RemoveInfoGroupCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    InfoGroup entity = ontologyRepository.Context.Query<InfoGroup>().FirstOrDefault(a => a.Id == message.EntityID);
                    if (entity == null)
                    {
                        return;
                    }
                    var bkState = InfoGroupState.Create(entity);
                    lock (set.locker)
                    {
                        try
                        {
                            set._dic[host.Ontologies[bkState.OntologyID]].Remove(bkState);
                            ontologyRepository.Context.RegisterDeleted(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            set._dic[host.Ontologies[bkState.OntologyID]].Add(bkState);
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.PublishEvent(new InfoGroupRemovedEvent(entity));
                    set.host.CommitEventBus();
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
            private readonly IAppHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal OntologyOrganizationSet(IAppHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
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
                            var ontologyOrgs = host.GetRequiredService<INodeHostBootstrap>().GetOntologyOrganizations();
                            foreach (var ontologyOrg in ontologyOrgs)
                            {
                                OrganizationState org;
                                OntologyDescriptor ontology;
                                if (!host.Ontologies.TryGetOntology(ontologyOrg.OntologyID, out ontology))
                                {
                                    throw new CoreException("意外的本体组织结构本体标识" + ontologyOrg.OntologyID);
                                }
                                if (host.OrganizationSet.TryGetOrganization(ontologyOrg.OrganizationID, out org))
                                {
                                    if (!_dic.ContainsKey(ontology))
                                    {
                                        _dic.Add(ontology, new Dictionary<OrganizationState, OntologyOrganizationState>());
                                    }
                                    var ontologyOrgState = OntologyOrganizationState.Create(host, ontologyOrg);
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
                    var repository = host.GetRequiredService<IRepository<OntologyOrganization>>();
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(input.OntologyID, out ontology))
                    {
                        throw new CoreException("意外的本体标识" + input.OntologyID);
                    }
                    OrganizationState org;
                    if (!host.OrganizationSet.TryGetOrganization(input.OrganizationID, out org))
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
                        var ontologyOrgState = OntologyOrganizationState.Create(host, entity);
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
                        host.MessageDispatcher.DispatchMessage(new PrivateOntologyOrganizationAddedEvent(entity, input));
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
                    var repository = host.GetRequiredService<IRepository<OntologyOrganization>>();
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
                        host.MessageDispatcher.DispatchMessage(new PrivateOntologyOrganizationRemovedEvent(entity));
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
            private readonly IAppHost host;

            public Guid Id
            {
                get { return _id; }
            }

            internal TopicSet(IAppHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
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
                            var list = host.GetRequiredService<INodeHostBootstrap>().GetTopics();
                            foreach (var item in list)
                            {
                                var ontology = host.Ontologies[item.OntologyID];
                                if (!_dic.ContainsKey(ontology))
                                {
                                    _dic.Add(ontology, new Dictionary<topicCode, TopicState>(StringComparer.OrdinalIgnoreCase));
                                }
                                var state = TopicState.Create(host, item);
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
                private readonly TopicSet set;

                public CreateTopicCommandHandler(TopicSet set)
                {
                    this.set = set;
                }

                public void Handle(AddTopicCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
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
                    lock (set.locker)
                    {
                        try
                        {
                            set._dic[ontology].Add(entity.Code, TopicState.Create(host, entity));
                            ontologyRepository.Context.RegisterNew(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            set._dic[ontology].Remove(entity.Code);
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.PublishEvent(new TopicAddedEvent(entity));
                    set.host.CommitEventBus();
                }
            }
            #endregion

            #region UpdateTopicCommandHandler
            private class UpdateTopicCommandHandler : IHandler<UpdateTopicCommand>
            {
                private readonly TopicSet set;

                public UpdateTopicCommandHandler(TopicSet set)
                {
                    this.set = set;
                }

                public void Handle(UpdateTopicCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    if (string.IsNullOrEmpty(message.Input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    TopicState topic = null;
                    OntologyDescriptor ontology = null;
                    foreach (var item in set._dic)
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
                    var bkState = TopicState.Create(host, entity);
                    entity.Update(message.Input);
                    var newState = TopicState.Create(host, entity);
                    bool stateChanged = newState != bkState;
                    lock (set.locker)
                    {
                        try
                        {
                            if (stateChanged)
                            {
                                if (!set._dic[ontology].ContainsKey(newState.Code))
                                {
                                    set._dic[ontology].Add(newState.Code, newState);
                                    set._dic[ontology].Remove(bkState.Code);
                                }
                                else
                                {
                                    set._dic[ontology][newState.Code] = newState;
                                }
                            }
                            ontologyRepository.Context.RegisterModified(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                if (!set._dic[ontology].ContainsKey(bkState.Code))
                                {
                                    set._dic[ontology].Add(bkState.Code, bkState);
                                    set._dic[ontology].Remove(newState.Code);
                                }
                                else
                                {
                                    set._dic[ontology][bkState.Code] = bkState;
                                }
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.PublishEvent(new TopicUpdatedEvent(entity));
                    set.host.CommitEventBus();
                }
            }
            #endregion

            #region DeleteTopicCommandHandler
            private class DeleteTopicCommandHandler : IHandler<RemoveTopicCommand>
            {
                private readonly TopicSet set;

                public DeleteTopicCommandHandler(TopicSet set)
                {
                    this.set = set;
                }

                public void Handle(RemoveTopicCommand message)
                {
                    var host = set.host;
                    var ontologyRepository = host.GetRequiredService<IRepository<Ontology>>();
                    TopicState topic = null;
                    OntologyDescriptor ontology = null;
                    foreach (var item in set._dic)
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
                    var bkState = TopicState.Create(host, entity);
                    lock (set.locker)
                    {
                        try
                        {
                            set._dic[ontology].Remove(bkState.Code);
                            ontologyRepository.Context.RegisterDeleted(entity);
                            ontologyRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!set._dic[ontology].ContainsKey(bkState.Code))
                            {
                                set._dic[ontology].Add(bkState.Code, bkState);
                            }
                            ontologyRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.PublishEvent(new TopicRemovedEvent(entity));
                    set.host.CommitEventBus();
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
            private readonly IAppHost host;

            public Guid Id
            {
                get { return _id; }
            }

            /// <summary>
            /// 构造并接入总线
            /// </summary>
            internal ArchiveSet(IAppHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
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
                            var list = host.GetRequiredService<INodeHostBootstrap>().GetArchives();
                            foreach (var entity in list)
                            {
                                var archive = ArchiveState.Create(host, entity);
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
                private readonly ArchiveSet set;

                public CreateArchiveCommandHandler(ArchiveSet set)
                {
                    this.set = set;
                }

                public void Handle(AddArchiveCommand message)
                {
                    var host = set.host;
                    var archiveRepository = host.GetRequiredService<IRepository<Archive>>();
                    if (!message.Input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    ArchiveState archive;
                    if (host.Ontologies.TryGetArchive(message.Input.Id.Value, out archive))
                    {
                        throw new ValidationException("给定标识的归档记录已经存在");
                    }
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(message.Input.OntologyID, out ontology))
                    {
                        throw new ValidationException("意外的本体标识" + message.Input.OntologyID);
                    }
                    int numberID = archiveRepository.FindAll().Where(a => a.OntologyID == message.Input.OntologyID).OrderByDescending(a => a.NumberID).Select(a => a.NumberID).FirstOrDefault() + 1;

                    var entity = Archive.Create(message.Input);

                    if (string.IsNullOrEmpty(entity.RdbmsType))
                    {
                        entity.RdbmsType = RdbmsType.SqlServer.ToName();// 默认为SqlServer数据库
                    }
                    lock (set.locker)
                    {
                        try
                        {
                            var state = ArchiveState.Create(host, entity);
                            state.Archive(numberID);
                            entity.ArchiveOn = state.ArchiveOn;
                            entity.NumberID = state.NumberID;
                            entity.FilePath = state.FilePath;
                            if (!set._dicByID.ContainsKey(entity.Id))
                            {
                                set._dicByID.Add(entity.Id, state);
                            }
                            if (!set._dicByOntology.ContainsKey(ontology))
                            {
                                set._dicByOntology.Add(ontology, new List<ArchiveState>());
                            }
                            if (!set._dicByOntology[ontology].Contains(state))
                            {
                                set._dicByOntology[ontology].Add(state);
                            }
                            archiveRepository.Add(entity);
                            archiveRepository.Context.Commit();
                        }
                        catch
                        {
                            if (set._dicByID.ContainsKey(entity.Id))
                            {
                                set._dicByID.Remove(entity.Id);
                            }
                            if (set._dicByOntology.ContainsKey(ontology) && set._dicByOntology[ontology].Any(a => a.Id == entity.Id))
                            {
                                var item = set._dicByOntology[ontology].First(a => a.Id == entity.Id);
                                set._dicByOntology[ontology].Remove(item);
                            }
                            archiveRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.PublishEvent(new ArchivedEvent(entity));
                    set.host.CommitEventBus();
                }
            }
            #endregion

            #region UpdateArchiveCommandHandler
            private class UpdateArchiveCommandHandler : IHandler<UpdateArchiveCommand>
            {
                private readonly ArchiveSet set;

                public UpdateArchiveCommandHandler(ArchiveSet set)
                {
                    this.set = set;
                }

                public void Handle(UpdateArchiveCommand message)
                {
                    var host = set.host;
                    var archiveRepository = host.GetRequiredService<IRepository<Archive>>();
                    ArchiveState archive;
                    if (!host.Ontologies.TryGetArchive(message.Input.Id, out archive))
                    {
                        throw new NotExistException();
                    }
                    var entity = archiveRepository.GetByKey(message.Input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }
                    var bkState = ArchiveState.Create(host, entity);

                    entity.Update(message.Input);

                    var newState = ArchiveState.Create(host, entity);
                    bool stateChanged = newState != bkState;
                    lock (set.locker)
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
                        set.host.PublishEvent(new ArchiveUpdatedEvent(entity));
                        set.host.CommitEventBus();
                    }
                }

                private void Update(ArchiveState state)
                {
                    OntologyDescriptor ontology;
                    if (!set.host.Ontologies.TryGetOntology(state.OntologyID, out ontology))
                    {
                        throw new CoreException("意外的归档本体标识" + state.OntologyID);
                    }
                    set._dicByID[state.Id] = state;
                    if (set._dicByOntology.ContainsKey(ontology) && set._dicByOntology[ontology].Any(a => a.Id == state.Id))
                    {
                        var item = set._dicByOntology[ontology].First(a => a.Id == state.Id);
                        set._dicByOntology[ontology].Remove(item);
                        set._dicByOntology[ontology].Add(state);
                    }
                }
            }
            #endregion

            #region DeleteArchiveCommandHandler
            private class DeleteArchiveCommandHandler : IHandler<RemoveArchiveCommand>
            {
                private readonly ArchiveSet set;

                public DeleteArchiveCommandHandler(ArchiveSet set)
                {
                    this.set = set;
                }

                public void Handle(RemoveArchiveCommand message)
                {
                    var host = set.host;
                    var archiveRepository = host.GetRequiredService<IRepository<Archive>>();
                    ArchiveState archive;
                    if (!host.Ontologies.TryGetArchive(message.EntityID, out archive))
                    {
                        return;
                    }
                    Archive entity = archiveRepository.GetByKey(message.EntityID);
                    if (entity == null)
                    {
                        return;
                    }
                    var bkState = ArchiveState.Create(host, entity);
                    lock (set.locker)
                    {
                        try
                        {
                            set._dicByID.Remove(entity.Id);
                            if (set._dicByOntology.ContainsKey(archive.Ontology))
                            {
                                var item = set._dicByOntology[archive.Ontology].FirstOrDefault(a => a.Id == archive.Id);
                                if (item != null)
                                {
                                    set._dicByOntology[archive.Ontology].Remove(item);
                                }
                            }
                            archive.Ontology.EntityProvider.DropArchive(archive);
                            archiveRepository.Remove(entity);
                            archiveRepository.Context.Commit();
                        }
                        catch
                        {
                            set._dicByID.Add(entity.Id, bkState);
                            if (!set._dicByOntology.ContainsKey(archive.Ontology))
                            {
                                set._dicByOntology.Add(archive.Ontology, new List<ArchiveState>());
                            }
                            var item = set._dicByOntology[archive.Ontology].FirstOrDefault(a => a.Id == archive.Id);
                            if (item == null)
                            {
                                set._dicByOntology[archive.Ontology].Add(bkState);
                            }
                            archiveRepository.Context.Rollback();
                            throw;
                        }
                    }
                    set.host.CommandBus.Publish(new ArchiveDeletedEvent(entity));
                    set.host.CommandBus.Commit();
                }
            }
            #endregion
        }
        #endregion
    }
}