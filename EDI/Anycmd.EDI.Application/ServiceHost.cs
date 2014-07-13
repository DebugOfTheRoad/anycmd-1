
namespace Anycmd.EDI.Application
{
    using DataContracts;
    using Funq;
    using MessageServices;
    using ServiceStack;
    using System;
    using System.ComponentModel.Design;
    using System.Reflection;

    /// <summary>
    /// Create your ServiceHost web service application with a singleton ServiceHost.
    /// </summary>        
    public class ServiceHost : AppHostBase
    {
        private readonly AppHost appHost;

        /// <summary>
        /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
        /// </summary>
        public ServiceHost()
            : base("数据交换服务", typeof(MessageService).Assembly)
        {
            this.appHost = AppHost.Instance;
        }

        public ServiceHost(string serviceName, params Assembly[] assembliesWithServices)
            : base(serviceName, assembliesWithServices)
        {
            this.appHost = AppHost.Instance;
        }

        /// <summary>
        /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
        /// </summary>
        public ServiceHost(AppHost appHost)
            : base("数据交换服务", typeof(MessageService).Assembly)
        {
            if (appHost == null)
            {
                throw new ArgumentNullException("appHost");
            }
            this.appHost = appHost;
        }

        public ServiceHost(AppHost appHost, string serviceName, params Assembly[] assembliesWithServices)
            : base(serviceName, assembliesWithServices)
        {
            if (appHost == null)
            {
                throw new ArgumentNullException("appHost");
            }
            this.appHost = appHost;
        }

        /// <summary>
        /// Configure the container with the necessary routes for your ServiceStack application.
        /// </summary>
        /// <param name="container">The built-in IoC used with ServiceStack.</param>
        public override void Configure(Container container)
        {
            container.Adapter = new ServiceContainerAdapter(appHost.Container);

            SetConfig(new HostConfig
            {
                DebugMode = true,
                WsdlServiceNamespace = Consts.Namespace,
                EnableFeatures = Feature.Metadata | Feature.Json | Feature.Jsv | Feature.Html
            });
        }
    }
}
