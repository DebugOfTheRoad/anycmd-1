
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
    /// 信息字典模型视图控制器<see cref="InfoDic"/>
    /// </summary>
    public class InfoDicController : AnycmdController
    {
        private static readonly EntityTypeState infoDicEntityType;

        static InfoDicController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "InfoDic", out infoDicEntityType))
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
                    var data = new InfoDicInfo(Host, infoDicEntityType.GetData(id));
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
            return this.JsonResult(GetRequiredService<IRepository<InfoDic>>().GetByKey(id.Value));
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
            return this.JsonResult(new InfoDicInfo(Host, infoDicEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 分页获取信息字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取信息字典")]
        public ActionResult GetPlistInfoDics(GetPlistResult input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            EntityTypeState entityType;
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "InfoDic", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.InfoDic");
            }
            foreach (var filter in input.filters)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的InfoDic实体类型属性" + filter.field);
                }
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = Host.InfoDics.Select(a => InfoDicTr.Create(a)).AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<InfoDicTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 添加信息字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加信息字典")]
        [HttpPost]
        public ActionResult Create(InfoDicCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddInfoDic(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 更新信息字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新信息字典")]
        [HttpPost]
        public ActionResult Update(InfoDicUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateInfoDic(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 删除信息字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除信息字典")]
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
                    throw new ValidationException("意外的字典标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveInfoDic(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
