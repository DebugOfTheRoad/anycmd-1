
namespace Anycmd.AC
{
    using System;

    /// <summary>
    /// 角色。角色是智能性团队。是能力团队。同一角色的人完全相同。比如10086接线员
    /// 是一个角色，对你来说根本不关心是哪个具体的接线员，因为它们能力相同。
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// 标识
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 角色名称
        /// </summary>
        string Name { get; }

        string CategoryCode { get; }

        int IsEnabled { get; }

        int? PrivilegeState { get; }

        string Icon { get; }

        int SortCode { get; }
    }
}
