using System.Linq;

namespace Anycmd.Repositories
{
    using Model;

    /// <summary>
    /// Represents that the implemented classes are repositories.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    public interface IRepository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
    {
        /// <summary>
        /// Gets the instance of the repository context on which the repository was attached.
        /// </summary>
        IRepositoryContext Context { get; }
        /// <summary>
        /// Finds all the aggregate roots from repository.
        /// </summary>
        /// <returns>The aggregate roots.</returns>
        IQueryable<TAggregateRoot> FindAll();
        /// <summary>
        /// Gets the aggregate root instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the aggregate root.</param>
        /// <returns>The instance of the aggregate root.</returns>
        TAggregateRoot GetByKey(object key);
        /// <summary>
        /// Adds an aggregate root to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be added to the repository.</param>
        void Add(TAggregateRoot aggregateRoot);
        /// <summary>
        /// Removes the aggregate root from current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be removed.</param>
        void Remove(TAggregateRoot aggregateRoot);
        /// <summary>
        /// Updates the aggregate root in the current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be updated.</param>
        void Update(TAggregateRoot aggregateRoot);
    }
}
