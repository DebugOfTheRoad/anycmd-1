using System;

namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.EDI;
    using Host.EDI.Entities;
    using Host.EDI.Hecp;
    using Host.EDI.Messages;
    using MiniUI;
    using Repositories;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.NodeViewModels;
    using ViewModels.OntologyViewModels;

    /// <summary>
    /// 本体模型视图控制器<see cref="Ontology"/>
    /// </summary>
    public class OntologyController : AnycmdController
    {
        private static readonly EntityTypeState ontologyEntityType;

        static OntologyController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "Ontology", out ontologyEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewPages

        /// <summary>
        /// 本体管理
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("本体管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        /// <summary>
        /// 本体详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <param name="isTooltip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("本体详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new OntologyInfo(Host, ontologyEntityType.GetData(id));
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
        /// 信息组
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("信息组")]
        public ViewResultBase InfoGroups()
        {
            return ViewResult();
        }

        /// <summary>
        /// 动作
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("动作")]
        public ViewResultBase Actions()
        {
            return ViewResult();
        }

        /// <summary>
        /// 事件主题
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("事件主题")]
        public ViewResultBase Topics()
        {
            return ViewResult();
        }

        /// <summary>
        /// 本体元素
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("本体元素")]
        public ViewResultBase Elements()
        {
            return ViewResult();
        }

        /// <summary>
        /// 组织结构
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("组织结构")]
        public ViewResultBase Organizations()
        {
            return ViewResult();
        }

        #endregion

        /// <summary>
        /// 获取本体详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取本体详细信息")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IRepository<Ontology>>().GetByKey(id.Value));
        }

        /// <summary>
        /// 获取本体详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取本体详细信息")]
        public ActionResult GetInfo(Guid? id, string code)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new OntologyInfo(Host, ontologyEntityType.GetData(id.Value)));
        }

        /// <summary>
        /// 分页获取本体
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取本体")]
        public ActionResult GetPlistOntologies(GetPlistResult input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            EntityTypeState entityType;
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "Ontology", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.Ontology");
            }
            foreach (var filter in input.filters)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Ontology实体类型属性" + filter.field);
                }
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = Host.Ontologies.Select(a => OntologyTr.Create(a)).AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<OntologyTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 添加新本体
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加新本体")]
        [HttpPost]
        public ActionResult Create(OntologyCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddOntology(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 更新本体
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新本体")]
        [HttpPost]
        public ActionResult Update(OntologyUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateOntology(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 获取信息组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取信息组")]
        public ActionResult GetInfoGroup(Guid id)
        {
            var data = GetRequiredService<IRepository<Ontology>>().Context.Query<InfoGroup>().FirstOrDefault(a => a.Id == id);

            return this.JsonResult(data);
        }

        /// <summary>
        /// 获取动作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取动作")]
        public ActionResult GetAction(Guid id)
        {
            var data = GetRequiredService<IRepository<Ontology>>().Context.Query<Action>().FirstOrDefault(a => a.Id == id);

            return this.JsonResult(data);
        }

        /// <summary>
        /// 获取事件主题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取事件主题")]
        public ActionResult GetTopic(Guid id)
        {
            var data = GetRequiredService<IRepository<Ontology>>().Context.Query<Topic>().FirstOrDefault(a => a.Id == id);

            return this.JsonResult(data);
        }

        /// <summary>
        /// 获取信息组列表
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取信息组列表")]
        public ActionResult GetInfoGroups(Guid? ontologyID)
        {
            if (!ontologyID.HasValue)
            {
                throw new ValidationException("必须传入本体标识");
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyID.Value, out ontology))
            {
                throw new ValidationException("意外的本体标识" + ontologyID);
            }
            var list = ontology.InfoGroups.ToList();

            return this.JsonResult(new MiniGrid<IInfoGroup> { total = list.Count, data = list });
        }

        /// <summary>
        /// 分页获取信息组列表
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取信息组列表")]
        public ActionResult GetPlistInfoGroups(Guid? ontologyID, GetPlistResult input)
        {
            if (!ontologyID.HasValue)
            {
                throw new ValidationException("必须传入本体标识");
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyID.Value, out ontology))
            {
                throw new ValidationException("意外的本体标识" + ontologyID);
            }
            EntityTypeState entityType;
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "InfoGroup", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.InfoGroup");
            }
            foreach (var filter in input.filters)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的InfoGroup实体类型属性" + filter.field);
                }
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = ontology.InfoGroups.AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<IInfoGroup> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 分页获取动作列表
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取动作列表")]
        public ActionResult GetPlistActions(Guid? ontologyID, GetPlistResult input)
        {
            if (!ontologyID.HasValue)
            {
                throw new ValidationException("必须传入本体标识");
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyID.Value, out ontology))
            {
                throw new ValidationException("意外的本体标识" + ontologyID);
            }
            EntityTypeState entityType;
            if (!Host.EntityTypeSet.TryGetEntityType("EDI", "Action", out entityType))
            {
                throw new CoreException("意外的实体类型EDI.Action");
            }
            foreach (var filter in input.filters)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Action实体类型属性" + filter.field);
                }
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = ontology.Actions.Values.Select(a => ActionTr.Create(a)).AsQueryable();
            foreach (var filter in input.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<ActionTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 分页获取事件主题列表
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取事件主题列表")]
        public ActionResult GetPlistTopics(Guid? ontologyID)
        {
            if (!ontologyID.HasValue)
            {
                return this.JsonResult(new MiniGrid<Topic> { total = 0, data = new List<Topic>() });
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyID.Value, out ontology))
            {
                throw new ValidationException("非法的本体标识" + ontologyID);
            }
            var models = ontology.Topics.Values.Select(a => new Topic
            {
                Code = a.Code,
                Id = a.Id,
                IsAllowed = a.IsAllowed,
                OntologyID = a.OntologyID,
                Name = a.Name,
                Description = a.Description
            });
            var data = new MiniGrid<Topic> { total = models.Count(), data = models };

            return this.JsonResult(data);
        }

        /// <summary>
        /// 获取给定本体的组织结构
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取给定本体的组织结构")]
        public ActionResult GetOrganizationNodesByParentID(Guid? ontologyID, Guid? parentID)
        {
            if (!ontologyID.HasValue)
            {
                return this.JsonResult(null);
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyID.Value, out ontology))
            {
                throw new ValidationException("意外的本体标识" + ontologyID);
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
            var ontologyOrgDic = ontology.Organizations;
            var orgs = Host.OrganizationSet;
            return this.JsonResult(Host.OrganizationSet.Where(a => a != OrganizationState.VirtualRoot && string.Equals(a.ParentCode, parentCode, StringComparison.OrdinalIgnoreCase)).OrderBy(a => a.Code)
                .Select(a =>
                {
                    return new
                    {
                        a.Id,
                        a.Code,
                        a.Name,
                        ParentID = a.ParentCode,
                        isLeaf = Host.OrganizationSet.All(b => !a.Code.Equals(b.ParentCode, StringComparison.OrdinalIgnoreCase)),
                        expanded = false,
                        @checked = ontologyOrgDic.Values.Any(b => b.OrganizationID == a.Id),
                        OntologyID = ontologyID.Value
                    };
                }).ToList());
        }

        /// <summary>
        /// 获取给定本体组织结构的动作
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取给定本体组织结构的动作")]
        public ActionResult GetPlistOrganizationActions(GetPlistOntologyOrganizationActions input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(input.ontologyID, out ontology))
            {
                throw new ValidationException("意外的本体标识" + input.ontologyID);
            }
            OrganizationState organization;
            if (!Host.OrganizationSet.TryGetOrganization(input.organizationID, out organization))
            {
                throw new ValidationException("意外的组织结构标识" + input.organizationID);
            }
            List<OrganizationAssignActionTr> data = new List<OrganizationAssignActionTr>();
            OntologyOrganizationState ontologyOrg;
            if (!ontology.Organizations.TryGetValue(organization, out ontologyOrg))
            {
                return this.JsonResult(new MiniGrid<OrganizationAssignActionTr> { total = 0, data = data });
            }
            IReadOnlyDictionary<Verb, IOrganizationAction> actions = ontologyOrg.OrganizationActions;
            foreach (var item in Host.Ontologies.GetActons(ontology))
            {
                var action = item.Value;
                Guid id;
                string isAudit;
                string isAllowed;
                if (actions.ContainsKey(item.Key))
                {
                    id = actions[item.Key].Id;
                    isAudit = actions[item.Key].IsAudit;
                    isAllowed = actions[item.Key].IsAllowed;
                }
                else
                {
                    id = Guid.NewGuid();
                    isAudit = AuditType.NotAudit.ToName();
                    isAllowed = AllowType.ExplicitAllow.ToName();
                }
                data.Add(new OrganizationAssignActionTr
                {
                    ActionID = action.Id,
                    ActionIsAllowed = action.AllowType.ToName(),
                    ActionIsAudit = action.AuditType.ToName(),
                    Id = id,
                    IsAudit = isAudit,
                    IsAllowed = isAllowed,
                    Name = action.Name,
                    OntologyID = action.OntologyID,
                    OrganizationID = input.organizationID,
                    Verb = action.Verb,
                    OntologyOrganizationID = ontologyOrg.Id
                });
            }
            int pageIndex = input.pageIndex ?? 0;
            int pageSize = input.pageSize ?? 10;
            var queryable = data.AsQueryable();
            var list = queryable.OrderBy(input.sortField + " " + input.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<OrganizationAssignActionTr> { total = queryable.Count(), data = list });
        }

        #region AddOrRemoveOrganizations
        /// <summary>
        /// 添加或移除本体组织结构
        /// </summary>
        /// <param name="ontologyID"></param>
        /// <param name="addOrganizationIDs"></param>
        /// <param name="removeOrganizationIDs"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或移除本体组织结构")]
        [HttpPost]
        public ActionResult AddOrRemoveOrganizations(Guid ontologyID, string addOrganizationIDs, string removeOrganizationIDs)
        {
            string[] addIDs = addOrganizationIDs.Split(',');
            string[] removeIDs = removeOrganizationIDs.Split(',');
            foreach (var item in addIDs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var organizationID = new Guid(item);
                    Host.AddOntologyOrganization(new OntologyOrganizationCreateInput
                    {
                        Id = Guid.NewGuid(),
                        OntologyID = ontologyID,
                        OrganizationID = organizationID
                    });
                }
            }
            foreach (var item in removeIDs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var organizationID = new Guid(item);
                    Host.RemoveOntologyOrganization(ontologyID, organizationID);
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        // TODO:逻辑移动到应用服务层
        #region AddOrUpdateOrganizationActions
        /// <summary>
        /// 添加或更新本体组织结构级动作权限
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加或更新本体组织结构级动作权限")]
        [HttpPost]
        public ActionResult AddOrUpdateOrganizationActions()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";

                if (state == "modified" || state == "") //更新：_state为空或modified
                {
                    var inputModel = new OrganizationAction()
                    {
                        Id = new Guid(row["Id"].ToString()),
                        OrganizationID = new Guid(row["OrganizationID"].ToString()),
                        ActionID = new Guid(row["ActionID"].ToString()),
                        IsAudit = row["IsAudit"].ToString(),
                        IsAllowed = row["IsAllowed"].ToString()
                    };
                    var action = Host.Ontologies.GetAction(inputModel.ActionID);
                    if (action == null)
                    {
                        throw new ValidationException("意外的本体动作标识" + action.Id);
                    }
                    OntologyDescriptor ontology;
                    if (!Host.Ontologies.TryGetOntology(action.Id, out ontology))
                    {
                        throw new ValidationException("意外的动作本体标识" + action.OntologyID);
                    }
                    OrganizationState organization;
                    if (!Host.OrganizationSet.TryGetOrganization(inputModel.OrganizationID, out organization))
                    {
                        throw new ValidationException("意外的组织结构标识");
                    }
                    var ontologyOrgDic = Host.Ontologies.GetOntologyOrganizations(ontology);
                    OrganizationAction entity = null;
                    if (ontologyOrgDic.ContainsKey(organization))
                    {
                        entity = new OrganizationAction
                        {
                            ActionID = inputModel.ActionID,
                            IsAllowed = inputModel.IsAllowed,
                            IsAudit = inputModel.IsAudit,
                            Id = inputModel.Id,
                            OrganizationID = inputModel.OrganizationID
                        };
                        Host.PublishEvent(new OrganizationActionUpdatedEvent(entity));
                    }
                    else
                    {
                        entity = new OrganizationAction();
                        entity.Id = inputModel.Id;
                        entity.OrganizationID = inputModel.OrganizationID;
                        entity.ActionID = inputModel.ActionID;
                        entity.IsAudit = inputModel.IsAudit;
                        entity.IsAllowed = inputModel.IsAllowed;
                        Host.PublishEvent(new OrganizationActionAddedEvent(entity));
                    }
                    Host.CommitEventBus();
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        /// <summary>
        /// 获取节点关心的本体
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nodeID"></param>
        /// <param name="isAssigned"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("获取节点关心的本体")]
        public ActionResult GetNodeOntologyCares(Guid nodeID, GetPlistResult input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            NodeDescriptor node;
            if (!Host.Nodes.TryGetNodeByID(nodeID.ToString(), out node))
            {
                throw new ValidationException("意外的节点标识" + nodeID);
            }
            List<NodeAssignOntologyTr> data = new List<NodeAssignOntologyTr>();
            var nodeOntologyCares = Host.Nodes.GetNodeOntologyCares(node);
            foreach (var ontology in Host.Ontologies)
            {
                var id = Guid.NewGuid();
                var isAssigned = false;
                DateTime? createOn = null;
                var nodeOntologyCare = nodeOntologyCares.FirstOrDefault(a => a.NodeID == nodeID && a.OntologyID == ontology.Ontology.Id);
                if (nodeOntologyCare != null)
                {
                    id = nodeOntologyCare.Id;
                    isAssigned = true;
                    createOn = nodeOntologyCare.CreateOn;
                }
                data.Add(new NodeAssignOntologyTr
                {
                    Code = ontology.Ontology.Code,
                    CreateOn = createOn,
                    Icon = ontology.Ontology.Icon,
                    Id = id,
                    IsAssigned = isAssigned,
                    IsEnabled = ontology.Ontology.IsEnabled,
                    Name = ontology.Ontology.Name,
                    NodeID = nodeID,
                    OntologyID = ontology.Ontology.Id,
                    SortCode = ontology.Ontology.SortCode
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

            return this.JsonResult(new MiniGrid<NodeAssignOntologyTr> { total = queryable.Count(), data = list });
        }

        /// <summary>
        /// 添加信息组
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加信息组")]
        [Resource("InfoGroup")]
        [HttpPost]
        public ActionResult AddInfoGroup(InfoGroupCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddInfoGroup(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 更新信息组
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新信息组")]
        [Resource("InfoGroup")]
        [HttpPost]
        public ActionResult UpdateInfoGroup(InfoGroupUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateInfoGroup(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        #region DeleteInfoGroup
        /// <summary>
        /// 删除信息组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除信息组")]
        [Resource("InfoGroup")]
        [HttpPost]
        public ActionResult DeleteInfoGroup(string id)
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
                    throw new ValidationException("意外的信息组标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveInfoGroup(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
        #endregion

        /// <summary>
        /// 添加动作
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加动作")]
        [Resource("Action")]
        [HttpPost]
        public ActionResult AddAction(ActionCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddAction(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 更新动作
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新动作")]
        [Resource("Action")]
        [HttpPost]
        public ActionResult UpdateAction(ActionUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateAction(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        #region DeleteAction
        /// <summary>
        /// 删除动作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除动作")]
        [Resource("Action")]
        [HttpPost]
        public ActionResult DeleteAction(string id)
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
                    throw new ValidationException("意外的动作标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveAction(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
        #endregion

        /// <summary>
        /// 添加事件主题
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("添加事件主题")]
        [Resource("Topic")]
        [HttpPost]
        public ActionResult AddTopic(TopicCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddTopic(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        /// <summary>
        /// 更新事件主题
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新事件主题")]
        [Resource("Topic")]
        [HttpPost]
        public ActionResult UpdateTopic(TopicUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.UpdateTopic(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        #region DeleteTopic
        /// <summary>
        /// 删除事件主题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除事件主题")]
        [Resource("Topic")]
        [HttpPost]
        public ActionResult DeleteTopic(string id)
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
                    throw new ValidationException("意外的动作标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveTopic(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
        #endregion

        // TODO:逻辑移动到应用服务层
        #region UpdateOntologies
        /// <summary>
        /// 更新本体配置
        /// </summary>
        /// <returns></returns>
        [By("xuexs")]
        [Description("更新本体配置")]
        [HttpPost]
        public ActionResult UpdateOntologies()
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
                    var inputModel = new Ontology()
                    {
                        Id = new Guid(row["Id"].ToString()),
                        IsOrganizationalEntity = bool.Parse(row["IsOrganizationalEntity"].ToString()),
                        IsLogicalDeletionEntity = bool.Parse(row["IsLogicalDeletionEntity"].ToString())
                    };
                    Ontology entity = GetRequiredService<IRepository<Ontology>>().GetByKey(inputModel.Id);
                    if (entity != null)
                    {
                        entity.IsOrganizationalEntity = inputModel.IsOrganizationalEntity;
                        entity.IsLogicalDeletionEntity = inputModel.IsLogicalDeletionEntity;
                        GetRequiredService<IRepository<Ontology>>().Update(entity);
                        GetRequiredService<IRepository<Ontology>>().Context.Commit();
                        Host.CommitEventBus();
                    }
                    else
                    {
                        throw new CoreException("意外的本体");
                    }
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
                    var inputModel = new NodeOntologyCareCreateInput()
                    {
                        Id = new Guid(row["Id"].ToString()),
                        NodeID = new Guid(row["NodeID"].ToString()),
                        OntologyID = new Guid(row["OntologyID"].ToString())
                    };
                    bool isAssigned = bool.Parse(row["IsAssigned"].ToString());
                    NodeOntologyCare entity = GetRequiredService<IRepository<NodeOntologyCare>>().GetByKey(inputModel.Id);
                    bool isNew = true;
                    if (entity != null)
                    {
                        isNew = false;
                        if (!isAssigned)
                        {
                            GetRequiredService<IRepository<NodeOntologyCare>>().Remove(entity);
                        }
                    }
                    else
                    {
                        entity = new NodeOntologyCare
                        {
                            OntologyID = inputModel.OntologyID,
                            NodeID = inputModel.NodeID,
                            Id = inputModel.Id.Value
                        };
                    }
                    if (isAssigned)
                    {
                        if (isNew)
                        {
                            var count = GetRequiredService<IRepository<NodeOntologyCare>>().FindAll()
                                        .Where(a => a.OntologyID == entity.OntologyID
                                            && a.NodeID == entity.NodeID).Count();
                            if (count > 0)
                            {
                                throw new ValidationException("给定的节点已关心给定的本体，无需重复关心");
                            }
                            GetRequiredService<IRepository<NodeOntologyCare>>().Add(entity);
                        }
                        else
                        {
                            GetRequiredService<IRepository<NodeOntologyCare>>().Update(entity);
                        }
                        GetRequiredService<IRepository<NodeOntologyCare>>().Context.Commit();
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除本体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("删除本体")]
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
                    throw new ValidationException("意外的本体标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveOntology(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
        #endregion
    }
}
