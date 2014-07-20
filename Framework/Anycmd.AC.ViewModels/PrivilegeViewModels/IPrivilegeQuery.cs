
namespace Anycmd.AC.ViewModels.PrivilegeViewModels
{
    using Model;
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
        List<DicReader> GetPlistOrganizationAccountTrs(string key, string organizationCode, bool includeDescendants, PagingInput paging);
    }
}
