
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Infra.Messages;
    using Host.AC.Messages;
    using Infra;
    using Infra.ViewModels.OrganizationViewModels;
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
    using ViewModels.PrivilegeViewModels;

    /// <summary>
    /// 组织结构模型视图控制器<see cref="Organization"/>
    /// </summary>
    public class OrganizationController : AnycmdController
    {
        private readonly EntityTypeState entityType;

        public OrganizationController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("AC", "Organization", out entityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region 视图
        [By("xuexs")]
        [Description("组织结构管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("组织结构详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = OrganizationInfo.Create(entityType.GetData(id));
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
        [Description("岗位（工作组）列表")]
        public ViewResultBase Groups()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("下级组织结构列表")]
        public ViewResultBase Children()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("组织结构账户")]
        public ViewResultBase Accounts()
        {
            return ViewResult();
        }

        #endregion

        [By("xuexs")]
        [Description("根据ID获取组织结构")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(entityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取组织结构详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (id.HasValue)
            {
                return this.JsonResult(OrganizationInfo.Create(entityType.GetData(id.Value)));
            }
            else
            {
                return this.JsonResult(new Dictionary<string, object> {
                    {"AccountingID", Guid.Empty},
                    {"Address", string.Empty},
                    {"AssistantLeadershipID", Guid.Empty},
                    {"AssistantManagerID", Guid.Empty},
                    {"Bank", string.Empty},
                    {"BankAccount", string.Empty},
                    {"CashierID", Guid.Empty},
                    {"CategoryName", string.Empty},
                    {"Code", string.Empty},
                    {"CreateBy", string.Empty},
                    {"CreateOn", DateTime.MinValue},
                    {"CreateUserID", Guid.Empty},
                    {"Description", string.Empty},
                    {"Fax", string.Empty},
                    {"FinancialID", Guid.Empty},
                    {"Icon", string.Empty},
                    {"InnerPhone", string.Empty},
                    {"LeadershipID", Guid.Empty},
                    {"ManagerID", Guid.Empty},
                    {"ModifiedBy", string.Empty},
                    {"ModifiedOn", DateTime.MinValue},
                    {"ModifiedUserID", Guid.Empty},
                    {"Name", string.Empty},
                    {"OuterPhone", string.Empty},
                    {"ParentCode", string.Empty},
                    {"ParentID", Guid.Empty},
                    {"ParentName", string.Empty},
                    {"Postalcode", string.Empty},
                    {"PrincipalID", Guid.Empty},
                    {"PrincipalName", string.Empty},
                    {"ShortName", string.Empty},
                    {"WebPage", string.Empty}
                });
            }
        }

        // TODO:重构 因为方法太长
        [By("xuexs")]
        [Description("根据父节点获取子节点")]
        public ActionResult GetNodesByParentID(Guid? parentID)
        {
            if (parentID == Guid.Empty)
            {
                parentID = null;
            }
            if (!parentID.HasValue)
            {
                if (CurrentUser.IsDeveloper())
                {
                    return this.JsonResult(AppHostInstance.OrganizationSet.Where(a => a != OrganizationState.VirtualRoot && a.ParentCode == null).OrderBy(a => a.SortCode).Select(a => new OrganizationMiniNode
                    {
                        CategoryCode = a.CategoryCode,
                        Code = a.Code,
                        expanded = false,
                        Id = a.Id.ToString(),
                        isLeaf = !AppHostInstance.OrganizationSet.Any(o => a.Code.Equals(o.ParentCode, StringComparison.OrdinalIgnoreCase)),
                        Name = a.Name,
                        ParentCode = a.ParentCode,
                        ParentID = a.Parent.Id.ToString(),
                        SortCode = a.SortCode.ToString()
                    }));
                }
                else
                {
                    IList<IOrganization> orgs = CurrentUser.GetOrganizations();
                    if (orgs != null && orgs.Count > 0)
                    {
                        return this.JsonResult(orgs.Select(org => new OrganizationMiniNode
                        {
                            Code = org.Code ?? string.Empty,
                            Id = org.Id.ToString(),
                            Name = org.Name,
                            isLeaf = AppHostInstance.OrganizationSet.All(o => !org.Code.Equals(o.ParentCode, StringComparison.OrdinalIgnoreCase))
                        }));
                    }
                    return this.JsonResult(new List<OrganizationMiniNode>());
                }
            }
            else
            {
                var pid = parentID.Value;
                OrganizationState parentOrg;
                if (!AppHostInstance.OrganizationSet.TryGetOrganization(pid, out parentOrg))
                {
                    throw new ValidationException("意外的组织结构标识" + pid);
                }
                return this.JsonResult(AppHostInstance.OrganizationSet.Where(a => parentOrg.Code.Equals(a.ParentCode, StringComparison.OrdinalIgnoreCase)).OrderBy(a => a.SortCode).Select(a => new OrganizationMiniNode
                {
                    CategoryCode = a.CategoryCode,
                    Code = a.Code,
                    expanded = false,
                    Id = a.Id.ToString(),
                    isLeaf = !AppHostInstance.OrganizationSet.Any(o => a.Code.Equals(o.ParentCode, StringComparison.OrdinalIgnoreCase)),
                    Name = a.Name,
                    ParentCode = a.ParentCode,
                    ParentID = a.Parent.Id.ToString(),
                    SortCode = a.SortCode.ToString()
                }));
            }
        }

        [By("xuexs")]
        [Description("分页获取组织结构")]
        public ActionResult GetPlistChildren(GetPlistChildren requestModel)
        {
            if (requestModel.parentID == Guid.Empty)
            {
                requestModel.parentID = null;
            }
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!entityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Organization实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            IQueryable<OrganizationTr> queryable = null;
            if (requestModel.includeDescendants.HasValue && requestModel.includeDescendants.Value)
            {
                queryable = AppHostInstance.OrganizationSet.Where(a => a != OrganizationState.VirtualRoot).Select(a => OrganizationTr.Create(a)).AsQueryable().Where(a => a.Code.Contains(requestModel.parentCode));
            }
            else
            {
                if (requestModel.parentCode == string.Empty)
                {
                    requestModel.parentCode = null;
                }
                queryable = AppHostInstance.OrganizationSet.Where(a => a != OrganizationState.VirtualRoot && string.Equals(a.ParentCode, requestModel.parentCode, StringComparison.OrdinalIgnoreCase)).Select(a => OrganizationTr.Create(a)).AsQueryable();
            }
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<OrganizationTr> { total = queryable.Count(), data = list });
        }

        [By("xuexs")]
        [Description("添加数据集管理员")]
        [HttpPost]
        public ActionResult AddAccountOrganizations(string accountIDs, Guid organizationID)
        {
            if (string.IsNullOrEmpty(accountIDs))
            {
                throw new ValidationException("accountIDs不能为空");
            }
            if (organizationID == Guid.Empty)
            {
                throw new ValidationException("organizationID是必须的");
            }
            OrganizationState organization;
            if (!AppHostInstance.OrganizationSet.TryGetOrganization(organizationID, out organization))
            {
                throw new ValidationException("意外的组织结构标识" + organizationID);
            }
            string[] aIds = accountIDs.Split(',');
            foreach (var item in aIds)
            {
                var accountID = new Guid(item);
                AppHostInstance.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
                {
                    SubjectInstanceID = accountID,
                    SubjectType = ACSubjectType.Account.ToName(),
                    Id = Guid.NewGuid(),
                    ObjectInstanceID = organizationID,
                    ObjectType = ACObjectType.Organization.ToName()
                }));
            }

            return this.JsonResult(new ResponseData { success = true, id = accountIDs });
        }

        [By("xuexs")]
        [Description("移除数据集管理员")]
        [HttpPost]
        public ActionResult RemoveAccountOrganizations(string id)
        {
            string[] ids = id.Split(',');
            foreach (var item in ids)
            {
                AppHostInstance.Handle(new RemovePrivilegeBigramCommand(new Guid(item)));
            }

            return this.JsonResult(new ResponseData { success = true, id = id });
        }

        [By("xuexs")]
        [Description("添加组织结构")]
        [HttpPost]
        public ActionResult Create(OrganizationCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new AddOrganizationCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("更新组织结构")]
        [HttpPost]
        public ActionResult Update(OrganizationUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new UpdateOrganizationCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("删除组织结构")]
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
                    throw new ValidationException("意外的组织结构标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                AppHostInstance.Handle(new RemoveOrganizationCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }

        #region GrantOrDenyRoles
        [By("xuexs")]
        [Description("为指定组织结构下的全部账户逻辑授予或收回角色")]
        [HttpPost]
        public ActionResult GrantOrDenyRoles()
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
                    bool isAssigned = bool.Parse(row["IsAssigned"].ToString());
                    var entity = GetRequiredService<IRepository<PrivilegeBigram>>().GetByKey(id);
                    if (entity != null)
                    {
                        if (!isAssigned)
                        {
                            AppHostInstance.Handle(new RemovePrivilegeBigramCommand(id));
                        }
                    }
                    else if (isAssigned)
                    {
                        AppHostInstance.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
                        {
                            Id = new Guid(row["Id"].ToString()),
                            ObjectType = ACObjectType.Role.ToName(),
                            SubjectType = ACSubjectType.Organization.ToName(),
                            ObjectInstanceID = new Guid(row["RoleID"].ToString()),
                            SubjectInstanceID = new Guid(row["OrganizationID"].ToString())
                        }));
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion
    }
}
