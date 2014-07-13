
namespace Anycmd.Ef
{
    using Model;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SimpleEfContextStorage : DisposableObject, IEfContextStorage
    {
        private readonly ConcurrentDictionary<string, EfRepositoryContext> repositoryContexts = new ConcurrentDictionary<string, EfRepositoryContext>(StringComparer.OrdinalIgnoreCase);

        public void SetRepositoryContext(EfRepositoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.repositoryContexts[context.EfDbContextName] = context;
        }

        public EfRepositoryContext GetRepositoryContext(string key)
        {
            EfRepositoryContext cxt;
            if (!this.repositoryContexts.TryGetValue(key, out cxt))
            {
                return null;
            }

            return cxt;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var item in repositoryContexts.Values)
                {
                    item.Dispose();
                }
            }
        }
    }
}