
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 页面菜单模型接口
    /// </summary>
    public interface IPageButton
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid PageID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid? FunctionID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid ButtonID { get; }

        /// <summary>
        /// 菜单在界面的有效状态
        /// <remarks>是否可点击的意思</remarks>
        /// </summary>
        int IsEnabled { get; }
    }
}
