
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using AC.Infra;
    using Anycmd.Repositories;
    using Anycmd.Web.Mvc;
    using MiniUI;
    using Host.EDI.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Query;
    using System.Data.SqlClient;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.PluginViewModels;
    using Exceptions;
    using Anycmd.Host;

    /// <summary>
    /// 插件模型视图控制器<see cref="Plugin"/>
    /// </summary>
    public class PluginController : AnycmdController
    {
        private static readonly EntityTypeState pluginEntityType;

        static PluginController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("EDI", "Plugin", out pluginEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        /// <summary>
        /// 插件主页
        /// </summary>
        /// <param name="ontologyCode"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("插件主页")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 插件详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("插件详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new PluginInfo(AppHostInstance, pluginEntityType.GetData(id));
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
        /// 根据ID获取命令插件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取命令插件")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<Plugin>>().GetByKey(id.Value));
        }

        /// <summary>
        /// 根据ID获取命令插件详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取命令插件详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new PluginInfo(AppHostInstance, pluginEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 分页获取命令插件
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取命令插件")]
        public ActionResult GetPlistPlugins(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var dataDics = GetRequiredService<IPluginQuery>().GetPlist("Plugin", () =>
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
    }
}
