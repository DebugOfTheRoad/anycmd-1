﻿
namespace Anycmd.Ef
{
    using Model;
    using Repositories;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class EfRepositoryContext : RepositoryContext, IEfRepositoryContext
    {
        private readonly string efDbContextName;
        private DbContext _efContext;
        private readonly object sync = new object();
        private readonly AppHost host;

        public string EfDbContextName {
            get { return efDbContextName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public EfRepositoryContext(string efDbContextName)
        {
            CheckConfig(efDbContextName);
            this.host = AppHost.Instance;
            this.efDbContextName = efDbContextName;
        }

        public EfRepositoryContext(AppHost host, string efDbContextName)
        {
            CheckConfig(efDbContextName);
            this.host = host;
            this.efDbContextName = efDbContextName;
        }

        private void CheckConfig(string efDbContextName)
        {
            var connSetting = ConfigurationManager.ConnectionStrings[efDbContextName];
            if (connSetting == null || string.IsNullOrEmpty(connSetting.ConnectionString))
            {
                throw new Exceptions.CoreException("未配置name为" + efDbContextName + "的connectionStrings子节点");
            }
        }

        public DbContext DbContext
        {
            get
            {
                if (_efContext == null)
                {
                    _efContext = EfContext.CreateDbContext(host, efDbContextName);
                }
                return _efContext;
            }
        }

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
                // The dispose method will no longer be responsible for the commit
                // handling. Since the object container might handle the lifetime
                // of the repository context on the WCF per-request basis, users should
                // handle the commit logic by themselves.
                //if (!committed)
                //{
                //    Commit();
                //}
                if (_efContext != null)
                {
                    _efContext.Dispose();
                    _efContext = null;
                }
                base.Dispose(disposing);
            }
        }
        #endregion

        #region IRepositoryContext Members
        /// <summary>
        /// Registers a new object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public override void RegisterNew(object obj)
        {
            if (obj is IEntity)
            {
                if (((IEntity)obj).Id == Guid.Empty)
                {
                    ((IEntity)obj).Id = Guid.NewGuid();
                }
            }
            if ((obj is IEntityBase))
            {
                var entity = (obj as IEntityBase);
                if (entity.CreateUserID == null && host.User.Principal.Identity.IsAuthenticated)
                {
                    var user = host.User;
                    if (string.IsNullOrEmpty(entity.CreateBy))
                    {
                        entity.CreateBy = user.Worker.Name;
                    }                    
                    if (!entity.CreateUserID.HasValue)
                    {
                        entity.CreateUserID = user.Worker.Id;
                    }
                    if (!entity.CreateOn.HasValue)
                    {
                        entity.CreateOn = DateTime.Now;
                    }
                }
            }
            this.DbContext.Entry(obj).State = System.Data.Entity.EntityState.Added;
            Committed = false;
        }
        /// <summary>
        /// Registers a modified object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public override void RegisterModified(object obj)
        {
            var state = this.DbContext.Entry(obj).State;
            if ((obj is IEntityBase) && state == EntityState.Modified)
            {
                var entity = (obj as IEntityBase);
                if (entity.ModifiedUserID == null && host.User.Principal.Identity.IsAuthenticated)
                {
                    var user = host.User;
                    if (string.IsNullOrEmpty(entity.ModifiedBy))
                    {
                        entity.ModifiedBy = user.Worker.Name;
                    }
                    if (!entity.ModifiedUserID.HasValue)
                    {
                        entity.ModifiedUserID = user.Worker.Id;
                    }
                    if (!entity.ModifiedOn.HasValue)
                    {
                        entity.ModifiedOn = DateTime.Now;
                    }
                }
            }
            Committed = false;
        }
        /// <summary>
        /// Registers a deleted object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public override void RegisterDeleted(object obj)
        {
            this.DbContext.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
            Committed = false;
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
            get { return true; }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void Commit()
        {
            if (!Committed)
            {
                lock (sync)
                {
                    DbContext.SaveChanges();
                }
                Committed = true;
            }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            Committed = false;
        }

        public override IQueryable<TEntity> Query<TEntity>()
        {
            return DbContext.Set<TEntity>();
        }
        #endregion
    }
}
