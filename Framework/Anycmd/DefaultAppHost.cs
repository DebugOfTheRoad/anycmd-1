
namespace Anycmd
{
    using Bus;
    using Bus.DirectBus;
    using Container;
    using Host;
    using Host.AC;
    using Host.AC.Identity;
    using Host.AC.Infra;
    using Host.AC.MemorySets.Impl;
    using Host.AC.MessageHandlers;
    using Host.Rdb;
    using Logging;
    using Query;
    using Rdb;

    /// <summary>
    /// 系统实体宿主。
    /// </summary>
    public partial class DefaultAppHost : AppHost
    {
        public DefaultAppHost()
        {
            base.MessageDispatcher = new MessageDispatcher();
            base.CommandBus = new DirectCommandBus(this.MessageDispatcher);
            this.EventBus = new DirectEventBus(this.MessageDispatcher);

            base.Rdbs = new Rdbs(this);
            base.DbTables = new DbTables(this);
            base.DbViews = new DbViews(this);
            base.DbTableColumns = new DbTableColumns(this);
            base.DbViewColumns = new DbViewColumns(this);
            base.AppSystemSet = new AppSystemSet(this);
            base.ButtonSet = new ButtonSet(this);
            base.SysUsers = new SysUserSet(this);
            base.DicSet = new DicSet(this);
            base.EntityTypeSet = new EntityTypeSet(this);
            base.FunctionSet = new FunctionSet(this);
            base.OrganizationSet = new OrganizationSet(this);
            base.PageSet = new PageSet(this);
            base.ResourceSet = new ResourceTypeSet(this);
            base.PrivilegeSet = new PrivilegeSet(this);
            base.MenuSet = new MenuSet(this);
            base.RoleSet = new RoleSet(this);
            base.GroupSet = new GroupSet(this);
        }

        public override void Configure(AnycmdServiceContainer container)
        {
            this.AddDefaultService<IAppHostBootstrap>(container, new FastRdbAppHostBootstrap(this));
            this.AddDefaultService<IRdbMetaDataService>(container, new SQLServerMetaDataService());
            this.AddDefaultService<ISqlFilterStringBuilder>(container, new SqlFilterStringBuilder());
            this.AddDefaultService<ISecurityService>(container, new DefaultSecurityService());
            this.AddDefaultService<IPasswordEncryptionService>(container, new PasswordEncryptionService(this));

            base.MessageDispatcher.Register(new AccountLoginedEventHandler(this));
            base.MessageDispatcher.Register(new AccountLogoutedEventHandler(this));
            base.MessageDispatcher.Register(new AddVisitingLogCommandHandler(this));
            base.MessageDispatcher.Register(new AddAccountCommandHandler(this));
            base.MessageDispatcher.Register(new UpdateAccountCommandHandler(this));
            base.MessageDispatcher.Register(new RemoveAccountCommandHandler(this));
            base.MessageDispatcher.Register(new AddPasswordCommandHandler(this));
            base.MessageDispatcher.Register(new ChangePasswordCommandHandler(this));
            base.MessageDispatcher.Register(new SaveHelpCommandHandler(this));

            this.MessageDispatcher.Register(new OperatedEventHandler(this));

            base.Map(EntityTypeMap.Create<RDatabase>("AC"));
            base.Map(EntityTypeMap.Create<DbTable>("AC"));
            base.Map(EntityTypeMap.Create<DbView>("AC"));
            base.Map(EntityTypeMap.Create<DbTableColumn>("AC"));
            base.Map(EntityTypeMap.Create<DbViewColumn>("AC"));
            base.Map(EntityTypeMap.Create<DbTableSpace>("AC", "TableSpace"));
            base.Map(EntityTypeMap.Create<ExceptionLog>("AC"));
            base.Map(EntityTypeMap.Create<Parameter>("AC"));
            base.Map(EntityTypeMap.Create<OperationLog>("AC"));
            base.Map(EntityTypeMap.Create<OperationHelp>("AC", "Help"));
            base.Map(EntityTypeMap.Create<AnyLog>("AC"));

            base.Map(EntityTypeMap.Create<AppSystem>("AC"));
            base.Map(EntityTypeMap.Create<Button>("AC"));
            base.Map(EntityTypeMap.Create<Dic>("AC"));
            base.Map(EntityTypeMap.Create<DicItem>("AC"));
            base.Map(EntityTypeMap.Create<EntityType>("AC"));
            base.Map(EntityTypeMap.Create<Function>("AC"));
            base.Map(EntityTypeMap.Create<Menu>("AC"));
            base.Map(EntityTypeMap.Create<OperationHelp>("AC"));
            base.Map(EntityTypeMap.Create<Organization>("AC"));
            base.Map(EntityTypeMap.Create<Page>("AC"));
            base.Map(EntityTypeMap.Create<PageButton>("AC"));
            base.Map(EntityTypeMap.Create<Property>("AC"));
            base.Map(EntityTypeMap.Create<ResourceType>("AC"));

            base.Map(EntityTypeMap.Create<Account>("AC"));
            base.Map(EntityTypeMap.Create<DeveloperID>("AC"));
            base.Map(EntityTypeMap.Create<VisitingLog>("AC"));

            base.Map(EntityTypeMap.Create<Group>("AC"));
            base.Map(EntityTypeMap.Create<Role>("AC"));
            base.Map(EntityTypeMap.Create<PrivilegeBigram>("AC"));
        }

        private void AddDefaultService<T>(AnycmdServiceContainer container, object service)
        {
            if (container.GetService(typeof(T)) == null)
            {
                container.AddService(typeof(T), service);
            }
        }
    }
}
