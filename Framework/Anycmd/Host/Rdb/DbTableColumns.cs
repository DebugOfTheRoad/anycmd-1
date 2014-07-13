﻿
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
    public sealed class DbTableColumns : IDbTableColumns
    {
        private readonly Dictionary<RdbDescriptor, Dictionary<DbTable, Dictionary<string, DbTableColumn>>>
            _dic = new Dictionary<RdbDescriptor, Dictionary<DbTable, Dictionary<string, DbTableColumn>>>();
        private readonly Dictionary<RdbDescriptor, Dictionary<string, DbTableColumn>> _dicByID = new Dictionary<RdbDescriptor, Dictionary<string, DbTableColumn>>();
        private bool _initialized = false;
        private readonly AppHost host;

        public DbTableColumns(AppHost host)
        {
            this.host = host;
            // TODO:接入总线
        }

        public bool TryGetDbTableColumns(RdbDescriptor database, DbTable table, out IReadOnlyDictionary<string, DbTableColumn> dbTableColumns)
        {
            if (!_initialized)
            {
                Init();
            }
            if (!_dic.ContainsKey(database))
            {
                dbTableColumns = new Dictionary<string, DbTableColumn>(StringComparer.OrdinalIgnoreCase);
                return false;
            }
            Dictionary<string, DbTableColumn> outDic;
            var r = _dic[database].TryGetValue(table, out outDic);
            dbTableColumns = outDic;
            return r;
        }

        public bool TryGetDbTableColumn(RdbDescriptor database, string tableColumnID, out DbTableColumn dbTableColumn)
        {
            if (!_initialized)
            {
                Init();
            }
            if (!_dicByID.ContainsKey(database))
            {
                dbTableColumn = null;
                return false;
            }
            return _dicByID[database].TryGetValue(tableColumnID, out dbTableColumn);
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
                            var columns = host.GetRequiredService<IAppHostBootstrap>().GetTableColumns(database);
                            _dic.Add(database, new Dictionary<DbTable, Dictionary<string, DbTableColumn>>());
                            _dicByID.Add(database, new Dictionary<string, DbTableColumn>(StringComparer.OrdinalIgnoreCase));
                            foreach (var table in host.DbTables[database].Values)
                            {
                                if (_dic[database].ContainsKey(table))
                                {
                                    // 不计划支持Schema
                                    throw new CoreException("重名的数据库表" + database.Database.CatalogName + "." + table.SchemaName + "." + table.Name);
                                }
                                _dic[database].Add(table, new Dictionary<string, DbTableColumn>(StringComparer.OrdinalIgnoreCase));
                                foreach (var tableCol in columns.Where(a => a.TableName == table.Name && a.SchemaName == table.SchemaName))
                                {
                                    _dic[database][table].Add(tableCol.Name, tableCol);
                                    _dicByID[database].Add(tableCol.Id, tableCol);
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