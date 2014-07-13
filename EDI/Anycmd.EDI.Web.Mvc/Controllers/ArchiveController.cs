
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Anycmd.Host;
    using Anycmd.Host.EDI;
    using Anycmd.Repositories;
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host.EDI.Entities;
    using MiniUI;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.ArchiveViewModels;

    /// <summary>
    /// 归档模型视图控制器<see cref="Anycmd.EDI.Repositories.Entities.Archive"/>
    /// </summary>
    public class ArchiveController : AnycmdController
    {
        private static readonly EntityTypeState archiveEntityType;

        static ArchiveController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("EDI", "Archive", out archiveEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        /// <summary>
        /// 归档主页
        /// </summary>
        /// <param name="ontologyCode"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("归档主页")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 归档详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("归档详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new ArchiveInfo(archiveEntityType.GetData(id));
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

        /// <summary>
        /// 根据ID获取归档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取归档")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<Archive>>().GetByKey(id.Value));
        }
        #endregion

        /// <summary>
        /// 根据ID获取归档详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取归档详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new ArchiveInfo(archiveEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 分页获取归档
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取归档")]
        public ActionResult GetPlistArchives(GetPlistArchives input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            OntologyDescriptor ontology;
            if (!NodeHost.Instance.Ontologies.TryGetOntology(input.ontologyCode, out ontology))
            {
                throw new ValidationException("意外的本体码" + input.ontologyCode);
            }
            EntityTypeState entityType;
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("EDI", "Archive", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.Archive");
            }
            foreach (var filter in input.filters)
            {
                PropertyState property;
                if (!AppHostInstance.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Archive实体类型属性" + filter.field);
                }
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = ontology.GetArchives().Select(a => ArchiveTr.Create(a)).AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }

            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<ArchiveTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 添加归档
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加归档")]
        [HttpPost]
        public ActionResult Create(ArchiveCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.AddArchive(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 修改归档
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("修改归档")]
        [HttpPost]
        public ActionResult Update(ArchiveUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.UpdateArchive(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 删除归档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除归档")]
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
                    throw new ValidationException("意外的归档标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                AppHostInstance.RemoveArchive(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
