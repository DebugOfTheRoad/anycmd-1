
namespace Anycmd.AC.Infra
{
    using System;

    // TODO:重命名为ResourceType
    /// <summary>
    /// 资源模型接口
    /// </summary>
    public interface IResourceType
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        Guid AppSystemID { get; }

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
        int AllowEdit { get; }
        /// <summary>
        /// 
        /// </summary>
        int AllowDelete { get; }

        /// <summary>
        /// 
        /// </summary>
        string Icon { get; }

        int SortCode { get; }
    }
}
