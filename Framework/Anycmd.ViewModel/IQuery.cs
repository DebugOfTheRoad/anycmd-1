
namespace Anycmd.ViewModel
{
    using Host;
    using Model;
    using Query;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 查询接口
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        DicReader Get(string tableOrViewName, Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="filters"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        List<DicReader> GetPlist(string tableOrViewName, Func<SqlFilter> filterCallback, PagingInput paging);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="filters"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        List<DicReader> GetPlist(EntityTypeState entityType, Func<SqlFilter> filterCallback, PagingInput paging);
    }
}
