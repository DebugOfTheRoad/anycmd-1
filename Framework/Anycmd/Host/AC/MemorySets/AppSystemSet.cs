
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

    public sealed class AppSystemSet : IAppSystemSet
    {
        private readonly Dictionary<string, AppSystemState> _dicByCode = new Dictionary<string, AppSystemState>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, AppSystemState> _dicByID = new Dictionary<Guid, AppSystemState>();
        private bool _initialized = false;
        private readonly Guid _id = Guid.NewGuid();
        private readonly AppHost host;

        public Guid Id
        {
            get { return _id; }
        }

        #region Ctor
        public AppSystemSet(AppHost host)
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
            messageDispatcher.Register((IHandler<AddAppSystemCommand>)handler);
            messageDispatcher.Register((IHandler<AppSystemAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateAppSystemCommand>)handler);
            messageDispatcher.Register((IHandler<AppSystemUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveAppSystemCommand>)handler);
            messageDispatcher.Register((IHandler<AppSystemRemovedEvent>)handler);
        }
        #endregion

        public AppSystemState SelfAppSystem
        {
            get
            {
                if (!_initialized)
                {
                    Init();
                }
                if (_dicByCode.ContainsKey(host.Config.SelfAppSystemCode))
                {
                    return _dicByCode[host.Config.SelfAppSystemCode];
                }
                else
                {
                    throw new CoreException("尚未配置SelfAppSystemCode");
                }
            }
        }

        public bool TryGetAppSystem(string appSystemCode, out AppSystemState appSystem)
        {
            if (!_initialized)
            {
                Init();
            }
            if (appSystemCode == null)
            {
                appSystem = AppSystemState.Empty;
                return false;
            }
            return _dicByCode.TryGetValue(appSystemCode, out appSystem);
        }

        public bool TryGetAppSystem(Guid appSystemID, out AppSystemState appSystem)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.TryGetValue(appSystemID, out appSystem);
        }

        public bool ContainsAppSystem(Guid appSystemID)
        {
            if (!_initialized)
            {
                Init();
            }

            return _dicByID.ContainsKey(appSystemID);
        }

        public bool ContainsAppSystem(string appSystemCode)
        {
            if (!_initialized)
            {
                Init();
            }
            if (appSystemCode == null)
            {
                throw new ArgumentNullException("appSystemCode");
            }

            return _dicByCode.ContainsKey(appSystemCode);
        }

        internal void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
        }

        public IEnumerator<AppSystemState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByCode.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByCode.Values.GetEnumerator();
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
                        var appSystems = host.GetRequiredService<IAppHostBootstrap>().GetAllAppSystems();
                        foreach (var appSystem in appSystems)
                        {
                            if (!(appSystem is AppSystemBase))
                            {
                                throw new CoreException(appSystem.GetType().Name + "必须继承" + typeof(AppSystemBase).Name);
                            }
                            if (_dicByCode.ContainsKey(appSystem.Code))
                            {
                                throw new CoreException("意外重复的应用系统编码" + appSystem.Code);
                            }
                            if (_dicByID.ContainsKey(appSystem.Id))
                            {
                                throw new CoreException("意外重复的应用系统标识" + appSystem.Id);
                            }
                            var value = AppSystemState.Create(appSystem);
                            _dicByCode.Add(appSystem.Code, value);
                            _dicByID.Add(appSystem.Id, value);
                        }
                        _initialized = true;
                    }
                }
            }
        }
        #endregion

        #region MessageHandler
        private class MessageHandler :
            IHandler<AppSystemUpdatedEvent>,
            IHandler<AppSystemRemovedEvent>, 
            IHandler<AddAppSystemCommand>, 
            IHandler<AppSystemAddedEvent>, 
            IHandler<UpdateAppSystemCommand>, 
            IHandler<RemoveAppSystemCommand>
        {
            private readonly AppSystemSet set;

            public MessageHandler(AppSystemSet set)
            {
                this.set = set;
            }

            public void Handle(AddAppSystemCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(AppSystemAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateAppSystemAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IAppSystemCreateInput input, bool isCommand)
            {
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var host = set.host;
                var repository = host.GetRequiredService<IRepository<AppSystem>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                AppSystem entity;
                lock (this)
                {
                    if (host.AppSystemSet.ContainsAppSystem(input.Code))
                    {
                        throw new ValidationException("重复的应用系统编码" + input.Code);
                    }
                    if (!input.Id.HasValue || host.AppSystemSet.ContainsAppSystem(input.Id.Value))
                    {
                        throw new CoreException("意外的应用系统标识");
                    }

                    entity = AppSystem.Create(input);

                    var state = AppSystemState.Create(entity);
                    if (!_dicByCode.ContainsKey(state.Code))
                    {
                        _dicByCode.Add(state.Code, state);
                    }
                    if (!_dicByID.ContainsKey(state.Id))
                    {
                        _dicByID.Add(state.Id, state);
                    }
                    // 如果是命令则持久化
                    if (isCommand)
                    {
                        try
                        {
                            repository.Add(entity);
                            repository.Context.Commit();
                        }
                        catch
                        {
                            if (_dicByCode.ContainsKey(entity.Code))
                            {
                                _dicByCode.Remove(entity.Code);
                            }
                            if (_dicByID.ContainsKey(entity.Id))
                            {
                                _dicByID.Remove(entity.Id);
                            }
                            repository.Context.Rollback();
                            throw;
                        }
                    }
                }
                // 如果是命令则分发事件
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateAppSystemAddedEvent(entity, input));
                }
            }

            private class PrivateAppSystemAddedEvent : AppSystemAddedEvent
            {
                public PrivateAppSystemAddedEvent(AppSystemBase source, IAppSystemCreateInput input)
                    : base(source, input)
                {
                }
            }
            public void Handle(UpdateAppSystemCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(AppSystemUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateAppSystemUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IAppSystemUpdateInput input, bool isCommand)
            {
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var host = set.host;
                var repository = host.GetRequiredService<IRepository<AppSystem>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                AppSystemState bkState;
                if (!host.AppSystemSet.TryGetAppSystem(input.Id, out bkState))
                {
                    throw new NotExistException("意外的应用系统标识" + input.Id);
                }
                AppSystem entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    AppSystemState oldState;
                    if (!host.AppSystemSet.TryGetAppSystem(input.Id, out oldState))
                    {
                        throw new NotExistException("意外的应用系统标识" + input.Id);
                    }
                    AppSystemState outAppSystem;
                    if (host.AppSystemSet.TryGetAppSystem(input.Code, out outAppSystem) && outAppSystem.Id != input.Id)
                    {
                        throw new ValidationException("重复的应用系统编码" + input.Code);
                    }
                    entity = repository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = AppSystemState.Create(entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            repository.Update(entity);
                            repository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            repository.Context.Rollback();
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
                    host.MessageDispatcher.DispatchMessage(new PrivateAppSystemUpdatedEvent(entity, input));
                }
            }

            private void Update(AppSystemState state)
            {
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var host = set.host;
                var oldState = _dicByID[state.Id];
                string oldKey = oldState.Code;
                string newKey = state.Code;
                _dicByID[state.Id] = state;
                // 如果应用系统编码改变了
                if (!_dicByCode.ContainsKey(newKey))
                {
                    _dicByCode.Remove(oldKey);
                    _dicByCode.Add(newKey, _dicByID[state.Id]);
                }
                else
                {
                    _dicByCode[oldKey] = state;
                }
            }

            private class PrivateAppSystemUpdatedEvent : AppSystemUpdatedEvent
            {
                public PrivateAppSystemUpdatedEvent(AppSystemBase source, IAppSystemUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveAppSystemCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(AppSystemRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateAppSystemRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid appSystemID, bool isCommand)
            {
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var host = set.host;
                var repository = host.GetRequiredService<IRepository<AppSystem>>();
                AppSystemState bkState;
                if (!host.AppSystemSet.TryGetAppSystem(appSystemID, out bkState))
                {
                    return;
                }
                if (host.ResourceSet.Any(a => a.AppSystemID == appSystemID))
                {
                    throw new ValidationException("应用系统下有资源类型时不能删除应用系统。");
                }
                if (host.MenuSet.Any(a => a.AppSystemID == appSystemID))
                {
                    throw new ValidationException("应用系统下有菜单时不能删除应用系统");
                }
                AppSystem entity;
                lock (bkState)
                {
                    AppSystemState state;
                    if (!host.AppSystemSet.TryGetAppSystem(appSystemID, out state))
                    {
                        return;
                    }
                    entity = repository.GetByKey(appSystemID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (_dicByID.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new AppSystemRemovingEvent(entity));
                        }
                        if (_dicByCode.ContainsKey(bkState.Code))
                        {
                            _dicByCode.Remove(bkState.Code);
                        }
                        _dicByID.Remove(bkState.Id);
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
                            if (!_dicByID.ContainsKey(bkState.Id))
                            {
                                _dicByID.Add(bkState.Id, bkState);
                            }
                            if (!_dicByCode.ContainsKey(bkState.Code))
                            {
                                _dicByCode.Add(bkState.Code, bkState);
                            }
                            repository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateAppSystemRemovedEvent(entity));
                }
            }

            private class PrivateAppSystemRemovedEvent : AppSystemRemovedEvent
            {
                public PrivateAppSystemRemovedEvent(AppSystemBase source) : base(source) { }
            }
        }
        #endregion
    }
}
