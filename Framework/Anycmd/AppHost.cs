
namespace Anycmd
{
    using AC;
    using Bus;
    using Commands;
    using Container;
    using Dapper;
    using EDI;
    using Events;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Identity;
    using Host.AC.Identity.Messages;
    using Host.AC.Infra;
    using Host.AC.MemorySets;
    using Host.EDI;
    using Host.EDI.Handlers;
    using Host.EDI.Hecp;
    using Host.EDI.MemorySets;
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
    public abstract class AppHost : AnycmdServiceContainer, IAppHost
    {
        public static readonly IAppHost Empty = new EmptyAppHost();

        private static object locker = new object();
        private bool _pluginsLoaded;

        private readonly string _buildInPluginsBaseDirectory;
        private readonly Guid _id = Guid.NewGuid();

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

        protected AppHost()
        {
            lock (locker)
            {
                this.Name = "DefaultAppHost";
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                bool isASPNET =
                    !AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.EndsWith("dll.config", StringComparison.OrdinalIgnoreCase)
                    && !AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.EndsWith("exe.config", StringComparison.OrdinalIgnoreCase);
                _buildInPluginsBaseDirectory = isASPNET ? Path.Combine(dir, "Bin", "Plugins") : Path.Combine(dir, "Plugins");
                this.StartedAt = DateTime.UtcNow;
                this.StateCodes = new StateCodes(this);
                this.PreHecpRequestFilters = new List<Func<HecpContext, ProcessResult>>();
                this.GlobalEDIMessageHandingFilters = new List<Func<MessageContext, ProcessResult>>();
                this.GlobalEDIMessageHandledFilters = new List<Func<MessageContext, ProcessResult>>();
                this.GlobalHecpResponseFilters = new List<Func<HecpContext, ProcessResult>>();
                this.Plugins = new List<IPlugin>
                {
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual AppHost Init()
        {
            OnConfigLoad();

            Configure();

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

        public IUserSession UserSession
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
                        // 使用账户标识作为会话标识会导致一个账户只有一个会话
                        // TODO:支持账户和会话的一对多，为会话级的动态责任分离做准备
                        user = CreateSession(account.Id, AccountState.Create(account), accountPrivileges);
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

        private ILoggingService _loggingService;
        public ILoggingService LoggingService
        {
            get
            {
                if (_loggingService == null)
                {
                    _loggingService = GetRequiredService<ILoggingService>();
                }
                return _loggingService;
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
        public IResourceTypeSet ResourceTypeSet { get; protected set; }

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
        public abstract void Configure();

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

            LoadPlugin(Plugins.ToArray());
            _pluginsLoaded = true;

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
            return (T)this.GetService(typeof(T));
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
            object service = this.GetService(serviceType);
            if (service == null)
                throw new ServiceNotFoundException(serviceType);
            return service;
        }

        #region SignIn
        public virtual void SignIn(string loginName, string password, string rememberMe)
        {
            var passwordEncryptionService = GetRequiredService<IPasswordEncryptionService>();
            var userSessionRepository = GetRequiredService<IRepository<UserSession>>();
            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(password))
            {
                throw new ValidationException("用户名和密码不能为空");
            }
            if (this.UserSession.Principal.Identity.IsAuthenticated)
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

            if (account.PreviousLoginOn.HasValue && account.PreviousLoginOn.Value >= SystemTime.Now().AddMinutes(5))
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
            // 使用账户标识作为会话标识会导致一个账户只有一个会话
            // TODO:支持账户和会话的一对多，为会话级的动态责任分离做准备
            var sessionEntity = userSessionRepository.GetByKey(account.Id);
            IUserSession userSession;
            if (sessionEntity != null)
            {
                var principal = new AnycmdPrincipal(this, new AnycmdIdentity(sessionEntity.AuthenticationType, true, sessionEntity.LoginName));
                userSession = new UserSessionState(this, sessionEntity.Id, principal, AccountState.Create(account), accountPrivileges);
                sessionEntity.IsAuthenticated = true;
                userSessionRepository.Update(sessionEntity);
            }
            else
            {
                userSession = CreateSession(account.Id, AccountState.Create(account), accountPrivileges);
            }
            this.UserSession = userSession;
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
            this.UserSession.SetData("UserContext_Current_VisitingLogID", visitingLogID);
            userSessionRepository.Context.Commit();
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
            if (!this.UserSession.Principal.Identity.IsAuthenticated)
            {
                DeleteSession(this.UserSession.Worker.Id);
                return;
            }
            if (this.UserSession.Worker.Id == Guid.Empty)
            {
                Thread.CurrentPrincipal = new AnycmdPrincipal(this, new UnauthenticatedIdentity());
                DeleteSession(this.UserSession.Worker.Id);
                return;
            }
            if (HttpContext.Current != null)
            {
                FormsAuthentication.SignOut();
            }
            else
            {
                Thread.CurrentPrincipal = new AnycmdPrincipal(this, new UnauthenticatedIdentity());
            }
            userSessionStorage.Clear();
            OnSignOuted(this.UserSession.Id);
            var entity = this.GetAccountByID(this.UserSession.Worker.Id);
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
        /// <param name="sessionID">会话标识。会话级的权限依赖于会话的持久跟踪</param>
        /// <returns></returns>
        public IUserSession CreateSession(Guid sessionID, AccountState worker, List<PrivilegeBigramState> accountPrivileges)
        {
            var principal = new AnycmdPrincipal(this, new AnycmdIdentity("Anycmd", true, worker.LoginName));
            IUserSession user = new UserSessionState(this, sessionID, principal, worker, accountPrivileges);
            // TODO:持久化UserSession
            return user;
        }
        #endregion

        #region DeleteSession
        /// <summary>
        /// 删除会话
        /// <remarks>
        /// 会话不应该经常删除，会话级的权限依赖于会话的持久跟踪。用户退出系统只需要清空该用户的内存会话记录和更新数据库中的会话记录为IsAuthenticated为false而不需要删除Session。
        /// </remarks>
        /// </summary>
        /// <param name="sessionID"></param>
        public void DeleteSession(Guid sessionID)
        {
            // TODO:删除数据库中相应的会话记录（在UserSession表）
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

        protected internal virtual void OnSignOuted(Guid sessionID)
        {
            using (var conn = Db.GetConnection())
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                conn.Execute("update UserSession set IsAuthenticated=@IsAuthenticated where Id=@Id", new { IsAuthenticated = false, Id = sessionID });
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

        /// <summary>
        /// 管道插件
        /// </summary>
        public List<IPlugin> Plugins { get; protected set; }

        public StateCodes StateCodes { get; private set; }

        /// <summary>
        /// 本节点数据交换进程上下文。进程列表。
        /// </summary>
        public IProcesseSet Processs { get; protected set; }

        /// <summary>
        /// 节点上下文
        /// </summary>
        public INodeSet Nodes { get; protected set; }

        /// <summary>
        /// 信息字典上下文
        /// </summary>
        public IInfoDicSet InfoDics { get; protected set; }

        /// <summary>
        /// 本体上下文
        /// </summary>
        public IOntologySet Ontologies { get; protected set; }

        public HecpHandler HecpHandler { get; protected set; }

        /// <summary>
        /// 信息字符串转化器上下文
        /// </summary>
        public IInfoStringConverterSet InfoStringConverters { get; protected set; }

        /// <summary>
        /// 信息项验证器上下文
        /// </summary>
        public IInfoRuleSet InfoRules { get; protected set; }

        /// <summary>
        /// 命令提供程序上下文
        /// </summary>
        public IMessageProviderSet MessageProviders { get; protected set; }

        /// <summary>
        /// 命令生产者
        /// </summary>
        public IMessageProducer MessageProducer { get; protected set; }

        /// <summary>
        /// 数据提供程序上下文
        /// </summary>
        public IEntityProviderSet EntityProviders { get; protected set; }

        /// <summary>
        /// 命令转移器上下文
        /// </summary>
        public IMessageTransferSet Transfers { get; protected set; }

        /// <summary>
        /// 添加请求过滤器, 这些过滤器在Http请求被转化为Hecp请求后应用
        /// </summary>
        public List<Func<HecpContext, ProcessResult>> PreHecpRequestFilters { get; protected set; }

        /// <summary>
        /// 添加命令过滤器。这些过滤器在Command验证通过但被处理前应用
        /// </summary>
        public List<Func<MessageContext, ProcessResult>> GlobalEDIMessageHandingFilters { get; protected set; }

        /// <summary>
        /// 添加命令过滤器。这些过滤器在Command验证通过并被处理后应用
        /// </summary>
        public List<Func<MessageContext, ProcessResult>> GlobalEDIMessageHandledFilters { get; protected set; }

        /// <summary>
        /// 添加响应过滤器。这些过滤器在Hecp响应末段应用
        /// </summary>
        public List<Func<HecpContext, ProcessResult>> GlobalHecpResponseFilters { get; protected set; }

        #region GetPluginBaseDirectory
        /// <summary>
        /// 根据插件类型获取域内插件地址
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public virtual string GetPluginBaseDirectory(PluginType pluginType)
        {
            switch (pluginType)
            {
                case PluginType.Plugin:
                    return this.BuildInPluginsBaseDirectory;
                case PluginType.MessageProvider:
                    return Path.Combine(this.BuildInPluginsBaseDirectory, "MessageProviders");
                case PluginType.EntityProvider:
                    return Path.Combine(this.BuildInPluginsBaseDirectory, "EntityProviders");
                case PluginType.InfoStringConverter:
                    return Path.Combine(this.BuildInPluginsBaseDirectory, "InfoStringConverters");
                case PluginType.InfoConstraint:
                    return Path.Combine(this.BuildInPluginsBaseDirectory, "InfoConstraints");
                case PluginType.MessageTransfer:
                    return Path.Combine(this.BuildInPluginsBaseDirectory, "MessageTransfers");
                default:
                    throw new CoreException("意外的插件类型");
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugins"></param>
        public virtual void LoadPlugin(params IPlugin[] plugins)
        {
            foreach (var plugin in plugins)
            {
                try
                {
                    plugin.Register(this);
                }
                catch (Exception ex)
                {
                    LoggingService.Error("Error loading plugin " + plugin.GetType().Name, ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugins"></param>
        public void AddPlugin(params IPlugin[] plugins)
        {
            if (_pluginsLoaded)
            {
                LoadPlugin(plugins);
            }
            else
            {
                foreach (var plugin in plugins)
                {
                    Plugins.Add(plugin);
                }
            }
        }

        /// <summary>
        /// 应用Hecp管道过滤器，通过返回结果表达当前Hecp请求是否被处理过了，如果处理过了则就转到响应流程了。
        /// </summary>
        /// <returns></returns>
        public ProcessResult ApplyPreHecpRequestFilters(HecpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var result = new ProcessResult(context.Response.IsSuccess, context.Response.Body.Event.Status, context.Response.Body.Event.Description);

            foreach (var requestFilter in PreHecpRequestFilters)
            {
                result = requestFilter(context);
                if (context.Response.IsClosed) break;
            }

            return result;
        }

        /// <summary>
        /// 应用Command管道过滤器，通过返回结果表达当前Command请求是否被处理过了，如果处理过了则就转到响应流程了。
        /// </summary>
        /// <returns></returns>
        public ProcessResult ApplyEDIMessageHandingFilters(MessageContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var result = new ProcessResult(context.Result.IsSuccess, context.Result.Status, context.Result.Description);

            // 执行全局命令过滤器
            foreach (var processedFilter in GlobalEDIMessageHandingFilters)
            {
                result = processedFilter(context);
                if (context.Result.IsClosed) break; ;
            }

            return result;
        }

        /// <summary>
        /// 应用Command管道过滤器，通过返回结果表达当前Command请求是否被处理过了，如果处理过了则就转到响应流程了。
        /// </summary>
        /// <returns></returns>
        public ProcessResult ApplyEDIMessageHandledFilters(MessageContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var result = new ProcessResult(context.Result.IsSuccess, context.Result.Status, context.Result.Description);

            // 执行全局命令过滤器
            foreach (var processedFilter in GlobalEDIMessageHandledFilters)
            {
                result = processedFilter(context);
                if (context.Result.IsClosed) break; ;
            }

            return result;
        }

        /// <summary>
        /// 应用Hecp管道过滤器，通过返回结果表达当前Hecp请求是否被处理过了，如果处理过了则就转到响应流程了。
        /// </summary>
        /// <returns></returns>
        public ProcessResult ApplyHecpResponseFilters(HecpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var result = new ProcessResult(context.Response.IsSuccess, context.Response.Body.Event.Status, context.Response.Body.Event.Description);

            //Exec global filters
            foreach (var responseFilter in GlobalHecpResponseFilters)
            {
                result = responseFilter(context);
                if (context.Response.IsClosed) break;
            }

            return result;
        }

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

        #region EmptyAppHost
        private class EmptyAppHost : AnycmdServiceContainer, IAppHost
        {
            public IAppSystemSet AppSystemSet {
                get { return Host.AC.MemorySets.Impl.AppSystemSet.Empty; }
            }

            public IButtonSet ButtonSet
            {
                get { return Host.AC.MemorySets.Impl.ButtonSet.Empty; }
            }

            public ICommandBus CommandBus
            {
                get { return EmptyCommandBus.Empty; }
            }

            public IAppConfig Config
            {
                get
                {
                    return EmptyAppConfig.Empty;
                }
                set
                {
                    
                }
            }

            public IUserSession CreateSession(Guid sessionID, AccountState worker, List<PrivilegeBigramState> accountPrivileges)
            {
                return UserSessionState.Empty;
            }

            public IDbTableColumns DbTableColumns
            {
                get { return Host.Rdb.DbTableColumns.Empty; }
            }

            public IDbTables DbTables
            {
                get { return Host.Rdb.DbTables.Empty; }
            }

            public IDbViewColumns DbViewColumns
            {
                get { return Host.Rdb.DbViewColumns.Empty; }
            }

            public IDbViews DbViews
            {
                get { return Host.Rdb.DbViews.Empty; }
            }

            public void DeleteSession(Guid sessionID)
            {
                
            }

            public IDicSet DicSet
            {
                get { return Host.AC.MemorySets.Impl.DicSet.Empty; }
            }

            public IEntityTypeSet EntityTypeSet
            {
                get { return Host.AC.MemorySets.Impl.EntityTypeSet.Empty; }
            }

            public IEventBus EventBus
            {
                get { return EmptyEventBus.Empty; }
            }

            public IFunctionSet FunctionSet
            {
                get { return Host.AC.MemorySets.Impl.FunctionSet.Empty; }
            }

            public IGroupSet GroupSet
            {
                get { return Host.AC.MemorySets.Impl.GroupSet.Empty; }
            }

            public Guid Id
            {
                get { return Guid.Empty; }
            }

            public IMenuSet MenuSet
            {
                get { return Host.AC.MemorySets.Impl.MenuSet.Empty; }
            }

            public IMessageDispatcher MessageDispatcher
            {
                get { return EmptyMessageDispatcher.Empty; }
            }

            public string Name
            {
                get { return "EmptyAppHost"; }
            }

            public IOrganizationSet OrganizationSet
            {
                get { return Host.AC.MemorySets.Impl.OrganizationSet.Empty; }
            }

            public IPageSet PageSet
            {
                get { return Host.AC.MemorySets.Impl.PageSet.Empty; }
            }

            public IPrivilegeSet PrivilegeSet
            {
                get { return Host.AC.MemorySets.Impl.PrivilegeSet.Empty; }
            }

            public IRdbs Rdbs
            {
                get { return Host.Rdb.Rdbs.Empty; }
            }

            public IResourceTypeSet ResourceTypeSet
            {
                get { return Host.AC.MemorySets.Impl.ResourceTypeSet.Empty; }
            }

            public IRoleSet RoleSet
            {
                get { return Host.AC.MemorySets.Impl.RoleSet.Empty; }
            }

            public void SignIn(string loginName, string password, string rememberMe)
            {
                
            }

            public void SignOut()
            {
                
            }

            public ISysUserSet SysUsers
            {
                get { return Host.AC.MemorySets.Impl.SysUserSet.Empty; }
            }

            public IUserSession UserSession
            {
                get { return UserSessionState.Empty; }
            }

            public string BuildInPluginsBaseDirectory
            {
                get { return string.Empty; }
            }

            public ILoggingService LoggingService
            {
                get { return EmptyLoggingService.Instance; }
            }

            public List<IPlugin> Plugins
            {
                get { return new List<IPlugin>(); }
            }

            public StateCodes StateCodes
            {
                get
                {
                    return Host.EDI.StateCodes.Empty;
                }
            }

            public IProcesseSet Processs
            {
                get { throw new NotImplementedException(); }
            }

            public INodeSet Nodes
            {
                get { throw new NotImplementedException(); }
            }

            public IInfoDicSet InfoDics
            {
                get { throw new NotImplementedException(); }
            }

            public IOntologySet Ontologies
            {
                get { throw new NotImplementedException(); }
            }

            public IInfoStringConverterSet InfoStringConverters
            {
                get { throw new NotImplementedException(); }
            }

            public IInfoRuleSet InfoRules
            {
                get { throw new NotImplementedException(); }
            }

            public IMessageProviderSet MessageProviders
            {
                get { throw new NotImplementedException(); }
            }

            public IEntityProviderSet EntityProviders
            {
                get { throw new NotImplementedException(); }
            }

            public IMessageProducer MessageProducer
            {
                get { throw new NotImplementedException(); }
            }

            public IMessageTransferSet Transfers
            {
                get { throw new NotImplementedException(); }
            }

            public HecpHandler HecpHandler
            {
                get
                {
                    return new HecpHandler(this);
                }

            }

            public List<Func<HecpContext, ProcessResult>> PreHecpRequestFilters
            {
                get { return new List<Func<HecpContext,ProcessResult>>(); }
            }

            public List<Func<MessageContext, ProcessResult>> GlobalEDIMessageHandingFilters
            {
                get { return new List<Func<MessageContext,ProcessResult>>(); }
            }

            public List<Func<MessageContext, ProcessResult>> GlobalEDIMessageHandledFilters
            {
                get { return new List<Func<MessageContext,ProcessResult>>(); }
            }

            public List<Func<HecpContext, ProcessResult>> GlobalHecpResponseFilters
            {
                get { return new List<Func<HecpContext,ProcessResult>>(); }
            }

            public string GetPluginBaseDirectory(PluginType pluginType)
            {
                return string.Empty;
            }

            public ProcessResult ApplyPreHecpRequestFilters(HecpContext context)
            {
                return ProcessResult.Ok;
            }

            public ProcessResult ApplyEDIMessageHandingFilters(MessageContext context)
            {
                return ProcessResult.Ok;
            }

            public ProcessResult ApplyEDIMessageHandledFilters(MessageContext context)
            {
                return ProcessResult.Ok;
            }

            public ProcessResult ApplyHecpResponseFilters(HecpContext context)
            {
                return ProcessResult.Ok;
            }

            private class EmptyAppConfig : IAppConfig
            {
                public static readonly IAppConfig Empty = new EmptyAppConfig();

                public bool EnableClientCache
                {
                    get { return false; }
                }

                public bool EnableOperationLog
                {
                    get { return false; }
                }

                public IReadOnlyCollection<IParameter> Parameters
                {
                    get { return new List<IParameter>(); }
                }

                public string SelfAppSystemCode
                {
                    get { return string.Empty; }
                }

                public string SqlServerTableColumnsSelect
                {
                    get { return string.Empty; }
                }

                public string SqlServerTablesSelect
                {
                    get { return string.Empty; }
                }

                public string SqlServerViewColumnsSelect
                {
                    get { return string.Empty; }
                }

                public string SqlServerViewsSelect
                {
                    get { return string.Empty; }
                }

                public int TicksTimeout
                {
                    get { return 0; }
                }


                public string InfoFormat
                {
                    get { return string.Empty; }
                }

                public string EntityArchivePath
                {
                    get { return string.Empty; }
                }

                public string EntityBackupPath
                {
                    get { return string.Empty; }
                }

                public bool ServiceIsAlive
                {
                    get { return false; }
                }

                public bool TraceIsEnabled
                {
                    get { return false; }
                }

                public int BeatPeriod
                {
                    get { return int.MaxValue; }
                }

                public string CenterNodeID
                {
                    get { return string.Empty; }
                }

                public string ThisNodeID
                {
                    get { return string.Empty; }
                }

                public ConfigLevel AuditLevel
                {
                    get { return ConfigLevel.Invalid; }
                }

                public AuditType ImplicitAudit
                {
                    get { return AuditType.Invalid; }
                }

                public ConfigLevel ACLLevel
                {
                    get { return ConfigLevel.Invalid; }
                }

                public AllowType ImplicitAllow
                {
                    get { return AllowType.Invalid; }
                }

                public ConfigLevel EntityLogonLevel
                {
                    get { return ConfigLevel.Invalid; }
                }

                public EntityLogon ImplicitEntityLogon
                {
                    get { return EntityLogon.Invalid; }
                }
            }

            private class EmptyLoggingService : ILoggingService
            {
                public static readonly ILoggingService Instance = new EmptyLoggingService();

                public void Log(IAnyLog anyLog)
                {
                    
                }

                public void Log(IAnyLog[] anyLogs)
                {
                    
                }

                public IAnyLog Get(Guid id)
                {
                    return new AnyLog(id)
                    {
                    };
                }

                public IList<IAnyLog> GetPlistAnyLogs(List<Query.FilterData> filters, Query.PagingInput paging)
                {
                    return new List<IAnyLog>();
                }

                public IList<OperationLog> GetPlistOperationLogs(Guid? targetID, DateTime? leftCreateOn, DateTime? rightCreateOn, List<Query.FilterData> filters, Query.PagingInput paging)
                {
                    return new List<OperationLog>();
                }

                public IList<ExceptionLog> GetPlistExceptionLogs(List<Query.FilterData> filters, Query.PagingInput paging)
                {
                    return new List<ExceptionLog>();
                }

                public void ClearAnyLog()
                {
                    
                }

                public void ClearExceptionLog()
                {
                    
                }

                public void Debug(object message)
                {
                    
                }

                public void DebugFormatted(string format, params object[] args)
                {
                    
                }

                public void Info(object message)
                {
                    
                }

                public void InfoFormatted(string format, params object[] args)
                {
                    
                }

                public void Warn(object message)
                {
                    
                }

                public void Warn(object message, Exception exception)
                {
                    
                }

                public void WarnFormatted(string format, params object[] args)
                {
                    
                }

                public void Error(object message)
                {
                    
                }

                public void Error(object message, Exception exception)
                {
                    
                }

                public void ErrorFormatted(string format, params object[] args)
                {
                    
                }

                public void Fatal(object message)
                {
                    
                }

                public void Fatal(object message, Exception exception)
                {
                    
                }

                public void FatalFormatted(string format, params object[] args)
                {
                    
                }

                public bool IsDebugEnabled
                {
                    get { return false; }
                }

                public bool IsInfoEnabled
                {
                    get { return false; }
                }

                public bool IsWarnEnabled
                {
                    get { return false; }
                }

                public bool IsErrorEnabled
                {
                    get { return false; }
                }

                public bool IsFatalEnabled
                {
                    get { return false; }
                }
            }

            private class EmptyCommandBus : ICommandBus
            {
                public static readonly ICommandBus Empty = new EmptyCommandBus();

                public void Publish<TMessage>(TMessage message) where TMessage : Bus.IMessage
                {
                    
                }

                public void Publish<TMessage>(IEnumerable<TMessage> messages) where TMessage : Bus.IMessage
                {
                    
                }

                public void Clear()
                {
                    
                }

                public bool DistributedTransactionSupported
                {
                    get { return false; }
                }

                public bool Committed
                {
                    get { return true; }
                }

                public void Commit()
                {
                    
                }

                public void Rollback()
                {
                    
                }

                public void Dispose()
                {
                    
                }
            }

            private class EmptyEventBus : IEventBus
            {
                public static readonly IEventBus Empty = new EmptyEventBus();

                public void Publish<TMessage>(TMessage message) where TMessage : Bus.IMessage
                {
                    
                }

                public void Publish<TMessage>(IEnumerable<TMessage> messages) where TMessage : Bus.IMessage
                {
                    
                }

                public void Clear()
                {
                    
                }

                public bool DistributedTransactionSupported
                {
                    get { return false; }
                }

                public bool Committed
                {
                    get { return true; }
                }

                public void Commit()
                {
                    
                }

                public void Rollback()
                {
                    
                }

                public void Dispose()
                {
                    
                }
            }

            private class EmptyMessageDispatcher : IMessageDispatcher
            {
                public static readonly IMessageDispatcher Empty = new EmptyMessageDispatcher();

                public void Clear()
                {
                    
                }

                public void DispatchMessage<T>(T message) where T : Bus.IMessage
                {
                    
                }

                public void Register<T>(IHandler<T> handler) where T : Bus.IMessage
                {
                    
                }

                public void UnRegister<T>(IHandler<T> handler) where T : Bus.IMessage
                {
                    
                }

                public event EventHandler<MessageDispatchEventArgs> Dispatching;

                public event EventHandler<MessageDispatchEventArgs> DispatchFailed;

                public event EventHandler<MessageDispatchEventArgs> Dispatched;
            }
        }
        #endregion
    }
}
