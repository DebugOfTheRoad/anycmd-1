
namespace Anycmd.Host.AC.MemorySets
{
    using AC;
    using AC.Messages;
    using AC.ValueObjects;
    using Anycmd.AC;
    using Bus;
    using Exceptions;
    using Extensions;
    using Host;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using roleID = System.Guid;

    public sealed class RoleSet : IRoleSet
    {
        private readonly Dictionary<roleID, RoleState> _roleDic = new Dictionary<roleID, RoleState>();
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly AppHost host;

        public Guid Id
        {
            get { return _id; }
        }

        #region Ctor
        public RoleSet(AppHost host)
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
            messageDispatcher.Register((IHandler<AddRoleCommand>)handler);
            messageDispatcher.Register((IHandler<RoleAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateRoleCommand>)handler);
            messageDispatcher.Register((IHandler<RoleUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveRoleCommand>)handler);
            messageDispatcher.Register((IHandler<RoleRemovedEvent>)handler);
        }
        #endregion

        public bool TryGetRole(Guid roleID, out RoleState role)
        {
            if (!_initialized)
            {
                Init();
            }
            return _roleDic.TryGetValue(roleID, out role);
        }

        public IEnumerator<RoleState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _roleDic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _roleDic.Values.GetEnumerator();
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _roleDic.Clear();
                        var roles = host.GetRequiredService<IAppHostBootstrap>().GetAllRoles();
                        foreach (var role in roles)
                        {
                            if (!(role is RoleBase))
                            {
                                throw new CoreException(role.GetType().Name + "必须继承" + typeof(RoleBase).Name);
                            }
                            var roleState = RoleState.Create(role);
                            if (!_roleDic.ContainsKey(role.Id))
                            {
                                _roleDic.Add(role.Id, roleState);
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        #region MessageHandler
        private class MessageHandler :
            IHandler<UpdateRoleCommand>,
            IHandler<RoleUpdatedEvent>,
            IHandler<RoleRemovedEvent>, 
            IHandler<AddRoleCommand>, 
            IHandler<RoleAddedEvent>, 
            IHandler<RemoveRoleCommand>
        {
            private readonly RoleSet set;

            public MessageHandler(RoleSet set)
            {
                this.set = set;
            }

            public void Handle(AddRoleCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(RoleAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateRoleAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IRoleCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _roleDic = set._roleDic;
                var roleRepository = host.GetRequiredService<IRepository<Role>>();
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                Role entity;
                lock (this)
                {
                    RoleState role;
                    if (host.RoleSet.TryGetRole(input.Id.Value, out role))
                    {
                        throw new ValidationException("已经存在");
                    }
                    if (host.RoleSet.Any(a => string.Equals(a.Name, input.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new ValidationException("同名的角色已经存在");
                    }

                    entity = Role.Create(input);

                    if (!_roleDic.ContainsKey(entity.Id))
                    {
                        var state = RoleState.Create(entity);
                        _roleDic.Add(entity.Id, state);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            roleRepository.Add(entity);
                            roleRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_roleDic.ContainsKey(entity.Id))
                            {
                                _roleDic.Remove(entity.Id);
                            }
                            roleRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateRoleAddedEvent(entity, input));
                }
            }

            private class PrivateRoleAddedEvent : RoleAddedEvent
            {
                public PrivateRoleAddedEvent(RoleBase source, IRoleCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateRoleCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(RoleUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateRoleUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IRoleUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _roleDic = set._roleDic;
                var roleRepository = host.GetRequiredService<IRepository<Role>>();
                RoleState bkState;
                if (!host.RoleSet.TryGetRole(input.Id, out bkState))
                {
                    throw new NotExistException();
                }
                Role entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    RoleState oldState;
                    if (!host.RoleSet.TryGetRole(input.Id, out oldState))
                    {
                        throw new NotExistException();
                    }
                    if (host.RoleSet.Any(a => string.Equals(a.Name, input.Name, StringComparison.OrdinalIgnoreCase) && a.Id != input.Id))
                    {
                        throw new ValidationException("角色名称重复");
                    }
                    entity = roleRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = RoleState.Create(entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            roleRepository.Update(entity);
                            roleRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            roleRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateRoleUpdatedEvent(entity, input));
                }
            }

            private void Update(RoleState state)
            {
                var host = set.host;
                var _roleDic = set._roleDic;
                _roleDic[state.Id] = state;
            }

            private class PrivateRoleUpdatedEvent : RoleUpdatedEvent
            {
                public PrivateRoleUpdatedEvent(RoleBase source, IRoleUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveRoleCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(RoleRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateRoleRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid roleID, bool isCommand)
            {
                var host = set.host;
                var _roleDic = set._roleDic;
                var roleRepository = host.GetRequiredService<IRepository<Role>>();
                RoleState bkState;
                if (!host.RoleSet.TryGetRole(roleID, out bkState))
                {
                    return;
                }
                Role entity;
                lock (bkState)
                {
                    RoleState state;
                    if (!host.RoleSet.TryGetRole(roleID, out state))
                    {
                        return;
                    }
                    entity = roleRepository.GetByKey(roleID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (_roleDic.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new RoleRemovingEvent(entity));
                        }
                        _roleDic.Remove(bkState.Id);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            roleRepository.Remove(entity);
                            roleRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_roleDic.ContainsKey(bkState.Id))
                            {
                                _roleDic.Add(bkState.Id, bkState);
                            }
                            roleRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateRoleRemovedEvent(entity));
                }
            }

            private class PrivateRoleRemovedEvent : RoleRemovedEvent
            {
                public PrivateRoleRemovedEvent(RoleBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion
    }
}