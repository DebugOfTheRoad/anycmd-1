
namespace Anycmd.Snapshots
{
    /// <summary>
    /// Represents that the implemented classes are snapshot originators.
    /// </summary>
    public interface ISnapshotOrignator
    {
        /// <summary>
        /// Builds the originator from the specific snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot from which the originator is built.</param>
        void BuildFromSnapshot(ISnapshot snapshot);
        /// <summary>
        /// Creates the snapshot.
        /// </summary>
        /// <returns>The snapshot that was created based on the current originator.</returns>
        ISnapshot CreateSnapshot();
    }
}
