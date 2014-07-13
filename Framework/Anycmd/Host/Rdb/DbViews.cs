
namespace Anycmd.Host.Rdb
{
    using Anycmd.Rdb;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 数据库视图上下文
    /// </summary>
    public sealed class DbViews : IDbViews
    {
        private readonly Dictionary<RdbDescriptor, Dictionary<string, DbView>> _dicByID = new Dictionary<RdbDescriptor, Dictionary<string, DbView>>();
        private bool _initialized = false;
        private readonly AppHost host;

        public DbViews(AppHost host)
        {
            this.host = host;
        }

        /// <summary>
        /// 根据数据库索引该库的所有视图
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<string, DbView> this[RdbDescriptor db]
        {
            get
            {
                if (!_initialized)
                {
                    Init();
                }
                if (!_dicByID.ContainsKey(db))
                {
                    return new Dictionary<string, DbView>(StringComparer.OrdinalIgnoreCase);
                }
                return _dicByID[db];
            }
        }

        public bool TryGetDbView(RdbDescriptor db, string dbViewID, out DbView dbView)
        {
            if (!_initialized)
            {
                Init();
            }
            if (!_dicByID.ContainsKey(db))
            {
                dbView = null;
                return false;
            }
            return _dicByID[db].TryGetValue(dbViewID, out dbView);
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
                            _dicByID.Add(db, new Dictionary<string, DbView>(StringComparer.OrdinalIgnoreCase));
                            var views = host.GetRequiredService<IAppHostBootstrap>().GetDbViews(db);
                            foreach (var item in views)
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