
namespace Anycmd
{
    using Bus;
    using Bus.DirectBus;
    using Host;
    using Host.AC;
    using Host.AC.Identity;
    using Host.AC.Infra;
    using Host.AC.MemorySets.Impl;
    using Host.AC.MessageHandlers;
    using Host.EDI;
    using Host.EDI.Entities;
    using Host.EDI.Handlers;
    using Host.EDI.Hecp;
    using Host.EDI.MemorySets.Impl;
    using Host.EDI.MessageHandlers;
    using Host.Rdb;
    using Logging;
    using Query;
    using Rdb;

    /// <summary>
    /// 系统实体宿主。
    /// </summary>
    public class DefaultAppHost : AppHost
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
            base.ResourceTypeSet = new ResourceTypeSet(this);
            base.PrivilegeSet = new PrivilegeSet(this);
            base.MenuSet = new MenuSet(this);
            base.RoleSet = new RoleSet(this);
            base.GroupSet = new GroupSet(this);

            this.HecpHandler = new HecpHandler(this);
            this.MessageProducer = new DefaultMessageProducer(this);
            this.Ontologies = new OntologySet(this);
            this.Processs = new ProcesseSet(this);
            this.Nodes = new NodeSet(this);
            this.InfoDics = new InfoDicSet(this);
            this.InfoStringConverters = new InfoStringConverterSet(this);
            this.InfoRules = new InfoRuleSet(this);
            this.MessageProviders = new MessageProviderSet(this);
            this.EntityProviders = new EntityProviderSet(this);
            this.Transfers = new MessageTransferSet(this);
        }

        public override void Configure()
        {
            this.AddDefaultService<IAppHostBootstrap>(new FastRdbAppHostBootstrap(this));
            this.AddDefaultService<IRdbMetaDataService>(new SQLServerMetaDataService(this));
            this.AddDefaultService<ISqlFilterStringBuilder>(new SqlFilterStringBuilder());
            this.AddDefaultService<ISecurityService>(new DefaultSecurityService());
            this.AddDefaultService<IPasswordEncryptionService>(new PasswordEncryptionService(this));

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

            this.AddService(typeof(INodeHostBootstrap), new FastNodeHostBootstrap(this));
            this.MessageDispatcher.Register(new AddBatchCommandHandler(this));
            this.MessageDispatcher.Register(new UpdateBatchCommandHandler(this));
            this.MessageDispatcher.Register(new RemoveBatchCommandHandler(this));

            this.Map(EntityTypeMap.Create<Action>("EDI"));
            this.Map(EntityTypeMap.Create<Archive>("EDI"));
            this.Map(EntityTypeMap.Create<Batch>("EDI"));
            this.Map(EntityTypeMap.Create<Element>("EDI"));
            this.Map(EntityTypeMap.Create<InfoDic>("EDI"));
            this.Map(EntityTypeMap.Create<InfoDicItem>("EDI"));
            this.Map(EntityTypeMap.Create<InfoGroup>("EDI"));
            this.Map(EntityTypeMap.Create<InfoRule>("EDI"));
            this.Map(EntityTypeMap.Create<Node>("EDI"));
            this.Map(EntityTypeMap.Create<NodeElementAction>("EDI"));
            this.Map(EntityTypeMap.Create<NodeElementCare>("EDI"));
            this.Map(EntityTypeMap.Create<NodeTopic>("EDI"));
            this.Map(EntityTypeMap.Create<NodeOntologyCare>("EDI"));
            this.Map(EntityTypeMap.Create<NodeOntologyOrganization>("EDI"));
            this.Map(EntityTypeMap.Create<Ontology>("EDI"));
            this.Map(EntityTypeMap.Create<OntologyOrganization>("EDI"));
            this.Map(EntityTypeMap.Create<Plugin>("EDI"));
            this.Map(EntityTypeMap.Create<Process>("EDI"));
            this.Map(EntityTypeMap.Create<StateCode>("EDI"));
            this.Map(EntityTypeMap.Create<Topic>("EDI"));
            this.Map(EntityTypeMap.Create<MessageEntity>("EDI", "Command"));

            // TODO:实现一个良好的插件架构
            // TODO:参考InfoRule模块完成命令插件模块的配置
            //var plugins = Resolver.Resolve<IPluginImporter>().GetPlugins();
            //if (plugins != null) {
            //    foreach (var item in plugins) {
            //        this.Plugins.Add(item);
            //    }
            //}

            this.Map(EntityTypeMap.Create<RDatabase>("AC"));
            this.Map(EntityTypeMap.Create<DbTable>("AC"));
            this.Map(EntityTypeMap.Create<DbView>("AC"));
            this.Map(EntityTypeMap.Create<DbTableColumn>("AC"));
            this.Map(EntityTypeMap.Create<DbViewColumn>("AC"));
            this.Map(EntityTypeMap.Create<DbTableSpace>("AC", "TableSpace"));
            this.Map(EntityTypeMap.Create<ExceptionLog>("AC"));
            this.Map(EntityTypeMap.Create<Parameter>("AC"));
            this.Map(EntityTypeMap.Create<OperationLog>("AC"));
            this.Map(EntityTypeMap.Create<OperationHelp>("AC", "Help"));
            this.Map(EntityTypeMap.Create<AnyLog>("AC"));

            this.Map(EntityTypeMap.Create<AppSystem>("AC"));
            this.Map(EntityTypeMap.Create<Button>("AC"));
            this.Map(EntityTypeMap.Create<Dic>("AC"));
            this.Map(EntityTypeMap.Create<DicItem>("AC"));
            this.Map(EntityTypeMap.Create<EntityType>("AC"));
            this.Map(EntityTypeMap.Create<Function>("AC"));
            this.Map(EntityTypeMap.Create<Menu>("AC"));
            this.Map(EntityTypeMap.Create<OperationHelp>("AC"));
            this.Map(EntityTypeMap.Create<Organization>("AC"));
            this.Map(EntityTypeMap.Create<Page>("AC"));
            this.Map(EntityTypeMap.Create<PageButton>("AC"));
            this.Map(EntityTypeMap.Create<Property>("AC"));
            this.Map(EntityTypeMap.Create<ResourceType>("AC"));

            this.Map(EntityTypeMap.Create<Account>("AC"));
            this.Map(EntityTypeMap.Create<DeveloperID>("AC"));
            this.Map(EntityTypeMap.Create<VisitingLog>("AC"));

            this.Map(EntityTypeMap.Create<Group>("AC"));
            this.Map(EntityTypeMap.Create<Role>("AC"));
            this.Map(EntityTypeMap.Create<PrivilegeBigram>("AC"));
        }

        private void AddDefaultService<T>(object service)
        {
            if (this.GetService(typeof(T)) == null)
            {
                this.AddService(typeof(T), service);
            }
        }
    }
}
