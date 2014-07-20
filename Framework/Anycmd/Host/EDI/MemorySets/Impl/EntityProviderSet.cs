﻿
namespace Anycmd.Host.EDI.MemorySets.Impl
{
    using Anycmd.EDI;
    using Handlers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class EntityProviderSet : IEntityProviderSet
    {
        private readonly Dictionary<Guid, IEntityProvider> _dic = new Dictionary<Guid, IEntityProvider>();
        private bool _initialized = false;
        private readonly object locker = new object();
        private readonly Guid _id = Guid.NewGuid();
        private readonly IAppHost host;

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// 构造并接入总线
        /// </summary>
        public EntityProviderSet(IAppHost host)
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
        /// <param name="providerID"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public bool TryGetEntityProvider(Guid providerID, out IEntityProvider provider)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dic.TryGetValue(providerID, out provider);
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

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator<IEntityProvider> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _dic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _dic.Values.GetEnumerator();
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (locker)
                {
                    if (!_initialized)
                    {
                        _dic.Clear();

                        var entityProviders = GetEntityProviders();
                        if (entityProviders != null)
                        {
                            foreach (var item in entityProviders)
                            {
                                var item1 = item;
                                _dic.Add(item.Id, entityProviders.Single(a => a.Id == item1.Id));
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        private IEnumerable<IEntityProvider> GetEntityProviders()
        {
            IEnumerable<IEntityProvider> r = null;
            var catalog = new DirectoryCatalog(Path.Combine(host.GetPluginBaseDirectory(PluginType.EntityProvider), "Bin"));
            var container = new CompositionContainer(catalog);
            var infoValueConverterImport = new EntityProviderImport();
            infoValueConverterImport.ImportsSatisfied += (sender, e) =>
            {
                r = e.EntityProviders;
            };
            container.ComposeParts(infoValueConverterImport);
            return r;
        }
        private class EntityProviderImport : IPartImportsSatisfiedNotification
        {
            /// <summary>
            /// 
            /// </summary>
            [ImportMany(typeof(IEntityProvider), AllowRecomposition = true)]
            private IEnumerable<IEntityProvider> EntityProviders { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public event EventHandler<EntityProviderImportEventArgs> ImportsSatisfied;

            /// <summary>
            /// 
            /// </summary>
            public void OnImportsSatisfied()
            {
                if (ImportsSatisfied != null)
                {
                    ImportsSatisfied(this, new EntityProviderImportEventArgs(
                        this.EntityProviders));
                }
            }
        }

        private class EntityProviderImportEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="entityProviders"></param>
            public EntityProviderImportEventArgs(
                IEnumerable<IEntityProvider> entityProviders)
            {
                this.EntityProviders = entityProviders;
            }

            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<IEntityProvider> EntityProviders
            {
                get;
                private set;
            }
        }
    }
}
