﻿
using System;

namespace Anycmd.Snapshots
{
    /// <summary>
    /// Represents that the implemented classes are snapshots.
    /// </summary>
    public interface ISnapshot
    {
        /// <summary>
        /// Gets or sets the timestamp on which the snapshot was taken.
        /// </summary>
        DateTime Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the aggregate root which the
        /// snapshot represents.
        /// </summary>
        Guid AggregateRootID { get; set; }
        /// <summary>
        /// Gets or sets the version of the snapshot, which commonly would
        /// be the version of the aggregate root.
        /// </summary>
        long Version { get; set; }
        /// <summary>
        /// Gets or sets the branch on which the snapshot exists.
        /// </summary>
        long Branch { get; set; }
    }
}
