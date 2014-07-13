
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 定义页面
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 帮助、提示信息
        /// </summary>
        string Tooltip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string Icon { get; }
    }
}
