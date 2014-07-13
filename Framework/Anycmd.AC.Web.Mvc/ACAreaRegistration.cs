using System.Web.Mvc;

namespace Anycmd.AC.Web.Mvc
{
    public class ACAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AC_default",
                "AC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "Anycmd.AC.Web.Mvc.Controllers" }
            );
        }
    }
}
