
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
    using ViewModels.PrivilegeViewModels;
    using ViewModels.RoleViewModels;

    /// <summary>
    /// 系统角色模型视图控制器<see cref="Role"/>
    /// </summary>
    public class RoleController : AnycmdController
    {
        private readonly EntityTypeState roleEntityType;

        public RoleController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Role", out roleEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        [By("xuexs")]
        [Description("角色管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("角色详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = roleEntityType.GetData(id);
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
        [Description("角色成员(账户)列表")]
        public ViewResultBase Accounts()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("角色功能权限列表")]
        public ViewResultBase Permissions()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("角色菜单")]
        public ViewResultBase Menus()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("角色工作组列表")]
        public ViewResultBase Groups()
        {
            return ViewResult();
        }

        #endregion

        [By("xuexs")]
        [Description("根据ID获取角色")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(roleEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取角色详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(roleEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("跟夜获取角色信息")]
        public ActionResult GetPlistRoles(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistRoles(requestModel);

            return this.JsonResult(new MiniGrid<RoleTr> { total = requestModel.total.Value, data = data });
        }

        #region GetPlistAccountRoles
        [By("xuexs")]
        [Description("根据账户ID分页获取角色")]
        public ActionResult GetPlistAccountRoles(GetPlistAccountRoles requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            List<AccountAssignRoleTr> data = new List<AccountAssignRoleTr>();
            var privilegeType = ACObjectType.Role.ToName();
            var accountRoles = GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().Where(a => a.SubjectInstanceID == requestData.accountID && a.ObjectType == privilegeType);
            if (requestData.isAssigned.HasValue)
            {
                if (requestData.isAssigned.Value)
                {
                    foreach (var ar in accountRoles)
                    {
                        RoleState role;
                        if (!Host.RoleSet.TryGetRole(ar.ObjectInstanceID, out role))
                        {
                            throw new CoreException("意外的角色标识" + ar.ObjectInstanceID);
                        }
                        data.Add(new AccountAssignRoleTr
                        {
                            AccountID = requestData.accountID,
                            IsAssigned = true,
                            RoleID = ar.ObjectInstanceID,
                            CreateBy = ar.CreateBy,
                            CreateOn = ar.CreateOn,
                            CreateUserID = ar.CreateUserID,
                            Id = ar.Id,
                            IsEnabled = role.IsEnabled,
                            CategoryCode = role.CategoryCode,
                            Name = role.Name,
                            SortCode = role.SortCode,
                            Icon = role.Icon
                        });
                    }
                }
                else
                {
                    foreach (var role in Host.RoleSet)
                    {
                        if (!accountRoles.Any(a => a.ObjectInstanceID == role.Id))
                        {
                            data.Add(new AccountAssignRoleTr
                            {
                                AccountID = requestData.accountID,
                                IsAssigned = false,
                                RoleID = role.Id,
                                CreateBy = null,
                                CreateOn = null,
                                CreateUserID = null,
                                Id = Guid.NewGuid(),
                                IsEnabled = role.IsEnabled,
                                CategoryCode = role.CategoryCode,
                                Name = role.Name,
                                SortCode = role.SortCode,
                                Icon = role.Icon
                            });
                        }
                    }
                }
            }
            else
            {
                foreach (var role in Host.RoleSet)
                {
                    var ar = accountRoles.FirstOrDefault(a => a.ObjectInstanceID == role.Id);
                    if (ar == null)
                    {
                        data.Add(new AccountAssignRoleTr
                        {
                            AccountID = requestData.accountID,
                            IsAssigned = false,
                            RoleID = role.Id,
                            CreateBy = null,
                            CreateOn = null,
                            CreateUserID = null,
                            Id = Guid.NewGuid(),
                            IsEnabled = role.IsEnabled,
                            CategoryCode = role.CategoryCode,
                            Name = role.Name,
                            SortCode = role.SortCode,
                            Icon = role.Icon
                        });
                    }
                    else
                    {
                        data.Add(new AccountAssignRoleTr
                        {
                            AccountID = requestData.accountID,
                            IsAssigned = true,
                            RoleID = ar.ObjectInstanceID,
                            CreateBy = ar.CreateBy,
                            CreateOn = ar.CreateOn,
                            CreateUserID = ar.CreateUserID,
                            Id = ar.Id,
                            IsEnabled = role.IsEnabled,
                            CategoryCode = role.CategoryCode,
                            Name = role.Name,
                            SortCode = role.SortCode,
                            Icon = role.Icon
                        });
                    }
                }
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = data.AsQueryable();
            if (!string.IsNullOrEmpty(requestData.key))
            {
                queryable = queryable.Where(a => a.Name.Contains(requestData.key) || a.CategoryCode.Contains(requestData.key));
            }
            var list = queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<AccountAssignRoleTr> { total = queryable.Count(), data = list });
        }
        #endregion

        #region GetPlistGroupRoles
        [By("xuexs")]
        [Description("根据工作组ID分页获取角色")]
        public ActionResult GetPlistGroupRoles(GetPlistGroupRoles requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistGroupRoles(requestData);

            return this.JsonResult(new MiniGrid<GroupAssignRoleTr> { total = requestData.total.Value, data = data });
        }
        #endregion

        [By("xuexs")]
        [Description("根据菜单ID分页获取角色")]
        public ActionResult GetPlistMenuRoles(GetPlistMenuRoles requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            if (requestData.menuID.HasValue && requestData.menuID.Value != Guid.Empty)
            {
                var data = Host.GetPlistMenuRoles(requestData);

                return this.JsonResult(new MiniGrid<MenuAssignRoleTr> { total = requestData.total.Value, data = data });
            }
            else
            {
                return this.JsonResult(new MiniGrid<MenuAssignRoleTr> { total = 0, data = new List<MenuAssignRoleTr>() });
            }
        }

        [By("xuexs")]
        [Description("创建角色")]
        [HttpPost]
        public ActionResult Create(RoleCreateInput input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            Host.Handle(new AddRoleCommand(input));

            return this.JsonResult(new ResponseData { success = true, id = input.Id });
        }

        [By("xuexs")]
        [Description("更新角色")]
        [HttpPost]
        public ActionResult Update(RoleUpdateInput input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            Host.Handle(new UpdateRoleCommand(input));

            return this.JsonResult(new ResponseData { success = true, id = input.Id });
        }

        [By("xuexs")]
        [Description("删除角色")]
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
                    throw new ValidationException("意外的角色标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.Handle(new RemoveRoleCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }

        #region GrantOrDenyRoleFunctions
        [By("xuexs")]
        [Description("添加或禁用权限")]
        [HttpPost]
        public ActionResult GrantOrDenyRoleFunctions()
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
                    PrivilegeBigram entity = GetRequiredService<IRepository<PrivilegeBigram>>().GetByKey(id);
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
                                    Id = entity.Id,
                                    PrivilegeConstraint = row["PrivilegeConstraint"] == null ? null : row["PrivilegeConstraint"].ToString()
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
                            ObjectInstanceID = new Guid(row["FunctionID"].ToString()),
                            ObjectType = ACObjectType.Function.ToName(),
                            PrivilegeConstraint = null,
                            PrivilegeOrientation = 1
                        };
                        if (row.ContainsKey("PrivilegeConstraint"))
                        {
                            createInput.PrivilegeConstraint = row["PrivilegeConstraint"] == null ? null : row["PrivilegeConstraint"].ToString();
                        }
                        Host.Handle(new AddPrivilegeBigramCommand(createInput));
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        #region AddOrRemoveMenus
        [By("xuexs")]
        [Description("往角色中添加或移除菜单")]
        [HttpPost]
        public ActionResult AddOrRemoveMenus(Guid roleID, string addMenuIDs, string removeMenuIDs)
        {
            string[] addIDs = addMenuIDs.Split(',');
            string[] removeIDs = removeMenuIDs.Split(',');
            var subjectType = ACSubjectType.Role.ToName();
            var acObjectType = ACObjectType.Menu.ToName();
            foreach (var item in addIDs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var mID = new Guid(item);
                    var entity = GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().FirstOrDefault(a => a.SubjectType == subjectType && a.SubjectInstanceID == roleID && a.ObjectType == acObjectType && a.ObjectInstanceID == mID);
                    if (entity == null)
                    {
                        var createInput = new PrivilegeBigramCreateInput
                        {
                            Id = Guid.NewGuid(),
                            SubjectType = subjectType,
                            SubjectInstanceID = roleID,
                            ObjectInstanceID = mID,
                            ObjectType = acObjectType,
                            PrivilegeOrientation = 1,
                            PrivilegeConstraint = null
                        };
                        Host.Handle(new AddPrivilegeBigramCommand(createInput));
                    }
                }
            }
            foreach (var item in removeIDs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var mID = new Guid(item);
                    var entity = GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().FirstOrDefault(a => a.SubjectType == subjectType && a.SubjectInstanceID == roleID && a.ObjectType == acObjectType && a.ObjectInstanceID == mID);
                    if (entity != null)
                    {
                        Host.Handle(new RemovePrivilegeBigramCommand(entity.Id));
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        [By("xuexs")]
        [Description("添加角色成员账户")]
        [HttpPost]
        public ActionResult AddRoleAccounts(string accountIDs, Guid roleID)
        {
            string[] aIds = accountIDs.Split(',');
            foreach (var item in aIds)
            {
                var accountID = new Guid(item);
                Host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
                {
                    Id = Guid.NewGuid(),
                    ObjectInstanceID = roleID,
                    SubjectInstanceID = accountID,
                    SubjectType = ACSubjectType.Account.ToName(),
                    ObjectType = ACObjectType.Role.ToName()
                }));
            }

            return this.JsonResult(new ResponseData { success = true, id = accountIDs });
        }

        [By("xuexs")]
        [Description("移除角色成员账户")]
        [HttpPost]
        public ActionResult RemoveRoleAccounts(string id)
        {
            string[] ids = id.Split(',');
            foreach (var item in ids)
            {
                Host.Handle(new RemovePrivilegeBigramCommand(new Guid(item)));
            }

            return this.JsonResult(new ResponseData { success = true, id = id });
        }

        #region GrantOrDenyGroups
        [By("xuexs")]
        [Description("将角色授予或收回工作组")]
        [HttpPost]
        public ActionResult GrantOrDenyGroups()
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
                            Host.Handle(new RemovePrivilegeBigramCommand(id));
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
                            ObjectInstanceID = new Guid(row["GroupID"].ToString()),
                            ObjectType = ACObjectType.Group.ToName(),
                            SubjectType = ACSubjectType.Role.ToName(),
                            SubjectInstanceID = new Guid(row["RoleID"].ToString()),
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
        #endregion
    }
}
