
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 系统字典
    /// </summary>
    public interface IDic
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 字典码
        /// </summary>
        string Code { get; }
        /// <summary>
        /// 字典名
        /// </summary>
        string Name { get; }

        int IsEnabled { get; }

        int SortCode { get; }
    }
}
