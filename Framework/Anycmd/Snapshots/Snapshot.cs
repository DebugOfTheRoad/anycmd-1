
using System;

namespace Anycmd.Snapshots
{
    /// <summary>
    /// Represents the snapshot.
    /// </summary>
    [Serializable]
    public abstract class Snapshot : ISnapshot
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>Snapshot</c> class.
        /// </summary>
        public Snapshot() { }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the version of the snapshot.
        /// </summary>
        public long Version { get; set; }
        /// <summary>
        /// Gets or sets the branch of the snapshot.
        /// </summary>
        public long Branch { get; set; }
        /// <summary>
        /// Gets or sets the timestamp on which the snapshot was taken.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the aggregate root which the
        /// snapshot represents.
        /// </summary>
        public Guid AggregateRootID { get; set; }
        #endregion
    }
}
