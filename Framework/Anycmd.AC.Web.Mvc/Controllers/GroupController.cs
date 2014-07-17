
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Messages;
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
    using ViewModels.GroupViewModels;
    using ViewModels.PrivilegeViewModels;

    /// <summary>
    /// 工作组模型视图控制器<see cref="ACEntities.Group"/>
    /// </summary>
    public class GroupController : AnycmdController
    {
        private readonly EntityTypeState groupEntityType;

        public GroupController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Group", out groupEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        [By("xuexs")]
        [Description("工作组管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("工作组详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = new GroupInfo(Host, groupEntityType.GetData(id));
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
        [Description("工作组拥有的角色列表")]
        public ViewResultBase Roles()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("工作组内的账户列表")]
        public ViewResultBase Accounts()
        {
            return ViewResult();
        }

        #endregion

        [By("xuexs")]
        [Description("根据ID获取工作组")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(groupEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取工作组详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(new GroupInfo(Host, groupEntityType.GetData(id.Value)));
        }

        [By("xuexs")]
        [Description("分页获取工作组")]
        public ActionResult GetPlistGroups(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistGroups(requestModel);

            return this.JsonResult(new MiniGrid<GroupTr> { total = requestModel.total.Value, data = data });
        }

        #region GetPlistAccountGroups
        [By("xuexs")]
        [Description("根据账户ID分页获取工作组")]
        public ActionResult GetPlistAccountGroups(GetPlistAccountGroups requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            List<AccountAssignGroupTr> data = new List<AccountAssignGroupTr>();
            var privilegeType = ACObjectType.Group.ToName();
            var accountGroups = GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().Where(a => a.SubjectInstanceID == requestData.accountID && a.ObjectType == privilegeType);
            foreach (var group in Host.GroupSet)
            {
                var accountGroup = accountGroups.FirstOrDefault(a => a.ObjectInstanceID == group.Id);
                if (requestData.isAssigned.HasValue)
                {
                    if (requestData.isAssigned.Value)
                    {
                        if (accountGroup == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (accountGroup != null)
                        {
                            continue;
                        }
                    }
                }
                string createBy = null;
                DateTime? createOn = null;
                Guid? createUserID = null;
                Guid id;
                bool isAssigned;
                if (accountGroup != null)
                {
                    createBy = accountGroup.CreateBy;
                    createOn = accountGroup.CreateOn;
                    createUserID = accountGroup.CreateUserID;
                    id = accountGroup.Id;
                    isAssigned = true;
                }
                else
                {
                    id = Guid.NewGuid();
                    isAssigned = false;
                }
                data.Add(new AccountAssignGroupTr
                {
                    CategoryCode = group.CategoryCode,
                    CreateBy = createBy,
                    CreateOn = createOn,
                    CreateUserID = createUserID,
                    GroupID = group.Id,
                    Id = id,
                    IsAssigned = isAssigned,
                    IsEnabled = group.IsEnabled,
                    Name = group.Name,
                    SortCode = group.SortCode,
                    AccountID = requestData.accountID
                });
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = data.AsQueryable();
            if (!string.IsNullOrEmpty(requestData.key))
            {
                queryable = queryable.Where(a => a.Name.Contains(requestData.key) || a.CategoryCode.Contains(requestData.key));
            }
            var list = queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<AccountAssignGroupTr> { total = queryable.Count(), data = list });
        }
        #endregion

        #region GetPlistRoleGroups
        [By("xuexs")]
        [Description("根据角色ID分页获取工作组")]
        public ActionResult GetPlistRoleGroups(GetPlistRoleGroups requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistRoleGroups(requestData);

            return this.JsonResult(new MiniGrid<RoleAssignGroupTr> { total = requestData.total.Value, data = data });
        }
        #endregion

        [By("xuexs")]
        [Description("添加工作组")]
        [HttpPost]
        public ActionResult Create(GroupCreateInput input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            if (!"AC".Equals(input.TypeCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException("非法的操作，试图越权。");
            }
            Host.Handle(new AddGroupCommand(input));

            return this.JsonResult(new ResponseData { success = true, id = input.Id });
        }

        [By("xuexs")]
        [Description("更新工作组")]
        [HttpPost]
        public ActionResult Update(GroupUpdateInput input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            if (!"AC".Equals(input.TypeCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException("非法的操作，试图越权。");
            }
            Host.Handle(new UpdateGroupCommand(input));

            return this.JsonResult(new ResponseData { success = true, id = input.Id });
        }

        [By("xuexs")]
        [Description("删除工作组")]
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
                Host.Handle(new RemoveGroupCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }

        [By("xuexs")]
        [Description("为工作组授予或收回角色")]
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
                            Host.Handle(new RemovePrivilegeBigramCommand(entity.Id));
                        }
                        else
                        {
                            if (row.ContainsKey("PrivilegeConstraint"))
                            {
                                Host.Handle(new UpdatePrivilegeBigramCommand(new PrivilegeBigramUpdateInput
                                {
                                    Id = id,
                                    PrivilegeConstraint = row["PrivilegeConstraint"].ToString()
                                }));
                            }
                        }
                    }
                    else if (isAssigned)
                    {
                        var createInput = new PrivilegeBigramCreateInput()
                        {
                            Id = new Guid(row["Id"].ToString()),
                            SubjectType = ACSubjectType.Role.ToName(),
                            SubjectInstanceID = new Guid(row["RoleID"].ToString()),
                            ObjectInstanceID = new Guid(row["GroupID"].ToString()),
                            ObjectType = ACObjectType.Group.ToName(),
                            PrivilegeConstraint = null,
                            PrivilegeOrientation = 1
                        };
                        if (row.ContainsKey("PrivilegeConstraint"))
                        {
                            createInput.PrivilegeConstraint = row["PrivilegeConstraint"].ToString();
                        }
                        Host.Handle(new AddPrivilegeBigramCommand(createInput));
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }

        [By("xuexs")]
        [Description("添加工作组成员账户")]
        [HttpPost]
        public ActionResult AddGroupAccounts(string accountIDs, Guid groupID)
        {
            string[] aIds = accountIDs.Split(',');
            foreach (var item in aIds)
            {
                var accountID = new Guid(item);
                Host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
                {
                    Id = Guid.NewGuid(),
                    ObjectType = ACObjectType.Group.ToName(),
                    SubjectType = ACSubjectType.Account.ToName(),
                    ObjectInstanceID = groupID,
                    SubjectInstanceID = accountID
                }));
            }

            return this.JsonResult(new ResponseData { success = true, id = accountIDs });
        }

        [By("xuexs")]
        [Description("移除工作组成员账户")]
        [HttpPost]
        public ActionResult RemoveGroupAccounts(string id)
        {
            string[] ids = id.Split(',');
            foreach (var item in ids)
            {
                Host.Handle(new RemovePrivilegeBigramCommand(new Guid(item)));
            }

            return this.JsonResult(new ResponseData { success = true, id = id });
        }
    }
}
