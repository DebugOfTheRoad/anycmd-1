
namespace Anycmd.Snapshots.Providers
{
    /// <summary>
    /// Represents the snapshot provider option.
    /// </summary>
    public enum SnapshotProviderOption
    {
        /// <summary>
        /// Indicates that immediate snapshot create/update should
        /// be performed.
        /// </summary>
        Immediate,
        /// <summary>
        /// Indicates that the creating/updating of the snapshots
        /// would be postponed to a later scenario. 
        /// </summary>
        Postpone
    }
}
