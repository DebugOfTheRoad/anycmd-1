
namespace Anycmd.EDI.Service.Tests
{
    using Anycmd.Web;
    using Application;
    using Ef;
    using Host;
    using Logging;
    using System.Collections.Generic;

    public class Boot
    {
        public Boot()
        {
            var appHost = new DefaultAppHost();
            EfContext.InitStorage(new SimpleEfContextStorage());
            var container = appHost.Container;
            container.AddService(typeof(ILoggingService), new log4netLoggingService(appHost));
            container.AddService(typeof(IUserSessionStorage), new WebUserSessionStorage());
            appHost.Init();
            appHost.RegisterRepository(new List<string>
            {
                "EDIEntities",
                "ACEntities",
                "InfraEntities",
                "IdentityEntities"
            }, typeof(AppHost).Assembly);
            appHost.RegisterEDICore();
            new DefaultNodeHost(appHost).Init();
        }
    }
}
