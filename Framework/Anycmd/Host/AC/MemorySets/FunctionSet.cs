
namespace Anycmd.Host.AC.MemorySets
{
    using AC.Infra;
    using Anycmd.AC.Infra;
    using Anycmd.Bus;
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
    using functionCode = System.String;

    public sealed class FunctionSet : IFunctionSet
    {
        private readonly Dictionary<ResourceTypeState, Dictionary<functionCode, FunctionState>>
            _dicByCode = new Dictionary<ResourceTypeState, Dictionary<functionCode, FunctionState>>();
        private readonly Dictionary<Guid, FunctionState> _dicByID = new Dictionary<Guid, FunctionState>();
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly AppHost host;

        public Guid Id
        {
            get { return _id; }
        }

        public FunctionSet(AppHost host)
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
            messageDispatcher.Register((IHandler<AddFunctionCommand>)handler);
            messageDispatcher.Register((IHandler<FunctionAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateFunctionCommand>)handler);
            messageDispatcher.Register((IHandler<FunctionUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveFunctionCommand>)handler);
            messageDispatcher.Register((IHandler<FunctionRemovedEvent>)handler);
            messageDispatcher.Register((IHandler<ResourceTypeUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<ResourceTypeRemovedEvent>)handler);
        }

        public bool TryGetFunction(ResourceTypeState resource, string functionCode, out FunctionState function)
        {
            if (!_initialized)
            {
                Init();
            }
            if (!_dicByCode.ContainsKey(resource))
            {
                function = FunctionState.Empty;
                return false;
            }
            if (functionCode == null)
            {
                function = FunctionState.Empty;
                return false;
            }
            return _dicByCode[resource].TryGetValue(functionCode, out function);
        }

