
namespace Anycmd.Mis.Web.Mvc.Controllers
{
    using AC.Infra;
    using Anycmd.Web.Mvc;
    using Host;
    using Host.AC.Identity.Messages;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 首页控制器
    /// </summary>
    public class HomeController : AnycmdController
    {
        /// <summary>
        /// 首页
        /// </summary>
        [By("xuexs")]
        [CacheFilter]
        public ViewResultBase Index()
        {
            return View();
        }

        [By("xuexs")]
        public ViewResultBase About()
        {
            return ViewResult();
        }

        /// <summary>
        /// 登录
        /// </summary>
        [By("xuexs")]
        [CacheFilter]
        public ActionResult LogOn()
        {
            if (Host.User.Principal.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index");
            }
            return View();
        }

        #region GetAccountInfo
        [By("xuexs")]
        [Description("获取登录信息")]
        public ActionResult GetAccountInfo()
        {
            if (Host.User.Principal.Identity.IsAuthenticated)
            {
                var account = Host.User.Worker;
                var menuList = new List<IMenu>();
                foreach (var item in Host.User.GetAllMenus())
                {
                    menuList.Add(item);
                }
                foreach (var item in EDIMenuHelper.GetEntityMenus())
                {
                    menuList.Add(item);
                }
                var menus = menuList.Select(m => new
                {
                    id = m.Id,
                    text = m.Name,
                    pid = m.ParentID,
                    url = m.Url,
                    img = m.Icon
                });
                return this.JsonResult(new
                {
                    isLogined = Host.User.Principal.Identity.IsAuthenticated,
                    loginName = Host.User.IsDeveloper() ? string.Format("{0}(开发人员)", account.LoginName) : account.LoginName,
                    wallpaper = account.Wallpaper ?? string.Empty,
                    backColor = account.BackColor ?? string.Empty,
                    menus = menus,
                    roles = Host.User.GetRoles(),
                    groups = Host.User.GetGroups()
                });
            }
            else
            {
                return this.JsonResult(new ResponseData { success = false, msg = "对不起，您没有登录" }.Error());
            }
        }
        #endregion

        [By("xuexs")]
        [Description("登录")]
        [HttpPost]
        [IgnoreAuth]
        public ActionResult SignIn(string loginName, string password, string rememberMe)
        {
            Host.SignIn(loginName, password, rememberMe);

            return this.JsonResult(new ResponseData { success = true });
        }

        [By("xuexs")]
        [Description("登出")]
        [HttpPost]
        public ActionResult SignOut()
        {
            Host.SignOut();

            return this.JsonResult(new ResponseData { success = true });
        }

        private static ActionResult iconImgResult = null;
        [By("xuexs")]
        [IgnoreAuth]
        [Description("获取图标")]
        [CacheFilter(Enable = true)]
        public ActionResult GetIcons()
        {
            if (iconImgResult == null)
            {
                //[{'id':1,'phrase':'[呵呵]','url':'1.gif'},{'id':2,'phrase':'[嘻嘻]','url':'2.gif'}]
                var dtinfo = new DirectoryInfo(Server.MapPath("~/Content/icons/16x16/"));
                var files = dtinfo.GetFiles();
                var icons = files.Select(f => new
                {
                    id = f.Name.Substring(0, f.Name.Length - f.Extension.Length).ToLower(),
                    phrase = f.Name.Substring(0, f.Name.Length - f.Extension.Length),
                    icon = f.Name.ToLower(),
                    url = "Content/icons/16x16/" + f.Name,
                    extension = f.Extension
                });

                iconImgResult = this.JsonResult(icons);
            }

            return iconImgResult;
        }
    }
}