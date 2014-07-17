
namespace Anycmd
{
    using Host;
    using Host.AC;
    using Host.AC.Identity;
    using Host.AC.ValueObjects;
    using System;
    using System.Collections.Generic;

    public interface IRBACService
    {
        /// <summary>
        /// 添加新用户，同时可选地为该新用户添加一个账户
        /// </summary>
        /// <param name="input"></param>
        void AddUser(IAccountCreateInput input);

        /// <summary>
        /// 删除给定标识的用户并同时删除该用户的所有账户
        /// </summary>
        /// <param name="userID"></param>
        void DeleteUser(Guid userID);

        /// <summary>
        /// 添加新角色
        /// </summary>
        /// <param name="input"></param>
        void AddRole(IRoleCreateInput input);

        /// <summary>
        /// 删除给定标识的角色。同时删除相应的角色权限关系、账户角色关系、组织结构角色关系、工作组角色关系。
        /// </summary>
        /// <param name="roleID"></param>
        void DeleteRole(Guid roleID);

        /// <summary>
        /// 指派账户
        /// </summary>
        /// <param name="role"></param>
        /// <param name="accountID"></param>
        void AssignUser(IAccountPrivilegeCreateInput input);

        /// <summary>
        /// 撤回指派的账户
        /// </summary>
        /// <param name="accountPrivilegeID"></param>
        void DeassignUser(Guid accountPrivilegeID);

        /// <summary>
        /// 为角色授权
        /// </summary>
        /// <param name="input"></param>
        void GrantPermission(IPrivilegeBigramCreateInput input);

        /// <summary>
        /// 撤回角色授权
        /// </summary>
        /// <param name="rolePrivilegeID"></param>
        void RevokePermission(Guid rolePrivilegeID);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IUserSession GetUserSession();

        /// <summary>
        /// 返回与一个给定角色相关联的账户集
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        List<Account> AssignedUsers(Guid roleID);

        /// <summary>
        /// 返回与一个给定角色直接相关联的账户以及与从此给定角色继承而来的角色直接相关联的用户集合。
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        List<Account> AuthorizedUsers(Guid roleID);

        /// <summary>
        /// 返回与一个给定账户相关联的角色集
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        List<Role> AssignedRoles(Guid accountID);

        /// <summary>
        /// 返回与一个给定账户直接相关联的角色以及那些被这些直接相关联的角色所继承的角色集合。
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        List<Role> AuthorizedRoles(Guid accountID);

        // TODO:定义新类型，RolePrivilege类型用于该方法的返回不合适。
        /// <summary>
        /// 返回与一个给定角色相关联的权限集
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        List<PrivilegeBigram> RolePermissions(Guid roleID);

        // TODO:定义新类型，AccountPrivilege类型用于该方法的返回不合。
        /// <summary>
        /// 返回一个账户通过与他或他相关联的角色所得到的权限集
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        List<PrivilegeBigramState> UserPermissions(Guid accountID);

        // TODO:定义新类型，RolePrivilege类型用于该方法的返回不合适。
        /// <summary>
        /// 返回一个给定角色对一个给定资源类型可能执行的操作集
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="resourceTypeID"></param>
        /// <returns></returns>
        List<PrivilegeBigram> RoleOperationsOnResource(Guid roleID, Guid resourceTypeID);

        // TODO:定义新类型，AccountPrivilege类型用于该方法的返回不合。
        /// <summary>
        /// 返回一个给定账户对一个给定资源类型可能执行的操作集
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="resourceTypeID"></param>
        /// <returns></returns>
        List<PrivilegeBigramState> UserOperationsOnResource(Guid accountID, Guid resourceTypeID);

        // TODO:定义新类型，RolePrivilege类型用于该方法的返回不合适。
        /// <summary>
        /// 返回一个给定角色对一个给定对象可能执行的操作集
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="entityTypeID"></param>
        /// <param name="objID"></param>
        /// <returns></returns>
        List<PrivilegeBigram> RoleOperationsOnObject(Guid roleID, Guid entityTypeID, Guid objID);

