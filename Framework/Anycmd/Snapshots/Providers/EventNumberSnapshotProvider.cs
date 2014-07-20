
namespace Anycmd.Snapshots.Providers
{
    using Events.Storage;
    using Model;
    using Specifications;
    using Storage;
    using System;
    using System.Collections.Generic;
    using Util;

    /// <summary>
    /// Represents the snapshot provider that takes the number of stored domain events
    /// as the strategy to implement its snapshot functionalities.
    /// </summary>
    public class EventNumberSnapshotProvider : StorageBasedSnapshotProvider
    {
        #region EventNumberSnapshotMappingKey class
        /// <summary>
        /// Represents the class that is used as the dictionary key when
        /// maintaining the snapshot mappings.
        /// </summary>
        sealed class EventNumberSnapshotMappingKey : IEquatable<EventNumberSnapshotMappingKey>
        {
            #region Private Fields
            private readonly string typeName;
            private readonly Guid id;
            #endregion

            #region Ctor
            /// <summary>
            /// Initializes a new instance of <c>EventNumberSnapshotMappingKey</c> class.
            /// </summary>
            /// <param name="typeName">The full name of the aggregate root type.</param>
            /// <param name="id">The aggregate root id.</param>
            public EventNumberSnapshotMappingKey(string typeName, Guid id)
            {
                this.typeName = typeName;
                this.id = id;
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// Gets the full name of the aggregate root type.
            /// </summary>
            public string TypeName
            {
                get { return typeName; }
            }
            /// <summary>
            /// Gets the aggregate root id.
            /// </summary>
            public Guid Id
            {
                get { return id; }
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// Checks whether the given object is equal to this object.
            /// </summary>
            /// <param name="obj">The given object to be checked.</param>
            /// <returns>True if the two are equal, otherwise false.</returns>
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (this.GetType() != obj.GetType())
                    return false;
                return this.Equals(obj as EventNumberSnapshotMappingKey);
            }
            /// <summary>
            /// Gets the hash code of the current object.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return ReflectionHelper.GetHashCode(this.id.GetHashCode(), this.typeName.GetHashCode());
            }
            /// <summary>
            /// Gets the string presentation of the current object.
            /// </summary>
            /// <returns>The string presentation.</returns>
            public override string ToString()
            {
                return string.Format("{0}, {1}", this.typeName, this.id);
            }
            #endregion

            #region Operator Overrides
            public static bool operator ==(EventNumberSnapshotMappingKey a, EventNumberSnapshotMappingKey b)
            {
                if (object.ReferenceEquals(a, b))
                    return true;
                if (((object)a == null) || ((object)b == null))
                    return false;
                return a.Id == b.Id && a.TypeName == b.TypeName;
            }
            public static bool operator !=(EventNumberSnapshotMappingKey a, EventNumberSnapshotMappingKey b)
            {
                return !(a == b);
            }
            #endregion

            #region IEquatable<EventNumberSnapshotMappingKey> Members
            /// <summary>
            /// Checks whether the given object is equal to this object.
            /// </summary>
            /// <param name="other">The given object to be checked.</param>
            /// <returns>True if the two are equal, otherwise false.</returns>
            public bool Equals(EventNumberSnapshotMappingKey other)
            {
                if (object.ReferenceEquals(this, other))
                    return true;
                if (other == null)
                    return false;
                return this.id == other.Id && this.typeName == other.TypeName;
            }
            #endregion
        }
        #endregion

