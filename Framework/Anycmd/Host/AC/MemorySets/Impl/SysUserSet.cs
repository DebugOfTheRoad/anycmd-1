
namespace Anycmd.Host.AC.MemorySets.Impl
{
    using Anycmd.AC.Identity;
    using Bus;
    using Exceptions;
    using Extensions;
    using Host;
    using Identity;
    using Identity.Messages;
    using Repositories;
    using System;
    using System.Collections.Generic;
    using loginName = System.String;

    public sealed class SysUserSet : ISysUserSet
    {
        public static readonly ISysUserSet Empty = new SysUserSet(AppHost.Empty);

        private readonly Dictionary<Guid, AccountState> _devAccountByID = new Dictionary<Guid, AccountState>();
        private readonly Dictionary<loginName, AccountState> _devAccountByLoginName = new Dictionary<loginName, AccountState>(StringComparer.OrdinalIgnoreCase);
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly IAppHost host;

        public Guid Id
        {
            get { return _id; }
        }

        public SysUserSet(IAppHost host)
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
            messageDispatcher.Register((IHandler<AddDeveloperCommand>)handler);
            messageDispatcher.Register((IHandler<DeveloperAddedEvent>)handler);
            messageDispatcher.Register((IHandler<DeveloperUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveDeveloperCommand>)handler);
            messageDispatcher.Register((IHandler<DeveloperRemovedEvent>)handler);
        }

        public IReadOnlyCollection<AccountState> GetDevAccounts()
        {
            if (!_initialized)
            {
                Init();
            }
            // 不存储就要计算，存储就会占内存
            return new List<AccountState>(_devAccountByID.Values);
        }

        public bool TryGetDevAccount(string developerCode, out AccountState developer)
        {
            if (!_initialized)
            {
                Init();
            }
            if (developerCode == null)
            {
                developer = null;
                return false;
            }

            return _devAccountByLoginName.TryGetValue(developerCode, out developer);
        }

        public bool TryGetDevAccount(Guid developerID, out AccountState developer)
        {
            if (!_initialized)
            {
                Init();
            }

            return _devAccountByID.TryGetValue(developerID, out developer);
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
                        _devAccountByID.Clear();
                        _devAccountByLoginName.Clear();
                        var accounts = host.GetRequiredService<IAppHostBootstrap>().GetAllDevAccounts();
                        foreach (var account in accounts)
                        {
                            if (!(account is AccountBase))
                            {
                                throw new CoreException(account.GetType().Name + "必须继承" + typeof(AccountBase).Name);
                            }
                            var accountState = AccountState.Create(account);
                            if (!_devAccountByID.ContainsKey(account.Id))
                            {
                                _devAccountByID.Add(account.Id, accountState);
                            }
                            if (!_devAccountByLoginName.ContainsKey(account.LoginName))
                            {
                                _devAccountByLoginName.Add(account.LoginName, accountState);
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        private class MessageHandle :
            IHandler<AddDeveloperCommand>,
            IHandler<DeveloperUpdatedEvent>,
            IHandler<DeveloperRemovedEvent>,
            IHandler<RemoveDeveloperCommand>,
            IHandler<DeveloperAddedEvent>
        {
            private readonly SysUserSet set;

            public MessageHandle(SysUserSet set)
            {
                this.set = set;
            }

            public void Handle(AddDeveloperCommand message)
            {
                this.Handle(message.AccountID, true);
            }

            public void Handle(DeveloperAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateDeveloperAddedEvent))
                {
                    return;
                }

                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid accountID, bool isCommand)
            {
                var host = set.host;
                var _devAccountByID = set._devAccountByID;
                var _devAccountByLoginName = set._devAccountByLoginName;
                var accountRepository = host.GetRequiredService<IRepository<Account>>();
                var developerRepository = host.GetRequiredService<IRepository<DeveloperID>>();
                DeveloperID entity;
                lock (this)
                {
                    var account = accountRepository.GetByKey(accountID);
                    if (account == null)
                    {
                        throw new ValidationException("账户不存在");
                    }
                    if (_devAccountByID.ContainsKey(accountID))
                    {
                        throw new ValidationException("给定标识标识的开发人员已经存在" + accountID);
                    }
                    entity = new DeveloperID
                    {
                        Id = accountID
                    };
                    try
                    {
                        var accountState = AccountState.Create(account);
                        _devAccountByID.Add(accountID, accountState);
                        _devAccountByLoginName.Add(account.LoginName, accountState);
                        if (isCommand)
                        {
                            developerRepository.Add(entity);
                            developerRepository.Context.Commit();
                        }
                    }
                    catch
                    {
                        _devAccountByID.Remove(accountID);
                        _devAccountByLoginName.Remove(account.LoginName);
                        developerRepository.Context.Rollback();
                        throw;
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateDeveloperAddedEvent(entity));
                }
            }

            private class PrivateDeveloperAddedEvent : DeveloperAddedEvent
            {
                public PrivateDeveloperAddedEvent(DeveloperID source) : base(source) { }
            }

            public void Handle(DeveloperUpdatedEvent message)
            {
                var _devAccountByID = set._devAccountByID;
                var _devAccountByLoginName = set._devAccountByLoginName;
                var entity = message.Source as AccountBase;
                AccountState oldState;
                if (!_devAccountByID.TryGetValue(message.Source.Id, out oldState))
                {
                    throw new CoreException("给定标识的用户不存在");
                }
                var newState = AccountState.Create(entity);
                _devAccountByID[message.Source.Id] = newState;
                if (!_devAccountByLoginName.ContainsKey(newState.LoginName))
                {
                    _devAccountByLoginName.Add(newState.LoginName, newState);
                    _devAccountByLoginName.Remove(oldState.LoginName);
                }
                else
                {
                    _devAccountByLoginName[newState.LoginName] = newState;
                }
            }

            public void Handle(RemoveDeveloperCommand message)
            {
                this.HandleRemove(message.AccountID, true);
            }

            public void Handle(DeveloperRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateDeveloperRemovedEvent))
                {
                    return;
                }
                this.HandleRemove(message.Source.Id, false);
            }

            private void HandleRemove(Guid accountID, bool isCommand)
            {
                var host = set.host;
                var _devAccountByID = set._devAccountByID;
                var _devAccountByLoginName = set._devAccountByLoginName;
                var developerRepository = host.GetRequiredService<IRepository<DeveloperID>>();
                if (!_devAccountByID.ContainsKey(accountID))
                {
                    return;
                }
                var bkState = _devAccountByID[accountID];
                DeveloperID entity;
                lock (bkState)
                {
                    if (!_devAccountByID.ContainsKey(accountID))
                    {
                        return;
                    }
                    entity = developerRepository.GetByKey(accountID);
                    if (entity == null)
                    {
                        return;
                    }
                    try
                    {
                        _devAccountByID.Remove(accountID);
                        _devAccountByLoginName.Remove(bkState.LoginName);
                        if (isCommand)
                        {
                            developerRepository.Remove(entity);
                            developerRepository.Context.Commit();
                        }
                    }
                    catch
                    {
                        _devAccountByID.Add(accountID, bkState);
                        _devAccountByLoginName.Add(bkState.LoginName, bkState);
                        developerRepository.Context.Rollback();
                        throw;
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateDeveloperRemovedEvent(entity));
                }
            }

            private class PrivateDeveloperRemovedEvent : DeveloperRemovedEvent
            {
                public PrivateDeveloperRemovedEvent(DeveloperID source) : base(source) { }
            }
        }
    }
}
