using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Anycmd.Repositories
{
    using Model;

    /// <summary>
    /// Represents the repository context.
    /// </summary>
    public abstract class RepositoryContext : DisposableObject, IRepositoryContext
    {
        #region Private Fields
        private readonly Guid id = Guid.NewGuid();
        private ThreadLocal<List<object>> _localNewCollection = new ThreadLocal<List<object>>(() => new List<object>());
        private ThreadLocal<List<object>> _localModifiedCollection = new ThreadLocal<List<object>>(() => new List<object>());
        private ThreadLocal<List<object>> _localDeletedCollection = new ThreadLocal<List<object>>(() => new List<object>());
        private ThreadLocal<bool> _localCommitted = new ThreadLocal<bool>(() => true);
        #endregion

        private ThreadLocal<List<object>> localNewCollection
        {
            get
            {
                if (_localNewCollection == null)
                {
                    _localNewCollection = new ThreadLocal<List<object>>(() => new List<object>());
                }
                return _localNewCollection;
            }
        }

        private ThreadLocal<List<object>> localModifiedCollection
        {
            get
            {
                if (_localModifiedCollection == null)
                {
                    _localModifiedCollection = new ThreadLocal<List<object>>(() => new List<object>());
                }
                return _localModifiedCollection;
            }
        }

        private ThreadLocal<List<object>> localDeletedCollection
        {
            get
            {
                if (_localDeletedCollection == null)
                {
                    _localDeletedCollection = new ThreadLocal<List<object>>(() => new List<object>());
                }
                return _localDeletedCollection;
            }
        }

        private ThreadLocal<bool> localCommitted
        {
            get
            {
                if (_localCommitted == null)
                {
                    _localCommitted = new ThreadLocal<bool>(() => true);
                }
                return _localCommitted;
            }
        }

        #region Protected Properties
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be added to the repository.
        /// </summary>
        protected IEnumerable<object> NewCollection
        {
            get { return localNewCollection.Value; }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be modified in the repository.
        /// </summary>
        protected IEnumerable<object> ModifiedCollection
        {
            get { return localModifiedCollection.Value; }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be deleted from the repository.
        /// </summary>
        protected IEnumerable<object> DeletedCollection
        {
            get { return localDeletedCollection.Value; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Clears all the registration in the repository context.
        /// </summary>
        /// <remarks>Note that this can only be called after the repository context has successfully committed.</remarks>
        protected void ClearRegistrations()
        {
            this.localNewCollection.Value.Clear();
            this.localModifiedCollection.Value.Clear();
            this.localDeletedCollection.Value.Clear();
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
                if (_localCommitted != null)
                {
                    this._localCommitted.Dispose();
                    _localCommitted = null;
                }
                if (_localDeletedCollection != null)
                {
                    this._localDeletedCollection.Dispose();
                    _localDeletedCollection = null;
                }
                if (_localModifiedCollection != null)
                {
                    this._localModifiedCollection.Dispose();
                    _localModifiedCollection = null;
                }
                if (_localNewCollection != null)
                {
                    this._localNewCollection.Dispose();
                    _localNewCollection = null;
                }
            }
        }
        #endregion

        #region IRepositoryContext Members
        /// <summary>
        /// Gets the ID of the repository context.
        /// </summary>
        public Guid Id
        {
            get { return id; }
        }
        /// <summary>
        /// Registers a new object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterNew(object obj)
        {
            //if (localModifiedCollection.Value.Contains(obj))
            //   throw new InvalidOperationException("The object cannot be registered as a new object since it was marked as modified.");
            //if (localNewCollection.Value.Contains(obj))
            //    throw new InvalidOperationException("The object has already been registered as a new object.");
            localNewCollection.Value.Add(obj);
            Committed = false;
        }
        /// <summary>
        /// Registers a modified object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterModified(object obj)
        {
            if (localDeletedCollection.Value.Contains(obj))
                throw new InvalidOperationException("The object cannot be registered as a modified object since it was marked as deleted.");
            if (!localModifiedCollection.Value.Contains(obj) && !localNewCollection.Value.Contains(obj))
                localModifiedCollection.Value.Add(obj);
            Committed = false;
        }
        /// <summary>
        /// Registers a deleted object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterDeleted(object obj)
        {
            if (localNewCollection.Value.Contains(obj))
            {
                if (localNewCollection.Value.Remove(obj))
                    return;
            }
            bool removedFromModified = localModifiedCollection.Value.Remove(obj);
            bool addedToDeleted = false;
            if (!localDeletedCollection.Value.Contains(obj))
            {
                localDeletedCollection.Value.Add(obj);
                addedToDeleted = true;
            }
            localCommitted.Value = !(removedFromModified || addedToDeleted);
        }
        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public virtual bool DistributedTransactionSupported
        {
            get { return false; }
        }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work was successfully committed.
        /// </summary>
        public virtual bool Committed
        {
            get { return localCommitted.Value; }
            protected set { localCommitted.Value = value; }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public abstract void Commit();
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public abstract void Rollback();

        public abstract IQueryable<TEntity> Query<TEntity>() where TEntity : class;
        #endregion
    }
}
