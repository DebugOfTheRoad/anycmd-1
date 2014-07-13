
namespace Anycmd.Host.AC.MemorySets
{
    using AC.Infra;
    using Anycmd.AC.Infra;
    using Anycmd.Bus;
    using Exceptions;
    using Extensions;
    using Host;
    using Infra.Messages;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ValueObjects;

    public sealed class DicSet : IDicSet
    {
        private readonly Dictionary<Guid, DicState> _dicByID = new Dictionary<Guid, DicState>();
        private readonly Dictionary<string, DicState> _dicByCode = new Dictionary<string, DicState>(StringComparer.OrdinalIgnoreCase);
        private bool _initialized = false;

        private readonly AppHost host;
        private DicItemSet dicItemSet;
        private readonly Guid _id = Guid.NewGuid();

        public Guid Id
        {
            get { return _id; }
        }

        #region Ctor
        public DicSet(AppHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
            dicItemSet = new DicItemSet(host);
            var messageDispatcher = host.MessageDispatcher;
            if (messageDispatcher == null)
            {
                throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
            }
            var handles = new MessageHandler(this);
            messageDispatcher.Register((IHandler<AddDicCommand>)handles);
            messageDispatcher.Register((IHandler<DicAddedEvent>)handles);
            messageDispatcher.Register((IHandler<UpdateDicCommand>)handles);
            messageDispatcher.Register((IHandler<DicUpdatedEvent>)handles);
            messageDispatcher.Register((IHandler<RemoveDicCommand>)handles);
            messageDispatcher.Register((IHandler<DicRemovedEvent>)handles);
        }
        #endregion

        public bool ContainsDic(Guid dicID)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.ContainsKey(dicID);
        }

