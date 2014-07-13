using System;
using System.Collections.Generic;
using System.Linq;

namespace Anycmd.Repositories
{
    using Model;

    /// <summary>
    /// Represents the domain repository that uses the <see cref="Anycmd.Repositories.IRepositoryContext"/>
    /// and <see cref="Anycmd.Repositories.IRepository&lt;TAggregateRoot&gt;"/> instances to perform aggregate
    /// operations.
    /// </summary>
    public class RegularDomainRepository : DomainRepository
    {
        #region Private Fields
        private readonly IRepositoryContext context;
        private readonly HashSet<ISourcedAggregateRoot> dirtyHash = new HashSet<ISourcedAggregateRoot>();
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>RegularDomainRepository</c> class.
        /// </summary>
        /// <param name="context">The <see cref="Anycmd.Repositories.IRepositoryContext"/> instance to which the 
        /// <c>RegularDomainRepository</c> forwards the repository operations.</param>
        public RegularDomainRepository(IRepositoryContext context)
        {
            this.context = context;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="Anycmd.Repositories.IRepositoryContext"/> instance.
        /// </summary>
        public IRepositoryContext Context
        {
            get { return this.context; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Commits the changes registered in the domain repository.
        /// </summary>
        protected override void DoCommit()
        {
            foreach (var aggregateRootObj in this.SaveHash)
            {
                this.context.RegisterNew(aggregateRootObj);
            }
            foreach (var aggregateRootObj in this.dirtyHash)
            {
                this.context.RegisterModified(aggregateRootObj);
            }

            this.context.Commit();

            this.dirtyHash.ToList().ForEach(this.DelegatedUpdateAndClearAggregateRoot);
            this.dirtyHash.Clear();
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
                this.context.Dispose();
            }
            base.Dispose(disposing);
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
            var querySaveHash = from p in this.SaveHash
                                where p.Id.Equals(id)
                                select p;
            var queryDirtyHash = from p in this.dirtyHash
                                 where p.Id.Equals(id)
                                 select p;
            if (querySaveHash != null && querySaveHash.Count() > 0)
                return querySaveHash.FirstOrDefault() as TAggregateRoot;
            if (queryDirtyHash != null && queryDirtyHash.Count() > 0)
                return queryDirtyHash.FirstOrDefault() as TAggregateRoot;

            var result = context.Query<TAggregateRoot>().FirstOrDefault(ar => ar.Id.Equals(id));
            // Clears the aggregate root since version info is not needed in regular repositories.
            this.DelegatedUpdateAndClearAggregateRoot(result);
            return result;
        }
        /// <summary>
        /// Saves the aggregate represented by the specified aggregate root to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root that is going to be saved.</param>
        public override void Save<TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            if (context.Query<TAggregateRoot>().Any(ar => ar.Id.Equals(aggregateRoot.Id)))
            {
                if (!this.dirtyHash.Contains(aggregateRoot))
                    this.dirtyHash.Add(aggregateRoot);
                this.Committed = false;
            }
            else
            {
                base.Save<TAggregateRoot>(aggregateRoot);
            }

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
            get { return context.DistributedTransactionSupported; }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            this.context.Rollback();
        }
        #endregion

    }
}
