
namespace Anycmd.Host
{
    using Anycmd.AC;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultSecurityService : ISecurityService
    {
        public bool Permit(IUserSession user, FunctionState function, IManagedEntityData data)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            // 如果非托管
            if (!function.IsManaged)
            {
                return true;
            }
            if (!user.Principal.Identity.IsAuthenticated)
            {
                return false;
            }
            if (user.IsDeveloper())
            {
                return true;
            }
            var functionIDs = user.GetData<HashSet<Guid>>(ConstKeys.CURRENT_USER_FUNCTIONS);
            if (functionIDs == null)
            {
                functionIDs = new HashSet<Guid>();
                // TODO:考虑在PrivilegeSet集合中计算好缓存起来，从而可以直接根据角色索引而
                // 账户的角色授权
                // 这些角色是以下角色集合的并集：
                // 1，当前账户直接得到的角色；
                // 2，当前账户所在的工作组的角色；
                // 3，当前账户所在的组织结构的角色；
                // 4，当前账户所在的组织结构加入的工作组的角色。
                var roles = user.GetAllRoles();
                foreach (var privilegeBigram in user.AppHost.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Function && roles.Any(r => r.Id == a.SubjectInstanceID)))
                {
                    functionIDs.Add(privilegeBigram.ObjectInstanceID);
                }
                // 追加账户的直接功能授权
                foreach (var privilegeBigram in user.GetAccountPrivileges().Where(a => a.ObjectType == ACObjectType.Function))
                {
                    functionIDs.Add(privilegeBigram.ObjectInstanceID);
                }
                user.SetData(ConstKeys.CURRENT_USER_FUNCTIONS, functionIDs);
            }

            if (!functionIDs.Contains(function.Id))
            {
                return false;
            }
            if (data != null)
            {
                // TODO:验证实体级权限
            }
            return true;
        }
    }
}
