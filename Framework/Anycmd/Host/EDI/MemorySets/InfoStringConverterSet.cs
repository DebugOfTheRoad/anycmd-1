﻿
namespace Anycmd.Host.EDI.MemorySets
{
    using Anycmd.EDI;
    using Exceptions;
    using Info;
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
    public sealed class InfoStringConverterSet : IInfoStringConverterSet
    {
        private readonly Dictionary<string, IInfoStringConverter>
            _dic = new Dictionary<string, IInfoStringConverter>(StringComparer.OrdinalIgnoreCase);
        private bool _initialized = false;
        private readonly object locker = new object();
        private readonly Guid _id = Guid.NewGuid();
        private readonly NodeHost host;

        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// 构造并接入总线
        /// </summary>
        public InfoStringConverterSet(NodeHost host)
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
        /// <param name="infoFormat"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public bool TryGetInfoStringConverter(string infoFormat, out IInfoStringConverter converter)
        {
            if (!_initialized)
            {
                Init();
            }
            if (infoFormat == null)
            {
                converter = null;
                return false;
            }
            if (!_dic.ContainsKey(infoFormat))
            {
                converter = null;
                return false;
            }
            return _dic.TryGetValue(infoFormat, out converter);
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
        public IEnumerator<IInfoStringConverter> GetEnumerator()
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

                        var convertors = GetInfoStringConverters();
                        if (convertors != null)
                        {
                            foreach (var item in convertors)
                            {
                                if (_dic.ContainsKey(item.InfoFormat))
                                {
                                    throw new CoreException("信息格式转化器暂不支持优先级策略，每种格式只允许映射一个转化器");
                                }
                                var item1 = item;
                                _dic.Add(item.InfoFormat, convertors.Single(a => a.Id == item1.Id));
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        private IEnumerable<IInfoStringConverter> GetInfoStringConverters()
        {
            IEnumerable<IInfoStringConverter> r = null;
            var catalog = new DirectoryCatalog(Path.Combine(host.GetPluginBaseDirectory(PluginType.InfoStringConverter), "Bin"));
            var container = new CompositionContainer(catalog);
            var infoValueConverterImport = new InfoStringConverterImport();
            infoValueConverterImport.ImportsSatisfied += (sender, e) =>
            {
                r = e.InfoStringConverters;
            };
            container.ComposeParts(infoValueConverterImport);

            return r;
        }

        private class InfoStringConverterImport : IPartImportsSatisfiedNotification
        {
            /// <summary>
            /// 
            /// </summary>
            [ImportMany(typeof(IInfoStringConverter), AllowRecomposition = true)]
            private IEnumerable<IInfoStringConverter> InfoStringConverters { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public event EventHandler<InfoStringConverterImportEventArgs> ImportsSatisfied;

            /// <summary>
            /// 
            /// </summary>
            public void OnImportsSatisfied()
            {
                if (ImportsSatisfied != null)
                {
                    ImportsSatisfied(this, new InfoStringConverterImportEventArgs(
                        this.InfoStringConverters));
                }
            }
        }

        private class InfoStringConverterImportEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="infoValueConverters"></param>
            public InfoStringConverterImportEventArgs(
                IEnumerable<IInfoStringConverter> infoValueConverters)
            {
                this.InfoStringConverters = infoValueConverters;
            }

            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<IInfoStringConverter> InfoStringConverters
            {
                get;
                private set;
            }
        }
    }
}