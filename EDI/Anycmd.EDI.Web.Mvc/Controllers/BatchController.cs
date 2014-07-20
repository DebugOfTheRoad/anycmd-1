
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Anycmd.Host;
    using Anycmd.Host.EDI;
    using Anycmd.Repositories;
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host.EDI.Entities;
    using MiniUI;
    using Query;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.BatchViewModels;

    /// <summary>
    /// 批模型视图控制器<see cref="Anycmd.EDI.Repositories.Entities.Batch"/>
    /// </summary>
    public class BatchController : AnycmdController
    {
        private static readonly EntityTypeState batchEntityType;

        static BatchController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "Batch", out batchEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        /// <summary>
        /// 批主页
        /// </summary>
        /// <param name="ontologyCode"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("批主页")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 批详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("批详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new BatchInfo(batchEntityType.GetData(id));
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
        /// 根据ID获取批
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取批")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<Batch>>().GetByKey(id.Value));
        }

        /// <summary>
        /// 根据ID获取批详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取批详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new BatchInfo(batchEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 分页获取批
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取批")]
        public ActionResult GetPlistBatches(GetPlistBatchs input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Guid? ontologyID = null;
            if (string.IsNullOrEmpty(input.ontologyCode))
            {
                ontologyID = null;
            }
            else
            {
                OntologyDescriptor ontology;
                if (!Host.Ontologies.TryGetOntology(input.ontologyCode, out ontology))
                {
                    throw new ValidationException("意外的本体码" + input.ontologyCode);
                }
                ontologyID = ontology.Ontology.Id;
            }
            var pagingData = new PagingInput(input.pageIndex.Value
                , input.pageSize.Value, input.sortField, input.sortOrder);
            if (ontologyID != null && ontologyID.HasValue)
            {
                input.filters.Insert(0, FilterData.EQ("OntologyID", ontologyID.Value));
            }
            var data = GetRequiredService<IBatchQuery>().GetPlist("Batch", () =>
            {
                List<SqlParameter> ps;
                var filterString = new SqlFilterStringBuilder().FilterString(input.filters, "a", out ps);
                if (!string.IsNullOrEmpty(filterString))
                {
                    filterString = " where " + filterString;
                }
                return new SqlFilter(filterString, ps.ToArray());
            }, pagingData);

            return this.JsonResult(new MiniGrid<BatchTr> { total = pagingData.total.Value, data = data.Select(a => new BatchTr(a)) });
        }

        /// <summary>
        /// 添加批
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加批")]
        [HttpPost]
        public ActionResult Create(BatchCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddBatch(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 修改批
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("修改批")]
        [HttpPost]
        public ActionResult Update(BatchUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateBatch(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 删除批
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除批")]
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
                    throw new ValidationException("意外的批标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveBatch(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
