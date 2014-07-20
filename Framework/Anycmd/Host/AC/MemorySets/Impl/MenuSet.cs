
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

    public sealed class MenuSet : IMenuSet
    {
        public static readonly IMenuSet Empty = new MenuSet(AppHost.Empty);

        private readonly Dictionary<Guid, MenuState> _menuByID = new Dictionary<Guid, MenuState>();
        private bool _initialized = false;

        private readonly Guid _id = Guid.NewGuid();
        private readonly IAppHost host;

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public MenuSet(IAppHost host)
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
            var handler = new MessageHandler(this);
            messageDispatcher.Register((IHandler<AddMenuCommand>)handler);
            messageDispatcher.Register((IHandler<MenuAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateMenuCommand>)handler);
            messageDispatcher.Register((IHandler<MenuUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveMenuCommand>)handler);
            messageDispatcher.Register((IHandler<MenuRemovedEvent>)handler);
        }

        public bool TryGetMenu(Guid menuID, out MenuState menu)
        {
            if (!_initialized)
            {
                Init();
            }
            return _menuByID.TryGetValue(menuID, out menu);
        }

        public IEnumerator<MenuState> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _menuByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _menuByID.Values.GetEnumerator();
        }

        private void Init()
        {
            if (!_initialized)
            {
                lock (this)
                {
                    if (!_initialized)
                    {
                        _menuByID.Clear();
                        var menus = host.GetRequiredService<IAppHostBootstrap>().GetAllMenus().OrderBy(a => a.SortCode);
                        foreach (var menu in menus)
                        {
                            _menuByID.Add(menu.Id, MenuState.Create(host, menu));
                        }
                        _initialized = true;
                    }
                }
            }
        }

        #region MessageHandler
        private class MessageHandler :
            IHandler<MenuUpdatedEvent>, 
            IHandler<AddMenuCommand>, 
            IHandler<MenuAddedEvent>, 
            IHandler<UpdateMenuCommand>, 
            IHandler<RemoveMenuCommand>, 
            IHandler<MenuRemovedEvent>
        {
            private readonly MenuSet set;

            public MessageHandler(MenuSet set)
            {
                this.set = set;
            }

            public void Handle(AddMenuCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(MenuAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateMenuAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IMenuCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _menuByID = set._menuByID;
                var menuRepository = host.GetRequiredService<IRepository<Menu>>();
                if (!input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                MenuState menu;
                if (host.MenuSet.TryGetMenu(input.Id.Value, out menu))
                {
                    throw new ValidationException("给定标识的实体已经存在" + input.Id);
                }
                if (input.ParentID.HasValue)
                {
                    MenuState parentMenu;
                    if (!host.MenuSet.TryGetMenu(input.ParentID.Value, out parentMenu))
                    {
                        throw new NotExistException("标识为" + input.ParentID.Value + "的父菜单不存在");
                    }
                }

                var entity = Menu.Create(input);

                lock (this)
                {

                    if (host.MenuSet.TryGetMenu(input.Id.Value, out menu))
                    {
                        throw new ValidationException("给定标识的实体已经存在" + input.Id);
                    }
                    if (input.ParentID.HasValue)
                    {
                        MenuState parentMenu;
                        if (!host.MenuSet.TryGetMenu(input.ParentID.Value, out parentMenu))
                        {
                            throw new NotExistException("标识为" + input.ParentID.Value + "的父菜单不存在");
                        }
                    }
                    var menuState = MenuState.Create(host, entity);
                    if (!_menuByID.ContainsKey(entity.Id))
                    {
                        _menuByID.Add(entity.Id, menuState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            menuRepository.Add(entity);
                            menuRepository.Context.Commit();
                        }
                        catch
                        {
                            if (_menuByID.ContainsKey(entity.Id))
                            {
                                _menuByID.Remove(entity.Id);
                            }
                            menuRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateMenuAddedEvent(entity, input));
                }
            }

            private class PrivateMenuAddedEvent : MenuAddedEvent
            {
                public PrivateMenuAddedEvent(MenuBase source, IMenuCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateMenuCommand message)
            {
                this.Handle(message.Input, true);
            }

            public void Handle(MenuUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateMenuUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, false);
            }

            private void Handle(IMenuUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _menuByID = set._menuByID;
                var menuRepository = host.GetRequiredService<IRepository<Menu>>();
                MenuState bkState;
                if (!host.MenuSet.TryGetMenu(input.Id, out bkState))
                {
                    throw new ValidationException("给定标识的菜单不存在" + input.Id);
                }
                Menu entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    MenuState oldState;
                    if (!host.MenuSet.TryGetMenu(input.Id, out oldState))
                    {
                        throw new ValidationException("给定标识的菜单不存在" + input.Id);
                    }
                    entity = menuRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = MenuState.Create(host, entity);
                    stateChanged = newState != bkState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            menuRepository.Update(entity);
                            menuRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            menuRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand && stateChanged)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateMenuUpdatedEvent(entity, input));
                }
            }

            private class PrivateMenuUpdatedEvent : MenuUpdatedEvent
            {
                public PrivateMenuUpdatedEvent(MenuBase source, IMenuUpdateInput input)
                    : base(source, input)
                {

                }
            }

            private void Update(MenuState state)
            {
                var host = set.host;
                var _menuByID = set._menuByID;
                _menuByID[state.Id] = state;
            }

            private void RecChildMenus(Guid menuID, List<IMenu> menus)
            {
                var host = set.host;
                var _menuByID = set._menuByID;
                foreach (var menu in host.MenuSet)
                {
                    if (menu.ParentID == menuID)
                    {
                        menus.Add(menu);
                        RecChildMenus(menu.Id, menus);
                    }
                }
            }
            public void Handle(RemoveMenuCommand message)
            {
                this.Handle(message.EntityID, true);
            }

            public void Handle(MenuRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateMenuRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, false);
            }

            private void Handle(Guid menuID, bool isCommand)
            {
                var host = set.host;
                var _menuByID = set._menuByID;
                var menuRepository = host.GetRequiredService<IRepository<Menu>>();
                MenuState bkState;
                if (!host.MenuSet.TryGetMenu(menuID, out bkState))
                {
                    return;
                }
                Menu entity;
                lock (bkState)
                {
                    MenuState state;
                    if (!host.MenuSet.TryGetMenu(menuID, out state))
                    {
                        return;
                    }
                    entity = menuRepository.GetByKey(menuID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (entity.AllowDelete.HasValue && entity.AllowDelete.Value == 0)
                    {
                        throw new ValidationException("该菜单不允许删除");
                    }
                    if (menuRepository.FindAll().Any(a => a.ParentID.HasValue && a.ParentID.Value == entity.Id))
                    {
                        throw new ValidationException("不能删除父级菜单");
                    }
                    if (_menuByID.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new MenuRemovingEvent(entity));
                        }
                        _menuByID.Remove(bkState.Id);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            menuRepository.Remove(entity);
                            menuRepository.Context.Commit();
                        }
                        catch
                        {
                            if (!_menuByID.ContainsKey(bkState.Id))
                            {
                                _menuByID.Add(bkState.Id, bkState);
                            }
                            menuRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateMenuRemovedEvent(entity));
                }
            }

            private class PrivateMenuRemovedEvent : MenuRemovedEvent
            {
                public PrivateMenuRemovedEvent(MenuBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion
    }
}