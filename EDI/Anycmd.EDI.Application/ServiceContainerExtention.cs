
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
        public static void RegisterEDICore(this IAppHost host)
        {
            #region EDI
            host.AddService(typeof(IExecutorFactory), new ExecutorFactory());
            host.AddService(typeof(IDispatcherFactory), new DispatcherFactory());
            host.AddService(typeof(IAuthenticator), new DefaultAuthenticator(host));
            host.AddService(typeof(IMessageProducer), new DefaultMessageProducer(host));
            host.AddService(typeof(IStackTraceFormater), new JsonStackTraceFormater());
            host.AddService(typeof(IInputValidator), new DefaultInputValidator(host));
            host.AddService(typeof(IAuditDiscriminator), new DefaultAuditDiscriminator(host));
            host.AddService(typeof(IPermissionValidator), new DefaultPermissionValidator());
            #endregion
        }
    }
}
