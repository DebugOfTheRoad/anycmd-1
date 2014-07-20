
namespace Anycmd.Host.EDI.MemorySets.Impl
{
    using Anycmd.EDI;
    using Exceptions;
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
    public sealed class MessageTransferSet : IMessageTransferSet
    {
        private readonly Dictionary<Guid, IMessageTransfer> _dic = new Dictionary<Guid, IMessageTransfer>();
        private bool _initialized = false;
        private object locker = new object();
        private readonly Guid _id = Guid.NewGuid();
        private readonly IAppHost host;

        public Guid Id
        {
            get { return _id; }
        }

        public MessageTransferSet(IAppHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
        }

        /// <summary>
        /// 根据发送策略名索引发送策略
        /// </summary>
        /// <param name="transferID">发送策略名</param>
        /// <exception cref="CoreException">当给定名称的发送策略不存在时引发</exception>
        /// <returns></returns>
        /// <exception cref="CoreException">当转移器标识非法时抛出</exception>
        public IMessageTransfer this[Guid transferID]
        {
            get
            {
                if (!_initialized)
                {
                    Init();
                }
                if (!_dic.ContainsKey(transferID))
                {
                    throw new CoreException("意外的转移器标识");
                }

                return _dic[transferID];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferID"></param>
        /// <param name="sendStrategy"></param>
        /// <returns></returns>
        public bool TryGetTransfer(Guid transferID, out IMessageTransfer sendStrategy)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dic.TryGetValue(transferID, out sendStrategy);
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
                lock (locker)
                {
                    if (!_initialized)
                    {
                        foreach (var item in _dic.Values)
                        {
                            item.Dispose();
                        }
                        _dic.Clear();

                        var transfers = GetTransfers();
                        if (transfers != null)
                        {
                            foreach (var item in transfers)
                            {
                                var item1 = item;
                                _dic.Add(item.Id, transfers.Single(a => a.Id == item1.Id));
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator<IMessageTransfer> GetEnumerator()
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

        private IEnumerable<IMessageTransfer> GetTransfers()
        {
            IEnumerable<IMessageTransfer> r = null;
            var catalog = new DirectoryCatalog(Path.Combine(host.GetPluginBaseDirectory(PluginType.MessageTransfer), "Bin"));
            var container = new CompositionContainer(catalog);
            var infoValueConverterImport = new MessageTransferImport();
            infoValueConverterImport.ImportsSatisfied += (sender, e) =>
            {
                r = e.Transfers;
            };
            container.ComposeParts(infoValueConverterImport);
            return r;
        }

        private class MessageTransferImport : IPartImportsSatisfiedNotification
        {
            /// <summary>
            /// 
            /// </summary>
            [ImportMany(typeof(IMessageTransfer), AllowRecomposition = true)]
            private IEnumerable<IMessageTransfer> Transfers { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public event EventHandler<MessageTransferImportEventArgs> ImportsSatisfied;

            /// <summary>
            /// 
            /// </summary>
            public void OnImportsSatisfied()
            {
                if (ImportsSatisfied != null)
                {
                    ImportsSatisfied(this, new MessageTransferImportEventArgs(
                        this.Transfers));
                }
            }
        }

        private class MessageTransferImportEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="transfers"></param>
            public MessageTransferImportEventArgs(IEnumerable<IMessageTransfer> transfers)
            {
                this.Transfers = transfers;
            }

            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<IMessageTransfer> Transfers
            {
                get;
                private set;
            }
        }
    }
}