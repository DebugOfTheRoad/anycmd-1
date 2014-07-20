
namespace Anycmd.Host.AC.MemorySets.Impl
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

    public sealed class OrganizationSet : IOrganizationSet
    {
        public static readonly IOrganizationSet Empty = new OrganizationSet(AppHost.Empty);

        private readonly Dictionary<string, OrganizationState> _dicByCode = new Dictionary<string, OrganizationState>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, OrganizationState> _dicByID = new Dictionary<Guid, OrganizationState>();
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly IAppHost host;

        public Guid Id
        {
            get { return _id; }
        }

        public OrganizationSet(IAppHost host)
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
            messageDispatcher.Register((IHandler<AddOrganizationCommand>)handler);
            messageDispatcher.Register((IHandler<OrganizationAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateOrganizationCommand>)handler);
            messageDispatcher.Register((IHandler<OrganizationUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveOrganizationCommand>)handler);
            messageDispatcher.Register((IHandler<OrganizationRemovedEvent>)handler);
        }

        public bool TryGetOrganization(Guid organizationID, out OrganizationState oragnization)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.TryGetValue(organizationID, out oragnization);
        }

        public bool TryGetOrganization(string organizationCode, out OrganizationState organization)
        {
            if (!_initialized)
            {
                Init();
            }
            if (organizationCode == null)
            {
                organization = OrganizationState.Empty;
                return false;
            }
            return _dicByCode.TryGetValue(organizationCode, out organization);
        }

        internal void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
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
                        _dicByCode.Add(OrganizationState.VirtualRoot.Code, OrganizationState.VirtualRoot);
                        _dicByID.Add(OrganizationState.VirtualRoot.Id, OrganizationState.VirtualRoot);
                        var allOrganizations = host.GetRequiredService<IAppHostBootstrap>().GetOrganizations().OrderBy(a => a.ParentCode);
                        foreach (var organization in allOrganizations)
                        {
                            if (!(organization is OrganizationBase))
                            {
                                throw new CoreException(organization.GetType().Name + "必须继承" + typeof(OrganizationBase).Name);
                            }
                            OrganizationState orgState;
                            orgState = OrganizationState.Create(host, organization);
                            if (!_dicByCode.ContainsKey(organization.Code))
                            {
                                _dicByCode.Add(organization.Code, orgState);
                            }
                            if (!_dicByID.ContainsKey(organization.Id))
                            {
                                _dicByID.Add(organization.Id, orgState);
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        public IEnumerator<OrganizationState> GetEnumerator()
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

        #region MessageHandler
        private class MessageHandler :
            IHandler<UpdateOrganizationCommand>, 
            IHandler<AddOrganizationCommand>, 
            IHandler<OrganizationAddedEvent>, 
            IHandler<OrganizationUpdatedEvent>, 
            IHandler<RemoveOrganizationCommand>, 
            IHandler<OrganizationRemovedEvent>
        {
            private readonly OrganizationSet set;

            public MessageHandler(OrganizationSet set)
            {
                this.set = set;
            }

            public void Handle(AddOrganizationCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(OrganizationAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateOrganizationAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IOrganizationCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var organizationRepository = host.GetRequiredService<IRepository<Organization>>();
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (string.IsNullOrEmpty(input.Name))
                {
                    throw new ValidationException("名称是必须的");
                }
                Organization entity;
                lock (this)
                {
                    OrganizationState organization;
                    if (host.OrganizationSet.TryGetOrganization(input.Id.Value, out organization))
                    {
                        throw new ValidationException("给定标识的组织结构已经存在");
                    }
                    if (host.OrganizationSet.TryGetOrganization(input.Code, out organization))
                    {
                        throw new ValidationException("重复的组织结构码");
                    }
                    if (!string.IsNullOrEmpty(input.ParentCode))
                    {
                        OrganizationState parentOragnization;
                        if (!host.OrganizationSet.TryGetOrganization(input.ParentCode, out parentOragnization))
                        {
                            throw new NotExistException("标识为" + input.ParentCode + "的父组织结构不存在");
                        }
                        if (string.Equals(input.Code, input.ParentCode) || !input.Code.StartsWith(parentOragnization.Code, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new ValidationException("子级组织结构的编码必须以父级组织结构编码为前缀");
                        }
                        if (host.OrganizationSet.Any(a => string.Equals(a.ParentCode, input.ParentCode) && string.Equals(a.Name, input.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            throw new ValidationException("兄弟组织结构间不能重名");
                        }
                    }
                    else
                    {
                        if (host.OrganizationSet.Any(a => string.IsNullOrEmpty(a.ParentCode) && string.Equals(a.Name, input.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            throw new ValidationException("重复的组织结构名");
                        }
                    }

                    entity = Organization.Create(input);
                    var state = OrganizationState.Create(host, entity);
                    if (!_dicByCode.ContainsKey(entity.Code))
                    {
                        _dicByCode.Add(entity.Code, state);
                    }
                    if (!_dicByID.ContainsKey(entity.Id))
                    {
                        _dicByID.Add(entity.Id, state);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            organizationRepository.Add(entity);
                            organizationRepository.Context.Commit();
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
                            organizationRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateOrganizationAddedEvent(entity, input));
                }
            }

            private class PrivateOrganizationAddedEvent : OrganizationAddedEvent
            {
                public PrivateOrganizationAddedEvent(OrganizationBase source, IOrganizationCreateInput input)
                    : base(source, input)
                {

                }
            }

            public void Handle(UpdateOrganizationCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(OrganizationUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateOrganizationUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IOrganizationUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var organizationRepository = host.GetRequiredService<IRepository<Organization>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                OrganizationState bkState;
                if (!host.OrganizationSet.TryGetOrganization(input.Id, out bkState))
                {
                    throw new ValidationException("给定标识的组织结构不存在" + input.Id);
                }
                Organization entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    OrganizationState oragnization;
                    if (host.OrganizationSet.TryGetOrganization(input.Code, out oragnization) && oragnization.Id != input.Id)
                    {
                        throw new ValidationException("重复的组织结构码" + input.Code);
                    }
                    if (!string.IsNullOrEmpty(input.ParentCode))
                    {
                        OrganizationState parentOragnization;
                        if (!host.OrganizationSet.TryGetOrganization(input.ParentCode, out parentOragnization))
                        {
                            throw new NotExistException("标识为" + input.ParentCode + "的父组织结构不存在");
                        }
                        if (input.ParentCode.Equals(input.Code, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new CoreException("组织结构的父组织结构不能是自己");
                        }
                        if (!input.Code.StartsWith(parentOragnization.Code, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new ValidationException("子级组织结构的编码必须以父级组织结构编码为前缀");
                        }
                        List<IOrganization> childOrgs = new List<IOrganization>();
                        if (input.ParentCode.StartsWith(input.Code, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new CoreException("组织结构的父组织结构不能是自己的子孙级组织结构");
                        }
                    }
                    entity = organizationRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = OrganizationState.Create(host, entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            organizationRepository.Update(entity);
                            organizationRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            organizationRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateOrganizationUpdatedEvent(entity, input));
                }
            }
            private void Update(OrganizationState state)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var oldKey = _dicByID[state.Id].Code;
                var newKey = state.Code;
                _dicByID[state.Id] = state;
                if (!_dicByCode.ContainsKey(newKey))
                {
                    _dicByCode.Add(newKey, state);
                    _dicByCode.Remove(oldKey);
                }
                else
                {
                    _dicByCode[newKey] = state;
                }
            }

            private class PrivateOrganizationUpdatedEvent : OrganizationUpdatedEvent
            {
                public PrivateOrganizationUpdatedEvent(OrganizationBase source, IOrganizationUpdateInput input) : base(source, input) { }
            }

            public void Handle(RemoveOrganizationCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(OrganizationRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateOrganizationRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid organizationID, bool isCommand)
            {
                var host = set.host;
                var _dicByCode = set._dicByCode;
                var _dicByID = set._dicByID;
                var organizationRepository = host.GetRequiredService<IRepository<Organization>>();
                OrganizationState bkState;
                if (!host.OrganizationSet.TryGetOrganization(organizationID, out bkState))
                {
                    return;
                }
                Organization entity;
                lock (bkState)
                {
                    OrganizationState state;
                    if (!host.OrganizationSet.TryGetOrganization(organizationID, out state))
                    {
                        return;
                    }
                    if (host.OrganizationSet.Any(a => bkState.Code.Equals(a.ParentCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new ValidationException("组织结构下有子组织结构时不能删除");
                    }
                    entity = organizationRepository.GetByKey(organizationID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (_dicByID.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new OrganizationRemovingEvent(entity));
                        }
                        _dicByID.Remove(bkState.Id);
                        _dicByCode.Remove(bkState.Code);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            organizationRepository.Remove(entity);
                            organizationRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_dicByID.ContainsKey(bkState.Id))
                            {
                                _dicByID.Add(bkState.Id, bkState);
                                _dicByCode.Add(bkState.Code, bkState);
                            }
                            organizationRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateOrganizationRemovedEvent(entity));
                }
            }

            private class PrivateOrganizationRemovedEvent : OrganizationRemovedEvent
            {
                public PrivateOrganizationRemovedEvent(OrganizationBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion
    }
}