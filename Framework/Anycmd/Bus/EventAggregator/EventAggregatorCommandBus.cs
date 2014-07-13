
namespace Anycmd.Bus.EventAggregator
{
    using Events;

    /// <summary>
    /// Represents the command bus implemented by using the event aggregator.
    /// </summary>
    public sealed class EventAggregatorCommandBus : EventAggregatorBus, ICommandBus
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>EventAggregatorCommandBus</c> class.
        /// </summary>
        public EventAggregatorCommandBus(IEventAggregator eventAggregator) : base(eventAggregator) { }
        #endregion
    }
}