        #region Private Fields
        private readonly int numOfEvents;
        private readonly Dictionary<EventNumberSnapshotMappingKey, ISnapshot> snapshotMapping = new Dictionary<EventNumberSnapshotMappingKey, ISnapshot>();
        private readonly IAppHost host;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>EventNumberSnapshotProvider</c> class.
        /// </summary>
        /// <param name="eventStorage">The instance of the event storage that is used for initializing the <c>EventNumberSnapshotProvider</c> class.</param>
        /// <param name="snapshotStorage">The instance of the snapshot storage this is used for initializing the <c>EventNumberSnapshotProvider</c> class.</param>
        /// <param name="option">The snapshot provider option.</param>
        /// <param name="numOfEvents">The maximum number of events.</param>
        public EventNumberSnapshotProvider(IAppHost host, IStorage eventStorage, IStorage snapshotStorage, SnapshotProviderOption option, int numOfEvents)
            : base(eventStorage, snapshotStorage, option)
        {
            this.host = host;
            this.numOfEvents = numOfEvents;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the maximum number of events.
        /// </summary>
        public int NumberOfEvents
        {
            get { return this.numOfEvents; }
        }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work supports MS-DTC.
        /// </summary>
        public override bool DistributedTransactionSupported
        {
            get { return this.SnapshotStorage.DistributedTransactionSupported; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.Committed)
                {
                    try
                    {
                        this.Commit();
                    }
                    catch
                    {
                        this.Rollback();
                        throw;
                    }
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value which indicates
        /// whether the snapshot should be created or updated for the given
        /// aggregate root.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root.</param>
        /// <returns>True if the snapshot should be created or updated, 
        /// otherwise false.</returns>
        public override bool CanCreateOrUpdateSnapshot(ISourcedAggregateRoot aggregateRoot)
        {
            if (this.HasSnapshot(aggregateRoot.GetType(), aggregateRoot.Id))
            {
                ISnapshot snapshot = this.GetSnapshot(aggregateRoot.GetType(), aggregateRoot.Id);
                return snapshot.Version + numOfEvents <= aggregateRoot.Version;
            }
            else
            {
                string aggregateRootTypeName = aggregateRoot.GetType().AssemblyQualifiedName;
                Guid aggregateRootId = aggregateRoot.Id;
                long version = aggregateRoot.Version;
                ISpecification<DomainEventDataObject> spec = Specification<DomainEventDataObject>.Eval(
                    p => p.SourceID == aggregateRootId &&
                        p.AssemblyQualifiedSourceType == aggregateRootTypeName &&
                        p.Version <= version);
                int eventCnt = this.EventStorage.GetRecordCount<DomainEventDataObject>(spec);
                return eventCnt >= this.numOfEvents;
            }
        }
        /// <summary>
        /// Creates or updates the snapshot for the given aggregate root.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root on which the snapshot is created or updated.</param>
        public override void CreateOrUpdateSnapshot(ISourcedAggregateRoot aggregateRoot)
        {
            ISnapshot snapshot = aggregateRoot.CreateSnapshot();
            SnapshotDataObject dataObj = host.CreateFromAggregateRoot(aggregateRoot);
            PropertyBag insertOrUpdateData = new PropertyBag(dataObj);
            var key = new EventNumberSnapshotMappingKey(aggregateRoot.GetType().AssemblyQualifiedName, aggregateRoot.Id);

            if (this.HasSnapshot(aggregateRoot.GetType(), aggregateRoot.Id))
            {
                string aggregateRootTypeName = aggregateRoot.GetType().AssemblyQualifiedName;
                Guid aggregateRootId = aggregateRoot.Id;
                ISpecification<SnapshotDataObject> spec = Specification<SnapshotDataObject>.Eval(
                    p => p.AggregateRootType == aggregateRootTypeName &&
                        p.AggregateRootID == aggregateRootId);
                this.SnapshotStorage.Update<SnapshotDataObject>(insertOrUpdateData, spec);
                this.Committed = false;
                if (snapshotMapping.ContainsKey(key))
                    snapshotMapping[key] = snapshot;
                else
                    snapshotMapping.Add(key, snapshot);
            }
            else
            {
                this.SnapshotStorage.Insert<SnapshotDataObject>(insertOrUpdateData);
                this.Committed = true;
                snapshotMapping.Add(key, snapshot);
            }
        }
        /// <summary>
        /// Gets the snapshot for the aggregate root with the given type and identifier.
        /// </summary>
        /// <param name="aggregateRootType">The type of the aggregate root.</param>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <returns>The snapshot instance.</returns>
        public override ISnapshot GetSnapshot(Type aggregateRootType, Guid id)
        {
            var key = new EventNumberSnapshotMappingKey(aggregateRootType.AssemblyQualifiedName, id);
            if (snapshotMapping.ContainsKey(key))
                return snapshotMapping[key];
            string aggregateRootTypeName = aggregateRootType.AssemblyQualifiedName;
            ISpecification<SnapshotDataObject> spec = Specification<SnapshotDataObject>.Eval(
                p => p.AggregateRootType == aggregateRootTypeName && p.AggregateRootID == id);
            SnapshotDataObject dataObj = this.SnapshotStorage.SelectFirstOnly<SnapshotDataObject>(spec);
            if (dataObj == null)
                return null;
            ISnapshot snapshot = host.ExtractSnapshot(dataObj);
            this.snapshotMapping.Add(key, snapshot);
            return snapshot;
        }
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value which indicates whether the snapshot
        /// exists for the aggregate root with the given type and identifier.
        /// </summary>
        /// <param name="aggregateRootType">The type of the aggregate root.</param>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <returns>True if the snapshot exists, otherwise false.</returns>
        public override bool HasSnapshot(Type aggregateRootType, Guid id)
        {
            var key = new EventNumberSnapshotMappingKey(aggregateRootType.AssemblyQualifiedName, id);
            if (snapshotMapping.ContainsKey(key))
                return true;
            string aggregateRootTypeName = aggregateRootType.AssemblyQualifiedName;
            ISpecification<SnapshotDataObject> spec = Specification<SnapshotDataObject>.Eval(
                p => p.AggregateRootType == aggregateRootTypeName && p.AggregateRootID == id);
            int snapshotRecordCnt = this.SnapshotStorage.GetRecordCount<SnapshotDataObject>(spec);
            if (snapshotRecordCnt > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void Commit()
        {
            this.SnapshotStorage.Commit();
            this.Committed = true;
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            this.SnapshotStorage.Rollback();
            this.Committed = false;
        }
        #endregion
    }
}
