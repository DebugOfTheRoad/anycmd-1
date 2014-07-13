
namespace Anycmd.Host.AC.Infra
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 系统页面集合
    /// </summary>
    public interface IPageSet : IEnumerable<PageState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <exception cref="CoreException">当索引的页面不存在时引发</exception>
        bool TryGetPage(FunctionState function, out PageState page);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        bool TryGetPage(Guid pageID, out PageState page);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<PageButtonState> GetPageButtons();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        IReadOnlyList<PageButtonState> GetPageButtons(PageState page);
    }
}
