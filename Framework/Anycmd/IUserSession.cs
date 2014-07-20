
namespace Anycmd
{
    using Host;
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    /// <summary>
    /// 用户会话。起标识用户的作用，在AC命名空间下会往这个接口上扩展一些AC方面的方法。
    /// <para>
    /// 持久的UserSession与内存中的UserSession：持久UserSession是对内存中的UserSession的持久跟踪，是对实现会话级的动态责任分离特性的必要准备。
    /// 持久的UserSession是这样一个概念，一个账户在第一次登录的时候会建立一个内存中的UserSession，这个UserSession会被持久化起来。用户退出系统时
    /// 会更新持久的UserSession的IsAuthenticated为false但不会删除这条UserSession记录。用户下次登录的成功时IsAuthenticated会再次更新为true，
    /// 持久的UserSession只在用户登录和退出系统时访问，持久的UserSession的存在使得安全管理员有机会面向用户的UserSession建立用户会话级的动态责任分离策略。
    /// </para>
    /// <para>
    /// 一个账户可以对应多个UserSession，安全管理员可以控制哪个UserSession在什么情况下激活而哪些UserSession不能激活。安全管理员可以为某个账户建立新的
    /// UserSession但不马上切换为它，安全管理员针对这个UserSession进行会话级的动态责任分离授权并测试符合预期后再禁用用户原来的UserSession切换为新的UserSession，
    /// 系统可以让UserSession被禁用的那个账户下线然后他再次登录就切换到新的UserSession了，系统也应该能做到在用户不知觉的情况下平滑的切换掉他的UserSession。
    /// </para>
    /// </summary>
    public interface IUserSession
    {
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IAppHost Host { get; }
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
