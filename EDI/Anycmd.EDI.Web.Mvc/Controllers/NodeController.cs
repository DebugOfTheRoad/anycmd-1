
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Host.EDI;
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
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
    using ViewModels.NodeViewModels;

    /// <summary>
    /// 节点模型视图控制器<see cref="Node"/>
    /// </summary>
    public class NodeController : AnycmdController
    {
        private static readonly EntityTypeState nodeEntityType;

        static NodeController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "Node", out nodeEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults

        /// <summary>
        /// 节点管理
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("节点管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 节点详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("节点详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new NodeInfo(nodeEntityType.GetData(id));
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
        /// 用以控制权限，Action名和当前Action所在应用系统名、区域名、控制器名用来生成操作码和权限码。
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或修改")]
        public ViewResultBase Edit()
        {
            return ViewResult();
        }

        /// <summary>
        /// 关心本体元素
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("关心本体元素")]
        public ViewResultBase NodeElementCares()
        {
            return ViewResult();
        }

        /// <summary>
        /// 节点组织结构
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("节点组织结构")]
        public ViewResultBase Organizations()
        {
            return ViewResult();
        }

        /// <summary>
        /// 权限
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("权限")]
        public ViewResultBase Permissions()
        {
            return ViewResult();
        }

        /// <summary>
        /// 关心本本体的节点
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("关心本本体的节点")]
        public ViewResultBase OntologyNodeCares()
        {
            return ViewResult();
        }

        #endregion

        /// <summary>
        /// 根据节点ID获取节点详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据节点ID获取节点详细信息")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<Node>>().GetByKey(id.Value));
        }

        /// <summary>
        /// 根据节点ID获取节点详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据节点ID获取节点详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new NodeInfo(nodeEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 根据本体ID和节点ID分页获取节点动作
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据本体ID和节点ID分页获取节点动作")]
        public ActionResult GetPlistNodeActions(GetPlistNodeActions requestModel)
        {
            NodeDescriptor node;
            if (!Host.Nodes.TryGetNodeByID(requestModel.nodeID.ToString(), out node))
            {
                throw new ValidationException("意外的节点标识" + requestModel.nodeID);
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(requestModel.ontologyID, out ontology))
            {
                throw new ValidationException("意外的本体标识" + requestModel.ontologyID);
            }
            List<NodeAssignActionTr> list = new List<NodeAssignActionTr>();
            foreach (var action in ontology.Actions.Values)
            {
                Guid id;
                string isAllowed;
                string isAudit;
                var nodeActions = node.Node.NodeActions;
                if (nodeActions.ContainsKey(ontology) && nodeActions[ontology].ContainsKey(action.ActionVerb))
                {
                    var nodeAction = nodeActions[ontology][action.ActionVerb];
                    id = nodeAction.Id;
                    isAllowed = nodeAction.IsAllowed;
                    isAudit = nodeAction.IsAudit;
                }
                else
                {
                    id = Guid.NewGuid();
                    isAllowed = AllowType.ImplicitAllow.ToName();
                    isAudit = AuditType.ImplicitAudit.ToName();
                }
                NodeAssignActionTr item = new NodeAssignActionTr
                {
                    ActionID = action.Id,
                    ActionIsAllow = action.AllowType.ToName(),
                    ActionIsAudit = action.AuditType.ToName(),
                    ActionIsPersist = action.IsPersist,
                    Id = id,
                    Name = action.Name,
                    NodeCode = node.Node.Code,
                    NodeID = node.Node.Id,
                    NodeName = node.Node.Name,
                    OntologyID = action.OntologyID,
                    OntologyName = ontology.Ontology.Name,
                    Verb = action.Verb,
                    IsAllowed = isAllowed,
                    IsAudit = isAudit
                };
                list.Add(item);
            }
            var data = new MiniGrid<NodeAssignActionTr> { total = list.Count, data = list };

            return this.JsonResult(data);
        }

        /// <summary>
        /// 根据本体元素ID和节点ID分页获取动作
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据本体元素ID和节点ID分页获取动作")]
        public ActionResult GetPlistNodeElementActions(Guid? nodeID, Guid? elementID, GetPlistResult requestModel)
        {
            if (!nodeID.HasValue)
            {
                throw new ValidationException("nodeID是必须的");
            }
            if (!elementID.HasValue)
            {
                throw new ValidationException("elementID是必须的");
            }
            NodeDescriptor node;
            if (!Host.Nodes.TryGetNodeByID(nodeID.Value.ToString(), out node))
            {
                throw new ValidationException("意外的节点标识" + nodeID);
            }
            ElementDescriptor element;
            if (!Host.Ontologies.TryGetElement(elementID.Value, out element))
            {
                throw new ValidationException("意外的本体元素标识" + elementID);
            }
            var elementActions = element.Element.ElementActions;
            List<NodeElementAssignActionTr> data = new List<NodeElementAssignActionTr>();
            foreach (var action in element.Ontology.Actions.Values)
            {
                var elementAction = elementActions.ContainsKey(action.ActionVerb) ? elementActions[action.ActionVerb] : null;
                string elementActionAllowType = AllowType.ImplicitAllow.ToName();
                string elementActionAuditType = AuditType.ImplicitAudit.ToName();
                if (elementAction != null)
                {
                    elementActionAllowType = elementAction.AllowType.ToName();
                    elementActionAuditType = elementAction.AuditType.ToName();
                }
                var nodeElementActions = Host.Nodes.GetNodeElementActions(node, element).Values;
                var id = Guid.NewGuid();
                DateTime? createOn = null;
                bool isAllowed = false;
                bool isAudit = false;
                var nodeElementAction = nodeElementActions.FirstOrDefault(a => a.NodeID == node.Node.Id && a.ElementID == element.Element.Id && a.ActionID == action.Id);
                if (nodeElementAction != null)
                {
                    id = nodeElementAction.Id;
                    createOn = nodeElementAction.CreateOn;
                    isAllowed = nodeElementAction.IsAllowed;
                    isAudit = nodeElementAction.IsAudit;
                }
                data.Add(new NodeElementAssignActionTr
                {
                    Id = id,
                    Name = action.Name,
                    NodeID = node.Node.Id,
                    OntologyID = action.OntologyID,
                    SortCode = action.SortCode,
                    ActionID = action.Id,
                    ActionIsAllow = action.AllowType.ToName(),
                    ElementActionIsAllow = elementActionAllowType,
                    ElementActionIsAudit = elementActionAuditType,
                    ElementCode = element.Element.Code,
                    ElementID = element.Element.Id,
                    ElementName = element.Element.Name,
                    IsAllowed = isAllowed,
                    IsAudit = isAudit,
                    Verb = action.Verb
                });
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = data.AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<NodeElementAssignActionTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 根据本体ID分页获取关心该本体的节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ontologyID"></param>
        /// <param name="isAssigned"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据本体ID分页获取关心该本体的节点")]
        public ActionResult GetPlistNodeOntologyCares(Guid ontologyID, GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyID, out ontology))
            {
                throw new ValidationException("意外的本体标识" + ontologyID);
            }
            List<OntologyAssignNodeTr> data = new List<OntologyAssignNodeTr>();
            foreach (var node in Host.Nodes)
            {
                var nodeOntologyCares = Host.Nodes.GetNodeOntologyCares(node);
                var id = Guid.NewGuid();
                var isAssigned = false;
                DateTime? createOn = null;
                var nodeOntologyCare = nodeOntologyCares.FirstOrDefault(a => a.NodeID == node.Node.Id && a.OntologyID == ontology.Ontology.Id);
                if (nodeOntologyCare != null)
                {
                    id = nodeOntologyCare.Id;
                    isAssigned = true;
                    createOn = nodeOntologyCare.CreateOn;
                }
                data.Add(new OntologyAssignNodeTr
                {
                    Code = node.Node.Code,
                    CreateOn = createOn,
                    Icon = node.Node.Icon,
                    Id = id,
                    IsAssigned = isAssigned,
                    IsEnabled = node.Node.IsEnabled,
                    Name = node.Node.Name,
                    NodeID = node.Node.Id,
                    OntologyID = ontology.Ontology.Id,
                    SortCode = node.Node.SortCode
                });
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = data.AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<OntologyAssignNodeTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 分页获取节点
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取节点")]
        public ActionResult GetPlistNodes(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            EntityTypeState entityType;
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "Node", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.Node");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Node实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = Host.Nodes.Select(a => NodeTr.Create(a)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<NodeTr> { total = queryable.Count(), data = list });
        }

        #region GetOrganizationNodesByParentID
        /// <summary>
        /// 获取给定本体的组织结构
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="ontologyID"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取给定本体的组织结构")]
        public ActionResult GetOrganizationNodesByParentID(Guid? nodeID, Guid? ontologyID, Guid? parentID)
        {
            if (!nodeID.HasValue)
            {
                throw new ValidationException("未传入节点标识");
            }
            if (!ontologyID.HasValue)
            {
                throw new ValidationException("未传入本体标识");
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyID.Value, out ontology))
            {
                throw new ValidationException("意外的本体表示");
            }
            if (parentID == Guid.Empty)
            {
                parentID = null;
            }
            string parentCode = null;
            if (parentID.HasValue)
            {
                OrganizationState org;
                if (!Host.OrganizationSet.TryGetOrganization(parentID.Value, out org))
                {
                    throw new ValidationException("意外的组织结构标识" + parentID);
                }
                parentCode = org.Code;
            }
            var ontologyOrgs = ontology.Organizations;
            var orgs = Host.OrganizationSet.Where(a => ontologyOrgs.ContainsKey(a));
            var noos = GetRequiredService<IRepository<NodeOntologyOrganization>>().FindAll().Where(a => a.OntologyID == ontologyID.Value && a.NodeID == nodeID.Value).ToList<NodeOntologyOrganization>();
            return this.JsonResult(orgs.Where(a => string.Equals(a.ParentCode, parentCode, StringComparison.OrdinalIgnoreCase)).OrderBy(a => a.Code)
                .Select(a =>
                {
                    var norg = noos.FirstOrDefault(b => b.OrganizationID == a.Id);
                    bool @checked = true;
                    if (norg == null)
                    {
                        @checked = false;
                    }
                    else
                    {
                        @checked = true;
                    }

                    return new
                    {
                        a.Id,
                        a.Code,
                        a.Name,
                        ParentID = a.ParentCode,
                        isLeaf = Host.OrganizationSet.All(b => !a.Code.Equals(b.ParentCode, StringComparison.OrdinalIgnoreCase)),
                        expanded = false,
                        @checked = @checked,
                        NodeID = nodeID,
                        OntologyID = ontologyID
                    };
                }).ToList());
        }
        #endregion

        // TODO:逻辑移动到应用服务层
        #region AddOrRemoveOrganizations
        /// <summary>
        /// 添加或移除本体组织结构
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="ontologyID"></param>
        /// <param name="addOrganizationIDs"></param>
        /// <param name="removeOrganizationIDs"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或移除本体组织结构")]
        [HttpPost]
        public ActionResult AddOrRemoveOrganizations(Guid nodeID, Guid ontologyID, string addOrganizationIDs, string removeOrganizationIDs)
        {
            string[] addIDs = addOrganizationIDs.Split(',');
            string[] removeIDs = removeOrganizationIDs.Split(',');
            foreach (var item in addIDs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var mID = new Guid(item);
                    var entity = GetRequiredService<IRepository<NodeOntologyOrganization>>().FindAll().FirstOrDefault(a => a.OntologyID == ontologyID && a.NodeID == nodeID && a.OrganizationID == mID);
                    if (entity == null)
                    {
                        entity = new NodeOntologyOrganization()
                        {
                            Id = Guid.NewGuid(),
                            NodeID = nodeID,
                            OntologyID = ontologyID,
                            OrganizationID = mID
                        };
                        GetRequiredService<IRepository<NodeOntologyOrganization>>().Add(entity);
                    }
                }
            }
            foreach (var item in removeIDs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var mID = new Guid(item);
                    var entity = GetRequiredService<IRepository<NodeOntologyOrganization>>().FindAll().FirstOrDefault(a => a.OntologyID == ontologyID && a.NodeID == nodeID && a.OrganizationID == mID);
                    if (entity != null)
                    {
                        GetRequiredService<IRepository<NodeOntologyOrganization>>().Remove(entity);
                    }
                }
            }
            GetRequiredService<IRepository<NodeOntologyOrganization>>().Context.Commit();

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        /// <summary>
        /// 添加新节点
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加新节点")]
        [HttpPost]
        public ActionResult Create(NodeCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddNode(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 更新节点
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新节点")]
        [HttpPost]
        public ActionResult Update(NodeUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateNode(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        // TODO:逻辑移动到应用服务层
        #region UpdateNodes
        /// <summary>
        /// 更新节点配置
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新节点配置")]
        [HttpPost]
        public ActionResult UpdateNodes()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";
                //更新：_state为空或modified
                if (state == "modified" || state == "")
                {
                    var id = new Guid(row["Id"].ToString());
                    var isExecuteEnabled = bool.Parse(row["IsExecuteEnabled"].ToString());
                    var isProduceEnabled = bool.Parse(row["IsProduceEnabled"].ToString());
                    var isReceiveEnabled = bool.Parse(row["IsReceiveEnabled"].ToString());
                    var isTransferEnabled = bool.Parse(row["IsDistributeEnabled"].ToString());
                    Node entity = GetRequiredService<IRepository<Node>>().GetByKey(id);
                    if (entity != null)
                    {
                        entity.IsExecuteEnabled = isExecuteEnabled;
                        entity.IsProduceEnabled = isProduceEnabled;
                        entity.IsReceiveEnabled = isReceiveEnabled;
                        entity.IsDistributeEnabled = isTransferEnabled;
                        GetRequiredService<IRepository<Node>>().Update(entity);
                        GetRequiredService<IRepository<Node>>().Context.Commit();
                        Host.PublishEvent(new NodeUpdatedEvent(entity, new NodeUpdateInput
                        {
                            Abstract = entity.Abstract,
                            AnycmdApiAddress = entity.AnycmdApiAddress,
                            AnycmdWSAddress = entity.AnycmdWSAddress,
                            BeatPeriod = entity.BeatPeriod,
                            Code = entity.Code,
                            Description = entity.Description,
                            Email = entity.Email,
                            IsEnabled = entity.IsEnabled,
                            Icon = entity.Icon,
                            Id = entity.Id,
                            Mobile = entity.Mobile,
                            Name = entity.Name,
                            Organization = entity.Organization,
                            PublicKey = entity.PublicKey,
                            QQ = entity.QQ,
                            SecretKey = entity.SecretKey,
                            SortCode = entity.SortCode,
                            Steward = entity.Steward,
                            Telephone = entity.Telephone,
                            TransferID = entity.TransferID
                        }));
                        Host.CommitEventBus();
                    }
                    else
                    {
                        throw new CoreException("意外的节点");
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        #region AddOrRemoveOntologies
        /// <summary>
        /// 添加或移除关心本体
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或移除关心本体")]
        [HttpPost]
        public ActionResult AddOrRemoveOntologies()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";
                var id = new Guid(row["Id"].ToString());
                if (state == "modified" || state == "") //更新：_state为空或modified
                {
                    bool isAssigned = bool.Parse(row["IsAssigned"].ToString());
                    NodeOntologyCare entity = GetRequiredService<IRepository<NodeOntologyCare>>().GetByKey(id);
                    if (entity != null)
                    {
                        if (!isAssigned)
                        {
                            Host.RemoveNodeOntologyCare(id);
                        }
                    }
                    else if (isAssigned)
                    {
                        Host.AddNodeOntologyCare(new NodeOntologyCareCreateInput
                        {
                            Id = id,
                            NodeID = new Guid(row["NodeID"].ToString()),
                            OntologyID = new Guid(row["OntologyID"].ToString())
                        });
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        // TODO:逻辑移动到应用服务层
        #region AddOrRemoveElementCares
        /// <summary>
        /// 添加或移除关心本体元素
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或移除关心本体元素")]
        [HttpPost]
        public ActionResult AddOrRemoveElementCares()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";
                var id = new Guid(row["Id"].ToString());
                if (state == "modified" || state == "") //更新：_state为空或modified
                {
                    bool isAssigned = bool.Parse(row["IsAssigned"].ToString());
                    NodeElementCare entity = GetRequiredService<IRepository<NodeElementCare>>().GetByKey(id);
                    if (entity != null)
                    {
                        if (!isAssigned)
                        {
                            Host.RemoveNodeElementCare(id);
                        }
                        else
                        {
                            // TODO:IsInfoIDItem字段需要update
                        }
                    }
                    else if (isAssigned)
                    {
                        Host.AddNodeElementCare(new NodeElementCareCreateInput
                        {
                            Id = id,
                            NodeID = new Guid(row["NodeID"].ToString()),
                            ElementID = new Guid(row["ElementID"].ToString()),
                            IsInfoIDItem = bool.Parse(row["IsInfoIDItem"].ToString())
                        });
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        // TODO:逻辑移动到应用服务层
        #region AddOrUpdateNodeActions
        /// <summary>
        /// 添加或更新本体级权限
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或更新本体级权限")]
        [HttpPost]
        public ActionResult AddOrUpdateNodeActions()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                var id = new Guid(row["Id"].ToString());
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";
                //更新：_state为空或modified
                if (state == "modified" || state == "")
                {
                    var inputModel = new NodeAction()
                    {
                        Id = new Guid(row["Id"].ToString()),
                        NodeID = new Guid(row["NodeID"].ToString()),
                        ActionID = new Guid(row["ActionID"].ToString()),
                        IsAllowed = row["IsAllowed"].ToString(),
                        IsAudit = row["IsAudit"].ToString()
                    };
                    NodeDescriptor nodeDescriptor;
                    if (!Host.Nodes.TryGetNodeByID(inputModel.NodeID.ToString(), out nodeDescriptor))
                    {
                        throw new ValidationException("意外的节点标识" + inputModel.NodeID);
                    }
                    NodeAction entity = null;
                    if (nodeDescriptor != null)
                    {
                        entity = new NodeAction
                        {
                            Id = inputModel.Id,
                            ActionID = inputModel.ActionID,
                            IsAllowed = inputModel.IsAllowed,
                            IsAudit = inputModel.IsAudit,
                            NodeID = inputModel.NodeID
                        };
                        Host.PublishEvent(new NodeActionUpdatedEvent(entity));
                    }
                    else
                    {
                        entity = new NodeAction();
                        entity.Id = inputModel.Id;
                        entity.NodeID = inputModel.NodeID;
                        entity.ActionID = inputModel.ActionID;
                        entity.IsAudit = inputModel.IsAudit;
                        entity.IsAllowed = inputModel.IsAllowed;
                        Host.PublishEvent(new NodeActionAddedEvent(entity));
                    }
                    Host.CommitEventBus();
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        // TODO:逻辑移动到应用服务层
        #region AddOrUpdateNodeElementActions
        /// <summary>
        /// 添加或更新节点本体元素级权限
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或更新节点本体元素级权限")]
        [HttpPost]
        public ActionResult AddOrUpdateNodeElementActions()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                var id = new Guid(row["Id"].ToString());
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";
                //更新：_state为空或modified
                if (state == "modified" || state == "")
                {
                    var inputModel = new NodeElementAction()
                    {
                        Id = new Guid(row["Id"].ToString()),
                        NodeID = new Guid(row["NodeID"].ToString()),
                        ElementID = new Guid(row["ElementID"].ToString()),
                        ActionID = new Guid(row["ActionID"].ToString()),
                        IsAllowed = bool.Parse(row["IsAllowed"].ToString()),
                        IsAudit = bool.Parse(row["IsAudit"].ToString())
                    };
                    NodeElementAction entity = GetRequiredService<IRepository<NodeElementAction>>().GetByKey(inputModel.Id);
                    if (entity != null)
                    {
                        entity.IsAudit = inputModel.IsAudit;
                        entity.IsAllowed = inputModel.IsAllowed;
                        GetRequiredService<IRepository<NodeElementAction>>().Update(entity);
                    }
                    else
                    {
                        entity = new NodeElementAction();
                        entity.Id = inputModel.Id;
                        entity.NodeID = inputModel.NodeID;
                        entity.ElementID = inputModel.ElementID;
                        entity.ActionID = inputModel.ActionID;
                        entity.IsAudit = inputModel.IsAudit;
                        entity.IsAllowed = inputModel.IsAllowed;
                        var count = GetRequiredService<IRepository<NodeElementAction>>().FindAll()
                            .Where(a => a.NodeID == entity.NodeID
                                    && a.ElementID == entity.ElementID
                                    && a.ActionID == entity.ActionID).Count();
                        if (count > 0)
                        {
                            throw new ValidationException("给定的节点已拥有给定的动作，无需重复关联");
                        }
                        GetRequiredService<IRepository<NodeElementAction>>().Add(entity);
                    }
                    GetRequiredService<IRepository<NodeElementAction>>().Context.Commit();
                    Host.CommitEventBus();
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除节点")]
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
                    throw new ValidationException("意外的节点标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveNode(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
