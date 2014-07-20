
namespace Anycmd.Host.AC.MemorySets.Impl
{
    using AC.Infra;
    using Anycmd.AC.Infra;
    using Bus;
    using Exceptions;
    using Extensions;
    using Host;
    using Infra.Messages;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ValueObjects;

    /// <summary>
    /// 资源上下文
    /// </summary>
    public sealed class ResourceTypeSet : IResourceTypeSet
    {
        public static readonly IResourceTypeSet Empty = new ResourceTypeSet(AppHost.Empty);

        private readonly Dictionary<AppSystemState, Dictionary<string, ResourceTypeState>> _dicByCode = new Dictionary<AppSystemState,Dictionary<string,ResourceTypeState>>();
        private readonly Dictionary<Guid, ResourceTypeState> _dicByID = new Dictionary<Guid, ResourceTypeState>();
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly IAppHost host;

        public Guid Id
        {
            get { return _id; }
        }

        public ResourceTypeSet(IAppHost host)
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
            messageDispatcher.Register((IHandler<AddResourceCommand>)handler);
            messageDispatcher.Register((IHandler<ResourceTypeAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateResourceCommand>)handler);
            messageDispatcher.Register((IHandler<ResourceTypeUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveResourceTypeCommand>)handler);
            messageDispatcher.Register((IHandler<ResourceTypeRemovedEvent>)handler);
        }

        public bool TryGetResource(AppSystemState appSystem, string resourceTypeCode, out ResourceTypeState resource)
        {
            if (!_initialized)
            {
                Init();
            }
            if (appSystem == null)
            {
                throw new ArgumentNullException("appSystem");
            }
            if (resourceTypeCode == null)
            {
                throw new ArgumentNullException("resourceTypeCode");
            }
            if (!_dicByCode.ContainsKey(appSystem))
            {
                resource = ResourceTypeState.Empty;
                return false;
            }

            return _dicByCode[appSystem].TryGetValue(resourceTypeCode, out resource);
        }

        public bool TryGetResource(Guid resourceTypeID, out ResourceTypeState resource)
        {
            if (!_initialized)
            {
                Init();
            }

            return _dicByID.TryGetValue(resourceTypeID, out resource);
        }

