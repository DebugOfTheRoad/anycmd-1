
namespace Anycmd.Host.EDI.MemorySets
{
    using Anycmd.Repositories;
    using Bus;
    using Entities;
    using Exceptions;
    using Extensions;
    using Messages;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ProcesseSet : IProcesseSet
    {
        private readonly Dictionary<Guid, ProcessDescriptor> _dic = new Dictionary<Guid, ProcessDescriptor>();
        private bool _initialized = false;
        private object locker = new object();

        private readonly Guid _id = Guid.NewGuid();
        private readonly NodeHost host;

        public Guid Id
        {
            get { return _id; }
        }

        public ProcesseSet(NodeHost host)
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
            messageDispatcher.Register(new CreateProcessCommandHandler(this));
            messageDispatcher.Register(new UpdateProcessCommandHandler(this));
            messageDispatcher.Register(new DeleteProcessCommandHandler(this));
            messageDispatcher.Register(new ChangeProcessOrganizationCommandHandler(this));
            messageDispatcher.Register(new ChangeProcessNetPortCommandHandler(this));
        }

        /// <summary>
        /// 根据发送策略名索引发送策略
        /// </summary>
        /// <param name="processID">发送策略名</param>
        /// <exception cref="CoreException">当给定名称的发送策略不存在时引发</exception>
        /// <returns></returns>
        /// <exception cref="CoreException">当进程标识非法时抛出</exception>
        public ProcessDescriptor this[Guid processID]
        {
            get
            {
                if (!_initialized)
                {
                    Init();
                }
                if (!_dic.ContainsKey(processID))
                {
                    throw new CoreException("意外的进程标识");
                }

                return _dic[processID];
            }
        }

        public bool ContainsProcess(Guid processID)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dic.ContainsKey(processID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public bool TryGetProcess(Guid processID, out ProcessDescriptor process)
        {
            if (!_initialized)
            {
                Init();
            }
            return _dic.TryGetValue(processID, out process);
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
        public IEnumerator<ProcessDescriptor> GetEnumerator()
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
                        var processes = host.AppHost.GetRequiredService<INodeHostBootstrap>().GetProcesses();
                        foreach (var process in processes)
                        {
                            _dic.Add(process.Id, new ProcessDescriptor(ProcessState.Create(process)));
                        }
                        _initialized = true;
                    }
                }
            }
        }

        #region CreateProcessCommandHandler
        private class CreateProcessCommandHandler : IHandler<AddProcessCommand>
        {
            private readonly ProcesseSet processSet;

            public CreateProcessCommandHandler(ProcesseSet processSet)
            {
                this.processSet = processSet;
            }

            public void Handle(AddProcessCommand message)
            {
                var processRepository = processSet.host.AppHost.GetRequiredService<IRepository<Process>>();
                if (!message.Input.Id.HasValue)
                {
                    throw new ValidationException("标识是必须的");
                }
                if (NodeHost.Instance.Processs.ContainsProcess(message.Input.Id.Value))
                {
                    throw new ValidationException("给定标识标识的记录已经存在");
                }

                var entity = Process.Create(message.Input);

                lock (processSet.locker)
                {
                    if (!processSet._dic.ContainsKey(entity.Id))
                    {
                        processSet._dic.Add(entity.Id, new ProcessDescriptor(ProcessState.Create(entity)));
                    }
                    try
                    {
                        processRepository.Add(entity);
                        processRepository.Context.Commit();
                    }
                    catch
                    {
                        if (processSet._dic.ContainsKey(entity.Id))
                        {
                            processSet._dic.Remove(entity.Id);
                        }
                        processRepository.Context.Rollback();
                        throw;
                    }
                }
                processSet.host.AppHost.PublishEvent(new ProcessAddedEvent(entity));
                processSet.host.AppHost.CommitEventBus();
            }
        }
        #endregion

        #region UpdateProcessCommandHandler
        private class UpdateProcessCommandHandler : IHandler<UpdateProcessCommand>
        {
            private readonly ProcesseSet processSet;

            public UpdateProcessCommandHandler(ProcesseSet processSet)
            {
                this.processSet = processSet;
            }

            public void Handle(UpdateProcessCommand message)
            {
                var processRepository = processSet.host.AppHost.GetRequiredService<IRepository<Process>>();
                if (!NodeHost.Instance.Processs.ContainsProcess(message.Input.Id))
                {
                    throw new NotExistException();
                }
                var entity = processRepository.GetByKey(message.Input.Id);
                if (entity == null)
                {
                    throw new NotExistException();
                }
                var bkState = processSet._dic[entity.Id];

                entity.Update(message.Input);

                var newState = new ProcessDescriptor(ProcessState.Create(entity));
                bool stateChanged = newState != bkState;
                lock (processSet.locker)
                {
                    if (stateChanged)
                    {
                        Update(newState);
                    }
                }
                try
                {
                    processRepository.Update(entity);
                    processRepository.Context.Commit();
                }
                catch
                {
                    if (stateChanged)
                    {
                        Update(bkState);
                    }
                    processRepository.Context.Rollback();
                    throw;
                }
                if (stateChanged)
                {
                    processSet.host.AppHost.PublishEvent(new ProcessUpdatedEvent(entity));
                    processSet.host.AppHost.CommitEventBus();
                }
            }

            private void Update(ProcessDescriptor state)
            {
                processSet._dic[state.Process.Id] = state;
            }
        }
        #endregion

        #region DeleteProcessCommandHandler
        private class DeleteProcessCommandHandler : IHandler<RemoveProcessCommand>
        {
            private readonly ProcesseSet processSet;

            public DeleteProcessCommandHandler(ProcesseSet processSet)
            {
                this.processSet = processSet;
            }

            public void Handle(RemoveProcessCommand message)
            {
                // TODO:
            }
        }
        #endregion

        #region ChangeProcessOrganizationCommandHandler
        private class ChangeProcessOrganizationCommandHandler : IHandler<ChangeProcessOrganizationCommand>
        {
            private readonly ProcesseSet processSet;

            public ChangeProcessOrganizationCommandHandler(ProcesseSet processSet)
            {
                this.processSet = processSet;
            }

            public void Handle(ChangeProcessOrganizationCommand message)
            {
                // TODO:
            }
        }
        #endregion

        #region ChangeProcessNetPortCommandHandler
        private class ChangeProcessNetPortCommandHandler : IHandler<ChangeProcessNetPortCommand>
        {
            private readonly ProcesseSet processSet;

            public ChangeProcessNetPortCommandHandler(ProcesseSet processSet)
            {
                this.processSet = processSet;
            }

            public void Handle(ChangeProcessNetPortCommand message)
            {
                // TODO:
            }
        }
        #endregion
    }
}