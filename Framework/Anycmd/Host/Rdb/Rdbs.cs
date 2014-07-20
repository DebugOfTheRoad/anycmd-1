
namespace Anycmd.Host.Rdb
{
    using Anycmd.Rdb;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// SQLServer数据库上下文
    /// </summary>
    public sealed class Rdbs : IRdbs
    {
        public static readonly IRdbs Empty = new Rdbs(AppHost.Empty);

        private readonly Dictionary<Guid, RdbDescriptor> _dicByID = new Dictionary<Guid, RdbDescriptor>();
        private bool _initialized = false;
        private readonly IAppHost host;

        public Rdbs(IAppHost host)
        {
            this.host = host;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseID"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public bool TryDb(Guid databaseID, out RdbDescriptor database)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.TryGetValue(databaseID, out database);
        }

        public bool ContainsDb(Guid databaseID)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.ContainsKey(databaseID);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
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
                        var list = host.GetRequiredService<IAppHostBootstrap>().GetAllRDatabases();
                        foreach (var item in list)
                        {
                            _dicByID.Add(item.Id, new RdbDescriptor(host, item));
                        }
                        _initialized = true;
                    }
                }
            }
        }

        public IEnumerator<RdbDescriptor> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.Values.GetEnumerator();
        }
    }
}