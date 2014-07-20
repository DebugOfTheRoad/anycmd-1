
namespace Anycmd.Host.Rdb
{
    using Anycmd.Rdb;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 数据库表上下文
    /// </summary>
    public sealed class DbTables : IDbTables
    {
        public static readonly IDbTables Empty = new DbTables(AppHost.Empty);

        private readonly Dictionary<RdbDescriptor, Dictionary<string, DbTable>> _dicByID = new Dictionary<RdbDescriptor, Dictionary<string, DbTable>>();
        private bool _initialized = false;
        private readonly IAppHost host;

        public DbTables(IAppHost host)
        {
            this.host = host;
            // TODO:接入总线
        }

        /// <summary>
        /// 根据数据库索引该库的全部表
        /// </summary>
        /// <param name="db">数据库模型实例</param>
        /// <returns></returns>
        public IReadOnlyDictionary<string, DbTable> this[RdbDescriptor db]
        {
            get
            {
                if (!_initialized)
                {
                    Init();
                }
                if (!_dicByID.ContainsKey(db))
                {
                    return new Dictionary<string, DbTable>(StringComparer.OrdinalIgnoreCase);
                }
                return _dicByID[db];
            }
        }

        public bool TryGetDbTable(RdbDescriptor db, string dbTableID, out DbTable dbTable)
        {
            if (!_initialized)
            {
                Init();
            }
            if (!_dicByID.ContainsKey(db))
            {
                dbTable = null;
                return false;
            }
            return _dicByID[db].TryGetValue(dbTableID, out dbTable);
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _dicByID.Clear();
                        foreach (var db in host.Rdbs)
                        {
                            _dicByID.Add(db, new Dictionary<string, DbTable>(StringComparer.OrdinalIgnoreCase));
                            var tables = host.GetRequiredService<IAppHostBootstrap>().GetDbTables(db);
                            foreach (var item in tables)
                            {
                                _dicByID[db].Add(item.Id, item);
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }
    }
}