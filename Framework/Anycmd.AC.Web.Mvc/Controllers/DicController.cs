
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Infra;
    using Infra.ViewModels.DicViewModels;
    using MiniUI;
    using Repositories;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 系统字典模型视图控制器<see cref="ACEntities.Dic"/>
    /// </summary>
    public class DicController : AnycmdController
    {
        private readonly EntityTypeState dicEntityType;

        public DicController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Dic", out dicEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region views
        [By("xuexs")]
        [Description("字典列表")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("系统字典管理")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = DicInfo.Create(dicEntityType.GetData(id));
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
        [Description("根据ID获取字典")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<Dic>>().GetByKey(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取字典详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(DicInfo.Create(dicEntityType.GetData(id.Value)));
        }

        [By("xuexs")]
        [Description("分页获取字典")]
        public ActionResult GetPlistDics(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistDics(requestModel);

            return this.JsonResult(new MiniGrid<DicTr> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("删除字典")]
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
                    throw new ValidationException("意外的系统字典标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveDic(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
