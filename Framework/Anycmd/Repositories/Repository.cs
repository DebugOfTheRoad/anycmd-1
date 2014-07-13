using System.Linq;

namespace Anycmd.Repositories
{
    using Model;

    /// <summary>
    /// Represents the base class for repositories.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    public abstract class Repository<TAggregateRoot> : IRepository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
    {
        #region Private Fields
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>Repository&lt;TAggregateRoot&gt;</c> class.
        /// </summary>
        public Repository()
        {
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Adds an aggregate root to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be added to the repository.</param>
        protected abstract void DoAdd(TAggregateRoot aggregateRoot);
        /// <summary>
        /// Gets the aggregate root instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the aggregate root.</param>
        /// <returns>The instance of the aggregate root.</returns>
        protected abstract TAggregateRoot DoGetByKey(object key);
        /// <summary>
        /// Removes the aggregate root from current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be removed.</param>
        protected abstract void DoRemove(TAggregateRoot aggregateRoot);
        /// <summary>
        /// Updates the aggregate root in the current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be updated.</param>
        protected abstract void DoUpdate(TAggregateRoot aggregateRoot);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract IQueryable<TAggregateRoot> DoFindAll();
        #endregion

        #region IRepository<TAggregateRoot> Members
        /// <summary>
        /// Gets the <see cref="Anycmd.Repositories.IRepositoryContext"/> instance.
        /// </summary>
        public abstract IRepositoryContext Context { get; }
        /// <summary>
        /// Finds all the aggregate roots from repository.
        /// </summary>
        /// <returns>The aggregate roots.</returns>
        public IQueryable<TAggregateRoot> FindAll()
        {
            return this.DoFindAll();
        }
        /// <summary>
        /// Gets the aggregate root instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the aggregate root.</param>
        /// <returns>The instance of the aggregate root.</returns>
        public TAggregateRoot GetByKey(object key)
        {
            return this.DoGetByKey(key);
        }
        /// <summary>
        /// Adds an aggregate root to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be added to the repository.</param>
        public void Add(TAggregateRoot aggregateRoot)
        {
            this.DoAdd(aggregateRoot);
        }
        /// <summary>
        /// Updates the aggregate root in the current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be updated.</param>
        public void Update(TAggregateRoot aggregateRoot)
        {
            this.DoUpdate(aggregateRoot);
        }
        /// <summary>
        /// Removes the aggregate root from current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be removed.</param>
        public void Remove(TAggregateRoot aggregateRoot)
        {
            this.DoRemove(aggregateRoot);
        }
        #endregion
    }
}
