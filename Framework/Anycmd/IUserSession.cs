
namespace Anycmd
{
    using Host;
    using System.Collections.Generic;
    using System.Security.Principal;

    /// <summary>
    /// 用户会话。起标识用户的作用，在AC命名空间下会往这个接口上扩展一些AC方面的方法。
    /// </summary>
    public interface IUserSession
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        AppHost AppHost { get; }
        /// <summary>
        /// 当事人
        /// </summary>
        IPrincipal Principal { get; }
        /// <summary>
        /// 工人
        /// </summary>
        /// <returns></returns>
        AccountState Worker { get; }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyCollection<PrivilegeBigramState> AccountPrivileges { get; }
    }
}
