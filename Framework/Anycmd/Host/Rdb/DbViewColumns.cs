
namespace Anycmd.Host.Rdb
{
    using Anycmd.Rdb;
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 表列上下文
    /// </summary>
    public sealed class DbViewColumns : IDbViewColumns
    {
        public static readonly IDbViewColumns Empty = new DbViewColumns(AppHost.Empty);

        private readonly Dictionary<RdbDescriptor, Dictionary<DbView, Dictionary<string, DbViewColumn>>>
            _dic = new Dictionary<RdbDescriptor, Dictionary<DbView, Dictionary<string, DbViewColumn>>>();
        private readonly Dictionary<RdbDescriptor, Dictionary<string, DbViewColumn>> _dicByID = new Dictionary<RdbDescriptor, Dictionary<string, DbViewColumn>>();
        private bool _initialized = false;
        private readonly IAppHost host;

        public DbViewColumns(IAppHost host)
        {
            this.host = host;
            // TODO:接入总线
        }

        public bool TryGetDbViewColumns(RdbDescriptor database, DbView view, out IReadOnlyDictionary<string, DbViewColumn> dbViewColumns)
        {
            if (!_initialized)
            {
                Init();
            }
            if (!_dic.ContainsKey(database))
            {
                dbViewColumns = new Dictionary<string, DbViewColumn>(StringComparer.OrdinalIgnoreCase);
                return false;
            }
            Dictionary<string, DbViewColumn> outDic;
            var r = _dic[database].TryGetValue(view, out outDic);
            dbViewColumns = outDic;
            return r;
        }

        public bool TryGetDbViewColumn(RdbDescriptor database, string viewColumnID, out DbViewColumn dbViewColumn)
        {
            if (!_initialized)
            {
                Init();
            }
            if (!_dicByID.ContainsKey(database))
            {
                dbViewColumn = null;
                return false;
            }
            return _dicByID[database].TryGetValue(viewColumnID, out dbViewColumn);
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _dic.Clear();
                        _dicByID.Clear();
                        foreach (var database in host.Rdbs)
                        {
                            _dic.Add(database, new Dictionary<DbView, Dictionary<string, DbViewColumn>>());
                            _dicByID.Add(database, new Dictionary<string, DbViewColumn>(StringComparer.OrdinalIgnoreCase));
                            var columns = host.GetRequiredService<IAppHostBootstrap>().GetViewColumns(database);
                            foreach (var view in host.DbViews[database].Values)
                            {
                                if (_dic[database].ContainsKey(view))
                                {
                                    // TODO:暂不支持Schema
                                    throw new CoreException("重名的数据库视图" + database.Database.CatalogName + "." + view.SchemaName + "." + view.Name);
                                }
                                _dic[database].Add(view, new Dictionary<string, DbViewColumn>(StringComparer.OrdinalIgnoreCase));
                                foreach (var viewCol in columns.Where(a => a.ViewName == view.Name && a.SchemaName == view.SchemaName))
                                {
                                    _dic[database][view].Add(viewCol.Name, viewCol);
                                    _dicByID[database].Add(viewCol.Id, viewCol);
                                }
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }
    }
}