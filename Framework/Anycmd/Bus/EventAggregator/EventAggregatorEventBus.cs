
namespace Anycmd.Bus.EventAggregator
{
    using Anycmd.Events;

    /// <summary>
    /// Represents the command bus implemented by using the event aggregator.
    /// </summary>
    public sealed class EventAggregatorEventBus : EventAggregatorBus, IEventBus
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>EventAggregatorEventBus</c> class.
        /// </summary>
        public EventAggregatorEventBus(IEventAggregator eventAggregator) : base(eventAggregator) { }
        #endregion
    }
}
