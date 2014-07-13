
namespace Anycmd.AC.Identity
{
    using Model;
    using System;

    /// <summary>
    /// 账户模型接口
    /// </summary>
    public interface IAccount : IEntity
    {
        /// <summary>
        /// 数字标识
        /// <remarks>
        /// 数字标识作为对人类友好的标识提供给外部。如审计系统。审批工作流中的角色采用数字标识。
        /// </remarks>
        /// </summary>
        int NumberID { get; }

        /// <summary>   
        /// 账户所属用户用户ID
        /// </summary>
        Guid? ContractorID { get; }

        /// <summary>
        /// 登录名
        /// </summary>
        string LoginName { get; }

        int? PrivilegeState { get; }

        /// <summary>
        /// 我的本体
        /// </summary>
        string Theme { get; }

        /// <summary>
        /// 我的墙纸
        /// </summary>
        string Wallpaper { get; }

        /// <summary>
        /// 我的背景色
        /// </summary>
        string BackColor { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 编码
        /// </summary>
        string Code { get; }

        /// <summary>
        /// 
        /// </summary>
        string Email { get; }

        /// <summary>
        /// 
        /// </summary>
        string QQ { get; }

        /// <summary>
        /// 
        /// </summary>
        string Mobile { get; }

        /// <summary>
        /// 
        /// </summary>
        string Telephone { get; }
    }
}
