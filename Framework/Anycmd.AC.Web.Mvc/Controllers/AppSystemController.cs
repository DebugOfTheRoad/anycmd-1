
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC.Infra.Messages;
    using Infra.ViewModels.AppSystemViewModels;
    using MiniUI;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 应用系统模型视图控制器<see cref="AppSystem"/>
    /// </summary>
    public class AppSystemController : AnycmdController
    {
        private readonly EntityTypeState appSystemEntityType;

        public AppSystemController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("AC", "AppSystem", out appSystemEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region 视图

        [By("xuexs")]
        [Description("权限应用系统管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("应用系统详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = AppSystemInfo.Create(appSystemEntityType.GetData(id));
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
        [Description("根据应用系统主键获取应用系统")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(appSystemEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据应用系统主键获取应用系统相信信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(AppSystemInfo.Create(appSystemEntityType.GetData(id.Value)));
        }

        [By("xuexs")]
        [Description("分页查询应用系统")]
        public ActionResult GetPlistAppSystems(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistAppSystems(requestModel);

            return this.JsonResult(new MiniGrid<AppSystemTr> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("添加应用系统")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Create(AppSystemCreateInput requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new AddAppSystemCommand(requestModel));

            return this.JsonResult(new ResponseData { id = requestModel.Id, success = true });
        }

        [By("xuexs")]
        [Description("更新应用系统")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(AppSystemUpdateInput requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new UpdateAppSystemCommand(requestModel));

            return this.JsonResult(new ResponseData { id = requestModel.Id, success = true });
        }

        [By("xuexs")]
        [Description("删除应用系统")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Delete(string id)
        {
            string[] ids = id.Split(',');
            var idArray = new Guid[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                Guid tmp;
                if (Guid.TryParse(ids[i], out tmp))
                {
                    idArray[i] = tmp;
                }
                else
                {
                    throw new ValidationException("意外的应用系统标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                AppHostInstance.Handle(new RemoveAppSystemCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
