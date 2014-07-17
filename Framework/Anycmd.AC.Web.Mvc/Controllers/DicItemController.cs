
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
    using Infra.ViewModels.DicViewModels;
    using MiniUI;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 系统字典项模型视图控制器<see cref="ACEntities.DicItem"/>
    /// </summary>
    public class DicItemController : AnycmdController
    {
        private readonly EntityTypeState dicItemEntityType;

        public DicItemController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "DicItem", out dicItemEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        [By("xuexs")]
        [Description("字典管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("字典详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = DicItemInfo.Create(dicItemEntityType.GetData(id));
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
        [Description("根据ID获取字典项")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(dicItemEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取字典项详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(DicItemInfo.Create(dicItemEntityType.GetData(id.Value)));
        }

        [By("xuexs")]
        [Description("分页获取字典项")]
        public ActionResult GetPlistDicItems(GetPlistDicItems requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistDicItems(requestModel);

            return this.JsonResult(new MiniGrid<DicItemTr> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("添加字典项")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Create(DicItemCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddDicItem(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("更新字典项")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(DicItemUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateDicItem(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("删除字典项")]
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
                    throw new ValidationException("意外的字典项标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveDicItem(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
