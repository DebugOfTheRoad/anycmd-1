
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.EDI;
    using Host.EDI.Entities;
    using MiniUI;
    using Repositories;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.InfoDicViewModels;

    /// <summary>
    /// 信息字典项模型视图控制器<see cref="InfoDicItem"/>
    /// </summary>
    public class InfoDicItemController : AnycmdController
    {
        private static readonly EntityTypeState infoDicItemEntityType;

        static InfoDicItemController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "InfoDicItem", out infoDicItemEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }
        
        #region ViewResults
        /// <summary>
        /// 信息字典项管理
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("信息字典项管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 信息字典项详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("信息字典项详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new InfoDicItemInfo(Host, infoDicItemEntityType.GetData(id));
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
        /// 根据ID获取信息字典项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取信息字典项")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<InfoDicItem>>().GetByKey(id.Value));
        }

        /// <summary>
        /// 根据ID获取信息字典项详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取信息字典项详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new InfoDicItemInfo(Host, infoDicItemEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 根据字典主键获取字典项列表
        /// </summary>
        /// <param name="dicID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据字典主键获取字典项列表")]
        public ActionResult GetDicItemsByDicID(Guid dicID)
        {
            InfoDicState infoDic;
            if (!NodeHost.Instance.InfoDics.TryGetInfoDic(dicID, out infoDic))
            {
                return this.JsonResult(null);
            }
            var data = NodeHost.Instance.InfoDics.GetInfoDicItems(infoDic).Select(d => new { code = d.Code, name = d.Name });

            return this.JsonResult(data);
        }

        /// <summary>
        /// 分页查询字典项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页查询字典项")]
        public ActionResult GetPlistInfoDicItems(Guid? dicID, GetPlistResult input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            if (!dicID.HasValue)
            {
                throw new ValidationException("infoDicID参数是必须的");
            }
            InfoDicState infoDic;
            if (!NodeHost.Instance.InfoDics.TryGetInfoDic(dicID.Value, out infoDic))
            {
                throw new ValidationException("意外的信息字典标识" + dicID);
            }
            EntityTypeState entityType;
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "InfoDicItem", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.InfoDicItem");
            }
            foreach (var filter in input.filters)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的InfoDicItem实体类型属性" + filter.field);
                }
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = NodeHost.Instance.InfoDics.GetInfoDicItems(infoDic).Select(a => InfoDicItemTr.Create(a)).AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<InfoDicItemTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 添加字典项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加字典项")]
        [HttpPost]
        public ActionResult Create(InfoDicItemCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddInfoDicItem(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 更新字典项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新字典项")]
        [HttpPost]
        public ActionResult Update(InfoDicItemUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateInfoDicItem(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 删除字典项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除字典项")]
        [HttpPost]
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
                Host.RemoveInfoDicItem(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
