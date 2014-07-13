
namespace Anycmd.Host.EDI {
    using Hecp;

    /// <summary>
    /// 节点身份验证器/令牌验证器
    /// </summary>
    public interface IAuthenticator : IWfResource {
        /// <summary>
        /// 认证给定的EDI请求
        /// </summary>
        /// <param name="request">EDI请求消息</param>
        /// <returns></returns>
        ProcessResult Auth(HecpRequest request);
    }
}
