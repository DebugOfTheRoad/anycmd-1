
namespace Anycmd.Events.Storage
{
    using Anycmd.Storage;
    using Model;
    using Specifications;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the base class for domain event storages which are built based on the
    /// relational database systems.
    /// </summary>
    /// <typeparam name="TRdbmsStorage">The type of the <c>RdbmsStorage</c> which provides
    /// the required operations on the external storage mechanism.</typeparam>
    public abstract class RdbmsDomainEventStorage<TRdbmsStorage> : DisposableObject, IDomainEventStorage
        where TRdbmsStorage : RdbmsStorage
    {
        #region Private Fields
        private TRdbmsStorage storage;
        private readonly string connectionString;
        private readonly IStorageMappingResolver mappingResolver;
        private readonly IAppHost host;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <c>RdbmsDomainEventStorage&lt;TRdbmsStorage&gt;</c> class.
        /// </summary>
        /// <param name="connectionString">The connection string which is used when connecting
        /// to the relational database system. For more information about the connection strings
        /// for different database providers, please refer to http://www.connectionstrings.com.
        /// </param>
        /// <param name="mappingResolver">The instance of the mapping resolver which resolves the table and column mappings
        /// between data objects and the relational database system.</param>
        public RdbmsDomainEventStorage(IAppHost host, string connectionString, IStorageMappingResolver mappingResolver)
        {
            try
            {
                this.host = host;
                this.connectionString = connectionString;
                this.mappingResolver = mappingResolver;
                Type storageType = typeof(TRdbmsStorage);
                storage = (TRdbmsStorage)Activator.CreateInstance(storageType, new object[] { connectionString, mappingResolver });
            }
            catch
            {
                GC.SuppressFinalize(this);
                throw;
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the connection string which is used when connecting
        /// to the relational database system. For more information about the connection strings
        /// for different database providers, please refer to http://www.connectionstrings.com.
        /// </summary>
        public string ConnectionString
        {
            get { return this.connectionString; }
        }
        /// <summary>
        /// Gets the instance of the mapping resolver which resolves the table and column mappings
        /// between data objects and relational database system.
        /// </summary>
        public IStorageMappingResolver MappingResolver
        {
            get { return this.mappingResolver; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.Committed)
                this.Commit();
            storage.Dispose();
        }
        #endregion

        #region IDomainEventStorage Members
        /// <summary>
        /// Saves the specified domain event to the event storage.
        /// </summary>
        /// <param name="domainEvent">The domain event to be saved.</param>
        public void SaveEvent(IDomainEvent domainEvent)
        {
            try
            {
                DomainEventDataObject dataObject = host.FromDomainEvent(domainEvent);
                storage.Insert<DomainEventDataObject>(new PropertyBag(dataObject));
            }
            catch { throw; }
        }
        /// <summary>
        /// Loads all the domain events for the specific aggregate root from the storage.
        /// </summary>
        /// <param name="aggregateRootType">The type of the aggregate root.</param>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <returns>A list of domain events for the specific aggregate root.</returns>
        public IEnumerable<IDomainEvent> LoadEvents(Type aggregateRootType, Guid id)
        {
            try
            {
                PropertyBag sort = new PropertyBag();
                sort.AddSort<long>("Version");
                var aggregateRootTypeName = aggregateRootType.AssemblyQualifiedName;
                ISpecification<DomainEventDataObject> specification = Specification<DomainEventDataObject>.Eval(p => p.SourceID == id && p.AssemblyQualifiedSourceType == aggregateRootTypeName);
                return storage.Select<DomainEventDataObject>(specification, sort, Anycmd.Storage.SortOrder.Ascending).Select(p => host.ToDomainEvent(p));
            }
            catch { throw; }
        }
        /// <summary>
        /// Loads all the domain events for the specific aggregate root from the storage.
        /// </summary>
        /// <param name="aggregateRootType">The type of the aggregate root.</param>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <param name="version">The version number.</param>
        /// <returns>A list of domain events for the specific aggregate root which occur just after
        /// the given version number.</returns>
        public IEnumerable<IDomainEvent> LoadEvents(Type aggregateRootType, Guid id, long version)
        {
            PropertyBag sort = new PropertyBag();
            sort.AddSort<long>("Version");
            var aggregateRootTypeName = aggregateRootType.AssemblyQualifiedName;
            ISpecification<DomainEventDataObject> specification = Specification<DomainEventDataObject>
                .Eval(p => p.SourceID == id && p.AssemblyQualifiedSourceType == aggregateRootTypeName && p.Version > version);
            return storage.Select<DomainEventDataObject>(specification, sort, Anycmd.Storage.SortOrder.Ascending).Select(p => host.ToDomainEvent(p));
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
            get
            {
                return storage.DistributedTransactionSupported;
            }
        }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work was successfully committed.
        /// </summary>
        public bool Committed
        {
            get { return this.storage.Committed; }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public virtual void Commit()
        {
            storage.Commit();
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public virtual void Rollback()
        {
            storage.Rollback();
        }

        #endregion
    }
}
