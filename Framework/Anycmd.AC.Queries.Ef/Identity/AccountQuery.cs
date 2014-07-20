
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
	/// 查询接口实现<see cref="IAccountQuery"/>
	/// </summary>
	public sealed class AccountQuery : QueryBase, IAccountQuery
	{
		public AccountQuery(AppHost host)
			: base(host, "IdentityEntities")
		{
		}

		#region GetPlistAccountTrs
		public List<DicReader> GetPlistAccountTrs(List<FilterData> filters, string organizationCode, bool includeDescendants, PagingInput paging)
		{
			paging.Valid();
			bool byOrgCode = !string.IsNullOrEmpty(organizationCode);
			Func<SqlFilter> filter = () =>
			{
				List<SqlParameter> parameters;
				var filterString = new SqlFilterStringBuilder().FilterString(filters, "a", out parameters);
				if (!string.IsNullOrEmpty(filterString))
				{
					filterString = " where 1=1 and " + filterString;
				}
				else
				{
					filterString = " where 1=1 ";
				}
				if (!includeDescendants)
				{
					if (byOrgCode)
					{
						if (!string.IsNullOrEmpty(organizationCode))
						{
							parameters.Add(new SqlParameter("OrganizationCode", organizationCode));
							filterString += "and a.OrganizationCode=@OrganizationCode ";
						}
					}
				}
				else
				{
					if (byOrgCode)
					{
						if (!string.IsNullOrEmpty(organizationCode))
						{
							parameters.Add(new SqlParameter("OrganizationCode", organizationCode + "%"));
							filterString += "and a.OrganizationCode like @OrganizationCode ";
						}
					}
				}
				return new SqlFilter(filterString, parameters.ToArray());
			};
			return base.GetPlist("AccountTr", filter, paging);
		}
		#endregion

		#region GetPlistRoleAccountTrs
		public List<DicReader> GetPlistRoleAccountTrs(string key, Guid roleID, PagingInput paging)
		{
			paging.Valid();
			Func<SqlFilter> filter = () =>
			{
				List<SqlParameter> parameters = new List<SqlParameter>();
				var filterString =
@" where (a.Name like @key
	or a.Code like @key
	or a.LoginName like @key) and a.RoleID=@RoleID";
				parameters.Add(new SqlParameter("key", "%" + key + "%"));
				parameters.Add(new SqlParameter("RoleID", roleID));
				return new SqlFilter(filterString, parameters.ToArray());
			};
			return base.GetPlist("RoleAccountTr", filter, paging);
		}
		#endregion

		#region GetPlistGroupAccountTrs
		public List<DicReader> GetPlistGroupAccountTrs(string key, Guid groupID, PagingInput paging)
		{
			paging.Valid();
			Func<SqlFilter> filter = () =>
			{
				List<SqlParameter> parameters = new List<SqlParameter>();
				var filterString =
@" where (a.Name like @key
	or a.Code like @key
	or a.LoginName like @key) and a.GroupID=@GroupID";
				parameters.Add(new SqlParameter("key", "%" + key + "%"));
				parameters.Add(new SqlParameter("GroupID", groupID));
				return new SqlFilter(filterString, parameters.ToArray());
			};
			return base.GetPlist("GroupAccountTr", filter, paging);
		}
		#endregion

		public List<DicReader> GetPlistContractorTrs(List<FilterData> filters, string organizationCode, bool includeOrgChild, PagingInput paging)
		{
			paging.Valid();
			bool byOrgCode = !string.IsNullOrEmpty(organizationCode);
			Func<SqlFilter> filter = () =>
			{
				List<SqlParameter> parameters;
				var filterString = new SqlFilterStringBuilder().FilterString(filters, "a", out parameters);
				if (!string.IsNullOrEmpty(filterString))
				{
					filterString = " where 1=1 and " + filterString;
				}
				else
				{
					filterString = " where 1=1 ";
				}
				if (!includeOrgChild)
				{
					if (byOrgCode)
					{
						if (!string.IsNullOrEmpty(organizationCode))
						{
							parameters.Add(new SqlParameter("OrganizationCode", organizationCode));
							filterString += " and a.OrganizationCode=@OrganizationCode";
						}
					}
				}
				else
				{
					if (byOrgCode)
					{
						if (!string.IsNullOrEmpty(organizationCode))
						{
							parameters.Add(new SqlParameter("OrganizationCode", organizationCode + "%"));
							filterString += " and a.OrganizationCode like @OrganizationCode";
						}
					}
				}
				return new SqlFilter(filterString, parameters.ToArray());
			};
			return base.GetPlist("AccountTr", filter, paging);
		}
	}
}
