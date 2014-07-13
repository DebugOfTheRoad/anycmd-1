
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Anycmd.Repositories;
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.EDI.Messages;
    using Host.EDI.Entities;
    using MiniUI;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.ProcessViewModels;

    /// <summary>
    /// 进程模型视图控制器<see cref="Process"/>
    /// </summary>
    public class ProcessController : AnycmdController
    {
        private static readonly EntityTypeState processEntityType;

        static ProcessController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("EDI", "Process", out processEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        /// <summary>
        /// 进程主页
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("进程主页")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 进程详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("进程详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new ProcessInfo(processEntityType.GetData(id));
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
        /// 根据ID获取进程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取进程")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<Process>>().GetByKey(id.Value));
        }

        /// <summary>
        /// 根据ID获取进程详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取进程详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new ProcessInfo(processEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 分页获取进程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取进程")]
        public ActionResult GetPlistProcesses(GetPlistResult input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            EntityTypeState entityType;
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("EDI", "Process", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.Process");
            }
            foreach (var filter in input.filters)
            {
                PropertyState property;
                if (!AppHostInstance.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Process实体类型属性" + filter.field);
                }
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = NodeHost.Instance.Processs.Select(a => ProcessTr.Create(a)).AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<ProcessTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 添加进程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加进程")]
        [HttpPost]
        public ActionResult Create(ProcessCreateInput input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new AddProcessCommand(input));

            return this.JsonResult(new ResponseData { success = true, id = input.Id.Value });
        }

        /// <summary>
        /// 更新进程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新进程")]
        [HttpPost]
        public ActionResult Update(ProcessUpdateInput input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new UpdateProcessCommand(input));

            return this.JsonResult(new ResponseData { success = true, id = input.Id });
        }
    }
}