        public bool ContainsDic(string dicCode)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByCode.ContainsKey(dicCode);
        }

        public bool TryGetDic(Guid dicID, out DicState dic)
        {
            if (!_initialized)
            {
                Init();
            }

            return _dicByID.TryGetValue(dicID, out dic);
        }

        public bool TryGetDic(string dicCode, out DicState dic)
        {
            if (dicCode == null)
            {
                throw new ArgumentNullException("dicCode");
            }
            if (!_initialized)
            {
                Init();
            }
            return _dicByCode.TryGetValue(dicCode, out dic);
        }

        public bool ContainsDicItem(Guid dicItemID)
        {
            if (!_initialized)
            {
                Init();
            }
            return dicItemSet.ContainsDicItem(dicItemID);
        }

        public bool ContainsDicItem(DicState dic, string dicItemCode)
        {
            if (!_initialized)
            {
                Init();
            }
            return dicItemSet.ContainsDicItem(dic, dicItemCode);
        }

        public IReadOnlyDictionary<string, DicItemState> GetDicItems(DicState dic)
        {
            if (!_initialized)
            {
                Init();
            }
            Dictionary<string, DicItemState> dicItems;
            if (!dicItemSet.TryGetDicItems(dic, out dicItems))
            {
                return new Dictionary<string, DicItemState>(StringComparer.OrdinalIgnoreCase);
            }
            return dicItems;
        }

        public bool TryGetDicItem(Guid dicItemID, out DicItemState dicItem)
        {
            if (!_initialized)
            {
                Init();
            }
            return dicItemSet.TryGetDicItem(dicItemID, out dicItem);
        }

        public bool TryGetDicItem(DicState dicState, string dicItemCode, out DicItemState dicItem)
        {
            if (!_initialized)
            {
                Init();
            }
            dicItem = DicItemState.Empty;
            if (dicState == DicState.Empty)
            {
                return false;
            }
            return dicItemSet.TryGetDicItem(dicState, dicItemCode, out dicItem);
        }

        public IEnumerator<DicState> GetEnumerator()
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

        #region Init
        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _dicByID.Clear();
                        _dicByCode.Clear();
                        var dics = host.GetRequiredService<IAppHostBootstrap>().GetAllDics().ToList();
                        foreach (var dic in dics)
                        {
                            if (!(dic is DicBase))
                            {
                                throw new CoreException(dic.GetType().Name + "必须继承" + typeof(DicBase).Name);
                            }
                            if (_dicByID.ContainsKey(dic.Id))
                            {
                                throw new CoreException("意外重复的字典标识" + dic.Id);
                            }
                            if (_dicByCode.ContainsKey(dic.Code))
                            {
                                throw new CoreException("意外重复的字典编码" + dic.Code);
                            }
                            var dicState = DicState.Create(dic);
                            _dicByID.Add(dic.Id, dicState);
                            _dicByCode.Add(dic.Code, dicState);
                        }
                        _initialized = true;
                    }
                }
            }
        }
        #endregion

        #region MessageHandler
        private class MessageHandler:
            IHandler<AddDicCommand>,
            IHandler<DicAddedEvent>, 
            IHandler<UpdateDicCommand>, 
            IHandler<DicUpdatedEvent>, 
            IHandler<RemoveDicCommand>, 
            IHandler<DicRemovedEvent>
        {
            private readonly DicSet set;

            public MessageHandler(DicSet set)
            {
                this.set = set;
            }

            public void Handle(AddDicCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(DicAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateDicAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IDicCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var dicRepository = host.GetRequiredService<IRepository<Dic>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (!input.Id.HasValue)
                {
                    throw new CoreException("意外的字典标识" + input.Id);
                }
                Dic entity;
                lock (this)
                {
                    if (host.DicSet.ContainsDic(input.Id.Value))
                    {
                        throw new CoreException("记录已经存在");
                    }
                    if (host.DicSet.ContainsDic(input.Code))
                    {
                        throw new ValidationException("重复的字典编码" + input.Code);
                    }

                    entity = Dic.Create(input);

                    var dicState = DicState.Create(entity);
                    if (!_dicByID.ContainsKey(entity.Id))
                    {
                        _dicByID.Add(entity.Id, dicState);
                    }
                    if (!_dicByCode.ContainsKey(entity.Code))
                    {
                        _dicByCode.Add(entity.Code, dicState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            dicRepository.Add(entity);
                            dicRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_dicByID.ContainsKey(entity.Id))
                            {
                                _dicByID.Remove(entity.Id);
                            }
                            if (_dicByCode.ContainsKey(entity.Code))
                            {
                                _dicByCode.Remove(entity.Code);
                            }
                            dicRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateDicAddedEvent(entity, input));
                }
            }

            private class PrivateDicAddedEvent : DicAddedEvent
            {
                public PrivateDicAddedEvent(DicBase source, IDicCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateDicCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(DicUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateDicUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IDicUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var dicRepository = host.GetRequiredService<IRepository<Dic>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                DicState dic;
                if (host.DicSet.TryGetDic(input.Code, out dic) && dic.Id != input.Id)
                {
                    throw new ValidationException("重复的字典编码" + input.Code);
                }
                DicState bkState;
                if (!host.DicSet.TryGetDic(input.Id, out bkState))
                {
                    throw new NotExistException("记录在内存数据集中不存在" + input.Id);
                }
                Dic entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    entity = dicRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException("记录在持久库中不存在");
                    }

                    entity.Update(input);

                    var newState = DicState.Create(entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            dicRepository.Update(entity);
                            dicRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            dicRepository.Context.Rollback();
                            throw;
                        }
                    }
                    if (!stateChanged)
                    {
                        return;
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateDicUpdatedEvent(entity, input));
                }
            }

            private void Update(DicState state)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                string oldKey = _dicByID[state.Id].Code;
                string newKey = state.Code;
                _dicByID[state.Id] = state;
                // 如果字典编码有改变
                if (!_dicByCode.ContainsKey(newKey))
                {
                    _dicByCode.Add(newKey, state);
                    _dicByCode.Remove(oldKey);
                }
                else
                {
                    _dicByCode[newKey] = state;
                }
            }

            private class PrivateDicUpdatedEvent : DicUpdatedEvent
            {
                public PrivateDicUpdatedEvent(DicBase source, IDicUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveDicCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(DicRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateDicRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid dicID, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var dicRepository = host.GetRequiredService<IRepository<Dic>>();
                var dicItemRepository = host.GetRequiredService<IRepository<DicItem>>();
                DicState bkState;
                if (!host.DicSet.TryGetDic(dicID, out bkState))
                {
                    return;
                }
                var dicItems = host.DicSet.GetDicItems(bkState);
                if (dicItems != null && dicItems.Count > 0)
                {
                    throw new ValidationException("系统字典下有字典项时不能删除");
                }
                var properties = host.EntityTypeSet.GetProperties().Where(a => a.DicID.HasValue && a.DicID.Value == dicID).ToList();
                if (properties.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var property in properties)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append("、");
                        }
                        EntityTypeState entityType;
                        host.EntityTypeSet.TryGetEntityType(property.EntityTypeID, out entityType);
                        sb.Append(entityType.Code + "." + property.Code);
                    }
                    throw new ValidationException("系统字典被实体属性关联后不能删除：" + sb.ToString());
                }
                Dic entity;
                lock (this)
                {
                    entity = dicRepository.GetByKey(dicID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (_dicByID.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new DicRemovingEvent(entity));
                        }
                        _dicByID.Remove(bkState.Id);
                    }
                    if (_dicByCode.ContainsKey(bkState.Code))
                    {
                        _dicByCode.Remove(bkState.Code);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            dicRepository.Remove(entity);
                            dicRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_dicByID.ContainsKey(bkState.Id))
                            {
                                _dicByID.Add(bkState.Id, bkState);
                            }
                            if (!_dicByCode.ContainsKey(bkState.Code))
                            {
                                _dicByCode.Add(bkState.Code, bkState);
                            }
                            dicRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateDicRemovedEvent(entity));
                }
            }

            private class PrivateDicRemovedEvent : DicRemovedEvent
            {
                public PrivateDicRemovedEvent(DicBase dic) : base(dic) { }
            }
        }
        #endregion

        // 内部类
        private sealed class DicItemSet
        {
            private readonly Dictionary<DicState, Dictionary<string, DicItemState>> _dicItemsByCode = new Dictionary<DicState, Dictionary<string, DicItemState>>();
            private readonly Dictionary<Guid, DicItemState> _dicItemByID = new Dictionary<Guid, DicItemState>();
            private bool _initialized = false;
            private readonly AppHost host;

            #region Ctor
            public DicItemSet(AppHost host)
            {
                if (host == null)
                {
                    throw new ArgumentNullException("host");
                }
                this.host = host;
                var messageDispatcher = host.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
                }
                var handles = new MessageHandler(this);
                messageDispatcher.Register((IHandler<AddDicItemCommand>)handles);
                messageDispatcher.Register((IHandler<DicItemAddedEvent>)handles);
                messageDispatcher.Register((IHandler<UpdateDicItemCommand>)handles);
                messageDispatcher.Register((IHandler<DicItemUpdatedEvent>)handles);
                messageDispatcher.Register((IHandler<RemoveDicItemCommand>)handles);
                messageDispatcher.Register((IHandler<DicItemRemovedEvent>)handles);
                messageDispatcher.Register((IHandler<DicUpdatedEvent>)handles);
                messageDispatcher.Register((IHandler<DicRemovedEvent>)handles);
            }
            #endregion

            public bool ContainsDicItem(Guid dicItemID)
            {
                if (!_initialized)
                {
                    Init();
                }
                return _dicItemByID.ContainsKey(dicItemID);
            }

            public bool ContainsDicItem(DicState dic, string dicItemCode)
            {
                if (!_initialized)
                {
                    Init();
                }
                return _dicItemsByCode.ContainsKey(dic) && _dicItemsByCode[dic].ContainsKey(dicItemCode);
            }

            public bool TryGetDicItems(DicState dicState, out Dictionary<string, DicItemState> dicItems)
            {
                if (!_initialized)
                {
                    Init();
                }
                return _dicItemsByCode.TryGetValue(dicState, out dicItems);
            }

            public bool TryGetDicItem(Guid dicItemID, out DicItemState dicItem)
            {
                if (!_initialized)
                {
                    Init();
                }
                return _dicItemByID.TryGetValue(dicItemID, out dicItem);
            }

            public bool TryGetDicItem(DicState dicState, string dicItemCode, out DicItemState dicItem)
            {
                if (!_initialized)
                {
                    Init();
                }
                if (!_dicItemsByCode.ContainsKey(dicState))
                {
                    dicItem = DicItemState.Empty;
                    return false;
                }
                return _dicItemsByCode[dicState].TryGetValue(dicItemCode, out dicItem);
            }

            #region Init
            private void Init()
            {
                if (!_initialized)
                {
                    lock (this)
                    {
                        if (!_initialized)
                        {
                            _dicItemsByCode.Clear();
                            _dicItemByID.Clear();
                            var dicItems = host.GetRequiredService<IAppHostBootstrap>().GetAllDicItems().OrderBy(di => di.SortCode);
                            foreach (var dicItem in dicItems)
                            {
                                if (!(dicItem is DicItemBase))
                                {
                                    throw new CoreException(dicItem.GetType().Name + "必须继承" + typeof(DicItemBase).Name);
                                }
                                DicState dic;
                                if (!host.DicSet.TryGetDic(dicItem.DicID, out dic))
                                {
                                    throw new CoreException("意外的字典项字典标识" + dicItem.DicID);
                                }
                                if (_dicItemByID.ContainsKey(dicItem.Id))
                                {
                                    throw new CoreException("意外重复的字典项标识" + dicItem.Id);
                                }
                                Dictionary<string, DicItemState> dicItemDic;
                                if (!_dicItemsByCode.TryGetValue(dic, out dicItemDic))
                                {
                                    _dicItemsByCode.Add(dic, dicItemDic = new Dictionary<string, DicItemState>(StringComparer.OrdinalIgnoreCase));
                                }
                                if (dicItemDic.ContainsKey(dicItem.Code))
                                {
                                    throw new CoreException("意外重复的字典项编码" + dicItem.Code);
                                }
                                var dicItemState = DicItemState.Create(host, dicItem);
                                _dicItemsByCode[dic].Add(dicItem.Code, dicItemState);
                                _dicItemByID.Add(dicItem.Id, dicItemState);
                            }
                            _initialized = true;
                        }
                    }
                }
            }
            #endregion

            #region MessageHandler
            private class MessageHandler:
                IHandler<AddDicItemCommand>,
                IHandler<DicItemAddedEvent>,
                IHandler<UpdateDicItemCommand>,
                IHandler<DicItemUpdatedEvent>,
                IHandler<RemoveDicItemCommand>,
                IHandler<DicItemRemovedEvent>,
                IHandler<DicUpdatedEvent>,
                IHandler<DicRemovedEvent>
            {
                private readonly DicItemSet set;

                public MessageHandler(DicItemSet set)
                {
                    this.set = set;
                }

                public void Handle(DicUpdatedEvent message)
                {
                    var host = set.host;
                    var _dicItemsByCode = set._dicItemsByCode;
                    var _dicItemByID = set._dicItemByID;
                    DicState newKey;
                    if (!host.DicSet.TryGetDic(message.Source.Id, out newKey))
                    {
                        throw new CoreException("意外的字典标识" + message.Source.Id);
                    }
                    var oldKey = _dicItemsByCode.Keys.FirstOrDefault(a => a.Id == newKey.Id);
                    if (oldKey != null && !_dicItemsByCode.ContainsKey(newKey))
                    {
                        _dicItemsByCode.Add(newKey, _dicItemsByCode[oldKey]);
                        _dicItemsByCode.Remove(oldKey);
                    }
                }

                public void Handle(DicRemovedEvent message)
                {
                    var host = set.host;
                    var _dicItemsByCode = set._dicItemsByCode;
                    var _dicItemByID = set._dicItemByID;
                    var dicState = _dicItemsByCode.Keys.FirstOrDefault(a => a.Id == message.Source.Id);
                    if (dicState != null)
                    {
                        _dicItemsByCode.Remove(dicState);
                    }
                }

                public void Handle(AddDicItemCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(DicItemAddedEvent message)
                {
                    if (message.GetType() == typeof(PrivateDicItemAddedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(IDicItemCreateInput input, bool isCommand)
                {
                    var host = set.host;
                    var _dicItemsByCode = set._dicItemsByCode;
                    var _dicItemByID = set._dicItemByID;
                    var dicItemRepository = host.GetRequiredService<IRepository<DicItem>>();
                    if (string.IsNullOrEmpty(input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    DicItem entity;
                    lock (this)
                    {
                        DicState dicState;
                        if (!host.DicSet.TryGetDic(input.DicID, out dicState))
                        {
                            throw new ValidationException("意外的字典项字典标识" + input.DicID);
                        }
                        if (host.DicSet.ContainsDicItem(dicState, input.Code))
                        {
                            throw new ValidationException("重复的字典项编码" + input.Code);
                        }
                        if (!input.Id.HasValue)
                        {
                            throw new ValidationException("标识为空");
                        }
                        if (host.DicSet.ContainsDicItem(input.Id.Value))
                        {
                            throw new CoreException("重复的字典项标识" + input.Id);
                        }

                        entity = DicItem.Create(input);

                        var dicItemState = DicItemState.Create(host, entity);
                        if (!_dicItemByID.ContainsKey(dicItemState.Id))
                        {
                            _dicItemByID.Add(dicItemState.Id, dicItemState);
                        }
                        Dictionary<string, DicItemState> dicItemDic;
                        if (!_dicItemsByCode.TryGetValue(dicState, out dicItemDic))
                        {
                            _dicItemsByCode.Add(dicState, dicItemDic = new Dictionary<string, DicItemState>(StringComparer.OrdinalIgnoreCase));
                        }
                        if (!dicItemDic.ContainsKey(dicItemState.Code))
                        {
                            dicItemDic.Add(dicItemState.Code, dicItemState);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                dicItemRepository.Add(entity);
                                dicItemRepository.Context.Commit();
                            }
                            catch
                            {
                                if (_dicItemByID.ContainsKey(entity.Id))
                                {
                                    _dicItemByID.Remove(entity.Id);
                                }
                                if (_dicItemsByCode.TryGetValue(dicState, out dicItemDic) && dicItemDic.ContainsKey(entity.Code))
                                {
                                    dicItemDic.Remove(entity.Code);
                                }
                                dicItemRepository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivateDicItemAddedEvent(entity, input));
                    }
                }

                private class PrivateDicItemAddedEvent : DicItemAddedEvent
                {
                    public PrivateDicItemAddedEvent(DicItemBase source, IDicItemCreateInput input)
                        : base(source, input)
                    {

                    }
                }

                public void Handle(UpdateDicItemCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(DicItemUpdatedEvent message)
                {
                    if (message.GetType() == typeof(PrivateDicItemUpdatedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(IDicItemUpdateInput input, bool isCommand)
                {
                    var host = set.host;
                    var _dicItemsByCode = set._dicItemsByCode;
                    var _dicItemByID = set._dicItemByID;
                    var dicItemRepository = host.GetRequiredService<IRepository<DicItem>>();
                    if (string.IsNullOrEmpty(input.Code))
                    {
                        throw new ValidationException("编码不能为空");
                    }
                    DicItemState bkState;
                    if (!host.DicSet.TryGetDicItem(input.Id, out bkState))
                    {
                        throw new NotExistException();
                    }
                    DicItem entity;
                    bool stateChanged = false;
                    lock (bkState)
                    {
                        DicState dicState;
                        if (!host.DicSet.TryGetDic(bkState.DicID, out dicState))
                        {
                            throw new CoreException("意外的字典项字典标识" + bkState.DicID);
                        }
                        DicItemState dicItemState;
                        if (host.DicSet.TryGetDicItem(dicState, input.Code, out dicItemState) && dicItemState.Id != input.Id)
                        {
                            throw new ValidationException("重复的字典项编码" + input.Code);
                        }
                        entity = dicItemRepository.GetByKey(input.Id);
                        if (entity == null)
                        {
                            throw new NotExistException();
                        }

                        entity.Update(input);

                        var newState = DicItemState.Create(host, entity);
                        stateChanged = newState != bkState;
                        if (stateChanged)
                        {
                            Update(newState);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                dicItemRepository.Update(entity);
                                dicItemRepository.Context.Commit();
                            }
                            catch
                            {
                                if (stateChanged)
                                {
                                    Update(bkState);
                                }
                                dicItemRepository.Context.Rollback();
                                throw;
                            }
                        }
                        if (!stateChanged)
                        {
                            return;
                        }
                    }
                    if (isCommand && stateChanged)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivateDicItemUpdatedEvent(entity, input));
                    }
                }

                private void Update(DicItemState state)
                {
                    var host = set.host;
                    var _dicItemsByCode = set._dicItemsByCode;
                    var _dicItemByID = set._dicItemByID;
                    DicState dicState;
                    if (!host.DicSet.TryGetDic(_dicItemByID[state.Id].DicID, out dicState))
                    {
                        throw new ValidationException("意外的字典项字典标识");
                    }
                    string oldKey = _dicItemByID[state.Id].Code;
                    string newKey = state.Code;
                    _dicItemByID[state.Id] = state;
                    // 如果字典项编码有改变
                    if (!_dicItemsByCode[dicState].ContainsKey(newKey))
                    {
                        _dicItemsByCode[dicState].Remove(oldKey);
                        _dicItemsByCode[dicState].Add(newKey, state);
                    }
                    else
                    {
                        _dicItemsByCode[dicState][newKey] = state;
                    }
                }

                private class PrivateDicItemUpdatedEvent : DicItemUpdatedEvent
                {
                    public PrivateDicItemUpdatedEvent(DicItemBase source, IDicItemUpdateInput input)
                        : base(source, input)
                    {

                    }
                }

                public void Handle(RemoveDicItemCommand message)
                {
                    this.Handle(message.EntityID, true);
                }

                public void Handle(DicItemRemovedEvent message)
                {
                    if (message.GetType() == typeof(PrivateDicItemRemovedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Source.Id, false);
                }

                private void Handle(Guid dicItemID, bool isCommand)
                {
                    var host = set.host;
                    var _dicItemsByCode = set._dicItemsByCode;
                    var _dicItemByID = set._dicItemByID;
                    var dicItemRepository = host.GetRequiredService<IRepository<DicItem>>();
                    DicItemState bkState;
                    if (!host.DicSet.TryGetDicItem(dicItemID, out bkState))
                    {
                        return;
                    }
                    DicItem entity;
                    lock (bkState)
                    {
                        entity = dicItemRepository.GetByKey(dicItemID);
                        if (entity == null)
                        {
                            return;
                        }
                        if (_dicItemByID.ContainsKey(bkState.Id))
                        {
                            DicState dic;
                            if (!host.DicSet.TryGetDic(bkState.DicID, out dic))
                            {
                                throw new CoreException("意外的字典标识" + bkState.DicID);
                            }
                            if (_dicItemsByCode.ContainsKey(dic) && _dicItemsByCode[dic].ContainsKey(bkState.Code))
                            {
                                _dicItemsByCode[dic].Remove(bkState.Code);
                            }
                            _dicItemByID.Remove(bkState.Id);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                dicItemRepository.Remove(entity);
                                dicItemRepository.Context.Commit();
                            }
                            catch
                            {
                                if (!_dicItemByID.ContainsKey(bkState.Id))
                                {
                                    DicState dic;
                                    if (!host.DicSet.TryGetDic(bkState.DicID, out dic))
                                    {
                                        throw new CoreException("意外的字典标识" + bkState.DicID);
                                    }
                                    Dictionary<string, DicItemState> dicItemDic;
                                    if (!_dicItemsByCode.TryGetValue(dic, out dicItemDic))
                                    {
                                        _dicItemsByCode.Add(dic, dicItemDic = new Dictionary<string, DicItemState>(StringComparer.OrdinalIgnoreCase));
                                    }
                                    if (!dicItemDic.ContainsKey(bkState.Code))
                                    {
                                        dicItemDic.Add(bkState.Code, bkState);
                                    }
                                    _dicItemByID.Add(bkState.Id, bkState);
                                }
                                dicItemRepository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivateDicItemRemovedEvent(entity));
                    }
                }

                private class PrivateDicItemRemovedEvent : DicItemRemovedEvent
                {
                    public PrivateDicItemRemovedEvent(DicItemBase source)
                        : base(source)
                    {

                    }
                }
            }
            #endregion
        }
    }
}
