
namespace Anycmd.Host
{
    using Anycmd.AC;
    using Exceptions;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class UserSessionExtension
    {
        private const string CURRENT_ORGANIZATIONS = "UserContext_Current_Organizations";
        private const string CURRENT_ROLES = "UserContext_Current_Roles";
        private const string CURRENT_GROUPS = "UserContext_Current_Groups";
        private const string CURRENT_FUNCTIONS = "UserContext_Current_Functions";
        private const string CURRENT_MENUS = "UserContext_Current_Menus";
        private const string CURRENT_APPSYSTEMS = "UserContext_Current_AppSystems";
        private const string CURRENT_RIVILEGE_INITED = "UserContext_Current_PrivilegeInited";

        private const string CURRENT_ALL_FUNCTIONIDS = "Current_GetAllFunctionIDs";
        private const string CURRENT_ALL_ROLEIDS = "Current_GetAllRoleIDs";
        private const string CURRENT_ALL_MENUS = "Current_GetAllMenus";

        #region IsDeveloper
        /// <summary>
        /// 判断当前用户是否是超级管理员
        /// </summary>
        /// <returns>True表示是超级管理员，False不是</returns>
        public static bool IsDeveloper(this IUserSession user)
        {
            AccountState account;
            return user.Principal.Identity.IsAuthenticated && user.AppHost.SysUsers.TryGetDevAccount(user.Worker.Id, out account);
        }
        #endregion

        #region 用户会话级数据存取接口
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetData<T>(this IUserSession user, string key)
        {
            var userSessionStorage = user.AppHost.GetRequiredService<IUserSessionStorage>();
            var obj = userSessionStorage.GetData(key);
            if (obj is T)
            {
                return (T)obj;
            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void SetData(this IUserSession user, string key, object data)
        {
            var userSessionStorage = user.AppHost.GetRequiredService<IUserSessionStorage>();
            userSessionStorage.SetData(key, data);
        }
        #endregion

        #region GetOrganizations
        /// <summary>
        /// 获取当前用户直接担任管理员的组织结构，直接担任管理员的组织结构不包括直接管理的组织结构下属的组织结构。
        /// </summary>
        /// <returns></returns>
        public static HashSet<OrganizationState> GetOrganizations(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new HashSet<OrganizationState>();
            }
            CalculateOnce(user);

            return user.GetData<HashSet<OrganizationState>>(CURRENT_ORGANIZATIONS);
        }
        #endregion

        #region GetRoles
        /// <summary>
        /// 从账户角色二元关系直接得到的角色。不包括从组织结构和工作组等得到的角色。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static HashSet<RoleState> GetRoles(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new HashSet<RoleState>();
            }
            CalculateOnce(user);

            return user.GetData<HashSet<RoleState>>(CURRENT_ROLES);
        }

