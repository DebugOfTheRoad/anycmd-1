
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Infra.ViewModels.LogViewModels;
    using Logging;
    using MiniUI;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;

    /// <summary>
    /// 系统操作日志模型视图控制器<see cref="Logging.OperationLogBase"/>
    /// </summary>
    public class OperationLogController : AnycmdController
    {
        #region ViewPage

        [By("xuexs")]
        [Description("操作日志主页")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("操作日志")]
        public ViewResultBase OperationLogs()
        {
            return ViewResult();
        }
        #endregion

        [By("xuexs")]
        [Description("分页获取操作日志")]
        public ActionResult GetPlistOperationLogs(GetPlistOperationLogs requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var operationlogs = GetRequiredService<ILoggingService>().GetPlistOperationLogs(
                requestData.targetID,
                requestData.leftCreateOn,
                requestData.rightCreateOn,
                requestData.filters,
                requestData);
            var data = new MiniGrid<OperationLog> { data = operationlogs, total = requestData.total.Value };

            return this.JsonResult(data);
        }
    }
}
