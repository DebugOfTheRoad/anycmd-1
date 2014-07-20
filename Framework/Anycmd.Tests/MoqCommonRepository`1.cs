
namespace Anycmd.Tests
{
    using Model;
    using Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class MoqCommonRepository<TAggregateRoot> : IRepository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
    {
        private MoqRepositoryContext context;
        private readonly IAppHost host;

        public MoqCommonRepository(IAppHost host)
        {
            this.host = host;
            context = new MoqRepositoryContext(host);
        }

        public IRepositoryContext Context
        {
            get { return context; }
        }

        public IQueryable<TAggregateRoot> FindAll()
        {
            return Context.Query<TAggregateRoot>();
        }

        public TAggregateRoot GetByKey(object key)
        {
            return Context.Query<TAggregateRoot>().FirstOrDefault(a => a.Id == (Guid)key);
        }

        public void Add(TAggregateRoot aggregateRoot)
        {
            Context.RegisterNew(aggregateRoot);
        }

        public void Remove(TAggregateRoot aggregateRoot)
        {
            Context.RegisterDeleted(aggregateRoot);
        }

        public void Update(TAggregateRoot aggregateRoot)
        {
            Context.RegisterModified(aggregateRoot);
        }
    }
    public class MoqRepositoryContext : RepositoryContext, IRepositoryContext
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly object sync = new object();

        private static readonly ThreadLocal<Dictionary<IAppHost, Dictionary<Type, List<IAggregateRoot>>>> 
            data = new ThreadLocal<Dictionary<IAppHost, Dictionary<Type, List<IAggregateRoot>>>>(() => new Dictionary<IAppHost, Dictionary<Type, List<IAggregateRoot>>>());
        private readonly IAppHost host;

        public MoqRepositoryContext(IAppHost host)
        {
            this.host = host;
            if (!data.Value.ContainsKey(host))
            {
                data.Value.Add(host, new Dictionary<Type, List<IAggregateRoot>>());
            }
        }

        public override void Commit()
        {
            lock (sync)
            {
                foreach (var item in base.NewCollection)
                {
                    if (!data.Value[host].ContainsKey(item.GetType()))
                    {
                        data.Value[host].Add(item.GetType(), new List<IAggregateRoot>());
                    }
                    if (data.Value[host][item.GetType()].Any(a => a.Id == ((IAggregateRoot)item).Id))
                    {
                        throw new Exception();
                    }
                    data.Value[host][item.GetType()].Add((IAggregateRoot)item);
                }
                foreach (var item in base.ModifiedCollection)
                {
                    data.Value[host][item.GetType()].Remove(data.Value[host][item.GetType()].First(a => a.Id == ((IAggregateRoot)item).Id));
                    data.Value[host][item.GetType()].Add((IAggregateRoot)item);
                }
                foreach (var item in DeletedCollection)
                {
                    data.Value[host][item.GetType()].Remove(data.Value[host][item.GetType()].First(a => a.Id == ((IAggregateRoot)item).Id));
                }
                base.Committed = true;
                base.ClearRegistrations();
            }
        }

        public override void Rollback()
        {
            base.ClearRegistrations();
            base.Committed = false;
        }

        public override IQueryable<TEntity> Query<TEntity>()
        {
            if (!data.Value[host].ContainsKey(typeof(TEntity)))
            {
                return new List<TEntity>().AsQueryable();
            }
            return data.Value[host][typeof(TEntity)].Cast<TEntity>().AsQueryable<TEntity>();
        }
    }
}
