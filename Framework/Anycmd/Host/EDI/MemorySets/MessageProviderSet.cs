
namespace Anycmd.Host.EDI.MemorySets
{
    using Anycmd.EDI;
    using Anycmd.Host.EDI.Handlers;
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
    public sealed class MessageProviderSet : IMessageProviderSet
    {
        private readonly Dictionary<Guid, IMessageProvider> _dic = new Dictionary<Guid, IMessageProvider>();
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
        public MessageProviderSet(NodeHost host)
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
        public bool TryGetMessageProvider(Guid providerID, out IMessageProvider provider)
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
        public IEnumerator<IMessageProvider> GetEnumerator()
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
                        var messageProviders = GetMessageProviders();
                        if (messageProviders != null)
                        {
                            foreach (var item in messageProviders)
                            {
                                var item1 = item;
                                _dic.Add(item.Id, messageProviders.Single(a => a.Id == item1.Id));
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        private IEnumerable<IMessageProvider> GetMessageProviders()
        {
            IEnumerable<IMessageProvider> r = null;
            var catalog = new DirectoryCatalog(Path.Combine(host.GetPluginBaseDirectory(PluginType.MessageProvider), "Bin"));
            var container = new CompositionContainer(catalog);
            var infoValueConverterImport = new MessageProviderImport();

            infoValueConverterImport.ImportsSatisfied += (sender, e) =>
            {
                r = e.MessageProviders;
            };
            container.ComposeParts(infoValueConverterImport);
            return r;
        }

        private class MessageProviderImport : IPartImportsSatisfiedNotification
        {
            [ImportMany(typeof(IMessageProvider), AllowRecomposition = true)]
            private IEnumerable<IMessageProvider> MessageProviders { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public event EventHandler<MessageProviderImportEventArgs> ImportsSatisfied;

            /// <summary>
            /// 
            /// </summary>
            public void OnImportsSatisfied()
            {
                if (ImportsSatisfied != null)
                {
                    ImportsSatisfied(this, new MessageProviderImportEventArgs(
                        this.MessageProviders));
                }
            }
        }

        private class MessageProviderImportEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="messageProviders"></param>
            public MessageProviderImportEventArgs(
                IEnumerable<IMessageProvider> messageProviders)
            {
                this.MessageProviders = messageProviders;
            }

            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<IMessageProvider> MessageProviders
            {
                get;
                private set;
            }
        }
    }
}