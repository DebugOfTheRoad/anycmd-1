
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Infra.ViewModels.ResourceViewModels;
    using MiniUI;
    using Repositories;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 系统资源类型模型视图控制器<see cref="ACEntities.Resource"/>
    /// </summary>
    public class ResourceTypeController : AnycmdController
    {
        private readonly EntityTypeState entityType;

        public ResourceTypeController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "ResourceType", out entityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        [By("xuexs")]
        [Description("资源类型集")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("资源类型详细信息")]
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
        [Description("根据ID获取资源类型")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<ResourceType>>().GetByKey(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取资源类型详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(entityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("分页获取资源类型")]
        public ActionResult GetPlistResources(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistResources(requestModel);

            return this.JsonResult(new MiniGrid<ResourceTypeTr> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("根据区域分页获取资源类型")]
        public ActionResult GetPlistAppSystemResources(GetPlistAreaResourceTypes requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistAppSystemResources(requestModel);

            return this.JsonResult(new MiniGrid<ResourceTypeTr> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("添加资源类型")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Create(ResourceTypeCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.Handle(new AddResourceCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("更新资源类型")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(ResourceTypeUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.Handle(new UpdateResourceCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("删除资源类型")]
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
                    throw new ValidationException("意外的资源标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.Handle(new RemoveResourceTypeCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
