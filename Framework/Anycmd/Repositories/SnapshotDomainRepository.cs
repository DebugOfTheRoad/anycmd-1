using Anycmd.Bus;
using Anycmd.Snapshots;
using Anycmd.Specifications;
using Anycmd.Storage;
using System;
using System.Transactions;

namespace Anycmd.Repositories
{
    using Model;

    /// <summary>
    /// Represents the domain repository that uses the snapshots to perform
    /// repository operations and publishes the domain events to the specified
    /// event bus.
    /// </summary>
    public class SnapshotDomainRepository : EventPublisherDomainRepository
    {
        #region Private Fields
        private readonly IStorage storage;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>SnapshotDomainRepository</c> class.
        /// </summary>
        /// <param name="storage">The <see cref="Anycmd.Storage.IStorage"/> instance that is used
        /// by the current domain repository to manipulate snapshot data.</param>
        /// <param name="eventBus">The <see cref="Anycmd.Bus.IEventBus"/> instance to which
        /// the domain events are published.</param>
        public SnapshotDomainRepository(IStorage storage, IEventBus eventBus)
            : base(eventBus)
        {
            this.storage = storage;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Commits the changes registered in the domain repository.
        /// </summary>
        protected override void DoCommit()
        {
            foreach (ISourcedAggregateRoot aggregateRoot in this.SaveHash)
            {
                SnapshotDataObject snapshotDataObject = SnapshotDataObject.CreateFromAggregateRoot(aggregateRoot);
                var aggregateRootId = aggregateRoot.Id;
                var aggregateRootType = aggregateRoot.GetType().AssemblyQualifiedName;
                ISpecification<SnapshotDataObject> spec = Specification<SnapshotDataObject>.Eval(p => p.AggregateRootID == aggregateRootId && p.AggregateRootType == aggregateRootType);
                var firstMatch = this.storage.SelectFirstOnly<SnapshotDataObject>(spec);
                if (firstMatch != null)
                    this.storage.Update<SnapshotDataObject>(new PropertyBag(snapshotDataObject), spec);
                else
                    this.storage.Insert<SnapshotDataObject>(new PropertyBag(snapshotDataObject));
                foreach (var evnt in aggregateRoot.UncommittedEvents)
                {
                    this.EventBus.Publish(evnt);
                }
            }
            if (this.DistributedTransactionSupported)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    this.storage.Commit();
                    this.EventBus.Commit();
                    ts.Complete();
                }
            }
            else
            {
                this.storage.Commit();
                this.EventBus.Commit();
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
                this.storage.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="Anycmd.Storage.IStorage"/> instance that is used
        /// by the current domain repository to manipulate snapshot data.
        /// </summary>
        public IStorage Storage
        {
            get { return this.storage; }
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
            string aggregateRootType = typeof(TAggregateRoot).AssemblyQualifiedName;
            ISpecification<SnapshotDataObject> spec = Specification<SnapshotDataObject>.Eval(p => p.AggregateRootID == id && p.AggregateRootType == aggregateRootType);
            SnapshotDataObject snapshotDataObject = this.storage.SelectFirstOnly<SnapshotDataObject>(spec);
            if (snapshotDataObject == null)
                throw new RepositoryException("The aggregate (id={0}) cannot be found in the domain repository.", id);
            ISnapshot snapshot = snapshotDataObject.ExtractSnapshot();
            TAggregateRoot aggregateRoot = this.CreateAggregateRootInstance<TAggregateRoot>();
            aggregateRoot.BuildFromSnapshot(snapshot);
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
            get { return this.storage.DistributedTransactionSupported && base.DistributedTransactionSupported; }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            base.Rollback();
            this.storage.Rollback();
        }
        #endregion
    }
}
