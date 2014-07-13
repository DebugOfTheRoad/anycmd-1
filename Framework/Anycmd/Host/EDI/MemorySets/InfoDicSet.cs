
namespace Anycmd.Host.EDI.MemorySets
{
    using Anycmd.EDI;
    using Anycmd.Repositories;
    using Bus;
    using Entities;
    using Exceptions;
    using Extensions;
    using Info;
    using Messages;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ValueObjects;
    using dicCode = System.String;
    using dicID = System.Guid;
    using dicItemCode = System.String;

    /// <summary>
    /// 
    /// </summary>
    public sealed class InfoDicSet : IInfoDicSet
    {
        private readonly Dictionary<dicID, InfoDicState> _infoDicDicByID = new Dictionary<dicID, InfoDicState>();
        private readonly Dictionary<dicCode, InfoDicState> _infoDicDicByCode = new Dictionary<dicCode, InfoDicState>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<InfoDicState, Dictionary<dicItemCode, InfoDicItemState>> _infoDicItemByDic = new Dictionary<InfoDicState, Dictionary<dicItemCode, InfoDicItemState>>();
        private readonly Dictionary<Guid, InfoDicItemState> _infoDicItemDic = new Dictionary<dicID, InfoDicItemState>();
        private readonly List<InfoDicItemState> EmptyInfoDicItems = new List<InfoDicItemState>();
        private bool _initialized = false;
        private object locker = new object();

