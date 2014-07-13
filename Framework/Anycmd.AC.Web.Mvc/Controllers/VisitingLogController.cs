
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Identity.ViewModels.AccountViewModels;
    using MiniUI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Util;

    /// <summary>
    /// 系统来访日志模型视图控制器<see cref="ACEntities.VisitingLog"/>
    /// </summary>
    public class VisitingLogController : AnycmdController
    {
        #region ViewResults
        [By("xuexs")]
        [Description("来访日志")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("来访日志详细信息")]
        public ViewResultBase Details()
        {
            return this.DetailsResult(GetRequiredService<IVisitingLogQuery>(), "VisitingLogInfo");
        }

        #endregion

        [By("xuexs")]
        [Description("根据ID获取来访日志详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IVisitingLogQuery>().Get("VisitingLogInfo", id.Value));
        }

        [By("xuexs")]
        [Description("分页获取来访日志")]
        public ActionResult GetPlistVisitingLogs(GetPlistVisitingLogs requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var visitingLogs = GetRequiredService<IVisitingLogQuery>().GetPlistVisitingLogTrs(
                requestData.key, requestData.leftVisitOn, requestData.rightVisitOn, requestData);
            var data = new MiniGrid<VisitingLogTr> { total = requestData.total.Value, data = visitingLogs.Select(a => new VisitingLogTr(a)) };

            return this.JsonResult(data);
        }

        [By("xuexs")]
        [Description("分页获取我的来访日志")]
        public ActionResult GetPlistMyVisitingLogs(GetPlistMyVisitingLogs requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            if (!User.Identity.IsAuthenticated)
            {
                return this.JsonResult(new MiniGrid<Dictionary<string, object>> { total = 0, data = new List<Dictionary<string, object>> { } });
            }
            var visitingLogs = GetRequiredService<IVisitingLogQuery>().GetPlistVisitingLogTrs(
                CurrentUser.GetAccountID(), User.Identity.Name, requestData.leftVisitOn, requestData.rightVisitOn
                , requestData);
            var data = new MiniGrid<VisitingLogTr> { total = requestData.total.Value, data = visitingLogs.Select(a => new VisitingLogTr(a)) };

            return this.JsonResult(data);
        }
    }
}
