
namespace Anycmd.Host.AC.MemorySets.Impl
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
    using ValueObjects;

    public sealed class PageSet : IPageSet
    {
        public static readonly IPageSet Empty = new PageSet(AppHost.Empty);

        private readonly Dictionary<FunctionState, PageState> _pageDicByFunction = new Dictionary<FunctionState, PageState>();
        private readonly Dictionary<Guid, PageState> _pageDicByID = new Dictionary<Guid, PageState>();
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly IAppHost host;
        private readonly PageButtonSet pageButtonSet;

        public Guid Id
        {
            get { return _id; }
        }

        public PageSet(IAppHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
            pageButtonSet = new PageButtonSet(host);
            var messageDispatcher = host.MessageDispatcher;
            if (messageDispatcher == null)
            {
                throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
            }
            var handler = new MessageHandler(this);
            messageDispatcher.Register((IHandler<AddPageCommand>)handler);
            messageDispatcher.Register((IHandler<PageAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdatePageCommand>)handler);
            messageDispatcher.Register((IHandler<PageUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemovePageCommand>)handler);
            messageDispatcher.Register((IHandler<PageRemovedEvent>)handler);
            messageDispatcher.Register((IHandler<FunctionUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<FunctionRemovingEvent>)handler);
            messageDispatcher.Register((IHandler<FunctionRemovedEvent>)handler);
        }

        public bool TryGetPage(FunctionState function, out PageState page)
        {
            if (!_initialized)
            {
                Init();
            }

            return _pageDicByFunction.TryGetValue(function, out page);
        }

        public bool TryGetPage(Guid pageID, out PageState page)
        {
            if (!_initialized)
            {
                Init();
            }
            return _pageDicByID.TryGetValue(pageID, out page);
        }

        public IReadOnlyList<PageButtonState> GetPageButtons(PageState page)
        {
            if (!_initialized)
            {
                Init();
            }
            return pageButtonSet.GetPageButtons(page);
        }

        public IEnumerable<PageButtonState> GetPageButtons()
        {
            if (!_initialized)
            {
                Init();
            }
            return pageButtonSet;
        }

        internal void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
        }

        public IEnumerator<PageState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _pageDicByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _pageDicByID.Values.GetEnumerator();
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _pageDicByFunction.Clear();
                        _pageDicByID.Clear();
                        var pages = host.GetRequiredService<IAppHostBootstrap>().GetAllPages();
                        foreach (var page in pages)
                        {
                            if (!(page is PageBase))
                            {
                                throw new CoreException(page.GetType().Name + "必须继承" + typeof(PageBase).Name);
                            }
                            var pageState = PageState.Create(host, page);
                            _pageDicByID.Add(page.Id, pageState);
                            FunctionState function;
                            if (!host.FunctionSet.TryGetFunction(page.Id, out function))
                            {
                                throw new NotExistException("意外的功能标识" + page.Id);
                            }
                            if (!_pageDicByFunction.ContainsKey(function))
                            {
                                _pageDicByFunction.Add(function, pageState);
                            }
                        }
                        _initialized = true;
                    }
                }
            }
        }

        #region MessageHandler
        private class MessageHandler:
            IHandler<AddPageCommand>,
            IHandler<PageUpdatedEvent>,
            IHandler<RemovePageCommand>,
            IHandler<FunctionUpdatedEvent>,
            IHandler<FunctionRemovingEvent>,
            IHandler<FunctionRemovedEvent>, 
            IHandler<PageAddedEvent>, 
            IHandler<UpdatePageCommand>, 
            IHandler<PageRemovedEvent>
        {
            private readonly PageSet set;

            public MessageHandler(PageSet set)
            {
                this.set = set;
            }

            public void Handle(FunctionUpdatedEvent message)
            {
                var host = set.host;
                var _pageDicByFunction = set._pageDicByFunction;
                FunctionState newKey;
                if (!host.FunctionSet.TryGetFunction(message.Source.Id, out newKey))
                {
                    throw new CoreException("意外的功能标识" + message.Source.Id);
                }
                var oldKey = _pageDicByFunction.Keys.FirstOrDefault(a => a.Id == newKey.Id);
                if (oldKey != null && !_pageDicByFunction.ContainsKey(newKey))
                {
                    _pageDicByFunction.Add(newKey, _pageDicByFunction[oldKey]);
                    _pageDicByFunction.Remove(oldKey);
                }
            }

            public void Handle(FunctionRemovingEvent message)
            {
                var host = set.host;
                var _pageDicByFunction = set._pageDicByFunction;
                var _pageDicByID = set._pageDicByID;
                if (_pageDicByID.ContainsKey(message.Source.Id))
                {
                    host.Handle(new RemovePageCommand(message.Source.Id));
                }
            }

            public void Handle(FunctionRemovedEvent message)
            {
                var host = set.host;
                var _pageDicByFunction = set._pageDicByFunction;
                var _pageDicByID = set._pageDicByID;
                var key = _pageDicByFunction.Keys.FirstOrDefault(a => a.Id == message.Source.Id);
                if (key != null)
                {
                    _pageDicByFunction.Remove(key);
                }
            }

            public void Handle(AddPageCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(PageAddedEvent message)
            {
                if (message.GetType() == typeof(PrivatePageAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IPageCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _pageDicByFunction = set._pageDicByFunction;
                var _pageDicByID = set._pageDicByID;
                var pageRepository = host.GetRequiredService<IRepository<Page>>();
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                Page entity;
                lock (this)
                {
                    FunctionState function;
                    if (!host.FunctionSet.TryGetFunction(input.Id.Value, out function))
                    {
                        throw new ValidationException("意外的功能标识，页面首先是个功能。请先添加页面对应的功能记录。");
                    }
                    PageState page;
                    if (host.PageSet.TryGetPage(input.Id.Value, out page))
                    {
                        throw new ValidationException("给定标识的页面已经存在");
                    }

                    entity = Page.Create(input);

                    var state = PageState.Create(host, entity);
                    if (!_pageDicByID.ContainsKey(state.Id))
                    {
                        _pageDicByID.Add(state.Id, state);
                    }
                    if (!_pageDicByFunction.ContainsKey(function))
                    {
                        _pageDicByFunction.Add(function, state);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            pageRepository.Add(entity);
                            pageRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_pageDicByID.ContainsKey(entity.Id))
                            {
                                _pageDicByID.Remove(entity.Id);
                            }
                            if (_pageDicByFunction.ContainsKey(function))
                            {
                                _pageDicByFunction.Remove(function);
                            }
                            pageRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivatePageAddedEvent(entity, input));
                }
            }

            private class PrivatePageAddedEvent : PageAddedEvent
            {
                public PrivatePageAddedEvent(PageBase source, IPageCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdatePageCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(PageUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivatePageUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IPageUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _pageDicByFunction = set._pageDicByFunction;
                var _pageDicByID = set._pageDicByID;
                var pageRepository = host.GetRequiredService<IRepository<Page>>();
                PageState bkState;
                if (!host.PageSet.TryGetPage(input.Id, out bkState))
                {
                    throw new NotExistException();
                }
                Page entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    PageState state;
                    if (!host.PageSet.TryGetPage(input.Id, out state))
                    {
                        throw new NotExistException();
                    }
                    entity = pageRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = PageState.Create(host, entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            pageRepository.Update(entity);
                            pageRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            pageRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivatePageUpdatedEvent(entity, input));
                }
            }

            private void Update(PageState state)
            {
                var host = set.host;
                var _pageDicByFunction = set._pageDicByFunction;
                var _pageDicByID = set._pageDicByID;
                FunctionState function;
                host.FunctionSet.TryGetFunction(state.Id, out function);
                _pageDicByID[state.Id] = state;
                _pageDicByFunction[function] = state;
            }

            private class PrivatePageUpdatedEvent : PageUpdatedEvent
            {
                public PrivatePageUpdatedEvent(PageBase source, IPageUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemovePageCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(PageRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivatePageRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid pageID, bool isCommand)
            {
                var host = set.host;
                var _pageDicByFunction = set._pageDicByFunction;
                var _pageDicByID = set._pageDicByID;
                var pageRepository = host.GetRequiredService<IRepository<Page>>();
                var pageButtonRepository = host.GetRequiredService<IRepository<PageButton>>();
                PageState bkState;
                if (!host.PageSet.TryGetPage(pageID, out bkState))
                {
                    return;
                }
                Page entity;
                lock (bkState)
                {
                    PageState state;
                    if (!host.PageSet.TryGetPage(pageID, out state))
                    {
                        return;
                    }
                    FunctionState function;
                    if (!host.FunctionSet.TryGetFunction(pageID, out function))
                    {
                        throw new NotExistException("意外的功能标识" + pageID);
                    }
                    entity = pageRepository.GetByKey(pageID);
                    if (entity == null)
                    {
                        return;
                    }
                    foreach (var pageButton in pageButtonRepository.FindAll().Where(a => a.PageID == entity.Id).ToList())
                    {
                        pageButtonRepository.Remove(pageButton);
                    }
                    if (_pageDicByID.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new PageRemovingEvent(entity));
                        }
                        _pageDicByID.Remove(bkState.Id);
                    }
                    if (_pageDicByFunction.ContainsKey(function))
                    {
                        _pageDicByFunction.Remove(function);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            pageButtonRepository.Context.Commit();
                            pageRepository.Remove(entity);
                            pageRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_pageDicByID.ContainsKey(bkState.Id))
                            {
                                _pageDicByID.Add(bkState.Id, bkState);
                            }
                            if (!_pageDicByFunction.ContainsKey(function))
                            {
                                _pageDicByFunction.Add(function, bkState);
                            }
                            pageButtonRepository.Context.Rollback();
                            pageRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivatePageRemovedEvent(entity));
                }
            }

            private class PrivatePageRemovedEvent : PageRemovedEvent
            {
                public PrivatePageRemovedEvent(PageBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion

        // 内部类
        #region PageButtonSet
        /// <summary>
        /// 页面菜单上下文
        /// </summary>
        private sealed class PageButtonSet : IEnumerable<PageButtonState>
        {
            private readonly Dictionary<PageState, List<PageButtonState>> _pageButtonsByPage = new Dictionary<PageState, List<PageButtonState>>();
            private readonly Dictionary<Guid, PageButtonState> _pageButtonDicByID = new Dictionary<Guid, PageButtonState>();
            private bool _initialized = false;
            private readonly IAppHost host;

            public PageButtonSet(IAppHost host)
            {
                this.host = host;
                var messageDispatcher = host.MessageDispatcher;
                if (messageDispatcher == null)
                {
                    throw new ArgumentNullException("messageDispatcher has not be set of host:{0}".Fmt(host.Name));
                }
                var handler = new MessageHandler(this);
                messageDispatcher.Register((IHandler<AddPageButtonCommand>)handler);
                messageDispatcher.Register((IHandler<PageButtonAddedEvent>)handler);
                messageDispatcher.Register((IHandler<UpdatePageButtonCommand>)handler);
                messageDispatcher.Register((IHandler<PageButtonUpdatedEvent>)handler);
                messageDispatcher.Register((IHandler<RemovePageButtonCommand>)handler);
                messageDispatcher.Register((IHandler<PageButtonRemovedEvent>)handler);
                messageDispatcher.Register((IHandler<PageUpdatedEvent>)handler);
                messageDispatcher.Register((IHandler<PageRemovingEvent>)handler);
                messageDispatcher.Register((IHandler<PageRemovedEvent>)handler);
                messageDispatcher.Register((IHandler<FunctionRemovingEvent>)handler);
            }

            public IReadOnlyList<PageButtonState> GetPageButtons(PageState page)
            {
                if (!_initialized)
                {
                    Init();
                }
                if (!_pageButtonsByPage.ContainsKey(page))
                {
                    return new List<PageButtonState>();
                }

                return _pageButtonsByPage[page];
            }

            public IEnumerator<PageButtonState> GetEnumerator()
            {
                if (!_initialized)
                {
                    Init();
                }
                foreach (var item in _pageButtonDicByID.Values)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                if (!_initialized)
                {
                    Init();
                }
                foreach (var item in _pageButtonDicByID.Values)
                {
                    yield return item;
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
                            _pageButtonsByPage.Clear();
                            _pageButtonDicByID.Clear();
                            var allPageButtons = host.GetRequiredService<IAppHostBootstrap>().GetAllPageButtons();
                            foreach (var pageButton in allPageButtons)
                            {
                                if (!(pageButton is PageButtonBase))
                                {
                                    throw new CoreException(pageButton.GetType().Name + "必须继承" + typeof(PageButtonBase).Name);
                                }
                                var pageButtonState = PageButtonState.Create(host, pageButton);
                                if (!_pageButtonDicByID.ContainsKey(pageButton.Id))
                                {
                                    _pageButtonDicByID.Add(pageButton.Id, pageButtonState);
                                }
                                if (!_pageButtonsByPage.ContainsKey(pageButtonState.Page))
                                {
                                    _pageButtonsByPage.Add(pageButtonState.Page, new List<PageButtonState>());
                                }
                                _pageButtonsByPage[pageButtonState.Page].Add(pageButtonState);
                            }
                            foreach (var item in _pageButtonsByPage)
                            {
                                item.Value.Sort(new PageButtonCompare());
                            }
                            _initialized = true;
                        }
                    }
                }
            }

            #region MessageHandler
            private class MessageHandler:
                IHandler<PageButtonAddedEvent>,
                IHandler<PageButtonUpdatedEvent>,
                IHandler<RemovePageButtonCommand>,
                IHandler<PageButtonRemovedEvent>,
                IHandler<PageUpdatedEvent>,
                IHandler<FunctionRemovingEvent>,
                IHandler<AddPageButtonCommand>,
                IHandler<UpdatePageButtonCommand>,
                IHandler<PageRemovingEvent>,
                IHandler<PageRemovedEvent>
            {
                private readonly PageButtonSet set;

                public MessageHandler(PageButtonSet set)
                {
                    this.set = set;
                }

                public void Handle(PageUpdatedEvent message)
                {
                    var host = set.host;
                    var _pageButtonsByPage = set._pageButtonsByPage;
                    PageState newKey;
                    if (!host.PageSet.TryGetPage(message.Source.Id, out newKey))
                    {
                        throw new CoreException("意外的页面标识" + message.Source.Id);
                    }
                    var oldKey = _pageButtonsByPage.Keys.FirstOrDefault(a => a.Id == newKey.Id);
                    if (oldKey != null && !_pageButtonsByPage.ContainsKey(newKey))
                    {
                        _pageButtonsByPage.Add(newKey, _pageButtonsByPage[oldKey]);
                        _pageButtonsByPage.Remove(oldKey);
                    }
                }

                public void Handle(PageRemovingEvent message)
                {
                    var host = set.host;
                    var _pageButtonsByPage = set._pageButtonsByPage;
                    PageState key;
                    if (!host.PageSet.TryGetPage(message.Source.Id, out key))
                    {
                        throw new CoreException("意外的页面标识" + message.Source.Id);
                    }
                    if (_pageButtonsByPage.ContainsKey(key))
                    {
                        HashSet<Guid> pageButtonIDs = new HashSet<Guid>();
                        foreach (var item in _pageButtonsByPage[key])
                        {
                            pageButtonIDs.Add(item.Id);
                        }
                        foreach (var pageButtonID in pageButtonIDs)
                        {
                            host.Handle(new RemovePageButtonCommand(pageButtonID));
                        }
                        _pageButtonsByPage.Remove(key);
                    }
                }

                public void Handle(PageRemovedEvent message)
                {
                    var host = set.host;
                    var _pageButtonsByPage = set._pageButtonsByPage;
                    var key = _pageButtonsByPage.Keys.FirstOrDefault(a => a.Id == message.Source.Id);
                    if (key != null)
                    {
                        _pageButtonsByPage.Remove(key);
                    }
                }

                public void Handle(FunctionRemovingEvent message)
                {
                    var host = set.host;
                    var _pageButtonsByPage = set._pageButtonsByPage;
                    var _pageButtonDicByID = set._pageButtonDicByID;
                    HashSet<Guid> pageButtonIDs = new HashSet<Guid>();
                    foreach (var item in _pageButtonDicByID.Values)
                    {
                        if (item.FunctionID.HasValue && item.FunctionID.Value == message.Source.Id)
                        {
                            pageButtonIDs.Add(item.Id);
                        }
                    }
                    foreach (var pageButtonID in pageButtonIDs)
                    {
                        host.Handle(new RemovePageButtonCommand(pageButtonID));
                    }
                }

                public void Handle(AddPageButtonCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(PageButtonAddedEvent message)
                {
                    if (message.GetType() == typeof(PrivatePageButtonAddedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Handle(IPageButtonCreateInput input, bool isCommand)
                {
                    var host = set.host;
                    var _pageButtonsByPage = set._pageButtonsByPage;
                    var _pageButtonDicByID = set._pageButtonDicByID;
                    var pageButtonRepository = host.GetRequiredService<IRepository<PageButton>>();
                    if (!input.Id.HasValue)
                    {
                        throw new ValidationException("标识是必须的");
                    }
                    PageButton entity;
                    lock (this)
                    {
                        ButtonState button;
                        if (!host.ButtonSet.TryGetButton(input.ButtonID, out button))
                        {
                            throw new ValidationException("按钮不存在");
                        }
                        PageState page;
                        if (!host.PageSet.TryGetPage(input.PageID, out page))
                        {
                            throw new ValidationException("页面不存在");
                        }
                        if (input.FunctionID.HasValue)
                        {
                            FunctionState function;
                            if (!host.FunctionSet.TryGetFunction(input.FunctionID.Value, out function))
                            {
                                throw new ValidationException("托管功能不存在");
                            }
                        }
                        if (host.PageSet.GetPageButtons().Any(a => a.Id == input.Id.Value))
                        {
                            throw new ValidationException("给定标识的页面按钮已经存在");
                        }

                        entity = PageButton.Create(input);

                        var state = PageButtonState.Create(host, entity);
                        if (!_pageButtonsByPage.ContainsKey(page))
                        {
                            _pageButtonsByPage.Add(page, new List<PageButtonState>());
                        }
                        if (!_pageButtonsByPage[page].Contains(state))
                        {
                            _pageButtonsByPage[page].Add(state);
                        }
                        if (!_pageButtonDicByID.ContainsKey(state.Id))
                        {
                            _pageButtonDicByID.Add(state.Id, state);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                pageButtonRepository.Add(entity);
                                pageButtonRepository.Context.Commit();
                            }
                            catch
                            {
                                if (_pageButtonsByPage.ContainsKey(page))
                                {
                                    if (_pageButtonsByPage[page].Any(a => a.Id == entity.Id))
                                    {
                                        var item = _pageButtonsByPage[page].First(a => a.Id == entity.Id);
                                        _pageButtonsByPage[page].Remove(item);
                                    }
                                }
                                if (_pageButtonDicByID.ContainsKey(entity.Id))
                                {
                                    _pageButtonDicByID.Remove(entity.Id);
                                }
                                pageButtonRepository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivatePageButtonAddedEvent(entity, input));
                    }
                }

                private class PrivatePageButtonAddedEvent : PageButtonAddedEvent
                {
                    public PrivatePageButtonAddedEvent(PageButtonBase source, IPageButtonCreateInput input) : base(source, input) { }
                }
                public void Handle(UpdatePageButtonCommand message)
                {
                    this.Handle(message.Input, true);
                }

                public void Handle(PageButtonUpdatedEvent message)
                {
                    if (message.GetType() == typeof(PrivatePageButtonUpdatedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Input, false);
                }

                private void Update(PageButtonState state)
                {
                    var host = set.host;
                    var _pageButtonsByPage = set._pageButtonsByPage;
                    var _pageButtonDicByID = set._pageButtonDicByID;
                    var oldState = _pageButtonDicByID[state.Id];
                    _pageButtonDicByID[state.Id] = state;
                    foreach (var item in _pageButtonsByPage)
                    {
                        if (item.Value.Contains(oldState))
                        {
                            item.Value.Remove(oldState);
                            item.Value.Add(state);
                        }
                    }
                }

                private void Handle(IPageButtonUpdateInput input, bool isCommand)
                {
                    var host = set.host;
                    var _pageButtonsByPage = set._pageButtonsByPage;
                    var _pageButtonDicByID = set._pageButtonDicByID;
                    var pageButtonRepository = host.GetRequiredService<IRepository<PageButton>>();
                    var bkState = host.PageSet.GetPageButtons().FirstOrDefault(a => a.Id == input.Id);
                    if (bkState == null)
                    {
                        throw new NotExistException();
                    }
                    if (input.FunctionID.HasValue)
                    {
                        FunctionState function;
                        if (!host.FunctionSet.TryGetFunction(input.FunctionID.Value, out function))
                        {
                            throw new ValidationException("非法的托管功能标识" + input.FunctionID);
                        }
                    }
                    PageButton entity;
                    bool stateChanged = false;
                    lock (bkState)
                    {
                        if (!host.PageSet.GetPageButtons().Any(a => a.Id == input.Id))
                        {
                            throw new NotExistException();
                        }
                        entity = pageButtonRepository.GetByKey(input.Id);
                        if (entity == null)
                        {
                            throw new NotExistException();
                        }

                        entity.Update(input);

                        var newState = PageButtonState.Create(host, entity);
                        stateChanged = newState != bkState;
                        if (stateChanged)
                        {
                            Update(newState);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                pageButtonRepository.Update(entity);
                                pageButtonRepository.Context.Commit();
                            }
                            catch
                            {
                                if (stateChanged)
                                {
                                    Update(bkState);
                                }
                                pageButtonRepository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand && stateChanged)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivatePageButtonUpdatedEvent(entity, input));
                    }
                }

                private class PrivatePageButtonUpdatedEvent : PageButtonUpdatedEvent
                {
                    public PrivatePageButtonUpdatedEvent(PageButtonBase source, IPageButtonUpdateInput input)
                        : base(source, input)
                    {

                    }
                }
                public void Handle(RemovePageButtonCommand message)
                {
                    this.Handle(message.EntityID, true);
                }

                public void Handle(PageButtonRemovedEvent message)
                {
                    if (message.GetType() == typeof(PrivatePageButtonRemovedEvent))
                    {
                        return;
                    }
                    this.Handle(message.Source.Id, false);
                }

                private void Handle(Guid pageButtonID, bool isCommand)
                {
                    var host = set.host;
                    var _pageButtonsByPage = set._pageButtonsByPage;
                    var _pageButtonDicByID = set._pageButtonDicByID;
                    var pageButtonRepository = host.GetRequiredService<IRepository<PageButton>>();
                    var bkState = host.PageSet.GetPageButtons().FirstOrDefault(a => a.Id == pageButtonID);
                    if (bkState == null)
                    {
                        return;
                    }
                    PageButton entity;
                    lock (bkState)
                    {
                        if (!host.PageSet.GetPageButtons().Any(a => a.Id == pageButtonID))
                        {
                            return;
                        }
                        entity = pageButtonRepository.GetByKey(pageButtonID);
                        if (entity == null)
                        {
                            return;
                        }
                        if (_pageButtonDicByID.ContainsKey(bkState.Id))
                        {
                            if (_pageButtonsByPage.ContainsKey(bkState.Page) && _pageButtonsByPage[bkState.Page].Any(a => a.Id == bkState.Id))
                            {
                                var item = _pageButtonsByPage[bkState.Page].First(a => a.Id == bkState.Id);
                                _pageButtonsByPage[bkState.Page].Remove(item);
                            }
                            _pageButtonDicByID.Remove(bkState.Id);
                        }
                        if (isCommand)
                        {
                            try
                            {
                                pageButtonRepository.Remove(entity);
                                pageButtonRepository.Context.Commit();
                            }
                            catch
                            {
                                if (!_pageButtonDicByID.ContainsKey(bkState.Id))
                                {
                                    if (!_pageButtonsByPage.ContainsKey(bkState.Page))
                                    {
                                        _pageButtonsByPage.Add(bkState.Page, new List<PageButtonState>());
                                    }
                                    if (!_pageButtonsByPage[bkState.Page].Any(a => a.Id == bkState.Id))
                                    {
                                        _pageButtonsByPage[bkState.Page].Add(bkState);
                                    }
                                    _pageButtonDicByID.Add(bkState.Id, bkState);
                                }
                                pageButtonRepository.Context.Rollback();
                                throw;
                            }
                        }
                    }
                    if (isCommand)
                    {
                        host.MessageDispatcher.DispatchMessage(new PrivatePageButtonRemovedEvent(entity));
                    }
                }

                private class PrivatePageButtonRemovedEvent : PageButtonRemovedEvent
                {
                    public PrivatePageButtonRemovedEvent(PageButtonBase source)
                        : base(source)
                    {

                    }
                }
            }
            #endregion

            private class PageButtonCompare : IComparer<PageButtonState>
            {
                public int Compare(PageButtonState x, PageButtonState y)
                {
                    return x.Button.SortCode.CompareTo(y.Button.SortCode);
                }
            }
        }
        #endregion
    }
}