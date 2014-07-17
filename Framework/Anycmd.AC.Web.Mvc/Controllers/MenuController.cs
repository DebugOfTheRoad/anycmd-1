
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Infra.Messages;
    using Host.AC.Messages;
    using Infra.ViewModels.MenuViewModels;
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
    /// 系统菜单模型视图控制器<see cref="ACEntities.Menu"/>
    /// </summary>
    public class MenuController : AnycmdController
    {
        private readonly EntityTypeState menuEntityType;

        public MenuController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Menu", out menuEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        [By("xuexs")]
        [Description("菜单管理")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("菜单详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = menuEntityType.GetData(id);
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
        [Description("子菜单列表")]
        public ViewResultBase Children()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("拥有该菜单的角色列表")]
        public ViewResultBase Roles()
        {
            return ViewResult();
        }

        #endregion

        [By("xuexs")]
        [Description("根据ID获取菜单")]
        public ActionResult Get(Guid? id)
        {
            if (id == Guid.Empty)
            {
                id = null;
            }
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(menuEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取菜单详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (id == Guid.Empty)
            {
                id = null;
            }
            if (id.HasValue)
            {
                if (!id.HasValue)
                {
                    throw new ValidationException("未传入标识");
                }
                return this.JsonResult(menuEntityType.GetData(id.Value));
            }
            else
            {
                return this.JsonResult(new Dictionary<string, Object> {
                    {"CreateBy", string.Empty},
                    {"CreateOn", DateTime.MinValue},
                    {"CreateUserID", Guid.Empty},
                    {"Description", string.Empty},
                    {"Icon", string.Empty},
                    {"ModifiedBy", string.Empty},
                    {"ModifiedOn", DateTime.MinValue},
                    {"ModifiedUserID", Guid.Empty},
                    {"Name", string.Empty},
                    {"ParentID", Guid.Empty},
                    {"ParentName", string.Empty},
                    {"Url", string.Empty}
                });
            }
        }

        #region GetNodesByParentID
        [By("xuexs")]
        [Description("根据父菜单ID获取子菜单")]
        public ActionResult GetNodesByParentID(Guid? parentID, string rootNodeName)
        {
            if (parentID == Guid.Empty)
            {
                parentID = null;
            }
            var nodes = Host.MenuSet.Where(a => a.ParentID == parentID).Select(a => MenuMiniNode.Create(Host, a)).ToList();
            if (string.IsNullOrEmpty(rootNodeName))
            {
                rootNodeName = "全部";
            }
            if (!parentID.HasValue)
            {
                var rootNode = new MenuMiniNode(Host)
                {
                    Id = Guid.Empty,
                    Name = rootNodeName,
                    ParentID = null,
                    expanded = true
                };
                foreach (var node in nodes)
                {
                    if (node.ParentID == null)
                    {
                        node.ParentID = rootNode.Id;
                    }
                }
                nodes.Add(rootNode);
            }

            return this.JsonResult(nodes);
        }
        #endregion

        #region GetNodesByRoleID
        [By("xuexs")]
        [Description("根据角色ID和父菜单ID获取子菜单")]
        public ActionResult GetNodesByRoleID(Guid roleID)
        {
            RoleState role;
            if (!Host.RoleSet.TryGetRole(roleID, out role))
            {
                throw new ValidationException("意外的角色标识" + roleID);
            }
            var roleMenus = Host.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Menu && a.SubjectInstanceID == roleID);
            var menus = Host.MenuSet;
            var data = (from m in menus
                        let @checked = roleMenus.Any(a => a.ObjectInstanceID == m.Id)
                        let isLeaf = !menus.Any(a => a.ParentID == m.Id)
                        select new
                        {
                            @checked = @checked,
                            expanded = @checked,// 如果选中则展开
                            m.Id,
                            isLeaf = isLeaf,
                            MenuID = m.Id,
                            m.Name,
                            m.ParentID,
                            RoleID = roleID,
                            img = m.Icon
                        });


            return this.JsonResult(data);
        }
        #endregion

        [By("xuexs")]
        [Description("根据父菜单ID获取子菜单")]
        public ActionResult GetPlistMenuChildren(GetPlistMenuChildren requestModel)
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
                if (!menuEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Menu实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = Host.MenuSet.Where(a => a.ParentID == requestModel.parentID).Select(a => MenuTr.Create(a)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            var list = queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);

            return this.JsonResult(new MiniGrid<MenuTr> { total = queryable.Count(), data = list });
        }

        [By("xuexs")]
        [Description("添加菜单")]
        [HttpPost]
        public ActionResult Create(MenuCreateInput input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            Host.Handle(new AddMenuCommand(input));

            return this.JsonResult(new ResponseData { success = true, id = input.Id });
        }

        [By("xuexs")]
        [Description("更新菜单")]
        [HttpPost]
        public ActionResult Update(MenuUpdateInput input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            Host.Handle(new UpdateMenuCommand(input));

            return this.JsonResult(new ResponseData { success = true, id = input.Id });
        }

        #region GrantOrDenyRoles
        [By("xuexs")]
        [Description("将菜单授予或收回角色")]
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
                            Host.Handle(new RemovePrivilegeBigramCommand(id));
                        }
                        else
                        {
                            if (row.ContainsKey("PrivilegeConstraint"))
                            {
                                Host.Handle(new UpdatePrivilegeBigramCommand(new PrivilegeBigramUpdateInput
                                {
                                    Id = entity.Id,
                                    PrivilegeConstraint = row["PrivilegeConstraint"].ToString()
                                }));
                            }
                        }
                    }
                    else if (isAssigned)
                    {
                        var createInput = new PrivilegeBigramCreateInput
                        {
                            Id = new Guid(row["Id"].ToString()),
                            SubjectType = ACSubjectType.Role.ToName(),
                            SubjectInstanceID = new Guid(row["RoleID"].ToString()),
                            ObjectInstanceID = new Guid(row["MenuID"].ToString()),
                            ObjectType = ACObjectType.Menu.ToName(),
                            PrivilegeOrientation = 1,
                            PrivilegeConstraint = null
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

        [By("xuexs")]
        [Description("删除菜单")]
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
                    throw new ValidationException("意外的菜单标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.Handle(new RemoveMenuCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}
