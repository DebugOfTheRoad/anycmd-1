
namespace Anycmd.Host
{
    using Model;
    using System;

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
            var functionIDs = user.GetAllFunctionIDs();

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
