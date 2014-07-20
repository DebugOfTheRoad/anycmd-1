
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.EDI;
    using Host.EDI.Entities;
    using Host.EDI.Messages;
    using MiniUI;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.ElementViewModels;

    /// <summary>
    /// 本体元素模型视图控制器<see cref="Element"/>
    /// </summary>
    public class ElementController : AnycmdController
    {
        private static readonly EntityTypeState elementEntityType;

        static ElementController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "Element", out elementEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        /// <summary>
        /// 本体元素管理
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("本体元素管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 本体元素详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("本体元素详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = ElementInfo.Create(elementEntityType.GetData(id));
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

        [By("xuexs")]
        [Description("checks")]
        public ViewResultBase Checks()
        {
            return ViewResult();
        }

        /// <summary>
        /// 获取字段提示信息
        /// </summary>
        /// <param name="elementID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取字段提示信息")]
        public ActionResult Tooltip(Guid? elementID)
        {
            if (elementID.HasValue)
            {
                return this.PartialView(
                    "Partials/Tooltip",
                    Host.Ontologies.GetElement(elementID.Value).Element);
            }
            else
            {
                return this.Content("无效的elementID");
            }
        }

        /// <summary>
        /// 编辑本体元素帮助
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="tooltip"></param>
        /// <param name="elementID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("编辑本体元素帮助")]
        [ValidateInput(enableValidation: false)]
        public ActionResult TooltipEdit(string isInner, string tooltip, Guid? elementID)
        {
            if (elementID.HasValue)
            {
                var element = Host.Ontologies.GetElement(elementID.Value);
                if (Request.HttpMethod == "POST")
                {
                    var entity = Host.Ontologies.GetElement(elementID.Value).Element;
                    if (entity != null)
                    {
                        Host.UpdateElement(
                            new ElementUpdateInput
                            {
                                AllowFilter = entity.AllowFilter,
                                AllowSort = entity.AllowSort,
                                Code = entity.Code,
                                Description = entity.Description,
                                FieldCode = entity.FieldCode,
                                GroupID = entity.GroupID,
                                Icon = entity.Icon,
                                Id = entity.Id,
                                InfoDicID = entity.InfoDicID,
                                InputHeight = entity.InputHeight,
                                InputType = entity.InputType,
                                InputWidth = entity.InputWidth,
                                IsDetailsShow = entity.IsDetailsShow,
                                IsEnabled = entity.IsEnabled,
                                IsExport = entity.IsExport,
                                IsGridColumn = entity.IsGridColumn,
                                IsImport = entity.IsImport,
                                IsInfoIDItem = entity.IsInfoIDItem,
                                IsInput = entity.IsInput,
                                IsTotalLine = entity.IsTotalLine,
                                MaxLength = entity.MaxLength,
                                Name = entity.Name,
                                Nullable = entity.Nullable,
                                Ref = entity.Ref,
                                Regex = entity.Regex,
                                SortCode = entity.SortCode,
                                Width = entity.Width,
                                Tooltip = tooltip
                            });
                    }
                    return this.JsonResult(new ResponseData { success = true });
                }
                else
                {
                    return this.PartialView(element.Element);
                }
            }
            else
            {
                return this.Content("无效的elementID");
            }
        }
        #endregion

        /// <summary>
        /// 根据ID获取本体元素
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取本体元素")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<Element>>().GetByKey(id.Value));
        }

        /// <summary>
        /// 根据ID获取本体元素详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据ID获取本体元素详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(ElementInfo.Create(elementEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 分页查询本体元素
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页查询本体元素")]
        public ActionResult GetPlistElements(GetPlistElements input)
        {
            if (!input.ontologyID.HasValue)
            {
                throw new ValidationException("必须传入本体标识");
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(input.ontologyID.Value, out ontology))
            {
                throw new ValidationException("意外的本体标识" + input.ontologyID);
            }
            EntityTypeState entityType;
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "Element", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.Element");
            }
            foreach (var filter in input.filters)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Element实体类型属性" + filter.field);
                }
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = ontology.Elements.Values.Select(a => ElementTr.Create(a.Element)).AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            if (input.groupID.HasValue)
            {
                queryable = queryable.Where(a => a.GroupID == input.groupID.Value);
            }

            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<ElementTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 根据本体元素ID分页获取动作
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据本体元素ID分页获取动作")]
        public ActionResult GetElementActions(GetElementActions input)
        {
            ElementDescriptor element;
            if (!Host.Ontologies.TryGetElement(input.elementID, out element))
            {
                throw new ValidationException("意外的本体元素标识" + input.elementID);
            }
            List<ElementAssignActionTr> list = new List<ElementAssignActionTr>();
            foreach (var action in element.Ontology.Actions.Values)
            {
                Guid id;
                string isAllowed;
                string isAudit;
                if (element.Element.ElementActions.ContainsKey(action.ActionVerb))
                {
                    var elementAction = element.Element.ElementActions[action.ActionVerb];
                    id = elementAction.Id;
                    isAllowed = elementAction.IsAllowed;
                    isAudit = elementAction.IsAudit;
                }
                else
                {
                    id = Guid.NewGuid();
                    isAllowed = AllowType.ImplicitAllow.ToName();
                    isAudit = AuditType.ImplicitAudit.ToName();
                }
                var elementAssignAction = new ElementAssignActionTr
                {
                    ActionID = action.Id,
                    ActionIsAllow = action.AllowType.ToName(),
                    ElementCode = element.Element.Code,
                    ElementID = element.Element.Id,
                    ElementName = element.Element.Name,
                    Id = id,
                    IsAllowed = isAllowed,
                    IsAudit = isAudit,
                    Name = action.Name,
                    OntologyID = action.OntologyID,
                    Verb = action.Verb
                };
                list.Add(elementAssignAction);
            }
            var data = new MiniGrid<ElementAssignActionTr> { total = list.Count, data = list };

            return this.JsonResult(data);
        }

        /// <summary>
        /// 获取节点关心的本体元素
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取节点关心的本体元素")]
        public ActionResult GetNodeElementCares(GetNodeElementCares input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            NodeDescriptor node;
            if (!Host.Nodes.TryGetNodeByID(input.nodeID.ToString(), out node))
            {
                throw new ValidationException("意外的节点标识" + input.nodeID);
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(input.ontologyID, out ontology))
            {
                throw new ValidationException("意外的本体标识" + input.ontologyID);
            }
            List<NodeAssignElementTr> data = new List<NodeAssignElementTr>();
            var nodeElementCares = Host.Nodes.GetNodeElementCares(node);
            foreach (var element in ontology.Elements.Values)
            {
                var id = Guid.NewGuid();
                var isAssigned = false;
                var isInfoIDItem = false;
                var nodeElementCare = nodeElementCares.FirstOrDefault(a => a.NodeID == input.nodeID && a.ElementID == element.Element.Id);
                if (nodeElementCare != null)
                {
                    id = nodeElementCare.Id;
                    isAssigned = true;
                    isInfoIDItem = nodeElementCare.IsInfoIDItem;
                }
                data.Add(new NodeAssignElementTr
                {
                    Code = element.Element.Code,
                    CreateOn = element.Element.CreateOn,
                    ElementID = element.Element.Id,
                    ElementIsInfoIDItem = element.Element.IsInfoIDItem,
                    Icon = element.Element.Icon,
                    Id = id,
                    IsAssigned = isAssigned,
                    IsEnabled = element.Element.IsEnabled,
                    IsInfoIDItem = isInfoIDItem,
                    Name = element.Element.Name,
                    NodeID = input.nodeID,
                    OntologyID = input.ontologyID,
                    SortCode = element.Element.SortCode
                });
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = data.AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<NodeAssignElementTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 添加本体元素
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加本体元素")]
        [HttpPost]
        public ActionResult Create(ElementCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddElement(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 更新本体元素
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新本体元素")]
        [HttpPost]
        public ActionResult Update(ElementUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateElement(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        // TODO:逻辑移动到应用服务层
        #region AddOrUpdateElementActions
        /// <summary>
        /// 添加或更新本体元素级动作权限
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或更新本体元素级动作权限")]
        [HttpPost]
        public ActionResult AddOrUpdateElementActions()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                var id = new Guid(row["Id"].ToString());
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";

                if (state == "modified" || state == "") //更新：_state为空或modified
                {
                    var inputModel = new ElementAction()
                    {
                        Id = new Guid(row["Id"].ToString()),
                        ElementID = new Guid(row["ElementID"].ToString()),
                        ActionID = new Guid(row["ActionID"].ToString()),
                        IsAudit = row["IsAudit"].ToString(),
                        IsAllowed = row["IsAllowed"].ToString()
                    };
                    ElementDescriptor element;
                    Host.Ontologies.TryGetElement(inputModel.ElementID, out element);
                    ElementAction entity = null;
                    if (element != null)
                    {
                        entity = new ElementAction
                        {
                            ActionID = inputModel.ActionID,
                            IsAllowed = inputModel.IsAllowed,
                            IsAudit = inputModel.IsAudit,
                            ElementID = element.Element.Id,
                            Id = inputModel.Id
                        };
                        Host.PublishEvent(new ElementActionUpdatedEvent(entity));
                    }
                    else
                    {
                        entity = new ElementAction();
                        entity.Id = inputModel.Id;
                        entity.ElementID = inputModel.ElementID;
                        entity.ActionID = inputModel.ActionID;
                        entity.IsAudit = inputModel.IsAudit;
                        entity.IsAllowed = inputModel.IsAllowed;
                        Host.PublishEvent(new ElementActionAddedEvent(entity));
                    }
                    Host.CommitEventBus();
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        // TODO:逻辑移动到应用服务层
        #region AddOrRemoveNodes
        /// <summary>
        /// 添加或移除关心节点
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或移除关心节点")]
        [HttpPost]
        public ActionResult AddOrRemoveNodes()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                var id = new Guid(row["Id"].ToString());
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";

                if (state == "modified" || state == "") //更新：_state为空或modified
                {
                    var inputModel = new NodeElementCare()
                    {
                        Id = new Guid(row["Id"].ToString()),
                        NodeID = new Guid(row["NodeID"].ToString()),
                        ElementID = new Guid(row["ElementID"].ToString())
                    };
                    bool isAssigned = bool.Parse(row["IsAssigned"].ToString());
                    NodeElementCare entity = GetRequiredService<IRepository<NodeElementCare>>().GetByKey(inputModel.Id);
                    bool isNew = true;
                    if (entity != null)
                    {
                        isNew = false;
                        if (!isAssigned)
                        {
                            GetRequiredService<IRepository<NodeElementCare>>().Remove(entity);
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        entity = new NodeElementCare();
                        entity.Id = inputModel.Id;

                    }
                    if (isAssigned)
                    {
                        entity.NodeID = inputModel.NodeID;
                        entity.ElementID = inputModel.ElementID;
                        if (isNew)
                        {
                            var count = GetRequiredService<IRepository<NodeElementCare>>().FindAll()
                                            .Where(a => a.ElementID == entity.ElementID
                                                && a.NodeID == entity.NodeID).Count();
                            if (count > 0)
                            {
                                throw new ValidationException("给定的节点已关心给定的本体元素，无需重复关心");
                            }
                            GetRequiredService<IRepository<NodeElementCare>>().Add(entity);
                        }
                        else
                        {
                            GetRequiredService<IRepository<NodeElementCare>>().Update(entity);
                        }
                    }
                    GetRequiredService<IRepository<NodeElementCare>>().Context.Commit();
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除本体元素
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除本体元素")]
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
                    throw new ValidationException("意外的本体元素标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveElement(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
        #endregion
    }
}
