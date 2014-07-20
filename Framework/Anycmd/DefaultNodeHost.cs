
namespace Anycmd
{
    using Host.AC.Infra;
    using Host.EDI;
    using Host.EDI.Entities;
    using Host.EDI.Handlers;
    using Host.EDI.MemorySets.Impl;
    using Host.EDI.MessageHandlers;

    /// <summary>
    /// 
    /// </summary>
    public partial class DefaultNodeHost : NodeHost
    {
        /// <summary>
        /// 
        /// </summary>
        public DefaultNodeHost(AppHost appHost)
            : base(appHost)
        {
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

        /// <summary>
        /// 
        /// </summary>
        public override void Configure()
        {
            this.AppHost.AddService(typeof(INodeHostBootstrap), new FastNodeHostBootstrap(this.AppHost));
            this.AppHost.MessageDispatcher.Register(new AddBatchCommandHandler(this.AppHost));
            this.AppHost.MessageDispatcher.Register(new UpdateBatchCommandHandler(this.AppHost));
            this.AppHost.MessageDispatcher.Register(new RemoveBatchCommandHandler(this.AppHost));

            this.AppHost.Map(EntityTypeMap.Create<Action>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<Archive>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<Batch>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<Element>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<InfoDic>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<InfoDicItem>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<InfoGroup>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<InfoRule>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<Node>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<NodeElementAction>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<NodeElementCare>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<NodeTopic>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<NodeOntologyCare>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<NodeOntologyOrganization>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<Ontology>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<OntologyOrganization>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<Plugin>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<Process>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<StateCode>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<Topic>("EDI"));
            this.AppHost.Map(EntityTypeMap.Create<MessageEntity>("EDI", "Command"));

            // TODO:实现一个良好的插件架构
            // TODO:参考InfoRule模块完成命令插件模块的配置
            //var plugins = Resolver.Resolve<IPluginImporter>().GetPlugins();
            //if (plugins != null) {
            //    foreach (var item in plugins) {
            //        this.Plugins.Add(item);
            //    }
            //}
        }
    }
}
