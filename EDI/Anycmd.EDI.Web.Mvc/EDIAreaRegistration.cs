
namespace Anycmd.EDI.Web.Mvc {
    using System.Web.Mvc;

    /// <summary>
    /// 提供在 ASP.NET MVC 应用程序内注册EDI区域的方式。
    /// </summary>
    public class EDIAreaRegistration : AreaRegistration {
        /// <summary>
        /// 区域名：值为EDI
        /// </summary>
        public override string AreaName {
            get {
                return "EDI";
            }
        }

        /// <summary>
        /// 使用指定区域的上下文信息在 ASP.NET MVC 应用程序内注册某个区域。
        /// </summary>
        /// <param name="context">对注册区域所需的信息进行封装</param>
        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "EDI_default",
                "EDI/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "Anycmd.EDI.Web.Mvc.Controllers" }
            );
        }
    }
}
