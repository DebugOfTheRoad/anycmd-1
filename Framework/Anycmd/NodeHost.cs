﻿
namespace Anycmd
{
    using EDI;
    using Exceptions;
    using Host.EDI;
    using Host.EDI.Handlers;
    using Host.EDI.Hecp;
    using Host.EDI.MemorySets;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// 数据交换宿主抽象基类
    /// <remarks>实现自定义宿主必须从该基类继承</remarks>
    /// </summary>
    public abstract class NodeHost : INodeHost
    {
        private static object locker = new object();
        private static NodeHost _instance = null;

        private readonly Guid _id = Guid.NewGuid();
        private bool _pluginsLoaded;

        /// <summary>
        /// 
        /// </summary>
        public static NodeHost Instance
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

        /// <summary>
        /// 
        /// </summary>
        protected NodeHost(IAppHost appHost)
        {
            lock (locker)
            {
                _instance = this;
                this.AppHost = appHost;
                this.StartedAt = DateTime.UtcNow;
                this.PreHecpRequestFilters = new List<Func<HecpContext, ProcessResult>>();
                this.GlobalEDIMessageHandingFilters = new List<Func<MessageContext, ProcessResult>>();
                this.GlobalEDIMessageHandledFilters = new List<Func<MessageContext, ProcessResult>>();
                this.GlobalHecpResponseFilters = new List<Func<HecpContext, ProcessResult>>();
                this.HecpHandler = new HecpHandler(this);
                this.Plugins = new List<IPlugin>
                {
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual NodeHost Init()
        {
            OnConfigLoad();

            Configure();

            OnAfterInit();

            return this;
        }

        public IAppHost AppHost { get; protected set; }

        /// <summary>
        /// 管道插件
        /// </summary>
        public List<IPlugin> Plugins { get; protected set; }

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

        public HecpHandler HecpHandler { get; private set; }

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

        /// <summary>
        /// Config has changed
        /// </summary>
        public virtual void OnAfterConfigChanged()
        {
        }

        #region GetBuildinPluginBaseDirectory
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
                    return this.AppHost.BuildInPluginsBaseDirectory;
                case PluginType.MessageProvider:
                    return Path.Combine(this.AppHost.BuildInPluginsBaseDirectory, "MessageProviders");
                case PluginType.EntityProvider:
                    return Path.Combine(this.AppHost.BuildInPluginsBaseDirectory, "EntityProviders");
                case PluginType.InfoStringConverter:
                    return Path.Combine(this.AppHost.BuildInPluginsBaseDirectory, "InfoStringConverters");
                case PluginType.InfoConstraint:
                    return Path.Combine(this.AppHost.BuildInPluginsBaseDirectory, "InfoConstraints");
                case PluginType.MessageTransfer:
                    return Path.Combine(this.AppHost.BuildInPluginsBaseDirectory, "MessageTransfers");
                default:
                    throw new CoreException("意外的插件类型");
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public abstract void Configure();

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnConfigLoad()
        {
        }

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
                    AppHost.LoggingService.Error("Error loading plugin " + plugin.GetType().Name, ex);
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
        /// 当Configure方法调用后。
        /// </summary>
        public void OnAfterInit()
        {
            AfterInitAt = DateTime.UtcNow;

            LoadPlugin(Plugins.ToArray());
            _pluginsLoaded = true;

            ReadyAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 应用Hecp管道过滤器，通过返回结果表达当前Hecp请求是否被处理过了，如果处理过了则就转到响应流程了。
        /// </summary>
        /// <returns></returns>
        public ProcessResult ApplyPreRequestFilters(HecpContext context)
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
        public ProcessResult ApplyProcessingFilters(MessageContext context)
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
        public ProcessResult ApplyProcessedFilters(MessageContext context)
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
        public ProcessResult ApplyResponseFilters(HecpContext context)
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
    }
}
