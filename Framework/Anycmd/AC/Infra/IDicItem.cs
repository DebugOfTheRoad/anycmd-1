
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 系统字典项
    /// </summary>
    public interface IDicItem
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 编码
        /// </summary>
        string Code { get; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 所属字典
        /// </summary>
        Guid DicID { get; }
        /// <summary>
        /// 排序
        /// </summary>
        int SortCode { get; }

        int IsEnabled { get; }
    }
}
