
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 定义操作、功能
    /// </summary>
    public interface IFunction
    {
        Guid Id { get; }
        /// <summary>
        /// 所属资源
        /// </summary>
        Guid ResourceTypeID { get; }
        /// <summary>
        /// 操作码
        /// </summary>
        string Code { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsManaged { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid DeveloperID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int SortCode { get; }
        /// <summary>
        /// 
        /// </summary>
        int IsEnabled { get; }
        /// <summary>
        /// 操作说明
        /// </summary>
        string Description { get; }
    }
}