        public bool TryGetFunction(Guid functionID, out FunctionState function)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.TryGetValue(functionID, out function);
        }

        internal void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
        }

        public IEnumerator<FunctionState> GetEnumerator()
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
                        _dicByCode.Clear();
                        _dicByID.Clear();
                        var functions = host.GetRequiredService<IAppHostBootstrap>().GetAllFunctions();
                        foreach (var entity in functions)
                        {
                            if (!(entity is FunctionBase))
                            {
                                throw new CoreException(entity.GetType().Name + "必须继承" + typeof(FunctionBase).Name);
                            }
                            var function = FunctionState.Create(host, entity);
                            _dicByID.Add(function.Id, function);
                            if (!_dicByCode.ContainsKey(function.Resource))
                            {
                                _dicByCode.Add(function.Resource, new Dictionary<functionCode, FunctionState>(StringComparer.OrdinalIgnoreCase));
                            }
                            if (!_dicByCode[function.Resource].ContainsKey(function.Code))
                            {
                                _dicByCode[function.Resource].Add(function.Code, function);
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }
        #endregion

        #region MessageHandler
        private class MessageHandler :
            IHandler<FunctionAddedEvent>,
            IHandler<FunctionRemovedEvent>,
            IHandler<AddFunctionCommand>, 
            IHandler<UpdateFunctionCommand>, 
            IHandler<FunctionUpdatedEvent>, 
            IHandler<RemoveFunctionCommand>, 
            IHandler<ResourceTypeUpdatedEvent>, 
            IHandler<ResourceTypeRemovedEvent>
        {
            private readonly FunctionSet set;

            public MessageHandler(FunctionSet set)
            {
                this.set = set;
            }

            public void Handle(ResourceTypeUpdatedEvent message)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                ResourceTypeState newKey;
                if (!host.ResourceSet.TryGetResource(message.Source.Id, out newKey))
                {
                    throw new CoreException("意外的资源标识" + message.Source.Id);
                }
                var oldKey = _dicByCode.Keys.FirstOrDefault(a => a.Id == message.Source.Id);
                if (oldKey != null && !_dicByCode.ContainsKey(newKey))
                {
                    _dicByCode.Add(newKey, _dicByCode[oldKey]);
                    _dicByCode.Remove(oldKey);
                }
            }

            public void Handle(ResourceTypeRemovedEvent message)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var key = _dicByCode.Keys.FirstOrDefault(a => a.Id == message.Source.Id);
                if (key != null)
                {
                    _dicByCode.Remove(key);
                }
            }

            public void Handle(AddFunctionCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(FunctionAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateFunctionAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IFunctionCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var functionRepository = host.GetRequiredService<IRepository<Function>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                ResourceTypeState resource;
                if (!host.ResourceSet.TryGetResource(input.ResourceTypeID, out resource))
                {
                    throw new ValidationException("意外的功能资源标识" + input.ResourceTypeID);
                }

                var entity = Function.Create(input);

                lock (this)
                {
                    FunctionState functionState;
                    if (host.FunctionSet.TryGetFunction(input.Id.Value, out functionState))
                    {
                        throw new CoreException("记录已经存在");
                    }
                    var state = FunctionState.Create(host, entity);
                    if (host.FunctionSet.TryGetFunction(resource, input.Code, out functionState))
                    {
                        throw new ValidationException("重复的编码");
                    }
                    if (!_dicByID.ContainsKey(entity.Id))
                    {
                        _dicByID.Add(entity.Id, state);
                    }
                    if (!_dicByCode.ContainsKey(resource))
                    {
                        _dicByCode.Add(resource, new Dictionary<functionCode, FunctionState>(StringComparer.OrdinalIgnoreCase));
                    }
                    if (!_dicByCode[resource].ContainsKey(entity.Code))
                    {
                        _dicByCode[resource].Add(state.Code, state);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            functionRepository.Add(entity);
                            functionRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_dicByID.ContainsKey(entity.Id))
                            {
                                _dicByID.Remove(entity.Id);
                            }
                            if (_dicByCode.ContainsKey(resource) && _dicByCode[resource].ContainsKey(entity.Code))
                            {
                                _dicByCode[resource].Remove(entity.Code);
                            }
                            functionRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateFunctionAddedEvent(entity, input));
                }
            }

            private class PrivateFunctionAddedEvent : FunctionAddedEvent
            {
                public PrivateFunctionAddedEvent(FunctionBase source, IFunctionCreateInput input) : base(source, input) { }
            }

            public void Handle(UpdateFunctionCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(FunctionUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateFunctionUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IFunctionUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var functionRepository = host.GetRequiredService<IRepository<Function>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                FunctionState bkState;
                if (!host.FunctionSet.TryGetFunction(input.Id, out bkState))
                {
                    throw new NotExistException();
                }
                FunctionState functionState;
                ResourceTypeState resource;
                if (!host.ResourceSet.TryGetResource(bkState.ResourceTypeID, out resource))
                {
                    throw new ValidationException("意外的功能资源标识" + bkState.ResourceTypeID);
                }
                Function entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    FunctionState oldState;
                    if (!host.FunctionSet.TryGetFunction(input.Id, out oldState))
                    {
                        throw new NotExistException();
                    }
                    if (host.FunctionSet.TryGetFunction(resource, input.Code, out functionState) && functionState.Id != input.Id)
                    {
                        throw new ValidationException("重复的编码");
                    }
                    entity = functionRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException("更新的实体不存在");
                    }

                    entity.Update(input);

                    var newState = FunctionState.Create(host, entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            functionRepository.Update(entity);
                            functionRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            functionRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateFunctionUpdatedEvent(entity, input));
                }
            }

            private void Update(FunctionState state)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var oldState = _dicByID[state.Id];
                string oldKey = oldState.Code;
                string newKey = state.Code;
                _dicByID[state.Id] = state;
                ResourceTypeState resource;
                if (!host.ResourceSet.TryGetResource(oldState.ResourceTypeID, out resource))
                {
                    throw new ValidationException("意外的功能资源标识" + oldState.ResourceTypeID);
                }
                if (!_dicByCode[resource].ContainsKey(newKey))
                {
                    _dicByCode[resource].Remove(oldKey);
                    _dicByCode[resource].Add(newKey, state);
                }
                else
                {
                    _dicByCode[resource][newKey] = state;
                }
            }

            private class PrivateFunctionUpdatedEvent : FunctionUpdatedEvent
            {
                public PrivateFunctionUpdatedEvent(FunctionBase source, IFunctionUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveFunctionCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(FunctionRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateFunctionRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid functionID, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var functionRepository = host.GetRequiredService<IRepository<Function>>();
                var operationHelpRepository = host.GetRequiredService<IRepository<OperationHelp>>();

                FunctionState bkState;
                if (!host.FunctionSet.TryGetFunction(functionID, out bkState))
                {
                    return;
                }
                Function entity;
                lock (bkState)
                {
                    FunctionState state;
                    if (!host.FunctionSet.TryGetFunction(functionID, out state))
                    {
                        return;
                    }
                    entity = functionRepository.GetByKey(functionID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (_dicByID.ContainsKey(functionID))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new FunctionRemovingEvent(entity));
                        }
                        if (_dicByCode.ContainsKey(bkState.Resource)
                            && _dicByCode[bkState.Resource].ContainsKey(bkState.Code))
                        {
                            _dicByCode[bkState.Resource].Remove(bkState.Code);
                        }
                        _dicByID.Remove(functionID);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            // 删除操作帮助
                            var operationLog = operationHelpRepository.GetByKey(functionID);
                            if (operationLog != null)
                            {
                                operationHelpRepository.Remove(operationLog);
                                operationHelpRepository.Context.Commit();
                            }
                            functionRepository.Remove(entity);
                            functionRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_dicByID.ContainsKey(functionID))
                            {
                                if (!_dicByCode.ContainsKey(bkState.Resource))
                                {
                                    _dicByCode.Add(bkState.Resource, new Dictionary<functionCode, FunctionState>(StringComparer.OrdinalIgnoreCase));
                                }
                                if (!_dicByCode[bkState.Resource].ContainsKey(bkState.Code))
                                {
                                    _dicByCode[bkState.Resource].Add(bkState.Code, bkState);
                                }
                                _dicByID.Add(bkState.Id, bkState);
                            }
                            operationHelpRepository.Context.Rollback();
                            functionRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateFunctionRemovedEvent(entity));
                }
            }

            private class PrivateFunctionRemovedEvent : FunctionRemovedEvent
            {
                public PrivateFunctionRemovedEvent(FunctionBase function)
                    : base(function)
                {

                }
            }
        }
        #endregion
    }
}
