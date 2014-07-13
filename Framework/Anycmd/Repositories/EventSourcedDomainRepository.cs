
using Anycmd.Bus;
using Anycmd.Events;
using Anycmd.Events.Storage;
using Anycmd.Snapshots;
using Anycmd.Snapshots.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Anycmd.Repositories
{
    using Model;

    /// <summary>
    /// Represents the domain repository that supports the event sourcing facilities.
    /// </summary>
    public class EventSourcedDomainRepository : EventPublisherDomainRepository
    {
        #region Private Fields
        private readonly IDomainEventStorage domainEventStorage;
        private readonly ISnapshotProvider snapshotProvider;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>EventSourcedDomainRepository</c> class.
        /// </summary>
        /// <param name="domainEventStorage">The <see cref="Anycmd.Events.Storage.IDomainEventStorage"/> instance
        /// that handles the storage mechanism for domain events.</param>
        /// <param name="eventBus">The <see cref="Anycmd.Bus.IEventBus"/> instance to which the domain events
        /// are published.</param>
        /// <param name="snapshotProvider">The <see cref="Anycmd.Snapshots.Providers.ISnapshotProvider"/> instance
        /// that is used for handling the snapshot operations.</param>
        public EventSourcedDomainRepository(IDomainEventStorage domainEventStorage, IEventBus eventBus, ISnapshotProvider snapshotProvider)
            : base(eventBus)
        {
            this.domainEventStorage = domainEventStorage;
            this.snapshotProvider = snapshotProvider;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Commits the changes registered in the domain repository.
        /// </summary>
        protected override void DoCommit()
        {
            // firstly we save and publish the event via domain event storage
            // and the event bus.
            foreach (ISourcedAggregateRoot aggregateRoot in this.SaveHash)
            {
                if (this.snapshotProvider != null && this.snapshotProvider.Option == SnapshotProviderOption.Immediate)
                {
                    if (this.snapshotProvider.CanCreateOrUpdateSnapshot(aggregateRoot))
                    {
                        this.snapshotProvider.CreateOrUpdateSnapshot(aggregateRoot);
                    }
                }
                IEnumerable<IDomainEvent> events = aggregateRoot.UncommittedEvents;
                foreach (var evt in events)
                {
                    domainEventStorage.SaveEvent(evt);
                    this.EventBus.Publish(evt);
                }
            }
            // then commit the save/publish via UoW.
            if (this.DistributedTransactionSupported)
            {
                // the distributed transaction is supported either by domain event storage
                // or by the event bus. use the MS-DTC (Distributed Transaction Coordinator)
                // to commit the transaction. This solves the 2PC for deivces that are
                // distributed transaction compatible.
                using (TransactionScope ts = new TransactionScope())
                {
                    domainEventStorage.Commit();
                    this.EventBus.Commit();
                    if (this.snapshotProvider != null && this.snapshotProvider.Option == SnapshotProviderOption.Immediate)
                    {
                        this.snapshotProvider.Commit();
                    }
                    ts.Complete();
                }
            }
            else
            {
                domainEventStorage.Commit();
                this.EventBus.Commit();
                if (this.snapshotProvider != null && this.snapshotProvider.Option == SnapshotProviderOption.Immediate)
                {
                    this.snapshotProvider.Commit();
                }
            }
        }
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
                domainEventStorage.Dispose();
                if (snapshotProvider != null)
                    snapshotProvider.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the instance of the domain event storage which is used for storing domain events.
        /// </summary>
        public IDomainEventStorage DomainEventStorage
        {
            get { return this.domainEventStorage; }
        }
        /// <summary>
        /// Gets the <see cref="Anycmd.Snapshots.Providers.ISnapshotProvider"/> instance
        /// that is used for handling the snapshot operations.
        /// </summary>
        public ISnapshotProvider SnapshotProvider
        {
            get { return this.snapshotProvider; }
        }

        #endregion

        #region IDomainRepository Members
        /// <summary>
        /// Gets the instance of the aggregate root with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <returns>The instance of the aggregate root with the specified identifier.</returns>
        public override TAggregateRoot Get<TAggregateRoot>(Guid id)
        {
            TAggregateRoot aggregateRoot = this.CreateAggregateRootInstance<TAggregateRoot>();
            if (this.snapshotProvider != null && this.snapshotProvider.HasSnapshot(typeof(TAggregateRoot), id))
            {
                ISnapshot snapshot = snapshotProvider.GetSnapshot(typeof(TAggregateRoot), id);
                aggregateRoot.BuildFromSnapshot(snapshot);
                var eventsAfterSnapshot = this.domainEventStorage.LoadEvents(typeof(TAggregateRoot), id, snapshot.Version);
                if (eventsAfterSnapshot != null && eventsAfterSnapshot.Count() > 0)
                    aggregateRoot.BuildFromHistory(eventsAfterSnapshot);
            }
            else
            {
                aggregateRoot.Id = id;
                var evnts = this.domainEventStorage.LoadEvents(typeof(TAggregateRoot), id);
                if (evnts != null && evnts.Count() > 0)
                    aggregateRoot.BuildFromHistory(evnts);
                else
                    throw new RepositoryException("The aggregate (id={0}) cannot be found in the domain repository.", id);
            }
            return aggregateRoot;
        }
        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public override bool DistributedTransactionSupported
        {
            get { return domainEventStorage.DistributedTransactionSupported && base.DistributedTransactionSupported; }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            base.Rollback();
            domainEventStorage.Rollback();
            if (this.snapshotProvider != null && this.snapshotProvider.Option == SnapshotProviderOption.Immediate)
            {
                this.snapshotProvider.Rollback();
            }
        }
        #endregion
    }
}
