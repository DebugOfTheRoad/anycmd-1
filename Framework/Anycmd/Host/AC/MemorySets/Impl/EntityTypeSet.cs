
namespace Anycmd.Host.AC.MemorySets.Impl
{
    using AC.Infra;
    using Anycmd.AC.Infra;
    using Anycmd.Bus;
    using Exceptions;
    using Extensions;
    using Host;
    using Infra.Messages;
    using Model;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ValueObjects;
    using codespace = System.String;
    using entityTypeCode = System.String;
    using entityTypeID = System.Guid;
    using propertyCode = System.String;
    using propertyID = System.Guid;

    public sealed class EntityTypeSet : IEntityTypeSet
    {
        public static readonly IEntityTypeSet Empty = new EntityTypeSet(AppHost.Empty);

        private readonly Dictionary<entityTypeID, EntityTypeState> _dicByID = new Dictionary<entityTypeID, EntityTypeState>();
        private readonly Dictionary<codespace, Dictionary<entityTypeCode, EntityTypeState>> _dicByCode = new Dictionary<codespace, Dictionary<entityTypeCode, EntityTypeState>>(StringComparer.OrdinalIgnoreCase);
        private bool _initialized = false;

        private readonly IAppHost host;
        private readonly Guid _id = Guid.NewGuid();
        private readonly PropertySet propertySet;

        public Guid Id
        {
            get { return _id; }
        }

        #region Ctor
        public EntityTypeSet(IAppHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
            propertySet = new PropertySet(host);
            var messageDispatcher = host.MessageDispatcher;
            if (messageDispatcher == null)
            {
                throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
            }
            var handler = new MessageHandler(this);
            messageDispatcher.Register((IHandler<AddEntityTypeCommand>)handler);
            messageDispatcher.Register((IHandler<EntityTypeAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateEntityTypeCommand>)handler);
            messageDispatcher.Register((IHandler<EntityTypeUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveEntityTypeCommand>)handler);
            messageDispatcher.Register((IHandler<EntityTypeRemovedEvent>)handler);
        }
        #endregion

        public bool TryGetEntityType(Guid entityTypeID, out EntityTypeState entityType)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.TryGetValue(entityTypeID, out entityType);
        }

        public bool TryGetEntityType(string codespace, string entityTypeCode, out EntityTypeState entityType)
        {
            if (codespace == null)
            {
                throw new ArgumentNullException("codespace");
            }
            if (entityTypeCode == null)
            {
                throw new ArgumentNullException("entityTypeCode");
            }
            if (!_initialized)
            {
                Init();
            }
            if (!_dicByCode.ContainsKey(codespace))
            {
                entityType = EntityTypeState.Empty;
                return false;
            }
            return _dicByCode[codespace].TryGetValue(entityTypeCode, out entityType);
        }

        public bool TryGetProperty(Guid propertyID, out PropertyState property)
        {
            if (!_initialized)
            {
                Init();
            }
            return propertySet.TryGetProperty(propertyID, out property);
        }

        public bool TryGetProperty(EntityTypeState entityType, string propertyCode, out PropertyState property)
        {
            if (!_initialized)
            {
                Init();
            }
            if (propertyCode == null)
            {
                throw new ArgumentNullException("propertyCode");
            }
            return propertySet.TryGetProperty(entityType, propertyCode, out property);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityTypeID"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<string, PropertyState> GetProperties(EntityTypeState entityType)
        {
            if (!_initialized)
            {
                Init();
            }
            return propertySet.GetProperties(entityType);
        }

        public IEnumerable<PropertyState> GetProperties()
        {
            if (!_initialized)
            {
                Init();
            }
            return propertySet;
        }

        internal void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
        }

