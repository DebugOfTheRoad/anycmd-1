
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using AC.Infra;
    using Anycmd.Repositories;
    using Anycmd.Web.Mvc;
    using Anycmd.Host.EDI.Info;
    using MiniUI;
    using Host.EDI.Entities;
    using Exceptions;
    using Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Transactions;
    using ViewModel;
    using ViewModels.InfoConstraintViewModels;
    using Anycmd.Host;
    using Anycmd.Host.EDI;

    /// <summary>
    /// 信息验证器模型视图控制器<see cref="InfoRule"/>
    /// </summary>
    public class InfoRuleController : AnycmdController
    {
        private static readonly EntityTypeState infoRuleEntityType;

        static InfoRuleController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "InfoRule", out infoRuleEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        /// <summary>
        /// 信息项验证器主页
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("信息项验证器主页")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 信息项验证器详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("信息项验证器详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new InfoRuleInfo(infoRuleEntityType.GetData(id));
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
        /// 信息项验证器
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="elementID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("信息规则(Rule)")]
        public ViewResultBase ElementInfoRules(string isInner, Guid? elementID)
        {
            if (!string.IsNullOrEmpty(isInner))
            {
                return PartialView("Partials/ElementInfoRules");
            }
            return View();
        }

        #endregion

        /// <summary>
        /// 根据ID获取信息项验证器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取信息项验证器")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<InfoRule>>().GetByKey(id.Value));
        }

        /// <summary>
        /// 根据ID获取信息项验证器详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取信息项验证器详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new InfoRuleInfo(infoRuleEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 分页获取信息项验证器
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取信息项验证器")]
        public ActionResult GetPlistInfoRules(GetPlistInfoRules requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            EntityTypeState infoRuleEntityType;
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "InfoRule", out infoRuleEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestData.filters)
            {
                PropertyState property;
                if (!infoRuleEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的AppSystem实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = NodeHost.Instance.InfoRules.Select(a => InfoRuleTr.Create(Host, a)).AsQueryable();
            foreach (var filter in requestData.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestData.total = queryable.Count();
            var data = queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<InfoRuleTr> { total = requestData.total.Value, data = data });
        }

        /// <summary>
        /// 分页获取元素信息项验证器
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取元素信息项验证器")]
        public ActionResult GetPlistElementInfoRules(GetPlistElementInfoRules requestModel)
        {
            ElementDescriptor element;
            if (!NodeHost.Instance.Ontologies.TryGetElement(requestModel.elementID, out element))
            {
                throw new ValidationException("意外的本体元素标识" + requestModel.elementID);
            }
            List<ElementInfoRuleTr> list = new List<ElementInfoRuleTr>();
            foreach (var item in element.Element.ElementInfoRules)
            {
                InfoRuleState infoRule;
                if (NodeHost.Instance.InfoRules.TryGetInfoRule(item.InfoRuleID, out infoRule))
                {
                    list.Add(new ElementInfoRuleTr
                    {
                        InfoRuleID = infoRule.Id,
                        AuthorCode = infoRule.InfoRule.Author,
                        CreateOn = item.CreateOn,
                        ElementID = element.Element.Id,
                        FullName = infoRule.GetType().Name,
                        Id = item.Id,
                        Name = infoRule.InfoRule.Name,
                        Title = infoRule.InfoRule.Title,
                        SortCode = item.SortCode,
                        IsEnabled = item.IsEnabled
                    });
                }
            }
            var data = new MiniGrid<ElementInfoRuleTr> { total = list.Count, data = list };

            return this.JsonResult(data);
        }

        /// <summary>
        /// 更新信息项验证器
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新信息项验证器")]
        [HttpPost]
        public ActionResult Update(InfoRuleInput requestModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            var responseResult = new ResponseData { success = false };
            using (var coordinator = TransactionCoordinatorFactory.Create(GetRequiredService<IRepository<InfoRule>>().Context))
            {
                var entity = GetRequiredService<IRepository<InfoRule>>().GetByKey(requestModel.Id);
                entity.IsEnabled = requestModel.IsEnabled;
                GetRequiredService<IRepository<InfoRule>>().Update(entity);
                responseResult.id = entity.Id.ToString();
                responseResult.success = true;

                coordinator.Commit();
            }

            return this.JsonResult(responseResult);
        }
    }
}
