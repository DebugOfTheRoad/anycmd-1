
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC.Infra.Messages;
    using Infra.ViewModels.ButtonViewModels;
    using MiniUI;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 按钮模型视图控制器<see cref="Anycmd.AC.ACEntities.Button"/>
    /// </summary>
    public class ButtonController : AnycmdController
    {
        private readonly EntityTypeState buttonEntityType;

        public ButtonController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("AC", "Button", out buttonEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewPages

        [By("xuexs")]
        [Description("按钮管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("按钮详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = ButtonInfo.Create(buttonEntityType.GetData(id));
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

        #region Get
        [By("xuexs")]
        [Description("根据ID获取按钮")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(buttonEntityType.GetData(id.Value));
        }
        #endregion

        #region GetInfo
        [By("xuexs")]
        [Description("根据ID获取按钮详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(ButtonInfo.Create(buttonEntityType.GetData(id.Value)));
        }
        #endregion

        #region GetPlistButtons
        [By("xuexs")]
        [Description("分页获取按钮")]
        public ActionResult GetPlistButtons(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistButtons(requestModel);

            return this.JsonResult(new MiniGrid<ButtonTr> { total = requestModel.total.Value, data = data });
        }
        #endregion

        #region GetPlistPageButtons
        [By("xuexs")]
        [Description("分页获取页面按钮")]
        public ActionResult GetPlistPageButtons(GetPlistPageButtons requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistPageButtons(requestData);

            return this.JsonResult(new MiniGrid<PageAssignButtonTr> { total = requestData.total.Value, data = data });
        }
        #endregion

        #region Add
        [By("xuexs")]
        [Description("添加按钮")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Create(ButtonCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new AddButtonCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }
        #endregion

        #region Update
        [By("xuexs")]
        [Description("更新按钮")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(ButtonUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new UpdateButtonCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }
        #endregion

        #region Delete
        [By("xuexs")]
        [Description("删除按钮")]
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
                AppHostInstance.Handle(new RemoveButtonCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
        #endregion
    }
}
