
namespace Anycmd.Host.AC.MemorySets
{
    using AC;
    using Anycmd.AC;
    using Anycmd.Bus;
    using Exceptions;
    using Extensions;
    using Host;
    using Messages;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ValueObjects;

    public sealed class GroupSet : IGroupSet
    {
        private readonly Dictionary<Guid, GroupState> _groupDic = new Dictionary<Guid, GroupState>();
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly AppHost host;
        public Guid Id
        {
            get { return _id; }
        }

        public GroupSet(AppHost host)
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
            var handler = new MessageHandle(this);
            messageDispatcher.Register((IHandler<AddGroupCommand>)handler);
            messageDispatcher.Register((IHandler<GroupAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateGroupCommand>)handler);
            messageDispatcher.Register((IHandler<GroupUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveGroupCommand>)handler);
            messageDispatcher.Register((IHandler<GroupRemovedEvent>)handler);
        }

        public bool TryGetGroup(Guid groupID, out GroupState group)
        {
            if (!_initialized)
            {
                Init();
            }
            return _groupDic.TryGetValue(groupID, out group);
        }

        public IEnumerator<GroupState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _groupDic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _groupDic.Values.GetEnumerator();
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _groupDic.Clear();
                        var groups = host.GetRequiredService<IAppHostBootstrap>().GetAllGroups();
                        foreach (var group in groups)
                        {
                            if (!(group is GroupBase))
                            {
                                throw new CoreException(group.GetType().Name + "必须继承" + typeof(GroupBase).Name);
                            }
                            if (!_groupDic.ContainsKey(group.Id))
                            {
                                _groupDic.Add(group.Id, GroupState.Create(group));
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        #region MessageHandler
        private class MessageHandle : 
            IHandler<AddGroupCommand>,
            IHandler<GroupAddedEvent>,
            IHandler<GroupUpdatedEvent>, 
            IHandler<UpdateGroupCommand>, 
            IHandler<RemoveGroupCommand>, 
            IHandler<GroupRemovedEvent>
        {
            private readonly GroupSet set;

            public MessageHandle(GroupSet set)
            {
                this.set = set;
            }

            public void Handle(AddGroupCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(GroupAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateGroupAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IGroupCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _groupDic = set._groupDic;
                var groupRepository = host.GetRequiredService<IRepository<Group>>();
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                if (host.GroupSet.Any(a => a.Name.Equals(input.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ValidationException("重复的工作组名");
                }

                var entity = Group.Create(input);

                lock (this)
                {
                    GroupState group;
                    if (host.GroupSet.TryGetGroup(entity.Id, out group))
                    {
                        throw new CoreException("意外的重复标识");
                    }
                    if (!_groupDic.ContainsKey(entity.Id))
                    {
                        _groupDic.Add(entity.Id, GroupState.Create(entity));
                    }
                    if (isCommand)
                    {
                        try
                        {
                            groupRepository.Add(entity);
                            groupRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_groupDic.ContainsKey(entity.Id))
                            {
                                _groupDic.Remove(entity.Id);
                            }
                            groupRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateGroupAddedEvent(entity, input));
                }
            }

            private class PrivateGroupAddedEvent : GroupAddedEvent
            {
                public PrivateGroupAddedEvent(GroupBase source, IGroupCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateGroupCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(GroupUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateGroupUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IGroupUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _groupDic = set._groupDic;
                var groupRepository = host.GetRequiredService<IRepository<Group>>();
                GroupState bkState;
                if (!host.GroupSet.TryGetGroup(input.Id, out bkState))
                {
                    throw new NotExistException();
                }
                Group entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    GroupState oldState;
                    if (!host.GroupSet.TryGetGroup(input.Id, out oldState))
                    {
                        throw new NotExistException();
                    }
                    if (host.GroupSet.Any(a => a.Name.Equals(input.Name, StringComparison.OrdinalIgnoreCase) && a.Id != input.Id))
                    {
                        throw new ValidationException("重复的工作组名");
                    }
                    entity = groupRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = GroupState.Create(entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            groupRepository.Update(entity);
                            groupRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            groupRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateGroupUpdatedEvent(entity, input));
                }
            }

            private void Update(GroupState state)
            {
                var host = set.host;
                var _groupDic = set._groupDic;
                _groupDic[state.Id] = state;
            }

            private class PrivateGroupUpdatedEvent : GroupUpdatedEvent
            {
                public PrivateGroupUpdatedEvent(GroupBase source, IGroupUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveGroupCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(GroupRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateGroupRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid groupID, bool isCommand)
            {
                var host = set.host;
                var _groupDic = set._groupDic;
                var groupRepository = host.GetRequiredService<IRepository<Group>>();
                GroupState bkState;
                if (!host.GroupSet.TryGetGroup(groupID, out bkState))
                {
                    return;
                }
                Group entity;
                lock (bkState)
                {
                    GroupState state;
                    if (!host.GroupSet.TryGetGroup(groupID, out state))
                    {
                        return;
                    }
                    entity = groupRepository.GetByKey(groupID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (_groupDic.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new GroupRemovingEvent(entity));
                        }
                        _groupDic.Remove(bkState.Id);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            groupRepository.Remove(entity);
                            groupRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_groupDic.ContainsKey(entity.Id))
                            {
                                _groupDic.Add(bkState.Id, bkState);
                            }
                            groupRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateGroupRemovedEvent(entity));
                }
            }

            private class PrivateGroupRemovedEvent : GroupRemovedEvent
            {
                public PrivateGroupRemovedEvent(GroupBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion
    }
}
