
namespace Anycmd.AC
{
    using System;

    /// <summary>
    /// 工作组
    /// </summary>
    public interface IGroup
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 可空的
        /// </summary>
        string OrganizationCode { get; }
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        string CategoryCode { get; }

        int SortCode { get; }

        int IsEnabled { get; }

        int? PrivilegeState { get; }
    }
}
