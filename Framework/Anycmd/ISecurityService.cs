
namespace Anycmd
{
    using Anycmd.Host;
    using Anycmd.Model;

    /// <summary>
    /// 安全服务接口
    /// </summary>
    public interface ISecurityService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">主体</param>
        /// <param name="function">function = action = resourceType + verb</param>
        /// <param name="data">可为null，表示不验证实体级权限</param>
        /// <returns></returns>
        bool Permit(IUserSession user, FunctionState function, IManagedEntityData data);
    }
}