        private readonly Guid _id = Guid.NewGuid();
        private readonly NodeHost host;

        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// 构造并接入总线
        /// </summary>
        public InfoDicSet(NodeHost host)
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
            var handler = new MessageHandler(this);
            messageDispatcher.Register((IHandler<AddInfoDicCommand>)handler);
            messageDispatcher.Register((IHandler<InfoDicAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateInfoDicCommand>)handler);
            messageDispatcher.Register((IHandler<InfoDicUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveInfoDicCommand>)handler);
            messageDispatcher.Register((IHandler<InfoDicRemovedEvent>)handler);
            messageDispatcher.Register((IHandler<AddInfoDicItemCommand>)handler);
            messageDispatcher.Register((IHandler<InfoDicItemAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateInfoDicItemCommand>)handler);
            messageDispatcher.Register((IHandler<InfoDicItemUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveInfoDicItemCommand>)handler);
            messageDispatcher.Register((IHandler<InfoDicItemRemovedEvent>)handler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicID"></param>
        /// <param name="infoDic"></param>
        /// <returns></returns>
        public bool TryGetInfoDic(dicID dicID, out InfoDicState infoDic)
        {
            if (!_initialized)
            {
                Init();
            }
            return _infoDicDicByID.TryGetValue(dicID, out infoDic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicCode"></param>
        /// <param name="infoDic"></param>
        /// <returns></returns>
        public bool TryGetInfoDic(dicCode dicCode, out InfoDicState infoDic)
        {
            if (!_initialized)
            {
                Init();
            }
            if (dicCode == null)
            {
                infoDic = null;
                return false;
            }

            return _infoDicDicByCode.TryGetValue(dicCode, out infoDic);
        }

        /// <summary>
        /// 根据字典Id索引字典项集合
        /// </summary>
        /// <param name="infoDic">字典Id</param>
        /// <returns>字典项集合</returns>
        public IReadOnlyCollection<InfoDicItemState> GetInfoDicItems(InfoDicState infoDic)
        {
            if (!_initialized)
            {
                Init();
            }
            return !_infoDicItemByDic.ContainsKey(infoDic) ? EmptyInfoDicItems : new List<InfoDicItemState>(_infoDicItemByDic[infoDic].Values.OrderBy(item => item.SortCode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoDic"></param>
        /// <param name="itemCode"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetInfoDicItem(InfoDicState infoDic, dicItemCode itemCode, out InfoDicItemState item)
        {
            if (!_initialized)
            {
                Init();
            }
            if (!_infoDicItemByDic.ContainsKey(infoDic))
            {
                item = null;
                return false;
            }

            return _infoDicItemByDic[infoDic].TryGetValue(itemCode, out item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicItemID"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetInfoDicItem(Guid dicItemID, out InfoDicItemState item)
        {
            if (!_initialized)
            {
                Init();
            }
            return _infoDicItemDic.TryGetValue(dicItemID, out item);
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
        public IEnumerator<InfoDicState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _infoDicDicByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _infoDicDicByID.Values.GetEnumerator();
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (locker)
                {
                    if (!_initialized)
                    {
                        _infoDicDicByID.Clear();
                        _infoDicDicByCode.Clear();
                        _infoDicItemByDic.Clear();
                        _infoDicItemDic.Clear();
                        var allInfoDics = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetInfoDics();
                        foreach (var infoDic in allInfoDics)
                        {
                            if (!(infoDic is InfoDicBase))
                            {
                                throw new CoreException("信息字典模型必须继承InfoDicBase基类");
                            }
                            var infoDicState = InfoDicState.Create(infoDic);
                            _infoDicDicByID.Add(infoDic.Id, infoDicState);
                            _infoDicDicByCode.Add(infoDic.Code, infoDicState);
                            _infoDicItemByDic.Add(infoDicState, new Dictionary<string, InfoDicItemState>(StringComparer.OrdinalIgnoreCase));
                        }
                        var allDicItems = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetInfoDicItems();

                        foreach (var infoDicItem in allDicItems)
                        {
                            if (!(infoDicItem is InfoDicItemBase))
                            {
                                throw new CoreException("信息字典项模型必须继承InfoDicItemBase基类");
                            }
                            var infoDicItemState = InfoDicItemState.Create(infoDicItem);
                            var infoDic = _infoDicDicByID[infoDicItem.InfoDicID];
                            _infoDicItemByDic[infoDic].Add(infoDicItem.Code, infoDicItemState);
                            _infoDicItemDic.Add(infoDicItem.Id, infoDicItemState);
                        }
                        _initialized = true;
                    }
                }
            }
        }

        #region MessageHandler
        private class MessageHandler:
            IHandler<AddInfoDicCommand>,
            IHandler<InfoDicAddedEvent>,
            IHandler<UpdateInfoDicCommand>,
            IHandler<InfoDicUpdatedEvent>,
            IHandler<RemoveInfoDicCommand>,
            IHandler<InfoDicRemovedEvent>,
            IHandler<AddInfoDicItemCommand>,
            IHandler<InfoDicItemAddedEvent>,
            IHandler<UpdateInfoDicItemCommand>,
            IHandler<InfoDicItemUpdatedEvent>,
            IHandler<RemoveInfoDicItemCommand>,
            IHandler<InfoDicItemRemovedEvent>
        {
            private readonly InfoDicSet set;

            public MessageHandler(InfoDicSet set)
            {
                this.set = set;
            }

            public void Handle(AddInfoDicCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(InfoDicAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateInfoDicAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IInfoDicCreateInput input, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _infoDicDicByID = set._infoDicDicByID;
                var _infoDicDicByCode = set._infoDicDicByCode;
                var infoDicRepository = NodeHost.Instance.GetRequiredService<IRepository<InfoDic>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                InfoDic entity;
                lock (locker)
                {
                    InfoDicState infoDic;
                    if (NodeHost.Instance.InfoDics.TryGetInfoDic(input.Id.Value, out infoDic))
                    {
                        throw new ValidationException("给定标识标识的记录已经存在");
                    }
                    if (NodeHost.Instance.InfoDics.TryGetInfoDic(input.Code, out infoDic) && infoDic.Id != input.Id.Value)
                    {
                        throw new ValidationException("重复的编码");
                    }

                    entity = InfoDic.Create(input);

                    var state = InfoDicState.Create(entity);
                    if (!_infoDicDicByID.ContainsKey(entity.Id))
                    {
                        _infoDicDicByID.Add(entity.Id, state);
                    }
                    if (!_infoDicDicByCode.ContainsKey(entity.Code))
                    {
                        _infoDicDicByCode.Add(entity.Code, state);
                    }
                    if (isCommand)
                    {
                        try
                        {

                            infoDicRepository.Add(entity);
                            infoDicRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_infoDicDicByID.ContainsKey(entity.Id))
                            {
                                _infoDicDicByID.Remove(entity.Id);
                            }
                            if (_infoDicDicByCode.ContainsKey(entity.Code))
                            {
                                _infoDicDicByCode.Remove(entity.Code);
                            }
                            infoDicRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateInfoDicAddedEvent(entity, input));
                }
            }

            private class PrivateInfoDicAddedEvent : InfoDicAddedEvent
            {
                public PrivateInfoDicAddedEvent(InfoDicBase source, IInfoDicCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateInfoDicCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(InfoDicUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateInfoDicUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IInfoDicUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _infoDicDicByID = set._infoDicDicByID;
                var _infoDicDicByCode = set._infoDicDicByCode;
                var infoDicRepository = NodeHost.Instance.GetRequiredService<IRepository<InfoDic>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                InfoDicState infoDic;
                if (!NodeHost.Instance.InfoDics.TryGetInfoDic(input.Id, out infoDic))
                {
                    throw new NotExistException();
                }
                if (NodeHost.Instance.InfoDics.TryGetInfoDic(input.Code, out infoDic) && infoDic.Id != input.Id)
                {
                    throw new ValidationException("重复的编码");
                }
                var entity = infoDicRepository.GetByKey(input.Id);
                if (entity == null)
                {
                    throw new NotExistException();
                }
                var bkState = InfoDicState.Create(entity);

                entity.Update(input);

                var newState = InfoDicState.Create(entity);
                bool stateChanged = newState != bkState;
                lock (locker)
                {
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            infoDicRepository.Update(entity);
                            infoDicRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            infoDicRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateInfoDicUpdatedEvent(entity, input));
                }
            }

            private void Update(InfoDicState state)
            {
                var host = set.host;
                var locker = set.locker;
                var _infoDicDicByID = set._infoDicDicByID;
                var _infoDicDicByCode = set._infoDicDicByCode;
                var newKey = state.Code;
                var oldKey = _infoDicDicByID[state.Id].Code;
                _infoDicDicByID[state.Id] = state;
                if (!_infoDicDicByCode.ContainsKey(newKey))
                {
                    _infoDicDicByCode.Add(newKey, state);
                    _infoDicDicByCode.Remove(oldKey);
                }
                else
                {
                    _infoDicDicByCode[newKey] = state;
                }
            }

            private class PrivateInfoDicUpdatedEvent : InfoDicUpdatedEvent
            {
                public PrivateInfoDicUpdatedEvent(InfoDicBase source, IInfoDicUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveInfoDicCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(InfoDicRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateInfoDicRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid infoDicID, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _infoDicDicByID = set._infoDicDicByID;
                var _infoDicDicByCode = set._infoDicDicByCode;
                var infoDicRepository = NodeHost.Instance.GetRequiredService<IRepository<InfoDic>>();
                var infoDicItemRepository = NodeHost.Instance.GetRequiredService<IRepository<InfoDicItem>>();
                InfoDicState infoDic;
                if (!NodeHost.Instance.InfoDics.TryGetInfoDic(infoDicID, out infoDic))
                {
                    return;
                }
                var infoDicItems = NodeHost.Instance.InfoDics.GetInfoDicItems(infoDic);
                if (infoDicItems != null && infoDicItems.Count > 0)
                {
                    throw new ValidationException("信息字典下有信息字典项时不能删除");
                }
                InfoDic entity = infoDicRepository.GetByKey(infoDicID);
                if (entity == null)
                {
                    return;
                }
                var bkState = InfoDicState.Create(entity);
                lock (locker)
                {
                    if (_infoDicDicByID.ContainsKey(entity.Id))
                    {
                        _infoDicDicByID.Remove(entity.Id);
                    }
                    if (_infoDicDicByCode.ContainsKey(entity.Code))
                    {
                        _infoDicDicByCode.Remove(entity.Code);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            infoDicRepository.Remove(entity);
                            infoDicRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_infoDicDicByID.ContainsKey(entity.Id))
                            {
                                _infoDicDicByID.Add(entity.Id, bkState);
                            }
                            if (!_infoDicDicByCode.ContainsKey(entity.Code))
                            {
                                _infoDicDicByCode.Add(entity.Code, bkState);
                            }
                            infoDicRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateInfoDicRemovedEvent(entity));
                }
            }

            private class PrivateInfoDicRemovedEvent : InfoDicRemovedEvent
            {
                public PrivateInfoDicRemovedEvent(InfoDicBase source)
                    : base(source)
                {

                }
            }
            public void Handle(AddInfoDicItemCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(InfoDicItemAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateInfoDicItemAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IInfoDicItemCreateInput input, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _infoDicItemDic = set._infoDicItemDic;
                var _infoDicItemByDic = set._infoDicItemByDic;
                var infoDicItemRepository = NodeHost.Instance.GetRequiredService<IRepository<InfoDicItem>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                InfoDicState infoDic;
                if (!NodeHost.Instance.InfoDics.TryGetInfoDic(input.InfoDicID, out infoDic))
                {
                    throw new ValidationException("意外的信息字典标识");
                }
                InfoDicItemState infoDicItem;
                if (NodeHost.Instance.InfoDics.TryGetInfoDicItem(input.Id.Value, out infoDicItem))
                {
                    throw new ValidationException("给定的标识标识的记录已经存在");
                }
                if (NodeHost.Instance.InfoDics.TryGetInfoDicItem(infoDic, input.Code, out infoDicItem))
                {
                    throw new ValidationException("重复的编码");
                }

                var entity = InfoDicItem.Create(input);

                lock (locker)
                {
                    var state = InfoDicItemState.Create(entity);
                    if (!_infoDicItemDic.ContainsKey(entity.Id))
                    {
                        _infoDicItemDic.Add(entity.Id, state);
                    }
                    if (!_infoDicItemByDic.ContainsKey(infoDic))
                    {
                        _infoDicItemByDic.Add(infoDic, new Dictionary<dicItemCode, InfoDicItemState>(StringComparer.OrdinalIgnoreCase));
                    }
                    if (!_infoDicItemByDic[infoDic].ContainsKey(entity.Code))
                    {
                        _infoDicItemByDic[infoDic].Add(entity.Code, state);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            infoDicItemRepository.Add(entity);
                            infoDicItemRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_infoDicItemDic.ContainsKey(entity.Id))
                            {
                                _infoDicItemDic.Remove(entity.Id);
                            }
                            if (_infoDicItemByDic.ContainsKey(infoDic) && _infoDicItemByDic[infoDic].ContainsKey(entity.Code))
                            {
                                _infoDicItemByDic[infoDic].Remove(entity.Code);
                            }
                            infoDicItemRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateInfoDicItemAddedEvent(entity, input));
                }
            }

            private class PrivateInfoDicItemAddedEvent : InfoDicItemAddedEvent
            {
                public PrivateInfoDicItemAddedEvent(InfoDicItemBase source, IInfoDicItemCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateInfoDicItemCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(InfoDicItemUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateInfoDicItemUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IInfoDicItemUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _infoDicItemDic = set._infoDicItemDic;
                var _infoDicItemByDic = set._infoDicItemByDic;
                var infoDicItemRepository = NodeHost.Instance.GetRequiredService<IRepository<InfoDicItem>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                InfoDicState infoDic;
                if (!NodeHost.Instance.InfoDics.TryGetInfoDic(input.InfoDicID, out infoDic))
                {
                    throw new ValidationException("意外的信息字典项字典标识");
                }
                InfoDicItemState infoDicItem;
                if (!NodeHost.Instance.InfoDics.TryGetInfoDicItem(input.Id, out infoDicItem))
                {
                    throw new NotExistException();
                }
                if (NodeHost.Instance.InfoDics.TryGetInfoDicItem(infoDic, input.Code, out infoDicItem) && infoDicItem.Id != input.Id)
                {
                    throw new ValidationException("重复的编码");
                }
                var entity = infoDicItemRepository.GetByKey(input.Id);
                if (entity == null)
                {
                    throw new NotExistException();
                }
                var bkState = InfoDicItemState.Create(entity);

                entity.Update(input);

                var newState = InfoDicItemState.Create(entity);
                bool stateChanged = newState != bkState;
                lock (locker)
                {
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            infoDicItemRepository.Update(entity);
                            infoDicItemRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            infoDicItemRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateInfoDicItemUpdatedEvent(entity, input));
                }
            }

            private void Update(InfoDicItemState state)
            {
                var host = set.host;
                var locker = set.locker;
                var _infoDicItemDic = set._infoDicItemDic;
                var _infoDicItemByDic = set._infoDicItemByDic;
                var oldState = _infoDicItemDic[state.Id];
                var newKey = state.Code;
                var oldKey = oldState.Code;
                InfoDicState infoDic;
                if (!host.InfoDics.TryGetInfoDic(oldState.InfoDicID, out infoDic))
                {
                    throw new CoreException("意外的信息字典标识" + oldState.InfoDicID);
                }
                _infoDicItemDic[state.Id] = state;
                if (!_infoDicItemByDic[infoDic].ContainsKey(newKey))
                {
                    _infoDicItemByDic[infoDic].Add(newKey, state);
                    _infoDicItemByDic[infoDic].Remove(oldKey);
                }
                else
                {
                    _infoDicItemByDic[infoDic][newKey] = state;
                }
            }

            private class PrivateInfoDicItemUpdatedEvent : InfoDicItemUpdatedEvent
            {
                public PrivateInfoDicItemUpdatedEvent(InfoDicItemBase source, IInfoDicItemUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveInfoDicItemCommand message)
            {
                this.HandleItem(message.EntityID, true);
            }

            public void Handle(InfoDicItemRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateInfoDicItemRemovedEvent))
                {
                    return;
                }
                this.HandleItem(message.Source.Id, false);
            }

            private void HandleItem(Guid infoDicItemID, bool isCommand)
            {
                var host = set.host;
                var locker = set.locker;
                var _infoDicItemDic = set._infoDicItemDic;
                var _infoDicItemByDic = set._infoDicItemByDic;
                var infoDicItemRepository = NodeHost.Instance.GetRequiredService<IRepository<InfoDicItem>>();
                InfoDicItemState infoDicItem;
                if (!NodeHost.Instance.InfoDics.TryGetInfoDicItem(infoDicItemID, out infoDicItem))
                {
                    return;
                }
                InfoDicState infoDic;
                if (!NodeHost.Instance.InfoDics.TryGetInfoDic(infoDicItem.InfoDicID, out infoDic))
                {
                    throw new CoreException("意外的信息字典项字典标识");
                }
                InfoDicItem entity = infoDicItemRepository.GetByKey(infoDicItemID);
                if (entity == null)
                {
                    return;
                }
                var bkState = InfoDicItemState.Create(entity);
                lock (locker)
                {
                    if (_infoDicItemDic.ContainsKey(entity.Id))
                    {
                        _infoDicItemDic.Remove(entity.Id);
                    }
                    if (_infoDicItemByDic.ContainsKey(infoDic) && _infoDicItemByDic[infoDic].ContainsKey(entity.Code))
                    {
                        _infoDicItemByDic[infoDic].Remove(entity.Code);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            infoDicItemRepository.Remove(entity);
                            infoDicItemRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_infoDicItemDic.ContainsKey(entity.Id))
                            {
                                _infoDicItemDic.Add(entity.Id, bkState);
                            }
                            if (!_infoDicItemByDic.ContainsKey(infoDic))
                            {
                                _infoDicItemByDic.Add(infoDic, new Dictionary<dicItemCode, InfoDicItemState>(StringComparer.OrdinalIgnoreCase));
                            }
                            if (!_infoDicItemByDic[infoDic].ContainsKey(entity.Code))
                            {
                                _infoDicItemByDic[infoDic].Add(entity.Code, bkState);
                            }
                            infoDicItemRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.AppHost.MessageDispatcher.DispatchMessage(new PrivateInfoDicItemRemovedEvent(entity));
                }
            }

            private class PrivateInfoDicItemRemovedEvent : InfoDicItemRemovedEvent
            {
                public PrivateInfoDicItemRemovedEvent(InfoDicItemBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion
    }
}
