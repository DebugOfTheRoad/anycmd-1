
namespace Anycmd.Bus
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the message dispatcher.
    /// </summary>
    public class MessageDispatcher : IMessageDispatcher
    {
        #region Private Fields
        private readonly Dictionary<Type, List<dynamic>> handlers = new Dictionary<Type, List<dynamic>>();
        #endregion

        #region Protected Methods
        /// <summary>
        /// Occurs when the message dispatcher is going to dispatch a message.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDispatching(MessageDispatchEventArgs e)
        {
            var temp = this.Dispatching;
            if (temp != null)
                temp(this, e);
        }
        /// <summary>
        /// Occurs when the message dispatcher failed to dispatch a message.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDispatchFailed(MessageDispatchEventArgs e)
        {
            var temp = this.DispatchFailed;
            if (temp != null)
                temp(this, e);
        }
        /// <summary>
        /// Occurs when the message dispatcher has finished dispatching the message.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDispatched(MessageDispatchEventArgs e)
        {
            var temp = this.Dispatched;
            if (temp != null)
                temp(this, e);
        }
        #endregion

        #region IMessageDispatcher Members
        /// <summary>
        /// Clears the registration of the message handlers.
        /// </summary>
        public virtual void Clear()
        {
            handlers.Clear();
        }
        /// <summary>
        /// Dispatches the message.
        /// </summary>
        /// <param name="message">The message to be dispatched.</param>
        public virtual void DispatchMessage<T>(T message) where T : IMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            Type messageType = typeof(T);
            if (!messageType.IsPublic)
            {
                messageType = messageType.BaseType;
            }
            if (handlers.ContainsKey(messageType))
            {
                var messageHandlers = handlers[messageType];
                foreach (var messageHandler in messageHandlers)
                {
                    var dynMessageHandler = (IHandler<T>)messageHandler;
                    var evtArgs = new MessageDispatchEventArgs(message, messageHandler.GetType(), messageHandler);
                    this.OnDispatching(evtArgs);
                    try
                    {
                        dynMessageHandler.Handle(message);
                        this.OnDispatched(evtArgs);
                    }
                    catch
                    {
                        this.OnDispatchFailed(evtArgs);
                        // 原代码没有下面一行，从而吞掉了异常。是否有原因？
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Registers a message handler into message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handler">The handler to be registered.</param>
        public virtual void Register<T>(IHandler<T> handler) where T : IMessage
        {
            Type keyType = typeof(T);

            if (handlers.ContainsKey(keyType))
            {
                var registeredHandlers = handlers[keyType];
                if (registeredHandlers != null)
                {
                    if (!registeredHandlers.Contains(handler))
                        registeredHandlers.Add(handler);
                }
                else
                {
                    registeredHandlers = new List<dynamic>();
                    registeredHandlers.Add(handler);
                    handlers.Add(keyType, registeredHandlers);

                }
            }
            else
            {
                var registeredHandlers = new List<dynamic>();
                registeredHandlers.Add(handler);
                handlers.Add(keyType, registeredHandlers);
            }
        }
        /// <summary>
        /// Unregisters a message handler from the message dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handler">The handler to be registered.</param>
        public virtual void UnRegister<T>(IHandler<T> handler) where T : IMessage
        {
            var keyType = typeof(T);
            if (handlers.ContainsKey(keyType) &&
                handlers[keyType] != null &&
                handlers[keyType].Count > 0 &&
                handlers[keyType].Contains(handler))
            {
                handlers[keyType].Remove(handler);
            }
        }
        /// <summary>
        /// Occurs when the message dispatcher is going to dispatch a message.
        /// </summary>
        public event EventHandler<MessageDispatchEventArgs> Dispatching;
        /// <summary>
        /// Occurs when the message dispatcher failed to dispatch a message.
        /// </summary>
        public event EventHandler<MessageDispatchEventArgs> DispatchFailed;
        /// <summary>
        /// Occurs when the message dispatcher has finished dispatching the message.
        /// </summary>
        public event EventHandler<MessageDispatchEventArgs> Dispatched;

        #endregion
    }
}