        internal void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
        }

        public IEnumerator<ResourceTypeState> GetEnumerator()
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
                        var allResources = host.GetRequiredService<IAppHostBootstrap>().GetAllResources();
                        foreach (var resource in allResources)
                        {
                            if (!(resource is ResourceTypeBase))
                            {
                                throw new CoreException(resource.GetType().Name + "必须继承" + typeof(ResourceTypeBase).Name);
                            }
                            AppSystemState appSystem;
                            if (!host.AppSystemSet.TryGetAppSystem(resource.AppSystemID, out appSystem))
                            {
                                throw new CoreException("意外的资源类型应用系统标识" + resource.AppSystemID);
                            }
                            if (!_dicByCode.ContainsKey(appSystem))
                            {
                                _dicByCode.Add(appSystem, new Dictionary<string, ResourceTypeState>(StringComparer.OrdinalIgnoreCase));
                            }
                            if (_dicByCode[appSystem].ContainsKey(resource.Code))
                            {
                                throw new CoreException("意外重复的资源标识" + resource.Id);
                            }
                            if (_dicByID.ContainsKey(resource.Id))
                            {
                                throw new CoreException("意外重复的资源标识" + resource.Id);
                            }
                            var resourceState = ResourceTypeState.Create(resource);
                            _dicByCode[appSystem].Add(resource.Code, resourceState);
                            _dicByID.Add(resource.Id, resourceState);
                        }
                        _initialized = true;
                    }
                }
            }
        }

        #region MessageHandler
        private class MessageHandler:
            IHandler<AddResourceCommand>,
            IHandler<ResourceTypeAddedEvent>,
            IHandler<UpdateResourceCommand>,
            IHandler<ResourceTypeUpdatedEvent>,
            IHandler<RemoveResourceTypeCommand>,
            IHandler<ResourceTypeRemovedEvent>
        {
            private readonly ResourceTypeSet set;

            public MessageHandler(ResourceTypeSet set)
            {
                this.set = set;
            }

            public void Handle(AddResourceCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(ResourceTypeAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateResourceAddedEvent))
                {
                    return;
                }
                this.Handle(message.input, false);
            }

            private void Handle(IResourceCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var resourceRepository = host.GetRequiredService<IRepository<ResourceType>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                ResourceType entity;
                lock (this)
                {
                    ResourceTypeState resource;
                    if (host.ResourceTypeSet.TryGetResource(input.Id.Value, out resource))
                    {
                        throw new ValidationException("相同标识的资源已经存在" + input.Id.Value);
                    }
                    AppSystemState appSystem;
                    if (!host.AppSystemSet.TryGetAppSystem(input.AppSystemID, out appSystem))
                    {
                        throw new ValidationException("意外的应用系统标识" + input.AppSystemID);
                    }
                    if (host.ResourceTypeSet.TryGetResource(appSystem, input.Code, out resource))
                    {
                        throw new ValidationException("重复的资源编码" + input.Code);
                    }

                    entity = ResourceType.Create(input);

                    var state = ResourceTypeState.Create(entity);
                    if (!_dicByCode.ContainsKey(appSystem))
                    {
                        _dicByCode.Add(appSystem, new Dictionary<string, ResourceTypeState>(StringComparer.OrdinalIgnoreCase));
                    }
                    if (!_dicByCode[appSystem].ContainsKey(entity.Code))
                    {
                        _dicByCode[appSystem].Add(entity.Code, state);
                    }
                    if (!_dicByID.ContainsKey(entity.Id))
                    {
                        _dicByID.Add(entity.Id, state);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            resourceRepository.Add(entity);
                            resourceRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_dicByCode[appSystem].ContainsKey(entity.Code))
                            {
                                _dicByCode[appSystem].Remove(entity.Code);
                            }
                            if (_dicByID.ContainsKey(entity.Id))
                            {
                                _dicByID.Remove(entity.Id);
                            }
                            resourceRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateResourceAddedEvent(entity, input));
                }
            }

            private class PrivateResourceAddedEvent : ResourceTypeAddedEvent
            {
                public PrivateResourceAddedEvent(ResourceTypeBase source, IResourceCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateResourceCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(ResourceTypeUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateResourceUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IResourceUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var resourceRepository = host.GetRequiredService<IRepository<ResourceType>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                ResourceTypeState bkState;
                if (!host.ResourceTypeSet.TryGetResource(input.Id, out bkState))
                {
                    throw new NotExistException();
                }
                AppSystemState appSystem;
                if (!host.AppSystemSet.TryGetAppSystem(bkState.AppSystemID, out appSystem))
                {
                    throw new ValidationException("意外的应用系统标识" + bkState.AppSystemID);
                }
                ResourceType entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    ResourceTypeState oldState;
                    if (!host.ResourceTypeSet.TryGetResource(input.Id, out oldState))
                    {
                        throw new NotExistException();
                    }
                    ResourceTypeState resource;
                    if (host.ResourceTypeSet.TryGetResource(appSystem, input.Code, out resource) && resource.Id != input.Id)
                    {
                        throw new ValidationException("重复的资源编码" + input.Code);
                    }
                    entity = resourceRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = ResourceTypeState.Create(entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            resourceRepository.Update(entity);
                            resourceRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            resourceRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateResourceUpdatedEvent(entity, input));
                }
            }

            private void Update(ResourceTypeState state)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                AppSystemState appSystem;
                if (!host.AppSystemSet.TryGetAppSystem(state.AppSystemID, out appSystem))
                {
                    throw new ValidationException("意外的应用系统标识" + state.AppSystemID);
                }
                if (!_dicByCode.ContainsKey(appSystem))
                {
                    _dicByCode.Add(appSystem, new Dictionary<string, ResourceTypeState>(StringComparer.OrdinalIgnoreCase));
                }
                var oldResource = _dicByID[state.Id];
                string oldKey = oldResource.Code;
                string newKey = state.Code;
                _dicByID[state.Id] = state;
                if (!_dicByCode[appSystem].ContainsKey(newKey))
                {
                    _dicByCode[appSystem].Remove(oldKey);
                    _dicByCode[appSystem].Add(newKey, state);
                }
                else
                {
                    _dicByCode[appSystem][newKey] = state;
                }
            }

            private class PrivateResourceUpdatedEvent : ResourceTypeUpdatedEvent
            {
                public PrivateResourceUpdatedEvent(ResourceTypeBase source, IResourceUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveResourceTypeCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(ResourceTypeRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateResourceRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid resourceTypeID, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var resourceRepository = host.GetRequiredService<IRepository<ResourceType>>();
                ResourceTypeState bkState;
                if (!host.ResourceTypeSet.TryGetResource(resourceTypeID, out bkState))
                {
                    return;
                }
                ResourceType entity;
                lock (bkState)
                {
                    ResourceTypeState state;
                    if (!host.ResourceTypeSet.TryGetResource(resourceTypeID, out state))
                    {
                        return;
                    }
                    if (host.FunctionSet.Any(a => a.ResourceTypeID == resourceTypeID))
                    {
                        throw new ValidationException("资源下定义有功能时不能删除");
                    }
                    entity = resourceRepository.GetByKey(resourceTypeID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (_dicByID.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new ResourceTypeRemovingEvent(entity));
                        }
                        _dicByID.Remove(bkState.Id);
                    }
                    AppSystemState appSystem;
                    if (!host.AppSystemSet.TryGetAppSystem(state.AppSystemID, out appSystem))
                    {
                        throw new ValidationException("意外的应用系统标识" + state.AppSystemID);
                    }
                    if (_dicByCode[appSystem].ContainsKey(bkState.Code))
                    {
                        _dicByCode[appSystem].Remove(bkState.Code);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            resourceRepository.Remove(entity);
                            resourceRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_dicByID.ContainsKey(bkState.Id))
                            {
                                _dicByID.Add(bkState.Id, bkState);
                            }
                            if (!_dicByCode[appSystem].ContainsKey(bkState.Code))
                            {
                                _dicByCode[appSystem].Add(bkState.Code, bkState);
                            }
                            resourceRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateResourceRemovedEvent(entity));
                }
            }

            private class PrivateResourceRemovedEvent : ResourceTypeRemovedEvent
            {
                public PrivateResourceRemovedEvent(ResourceTypeBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion
    }
}