
namespace Anycmd.ViewModel
{
    using Anycmd.Host;
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
        Dictionary<string, object> Get(string tableOrViewName, Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="filters"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetPlist(string tableOrViewName, Func<SqlFilter> filterCallback, PagingInput paging);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="filters"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetPlist(EntityTypeState entityType, Func<SqlFilter> filterCallback, PagingInput paging);
    }
}
