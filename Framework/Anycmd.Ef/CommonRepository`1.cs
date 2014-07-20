
namespace Anycmd.Ef
{
    using Model;
    using Repositories;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class CommonRepository<TAggregateRoot> : Repository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
    {
        private readonly string efDbContextName;
        private readonly IAppHost host;

        public CommonRepository(IAppHost host, string efDbContextName)
        {
            this.host = host;
            this.efDbContextName = efDbContextName;
        }

        private EfRepositoryContext EFContext
        {
            get
            {
                var repositoryContext = EfContext.Storage.GetRepositoryContext(this.efDbContextName);
                if (repositoryContext == null)
                {
                    repositoryContext = new EfRepositoryContext(host, this.efDbContextName);
                    EfContext.Storage.SetRepositoryContext(repositoryContext);
                }
                return repositoryContext;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IRepositoryContext Context
        {
            get
            {
                return EFContext;
            }
        }

        public DbContext DbContext
        {
            get
            {
                return EFContext.DbContext;
            }
        }

        #region Protected Methods
        protected override IQueryable<TAggregateRoot> DoFindAll()
        {
            return DbContext.Set<TAggregateRoot>();
        }
        /// <summary>
        /// Gets the aggregate root instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the aggregate root.</param>
        /// <returns>The instance of the aggregate root.</returns>
        protected override TAggregateRoot DoGetByKey(object key)
        {
            return DbContext.Set<TAggregateRoot>().Where(p => p.Id == (Guid)key).FirstOrDefault();
        }
        /// <summary>
        /// Adds an aggregate root to the repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be added to the repository.</param>
        protected override void DoAdd(TAggregateRoot aggregateRoot)
        {
            EFContext.RegisterNew(aggregateRoot);
        }
        /// <summary>
        /// Updates the aggregate root in the current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be updated.</param>
        protected override void DoUpdate(TAggregateRoot aggregateRoot)
        {
            EFContext.RegisterModified(aggregateRoot);
        }
        /// <summary>
        /// Removes the aggregate root from current repository.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root to be removed.</param>
        protected override void DoRemove(TAggregateRoot aggregateRoot)
        {
            // TODO:区分标记删除
            EFContext.RegisterDeleted(aggregateRoot);
        }
        #endregion
    }
}
