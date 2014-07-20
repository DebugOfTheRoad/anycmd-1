
namespace Anycmd.Mis.Web.Mvc
{
    using AC.Identity.Queries.Ef;
    using Anycmd.Web;
    using Anycmd.Web.Mvc;
    using EDI.Application;
    using EDI.MessageServices;
    using EDI.Queries.Ef;
    using Ef;
    using Host;
    using Logging;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcApplication : HttpApplication
    {
        public override void Init()
        {
            EfContext.InitStorage(new WebEfContextStorage(this));
            base.Init();
        }

        protected void Application_Start()
        {
            #region ASP.NET MVC
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            ModelBinders.Binders.DefaultBinder = new PlistModelBinder();
            ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());
            AreaRegistration.RegisterAllAreas();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            RouteTable.Routes.IgnoreRoute("ws/{*pathInfo}");
            RouteTable.Routes.IgnoreRoute("api/{*pathInfo}");

            RouteTable.Routes.MapRoute(
                "HomeIndex",
                "",
                new { controller = "Home", action = "Index" });

            RouteTable.Routes.MapRoute(
                "Home",
                "Home/{action}",
                new { controller = "Home", action = "Index" });

            RouteTable.Routes.MapRoute(
               "Default",
               "{controller}/{action}/{id}",
               new { controller = "Error", action = "Http404", id = UrlParameter.Optional });
            #endregion

            var appHost = new DefaultAppHost();
            Application.Add("AppHostInstance", appHost);
            appHost.AddService(typeof(IFunctionListImport), new FunctionListImport());
            appHost.AddService(typeof(IEfFilterStringBuilder), new EfFilterStringBuilder());
            appHost.AddService(typeof(ILoggingService), new log4netLoggingService(appHost));
            appHost.AddService(typeof(IUserSessionStorage), new WebUserSessionStorage());
            appHost.Init();

            appHost.RegisterRepository(new List<string>
            {
                "EDIEntities",
                "ACEntities",
                "InfraEntities",
                "IdentityEntities"
            }, typeof(AppHost).Assembly);
            appHost.RegisterQuery(typeof(BatchQuery).Assembly, typeof(AccountQuery).Assembly);
            appHost.RegisterEDICore();

            (new ServiceHost(appHost, "", typeof(MessageService).Assembly)).Init();
        }
    }
}