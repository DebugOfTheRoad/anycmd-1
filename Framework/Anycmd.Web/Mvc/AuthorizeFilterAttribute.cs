
namespace Anycmd.Web.Mvc
{
    using Exceptions;
    using Host;
    using System;
    using System.Web.Mvc;
    using ViewModel;

    /// <summary>
    /// 操作拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        private const string NO_PERMISSION_JSON_RESULT = "对不起，您没有执行本操作的权限，权限码";
        private const string NO_PERMISSION_VIEW_RESULT = "对不起，您没有查看该页面的权限，权限码";
        private const string NOT_LOGON = "对不起，请先登录";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;
            string resourceCode = controller;
            string functionCode = actionName;
            object[] resourceAttrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(ResourceAttribute), inherit: true);
            if (resourceAttrs.Length == 1)
            {
                var resourceAttr = resourceAttrs[0] as ResourceAttribute;
                if (resourceAttr == null)
                {
                    throw new CoreException();
                }
                resourceCode = resourceAttr.ResourceCode;
            }
            var host = filterContext.HttpContext.Application["AppHostInstance"] as AppHost;
            ResourceTypeState resource;
            if (!host.ResourceSet.TryGetResource(host.AppSystemSet.SelfAppSystem, resourceCode, out resource))
            {
                throw new ValidationException("意外的资源码" + resourceCode);
            }
            FunctionState function;
            if (!host.FunctionSet.TryGetFunction(resource, functionCode, out function))
            {
                return;
            }
            if (function.IsEnabled != 1)
            {
                if (isAjaxRequest)
                {
                    filterContext.Result = new FormatJsonResult
                    {
                        Data = new ResponseData { success = false, msg = "对不起，" + function.Description + "的功能已禁用" }.Warning()
                    };
                }
                else
                {
                    filterContext.Result = new ContentResult { Content = "对不起，" + function.Description + "的页面已禁用" };
                }
                return;
            }
            if (filterContext.ActionDescriptor.IsDefined(typeof(IgnoreAuthAttribute), inherit: false))
            {
                return;
            }

            #region 登录验证
            if (!host.User.Principal.Identity.IsAuthenticated)
            {
                if (isAjaxRequest)
                {
                    filterContext.Result = new FormatJsonResult
                    {
                        Data = new ResponseData { success = false, msg = NOT_LOGON }.Info()
                    };
                }
                else
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "LogOn"
                    };
                }
                return;
            }
            #endregion

            #region 权限验证
            // 注意：数据约定
            var area = filterContext.RouteData.DataTokens["area"];
            string codespace = area == null ? string.Empty : area.ToString();
            string entityTypeCode = controller;
            object[] modelAttrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(ModelAttribute), inherit: true);
            if (modelAttrs.Length == 1)
            {
                var modelAttr = modelAttrs[0] as ModelAttribute;
                if (modelAttr == null)
                {
                    throw new CoreException();
                }
                entityTypeCode = modelAttr.EntityTypeCode;
            }
            EntityTypeState entityType;
            if (!host.EntityTypeSet.TryGetEntityType(codespace, entityTypeCode, out entityType))
            {
                return;
            }
            if (!host.User.Permit(function, null))
            {
                if (isAjaxRequest)
                {
                    filterContext.Result = new FormatJsonResult
                    {
                        Data = new ResponseData { success = false, msg = NO_PERMISSION_JSON_RESULT }.Warning()
                    };
                }
                else
                {
                    filterContext.Result = new ContentResult { Content = NO_PERMISSION_VIEW_RESULT };
                }
                return;
            }
            #endregion
        }
    }
}
