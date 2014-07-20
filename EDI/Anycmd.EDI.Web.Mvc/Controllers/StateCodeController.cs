
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Host;
    using Anycmd.Web.Mvc;
    using Exceptions;
    using MiniUI;
    using Query;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Web.Mvc;
    using Util;
    using ViewModels.StateCodeViewModels;

    /// <summary>
    /// 数据交换状态码模型视图控制器<see cref="StateCode"/>
    /// </summary>
    public class StateCodeController : AnycmdController
    {
        private static readonly EntityTypeState stateCodeEntityType;

        static StateCodeController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "StateCode", out stateCodeEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        /// <summary>
        /// 信息字典管理
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("信息字典管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 信息字典详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("信息字典详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = stateCodeEntityType.GetData(id);
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

        /// <summary>
        /// 根据ID获取信息字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取信息字典")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(stateCodeEntityType.GetData(id.Value));
        }

        /// <summary>
        /// 根据ID获取信息字典详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取信息字典详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(stateCodeEntityType.GetData(id.Value));
        }

        /// <summary>
        /// 分页获取信息字典
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取信息字典")]
        public ActionResult GetPlistStateCodes(GetPlistStateCodes requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var dataDics = GetRequiredService<IStateCodeQuery>().GetPlist("StateCode", () =>
            {
                List<SqlParameter> ps;
                var filterString = new SqlFilterStringBuilder().FilterString(requestModel.filters, "a", out ps);
                if (!string.IsNullOrEmpty(filterString))
                {
                    filterString = " where " + filterString;
                }
                return new SqlFilter(filterString, ps.ToArray());
            }, requestModel);
            var data = new MiniGrid<Dictionary<string, object>> { total = requestModel.total.Value, data = dataDics };

            return this.JsonResult(data);
        }

        /// <summary>
        /// 更新状态码
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新状态码")]
        [HttpPost]
        public ActionResult Update(StateCodeUpdateInput requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            throw new ValidationException("暂不支持修改");
        }
    }
}
