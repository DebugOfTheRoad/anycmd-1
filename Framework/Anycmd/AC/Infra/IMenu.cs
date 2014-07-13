
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 菜单模型接口
    /// <remarks>
    /// 系统中的菜单分为两种：一种是固化的菜单，固化的菜单可以分配给角色进而；
    /// 另一种是业务菜单，这种菜单不是固化的而是根据当前用户的数据集权限生成的。
    /// 而当前用户的菜单是这两种菜单的并集。
    /// </remarks>
    /// </summary>
    public interface IMenu
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        Guid AppSystemID { get; }

        /// <summary>
        /// 父级菜单的ID
        /// </summary>
        Guid? ParentID { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 菜单对应的Url
        /// </summary>
        string Url { get; }

        int? AllowEdit { get; }
        int? AllowDelete { get; }
        int SortCode { get; }

        /// <summary>
        /// 图标
        /// </summary>
        string Icon { get; }

        string Description { get; }
    }
}
