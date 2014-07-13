
namespace Anycmd
{
    using Bus;
    using Commands;
    using Container;
    using Events;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Identity;
    using Host.AC.Infra;
    using Logging;
    using Rdb;
    using ServiceStack.Text;
    using System;
    using System.Collections.Generic;
    using System.IO;

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

        public IUserSession UserSession { get; protected set; }

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
            return JsonSerializer.DeserializeFromString<T>(value);
        }

        public string SerializeToString<T>(T value)
        {
            return JsonSerializer.SerializeToString<T>(value);
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

        public void PublishOperatedEvent(Guid targetID)
        {
            if (this.Config.EnableOperationLog)
            {
                var storage = this.GetRequiredService<IOperationLogStorage>();
                if (storage != null)
                {
                    var logEntity = storage.Get();
                    if (logEntity != null)
                    {
                        logEntity.TargetID = targetID;
                        this.EventBus.Publish(new OperatedEvent(logEntity)
                        {
                            OperatedOn = DateTime.Now
                        });
                        storage.Remove();
                    }
                }
            }
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

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
