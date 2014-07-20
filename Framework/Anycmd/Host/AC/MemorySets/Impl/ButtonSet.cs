
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

    public sealed class ButtonSet : IButtonSet
    {
        public static readonly IButtonSet Empty = new ButtonSet(AppHost.Empty);

        private readonly Dictionary<Guid, ButtonState> _dicByID = new Dictionary<Guid, ButtonState>();
        private readonly Dictionary<string, ButtonState> _dicByCode = new Dictionary<string, ButtonState>(StringComparer.OrdinalIgnoreCase);
        private bool _initialized = false;
        private readonly IAppHost host;

        private readonly Guid _id = Guid.NewGuid();

        public Guid Id
        {
            get { return _id; }
        }

        public ButtonSet(IAppHost host)
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
            messageDispatcher.Register((IHandler<AddButtonCommand>)handler);
            messageDispatcher.Register((IHandler<ButtonAddedEvent>)handler);
            messageDispatcher.Register((IHandler<UpdateButtonCommand>)handler);
            messageDispatcher.Register((IHandler<ButtonUpdatedEvent>)handler);
            messageDispatcher.Register((IHandler<RemoveButtonCommand>)handler);
            messageDispatcher.Register((IHandler<ButtonRemovedEvent>)handler);
        }

        public bool ContainsButton(Guid buttonID)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.ContainsKey(buttonID);
        }

        public bool ContainsButton(string buttonCode)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByCode.ContainsKey(buttonCode);
        }

        public bool TryGetButton(Guid buttonID, out ButtonState button)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByID.TryGetValue(buttonID, out button);
        }

        public bool TryGetButton(string code, out ButtonState button)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dicByCode.TryGetValue(code, out button);
        }

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
                        _dicByCode.Clear();
                        var buttons = host.GetRequiredService<IAppHostBootstrap>().GetAllButtons().ToList();
                        foreach (var button in buttons)
                        {
                            if (!(button is ButtonBase))
                            {
                                throw new CoreException(button.GetType().Name + "必须继承" + typeof(ButtonBase).Name);
                            }
                            if (_dicByID.ContainsKey(button.Id))
                            {
                                throw new CoreException("意外重复的按钮标识");
                            }
                            if (_dicByCode.ContainsKey(button.Code))
                            {
                                throw new CoreException("意外重复的按钮编码");
                            }
                            var buttonState = ButtonState.Create(button);
                            _dicByID.Add(button.Id, buttonState);
                            _dicByCode.Add(button.Code, buttonState);
                        }
                        _initialized = true;
                    }
                }
            }
        }

        public IEnumerator<ButtonState> GetEnumerator()
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

        #region MessageHandler
        private class MessageHandler :
            IHandler<UpdateButtonCommand>, 
            IHandler<AddButtonCommand>, 
            IHandler<ButtonAddedEvent>, 
            IHandler<ButtonUpdatedEvent>, 
            IHandler<RemoveButtonCommand>, 
            IHandler<ButtonRemovedEvent>
        {
            private readonly ButtonSet set;

            public MessageHandler(ButtonSet set)
            {
                this.set = set;
            }

            public void Handle(AddButtonCommand message)
            {
                this.Handle(message.Input, isCommand: true);
            }

            public void Handle(ButtonAddedEvent message)
            {
                if (message.GetType() == typeof(PrivateButtonAddedEvent))
                {
                    return;
                }
                this.Handle(message.Input, isCommand: false);
            }

            private void Handle(IButtonCreateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var buttonRepository = host.GetRequiredService<IRepository<Button>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                Button entity;
                lock (this)
                {
                    if (!input.Id.HasValue || host.ButtonSet.ContainsButton(input.Id.Value))
                    {
                        throw new CoreException("意外的按钮标识");
                    }
                    if (host.ButtonSet.ContainsButton(input.Code))
                    {
                        throw new ValidationException("重复的按钮编码");
                    }

                    entity = Button.Create(input);

                    var buttonState = ButtonState.Create(entity);
                    if (!_dicByID.ContainsKey(entity.Id))
                    {
                        _dicByID.Add(entity.Id, buttonState);
                    }
                    if (!_dicByCode.ContainsKey(entity.Code))
                    {
                        _dicByCode.Add(entity.Code, buttonState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            buttonRepository.Add(entity);
                            buttonRepository.Context.Commit();
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
                            buttonRepository.Context.Rollback();
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateButtonAddedEvent(entity, input));
                }
            }

            private class PrivateButtonAddedEvent : ButtonAddedEvent
            {
                public PrivateButtonAddedEvent(ButtonBase source, IButtonCreateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(UpdateButtonCommand message)
            {
                this.Handle(message.Input, isCommand: true);
            }

            public void Handle(ButtonUpdatedEvent message)
            {
                if (message.GetType() == typeof(PrivateButtonUpdatedEvent))
                {
                    return;
                }
                this.Handle(message.Input, isCommand: false);
            }

            private void Handle(IButtonUpdateInput input, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var buttonRepository = host.GetRequiredService<IRepository<Button>>();
                if (string.IsNullOrEmpty(input.Code))
                {
                    throw new ValidationException("编码不能为空");
                }
                ButtonState bkState;
                if (!host.ButtonSet.TryGetButton(input.Id, out bkState))
                {
                    throw new NotExistException("意外的按钮标识" + input.Id);
                }
                Button entity;
                bool stateChanged = false;
                lock (bkState)
                {
                    ButtonState oldState;
                    if (!host.ButtonSet.TryGetButton(input.Id, out oldState))
                    {
                        throw new NotExistException("意外的按钮标识" + input.Id);
                    }
                    ButtonState button;
                    if (host.ButtonSet.TryGetButton(input.Code, out button) && button.Id != input.Id)
                    {
                        throw new ValidationException("重复的按钮编码");
                    }
                    entity = buttonRepository.GetByKey(input.Id);
                    if (entity == null)
                    {
                        throw new NotExistException();
                    }

                    entity.Update(input);

                    var newState = ButtonState.Create(entity);
                    stateChanged = bkState != newState;
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                    if (isCommand)
                    {
                        try
                        {
                            buttonRepository.Update(entity);
                            buttonRepository.Context.Commit();
                        }
                        catch
                        {
                            if (stateChanged)
                            {
                                Update(bkState);
                            }
                            buttonRepository.Context.Rollback();
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
                    host.MessageDispatcher.DispatchMessage(new PrivateButtonUpdatedEvent(entity, input));
                }
            }

            private void Update(ButtonState state)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                string oldKey = _dicByID[state.Id].Code;
                string newKey = state.Code;
                _dicByID[state.Id] = state;
                // 如果按钮编码改变了
                if (!_dicByCode.ContainsKey(newKey))
                {
                    _dicByCode.Remove(oldKey);
                    _dicByCode.Add(newKey, state);
                }
                else
                {
                    _dicByCode[newKey] = state;
                }
            }

            private class PrivateButtonUpdatedEvent : ButtonUpdatedEvent
            {
                public PrivateButtonUpdatedEvent(ButtonBase source, IButtonUpdateInput input)
                    : base(source, input)
                {

                }
            }
            public void Handle(RemoveButtonCommand message)
            {
                this.Handle(message.EntityID, isCommand: true);
            }

            public void Handle(ButtonRemovedEvent message)
            {
                if (message.GetType() == typeof(PrivateButtonRemovedEvent))
                {
                    return;
                }
                this.Handle(message.Source.Id, isCommand: false);
            }

            private void Handle(Guid buttonID, bool isCommand)
            {
                var host = set.host;
                var _dicByID = set._dicByID;
                var _dicByCode = set._dicByCode;
                var buttonRepository = host.GetRequiredService<IRepository<Button>>();
                var pageButtonRepository = host.GetRequiredService<IRepository<PageButton>>();
                ButtonState bkState;
                if (!host.ButtonSet.TryGetButton(buttonID, out bkState))
                {
                    return;
                }
                if (host.PageSet.GetPageButtons().Any(a => a.ButtonID == buttonID))
                {
                    throw new ValidationException("按钮关联页面后不能删除");
                }
                Button entity;
                lock (bkState)
                {
                    ButtonState state;
                    if (!host.ButtonSet.TryGetButton(buttonID, out state))
                    {
                        return;
                    }
                    entity = buttonRepository.GetByKey(buttonID);
                    if (entity == null)
                    {
                        return;
                    }
                    if (_dicByID.ContainsKey(bkState.Id))
                    {
                        if (isCommand)
                        {
                            host.MessageDispatcher.DispatchMessage(new ButtonRemovingEvent(entity));
                        }
                        _dicByID.Remove(bkState.Id);
                        if (_dicByCode.ContainsKey(bkState.Code))
                        {
                            _dicByCode.Remove(bkState.Code);
                        }
                    }
                    if (isCommand)
                    {
                        try
                        {
                            buttonRepository.Remove(entity);
                            buttonRepository.Context.Commit();
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
                            throw;
                        }
                    }
                }
                if (isCommand)
                {
                    host.MessageDispatcher.DispatchMessage(new PrivateButtonRemovedEvent(entity));
                }
            }

            private class PrivateButtonRemovedEvent : ButtonRemovedEvent
            {
                public PrivateButtonRemovedEvent(ButtonBase source)
                    : base(source)
                {

                }
            }
        }
        #endregion
    }
}
