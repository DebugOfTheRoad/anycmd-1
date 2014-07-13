
namespace Anycmd.Host.AC
{
    using System.Collections.Generic;

    /// <summary>
    /// 权限集。
    /// <remarks>
    /// 它是RolePrivilege、OrganizationPrivilege的集合而不是AccountPrivilege的集合。AccountPrivilege的集合是会话级的，在UserSession中而不在这里。
    /// </remarks>
    /// </summary>
    public interface IPrivilegeSet : IEnumerable<PrivilegeBigramState>
    {
    }
}
