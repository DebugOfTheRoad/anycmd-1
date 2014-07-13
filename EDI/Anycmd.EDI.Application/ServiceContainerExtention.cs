
namespace Anycmd.EDI.Application {
    using Host.EDI;
    using Host.EDI.Handlers;
    using Host.EDI.Handlers.Distribute;
    using Host.EDI.Handlers.Execute;

    /// <summary>
    /// 
    /// </summary>
    public static class ServiceContainerExtention {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public static void RegisterEDICore(this AppHost host) {
            var container = host.Container;
            #region EDI
            container.AddService(typeof(IExecutorFactory), new ExecutorFactory());
            container.AddService(typeof(IDispatcherFactory), new DispatcherFactory());
            container.AddService(typeof(IAuthenticator), new DefaultAuthenticator());
            container.AddService(typeof(IMessageProducer), new DefaultMessageProducer());
            container.AddService(typeof(IStackTraceFormater), new JsonStackTraceFormater());
            container.AddService(typeof(IInputValidator), new DefaultInputValidator());
            container.AddService(typeof(IAuditDiscriminator), new DefaultAuditDiscriminator());
            container.AddService(typeof(IPermissionValidator), new DefaultPermissionValidator());
            #endregion
        }
    }
}
