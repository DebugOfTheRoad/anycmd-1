
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Extensions;
    using Host;
    using Host.AC;
    using Host.AC.Identity;
    using Identity.ViewModels.AccountViewModels;
    using Identity.ViewModels.ContractorViewModels;
    using MiniUI;
    using Query;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.PrivilegeViewModels;

    /// <summary>
    /// 账户模型视图控制器<see cref="Account"/>
    /// </summary>
    public class AccountController : AnycmdController
    {
        private readonly EntityTypeState accountEntityType;

        public AccountController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Account", out accountEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region Views
        [By("xuexs")]
        [Description("账户")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("当前登录账户信息")]
        public ViewResultBase CurrentAccount(string isTooltip)
        {
            if (!string.IsNullOrEmpty(isTooltip))
            {
                var data = GetRequiredService<IAccountQuery>().Get("AccountInfo", Host.User.Worker.Id);

                return this.PartialView("Partials/Details", data);
            }
            else
            {
                return this.PartialView("Partials/Details");
            }
        }

        [By("xuexs")]
        [Description("账户详细信息")]
        public ViewResultBase Details()
        {
            return this.DetailsResult(GetRequiredService<IAccountQuery>(), "AccountInfo");
        }

        [By("xuexs")]
        [Description("账户拥有的角色列表")]
        public ViewResultBase Roles()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("账户所在的工作组列表")]
        public ViewResultBase Groups()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("账户所在的组织结构")]
        public ViewResultBase Organizations()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("包工头列表")]
        public ViewResultBase Contractors()
        {
            return ViewResult();
        }
        #endregion

        [By("xuexs")]
        [Description("修改指定账户的密码")]
        [HttpPost]
        public ActionResult AssignPassword(PasswordAssignInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AssignPassword(input);

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("修改自己的密码")]
        [HttpPost]
        public ActionResult ChangeSelfPassword(string oldPassword, string password, string passwordAgain)
        {
            if (password != passwordAgain)
            {
                throw new ValidationException("两次输入的密码不一致");
            }
            Host.ChangePassword(new PasswordChangeInput
            {
                LoginName = Host.User.Principal.Identity.Name,
                OldPassword = oldPassword,
                NewPassword = password
            });

            return this.JsonResult(new ResponseData { success = true });
        }

        [By("xuexs")]
        [Description("根据ID获取账户")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(accountEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取账户详细")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(GetRequiredService<IAccountQuery>().Get("AccountInfo", id.Value));
        }

        [By("xuexs")]
        [Description("根据用户ID获取账户")]
        public ActionResult GetAccountsByContractorID(GetAccountsByContractorID requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }

            requestModel.filters.Add(FilterData.EQ("ContractorID", requestModel.contractorID.Value));
            var accounts = GetRequiredService<IAccountQuery>().GetPlistAccountTrs(requestModel.filters, null, true, requestModel);
            var data = new MiniGrid<Dictionary<string, object>> { total = requestModel.total.Value, data = accounts };

            return this.JsonResult(data);
        }

        #region GetPlistAccounts
        [By("xuexs")]
        [Description("根据用户分页获取账户")]
        public ActionResult GetPlistAccounts(GetPlistAccounts requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            EntityTypeState entityType;
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Account", out entityType))
            {
                throw new CoreException("意外的实体类型AC.Account");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!Host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Account实体类型属性" + filter.field);
                }
            }
            requestModel.includeDescendants = requestModel.includeDescendants ?? false;
            List<Dictionary<string, object>> userAccountTrs = null;
            // TODO:注意，这里引入了权限逻辑。
            // 如果组织机构为空则需要检测是否是插法人员，因为只有开发人员才可以看到全部用户
            if (string.IsNullOrEmpty(requestModel.organizationCode))
            {
                if (!Host.User.IsDeveloper())
                {
                    throw new ValidationException("对不起，您没有查看全部账户的权限");
                }
                else
                {
                    userAccountTrs = GetRequiredService<IAccountQuery>().GetPlistAccountTrs(requestModel.filters, requestModel.organizationCode
                , requestModel.includeDescendants.Value, requestModel);
                }
            }
            else
            {
                userAccountTrs = GetRequiredService<IAccountQuery>().GetPlistAccountTrs(requestModel.filters, requestModel.organizationCode
                , requestModel.includeDescendants.Value, requestModel);
            }
            var data = new MiniGrid<Dictionary<string, object>> { total = requestModel.total.Value, data = userAccountTrs };

            return this.JsonResult(data);
        }
        #endregion

        [By("xuexs")]
        [Description("根据工作组ID分页获取账户")]
        public ActionResult GetPlistGroupAccounts(GetPlistGroupAccounts requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var groupUserAccountTrs = GetRequiredService<IAccountQuery>().GetPlistGroupAccountTrs(
                requestModel.key, requestModel.groupID.Value, requestModel);
            var data = new MiniGrid<Dictionary<string, object>> { total = requestModel.total.Value, data = groupUserAccountTrs };

            return this.JsonResult(data);
        }

        [By("xuexs")]
        [Description("根据角色ID分页获取账户")]
        public ActionResult GetPlistRoleAccounts(GetPlistRoleAccounts requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var roleUserAccountTrs = GetRequiredService<IAccountQuery>().GetPlistRoleAccountTrs(
                requestModel.key, requestModel.roleID.Value, requestModel);
            var data = new MiniGrid<Dictionary<string, object>> { total = requestModel.total.Value, data = roleUserAccountTrs };

            return this.JsonResult(data);
        }

        [By("xuexs")]
        [Description("添加账户")]
        [HttpPost]
        public ActionResult Create(AccountCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.AddAccount(input);

            return this.JsonResult(new ResponseData { success = true, id = input.Id });
        }

        [By("xuexs")]
        [Description("更新账户")]
        [HttpPost]
        public ActionResult Update(AccountUpdateInput requestModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            Host.UpdateAccount(requestModel);

            return this.JsonResult(new ResponseData { success = true, id = requestModel.Id });
        }

        [By("xuexs")]
        [Description("启用账户")]
        [HttpPost]
        public ActionResult EnableAccount(string id)
        {
            string[] ids = id.Split(',');
            foreach (var item in ids)
            {
                var entity = GetRequiredService<IRepository<Account>>().GetByKey(new Guid(item));
                if (entity == null)
                {
                    throw new NotExistException(item);
                }
                entity.IsEnabled = 1;
                GetRequiredService<IRepository<Account>>().Context.Commit();
            }

            return this.JsonResult(new ResponseData { success = true, id = id });
        }

        [By("xuexs")]
        [Description("禁用账户")]
        [HttpPost]
        public ActionResult DisableAccount(string id)
        {
            string[] ids = id.Split(',');
            foreach (var item in ids)
            {
                var entity = GetRequiredService<IRepository<Account>>().GetByKey(new Guid(item));
                if (entity == null)
                {
                    throw new NotExistException(item);
                }
                entity.IsEnabled = 0;
                GetRequiredService<IRepository<Account>>().Context.Commit();
            }

            return this.JsonResult(new ResponseData { success = true, id = id });
        }

        [By("xuexs")]
        [Description("删除账户")]
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
                    throw new ValidationException("意外的账户标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.RemoveAccount(item);
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }

        #region GrantOrDenyRoles
        [By("xuexs")]
        [Description("授予或收回角色")]
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
                            Host.RemovePrivilegeBigram(id);
                        }
                        else
                        {
                            if (row.ContainsKey("PrivilegeConstraint"))
                            {
                                Host.UpdatePrivilegeBigram(new PrivilegeBigramUpdateInput
                                {
                                    Id = id,
                                    PrivilegeConstraint = row["PrivilegeConstraint"].ToString()
                                });
                            }
                        }
                    }
                    else if (isAssigned)
                    {
                        var createInput = new PrivilegeBigramCreateInput
                        {
                            Id = new Guid(row["Id"].ToString()),
                            ObjectType = ACObjectType.Role.ToName(),
                            ObjectInstanceID = new Guid(row["RoleID"].ToString()),
                            SubjectInstanceID = new Guid(row["AccountID"].ToString()),
                            SubjectType = ACSubjectType.Account.ToName(),
                            PrivilegeConstraint = null,
                            PrivilegeOrientation = 1
                        };
                        if (row.ContainsKey("PrivilegeConstraint"))
                        {
                            createInput.PrivilegeConstraint = row["PrivilegeConstraint"].ToString();
                        }
                        Host.AddPrivilegeBigram(createInput);
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        #region JoinOrLeaveGroups
        [By("xuexs")]
        [Description("加入或脱离工作组")]
        [HttpPost]
        public ActionResult JoinOrLeaveGroups()
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
                            Host.RemovePrivilegeBigram(id);
                        }
                    }
                    else if (isAssigned)
                    {
                        Host.AddPrivilegeBigram(new PrivilegeBigramCreateInput
                        {
                            Id = id,
                            ObjectType = ACObjectType.Group.ToName(),
                            ObjectInstanceID = new Guid(row["GroupID"].ToString()),
                            SubjectInstanceID = new Guid(row["AccountID"].ToString()),
                            SubjectType = ACSubjectType.Account.ToName()
                        });
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
        #endregion

        [By("xuexs")]
        [Description("根据组织结构ID分页获取数据集管理员")]
        public ActionResult GetPlistAccountOrganizationPrivileges(GetPlistAccountOrganizationPrivileges requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            requestData.includeDescendants = requestData.includeDescendants ?? false;
            List<Dictionary<string, object>> data;
            if (string.IsNullOrEmpty(requestData.organizationCode))
            {
                if (!Host.User.IsDeveloper())
                {
                    throw new ValidationException("对不起，您没有查看全部管理员的权限");
                }
                else
                {
                    data = GetRequiredService<IPrivilegeQuery>().GetPlistOrganizationAccountTrs(requestData.key.SafeTrim(),
                    requestData.organizationCode, requestData.includeDescendants.Value, requestData);
                }
            }
            else
            {
                data = GetRequiredService<IPrivilegeQuery>().GetPlistOrganizationAccountTrs(requestData.key.SafeTrim(),
                    requestData.organizationCode, requestData.includeDescendants.Value, requestData);
            }

            return this.JsonResult(new MiniGrid<Dictionary<string, object>> { total = requestData.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("分页获取包工头")]
        public ActionResult GetPlistContractors(GetPlistContractors requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            requestData.includeDescendants = requestData.includeDescendants ?? false;
            List<Dictionary<string, object>> userTrs = null;
            // 如果组织机构为空则需要检测是否是超级管理员，因为只有超级管理员才可以看到全部包工头
            if (string.IsNullOrEmpty(requestData.organizationCode))
            {
                if (!Host.User.IsDeveloper())
                {
                    throw new ValidationException("对不起，您没有查看全部包工头的权限");
                }
                else
                {
                    userTrs = GetRequiredService<IAccountQuery>().GetPlistContractorTrs(
                        requestData.filters, requestData.organizationCode, requestData.includeDescendants.Value, requestData);
                }
            }
            else
            {
                userTrs = GetRequiredService<IAccountQuery>().GetPlistContractorTrs(
                    requestData.filters, requestData.organizationCode, requestData.includeDescendants.Value, requestData);
            }

            var data = new MiniGrid<Dictionary<string, object>> { total = requestData.total.Value, data = userTrs };

            return this.JsonResult(data);
        }
    }
}
