
namespace Anycmd.AC.Identity.ViewModels.AccountViewModels
{
    using Query;
    using System;
    using System.Collections.Generic;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public interface IAccountQuery : IQuery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="organizationCode"></param>
        /// <param name="includeDescendants"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetPlistAccountTrs(List<FilterData> filters, string organizationCode, bool includeDescendants, PagingInput paging);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="roleID"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetPlistRoleAccountTrs(string key, Guid roleID, PagingInput paging);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="groupID"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetPlistGroupAccountTrs(string key, Guid groupID, PagingInput paging);

        List<Dictionary<string, object>> GetPlistContractorTrs(List<FilterData> filters, string organizationCode, bool includeDescendants, PagingInput paging);
    }
}
