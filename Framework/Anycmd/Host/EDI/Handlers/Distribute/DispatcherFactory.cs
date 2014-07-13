﻿
namespace Anycmd.Host.EDI.Handlers.Distribute
{

    /// <summary>
    /// 命令分发者工厂。默认实现
    /// </summary>
    public class DispatcherFactory : IDispatcherFactory {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public IDispatcher CreateDispatcher(ProcessDescriptor process) {
            return new DefaultDispatcher(process);
        }
    }
}
