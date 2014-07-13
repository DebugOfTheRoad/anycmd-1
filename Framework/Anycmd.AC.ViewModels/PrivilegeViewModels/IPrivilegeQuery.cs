
namespace Anycmd.AC.ViewModels.PrivilegeViewModels
{
    using Query;
    using System.Collections.Generic;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public interface IPrivilegeQuery : IQuery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="organizationCode"></param>
        /// <param name="includeDescendants"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetPlistOrganizationAccountTrs(string key, string organizationCode, bool includeDescendants, PagingInput paging);
    }
}
