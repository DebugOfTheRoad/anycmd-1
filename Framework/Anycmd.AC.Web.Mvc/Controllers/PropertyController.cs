
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Infra.ViewModels.EntityTypeViewModels;
    using MiniUI;
    using Repositories;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 系统属性模型视图控制器<see cref="ACEntities.Property"/>
    /// </summary>
    public class PropertyController : AnycmdController
    {
        private readonly EntityTypeState propertyEntityType;

        public PropertyController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Property", out propertyEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        [By("xuexs")]
        [Description("字段")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("字段详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = propertyEntityType.GetData(id);
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
        [Description("根据ID获取字段")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(propertyEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取字段详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(propertyEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("获取字段提示信息")]
        public ActionResult Tooltip(Guid? propertyID)
        {
            if (propertyID.HasValue)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(propertyID.Value, out property))
                {
                    throw new ValidationException("意外的系统属性标识" + propertyID);
                }
                return this.PartialView("Partials/Tooltip", property);
            }
            else
            {
                return this.Content("无效的propertyID");
            }
        }

        [By("xuexs")]
        [Description("编辑字段帮助")]
        [ValidateInput(enableValidation: false)]
        public ActionResult TooltipEdit(string isInner, string tooltip, Guid? propertyID)
        {
            if (propertyID.HasValue)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(propertyID.Value, out property))
                {
                    throw new ValidationException("意外的系统属性标识" + propertyID);
                }
                if (Request.HttpMethod == "POST")
                {
                    var entity = GetRequiredService<IRepository<Property>>().GetByKey(propertyID.Value);
                    Host.Handle(new UpdatePropertyCommand(new PropertyUpdateInput
                    {
                        Code = entity.Code,
                        Description = entity.Description,
                        Icon = entity.Icon,
                        Id = entity.Id,
                        DicID = entity.DicID,
                        GuideWords = entity.GuideWords,
                        IsDetailsShow = entity.IsDetailsShow,
                        InputType = entity.InputType,
                        IsDeveloperOnly = entity.IsDeveloperOnly,
                        IsInput = entity.IsInput,
                        IsTotalLine = entity.IsTotalLine,
                        MaxLength = entity.MaxLength,
                        Name = entity.Name,
                        SortCode = entity.SortCode
                    }));
                    return this.JsonResult(new ResponseData { success = true });
                }
                else
                {
                    return this.PartialView(property);
                }
            }
            else
            {
                return this.Content("无效的propertyID");
            }
        }

        [By("xuexs")]
        [Description("根据实体类型标识分页获取实体属性")]
        public ActionResult GetPlistProperties(GetPlistProperties requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistProperties(requestModel);

            return this.JsonResult(new MiniGrid<PropertyTr> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("添加字段")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Create(PropertyCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.Handle(new AddPropertyCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("创建通用属性")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult CreateCommonProperties(Guid? entityTypeID)
        {
            if (!entityTypeID.HasValue)
            {
                throw new ValidationException("实体类型标识是必须的");
            }
            Host.Handle(new AddCommonPropertiesCommand(entityTypeID.Value));

            return this.JsonResult(new ResponseData { id = null, success = true });
        }

        [By("xuexs")]
        [Description("更新字段")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(PropertyUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.Handle(new UpdatePropertyCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("删除字段")]
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
                    throw new ValidationException("意外的字段标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.Handle(new RemovePropertyCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
