
namespace Anycmd.Host
{
    using AC.Identity;
    using Anycmd.Rdb;
    using Dapper;
    using Exceptions;
    using System;
    using System.Data;
    using System.Linq;
    using System.Security.Principal;
    using System.Web;

    public sealed class DefaultUserSession : IUserSession
    {
        private readonly AppHost host;
        private readonly Guid dbID = new Guid("67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB");
        private RdbDescriptor _db;

        // 置为私有
        public DefaultUserSession(AppHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
        }

        /// <summary>
        /// 
        /// </summary>
        public IPrincipal Principal
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.User;
                }
                else
                {
                    throw new NotSupportedException("暂不支持非ASP.NET环境");
                }
            }
        }

        public AppHost AppHost
        {
            get { return host; }
        }

        public RdbDescriptor Db
        {
            get
            {
                if (_db == null)
                {
                    if (!host.Rdbs.TryDb(dbID, out _db))
                    {
                        throw new CoreException("意外的数据库标识" + dbID);
                    }
                }
                return _db;
            }
        }

        #region GetAccount
        /// <summary>
        /// 当前账户
        /// <remarks>因为可能抛出异常所以以方法形式提供而不以属性形式提供</remarks>
        /// </summary>
        public AccountState GetAccount()
        {
            if (!Principal.Identity.IsAuthenticated)
            {
                return AccountState.Empty;
            }
            var account = GetData<AccountState>(ConstKeys.CURRENT_ACCOUNT);
            if (account == null)
            {
                using (var conn = Db.GetConnection())
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Account entity = conn.Query<Account>("select * from [Account] where LoginName=@LoginName", new { LoginName = Principal.Identity.Name }).FirstOrDefault();
                    if (entity != null)
                    {
                        account = AccountState.Create(entity);
                    }
                    else
                    {
                        account = AccountState.Empty;
                    }
                }
                SetData(ConstKeys.CURRENT_ACCOUNT, account);
            }

            return account;
        }
        #endregion

        public AccountState GetContractor()
        {
            if (!Principal.Identity.IsAuthenticated)
            {
                return AccountState.Empty;
            }
            var account = GetAccount();
            if (!account.ContractorID.HasValue)
            {
                return AccountState.Empty;
            }
            if (account.ContractorID.Value == account.Id)
            {
                return account;
            }
            var contractor = GetData<AccountState>(ConstKeys.CURRENT_CONTRACTOR);
            if (contractor == null)
            {
                using (var conn = Db.GetConnection())
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    var entity = conn.Query<Account>("select * from [Account] where Id=@ContractorID", new { ContractorID = account.ContractorID.Value }).FirstOrDefault();
                    if (entity != null)
                    {
                        contractor = AccountState.Create(entity);
                    }
                    else
                    {
                        contractor = AccountState.Empty;
                    }
                }
                SetData(ConstKeys.CURRENT_CONTRACTOR, contractor);
            }

            return contractor;
        }

        #region GetAccountID
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Guid GetAccountID()
        {
            return GetAccount().Id;
        }
        #endregion

        #region 用户会话级数据存取接口
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetData<T>(string key)
        {
            var userSessionStorage = AppHost.GetRequiredService<IUserSessionStorage>();
            var obj = userSessionStorage.GetData(key);
            if (obj is T)
            {
                return (T)obj;
            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetData(string key, object data)
        {
            var userSessionStorage = AppHost.GetRequiredService<IUserSessionStorage>();
            userSessionStorage.SetData(key, data);
        }
        #endregion
    }
}
