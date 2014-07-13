
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Infra.ViewModels.LogViewModels;
    using Logging;
    using MiniUI;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 系统异常模型视图控制器<see cref="ACEntities.ExceptionLog"/>
    /// </summary>
    [DeveloperFilter(Order = 21)]
    public class ExceptionLogController : AnycmdController
    {
        private readonly EntityTypeState entityType;

        public ExceptionLogController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("AC", "ExceptionLog", out entityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        [By("xuexs")]
        [Description("系统异常管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("系统异常详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = entityType.GetData(id);
                    return new PartialViewResult { ViewName = "Partials/Details", ViewData = new ViewDataDictionary(data) };
                }
                else
                {
                    throw new ValidationException("非法的Guid标识" + Request["id"]);
                }
            }
            else if (!string.IsNullOrEmpty(Request["isInner"]))
            {
                return new PartialViewResult { ViewName = "Partials/Details" };
            }
            else
            {
                return this.View();
            }
        }

        #endregion

        [By("xuexs")]
        [Description("根据ID获取系统异常详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(entityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("分页获取系统异常")]
        public ActionResult GetPlistExceptionLogs(GetPlistExceptionLogs requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var exceptionlogs = GetRequiredService<ILoggingService>().GetPlistExceptionLogs(requestData.filters, requestData);

            var data = new MiniGrid<ExceptionLog> { total = requestData.total.Value, data = exceptionlogs };

            return this.JsonResult(data);
        }

        [By("xuexs")]
        [Description("清空系统异常")]
        [HttpPost]
        public ActionResult ClearExceptionLog()
        {
            var responseResult = new ResponseData { success = false };
            GetRequiredService<ILoggingService>().ClearExceptionLog();
            responseResult.success = true;

            return this.JsonResult(responseResult);
        }
    }
}