        public IEnumerator<EntityTypeState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.Values.GetEnumerator();
        }

        #region Init
        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _dicByID.Clear();
                        _dicByCode.Clear();
                        var entityTypes = host.GetRequiredService<IAppHostBootstrap>().GetAllEntityTypes().OrderBy(a => a.SortCode);
                        foreach (var entityType in entityTypes)
                        {
                            if (!(entityType is EntityTypeBase))
                            {
                                throw new CoreException(entityType.GetType().Name + "必须继承" + typeof(EntityTypeBase).Name);
                            }
                            if (_dicByID.ContainsKey(entityType.Id))
                            {
                                throw new CoreException("意外的重复的实体类型标识" + entityType.Id);
                            }
                            if (!_dicByCode.ContainsKey(entityType.Codespace))
                            {
                                _dicByCode.Add(entityType.Codespace, new Dictionary<entityTypeCode, EntityTypeState>(StringComparer.OrdinalIgnoreCase));
                            }
                            if (_dicByCode[entityType.Codespace].ContainsKey(entityType.Code))
                            {
                                throw new CoreException("意外的重复的实体类型编码" + entityType.Codespace + "." + entityType.Code);
                            }
                            var map = host.GetEntityTypeMaps().FirstOrDefault(a => a.Codespace.Equals(entityType.Codespace, StringComparison.OrdinalIgnoreCase) && a.Code.Equals(entityType.Code, StringComparison.OrdinalIgnoreCase));
                            var entityTypeState = EntityTypeState.Create(host, entityType, map);
                            _dicByCode[entityType.Codespace].Add(entityType.Code, entityTypeState);
                            _dicByID.Add(entityType.Id, entityTypeState);
                        }
                        _initialized = true;
                    }
                }
            }
        }
        #endregion

        #region MessageHandler
        private class MessageHandler :
            IHandler<EntityTypeRemovedEvent>, 
            IHandler<AddEntityTypeCommand>, 
            IHandler<EntityTypeAddedEvent>, 
            IHandler<UpdateEntityTypeCommand>, 
            IHandler<EntityTypeUpdatedEvent>, 
            IHandler<RemoveEntityTypeCommand>
        {
            private readonly EntityTypeSet set;

            public MessageHandler(EntityTypeSet set)
            {
                this.set = set;
            }

            public void Handle(AddEntityTypeCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(EntityTypeAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateEntityTypeAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IEntityTypeCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var entityTypeRepository = host.GetRequiredService<IRepository<EntityType>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识不能为空");
                }
                EntityType entity;
                lock (this)
                {
                    EntityTypeState entityType;
                    if (host.EntityTypeSet.TryGetEntityType(input.Id.Value, out entityType))
                    {
                        throw new CoreException("重复的实体类型标识" + input.Id);
                    }
                    if (host.EntityTypeSet.TryGetEntityType(input.Codespace, input.Code, out entityType))
                    {
                        throw new ValidationException("重复的编码");
                    }

                    entity = EntityType.Create(input);

                    var map = host.GetEntityTypeMaps().FirstOrDefault(a => a.Codespace.Equals(entity.Codespace, StringComparison.OrdinalIgnoreCase) && a.Code.Equals(entity.Code, StringComparison.OrdinalIgnoreCase));
                    var state = EntityTypeState.Create(host, entity, map);
                    if (!_dicByID.ContainsKey(entity.Id))
                    {
                        _dicByID.Add(entity.Id, state);
                    }
                    if (!_dicByCode.ContainsKey(state.Codespace))
                    {
                        _dicByCode.Add(state.Codespace, new Dictionary<entityTypeCode, EntityTypeState>(StringComparer.OrdinalIgnoreCase));
                    }
                    if (!_dicByCode[state.Codespace].ContainsKey(state.Code))
                    {
                        _dicByCode[state.Codespace].Add(state.Code, state);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            entityTypeRepository.Add(entity);
                            entityTypeRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_dicByID.ContainsKey(entity.Id))
                            {
                                _dicByID.Remove(entity.Id);
                            }
                            if (_dicByCode.ContainsKey(entity.Codespace) && _dicByCode[entity.Codespace].ContainsKey(entity.Code))
                            {
                                _dicByCode[entity.Codespace].Remove(entity.Code);
                            }
                            entityTypeRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateEntityTypeAddedEvent(entity, input));
                }
            }

            private class PrivateEntityTypeAddedEvent : EntityTypeAddedEvent
            {
                public PrivateEntityTypeAddedEvent(EntityTypeBase source, IEntityTypeCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateEntityTypeCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(EntityTypeUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateEntityTypeUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IEntityTypeUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var entityTypeRepository = host.GetRequiredService<IRepository<EntityType>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                EntityTypeState bkState;
                if (!host.EntityTypeSet.TryGetEntityType(input.Id, out bkState))
                {
                    throw new NotExistException();
                }
                EntityType entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    EntityTypeState entityType;
                    if (host.EntityTypeSet.TryGetEntityType(input.Codespace, input.Code, out entityType) && entityType.Id != input.Id)
                    {
                        throw new ValidationException("重复的编码");
                    }
                    entity = entityTypeRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new ValidationException("更新的实体不存在");
                    }
                    var map = host.GetEntityTypeMaps().FirstOrDefault(a => a.Codespace.Equals(entity.Codespace, StringComparison.OrdinalIgnoreCase) && a.Code.Equals(entity.Code, StringComparison.OrdinalIgnoreCase));

                    entity.Update(input);

                    var newState = EntityTypeState.Create(host, entity, map);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            entityTypeRepository.Update(entity);
                            entityTypeRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            entityTypeRepository.Context.Rollback();
                            throw;
                        }
                    }
                    if (!stateChanged)
                    {
                        return;
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateEntityTypeUpdatedEvent(entity, input));
                }
            }

            private void Update(EntityTypeState state)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var oldState = _dicByID[state.Id];
                string oldCodespace = oldState.Codespace;
                string newCodespace = state.Codespace;
                string oldKey = oldState.Code;
                string newKey = state.Code;
                _dicByID[state.Id] = state;
                if (!_dicByCode[oldCodespace].ContainsKey(newKey))
                {
                    _dicByCode[oldCodespace].Add(newKey, state);
                    _dicByCode[oldCodespace].Remove(oldKey);
                }
                else
                {
                    _dicByCode[oldCodespace][newKey] = state;
                }
                if (!_dicByCode.ContainsKey(newCodespace))
                {
                    _dicByCode.Add(newCodespace, new Dictionary<entityTypeCode, EntityTypeState>(StringComparer.OrdinalIgnoreCase));
                    foreach (var item in _dicByCode[oldCodespace])
                    {
                        _dicByCode[newCodespace].Add(item.Key, item.Value);
                    }
                    _dicByCode.Remove(oldCodespace);
                }
            }

            private class PrivateEntityTypeUpdatedEvent : EntityTypeUpdatedEvent
            {
                public PrivateEntityTypeUpdatedEvent(EntityTypeBase source, IEntityTypeUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveEntityTypeCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(EntityTypeRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateEntityTypeRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid entityTypeID, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var propertyRepository = host.GetRequiredService<IRepository<Property>>();
                var entityTypeRepository = host.GetRequiredService<IRepository<EntityType>>();
                EntityTypeState bkState;
                if (!host.EntityTypeSet.TryGetEntityType(entityTypeID, out bkState))
                {
                    return;
                }
                EntityType entity;
                lock (bkState)
                {
                    entity = entityTypeRepository.GetByKey(entityTypeID);
                    if (entity == null)
                    {
                        return;
                    }
                    var properties = host.EntityTypeSet.GetProperties(bkState);
                    if (properties != null && properties.Count > 0)
                    {
                        throw new ValidationException("实体类型有属性后不能删除");
                    }
                    if (_dicByID.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new EntityTypeRemovingEvent(entity));
                        }
                        var entityType = _dicByID[bkState.Id];
                        if (_dicByCode.ContainsKey(entityType.Codespace) && _dicByCode[entityType.Codespace].ContainsKey(entityType.Code))
                        {
                            _dicByCode[entityType.Codespace].Remove(entityType.Code);
                        }
                        _dicByID.Remove(entityType.Id);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            entityTypeRepository.Remove(entity);
                            entityTypeRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_dicByID.ContainsKey(bkState.Id))
                            {
                                _dicByID.Add(bkState.Id, bkState);
                            }
                            if (!_dicByCode.ContainsKey(bkState.Codespace))
                            {
                                _dicByCode.Add(bkState.Codespace, new Dictionary<propertyCode, EntityTypeState>(StringComparer.OrdinalIgnoreCase));
                            }
                            if (!_dicByCode[bkState.Codespace].ContainsKey(bkState.Code))
                            {
                                _dicByCode[bkState.Codespace].Add(bkState.Code, bkState);
                            }
                            entityTypeRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateEntityTypeRemovedEvent(entity));
                }
            }

            private class PrivateEntityTypeRemovedEvent : EntityTypeRemovedEvent
            {
                public PrivateEntityTypeRemovedEvent(EntityTypeBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion

        // 内部类
        #region PropertySet
        /// <summary>
        /// 系统字段上下文
        /// </summary>
        private class PropertySet : IEnumerable<PropertyState>
        {
            private readonly Dictionary<EntityTypeState, Dictionary<propertyCode, PropertyState>> _dicByCode = new Dictionary<EntityTypeState, Dictionary<propertyCode, PropertyState>>();
            private readonly Dictionary<propertyID, PropertyState> _dicByID = new Dictionary<propertyID, PropertyState>();
            private bool _initialized = false;
            private readonly Guid _id = Guid.NewGuid();
            private readonly IAppHost host;

            public Guid Id
            {
                get { return _id; }
            }

            #region Ctor
            public PropertySet(IAppHost host)
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
                var handles = new MessageHandler(this);
                messageDispatcher.Register((IHandler<AddPropertyCommand>)handles);
                messageDispatcher.Register((IHandler<PropertyAddedEvent>)handles);
                messageDispatcher.Register((IHandler<UpdatePropertyCommand>)handles);
                messageDispatcher.Register((IHandler<PropertyUpdatedEvent>)handles);
                messageDispatcher.Register((IHandler<RemovePropertyCommand>)handles);
                messageDispatcher.Register((IHandler<PropertyRemovedEvent>)handles);
                messageDispatcher.Register((IHandler<AddCommonPropertiesCommand>)handles);
                messageDispatcher.Register((IHandler<EntityTypeUpdatedEvent>)handles);
                messageDispatcher.Register((IHandler<EntityTypeRemovedEvent>)handles);
            }
            #endregion

            public bool TryGetProperty(EntityTypeState entityType, string propertyCode, out PropertyState property)
            {
                if (!_initialized)
                {
                    Init();
                }
                if (propertyCode == null)
                {
                    throw new CoreException("属性编码为空");
                }
                if (!_dicByCode.ContainsKey(entityType)
                    || !_dicByCode[entityType].ContainsKey(propertyCode))
                {
                    property = PropertyState.CreateNoneProperty(propertyCode);
                    return false;
                }

                return _dicByCode[entityType].TryGetValue(propertyCode, out property);
            }

            public PropertyState this[Guid propertyID]
            {
                get
                {
                    if (!_initialized)
                    {
                        Init();
                    }
                    return !_dicByID.ContainsKey(propertyID) ? PropertyState.CreateNoneProperty(string.Empty) : _dicByID[propertyID];
                }
            }

            public bool TryGetProperty(Guid propertyID, out PropertyState property)
            {
                if (!_initialized)
                {
                    Init();
                }
                return _dicByID.TryGetValue(propertyID, out property);
            }

            public IReadOnlyDictionary<string, PropertyState> GetProperties(EntityTypeState entityType)
            {
                if (!_initialized)
                {
                    Init();
                }
                if (!_dicByCode.ContainsKey(entityType))
                {
                    return new Dictionary<string, PropertyState>(StringComparer.OrdinalIgnoreCase);
                }
                return _dicByCode[entityType];
            }

            internal void Refresh()
            {
                if (_initialized)
                {
                    _initialized = false;
                }
            }

            public IEnumerator<PropertyState> GetEnumerator()
            {
                if (!_initialized)
                {
                    Init();
                }
                foreach (var item in _dicByID.Values)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                if (!_initialized)
                {
                    Init();
                }
                foreach (var item in _dicByID.Values)
                {
                    yield return item;
                }
            }

            #region Init
            private void Init()
            {
                if (!_initialized)
                {
                    lock (this)
                    {
                        if (!_initialized)
                        {
                            _dicByCode.Clear();
                            _dicByID.Clear();
                            var properties = host.GetRequiredService<IAppHostBootstrap>().GetAllProperties().OrderBy(a => a.SortCode);
                            foreach (var property in properties)
                            {
                                if (!(property is PropertyBase))
                                {
                                    throw new CoreException(property.GetType().Name + "必须继承" + typeof(PropertyBase).Name);
                                }
                                EntityTypeState entityType;
                                if (!host.EntityTypeSet.TryGetEntityType(property.EntityTypeID, out entityType))
                                {
                                    throw new CoreException("意外的实体属性类型标识" + property.EntityTypeID);
                                }
                                var propertyState = PropertyState.Create(host, property);
                                if (!_dicByCode.ContainsKey(entityType))
                                {
                                    _dicByCode.Add(entityType, new Dictionary<propertyCode, PropertyState>(StringComparer.OrdinalIgnoreCase));
                                }
                                if (!_dicByCode[entityType].ContainsKey(property.Code))
                                {
                                    _dicByCode[entityType].Add(property.Code, propertyState);
                                }
                                if (_dicByID.ContainsKey(property.Id))
                                {
                                    throw new CoreException("意外的重复实体属性标识" + property.Id);
                                }
                                _dicByID.Add(property.Id, propertyState);
                            }
                            _initialized = true;
                        }
                    }
                }
            }
            #endregion

            #region MessageHandler
            private class MessageHandler:
                IHandler<AddPropertyCommand>,
                IHandler<PropertyAddedEvent>,
                IHandler<AddCommonPropertiesCommand>,
                IHandler<UpdatePropertyCommand>,
                IHandler<PropertyUpdatedEvent>,
                IHandler<RemovePropertyCommand>,
                IHandler<PropertyRemovedEvent>,
                IHandler<EntityTypeUpdatedEvent>,
                IHandler<EntityTypeRemovedEvent>
            {
                private readonly PropertySet set;

                public MessageHandler(PropertySet set)
                {
                    this.set = set;
                }

                public void Handle(EntityTypeUpdatedEvent message)
                {
                    var host = set.host;
                    var _dicByCode = set._dicByCode;
                    var _dicByID = set._dicByID;
                    EntityTypeState newKey;
                    if (!host.EntityTypeSet.TryGetEntityType(message.Source.Id, out newKey))
                    {
                        throw new CoreException("意外的实体类型标识" + message.Source.Id);
                    }
                    if (!_dicByCode.ContainsKey(newKey))
                    {
                        var oldKey = _dicByCode.Keys.FirstOrDefault(a => a.Id == newKey.Id);
                        if (oldKey != null)
                        {
                            _dicByCode.Add(newKey, _dicByCode[oldKey]);
                            _dicByCode.Remove(oldKey);
                        }
                    }
                }

                public void Handle(EntityTypeRemovedEvent message)
                {
                    var host = set.host;
                    var _dicByCode = set._dicByCode;
                    var _dicByID = set._dicByID;
                    var key = _dicByCode.Keys.FirstOrDefault(a => a.Id == message.Source.Id);
                    if (key != null)
                    {
                        _dicByCode.Remove(key);
                    }
                }

                public void Handle(AddPropertyCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(PropertyAddedEvent message)
                {
                    if (message.GetType() == typeof(PrivatePropertyAddedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(IPropertyCreateInput input, bool isCommand)
                {
                    var host = set.host;
                    var _dicByCode = set._dicByCode;
                    var _dicByID = set._dicByID;
                    var propertyRepository = host.GetRequiredService<IRepository<Property>>();
                    if (string.IsNullOrEmpty(input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    EntityTypeState entityType;
                    if (!host.EntityTypeSet.TryGetEntityType(input.EntityTypeID, out entityType))
                    {
                        throw new CoreException("记录已经存在" + input.EntityTypeID);
                    }
                    Property entity;
                    lock (this)
                    {
                        PropertyState property;
                        if (host.EntityTypeSet.TryGetProperty(entityType, input.Code, out property))
                        {
                            throw new ValidationException("编码为" + input.Code + "的属性已经存在");
                        }
                        if (!input.Id.HasValue)
                        {
                            throw new ValidationException("标识是必须的");
                        }
                        if (host.EntityTypeSet.TryGetProperty(input.Id.Value, out property))
                        {
                            throw new CoreException("记录已经存在");
                        }

                        entity = Property.Create(input);

                        var state = PropertyState.Create(host, entity);
                        if (!_dicByCode.ContainsKey(entityType))
                        {
                            _dicByCode.Add(entityType, new Dictionary<propertyCode, PropertyState>(StringComparer.OrdinalIgnoreCase));
                        }
                        if (!_dicByCode[entityType].ContainsKey(entity.Code))
                        {
                            _dicByCode[entityType].Add(entity.Code, state);
                        }
                        if (!_dicByID.ContainsKey(entity.Id))
                        {
                            _dicByID.Add(entity.Id, state);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                propertyRepository.Add(entity);
                                propertyRepository.Context.Commit();
                            }
                            catch
                            {
                                if (_dicByCode.ContainsKey(entityType) && _dicByCode[entityType].ContainsKey(entity.Code))
                                {
                                    _dicByCode[entityType].Remove(entity.Code);
                                }
                                if (_dicByID.ContainsKey(entity.Id))
                                {
                                    _dicByID.Remove(entity.Id);
                                }
                                propertyRepository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivatePropertyAddedEvent(entity, input));
                    }
                }

                private class PrivatePropertyAddedEvent : PropertyAddedEvent
                {
                    public PrivatePropertyAddedEvent(PropertyBase source, IPropertyCreateInput input)
                        : base(source, input)
                    {

                    }
                }

                public void Handle(AddCommonPropertiesCommand message)
                {
                    var host = set.host;
                    var _dicByCode = set._dicByCode;
                    var _dicByID = set._dicByID;
                    EntityTypeState entityType;
                    if (!host.EntityTypeSet.TryGetEntityType(message.EntityTypeID, out entityType))
                    {
                        throw new ValidationException("意外的实体类型标识" + message.EntityTypeID);
                    }
                    PropertyState property;
                    #region createIDProperty
                    if (!host.EntityTypeSet.TryGetProperty(entityType, "Id", out property))
                    {
                        var createIDProperty = new AddPropertyCommand(new PropertyCreateInput
                        {
                            Code = "Id",
                            Description = null,
                            DicID = null,
                            EntityTypeID = message.EntityTypeID,
                            GuideWords = null,
                            Icon = null,
                            Id = Guid.NewGuid(),
                            InputType = null,
                            IsDetailsShow = false,
                            IsDeveloperOnly = true,
                            IsInput = false,
                            IsTotalLine = false,
                            IsViewField = false,
                            MaxLength = null,
                            Name = "标识",
                            Nullable = false,
                            OType = "Guid",
                            SortCode = 0
                        });
                        host.Handle(createIDProperty);
                    }
                    #endregion
                    #region createCreateOnProperty
                    if (!host.EntityTypeSet.TryGetProperty(entityType, "CreateOn", out property))
                    {
                        var createCreateOnProperty = new AddPropertyCommand(new PropertyCreateInput
                        {
                            Code = "CreateOn",
                            Description = null,
                            DicID = null,
                            EntityTypeID = message.EntityTypeID,
                            GuideWords = null,
                            Icon = null,
                            Id = Guid.NewGuid(),
                            InputType = null,
                            IsDetailsShow = true,
                            IsDeveloperOnly = false,
                            IsInput = false,
                            IsTotalLine = false,
                            IsViewField = false,
                            MaxLength = null,
                            Name = "创建时间",
                            Nullable = true,
                            OType = "DateTime",
                            SortCode = 1
                        });
                        host.Handle(createCreateOnProperty);
                    }
                    #endregion
                    #region createCreateByProperty
                    if (!host.EntityTypeSet.TryGetProperty(entityType, "CreateBy", out property))
                    {
                        var createCreateByProperty = new AddPropertyCommand(new PropertyCreateInput
                        {
                            Code = "CreateBy",
                            Description = null,
                            DicID = null,
                            EntityTypeID = message.EntityTypeID,
                            GuideWords = null,
                            Icon = null,
                            Id = Guid.NewGuid(),
                            InputType = null,
                            IsDetailsShow = true,
                            IsDeveloperOnly = false,
                            IsInput = false,
                            IsTotalLine = false,
                            IsViewField = false,
                            MaxLength = null,
                            Name = "创建人",
                            Nullable = true,
                            OType = "String",
                            SortCode = 2
                        });
                        host.Handle(createCreateByProperty);
                    }
                    #endregion
                    #region createCreateUserIDProperty
                    if (!host.EntityTypeSet.TryGetProperty(entityType, "CreateUserID", out property))
                    {
                        var createCreateUserIDProperty = new AddPropertyCommand(new PropertyCreateInput
                        {
                            Code = "CreateUserID",
                            Description = null,
                            DicID = null,
                            EntityTypeID = message.EntityTypeID,
                            GuideWords = null,
                            Icon = null,
                            Id = Guid.NewGuid(),
                            InputType = null,
                            IsDetailsShow = false,
                            IsDeveloperOnly = true,
                            IsInput = false,
                            IsTotalLine = false,
                            IsViewField = false,
                            MaxLength = null,
                            Name = "创建人标识",
                            Nullable = true,
                            OType = "Guid",
                            SortCode = 0
                        });
                        host.Handle(createCreateUserIDProperty);
                    }
                    #endregion
                    #region createModifiedOnProperty
                    if (!host.EntityTypeSet.TryGetProperty(entityType, "ModifiedOn", out property))
                    {
                        var createModifiedOnProperty = new AddPropertyCommand(new PropertyCreateInput
                        {
                            Code = "ModifiedOn",
                            Description = null,
                            DicID = null,
                            EntityTypeID = message.EntityTypeID,
                            GuideWords = null,
                            Icon = null,
                            Id = Guid.NewGuid(),
                            InputType = null,
                            IsDetailsShow = true,
                            IsDeveloperOnly = false,
                            IsInput = false,
                            IsTotalLine = false,
                            IsViewField = false,
                            MaxLength = null,
                            Name = "最后修改时间",
                            Nullable = true,
                            OType = "DateTime",
                            SortCode = 1
                        });
                        host.Handle(createModifiedOnProperty);
                    }
                    #endregion
                    #region createModifiedByProperty
                    if (!host.EntityTypeSet.TryGetProperty(entityType, "ModifiedBy", out property))
                    {
                        var createModifiedByProperty = new AddPropertyCommand(new PropertyCreateInput
                        {
                            Code = "ModifiedBy",
                            Description = null,
                            DicID = null,
                            EntityTypeID = message.EntityTypeID,
                            GuideWords = null,
                            Icon = null,
                            Id = Guid.NewGuid(),
                            InputType = null,
                            IsDetailsShow = true,
                            IsDeveloperOnly = false,
                            IsInput = false,
                            IsTotalLine = false,
                            IsViewField = false,
                            MaxLength = null,
                            Name = "最后修改人",
                            Nullable = true,
                            OType = "String",
                            SortCode = 2
                        });
                        host.Handle(createModifiedByProperty);
                    }
                    #endregion
                    #region createModifiedUserIDProperty
                    if (!host.EntityTypeSet.TryGetProperty(entityType, "ModifiedUserID", out property))
                    {
                        var createModifiedUserIDProperty = new AddPropertyCommand(new PropertyCreateInput
                        {
                            Code = "ModifiedUserID",
                            Description = null,
                            DicID = null,
                            EntityTypeID = message.EntityTypeID,
                            GuideWords = null,
                            Icon = null,
                            Id = Guid.NewGuid(),
                            InputType = null,
                            IsDetailsShow = false,
                            IsDeveloperOnly = true,
                            IsInput = false,
                            IsTotalLine = false,
                            IsViewField = false,
                            MaxLength = null,
                            Name = "最后修改人标识",
                            Nullable = true,
                            OType = "Guid",
                            SortCode = 0
                        });
                        host.Handle(createModifiedUserIDProperty);
                    }
                    #endregion
                }

                private class PropertyCreateInput : EntityCreateInput, IInputModel, IPropertyCreateInput
                {
                    /// <summary>
                    /// 
                    /// </summary>
                    public Guid EntityTypeID { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public Guid? ForeignPropertyID { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string Code { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string Description { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public int? MaxLength { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public bool Nullable { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string OType { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string Name { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string Icon { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string GuideWords { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public int SortCode { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public Guid? DicID { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public bool IsViewField { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public bool IsDetailsShow { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public bool IsDeveloperOnly { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string InputType { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public bool IsInput { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public bool IsTotalLine { get; set; }


                    public propertyCode GroupCode { get; set; }

                    public propertyCode Tooltip { get; set; }
                }

                public void Handle(UpdatePropertyCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(PropertyUpdatedEvent message)
                {
                    if (message.GetType() == typeof(PrivatePropertyUpdatedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(IPropertyUpdateInput input, bool isCommand)
                {
                    var host = set.host;
                    var _dicByCode = set._dicByCode;
                    var _dicByID = set._dicByID;
                    var propertyRepository = host.GetRequiredService<IRepository<Property>>();
                    if (string.IsNullOrEmpty(input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    PropertyState bkState;
                    if (!host.EntityTypeSet.TryGetProperty(input.Id, out bkState))
                    {
                        throw new ValidationException("标识" + input.Id + "的属性不存在");
                    }
                    Property entity;
                    bool stateChanged = false;
                    lock (bkState)
                    {
                        EntityTypeState entityType;
                        PropertyState property;
                        if (!host.EntityTypeSet.TryGetEntityType(bkState.EntityTypeID, out entityType))
                        {
                            throw new ValidationException("意外的实体类型标识" + bkState.EntityTypeID);
                        }
                        if (host.EntityTypeSet.TryGetProperty(entityType, input.Code, out property) && property.Id != input.Id)
                        {
                            throw new ValidationException("编码为" + input.Code + "的属性在" + entityType.Name + "下已经存在");
                        }
                        entity = propertyRepository.GetByKey(input.Id);
                        if (entity == null)
                        {
                            throw new NotExistException();
                        }

                        entity.Update(input);

                        var newState = PropertyState.Create(host, entity);

                        stateChanged = newState != bkState;
                        if (stateChanged)
                        {
                            Update(newState);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                propertyRepository.Update(entity);
                                propertyRepository.Context.Commit();
                            }
                            catch
                            {
                                if (stateChanged)
                                {
                                    Update(bkState);
                                }
                                propertyRepository.Context.Rollback();
                                throw;
                            }
                        }
                        if (!stateChanged)
                        {
                            return;
                        }
                    }
                    if (isCommand && stateChanged)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivatePropertyUpdatedEvent(entity, input));
                    }
                }

                private void Update(PropertyState state)
                {
                    var host = set.host;
                    var _dicByCode = set._dicByCode;
                    var _dicByID = set._dicByID;
                    var oldState = _dicByID[state.Id];
                    EntityTypeState entityType;
                    if (!host.EntityTypeSet.TryGetEntityType(oldState.EntityTypeID, out entityType))
                    {
                        throw new CoreException("意外的实体属性类型标识" + oldState.EntityTypeID);
                    }
                    string oldKey = oldState.Code;
                    string newKey = state.Code;
                    _dicByID[state.Id] = state;
                    if (!_dicByCode[entityType].ContainsKey(newKey))
                    {
                        _dicByCode[entityType].Remove(oldKey);
                        _dicByCode[entityType].Add(newKey, state);
                    }
                    else
                    {
                        _dicByCode[entityType][newKey] = state;
                    }
                }

                private class PrivatePropertyUpdatedEvent : PropertyUpdatedEvent
                {
                    public PrivatePropertyUpdatedEvent(PropertyBase source, IPropertyUpdateInput input)
                        : base(source, input)
                    {

                    }
                }
                public void Handle(RemovePropertyCommand message)
                {
                    this.Handle(message.EntityID, true);
                }

                public void Handle(PropertyRemovedEvent message)
                {
                    if (message.GetType() == typeof(PrivatePropertyRemovedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Source.Id, false);
                }

                private void Handle(Guid propertyID, bool isCommand)
                {
                    var host = set.host;
                    var _dicByCode = set._dicByCode;
                    var _dicByID = set._dicByID;
                    var propertyRepository = host.GetRequiredService<IRepository<Property>>();
                    PropertyState bkState;
                    if (!host.EntityTypeSet.TryGetProperty(propertyID, out bkState))
                    {
                        return;
                    }
                    Property entity;
                    lock (bkState)
                    {
                        entity = propertyRepository.GetByKey(propertyID);
                        if (entity == null)
                        {
                            return;
                        }
                        EntityTypeState entityType;
                        if (!host.EntityTypeSet.TryGetEntityType(bkState.EntityTypeID, out entityType))
                        {
                            throw new CoreException("意外的实体属性类型标识" + bkState.EntityTypeID);
                        }
                        if (_dicByID.ContainsKey(bkState.Id))
                        {
                            if (_dicByCode.ContainsKey(entityType) && _dicByCode[entityType].ContainsKey(bkState.Code))
                            {
                                _dicByCode[entityType].Remove(bkState.Code);
                            }
                            _dicByID.Remove(bkState.Id);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                propertyRepository.Remove(entity);
                                propertyRepository.Context.Commit();
                            }
                            catch
                            {
                                if (!_dicByID.ContainsKey(bkState.Id))
                                {
                                    _dicByID.Add(bkState.Id, bkState);
                                }
                                if (!_dicByCode.ContainsKey(entityType))
                                {
                                    _dicByCode.Add(entityType, new Dictionary<propertyCode, PropertyState>(StringComparer.OrdinalIgnoreCase));
                                }
                                if (!_dicByCode[entityType].ContainsKey(bkState.Code))
                                {
                                    _dicByCode[entityType].Add(bkState.Code, bkState);
                                }
                                propertyRepository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivatePropertyRemovedEvent(entity));
                    }
                }

                private class PrivatePropertyRemovedEvent : PropertyRemovedEvent
                {
                    public PrivatePropertyRemovedEvent(PropertyBase source)
                        : base(source)
                    {

                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
