
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Infra.ViewModels.LogViewModels;
    using Logging;
    using MiniUI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 任何日志模型视图控制器<see cref="AnyLog"/>
    /// </summary>
    public class AnyLogController : AnycmdController
    {
        [By("xuexs")]
        [Description("运行日志管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("运行日志详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (!string.IsNullOrEmpty(Request["id"]))
                {
                    if (Guid.TryParse(Request["id"], out id))
                    {
                        var data = GetRequiredService<ILoggingService>().Get(id);
                        return new PartialViewResult { ViewName = "Partials/Details", ViewData = new ViewDataDictionary(data) };
                    }
                }
                throw new ValidationException("非法的Guid标识" + Request["id"]);
            }
            else if (!string.IsNullOrEmpty(Request["isInner"]))
            {
                return new PartialViewResult { ViewName = "Partials/Details" };
            }
            else
            {
                return new ViewResult { ViewName = "Details" };
            }
        }

        [By("xuexs")]
        [Description("根据ID获取运行日志详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            IAnyLog data = null;
            if (id.HasValue)
            {
                data = GetRequiredService<ILoggingService>().Get(id.Value);
            }

            return this.JsonResult(data);
        }

        [By("xuexs")]
        [Description("清空运行日志")]
        [HttpPost]
        public ActionResult ClearAnyLog()
        {
            var responseResult = new ResponseData { success = false };
            GetRequiredService<ILoggingService>().ClearAnyLog();
            responseResult.success = true;

            return this.JsonResult(responseResult);
        }

        [By("xuexs")]
        [Description("分页获取运行日志")]
        public ActionResult GetPlistAnyLogs(GetPlistAnyLogs requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            IList<IAnyLog> anyLogs = GetRequiredService<ILoggingService>().GetPlistAnyLogs(requestModel.filters, requestModel);
            var data = new MiniGrid<IAnyLog> { total = requestModel.total.Value, data = anyLogs };

            return this.JsonResult(data);
        }
    }
}