        public static HashSet<RoleState> GetAllRoles(this IUserSession user)
        {
            var roles = new HashSet<RoleState>();
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return roles;
            }
            CalculateOnce(user);
            foreach (var roleID in GetAllRoleIDs(user))
            {
                RoleState role;
                if (user.AppHost.RoleSet.TryGetRole(roleID, out role))
                {
                    roles.Add(role);
                }
            }
            return roles;
        }
        #endregion

        #region GetGroups
        /// <summary>
        /// 从账户工作组二元关系直接得到的工作组，不包括从组织结构得到的工作组。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static HashSet<GroupState> GetGroups(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new HashSet<GroupState>();
            }
            CalculateOnce(user);

            return user.GetData<HashSet<GroupState>>(CURRENT_GROUPS);
        }
        #endregion

        #region GetFunctions
        /// <summary>
        /// 从账户功能关系直接得到的功能，不包括从组角色得到的功能。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static HashSet<FunctionState> GetFunctions(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new HashSet<FunctionState>();
            }
            CalculateOnce(user);

            return user.GetData<HashSet<FunctionState>>(CURRENT_FUNCTIONS);
        }
        #endregion

        public static HashSet<FunctionState> GetAllFunctions(this IUserSession user)
        {
            var functions = new HashSet<FunctionState>();
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return functions;
            }
            CalculateOnce(user);
            foreach (var functionID in GetAllFunctionIDs(user))
            {
                FunctionState function;
                if (user.AppHost.FunctionSet.TryGetFunction(functionID, out function))
                {
                    functions.Add(function);
                }
            }
            return functions;
        }

        /// <summary>
        /// 当前用户的直接菜单。
        /// <remarks>
        /// 直接菜单是通过账户与菜单建立的直接关系得到的菜单，不包括从账户所在的角色得到的菜单。
        /// </remarks>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static HashSet<MenuState> GetMenus(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new HashSet<MenuState>();
            }
            CalculateOnce(user);

            return user.GetData<HashSet<MenuState>>(CURRENT_MENUS);
        }

        /// <summary>
        /// 当前账户所关联的应用系统集。用户集应是被作为用户中心节点运行的权限中心系统维护的，权限中心系统将具体的用户与具体的应用系统建立起关联并将用户“投放”到相关应用系统。
        /// 应用系统的用户集隶属于中心节点，是中心节点用户集的投影、子集。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static HashSet<AppSystemState> GetAppSystems(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new HashSet<AppSystemState>();
            }
            CalculateOnce(user);

            return user.GetData<HashSet<AppSystemState>>(CURRENT_APPSYSTEMS);
        }

        #region GetAllMenus

        /// <summary>
        /// 当前账户得到的所有菜单。
        /// <remarks>
        /// 它是当前账户从账户菜单二元关系直接得到的菜单和从当前账户的全部角色中得到的菜单的并集。
        /// </remarks>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static HashSet<MenuState> GetAllMenus(this IUserSession user)
        {
            var menus = user.GetData<HashSet<MenuState>>(CURRENT_ALL_MENUS);
            if (menus == null)
            {
                menus = new HashSet<MenuState>();
                List<MenuState> menuList = new List<MenuState>();
                if (user.IsDeveloper())
                {
                    foreach (var menu in user.AppHost.MenuSet)
                    {
                        menuList.Add(menu);
                    }
                }
                else
                {
                    var roleIDs = new HashSet<Guid>();
                    foreach (var roleID in user.GetAllRoleIDs())
                    {
                        roleIDs.Add(roleID);
                    }
                    foreach (var roleMenu in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Menu && roleIDs.Contains(a.SubjectInstanceID)))
                    {
                        MenuState menu;
                        if (user.AppHost.MenuSet.TryGetMenu(roleMenu.ObjectInstanceID, out menu))
                        {
                            menuList.Add(menu);
                        }
                    }
                    foreach (var menu in user.GetMenus())
                    {
                        menuList.Add(menu);
                    }
                }
                foreach (var menu in menuList.OrderBy(a => a.SortCode))
                {
                    menus.Add(menu);
                }
                user.SetData(CURRENT_ALL_MENUS, menus);
            }
            return menus;
        }

        #endregion

        /// <summary>
        /// 添加激活角色。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        public static void AddActiveRole(this IUserSession user, RoleState role)
        {
            var roles = user.GetData<HashSet<RoleState>>(CURRENT_ROLES);
            var allRoleIDs = user.GetData<HashSet<Guid>>(CURRENT_ALL_ROLEIDS);
            if (roles != null)
            {
                roles.Add(role);
            }
            if (allRoleIDs != null)
            {
                allRoleIDs.Add(role.Id);
            }
        }

        /// <summary>
        /// 删除激活角色。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        public static void DropActiveRole(this IUserSession user, RoleState role)
        {
            var roles = user.GetData<HashSet<RoleState>>(CURRENT_ROLES);
            var allRoleIDs = user.GetData<HashSet<Guid>>(CURRENT_ALL_ROLEIDS);
            if (roles != null)
            {
                roles.Remove(role);
            }
            if (allRoleIDs != null)
            {
                allRoleIDs.Remove(role.Id);
            }
        }

        #region Permit
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaCode"></param>
        /// <param name="resourceCode"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public static bool Permit(this IUserSession user, string resourceCode, string functionCode)
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();
            ResourceTypeState resource;
            if (!user.AppHost.ResourceSet.TryGetResource(user.AppHost.AppSystemSet.SelfAppSystem, resourceCode, out resource))
            {
                throw new ValidationException("意外的资源码" + resourceCode);
            }
            FunctionState function;
            if (!user.AppHost.FunctionSet.TryGetFunction(resource, functionCode, out function))
            {
                return true;
            }
            return securityService.Permit(user, function, null);
        }

        public static bool Permit<TEntity, TInput>(this IUserSession user, string resourceCode, string functionCode, ManagedEntityData<TEntity, TInput> currentEntity)
            where TEntity : IManagedPropertyValues
            where TInput : IManagedPropertyValues
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();
            ResourceTypeState resource;
            if (!user.AppHost.ResourceSet.TryGetResource(user.AppHost.AppSystemSet.SelfAppSystem, resourceCode, out resource))
            {
                throw new ValidationException("意外的资源码" + resourceCode);
            }
            FunctionState function;
            if (!user.AppHost.FunctionSet.TryGetFunction(resource, functionCode, out function))
            {
                return true;
            }
            return securityService.Permit(user, function, currentEntity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static bool Permit(this IUserSession user, PageState page)
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            if (page == PageState.Empty)
            {
                return true;
            }
            FunctionState function;
            if (!user.AppHost.FunctionSet.TryGetFunction(page.Id, out function))
            {
                return true;
            }
            return securityService.Permit(user, function, null);
        }

        public static bool Permit<TEntity, TInput>(this IUserSession user, PageState page, ManagedEntityData<TEntity, TInput> currentEntity)
            where TEntity : IManagedPropertyValues
            where TInput : IManagedPropertyValues
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            if (page == PageState.Empty)
            {
                return true;
            }
            FunctionState function;
            if (!user.AppHost.FunctionSet.TryGetFunction(page.Id, out function))
            {
                return true;
            }
            return securityService.Permit(user, function, currentEntity);
        }


        // 延迟加载当前账户的权限列表，延迟到当用户触发托管操作时，节省内存
        // TODO:考虑按资源划分会话
        /// <summary>
        /// 判断当前用户是否具有给定的权限码标识的权限
        /// </summary>
        /// <returns>True表示有权，False无权</returns>
        public static bool Permit(this IUserSession user, Guid functionID)
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();
            FunctionState function;
            if (!user.AppHost.FunctionSet.TryGetFunction(functionID, out function))
            {
                return true;
            }
            return securityService.Permit(user, function, null);
        }

        public static bool Permit<TEntity, TInput>(this IUserSession user, Guid functionID, ManagedEntityData<TEntity, TInput> currentEntity)
            where TEntity : IManagedPropertyValues
            where TInput : IManagedPropertyValues
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();
            FunctionState function;
            if (!user.AppHost.FunctionSet.TryGetFunction(functionID, out function))
            {
                return true;
            }
            return securityService.Permit(user, function, currentEntity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static bool Permit(this IUserSession user, FunctionState function)
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();

            return securityService.Permit(user, function, null);
        }

        public static bool Permit<T, TInput>(this IUserSession user, FunctionState function, ManagedEntityData<T, TInput> currentEntity)
            where T : IManagedPropertyValues
            where TInput : IManagedPropertyValues
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();

            return securityService.Permit(user, function, currentEntity);
        }

        public static bool Permit(this IUserSession user, FunctionState function, ManagedEntityData currentEntity)
        {
            var securityService = user.AppHost.GetRequiredService<ISecurityService>();

            return securityService.Permit(user, function, currentEntity);
        }
        #endregion

        #region internal Methods
        #region CalculateOnce
        /// <summary>
        /// 计算当前UserSession的权限，对于一个Session实例来说只会计算一次。
        /// </summary>
        /// <param name="user"></param>
        private static void CalculateOnce(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return;
            }
            var inited = user.GetData<object>(CURRENT_RIVILEGE_INITED);
            if (inited == null)
            {
                var organizations = new HashSet<OrganizationState>();
                var roles = new HashSet<RoleState>();
                var groups = new HashSet<GroupState>();
                var functions = new HashSet<FunctionState>();
                var menus = new HashSet<MenuState>();
                var appSystems = new HashSet<AppSystemState>();
                foreach (var accountPrivilege in user.AccountPrivileges)
                {
                    switch (accountPrivilege.ObjectType)
                    {
                        case ACObjectType.Undefined:
                            break;
                        case ACObjectType.Account:
                            break;
                        case ACObjectType.Organization:
                            {
                                Guid organizationID = accountPrivilege.ObjectInstanceID;
                                OrganizationState organization;
                                if (user.AppHost.OrganizationSet.TryGetOrganization(organizationID, out organization))
                                {
                                    organizations.Add(organization);
                                }
                                break;
                            }
                        case ACObjectType.Role:
                            {
                                Guid roleID = accountPrivilege.ObjectInstanceID;
                                RoleState role;
                                if (user.AppHost.RoleSet.TryGetRole(roleID, out role))
                                {
                                    roles.Add(role);
                                }
                                break;
                            }
                        case ACObjectType.Group:
                            {
                                Guid groupID = accountPrivilege.ObjectInstanceID;
                                GroupState group;
                                if (user.AppHost.GroupSet.TryGetGroup(groupID, out group))
                                {
                                    groups.Add(group);
                                }
                                break;
                            }
                        case ACObjectType.Function:
                            {
                                Guid functionID = accountPrivilege.ObjectInstanceID;
                                FunctionState function;
                                if (user.AppHost.FunctionSet.TryGetFunction(functionID, out function))
                                {
                                    functions.Add(function);
                                }
                                break;
                            }
                        case ACObjectType.Menu:
                            {
                                Guid menuID = accountPrivilege.ObjectInstanceID;
                                MenuState menu;
                                if (user.AppHost.MenuSet.TryGetMenu(menuID, out menu))
                                {
                                    menus.Add(menu);
                                }
                                break;
                            }
                        case ACObjectType.AppSystem:
                            {
                                Guid appSystemID = accountPrivilege.ObjectInstanceID;
                                AppSystemState appSystem;
                                if (user.AppHost.AppSystemSet.TryGetAppSystem(appSystemID, out appSystem))
                                {
                                    appSystems.Add(appSystem);
                                }
                                break;
                            }
                        case ACObjectType.ResourceType:
                            break;
                        case ACObjectType.Privilege:
                            break;
                        default:
                            break;
                    }
                }
                user.SetData(CURRENT_RIVILEGE_INITED, new object());
                user.SetData(CURRENT_ORGANIZATIONS, organizations);
                user.SetData(CURRENT_ROLES, roles);
                user.SetData(CURRENT_GROUPS, groups);
                user.SetData(CURRENT_FUNCTIONS, functions);
                user.SetData(CURRENT_MENUS, menus);
                user.SetData(CURRENT_APPSYSTEMS, appSystems);
            }
        }
        #endregion

        /// <summary>
        /// 账户的角色授权
        /// 这些角色是以下角色集合的并集：
        /// 1，当前账户直接得到的角色；
        /// 2，当前账户所在的工作组的角色；
        /// 3，当前账户所在的组织结构的角色；
        /// 4，当前账户所在的组织结构加入的工作组的角色。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static HashSet<Guid> GetAllRoleIDs(this IUserSession user)
        {
            var allRoles = user.GetData<HashSet<Guid>>(CURRENT_ALL_ROLEIDS);
            if (allRoles == null)
            {
                allRoles = new HashSet<Guid>();
                foreach (var role in user.GetRoles())
                {
                    allRoles.Add(role.Id);
                }
                foreach (var organization in user.GetOrganizations())
                {
                    foreach (var item in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Organization && a.SubjectInstanceID == organization.Id))
                    {
                        if (item.ObjectType == ACObjectType.Role)
                        {
                            RoleState role;
                            if (user.AppHost.RoleSet.TryGetRole(item.ObjectInstanceID, out role))
                            {
                                allRoles.Add(role.Id);
                            }
                        }
                        else if (item.ObjectType == ACObjectType.Group)
                        {
                            foreach (var roleGroup in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Group && a.ObjectInstanceID == item.ObjectInstanceID))
                            {
                                RoleState role;
                                if (user.AppHost.RoleSet.TryGetRole(roleGroup.SubjectInstanceID, out role))
                                {
                                    allRoles.Add(role.Id);
                                }
                            }
                        }
                    }
                }
                foreach (var group in user.GetGroups())
                {
                    foreach (var roleGroup in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Group && a.ObjectInstanceID == group.Id))
                    {
                        RoleState role;
                        if (user.AppHost.RoleSet.TryGetRole(roleGroup.SubjectInstanceID, out role))
                        {
                            allRoles.Add(role.Id);
                        }
                    }
                }
                user.SetData(CURRENT_ALL_ROLEIDS, allRoles);
            }

            return allRoles;
        }

        /// <summary>
        /// 该方法只返回功能标识。功能审计方法基于这个方法返回功能集。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static HashSet<Guid> GetAllFunctionIDs(this IUserSession user)
        {
            var functionIDs = user.GetData<HashSet<Guid>>(CURRENT_ALL_FUNCTIONIDS);
            if (functionIDs == null)
            {
                functionIDs = new HashSet<Guid>();
                // TODO:考虑在PrivilegeSet集合中计算好缓存起来，从而可以直接根据角色索引而
                var roleIDs = user.GetAllRoleIDs();
                foreach (var privilegeBigram in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Function && roleIDs.Contains(a.SubjectInstanceID)))
                {
                    functionIDs.Add(privilegeBigram.ObjectInstanceID);
                }
                // 追加账户所在组织结构的直接功能授权
                foreach (var organization in user.GetOrganizations())
                {
                    foreach (var item in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Organization && a.ObjectType == ACObjectType.Function && a.SubjectInstanceID == organization.Id))
                    {
                        Guid functionID = item.ObjectInstanceID;
                        functionIDs.Add(functionID);
                    }
                }
                // 追加账户的直接功能授权
                foreach (var fun in user.GetFunctions())
                {
                    functionIDs.Add(fun.Id);
                }
                user.SetData(CURRENT_ALL_FUNCTIONIDS, functionIDs);
            }
            return functionIDs;
        }
        #endregion
    }
}