        // TODO:定义新类型，AccountPrivilege类型用于该方法的返回不合。
        /// <summary>
        /// 返回一个给定账户对一个给定对象（可直接获得的或通过与她或他相关联的角色获得的）可能执行的操作集。
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="entityTypeID"></param>
        /// <param name="objID"></param>
        /// <returns></returns>
        List<PrivilegeBigramState> UserOperationsOnObject(Guid accountID, Guid entityTypeID, Guid objID);

        /// <summary>
        /// 在两个已经存在的角色之间建立直接继承关系
        /// </summary>
        /// <param name="parentRoleID"></param>
        /// <param name="childRoleID"></param>
        void AddInheritance(Guid parentRoleID, Guid childRoleID);

        /// <summary>
        /// 删除已经存在于两个角色之间的直接集成关系
        /// </summary>
        /// <param name="roleInheritanceID"></param>
        void DeleteInheritance(Guid roleInheritanceID);

        /// <summary>
        /// 创建一个新的角色，将它作为一个现有角色的前驱加入。
        /// </summary>
        /// <param name="childRoleID"></param>
        /// <param name="input"></param>
        void AddAscendant(Guid childRoleID, IRoleCreateInput input);

        /// <summary>
        /// 创建一个新的角色，将它作为一个现有角色的后继加入。
        /// </summary>
        /// <param name="parentRoleID"></param>
        /// <param name="input"></param>
        void AddDescendant(Guid parentRoleID, IRoleCreateInput input);

        /// <summary>
        /// 建立一个指定的SSD（静态责任分割）关系实例。
        /// </summary>
        /// <param name="input"></param>
        void CreateSSDSet(ISSDSetCreateInput input);

        /// <summary>
        /// 删除一个已有的SSD（静态责任分割）关系。
        /// </summary>
        /// <param name="ssdSetID"></param>
        void DeleteSSDSet(Guid ssdSetID);

        /// <summary>
        /// 添加一个角色到一个SSD(静态责任分割)角色集中。
        /// </summary>
        /// <param name="ssdSetID"></param>
        /// <param name="roleID"></param>
        void AddSSDRoleMember(Guid ssdSetID, Guid roleID);

        /// <summary>
        /// 从一个SSD(静态责任分割)关系集中删除一个角色。
        /// </summary>
        /// <param name="ssdRoleID"></param>
        void DeleteSSDRoleMember(Guid ssdRoleID);

        /// <summary>
        /// 设置由受限的普通用户集合所应用的SSD角色集子集的势。
        /// </summary>
        /// <param name="ssdSetID"></param>
        /// <param name="cardinality"></param>
        void SetSSDCardinality(Guid ssdSetID, int cardinality);

        /// <summary>
        /// 返回为SSD(静态责任分割)模型建立的指定SSD关系的集合。
        /// </summary>
        /// <param name="ssdSetID"></param>
        /// <returns></returns>
        List<SSDRole> SSDRoleSets(Guid ssdSetID);

        // TODO:疑惑。搜索关键字“鉴别与授权基于角色的访问控制模型”
        /// <summary>
        /// 返回与一个指定SSD角色集合相关联的角色的集合
        /// </summary>
        /// <param name="ssdSetID"></param>
        /// <returns></returns>
        List<Role> SSDRoleSetRoles(Guid ssdSetID);

        /// <summary>
        /// 返回由首先的普通用户集合所应用的SSD角色集子集的势
        /// </summary>
        /// <param name="ssdSetID"></param>
        /// <returns></returns>
        int SSDRoleSetCardinality(Guid ssdSetID);

        void CreateDSDSet(IDSDSetCreateInput input);

        void DeleteDSDSet(Guid dsdSetID);

        void AddDSDRoleMember(Guid dsdSetID, Guid roleID);

        void DeleteDSDRoleMember(Guid dsdRoleID);

        void SetDSDCardinality(Guid dsdSetID, int cardinality);

        List<DSDRole> DSDRoleSets(Guid dsdSetID);

        List<Role> DSDRoleSetRoles(Guid dsdSetID);

        int DSDRoleSetCardinality(Guid dsdSetID);
    }
}
