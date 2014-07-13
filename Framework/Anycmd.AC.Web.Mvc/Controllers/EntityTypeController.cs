
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC.Infra.Messages;
    using Infra.ViewModels.EntityTypeViewModels;
    using MiniUI;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 系统模型模型视图控制器<see cref="ACEntities.Model"/>
    /// </summary>
    public class EntityTypeController : AnycmdController
    {
        private readonly EntityTypeState entityTypeEntityType;

        public EntityTypeController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("AC", "EntityType", out entityTypeEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewPages

        [By("xuexs")]
        [Description("实体类型管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("实体类型详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new EntityTypeInfo(entityTypeEntityType.GetData(id));
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
        [Description("根据ID获取实体类型")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(entityTypeEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取实体类型详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new EntityTypeInfo(entityTypeEntityType.GetData(id.Value)));
        }

        [By("xuexs")]
        [Description("分页获取实体类型")]
        public ActionResult GetPlistEntityTypes(GetPlistResult requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistEntityTypes(requestData);

            return this.JsonResult(new MiniGrid<EntityTypeTr> { total = requestData.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("添加实体类型")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Create(EntityTypeCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new AddEntityTypeCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("更新实体类型")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(EntityTypeUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new UpdateEntityTypeCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("删除实体类型")]
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
                    throw new ValidationException("意外的实体类型标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                AppHostInstance.Handle(new RemoveEntityTypeCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
