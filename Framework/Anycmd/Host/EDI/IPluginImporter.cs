
namespace Anycmd.Host.EDI {
    using System.Collections.Generic;

    /// <summary>
    /// 命令插件导入器。将命令插件dll导入应用程序域并返回插件列表。
    /// </summary>
    public interface IPluginImporter {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlugin> GetPlugins();
    }
}
