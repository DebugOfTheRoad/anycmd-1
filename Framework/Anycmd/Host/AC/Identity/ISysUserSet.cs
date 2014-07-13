
namespace Anycmd.Host.AC.Identity
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 账户上下文访问接口。访问管理员和开发人员账户，普通用户的账户上下文访问<see cref="UserSession"/>
    /// </summary>
    public interface ISysUserSet
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 获取所有开发人员
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<AccountState> GetDevAccounts();

        /// <summary>
        /// 获取与指定的开发人员标识相关联的账户对象
        /// </summary>
        /// <param name="accountID">开发人员标识，即账户标识</param>
        /// <param name="account">开发人员账户</param>
        /// <returns></returns>
        bool TryGetDevAccount(Guid accountID, out AccountState account);

        /// <summary>
        /// 获取与指定的开发人员登录名相关联的账户对象
        /// </summary>
        /// <param name="loginName">开发人员登录名</param>
        /// <param name="account"></param>
        /// <returns></returns>
        bool TryGetDevAccount(string loginName, out AccountState account);
    }
}
