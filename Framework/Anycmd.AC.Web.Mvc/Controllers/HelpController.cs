
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Host;
    using Anycmd.Host.AC.Infra;
    using Anycmd.Host.AC.Infra.Messages;
    using Anycmd.Repositories;
    using Anycmd.Web.Mvc;
    using Exceptions;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 系统帮助模型视图控制器<see cref="ACEntities.Help"/>
    /// </summary>
    public class HelpController : AnycmdController
    {
        #region ViewPages
        [By("xuexs")]
        [Description("帮助热线")]
        public ViewResultBase Helpline(Guid? id, string isInner, string appSystemCode, string resourceCode, string functionCode)
        {
            if (!id.HasValue)
            {
                AppSystemState appSystem;
                if (!Host.AppSystemSet.TryGetAppSystem(appSystemCode, out appSystem))
                {
                    throw new ValidationException("意外的应用系统码" + appSystemCode);
                }
                ResourceTypeState resource;
                if (!Host.ResourceSet.TryGetResource(Host.AppSystemSet.SelfAppSystem, resourceCode, out resource))
                {
                    throw new ValidationException("意外的资源码" + resourceCode);
                }
                FunctionState function;
                if (!Host.FunctionSet.TryGetFunction(resource, functionCode, out function))
                {
                    throw new ValidationException(string.Format("非法的操作:{0}.{1}.{2}", appSystemCode, resourceCode, functionCode));
                }
                id = function.Id;
            }
            OperationHelp help = GetRequiredService<IRepository<OperationHelp>>().GetByKey(id.Value);
            if (help == null)
            {
                help = new OperationHelp()
                {
                    Id = id.Value,
                    Content = "没有帮助"
                };
            }
            if (!string.IsNullOrEmpty(isInner))
            {
                return PartialView("Partials/Helpline", help);
            }

            return View(help);
        }

        /// <summary>
        /// 用以控制权限，Action名和当前Action所在应用系统名、区域名、控制器名用来生成操作码和权限码。
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("编辑帮助")]
        public ActionResult Edit()
        {
            return ViewResult();
        }
        #endregion

        [By("xuexs")]
        [Description("根据帮助ID获取帮助")]
        public ActionResult Get(Guid? id, string appSystemCode, string resourceCode, string functionCode)
        {
            if (!id.HasValue)
            {
                AppSystemState appSystem;
                if (!Host.AppSystemSet.TryGetAppSystem(appSystemCode, out appSystem))
                {
                    throw new ValidationException("意外的应用系统码" + appSystemCode);
                }
                ResourceTypeState resource;
                if (!Host.ResourceSet.TryGetResource(Host.AppSystemSet.SelfAppSystem, resourceCode, out resource))
                {
                    throw new ValidationException("意外的资源码" + resourceCode);
                }
                FunctionState function;
                if (!Host.FunctionSet.TryGetFunction(resource, functionCode, out function))
                {
                    throw new ValidationException(string.Format("非法的操作:{0}.{1}.{2}", appSystemCode, resourceCode, functionCode));
                }
                id = function.Id;
            }
            OperationHelp help = GetRequiredService<IRepository<OperationHelp>>().GetByKey(id.Value);
            if (help == null)
            {
                help = new OperationHelp()
                {
                    Id = id.Value,
                    Content = "没有帮助"
                };
            }

            return this.JsonResult(help);
        }

        [By("xuexs")]
        [Description("保存帮助")]
        [ValidateInput(enableValidation: false)]
        public ActionResult SaveHelp(Guid? id, string appSystemCode, string resourceCode, string functionCode, string content, int? isEnabled)
        {
            if (!id.HasValue || id == Guid.Empty)
            {
                AppSystemState appSystem;
                if (!Host.AppSystemSet.TryGetAppSystem(appSystemCode, out appSystem))
                {
                    throw new ValidationException("意外的应用系统码" + appSystemCode);
                }
                ResourceTypeState resource;
                if (!Host.ResourceSet.TryGetResource(Host.AppSystemSet.SelfAppSystem, resourceCode, out resource))
                {
                    throw new ValidationException("意外的资源码" + resourceCode);
                }
                FunctionState function;
                if (!Host.FunctionSet.TryGetFunction(resource, functionCode, out function))
                {
                    throw new ValidationException(string.Format("非法的操作:{0}.{1}.{2}", appSystemCode, resourceCode, functionCode));
                }
                id = function.Id;
            }
            Host.Handle(new SaveHelpCommand(id.Value, content, isEnabled));

            return new FormatJsonResult { Data = new ResponseData { success = true } };
        }
    }
}
