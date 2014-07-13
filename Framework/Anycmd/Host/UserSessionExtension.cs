
namespace Anycmd.Host
{
    using Anycmd.AC;
    using Anycmd.AC.Infra;
    using Exceptions;
    using Host.AC;
    using Model;
    using Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Util;

    public static class UserSessionExtension
    {
        private const string CURRENT_ORGANIZATIONS = "Current_Organizations";
        private const string CURRENT_ALLROLES = "UserContext_Current_AllRoles";
        private const string CURRENT_ROLES = "UserContext_Current_Roles";
        private const string CURRENT_GROUPS = "UserContext_Current_Groups";
        private const string CURRENT_ALLMENUS = "UserContext_Current_AllMenus";
        private const string CURRENT_ALLPRIVILEGES = "UserContext_Current_AllPrivileges";

        #region IsDeveloper
        /// <summary>
        /// 判断当前用户是否是超级管理员
        /// </summary>
        /// <returns>True表示是超级管理员，False不是</returns>
        public static bool IsDeveloper(this IUserSession user)
        {
            AccountState account;
            return user.Principal.Identity.IsAuthenticated && user.AppHost.SysUsers.TryGetDevAccount(user.GetAccountID(), out account);
        }
        #endregion

        #region GetOrganizations
        /// <summary>
        /// 获取当前用户直接担任管理员的组织结构，直接担任管理员的组织结构不包括直接管理的组织结构下属的组织结构。
        /// </summary>
        /// <returns></returns>
        public static IList<IOrganization> GetOrganizations(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new List<IOrganization>();
            }
            GetAccountPrivileges(user);

            return user.GetData<IList<IOrganization>>(CURRENT_ORGANIZATIONS);
        }
        #endregion

        #region GetRoles
        /// <summary>
        /// 从账户角色二元关系直接得到的角色。不包括从组织结构和工作组等得到的角色。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IList<IRole> GetRoles(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new List<IRole>();
            }
            GetAccountPrivileges(user);

            return user.GetData<IList<IRole>>(CURRENT_ROLES);
        }
        #endregion

        #region GetGroups
        /// <summary>
        /// 从账户工作二元关系直接得到的工作组，不包括从组织结构得到的工作组。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IList<IGroup> GetGroups(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new List<IGroup>();
            }
            GetAccountPrivileges(user);

            return user.GetData<IList<IGroup>>(CURRENT_GROUPS);
        }
        #endregion

        #region GetAllMenus
        /// <summary>
        /// 获取当前用户的菜单列表
        /// </summary>
        /// <returns></returns>
        public static HashSet<IMenu> GetAllMenus(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new HashSet<IMenu>();
            }
            GetAccountPrivileges(user);

            return user.GetData<HashSet<IMenu>>(CURRENT_ALLMENUS);
        }
        #endregion

        public static void AddActiveRole(this IUserSession user, RoleState role)
        {
            throw new NotImplementedException();
        }

        public static void DropActiveRole(this IUserSession user, RoleState role)
        {
            throw new NotImplementedException();
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
        #region GetAllRoles
        /// <summary>
        /// <remarks>
        /// 这些角色是以下角色集合的并集：
        /// 1，当前账户直接得到的角色；
        /// 2，当前账户所在的工作组的角色；
        /// 3，当前账户所在的组织结构的角色；
        /// 4，当前账户所在的组织结构加入的工作组的角色。
        /// </remarks>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static HashSet<IRole> GetAllRoles(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new HashSet<IRole>();
            }
            GetAccountPrivileges(user);

            return user.GetData<HashSet<IRole>>(CURRENT_ALLROLES);
        }
        #endregion

        #region GetAccountPrivileges
        internal static IList<PrivilegeBigramState> GetAccountPrivileges(this IUserSession user)
        {
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return new List<PrivilegeBigramState>();
            }
            var accountPrivileges = user.GetData<IList<PrivilegeBigramState>>(CURRENT_ALLPRIVILEGES);
            if (accountPrivileges == null)
            {
                var accountID = user.GetAccountID();
                var subjectType = ACSubjectType.Account.ToName();
                accountPrivileges = user.AppHost.GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().Where(a => a.SubjectType == subjectType && a.SubjectInstanceID == accountID).ToList().Select(a => PrivilegeBigramState.Create(a)).ToList();
                var organizations = new List<IOrganization>();
                var roles = new List<IRole>();
                var allRoles = new HashSet<IRole>();
                var groups = new List<IGroup>();
                var menus = new HashSet<IMenu>();
                var menuList = new List<IMenu>();
                foreach (var accountPrivilege in accountPrivileges)
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
                                    foreach (var orgRole in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Organization && a.SubjectInstanceID == organization.Id && a.ObjectType == ACObjectType.Role))
                                    {
                                        RoleState role;
                                        if (user.AppHost.RoleSet.TryGetRole(orgRole.ObjectInstanceID, out role))
                                        {
                                            allRoles.Add(role);
                                        }
                                    }
                                    foreach (var groupID in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Organization && a.SubjectInstanceID == organization.Id && a.ObjectType == ACObjectType.Group).Select(a => a.ObjectInstanceID))
                                    {
                                        foreach (var roleGroup in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Group && a.ObjectInstanceID == groupID))
                                        {
                                            RoleState role;
                                            if (user.AppHost.RoleSet.TryGetRole(roleGroup.SubjectInstanceID, out role))
                                            {
                                                allRoles.Add(role);
                                            }
                                        }
                                    }
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
                                    allRoles.Add(role);
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
                                    foreach (var roleGroup in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Group && a.ObjectInstanceID == group.Id))
                                    {
                                        RoleState role;
                                        if (user.AppHost.RoleSet.TryGetRole(roleGroup.SubjectInstanceID, out role))
                                        {
                                            allRoles.Add(role);
                                        }
                                    }
                                }
                                break;
                            }
                        case ACObjectType.Function:
                            break;
                        case ACObjectType.Menu:
                            {
                                MenuState menu;
                                if (user.AppHost.MenuSet.TryGetMenu(accountPrivilege.ObjectInstanceID, out menu))
                                {
                                    menuList.Add(menu);
                                }
                                break;
                            }
                        case ACObjectType.AppSystem:
                            break;
                        case ACObjectType.ResourceType:
                            break;
                        case ACObjectType.Privilege:
                            break;
                        default:
                            break;
                    }
                }
                user.SetData(CURRENT_ORGANIZATIONS, organizations);
                user.SetData(CURRENT_ROLES, roles);
                user.SetData(CURRENT_ALLROLES, allRoles);
                user.SetData(CURRENT_GROUPS, groups);
                user.SetData(CURRENT_ALLPRIVILEGES, accountPrivileges);
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
                    foreach (var role in allRoles)
                    {
                        roleIDs.Add(role.Id);
                    }
                    foreach (var roleMenu in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Menu && roleIDs.Contains(a.SubjectInstanceID)))
                    {
                        MenuState menu;
                        if (user.AppHost.MenuSet.TryGetMenu(roleMenu.ObjectInstanceID, out menu))
                        {
                            menuList.Add(menu);
                        }
                    }
                }
                foreach (var menu in menuList.OrderBy(a => a.SortCode))
                {
                    menus.Add(menu);
                }
                user.SetData(CURRENT_ALLMENUS, menus);
            }

            return accountPrivileges;
        }
        #endregion
        #endregion
    }
}
