
namespace Anycmd.Host.AC.MemorySets
{
    using AC;
    using Anycmd.AC;
    using Bus;
    using Exceptions;
    using Extensions;
    using Host;
    using Host.AC.Identity;
    using Infra.Messages;
    using Messages;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Util;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public sealed class PrivilegeSet : IPrivilegeSet
    {
        private readonly List<PrivilegeBigramState> _privilegeList = new List<PrivilegeBigramState>();
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly AppHost host;

        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PrivilegeSet(AppHost host)
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
            messageDispatcher.Register((IHandler<AddPrivilegeBigramCommand>)handler);
            messageDispatcher.Register((IHandler<PrivilegeBigramAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdatePrivilegeBigramCommand>)handler);
            messageDispatcher.Register((IHandler<PrivilegeBigramUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemovePrivilegeBigramCommand>)handler);
            messageDispatcher.Register((IHandler<PrivilegeBigramRemovedEvent>)handler);
            messageDispatcher.Register((IHandler<OrganizationRemovingEvent>)handler);
            messageDispatcher.Register((IHandler<RoleRemovingEvent>)handler);
            messageDispatcher.Register((IHandler<FunctionRemovingEvent>)handler);
            messageDispatcher.Register((IHandler<MenuRemovingEvent>)handler);
            messageDispatcher.Register((IHandler<GroupRemovingEvent>)handler);
            messageDispatcher.Register((IHandler<AppSystemRemovingEvent>)handler);
            messageDispatcher.Register((IHandler<ResourceTypeRemovingEvent>)handler);
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _privilegeList.Clear();
                        var rolePrivileges = host.GetRequiredService<IAppHostBootstrap>().GetPrivilegeBigrams();
                        foreach (var rolePrivilege in rolePrivileges)
                        {
                            var rolePrivilegeState = PrivilegeBigramState.Create(rolePrivilege);
                            _privilegeList.Add(rolePrivilegeState);
                        }
                        _initialized = true;
                    }
                }
            }
        }

        public IEnumerator<PrivilegeBigramState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _privilegeList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _privilegeList.GetEnumerator();
        }

        #region MessageHandler
        private class MessageHandler :
            IHandler<AddPrivilegeBigramCommand>,
            IHandler<PrivilegeBigramAddedEvent>,
            IHandler<UpdatePrivilegeBigramCommand>,
            IHandler<PrivilegeBigramUpdatedEvent>,
            IHandler<RemovePrivilegeBigramCommand>,
            IHandler<PrivilegeBigramRemovedEvent>,
            IHandler<OrganizationRemovingEvent>,
            IHandler<RoleRemovingEvent>,
            IHandler<FunctionRemovingEvent>,
            IHandler<MenuRemovingEvent>,
            IHandler<GroupRemovingEvent>,
            IHandler<AppSystemRemovingEvent>,
            IHandler<ResourceTypeRemovingEvent>
        {
            private readonly PrivilegeSet set;

            public MessageHandler(PrivilegeSet set)
            {
                this.set = set;
            }

            public void Handle(OrganizationRemovingEvent message)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var items = new HashSet<PrivilegeBigramState>();
                foreach (var item in _privilegeList.Where(a => a.ObjectType == ACObjectType.Organization && a.ObjectInstanceID == message.Source.Id))
                {
                    host.Handle(new RemovePrivilegeBigramCommand(item.Id));
                }
            }

            public void Handle(RoleRemovingEvent message)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var accountPrivilegeRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                var accountPrivileges = accountPrivilegeRepository.FindAll().Where(a => a.ObjectInstanceID == message.Source.Id || a.SubjectInstanceID == message.Source.Id).ToList();
                foreach (var item in accountPrivileges)
                {
                    accountPrivilegeRepository.Remove(item);
                }
                foreach (var item in _privilegeList.Where(a => a.ObjectType == ACObjectType.Role && a.ObjectInstanceID == message.Source.Id))
                {
                    host.Handle(new RemovePrivilegeBigramCommand(item.Id));
                }
            }

            public void Handle(FunctionRemovingEvent message)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var accountPrivilegeRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                var accountPrivileges = accountPrivilegeRepository.FindAll().Where(a => a.ObjectInstanceID == message.Source.Id || a.SubjectInstanceID == message.Source.Id).ToList();
                foreach (var item in accountPrivileges)
                {
                    accountPrivilegeRepository.Remove(item);
                }
                foreach (var item in _privilegeList.Where(a => a.ObjectType == ACObjectType.Function && a.ObjectInstanceID == message.Source.Id))
                {
                    host.Handle(new RemovePrivilegeBigramCommand(item.Id));
                }
            }

            public void Handle(MenuRemovingEvent message)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var accountPrivilegeRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                var accountPrivileges = accountPrivilegeRepository.FindAll().Where(a => a.ObjectInstanceID == message.Source.Id || a.SubjectInstanceID == message.Source.Id).ToList();
                foreach (var item in accountPrivileges)
                {
                    accountPrivilegeRepository.Remove(item);
                }
                foreach (var item in _privilegeList.Where(a => a.ObjectType == ACObjectType.Menu && a.ObjectInstanceID == message.Source.Id))
                {
                    host.Handle(new RemovePrivilegeBigramCommand(item.Id));
                }
            }

            public void Handle(GroupRemovingEvent message)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var accountPrivilegeRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                var accountPrivileges = accountPrivilegeRepository.FindAll().Where(a => a.ObjectInstanceID == message.Source.Id || a.SubjectInstanceID == message.Source.Id).ToList();
                foreach (var item in accountPrivileges)
                {
                    accountPrivilegeRepository.Remove(item);
                }
                foreach (var item in _privilegeList.Where(a => a.ObjectType == ACObjectType.Group && a.ObjectInstanceID == message.Source.Id))
                {
                    host.Handle(new RemovePrivilegeBigramCommand(item.Id));
                }
            }

            public void Handle(AppSystemRemovingEvent message)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var accountPrivilegeRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                var accountPrivileges = accountPrivilegeRepository.FindAll().Where(a => a.ObjectInstanceID == message.Source.Id || a.SubjectInstanceID == message.Source.Id).ToList();
                foreach (var item in accountPrivileges)
                {
                    accountPrivilegeRepository.Remove(item);
                }
                foreach (var item in _privilegeList.Where(a => a.ObjectType == ACObjectType.AppSystem && a.ObjectInstanceID == message.Source.Id))
                {
                    host.Handle(new RemovePrivilegeBigramCommand(item.Id));
                }
            }

            public void Handle(ResourceTypeRemovingEvent message)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var accountPrivilegeRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                var accountPrivileges = accountPrivilegeRepository.FindAll().Where(a => a.ObjectInstanceID == message.Source.Id || a.SubjectInstanceID == message.Source.Id).ToList();
                foreach (var item in accountPrivileges)
                {
                    accountPrivilegeRepository.Remove(item);
                }
                foreach (var item in _privilegeList.Where(a => a.ObjectType == ACObjectType.ResourceType && a.ObjectInstanceID == message.Source.Id))
                {
                    host.Handle(new RemovePrivilegeBigramCommand(item.Id));
                }
            }

            public void Handle(AddPrivilegeBigramCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(PrivilegeBigramAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateRolePrivilegeAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IPrivilegeBigramCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var privilegeBigramRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                var accountRepository = host.GetRequiredService<IRepository<Account>>();
                if (!input.Id.HasValue || input.Id.Value == Guid.Empty)
                {
                    throw new CoreException("意外的标识");
                }
                ACSubjectType subjectType;
                if (!input.SubjectType.TryParse(out subjectType))
                {
                    throw new ValidationException("非法的主体类型" + input.SubjectType);
                }
                ACObjectType acObjectType;
                if (!input.ObjectType.TryParse(out acObjectType))
                {
                    throw new ValidationException("非法的客体类型" + input.ObjectType);
                }
                PrivilegeBigram entity;
                lock (this)
                {
                    switch (subjectType)
                    {
                        case ACSubjectType.Undefined:
                            throw new CoreException("意外的主体类型" + subjectType.ToString());
                        case ACSubjectType.Account:
                            if (!accountRepository.FindAll().Any(a => a.Id == input.SubjectInstanceID))
                            {
                                throw new ValidationException("给定标识的账户不存在" + input.SubjectInstanceID); ;
                            }
                            break;
                        case ACSubjectType.Role:
                            RoleState role;
                            if (!host.RoleSet.TryGetRole(input.SubjectInstanceID, out role))
                            {
                                throw new ValidationException("意外的角色标识" + input.SubjectInstanceID);
                            }
                            break;
                        case ACSubjectType.Organization:
                            OrganizationState org;
                            if (!host.OrganizationSet.TryGetOrganization(input.SubjectInstanceID, out org))
                            {
                                throw new ValidationException("意外的组织结构标识" + input.SubjectInstanceID);
                            }
                            break;
                        case ACSubjectType.Privilege:
                            throw new NotSupportedException();
                        default:
                            throw new CoreException("意外的主体类型" + subjectType.ToString());
                    }
                    switch (acObjectType)
                    {
                        case ACObjectType.Undefined:
                            throw new ValidationException("意外的账户权限类型" + input.SubjectType);
                        case ACObjectType.Organization:
                            OrganizationState organization;
                            if (!host.OrganizationSet.TryGetOrganization(input.ObjectInstanceID, out organization))
                            {
                                throw new ValidationException("意外的组织结构标识" + input.ObjectInstanceID);
                            }
                            break;
                        case ACObjectType.Role:
                            RoleState role;
                            if (!host.RoleSet.TryGetRole(input.ObjectInstanceID, out role))
                            {
                                throw new ValidationException("意外的角色标识" + input.ObjectInstanceID);
                            }
                            break;
                        case ACObjectType.Group:
                            GroupState group;
                            if (!host.GroupSet.TryGetGroup(input.ObjectInstanceID, out group))
                            {
                                throw new ValidationException("意外的工作组标识" + input.ObjectInstanceID);
                            }
                            break;
                        case ACObjectType.Function:
                            FunctionState function;
                            if (!host.FunctionSet.TryGetFunction(input.ObjectInstanceID, out function))
                            {
                                throw new ValidationException("意外的功能标识" + input.ObjectInstanceID);
                            }
                            break;
                        case ACObjectType.Menu:
                            MenuState menu;
                            if (!host.MenuSet.TryGetMenu(input.ObjectInstanceID, out menu))
                            {
                                throw new ValidationException("意外的菜单标识" + input.ObjectInstanceID);
                            }
                            break;
                        case ACObjectType.AppSystem:
                            if (!host.AppSystemSet.ContainsAppSystem(input.ObjectInstanceID))
                            {
                                throw new ValidationException("意外的应用系统标识" + input.ObjectInstanceID);
                            }
                            break;
                        case ACObjectType.ResourceType:
                            ResourceTypeState resource;
                            if (!host.ResourceSet.TryGetResource(input.ObjectInstanceID, out resource))
                            {
                                throw new ValidationException("意外的资源类型标识" + input.ObjectInstanceID);
                            }
                            break;
                        case ACObjectType.Privilege:
                            throw new ValidationException("暂不支持" + input.SubjectType + "类型的授权");
                        default:
                            throw new ValidationException("意外的账户权限类型" + input.SubjectType);
                    }
                    if (subjectType == ACSubjectType.Role && acObjectType == ACObjectType.Role)
                    {
                        if (input.SubjectInstanceID == input.ObjectInstanceID)
                        {
                            throw new ValidationException("角色不能继承自己");
                        }
                        var parentIDs = new HashSet<Guid>();
                        GetParentRoles(input.SubjectInstanceID, parentIDs);
                        if (parentIDs.Contains(input.SubjectInstanceID))
                        {
                            throw new ValidationException("角色不能继承自己的子孙");
                        }
                    }
                    if (subjectType == ACSubjectType.Account && acObjectType == ACObjectType.Account)
                    {
                        if (input.SubjectInstanceID == input.ObjectInstanceID)
                        {
                            throw new ValidationException("账户不能继承自己");
                        }
                        var parentIDs = new HashSet<Guid>();
                        GetParentAccounts(input.SubjectInstanceID, parentIDs);
                        if (parentIDs.Contains(input.SubjectInstanceID))
                        {
                            throw new ValidationException("账户不能继承自己的子孙");
                        }
                    }
                    if (subjectType == ACSubjectType.Organization && acObjectType == ACObjectType.Organization)
                    {
                        if (input.SubjectInstanceID == input.ObjectInstanceID)
                        {
                            throw new ValidationException("组织机构不能继承自己");
                        }
                        var parentIDs = new HashSet<Guid>();
                        GetParentOrganizations(input.SubjectInstanceID, parentIDs);
                        if (parentIDs.Contains(input.SubjectInstanceID))
                        {
                            throw new ValidationException("组织机构不能继承自己的子孙");
                        }
                    }
                    if (host.PrivilegeSet.Any(a => a.ObjectType == acObjectType && a.ObjectInstanceID == input.ObjectInstanceID && a.SubjectType == subjectType && a.SubjectInstanceID == input.SubjectInstanceID))
                    {
                        return;
                    }

                    entity = PrivilegeBigram.Create(input);

                    if (subjectType != ACSubjectType.Account && !_privilegeList.Any(a => a.Id == entity.Id))
                    {
                        _privilegeList.Add(PrivilegeBigramState.Create(entity));
                    }
                    if (isCommand)
                    {
                        try
                        {
                            privilegeBigramRepository.Add(entity);
                            privilegeBigramRepository.Context.Commit();
                        }
                        catch
                        {
                            if (subjectType != ACSubjectType.Account && _privilegeList.Any(a => a.Id == entity.Id))
                            {
                                var item = _privilegeList.First(a => a.Id == entity.Id);
                                _privilegeList.Remove(item);
                            }
                            privilegeBigramRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateRolePrivilegeAddedEvent(entity, input));
                }
            }

            private void GetParentRoles(Guid roleID, HashSet<Guid> parentIDs)
            {
                if (parentIDs == null)
                {
                    parentIDs = new HashSet<Guid>();
                } 
                var host = set.host;
                var _privilegeList = set._privilegeList;
                foreach (var item in _privilegeList.Where(a=>a.SubjectType == ACSubjectType.Role && a.SubjectInstanceID == roleID && a.ObjectType == ACObjectType.Role))
                {
                    GetParentRoles(item.ObjectInstanceID, parentIDs);
                    parentIDs.Add(item.ObjectInstanceID);
                }
            }

            private void GetParentOrganizations(Guid organizationID, HashSet<Guid> parentIDs)
            {
                if (parentIDs == null)
                {
                    parentIDs = new HashSet<Guid>();
                }
                var host = set.host;
                var _privilegeList = set._privilegeList;
                foreach (var item in _privilegeList.Where(a => a.SubjectType == ACSubjectType.Organization && a.SubjectInstanceID == organizationID && a.ObjectType == ACObjectType.Organization))
                {
                    GetParentRoles(item.ObjectInstanceID, parentIDs);
                    parentIDs.Add(item.ObjectInstanceID);
                }
            }

            private void GetParentAccounts(Guid accountID, HashSet<Guid> parentIDs)
            {
                if (parentIDs == null)
                {
                    parentIDs = new HashSet<Guid>();
                }
                var host = set.host;
                var _privilegeList = set._privilegeList;
                foreach (var item in _privilegeList.Where(a => a.SubjectType == ACSubjectType.Account && a.SubjectInstanceID == accountID && a.ObjectType == ACObjectType.Account))
                {
                    GetParentRoles(item.ObjectInstanceID, parentIDs);
                    parentIDs.Add(item.ObjectInstanceID);
                }
            }

            private class PrivateRolePrivilegeAddedEvent : PrivilegeBigramAddedEvent
            {
                public PrivateRolePrivilegeAddedEvent(PrivilegeBigramBase source, IPrivilegeBigramCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdatePrivilegeBigramCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(PrivilegeBigramUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateRoleprivilegeUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IPrivilegeBigramUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var privilegeRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                PrivilegeBigram entity = null;
                bool stateChanged = false;
                lock (this)
                {
                    entity = privilegeRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException("不存在的权限记录标识" + input.Id);
                    }
                    var bkState = host.PrivilegeSet.FirstOrDefault(a => a.Id == input.Id);
                    bool isAccountSubjectType = string.Equals(ACSubjectType.Account.ToName(), entity.SubjectType);

                    entity.Update(input);

                    var newState = PrivilegeBigramState.Create(entity);
                    stateChanged = newState != bkState;
                    if (!isAccountSubjectType && stateChanged)
                    {
                        _privilegeList.Remove(bkState);
                        _privilegeList.Add(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            privilegeRepository.Update(entity);
                            privilegeRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!isAccountSubjectType && stateChanged)
                            {
                                _privilegeList.Remove(newState);
                                _privilegeList.Add(bkState);
                            }
                            privilegeRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateRoleprivilegeUpdatedEvent(entity, input));
                }
            }

            private class PrivateRoleprivilegeUpdatedEvent : PrivilegeBigramUpdatedEvent
            {
                public PrivateRoleprivilegeUpdatedEvent(PrivilegeBigramBase source, IPrivilegeBigramUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemovePrivilegeBigramCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(PrivilegeBigramRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateRolePrivilegeRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid privilegeBigramID, bool isCommand)
            {
                var host = set.host;
                var _privilegeList = set._privilegeList;
                var privilegeRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                PrivilegeBigram entity;
                lock (this)
                {
                    var bkState = host.PrivilegeSet.FirstOrDefault(a => a.Id == privilegeBigramID);
                    entity = privilegeRepository.GetByKey(privilegeBigramID);
                    bool isAccountSubjectType = bkState == null;
                    if (entity == null)
                    {
                        return;
                    }
                    if (!isAccountSubjectType)
                    {
                        _privilegeList.Remove(bkState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            privilegeRepository.Remove(entity);
                            privilegeRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!isAccountSubjectType)
                            {
                                _privilegeList.Add(bkState);
                            }
                            privilegeRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateRolePrivilegeRemovedEvent(entity));
                }
            }

            private class PrivateRolePrivilegeRemovedEvent : PrivilegeBigramRemovedEvent
            {
                public PrivateRolePrivilegeRemovedEvent(PrivilegeBigramBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion
    }
}
