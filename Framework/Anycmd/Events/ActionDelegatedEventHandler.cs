﻿
using System;

namespace Anycmd.Events
{
    using Util;

    /// <summary>
    /// Represents the event handler which delegates the event handling process to
    /// a given <see cref="Action{T}"/> delegated method.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to be handled by current handler.</typeparam>
    public sealed class ActionDelegatedEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : class, IEvent
    {
        #region Private Fields
        private readonly Action<TEvent> action;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>ActionDelegatedEventHandler{TEvent}</c> class.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> instance that delegates the event handling process.</param>
        public ActionDelegatedEventHandler(Action<TEvent> action)
        {
            this.action = action;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <see cref="Boolean"/> value which indicates whether the current
        /// <c>ActionDelegatedEventHandler{T}</c> equals to the given object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> which is used to compare to
        /// the current <c>ActionDelegatedEventHandler{T}</c> instance.</param>
        /// <returns>If the given object equals to the current <c>ActionDelegatedEventHandler{T}</c>
        /// instance, returns true, otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            ActionDelegatedEventHandler<TEvent> other = obj as ActionDelegatedEventHandler<TEvent>;
            if (other == null)
                return false;
            return Delegate.Equals(this.action, other.action);
        }
        /// <summary>
        /// Gets the hash code of the current <c>ActionDelegatedEventHandler{T}</c> instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return ReflectionHelper.GetHashCode(this.action.GetHashCode());
        }
        #endregion

        #region IHandler<TDomainEvent> Members
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        public void Handle(TEvent message)
        {
            action(message);
        }

        #endregion
    }
}