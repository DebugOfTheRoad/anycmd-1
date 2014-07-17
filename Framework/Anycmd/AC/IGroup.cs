
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
        /// 工作组所属组织结构，如果该属性有指向组织结构的值的话该工作组就是岗位。岗位下的账户只来自这个组织结构和其子组织结构。
        /// <remarks>
        /// 工作组是跨组织结构的资源组。工作组是不应该绑定到具体组织结构的，“岗位”是一种绑定到具体组织结构的资源组，而这里的OrganizationCode属性就是用来把工作组绑定到具体的组织结构从而成为“岗位”。
        /// </remarks>
        /// </summary>
        string OrganizationCode { get; }
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        string CategoryCode { get; }

        int SortCode { get; }

        int IsEnabled { get; }
    }
}
