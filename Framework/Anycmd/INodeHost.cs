
namespace Anycmd {
    using Anycmd.Host.EDI;
    using Anycmd.Host.EDI.Handlers;
    using EDI;
    using Host.EDI.Hecp;
    using Host.EDI.Info;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 数据交换宿主
    /// </summary>
    public interface INodeHost {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 宿主配置
        /// </summary>
        HostConfig Config { get; }

        /// <summary>
        /// 插件
        /// </summary>
        List<IPlugin> Plugins { get; }

        /// <summary>
        /// 本节点数据交换进程上下文。进程列表。
        /// </summary>
        IProcesseSet Processs { get; }

        /// <summary>
        /// 节点上下文
        /// </summary>
        INodeSet Nodes { get; }

        /// <summary>
        /// 信息字典上下文
        /// </summary>
        IInfoDicSet InfoDics { get; }

        /// <summary>
        /// 本体上下文
        /// </summary>
        IOntologySet Ontologies { get; }

        /// <summary>
        /// 信息字符串转化器上下文
        /// </summary>
        IInfoStringConverterSet InfoStringConverters { get; }

        /// <summary>
        /// 信息项验证器上下文
        /// </summary>
        IInfoRuleSet InfoRules { get; }

        /// <summary>
        /// 命令提供程序上下文
        /// </summary>
        IMessageProviderSet MessageProviders { get; }

        /// <summary>
        /// 数据提供程序上下文
        /// </summary>
        IEntityProviderSet EntityProviders { get; }

        /// <summary>
        /// 命令转移器
        /// </summary>
        IMessageTransferSet Transfers { get; }

        /// <summary>
        /// 添加请求过滤器, 这些过滤器在Http请求被转化为Hecp请求后应用
        /// </summary>
        List<Func<HecpContext, ProcessResult>> PreRequestFilters { get; }

        /// <summary>
        /// 添加命令过滤器。这些过滤器在Command验证通过但被处理前应用
        /// </summary>
        List<Func<MessageContext, ProcessResult>> GlobalProcessingFilters { get; }

        /// <summary>
        /// 添加命令过滤器。这些过滤器在Command验证通过并被处理后应用
        /// </summary>
        List<Func<MessageContext, ProcessResult>> GlobalProcessedFilters { get; }

        /// <summary>
        /// 添加响应过滤器。这些过滤器在Hecp响应末段应用
        /// </summary>
        List<Func<HecpContext, ProcessResult>> GlobalResponseFilters { get; }

        /// <summary>
        /// 根据插件类型获取插件目录地址
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        string GetPluginBaseDirectory(PluginType pluginType);
    }
}
