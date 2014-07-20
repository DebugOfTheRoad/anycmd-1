
namespace Anycmd.Ef
{
    using Util;
    using Exceptions;
    using Host;
    using Model;
    using Query;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public abstract class QueryBase : IQuery
    {
        private IEfFilterStringBuilder _filterStringBuilder;

        private readonly string efDbContextName;
        private readonly IAppHost host;

        public QueryBase(IAppHost host, string efDbContextName)
        {
            this.host = host;
            this.efDbContextName = efDbContextName;
        }

        /// <summary>
        /// 
        /// </summary>
        protected DbContext DbContext
        {
            get
            {
                var repositoryContext = EfContext.Storage.GetRepositoryContext(this.efDbContextName);
                if (repositoryContext == null)
                {
                    repositoryContext = new EfRepositoryContext(host, this.efDbContextName);
                    EfContext.Storage.SetRepositoryContext(repositoryContext);
                }
                return repositoryContext.DbContext;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity">实体模型类型参数</typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return DbContext.Set<TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected IEfFilterStringBuilder FilterStringBuilder
        {
            get
            {
                if (_filterStringBuilder == null)
                {
                    _filterStringBuilder = host.GetRequiredService<IEfFilterStringBuilder>();
                }
                return _filterStringBuilder;
            }
        }

        public DicReader Get(string tableOrViewName, Guid id)
        {
            var sql = "select * from " + tableOrViewName + " as a where Id=@Id";
            using (var conn = DbContext.Database.Connection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqlParameter("Id", id));
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                {
                    if (reader.Read())
                    {
                        var dic = new DicReader(host);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dic.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        return dic;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filters"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        protected IList<T> GetPlist<T>(Func<ObjectFilter> filterCallback, PagingInput paging) where T : class, IEntity
        {
            paging.Valid();
            string setName = typeof(T).Name + "s";
            ObjectFilter filter = ObjectFilter.Empty;
            if (filterCallback != null)
            {
                filter = filterCallback();
            }
            var queryString =
@"select value a from " + setName + " as a " + filter.FilterString + " order by a." + paging.sortField + " " + paging.sortOrder;
            var countQS =
@"select value a from " + setName + " as a " + filter.FilterString;

            IQueryable<T> countQuery;
            IQueryable<T> query;
            if (filter.Parameters != null)
            {
                countQuery = this.DbContext.CreateQuery<T>(countQS, filter.Parameters);
                query = this.DbContext.CreateQuery<T>(queryString, filter.Parameters)
                    .Skip(paging.SkipCount).Take(paging.pageSize);
            }
            else
            {
                countQuery = this.DbContext.CreateQuery<T>(countQS);
                query = this.DbContext.CreateQuery<T>(queryString)
                    .Skip(paging.SkipCount).Take(paging.pageSize);
            }

            paging.total = countQuery.Count();

            return query.ToList<T>();
        }

        public List<DicReader> GetPlist(string tableOrViewName, Func<SqlFilter> filterCallback, PagingInput paging)
        {
            SqlFilter filter = SqlFilter.Empty;
            if (filterCallback != null)
            {
                filter = filterCallback();
            }
            string sql =
@"SELECT TOP " + paging.pageSize + " * FROM (SELECT ROW_NUMBER() OVER(ORDER BY " + paging.sortField + " " + paging.sortOrder + ") AS RowNumber,* FROM " + tableOrViewName + " as a " + filter.FilterString + " ) a WHERE a.RowNumber > " + paging.pageIndex * paging.pageSize;
            string countSql =
@"SELECT count(1) FROM " + tableOrViewName + " as a " + filter.FilterString;
            List<DicReader> list = new List<DicReader>();
            using (var conn = DbContext.Database.Connection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                if (filter.Parameters != null)
                {
                    foreach (var item in filter.Parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dic = new DicReader(host);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dic.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        list.Add(dic);
                    }
                    reader.Close();
                    cmd.CommandText = countSql;
                    paging.total = (int)cmd.ExecuteScalar();
                    conn.Close();
                }
                return list;
            }
        }

        public List<DicReader> GetPlist(EntityTypeState entityType, Func<SqlFilter> filterCallback, PagingInput paging)
        {
            if (string.IsNullOrEmpty(entityType.TableName))
            {
                throw new CoreException(entityType.Name + "未配置对应的数据库表");
            }
            return this.GetPlist(string.Format("[{0}]", entityType.TableName), filterCallback, paging);
        }
    }
}
