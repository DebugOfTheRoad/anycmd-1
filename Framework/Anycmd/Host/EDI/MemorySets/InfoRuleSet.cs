
namespace Anycmd.Host.EDI.MemorySets
{
    using Anycmd.EDI;
    using Anycmd.Repositories;
    using Entities;
    using Extensions;
    using Info;
    using Model;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using Transactions;

    /// <summary>
    /// 
    /// </summary>
    public sealed class InfoRuleSet : IInfoRuleSet
    {
        private readonly Dictionary<Guid, InfoRuleState> _infoRuleEntities = new Dictionary<Guid, InfoRuleState>();

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
        public InfoRuleSet(NodeHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
            var messageDispatcher = host.AppHost.MessageDispatcher;
            if (messageDispatcher == null)
            {
                throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.AppHost.Name));
            }
        }

        public bool TryGetInfoRule(Guid id, out InfoRuleState infoRule)
        {
            if (!_initialized)
            {
                Init();
            }
            return _infoRuleEntities.TryGetValue(id, out infoRule);
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
        public IEnumerator<InfoRuleState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _infoRuleEntities.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _infoRuleEntities.GetEnumerator();
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (locker)
                {
                    if (!_initialized)
                    {
                        foreach (var item in _infoRuleEntities)
                        {
                            item.Value.InfoRule.Dispose();
                        }
                        _infoRuleEntities.Clear();

                        var InfoRules = GetInfoRules();
                        if (InfoRules != null)
                        {
                            // 填充信息项验证器库
                            foreach (var infoRule in InfoRules.Where(a => a.IsEnabled == 1))
                            {
                                _infoRuleEntities.Add(infoRule.Id, infoRule);
                            }
                        }

                        _initialized = true;
                    }
                }
            }
        }

        private IEnumerable<InfoRuleState> GetInfoRules()
        {
            IEnumerable<IInfoRule> validatorPlugs = null;
            var catalog = new DirectoryCatalog(Path.Combine(host.GetPluginBaseDirectory(PluginType.InfoConstraint), "Bin"));
            var container = new CompositionContainer(catalog);
            InfoRuleImport InfoRuleImport = new InfoRuleImport();
            InfoRuleImport.ImportsSatisfied += (sender, e) =>
            {
                validatorPlugs = e.InfoRules;
            };
            container.ComposeParts(InfoRuleImport);

            var infoRuleRepository = host.GetRequiredService<IRepository<InfoRule>>();
            var oldEntities = infoRuleRepository.FindAll().ToList();
            var deleteList = new List<InfoRule>();
            var newList = new List<InfoRule>();
            IList<InfoRuleState> infoRules = new List<InfoRuleState>();
            List<InfoRule> entities = new List<InfoRule>();
            bool saveChanges = false;
            foreach (var item in validatorPlugs)
            {
                var entity = new InfoRule
                {
                    Id = item.Id,
                    IsEnabled = 0
                };
                var oldEntity = oldEntities.FirstOrDefault(a => a.Id == item.Id);
                if (oldEntity != null)
                {
                    ((IEntityBase)entity).CreateBy = oldEntity.CreateBy;
                    ((IEntityBase)entity).CreateOn = oldEntity.CreateOn;
                    ((IEntityBase)entity).CreateUserID = oldEntity.CreateUserID;
                    entity.IsEnabled = oldEntity.IsEnabled;
                    ((IEntityBase)entity).ModifiedBy = oldEntity.ModifiedBy;
                    ((IEntityBase)entity).ModifiedOn = oldEntity.ModifiedOn;
                    ((IEntityBase)entity).ModifiedUserID = oldEntity.ModifiedUserID;
                }
                entities.Add(entity);
                infoRules.Add(InfoRuleState.Create(entity, item));
            }
            // 待添加的新的
            foreach (var item in entities)
            {
                var item1 = item;
                var old = oldEntities.FirstOrDefault(a => a.Id == item1.Id);
                if (old == null)
                {
                    newList.Add(item);
                }
            }
            // 待移除的旧的
            foreach (var oldEntity in oldEntities)
            {
                var item2 = oldEntity;
                var entity = entities.FirstOrDefault(a => a.Id == item2.Id);
                if (entity == null)
                {
                    deleteList.Add(oldEntity);
                }
            }
            if (newList.Count > 0)
            {
                saveChanges = true;
                foreach (var item in newList)
                {
                    infoRuleRepository.Context.RegisterNew(item);
                }
            }
            if (deleteList.Count > 0)
            {
                saveChanges = true;
                foreach (var item in deleteList)
                {
                    infoRuleRepository.Context.RegisterDeleted(item);
                }
            }
            if (saveChanges)
            {
                using (var coordinator = TransactionCoordinatorFactory.Create(infoRuleRepository.Context, host.AppHost.EventBus))
                {
                    coordinator.Commit();
                }
            }

            return infoRules;
        }

        private class InfoRuleImport : IPartImportsSatisfiedNotification
        {
            [ImportMany(typeof(IInfoRule), AllowRecomposition = true)]
            private IEnumerable<IInfoRule> InfoRules { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public event EventHandler<InfoRuleImportEventArgs> ImportsSatisfied;

            /// <summary>
            /// 在信息验证器部件导入并可安全使用时调用。
            /// </summary>
            public void OnImportsSatisfied()
            {
                if (ImportsSatisfied != null)
                {
                    ImportsSatisfied(this, new InfoRuleImportEventArgs(
                        this.InfoRules));
                }
            }
        }

        private class InfoRuleImportEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="InfoRules"></param>
            public InfoRuleImportEventArgs(
                IEnumerable<IInfoRule> InfoRules)
            {
                this.InfoRules = InfoRules;
            }

            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<IInfoRule> InfoRules
            {
                get;
                private set;
            }
        }
    }
}
