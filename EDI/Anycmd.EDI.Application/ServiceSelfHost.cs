using Anycmd.Host.EDI;

namespace Anycmd.EDI.Application {
    using DataContracts;
    using Funq;
    using MessageServices;
    using ServiceStack;
    using System;
    using Util;

    /// <summary>
    /// Create your ServiceSelfHost web service application with a singleton ServiceSelfHost.
    /// </summary>
    public sealed class ServiceSelfHost : AppHostHttpListenerBase {
        private readonly ProcessDescriptor process;
        private readonly Anycmd.IAppHost appHost;

        public ServiceSelfHost(Anycmd.IAppHost appHost, ProcessDescriptor process)
            : base("Self-Host", typeof(MessageService).Assembly)
        {
            if (appHost == null)
            {
                throw new ArgumentNullException("appHost");
            }
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }
            this.appHost = appHost;
            this.process = process;
            this.ServiceName = process.ProcessType.ToName() + " - " + process.Process.Name;
        }

        public override ServiceStackHost Init() {
            var host = base.Init();
            host.Start(process.WebApiBaseAddress);
            return host;
        }

        public override void Configure(Container container) {
            var adapter = new ServiceContainerAdapter(appHost);
            container.Adapter = adapter;

            SetConfig(new HostConfig {
                DebugMode = true,
                WsdlServiceNamespace = Consts.Namespace,
                EnableFeatures = Feature.Metadata | Feature.Json | Feature.Jsv | Feature.Html
            });
        }
    }

}
