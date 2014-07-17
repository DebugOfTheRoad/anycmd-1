
namespace Anycmd
{
    using Anycmd.AC;
    using Bus;
    using Commands;
    using Container;
    using Dapper;
    using Events;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Identity;
    using Host.AC.Identity.Messages;
    using Host.AC.Infra;
    using Host.AC.MemorySets;
    using Logging;
    using Rdb;
    using Repositories;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Principal;
    using System.Threading;
    using System.Web;
    using System.Web.Security;
    using Util;

    /// <summary>
    /// Anycmd框架宿主。它是访问所有系统实体的入口。它确立了一个边界，如果进程中实例化了多个AppHost实例的话。
    /// </summary>
    public abstract class AppHost
    {
        private static AppHost _instance = null;
        private static object locker = new object();

        private readonly string _buildInPluginsBaseDirectory;
        private readonly Guid _id = Guid.NewGuid();
        private readonly HashSet<EntityTypeMap> _entityTypeMaps = new HashSet<EntityTypeMap>();

        /// <summary>
        /// 单件。根据appSetting的NodeHost节点配置构建。
        /// </summary>
        public static AppHost Instance
        {
            get
            {
                return _instance;
            }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Name { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartedAt { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime AfterInitAt { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ReadyAt { get; protected set; }

        public AnycmdServiceContainer Container { get; private set; }

        protected AppHost()
        {
            lock (locker)
            {
                _instance = this;
                this.Name = "DefaultAppHost";
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                bool isASPNET =
                    !AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.EndsWith("dll.config", StringComparison.OrdinalIgnoreCase)
                    && !AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.EndsWith("exe.config", StringComparison.OrdinalIgnoreCase);
                _buildInPluginsBaseDirectory = isASPNET ? Path.Combine(dir, "Bin", "Plugins") : Path.Combine(dir, "Plugins");
                this.Container = new AnycmdServiceContainer();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual AppHost Init()
        {
            OnConfigLoad();

            Configure(this.Container);

            this.Config = new AppConfig(this.GetRequiredService<IAppHostBootstrap>().GetParameters());
            OnAfterInit();

            return this;
        }

        private IAppConfig _config;
        /// <summary>
        /// 
        /// </summary>
        public IAppConfig Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
                OnAfterConfigChanged();
            }
        }

        public void Map(EntityTypeMap map)
        {
            this._entityTypeMaps.Add(map);
        }

        public IEnumerable<EntityTypeMap> GetEntityTypeMaps()
        {
            return this._entityTypeMaps;
        }

        #region 属性
        /// <summary>
        /// 应用程序域内插件基地址。
        /// </summary>
        public string BuildInPluginsBaseDirectory
        {
            get
            {
                return _buildInPluginsBaseDirectory;
            }
        }

        public IUserSession User
        {
            get
            {
                IPrincipal principal;
                if (HttpContext.Current != null)
                {
                    principal = HttpContext.Current.User;
                }
                else
                {
                    principal = Thread.CurrentPrincipal;
                }
                if (principal.Identity.IsAuthenticated)
                {
                    var storage = this.GetRequiredService<IUserSessionStorage>();
                    var user = storage.GetData("UserSession") as IUserSession;
                    if (user == null)
                    {
                        var account = this.GetAccountByLoginName(principal.Identity.Name);
                        if (account == null)
                        {
                            return UserSessionState.Empty;
                        }
                        var accountPrivileges = GetAccountPrivileges(account.Id);
                        user = CreateSession(AccountState.Create(account), accountPrivileges, Guid.NewGuid());
                        storage.SetData("UserSession", user);
                    }
                    return user;
                }
                else
                {
                    return UserSessionState.Empty;
                }
            }
            private set
            {
                var storage = this.GetRequiredService<IUserSessionStorage>();
                storage.SetData("UserSession", value);
            }
        }

        public IMessageDispatcher MessageDispatcher { get; protected set; }

        public ICommandBus CommandBus { get; protected set; }

        public IEventBus EventBus { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IRdbs Rdbs { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbTables DbTables { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbViews DbViews { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbTableColumns DbTableColumns { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbViewColumns DbViewColumns { get; protected set; }

        /// <summary>
        /// 应用系统。
        /// </summary>
        public IAppSystemSet AppSystemSet { get; protected set; }

        /// <summary>
        /// 系统按钮
        /// </summary>
        public IButtonSet ButtonSet { get; protected set; }

        /// <summary>
        /// 系统用户/账户
        /// </summary>
        public ISysUserSet SysUsers { get; protected set; }

        /// <summary>
        /// 系统字典
        /// </summary>
        public IDicSet DicSet { get; protected set; }

        /// <summary>
        /// 系统模型
        /// </summary>
        public IEntityTypeSet EntityTypeSet { get; protected set; }

        /// <summary>
        /// 系统操作
        /// </summary>
        public IFunctionSet FunctionSet { get; protected set; }

        /// <summary>
        /// 系统组织结构
        /// </summary>
        public IOrganizationSet OrganizationSet { get; protected set; }

        /// <summary>
        /// 系统页面
        /// </summary>
        public IPageSet PageSet { get; protected set; }

        /// <summary>
        /// 系统资源
        /// </summary>
        public IResourceSet ResourceSet { get; protected set; }

        /// <summary>
        /// 权限集
        /// </summary>
        public IPrivilegeSet PrivilegeSet { get; protected set; }

        /// <summary>
        /// 菜单集
        /// </summary>
        public IMenuSet MenuSet { get; protected set; }

        /// <summary>
        /// 角色集
        /// </summary>
        public IRoleSet RoleSet { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public IGroupSet GroupSet { get; protected set; }
        #endregion

        /// <summary>
        /// Config has changed
        /// </summary>
        public virtual void OnAfterConfigChanged()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Configure(AnycmdServiceContainer container);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public virtual void SetConfig(IAppConfig config)
        {
            Config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnConfigLoad()
        {
        }

        /// <summary>
        /// 当Configure方法调用后。
        /// </summary>
        public void OnAfterInit()
        {
            AfterInitAt = DateTime.UtcNow;

            ReadyAt = DateTime.UtcNow;
        }

        public T DeserializeFromString<T>(string value)
        {
            // TODO:移除对ServiceStack.Text的依赖
            return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
        }

        public string SerializeToString<T>(T value)
        {
            return ServiceStack.Text.JsonSerializer.SerializeToString<T>(value);
        }

        /// <summary>
        /// this.DirectEventBus.Publish(evnt);
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnt"></param>
        public void PublishEvent<TEvent>(TEvent evnt) where TEvent : class, IEvent
        {
            this.EventBus.Publish(evnt);
        }

        /// <summary>
        /// this.DirectEventBus.Commit();
        /// </summary>
        public void CommitEventBus()
        {
            this.EventBus.Commit();
        }

        /// <summary>
        /// this.DirectCommandBus.Publish(command);
        /// this.DirectCommandBus.Commit();
        /// </summary>
        /// <param name="command"></param>
        public void Handle(ISysCommand command)
        {
            this.CommandBus.Publish(command);
            this.CommandBus.Commit();
        }

        /// <summary>
        /// Retrieves the service of type <c>T</c> from the provider.
        /// If the service cannot be found, this method returns <c>null</c>.
        /// </summary>
        public T GetService<T>()
        {
            return (T)this.Container.GetService(typeof(T));
        }

        /// <summary>
        /// Retrieves the service of type <c>T</c> from the provider.
        /// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
        /// </summary>
        public T GetRequiredService<T>()
        {
            return (T)GetRequiredService(typeof(T));
        }

        /// <summary>
        /// Retrieves the service of type <paramref name="serviceType"/> from the provider.
        /// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
        /// </summary>
        public object GetRequiredService(Type serviceType)
        {
            object service = this.Container.GetService(serviceType);
            if (service == null)
                throw new ServiceNotFoundException(serviceType);
            return service;
        }

        #region SignIn
        public virtual void SignIn(string loginName, string password, string rememberMe)
        {
            var passwordEncryptionService = GetRequiredService<IPasswordEncryptionService>();
            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(password))
            {
                throw new ValidationException("用户名和密码不能为空");
            }
            if (this.User.Principal.Identity.IsAuthenticated)
            {
                return;
            }
            var addVisitingLogCommand = new AddVisitingLogCommand
            {
                Id = Guid.NewGuid(),
                IPAddress = GetClientIP(),
                LoginName = loginName,
                VisitedOn = null,
                VisitOn = DateTime.Now,
                Description = "登录成功",
                ReasonPhrase = VisitState.LogOnFail.ToName(),
                StateCode = (int)VisitState.LogOnFail
            };
            password = passwordEncryptionService.Encrypt(password);
            var account = GetAccountByLoginName(loginName);
            if (account == null)
            {
                addVisitingLogCommand.Description = "用户名错误";
                MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                throw new ValidationException(addVisitingLogCommand.Description);
            }
            else
            {
                addVisitingLogCommand.AccountID = account.Id;
            }
            if (password != account.Password)
            {
                addVisitingLogCommand.Description = "密码错误";
                MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                throw new ValidationException(addVisitingLogCommand.Description);
            }
            if (account.IsEnabled == 0)
            {
                addVisitingLogCommand.Description = "对不起，该账户已被禁用";
                MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                throw new ValidationException(addVisitingLogCommand.Description);
            }
            string auditState = account.AuditState == null ? account.AuditState : account.AuditState.ToLower();
            DicState dic;
            if (!DicSet.TryGetDic("auditStatus", out dic))
            {
                throw new CoreException("意外的字典编码auditStatus");
            }
            var auditStatusDic = DicSet.GetDicItems(dic);
            if (!auditStatusDic.ContainsKey(auditState))
            {
                auditState = null;
            }
            if (auditState == null
                || auditState == "notaudit")
            {
                addVisitingLogCommand.Description = "对不起，该账户尚未审核";
                MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                throw new ValidationException(addVisitingLogCommand.Description);
            }
            if (auditState != null && auditState == "auditnotpass")
            {
                addVisitingLogCommand.Description = "对不起，该账户未通过审核";
                MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                throw new ValidationException(addVisitingLogCommand.Description);
            }
            if (account.AllowStartTime.HasValue && SystemTime.Now() < account.AllowStartTime.Value)
            {
                addVisitingLogCommand.Description = "对不起，该账户的允许登录开始时间还没到。请在" + account.AllowStartTime.ToString() + "后登录";
                MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                throw new ValidationException(addVisitingLogCommand.Description);
            }
            if (account.AllowEndTime.HasValue && SystemTime.Now() > account.AllowEndTime.Value)
            {
                addVisitingLogCommand.Description = "对不起，该账户的允许登录时间已经过期";
                MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                throw new ValidationException(addVisitingLogCommand.Description);
            }
            if (account.LockEndTime.HasValue || account.LockStartTime.HasValue)
            {
                DateTime lockStartTime = account.LockStartTime ?? DateTime.MinValue;
                DateTime lockEndTime = account.LockEndTime ?? DateTime.MaxValue;
                if (SystemTime.Now() > lockStartTime && SystemTime.Now() < lockEndTime)
                {
                    addVisitingLogCommand.Description = "对不起，该账户暂被锁定";
                    MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                    throw new ValidationException(addVisitingLogCommand.Description);
                }
            }

            if (account.PreviousLoginOn.HasValue && account.PreviousLoginOn.Value >= SystemTime.Now())
            {
                addVisitingLogCommand.Description = "检测到您的上次登录时间在未来。这可能是因为本站点服务器的时间落后导致的，请联系管理员。";
                MessageDispatcher.DispatchMessage(addVisitingLogCommand);
                throw new ValidationException(addVisitingLogCommand.Description);
            }
            account.PreviousLoginOn = SystemTime.Now();
            if (!account.FirstLoginOn.HasValue)
            {
                account.FirstLoginOn = SystemTime.Now();
            }
            account.LoginCount = (account.LoginCount ?? 0) + 1;
            account.IPAddress = GetClientIP();

            var accountPrivileges = GetAccountPrivileges(account.Id);
            var userSession = CreateSession(AccountState.Create(account), accountPrivileges, Guid.NewGuid());
            this.User = userSession;
            if (HttpContext.Current != null)
            {
                bool createPersistentCookie = "rememberMe".Equals(rememberMe, StringComparison.OrdinalIgnoreCase);
                FormsAuthentication.SetAuthCookie(account.LoginName, createPersistentCookie);
                HttpContext.Current.User = userSession.Principal;
            }
            else
            {
                Thread.CurrentPrincipal = userSession.Principal;
            }
            Guid? visitingLogID = Guid.NewGuid();
            this.User.SetData("UserContext_Current_VisitingLogID", visitingLogID);
            EventBus.Publish(new AccountLoginedEvent(account));
            EventBus.Commit();
            addVisitingLogCommand.StateCode = (int)VisitState.Logged;
            addVisitingLogCommand.ReasonPhrase = VisitState.Logged.ToName();
            addVisitingLogCommand.Description = "登录成功";
            MessageDispatcher.DispatchMessage(addVisitingLogCommand);
        }
        #endregion

        #region SignOut
        public virtual void SignOut()
        {
            var userSessionStorage = GetRequiredService<IUserSessionStorage>();
            if (!this.User.Principal.Identity.IsAuthenticated)
            {
                DeleteSession(this.User.Worker.Id);
                return;
            }
            if (this.User.Worker.Id == Guid.Empty)
            {
                Thread.CurrentPrincipal = new AnycmdPrincipal(this, new AnycmdIdentity("Anycmd", false, string.Empty));
                DeleteSession(this.User.Worker.Id);
                return;
            }
            if (HttpContext.Current != null)
            {
                FormsAuthentication.SignOut();
            }
            else
            {
                Thread.CurrentPrincipal = new AnycmdPrincipal(this, new AnycmdIdentity("Anycmd", false, string.Empty));
            }
            var entity = this.GetAccountByID(this.User.Worker.Id);
            DeleteSession(this.User.Worker.Id);
            if (entity != null)
            {
                EventBus.Publish(new AccountLogoutedEvent(entity));
                EventBus.Commit();
            }
        }
        #endregion

        #region CreateSession
        /// <summary>
        /// 创建AC会话
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public IUserSession CreateSession(AccountState worker, List<PrivilegeBigramState> accountPrivileges, Guid sessionID)
        {
            var principal = new AnycmdPrincipal(this, new AnycmdIdentity("Anycmd", true, worker.LoginName));
            IUserSession user = new UserSessionState(this, principal, worker, accountPrivileges);

            return user;
        }
        #endregion

        #region DeleteSession
        /// <summary>
        /// 删除会话
        /// </summary>
        /// <param name="sessionID"></param>
        public void DeleteSession(Guid sessionID)
        {
            var userSessionStorage = GetRequiredService<IUserSessionStorage>();
            userSessionStorage.Clear();
        }
        #endregion

        private List<PrivilegeBigramState> GetAccountPrivileges(Guid accountID)
        {
            var subjectType = ACSubjectType.Account.ToName();
            var accountPrivileges = GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().Where(a => a.SubjectType == subjectType && a.SubjectInstanceID == accountID).ToList().Select(a => PrivilegeBigramState.Create(a)).ToList();
            return accountPrivileges;
        }

        #region AccountQuery
        // TODO:提取配置
        private readonly Guid dbID = new Guid("67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB");
        private RdbDescriptor _db;

        protected RdbDescriptor Db
        {
            get
            {
                if (_db == null)
                {
                    if (!Rdbs.TryDb(dbID, out _db))
                    {
                        throw new CoreException("意外的数据库标识" + dbID);
                    }
                }
                return _db;
            }
        }

        protected internal virtual Account GetAccountByLoginName(string loginName)
        {
            using (var conn = Db.GetConnection())
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                return conn.Query<Account>("select * from [Account] where LoginName=@LoginName", new { LoginName = loginName }).FirstOrDefault();
            }
        }

        protected internal virtual Account GetAccountByID(Guid accountID)
        {
            using (var conn = Db.GetConnection())
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                return conn.Query<Account>("select * from [Account] where Id=@ContractorID", new { ContractorID = accountID }).FirstOrDefault();
            }
        }
        #endregion

        private string GetClientIP()
        {
            if (HttpContext.Current == null)
            {
                return IPAddress.Loopback.ToString();
            }
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == ip || ip == String.Empty)
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == ip || ip == String.Empty)
            {
                ip = HttpContext.Current.Request.UserHostAddress;
            }
            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }

            return ip;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
