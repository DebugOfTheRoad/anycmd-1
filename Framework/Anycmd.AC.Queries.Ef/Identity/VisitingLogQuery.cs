
namespace Anycmd.AC.Identity.Queries.Ef
{
    using Anycmd.Ef;
    using Model;
    using Query;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using ViewModels.AccountViewModels;

    /// <summary>
    /// 查询接口实现<see cref="IVisitingLogQuery"/>
    /// </summary>
    public sealed class VisitingLogQuery : QueryBase, IVisitingLogQuery
    {
        public VisitingLogQuery(AppHost host)
            : base(host, "IdentityEntities")
        {
        }

        public List<DicReader> GetPlistVisitingLogTrs(string key, DateTime? leftVisitOn, DateTime? rightVisitOn, PagingInput paging)
        {
            paging.Valid();
            if (key != null)
            {
                key = key.Trim();
            }
            Func<SqlFilter> filter = () =>
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                var filterString = @" where a.LoginName like @key ";
                parameters.Add(new SqlParameter("key", "%" + key + "%"));
                if (leftVisitOn.HasValue)
                {
                    parameters.Add(new SqlParameter("leftVisitOn", leftVisitOn.Value));
                    filterString += "and a.VisitOn>=@leftVisitOn";
                }
                if (rightVisitOn.HasValue)
                {
                    parameters.Add(new SqlParameter("rightVisitOn", rightVisitOn.Value));
                    filterString += "and a.VisitOn<@rightVisitOn";
                }
                return new SqlFilter(filterString, parameters.ToArray());
            };
            return base.GetPlist("VisitingLog", filter, paging);
        }

        public List<DicReader> GetPlistVisitingLogTrs(Guid accountID, string loginName, DateTime? leftVisitOn, DateTime? rightVisitOn, PagingInput paging)
        {
            paging.Valid();
            loginName = (loginName ?? string.Empty).ToLower();
            Func<SqlFilter> filter = () =>
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                var filterString = @" where (a.AccountID=@AccountID or Lower(a.LoginName)=@LoginName) ";
                parameters.Add(new SqlParameter("LoginName", loginName));
                parameters.Add(new SqlParameter("AccountID", accountID));
                if (leftVisitOn.HasValue)
                {
                    parameters.Add(new SqlParameter("leftVisitOn", leftVisitOn.Value));
                    filterString += "and a.VisitOn>=@leftVisitOn";
                }
                if (rightVisitOn.HasValue)
                {
                    parameters.Add(new SqlParameter("rightVisitOn", rightVisitOn.Value));
                    filterString += "and a.VisitOn<@rightVisitOn";
                }
                return new SqlFilter(filterString, parameters.ToArray());
            };
            return base.GetPlist("VisitingLog", filter, paging);
        }
    }
}
